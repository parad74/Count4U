using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;
using System.IO;
using System.IO.Ports;
using NLog;
using System.Threading.Tasks;

namespace Count4U.Model.Lib.MultiPoint
{
	
	//public enum MPMemory { RAM = 0x00, FLASH = 0x02, ROM = 0x03, NONE = 0x01 };
	//public enum MPStage { Bell = 0x07, Upload = 0x4C, Download = 0x55 };
	//public enum MPError { NoAddress = -0x80, NAK = -0x15, FailStart = -0x10, FailBlock = -0x11, FileStream = -0x12, Abort = -0x13 };
	//public delegate void MPState(Multi from, MPStage stage, int state, string text, int current);

	public class Multi : IMulti 
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private int[] _baud_rates = { 57600, 38400, 19200, 9600, 4800 };
        private bool _hard_reset;
		private string _port;
		private byte _address;
		private string _terminalID;
		private int _baudrate;
		private SerialPort _serial;
		private Thread _threadUpDown;
		private Thread _threadWakeUP;
		private Object ChangeSerialLock = new Object();
		private Object ThreadUpDownLock = new Object();
		private Object ThreadWakeUPLock = new Object();

		/// <summary>
		/// 
		/// </summary>
		public Multi(string port, int baudrate)
		{
			this._threadWakeUP = null;
			this._terminalID = "not started";
			this._port = port;
			this._baudrate = baudrate;
			this._serial = null;
			this._address = 0x80;
			this._threadUpDown = null;
			this._hard_reset = false;
		}

		public Multi(string port, int baudrate, string address)
		{
			this._threadWakeUP = null;
			this._terminalID = "not started";
			this._port = port;
			this._baudrate = baudrate;
			this._serial = null;
			this._hard_reset = false;

			this._address = Convert.ToByte(address[0]);
			this._address += 0x80;
		}

		private byte GetAddress()
		{
			return (byte)(this._address - 0x80);
		}

		private string GetCOMPort()
		{
			return this._port;
		}

		public string Port
		{
			get { return this._port; }
		}

        public int[] Baud_rates
        {
            get { return _baud_rates; }
            set { _baud_rates = value; }
        }
		public string TerminalID
		{
			get { return this._terminalID; }
			set { this._terminalID = value; }
		}


		public int Baudrate
		{
			get { return _baudrate; }
			set { _baudrate = value; }
		}

		public byte Address
		{
			get { return (byte)(this._address - 0x80); }
		}

		public event MPState EventMPState;

		/// <summary>
		/// 
		/// </summary>
		private void CalculateCheckSum(ref byte[] buffer, int offset)
		{
			try //Marina
			{
				byte cs = 0;
				int i = 0;

				for (i = 1; i <= offset; i++)
					cs += buffer[i];

				cs += _address;
				cs += (byte)offset;

				offset++; // skip STX
				buffer[offset++] = (byte)(((cs & 0xF0) >> 4) + 0x40);
				buffer[offset++] = (byte)((cs & 0x0F) + 0x40);
			}
			catch (Exception ex) //Marina
			{
				_logger.Error("Multi : CalculateCheckSum()", ex);
			}
		}

		private bool CheckSum(ref byte[] buffer, int length)
		{
			byte cs, cs1, cs2;
			int i = 0;

			cs = cs1 = cs2 = 0;
			for (i = 1; i <= length - 4; i++)
				cs += buffer[i];

			cs += (byte)(length - 4);

			cs1 = (byte)(((cs & 0xF0) >> 4) + 0x40);
			cs2 = (byte)((cs & 0x0F) + 0x40);

			if (buffer[length - 3] == cs1 && buffer[length - 2] == cs2)
				return true;

			return false;
		}

		private void SendCommandBELL()
		{
			//string address_list = "ABCDEFGHIJKLMNOPQRSTUVWXY0123456";
			byte[] address_list = { 0x41, 0x42, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E, 0x4F,
															0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A,
															0x31, 0x32, 0x33, 0x34, 0x35, 0x36 };

			byte[] buffer = { 0x07, 0 };
			byte[] data = new byte[100];
			//string str;
			int j, i, count, index_baudrate, this_baudrate;

			this_baudrate = _baudrate;
			index_baudrate = 0;

			//lock (ChangeSerialLock) //был объект
			//{
				for (j = 0; j < address_list.Length; j++)
				{
					if (_address != 0x80) buffer[1] = _address;
					else buffer[1] = (byte)(0x80 + address_list[j]);

					for (i = 1; i <= 5; i++)
					{
						if (Thread.CurrentThread.Name == "Abort")
						{ _baudrate = this_baudrate; return; }

						try
						{
							_serial.Write(buffer, 0, 2);
							if (j == 0)
								Thread.Sleep(i * 300);
						}
						catch { continue; }

						try
						{
							count = _serial.Read(data, 0, 10);//serial.BytesToRead);
							if (count == 5)
								if (data[0] == 0x02 && data[1] == 0x07 && data[4] == 0x03)
								{
									_address = buffer[1];
									break;
								}
							i--; // again. usually NAK received
						}
						//catch(InvalidOperationException ioExp)
						//{
						//	//throw new SystemException("Port is Closed", ioExp);
						//}	
						catch //(TimeoutException)
						{
                            try// Marina 
                            {
                               if(_serial.BytesToRead > 0) i--; // again
                                else
                                    if (j > 0) break; // only for first letter 3 trying
                                    else continue;
                            }
							catch (Exception ex) // Marina 
                            {
								_logger.Error("SendCommandBELL Exception (_serial.BytesToRead)" + ex.Message);
                               // return;
                            }
						}
					}
					if (_address != 0x80)
						break;

					if (index_baudrate == _baud_rates.Length - 1)
					{ index_baudrate = 0; continue; }
					 
					int before_index_baudrate = index_baudrate;
					//while (index_baudrate < _baud_rates.Length && _baud_rates[index_baudrate++] == this_baudrate) ; //Замена на строчку ниже Marina
					for (int kk = 0; kk < _baud_rates.Length; kk++)
					{
						if (_baud_rates[kk] == this_baudrate)
						{
							index_baudrate = kk + 1;
						}
					}


						//index_baudrate++;

						if (index_baudrate == _baud_rates.Length)
						{ index_baudrate = 0; continue; }

					//if (_serial != null) // Marina 
					//{
					//	lock (_serial) // Marina 
					//	{
							Close();
							if (index_baudrate != before_index_baudrate)
							{
								_baudrate = _baud_rates[index_baudrate];
								string info = "SendCommandBELL () j=" + j + "_baudrate=" + _baudrate + "this_baudrate =" + this_baudrate + "before_index_baudrate =" + before_index_baudrate + "index_baudrate =" + index_baudrate;
								if (Start(info) == true)
								{
									_logger.Info("Start(if Start()) == true");
									j--; // hold on current Adrress step
								}
							}
             
					//	}
					//}
				}

				if (_baudrate != this_baudrate)
				{
					//lock (_serial)
					//{
					_logger.Info("SetConfiguration()  _baudrate = " + _baudrate + " this_baudrate = " + this_baudrate  + "port = " + Port);
						SetConfiguration(this_baudrate, _address, _serial.DataBits, _serial.StopBits, _serial.Parity, _serial.Handshake);
					//}
				}
				EventMPState(this, MPStage.Bell, _address, "", 0);
			//} //lock(ChangeSerialLock)
		
		}

		private bool SetConfiguration(int new_baudrate, int new_address, int data_bits, StopBits stop_bits, Parity parity, Handshake handshake)
		{
			try       // Marina
			{
				byte[] data = null;
				string request;

				Hashtable htBaudRate = new Hashtable();
				htBaudRate.Add(110, "0");
				htBaudRate.Add(150, "1");
				htBaudRate.Add(300, "2");
				htBaudRate.Add(600, "3");
				htBaudRate.Add(1200, "4");
				htBaudRate.Add(2400, "5");
				htBaudRate.Add(4800, "6");
				htBaudRate.Add(9600, "7");
				htBaudRate.Add(19200, "8");
				htBaudRate.Add(38400, "9");
				htBaudRate.Add(57600, "A");

				Hashtable htDataBits = new Hashtable();
				htDataBits.Add(7, "7");
				htDataBits.Add(8, "8");

				Hashtable htStopBits = new Hashtable();
				htStopBits.Add(StopBits.One, "1");
				htStopBits.Add(StopBits.Two, "2");

				Hashtable htParity = new Hashtable();
				htParity.Add(Parity.None, "N");
				htParity.Add(Parity.Odd, "O");
				htParity.Add(Parity.Even, "P");

				Hashtable htControl = new Hashtable();
				htControl.Add(Handshake.XOnXOff, "X");
				htControl.Add(Handshake.RequestToSend, "C");
				htControl.Add(Handshake.None, "F");

				request = htBaudRate[new_baudrate].ToString() +
					htStopBits[stop_bits].ToString() +
					htDataBits[data_bits].ToString() +
					htParity[parity].ToString() +
					htControl[handshake].ToString() +
					"M" +
					Convert.ToChar(new_address - 0x80) +
					" " + " " + Convert.ToChar(100);

				if (!TransmitCommand(0x43, request, ref data)) // C
					return false;
				if (data == null)
					return false;

				bool result = data[0] == 0 ? true : false;

				//if (_serial != null)
				//{
					//lock (_serial)
					//{
						if (result)
						{
							Close();
							_address = (byte)new_address;
							_baudrate = new_baudrate;
							Start("SetConfiguration");
						}
					//}
				//}

				return result;
			}
			catch (Exception ex) //Marina
			{
				_logger.Error("Multi : SetConfiguration()", ex);
				return false;
			}
		}

		private bool TransmitCommand(byte Command, ref byte[] request, int request_length, ref byte[] data)
		{
			byte[] out_data = new byte[300];
			byte[] in_data = new byte[300];
			int out_count, in_count, out_retry, in_retry;

			if (_address == 0x80)
				return false;

			out_count = 0;
			switch (Command)
			{
				case 0x06: // ACK
					out_data[out_count++] = Command;
					break;
				case 0x15: // NAK
					out_data[out_count++] = Command;
					break;
				case 0x07: // BELL
					out_data[out_count++] = Command;
					out_data[out_count++] = _address;
					break;

				default:
					out_data[out_count++] = 0x02;
					out_data[out_count++] = 0x1B;
					out_data[out_count++] = Command;

					if (request_length > 0)
					{
						request.CopyTo(out_data, out_count);
						out_count += request_length;
					}
					CalculateCheckSum(ref out_data, out_count - 1);
					out_count++;
					out_count++;
					out_data[out_count++] = _address;
					break;
			}
			out_retry = in_retry = 0;

		again:
			//lock (ChangeSerialLock) // был объект
			//{
				try { _serial.Write(out_data, 0, out_count); }
				catch
				{
					//try
					//{
						SendCommandBELL();
					//}
					//catch { } //Marina
					try { _serial.Write(out_data, 0, out_count); }
					catch { return false; }
				}

				if (Command == 0x06 || Command == 0x15) // ACK or NAK
					return true;

				// Receive response
				Thread.Sleep(10);

				in_count = 0;
				try
				{
					in_data[0] = (byte)_serial.ReadByte();
					if (in_data[0] == 0x06) // ACK
						return true;
					if (in_data[0] == 0x15) // NAK
					{
						if (out_retry == 0)
						{
							out_retry++;
							goto again;
						}
						return false;
					}
					in_count++;

					while (in_data[in_count - 1] != 0x03) // ETX
					{
						in_count += _serial.Read(in_data, in_count, 256 - in_count);
						Thread.Sleep(5);
					}
				}
				catch (TimeoutException)
				{
					if (in_count == 0 && in_retry == 0)
					{
						//try
						//{
							SendCommandBELL();
						//}
						//catch { } // Marina
						in_retry++;
						goto again;
					}
					//MessageBox.Show("TineOut "+serial.BytesToRead.ToString());
					return false;
				}
				catch { return false; }

				if (in_count == 0)
					return false;

				if (in_data[0] == 0x02 && in_count > 5)
				{
					data = new byte[in_count - 6];
					Array.Copy(in_data, 3, data, 0, in_count - 6);
					return CheckSum(ref in_data, in_count);
				}

				return false;
			//} //lock
		}


		private bool TransmitCommand(byte Command, string request, ref byte[] data)
		{
			byte[] buffer = Encoding.ASCII.GetBytes(request);

			return TransmitCommand(Command, ref buffer, buffer.Length, ref data);
		}

		private bool TransmitCommand(byte Command, ref byte[] data)
		{
			byte[] buffer = null;
			return TransmitCommand(Command, ref buffer, 0, ref data);
		}

		private bool TransmitCommand(byte Command)
		{
			byte[] buffer = null;
			return TransmitCommand(Command, ref buffer, 0, ref buffer);
		}

		/// <summary>
		/// 
		/// </summary>
		private bool SetAddress(string request)
		{
			try
			{  // Marina
				byte[] data = null;

				if (!TransmitCommand(0x35, ref data)) // 5
					return false;
				if (data == null)
					return false;

				if (data[0] != 0)
					return false;

				_address = Convert.ToByte(request[0]);
				_address += 0x80;

				return true;
			}
			catch (Exception ex) //Marina
			{
				_logger.Error("Multi : SetAddress() :" + request, ex);
				return false;
			}
		}

		public string GetTerminalID()
		{
			this._terminalID = "not started";
			if (this._address == 0x80) return this._terminalID;
							
			try
			{  // Marina
				if (SerialIsOpen() == false) {  return null; }

				byte[] data = null;

				if (!TransmitCommand(0x52, ref data)) // R
					return null;
				if (data == null)
					return null;

				this._terminalID = Encoding.ASCII.GetString(data);
				return this._terminalID;
			}
			catch (Exception ex) //Marina
			{
				_logger.Error("Multi : GetTerminalID() : " + ex.Message, ex);
				return null;
			}
		}

		public string GetTerminalVersion()
		{
			try
			{  // Marina
				byte[] data = null;

				if (!TransmitCommand(0x76, ref data)) // v
					return null;
				if (data == null)
					return null;

				return Encoding.ASCII.GetString(data);
			}
			catch (Exception ex) //Marina
			{
				_logger.Error("Multi : GetTerminalVersion() :", ex);
				return null;
			}
		}

		public bool SetDateTime(DateTime dt)
		{
			try
			{  // Marina
				byte[] data = null;
				string request = dt.ToString("yyyyMMddHHmmss");

				if (!TransmitCommand(0x4D, request, ref data)) // M
					return false;
				if (data == null)
					return false;

				return data[0] == 0 ? true : false;
			}
			catch (Exception ex) //Marina
			{
				_logger.Error("Multi : SetDateTime() :", ex);
				return false;
			}
		}

		public bool WarmStart()
		{
			return TransmitCommand(0x41); // A
		}

		private bool ColdStart()
		{
			bool result = TransmitCommand(0x48); // H

			_hard_reset = true;

			return result;
		}

		private int GetFileSize(string file)
		{
			try
			{  // Marina
				byte[] data = null;

				if (TransmitCommand(0x4A, file, ref data)) // J
					if (data != null)
						if (data.Length > 1)
							return Convert.ToInt32(Encoding.ASCII.GetString(data, 1, data.Length - 1));

				return -1;
			}
			catch (Exception ex) //Marina
			{
				_logger.Error("Multi : GetFileSize() :" + file, ex);
				return -1;
			}
		}

		public List<ItemDir> GetDirectory(bool with_size)
		{
			try
			{  // Marina
				byte[] data = null;
				string files = "";
				ItemDir tempFile;
				int index, count;
				bool part;
				List<ItemDir> list = new List<ItemDir>();

				while (true)
				{
					if (!TransmitCommand(0x44, ref data)) // D
						return null;
					if (data == null)
						return null;

					part = false;
					index = 0;
					count = data.Length;
					if (data.Length > 2)
					{
						if (data[0] == 0x5C && data[1] == 0x5C) // start \\
						{
							index = 2;
							count -= 2;
						}
						if (data[data.Length - 1] == 0x5C && data[data.Length - 2] == 0x5C) // end \\
						{
							count -= 2;
							part = true;
						}
					}
					files += Encoding.ASCII.GetString(data, index, count);

					if (part)
						files = files + ",";
					else
						break;
				}

				if (files.Length == 0)
					return list;

				foreach (string item in files.Split(','))
				{
					tempFile = new ItemDir();
					tempFile.name = item;
					tempFile.size = -1;
					//tempFile.memory = -1;
					//	tempFile.memory = data[0];
					if (with_size)
						tempFile.size = GetFileSize(item);

					list.Add(tempFile);
				}

				return list;
			}
			catch (Exception ex) //Marina
			{
				_logger.Error("Multi : GetDirectory() :", ex);
				return null;
			}
		}

		private bool EraseFile(string request)
		{
			try
			{  // Marina
				byte[] data = null;

				if (!TransmitCommand(0x45, request, ref data)) // E
					return false;
				if (data == null)
					return false;

				return data[0] == 0 ? true : false;
			}
			catch (Exception ex) //Marina
			{
				_logger.Error("Multi : EraseFile() :", ex);
				return false;
			}
		}

		//Удаление файлов на терминале
		public bool DeleteFiles(List<string> exclude)
		{
			try
			{  // Marina
				bool result = true;
				// files on PDA
				List<ItemDir> list = GetDirectory(false);

				foreach (ItemDir item in list)
				{		//exclude - don't delete files
					if (exclude.Contains(item.name) == false)
					{
						// если нет в списке exclude => удаляем
						result &= EraseFile(item.name);
					}
				}

				return result;
			}
			catch (Exception ex) //Marina
			{
				_logger.Error("Multi : DeleteFiles() :", ex);
				return false;
			}
		}

		//Удаление файлов на терминале
		public bool DeleteFiles(string[] exclude)
		{
			try
			{  // Marina
				List<string> list = new List<string>();

				foreach (string item in exclude)
					list.Add(item);

				return DeleteFiles(list);
			}
			catch (Exception ex) //Marina
			{
				_logger.Error("Multi : DeleteFiles() :", ex);
				return false;
			}
		}

		//Удаление файлов на терминале
		public bool DeleteFiles(List<ItemDir> exclude)
		{
			try
			{  // Marina
				List<string> list = new List<string>();

				foreach (ItemDir item in exclude)
					list.Add(item.name);

				return DeleteFiles(list);
			}
			catch (Exception ex) //Marina
			{
				_logger.Error("Multi : DeleteFiles() :", ex);
				return false;
			}
		}

		//Удаление файлов на терминале
		public bool DeleteFiles()
		{
			try
			{  // Marina
				return DeleteFiles(new List<string>());
			}
			catch (Exception ex) //Marina
			{
				_logger.Error("Multi : DeleteFiles() :", ex);
				return false;
			}
		}


		//Загрузка файлов c терминала на компьютер
		//private void DownloadFilesPrivate(List<string> list, string folder)
		private int DownloadFilesPrivate(List<string> list, string folder)
		{
			FileStream fs = null;
			byte[] block = new byte[260];
			byte[] data = null;
			int i, pos, size, file_size;
			bool finish, abort = false;

			try
			{
				foreach (string file in list)
				{
					file_size = GetFileSize(file);

					if (!TransmitCommand(0x55, file, ref data)) // U
					{ EventMPState(this, MPStage.Download, (int)MPError.FailStart, file, file_size); continue; }

					try { fs = File.Open(folder + "\\" + file, FileMode.Create); }
					catch
					{
						//if (fs != null) fs.Close();//Marina
						EventMPState(this, MPStage.Download, (int)MPError.FileStream, file, file_size);
						continue;
					}


					size = 0;
					finish = true;
					//try
					//{
					while (finish)
					{
						if (Thread.CurrentThread.Name != null)
						{
							EventMPState(this, MPStage.Download, (int)MPError.Abort, file, file_size);
							if (!TransmitCommand(0x79)) // y
								TransmitCommand(0x79); // retry 2
							abort = true;
							break;
						}

						data = null;
						if (!TransmitCommand(0x59, ref data)) // Y
						{ EventMPState(this, MPStage.Download, (int)MPError.FailBlock, file, file_size); continue; }

						if (data == null)
							finish = false;
						else
							if (data.Length == 0) // Z end of file
								finish = false;
							else
							//if (data[0] == 0x59) // Y next block
							{
								for (pos = i = 0; i < data.Length; i++)
									if (data[i] != 0x5C)
										block[pos++] = data[i];
									else
									{
										i++;
										if (data[i] == 0x5C) // \
											block[pos++] = data[i];
										else
											if (data[i] >= 0x80 && data[i] <= 0x9F)
												block[pos++] = (byte)(data[i] - 0x80);
											else
												if (data[i] >= 0x20 && data[i] <= 0x7F)
													block[pos++] = (byte)(data[i] + 0x80);
									}
								try { fs.Write(block, 0, pos); }
								catch { EventMPState(this, MPStage.Download, (int)MPError.FileStream, file, file_size); break; }

								size += pos;
								EventMPState(this, MPStage.Download, size, file, file_size);
							}

						if (!TransmitCommand(0x06)) // ACK
						{ EventMPState(this, MPStage.Download, (int)MPError.NAK, file, file_size); break; }

						Thread.Sleep(30);
					}
					//}
					try		{ fs.Close(); }
					catch { EventMPState(this, MPStage.Download, (int)MPError.FileStream, file, file_size); }

					if (abort)
					{
						//Marina

						// Marina end
						break;

					}
								
				}
				return 1;
			}
			catch (Exception ex) //Marina
			{
				_logger.Error("Multi : DownloadFilesPrivate() 2 :" + ex.Message , ex);
				return 0;
			}  //Marina
		}

		//Загрузка файлов c терминала на компьютер
		public bool DownloadFiles(List<string> list, string folder)
		{
			try
			{  // Marina
				string serialPortName = _port;
				int result = -1;
				_logger.Info("MultiPoint.DownloadFiles(List<string> list, string folder) -" + serialPortName );
				if (_threadUpDown != null)
					if (_threadUpDown.IsAlive == true)
						return false;

				_threadUpDown = new Thread(delegate() { result = this.DownloadFilesPrivate(list, folder); });
				lock (_threadUpDown)
				{
					_threadUpDown.Start();
					//thUpDown.IsBackground = true;
					
					
 				}
				return true;
			}
			catch (Exception ex) //Marina
			{
				_logger.Error("Multi : DownloadFiles() : serialPortName : " + ex.Message, ex);
				return false;
			}
			
		}

	//double getmul = 0;
	////3
	//Thread1 ClsMultiply = new Thread1();
	////4
	//Thread MulThread = new Thread(delegate()
	//	{
	//	getmul = ClsMultiply.Multiply(lOpr1, lOpr2);
	//	});
	////5
	//MulThread.Start();
	////6
	//while (MulThread.IsAlive)
	//	 Thread.Sleep(1);

		

		//public Task Delay(int milliseconds)		// Asynchronous NON-BLOCKING method
		//{
		//	var tcs = new TaskCompletionSource<object>();
		//	new Timer(_ => tcs.SetResult(null)).Change(milliseconds, -1);
		//	return tcs.Task;
		//}

		private void WaitForAsync(Action callback)		// Asynchronous NON-BLOCKING method
		{
			new Timer(_ => callback()).Change(100, -1);
		}

		private void AsyncCall()
		{
			int i = 1;
		}


		//Загрузка файлов c терминала на компьютер
		public bool DownloadFiles(List<ItemDir> list, string folder)
		{
			try
			{  // Marina
				string serialPortName = _port;
				int result = -1;
				_logger.Info("MultiPoint.DownloadFiles(List<ItemDir> list, string folder) - " + serialPortName);
				if (_threadUpDown != null)
					if (_threadUpDown.IsAlive == true)
						return false;

				List<string> newlist = new List<string>();

				foreach (ItemDir item in list)
					newlist.Add(item.name);

				_threadUpDown = new Thread(delegate() { result = this.DownloadFilesPrivate(newlist, folder); });
				lock (_threadUpDown)
				{
					_threadUpDown.Start();
					//thUpDown.IsBackground = true;
					
				}
				return true;
			}
			catch (Exception ex) //Marina
			{
				_logger.Error("Multi : DownloadFiles()  : serialPortName : " + ex.Message, ex);
				return false;
			}
		}

		//Загрузка файлов c терминала на компьютер
		public bool DownloadFiles(string[] list, string folder)
		{
			try
			{  // Marina
				string serialPortName = _port;
				int result = -1;
				_logger.Info("MultiPoint.DownloadFiles(string[] list, string folder) - " + serialPortName);
				if (_threadUpDown != null)
					if (_threadUpDown.IsAlive == true)
						return false;

				List<string> newlist = new List<string>();

				foreach (string item in list)
					newlist.Add(item);
				_threadUpDown = new Thread(delegate() { result = this.DownloadFilesPrivate(newlist, folder); });

				lock (_threadUpDown)
				{
					_threadUpDown.Start();
					//thUpDown.IsBackground = true;
				}
				return true;
			}
			catch (Exception ex) //Marina
			{
				_logger.Error("Multi : DownloadFiles()  : serialPortName : " + ex.Message, ex);
				return false;
			}
		}

		private void UploadFilesPrivate(List<string> list)
		{
			int pos, offset, file_size, byte1;
			FileStream fs = null;
			byte[] block = new byte[260];
			byte[] data = null;
			bool abort = false;

			foreach (string path in list)
			{
				FileInfo fi = new FileInfo(path);
				string file = fi.Name;

				if (!TransmitCommand(0x4C, file, ref data)) // L
				{ EventMPState(this, MPStage.Upload, (int)MPError.FailStart, file, 0); continue; }
				if (!File.Exists(path))
				{ EventMPState(this, MPStage.Upload, (int)MPError.FailStart, file, -1); continue; }

				try { fs = File.OpenRead(path); }
				catch { EventMPState(this, MPStage.Upload, (int)MPError.FileStream, file, 0); continue; }

				file_size = (int)fs.Length;
				for (pos = 0, offset = 1; offset <= file_size; offset++)
				{
					if (Thread.CurrentThread.Name != null)
					{
						EventMPState(this, MPStage.Upload, (int)MPError.Abort, file, file_size);
						if (!TransmitCommand(0x7A)) // z
							TransmitCommand(0x7A); // retry 2
						abort = true;
						break;
					}

					try { byte1 = fs.ReadByte(); }
					catch { EventMPState(this, MPStage.Upload, (int)MPError.FileStream, file, file_size); break; }

					if (byte1 == 0x5C) // \
					{ block[pos++] = 0x5C; block[pos++] = 0x5C; }
					else
						if (byte1 >= 0x00 && byte1 <= 0x1F)
						{ block[pos++] = 0x5C; block[pos++] = (byte)(byte1 + 0x80); }
						else
							if (byte1 >= 0xA0 && byte1 <= 0xFF && byte1 != 0xDC)
							{ block[pos++] = 0x5C; block[pos++] = (byte)(byte1 - 0x80); }
							else
								block[pos++] = (byte)byte1;

					if (offset != file_size && pos < 248)
						continue;

					if (!TransmitCommand(0x59, ref block, pos, ref data)) // Y
					{ EventMPState(this, MPStage.Upload, (int)MPError.FailBlock, file, file_size); break; }

					EventMPState(this, MPStage.Upload, offset, file, file_size);

					pos = 0;

					Thread.Sleep(30);

					if (offset == fs.Length)
						if (!TransmitCommand(0x5A, ref data)) // Z
						{ EventMPState(this, MPStage.Upload, (int)MPError.FailBlock, file, file_size); break; }
				}
				try { fs.Close(); }
				catch { EventMPState(this, MPStage.Upload, (int)MPError.FileStream, file, file_size); }

				if (abort)
					break;
			}
		}

		//Выгрузка файлов c компа на терминал
		public bool UploadFiles(List<string> list, string folder)
		{
			try  //Marina
			{
				if (_threadUpDown != null)
					if (_threadUpDown.IsAlive)
						return false;


				List<string> fullPathList = list.Select(r => Path.Combine(folder, r)).ToList();

				_threadUpDown = new Thread(delegate() { this.UploadFilesPrivate(fullPathList); });
				lock (_threadUpDown)
				{
					_threadUpDown.Start();
					//thUpDown.IsBackground = true;
					//thUpDown.IsBackground = true; //Marina
				}
				//thUpDown.Join(); //Marina
				
				return true;
			}
			catch (Exception ex) //Marina
			{
				_logger.Error("Multi : UploadFiles() :" + ex.Message, ex);
				return false;
			}
		}

		//Выгрузка файлов c компа на терминал
		public bool UploadFiles(List<ItemDir> list, string folder)
		{
			try  //Marina
			{
					if (_threadUpDown != null)
						if (_threadUpDown.IsAlive)
							return false;

				List<string> newlist = new List<string>();

				foreach (ItemDir item in list)
					newlist.Add(item.name);

				List<string> fullPathList = newlist.Select(r => Path.Combine(folder, r)).ToList();

				_threadUpDown = new Thread(delegate() { this.UploadFilesPrivate(fullPathList); });
				lock (_threadUpDown)
				{
					_threadUpDown.Start();
					//thUpDown.IsBackground = true;
					//thUpDown.Join(); //Marina
				}
				return true;
			}
			catch (Exception ex) //Marina
			{
				_logger.Error("Multi : UploadFiles() :" + ex.Message, ex);
				return false;
			}
		}

		//Выгрузка файлов c компа на терминал
		public bool UploadFiles(string[] list, string folder)
		{
			try  //Marina
			{
					if (_threadUpDown != null)
						if (_threadUpDown.IsAlive)
							return false;
				List<string> newlist = new List<string>();

				foreach (string item in list)
					newlist.Add(item);

				List<string> fullPathList = newlist.Select(r => Path.Combine(folder, r)).ToList();

				_threadUpDown = new Thread(delegate() { this.UploadFilesPrivate(fullPathList); });
				lock (_threadUpDown)
				{
					_threadUpDown.Start();
					//thUpDown.IsBackground = true;
				}
				//thUpDown.Join(); //Marina

				return true;
			}
			catch (Exception ex) //Marina
			{
				_logger.Error("Multi : UploadFiles() :" + ex.Message , ex);
				return false;
			}
		}

		//Выгрузка файлов c компа на терминал
		public bool UploadFiles(List<string> fullPathList)
		{
			try  //Marina
			{
				_logger.Info("MultiPoint.UploadFiles(List<string> fullPathList) 1");
				if (_threadUpDown != null)
					if (_threadUpDown.IsAlive == true)
						return false;


				_logger.Info("MultiPoint 2");
				_threadUpDown = new Thread(delegate() { this.UploadFilesPrivate(fullPathList); });
				_logger.Info("MultiPoint 3");
				lock (_threadUpDown)
				{

					_threadUpDown.Start();
					//thUpDown.IsBackground = true;

					//thUpDown.Join(); //Marina
						
					_logger.Info("MultiPoint 4");
				
				}//lock
				return true;
			}
			catch (Exception ex) //Marina
			{

				_logger.Error("Multi : UploadFiles() :" + " - " + ex.Message, ex);
				return false;
			}
		}

		public bool AbortUpDownLoading()
		{
			if (_threadUpDown == null)
				return false;

			lock (_threadUpDown)
			{
				if (!_threadUpDown.IsAlive)
					return false;
				if (_threadUpDown.Name != null)
					return false;

				_threadUpDown.Name = "Break";

				return true;
			}

		}

		//запуск файла на выполнение на терминале
		public bool Run(string file, MPMemory memory)
		{
			try  //Marina
			{
				byte[] data = null;

				if (!TransmitCommand(0x58, memory == MPMemory.ROM ? file + "/ROM" : file, ref data)) // X
					return false;
				if (data == null)
					return false;

				return data[0] == 0x01 ? false : true;
			}
			catch (Exception ex) //Marina
			{
				_logger.Error("Multi : Run() :", ex);
				return false;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		private void AbortWakeUp()
		{
			try
			{
				if (_threadWakeUP != null)
				{
					lock (_threadWakeUP)
					{
						if (_threadWakeUP.IsAlive)
							_threadWakeUP.Name = "Abort";
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error("Multi : AbortWakeUp() :" + ex.Message, ex);
			}
		}

		public void WakeUp()
		{
			try  //Marina
			{
				AbortWakeUp();
				if (_serial != null)
				{
					//lock (_serial)
					//{

						if (_serial.IsOpen)
						{
							_threadWakeUP = new Thread(this.SendCommandBELL);
							lock (_threadWakeUP)
							{
								if (_threadWakeUP != null)
								{
									_threadWakeUP.Start();
									//threadWakeUP.IsBackground = true;
									//threadWakeUP.Join(); //Marina
								}
							} //	lock (_threadWakeUP)
						}
					//} //lock (_serial)
				}
			}
			catch (Exception ex)
			{
				_logger.Error("Multi : WakeUp() :" + ex.Message, ex.Message);
			}
		}

		public bool Start(string from = "")
		{
			string serialPortName = _port;
			try
			{
				if (_serial != null)
				{
					//lock (_serial)
					//{
						if (_serial.IsOpen == true)
						{
							try
							{
							_serial.ErrorReceived -= new SerialErrorReceivedEventHandler(this.ErrorReceivedPort);
							_serial.Close();
							_serial.Dispose();
							Thread.Sleep(300);
								}
							catch (UnauthorizedAccessException ex)
							{
								_logger.Error("Start(" + from + ") :: Close  port :" + serialPortName + ex.Message);
								return false;
							}
						}
						//_serial = null;
					//}//lock
				}

               // _serial = new SerialPort(_port);
				_serial = new SerialPort(_port, _baudrate);
				if (_serial != null)
				{
					//lock (_serial)
					//{
						//_serial = null;
						//_serial = new SerialPort(_port, _baudrate);
						//Marina
						if (_serial.IsOpen == true)
						{
							try
							{
							_serial.ErrorReceived -= new SerialErrorReceivedEventHandler(this.ErrorReceivedPort);
							_serial.Close();
							_serial.Dispose();
							Thread.Sleep(30);
							}
							catch (UnauthorizedAccessException ex)
							{
								_logger.Error("Start(" + from + ") :: Close new  port :" + serialPortName + ex.Message);
								return false;
							}
						}

						_serial.BaudRate = _baudrate;
						_serial.ReadTimeout = 500;
						_serial.WriteTimeout = 100;
						_serial.Encoding = Encoding.ASCII;
						_serial.NewLine = "\x03";
						_serial.ErrorReceived += new SerialErrorReceivedEventHandler(this.ErrorReceivedPort);

						if (_serial.IsOpen == false)
						{
							int tryOneMoreTime = 0;
							try
							{
							_serial.Open();
							_logger.Info("Start(" + from + ") :: Open(if) port :" + serialPortName);
							}
							catch (UnauthorizedAccessException ex)
							{
								_logger.Error("Start(" + from + ") :: Open(if)  port one more time:" + serialPortName + ex.Message);
								try
								{
									tryOneMoreTime = 1;
									_serial.Close();
									_serial.Dispose();
								}
								catch (Exception){ }
								Thread.Sleep(30);
							}

							if (tryOneMoreTime == 1)
							{
								_serial.Open();
								_logger.Info("Start(" + from + ") :: Open(if)  port tryOneMoreTime:" + serialPortName);
							}
						}
						else
						{
							try
							{
							_serial.Close();
							_serial.Dispose();
							Thread.Sleep(30);
							_serial.Open();
							_logger.Info("Start(" + from + ") :: Open(else) port :" + serialPortName);
							}
							catch (UnauthorizedAccessException ex)
							{
								_logger.Error("Start(" + from + ") :: Open(else)  port :" + serialPortName + ex.Message);
								return false;
							}
						}
					//}lock
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				_logger.Error("Start(" + from + ") port :" + serialPortName + ex.Message);
				return false;
			}
			catch (Exception ex)
			{
				_logger.Error("Multi : Start(" + from + ") :" + ex.Message + " - " + ex.Message, ex);
				return false;
			}

			return true;
		}

		public void ErrorReceivedPort(object _sender, SerialErrorReceivedEventArgs _ea)
		{
			_logger.Error("ErrorReceivedPort :" + _ea.EventType + " - " + _ea, _ea);
		}


		public void Stop()
		{
			AbortWakeUp();
		}

		public void Close()
		{
			//string serialPortName = "";
			string serialPortName = _port;
			try
			{
				if (_serial != null)
				{
					//lock (_serial)
					//{
					if (_serial.IsOpen == true)
					{
						//serialPortName = serial.PortName;
						_serial.ErrorReceived -= new SerialErrorReceivedEventHandler(this.ErrorReceivedPort);
						_serial.Close();
						// fix last
					}
					//_serial = null; 
					//} //lock (_serial)
				}
			}
			catch (Exception ex)
			{
				_logger.Error("Multi : Close() :" + serialPortName + " - " + ex.Message, ex);
			}
		}

		public void Delete()
		{
			_logger.Info("Multi : Delete()  start");
			string serialPortName = _port;
			try
			{
				//_logger.Info("Multi : Delete()  : start " + serialPortName);
				if (_serial != null)
				{
					_logger.Info("Multi : Delete()  : serial != null " + serialPortName);
					//lock (_serial)
					//{
						serialPortName = _serial.PortName;
						if (_serial.IsOpen == true)
						{
							_serial.ErrorReceived -= new SerialErrorReceivedEventHandler(this.ErrorReceivedPort);
							_logger.Info("Multi : Delete()  : serial.IsOpen == true " + serialPortName);
							_serial.Close();
							_logger.Info("Multi : Delete()  : serial.Close " + serialPortName);
						}
						//_serial = null;
					//} //lock
				}

				_logger.Info("Multi : Delete()  : after serial !=null " + serialPortName);
				//serial = null;

				if (_threadUpDown != null)
				{
					lock (_threadUpDown)
					{
						if (_threadUpDown.IsAlive == true)
						{
							//thUpDown.Join(1000);
							if (_threadUpDown.Name != "Break")
							{
								_threadUpDown.Abort();
							}
						}
						//if (thUpDown.Join(2000) == false) _logger.Info("Multi : Delete() : thUpDown.Abort ()" + serialPortName + "not Aborted in 2 sec");
						_threadUpDown = null;
					}
				}

				if (_threadWakeUP != null)
				{

					lock (_threadWakeUP)
					{
						if (_threadWakeUP.IsAlive == true)
						{
							//threadWakeUP.Join(1000);
							if (_threadWakeUP.Name != "Break")
							{
								_threadWakeUP.Abort();
							}
						}
						//if (threadWakeUP.Join(2000) == false) _logger.Info("Multi : Delete() : threadWakeUP.Abort ()" + serialPortName + "not Aborted in 2 sec");
						_threadWakeUP = null;
					}
				}


				_logger.Info("Multi : Delete() :" + serialPortName);

			}
			catch (Exception ex)
			{
				_logger.Error("Multi : Delete() :" + serialPortName + " - " + ex.Message, ex);
			}


		}



		private void thUpDownWait()
		{
			if (_threadUpDown != null)
			{
				lock (_threadUpDown)
				{
					_threadUpDown.Join();
				}
			}
		}

		public bool SerialIsOpen()
		{
				if (_serial == null)
					return false;
				if (_serial.IsOpen == false)
					return false;
				return true;

			//try
			//{
			//	if (serial == null)
			//		return false;
			//	if (serial.IsOpen == false)
			//	{
			//		string serialPortName = serial == null ? "serialPortNameIsNull" : serial.PortName;
			//		serial.Open();
			//		if (serial.IsOpen == true)
			//		{
			//			_logger.Info("Multi : SerialIsOpen() :" + serialPortName + " Successfully Open");
			//		}
			//		else
			//		{
			//			_logger.Info("Multi : SerialIsOpen() : Try open Port " + serialPortName + "NOT Successfully Open");
			//		}
			//	}
			//}
			//catch (Exception ex)
			//{
			//	string serialPortName = serial == null ? "serialPortNameIsNull" : serial.PortName;
			//	_logger.Error("Multi : SerialIsOpen() :" + serialPortName, ex);
			//	return false;
			//}
			//return true;
		}

		
	}
}
