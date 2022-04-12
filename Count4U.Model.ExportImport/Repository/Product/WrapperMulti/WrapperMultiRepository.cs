using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Count4U.Model.Lib.MultiPoint;
using NLog;

namespace Count4U.Model.Count4U
{
	public class WrapperMultiRepository : IWrapperMultiRepository
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private Object AddMultiItemLock = new Object();
		private Object DeleteMultiItemLock = new Object();


		public List<IWrapperMulti> GetPortsAndWakeUP(int baudrate = 57600, string from = "", bool reWakeUp = true)
		{

			List<IWrapperMulti> result = new List<IWrapperMulti>();

			List<string> ports = System.IO.Ports.SerialPort.GetPortNames().ToList();

			_logger.Info("GetPortsAndWakeUP start from " + from);
			lock (AddMultiItemLock)
			{
				foreach (string port in ports)
				{
					IMulti multi = new Multi(port, baudrate); //имплементация конкретного Multi 
					IWrapperMulti unit = new WrapperMulti(multi);  //обертка конкретного Multi 
					unit.ComPortStatic = port;

					if (reWakeUp == true)  // сейчас вызов только с false
					{
						if (unit.Multi.Start("GetPortsAndWakeUP") == true)
						{
							Task wakeUpTask = new Task(() => { unit.Multi.WakeUp(); });
							wakeUpTask.Start();
							wakeUpTask.Wait();
							Task error = wakeUpTask.ContinueWith(ant => _logger.Info(ant.Exception),
									 TaskContinuationOptions.OnlyOnFaulted);
						}
					}
				
					result.Add(unit);
				
				}
				_logger.Info("GetPortsAndWakeUP finish " + from);
				return result;
			}//lock
		}

		public void WakeUpAllPorts(List<IWrapperMulti> wrapperMultis, int baudrate = 57600, string from = "")
		{
           _logger.Info("WakeUpAllPorts start from " + from + " baudrate:" + baudrate);
			if (wrapperMultis == null) return;
			List<IMulti> multis = wrapperMultis.Select(x => x.Multi).ToList();

			lock (AddMultiItemLock)
			{
				Task[] wakeUpTasks = new Task[multis.Count()];
				for (int i = 0; i < multis.Count(); i++)
				{
					if (multis[i].Start("WakeUpAllPorts") == true)
					{
						wakeUpTasks[i] = new Task(() => { multis[i].WakeUp(); });
						wakeUpTasks[i].Start();
						wakeUpTasks[i].Wait();

						Task error = wakeUpTasks[i].ContinueWith(ant => _logger.Info(ant.Exception),
								 TaskContinuationOptions.OnlyOnFaulted);
					}
				}

			//Task.WaitAll(wakeUpTasks);
				_logger.Info("WakeUpAllPorts finish " + from);
			}//lock
		}

        public void TryWakeUpAllNotStartPorts(List<IWrapperMulti> wrapperMultis, int baudrate = 57600, string from = "")
        {
            int[] _baud_rates = new int[] { 57600, 38400, 19200, 9600, 4800 };
            if (baudrate == 38400) _baud_rates = new int[] { 38400, 19200, 9600, 4800 };
            if (baudrate == 19200) _baud_rates = new int[] { 19200, 9600, 4800 };

            _logger.Info("TryWakeUpAllNotStartPorts start from " + from + " baudrate:" + baudrate);
            if (wrapperMultis == null) return;
            List<IMulti> multis = wrapperMultis.Select(x => x.Multi).ToList();

            lock (AddMultiItemLock)
            {
                Task[] wakeUpTasks = new Task[multis.Count()];
                for (int i = 0; i < multis.Count(); i++)
				{
					multis[i].Baud_rates = _baud_rates;
					if (multis[i].Start("TryWakeUpAllNotStartPorts") == true)
                    {
                        wakeUpTasks[i] = new Task(() => { multis[i].WakeUp(); });
                        wakeUpTasks[i].Start();
                        wakeUpTasks[i].Wait();

                        Task error = wakeUpTasks[i].ContinueWith(ant => _logger.Info(ant.Exception),
                                 TaskContinuationOptions.OnlyOnFaulted);
                    }
                }

                //Task.WaitAll(wakeUpTasks);
                _logger.Info("TryWakeUpAllNotStartPorts finish " + from);
            }//lock
        }

        public void ResetBaudrate(List<IWrapperMulti> wrapperMultis, int baudrate = 57600, string from = "")
        {
            int[] _baud_rates = new int[] { 57600, 38400, 19200, 9600, 4800 };

            _logger.Info("ResetBaudrate start from " + from + " baudrate:" + baudrate);
            if (wrapperMultis == null) return;
            List<IMulti> multis = wrapperMultis.Select(x => x.Multi).ToList();

            lock (AddMultiItemLock)
            {
                Task[] wakeUpTasks = new Task[multis.Count()];
                for (int i = 0; i < multis.Count(); i++)
                {
                    multis[i].Baud_rates = _baud_rates;
               
                }

                //Task.WaitAll(wakeUpTasks);
                _logger.Info("ResetBaudrate finish " + from);
            }//lock
        }

		public void GetTerminalIDAllPDA(List<IWrapperMulti> wrapperMultis, string from = "")
		{
			_logger.Info("GetTerminalIDAllPDA start from " + from);
			if (wrapperMultis == null) return;
			List<IMulti> multis = wrapperMultis.Select(x => x.Multi).ToList();
			//lock (AddMultiItemLock)
			//{
				lock (multis)
				{
					//Task<string>[] getIDTasks = new Task<string>[multis.Count()];
					for (int i = 0; i < multis.Count(); i++)
					{
						try
						{
                            //string terminalID = "not started";
                            //if (multis[i] != null)
                            //{
                            //    terminalID = multis[i].GetTerminalID();
                            //}

                            if (multis[i] != null)
                            {
                                Task getIDTask = new Task(() => { multis[i].GetTerminalID(); });
                                getIDTask.Start();
                                bool ret = getIDTask.Wait(500);
                                if (ret == false)
                                {
                                    _logger.Info("GetTerminalID() not .Wait(500) i  = " + i);
                                    multis[i].TerminalID = "not started";
                                }
                                Task error = getIDTask.ContinueWith(ant => _logger.Info(ant.Exception),
                                         TaskContinuationOptions.OnlyOnFaulted);
                            }

                            //string terminalID = "not started";
                            //getIDTasks[i] = new Task<string>(() => { var id = multis[i].GetTerminalID(); return id; });
                            //getIDTasks[i].Start();
                            //terminalID = getIDTasks[i].Result;
                            //Task error = getIDTasks[i].ContinueWith(ant => _logger.Info(terminalID + "ContinueWith OnFaulted : " + ant.Exception),
                            // TaskContinuationOptions.OnlyOnFaulted);
						}
						catch (Exception exc)
						{
							_logger.ErrorException("GetTerminalIDAllPDA :: i =" + i.ToString() + exc.Message, exc);
						}
					}
					//Task.WaitAll(getIDTasks);
					_logger.Info("GetTerminalIDAllPDA finish " + from);
				}//lock
			//}//lock
		}

		public void AbortThreadAllThreads(List<IWrapperMulti> wrapperMultis, string from = "")
		{
			_logger.Info("AbortThreadAllThreads start from " + from);
			if (wrapperMultis == null) return;
	        lock (wrapperMultis)
			{
				string comPort = "";
				for (int i = 0; i < wrapperMultis.Count(); i++)
				{
					try
					{
						if (wrapperMultis[i] != null)
						{
							comPort = wrapperMultis[i].ComPortStatic;
							//Thread.Sleep(wakeupTime * 200);
								wrapperMultis[i].Delete();
							
						}
					}
					catch (Exception exc)
					{
						_logger.ErrorException("AbortThreadAllThreads :: Delete i =" + i.ToString() + " : " + comPort + " ; " + exc.Message, exc);
					}
					//abortThreadTasks[i] = new Task(() => { units[i].Multi.Delete(); });
					//abortThreadTasks[i].Start();
					
					//Task error = abortThreadTasks[i].ContinueWith(ant => _logger.Info(ant.Exception),
					//		 TaskContinuationOptions.OnlyOnFaulted);
				}
				//Task.WaitAll(abortThreadTasks);

				_logger.Info("AbortThreadAllThreads finish " + from);
			}//lock
		}

		//public static void DeleteAllMulti(List<Multi> multis)
		//{
		//	lock (multis)
		//	{
		//		for (int i = 0; i < multis.Count(); i++)
		//		{
		//			try
		//			{
		//				if (multis[i] != null)
		//				{
		//					multis[i].Delete();
		//				}
		//			}
		//			catch (Exception exc)
		//			{
		//				_logger.ErrorException("AbortThreadAllThreads :: Delete i =" + i.ToString() + exc.Message, exc);
		//			}
		//		}
		//	}
		//}


		public void SetDateTimeAllPDA(List<IWrapperMulti> wrapperMultis, DateTime dateTime, string from = "")
		{
			_logger.Info("SetDateTimeAllPDA start from " + from);
			if (wrapperMultis == null) return;
			List<IMulti> multis = wrapperMultis.Select(x => x.Multi).ToList();
			if (dateTime == null || dateTime <= DateTime.MinValue) dateTime = DateTime.Now;

			lock (multis)
			{
				for (int i = 0; i < multis.Count(); i++)
				{
					try
					{
						//if (multis[i] != null)
						//{
						//	multis[i].SetDateTime(dateTime);
						//}

					if (multis[i] != null)
						{
							Task setDateTimeTask = new Task(() => { multis[i].SetDateTime(dateTime); });
							setDateTimeTask.Start();
							bool ret = setDateTimeTask.Wait(3000);
							if (ret == false)
							{
								_logger.Info("SetDateTime() not .Wait(3000) i  = " + i);
							}
							Task error = setDateTimeTask.ContinueWith(ant => _logger.Info(ant.Exception),
									 TaskContinuationOptions.OnlyOnFaulted);
						}
					}
					catch (Exception exc)
					{
						_logger.ErrorException("SetDateTimeAllPDA :: i =" + i.ToString() + exc.Message, exc);
					}
				}

				_logger.Info("SetDateTimeAllPDA finish " + from);
			}//lock
		}

		//Удаление файлов на терминалах
		public void DeleteFilesAllPDA(List<IWrapperMulti> wrapperMultis, List<string> exclude, string from = "")
		{
			_logger.Info("DeleteFilesAllPDA start from " + from);
			if (wrapperMultis == null) return;
			List<IMulti> multis = wrapperMultis.Select(x => x.Multi).ToList();
			if (exclude == null) exclude = new List<string>();

			lock (multis)
			{
				for (int i = 0; i < multis.Count(); i++)
				{
					try
					{
						if (multis[i] != null)
						{
							//Удаление файлов на терминале
							multis[i].DeleteFiles(exclude);
						}
					}
					catch (Exception exc)
					{
						_logger.ErrorException("DeleteFilesAllPDA :: i =" + i.ToString() + exc.Message, exc);
					}
				}

				_logger.Info("DeleteFilesAllPDA finish " + from);
			}//lock
		}

		public void WarmStartAllPDA(List<IWrapperMulti> wrapperMultis, string from = "")
		{
			_logger.Info("WarmStartAllPDA start from " + from);
			if (wrapperMultis == null) return;
			List<IMulti> multis = wrapperMultis.Select(x => x.Multi).ToList();
			lock (multis)
			{
				for (int i = 0; i < multis.Count(); i++)
				{
					try
					{
						if (multis[i] != null)
						{
							multis[i].WarmStart();
						}
					}
					catch (Exception exc)
					{
						_logger.ErrorException("WarmStartAllPDA :: i =" + i.ToString() + exc.Message, exc);
					}
				}

				_logger.Info("WarmStartAllPDA finish " + from);
			}//lock
		}

		//запуск файлов на выполнение на терминалах
		public void RunFilesAllPDA(List<IWrapperMulti> wrapperMultis, List<string> files, MPMemory memory, string from = "")
		{
			_logger.Info("RunFilesAllPDA start from " + from + "in memory " + memory.ToString());
			if (wrapperMultis == null) return;
			List<IMulti> multis = wrapperMultis.Select(x => x.Multi).ToList();

			if (files == null) return;

			lock (multis)
			{
				for (int i = 0; i < multis.Count(); i++)
				{
					try
					{
						if (multis[i] != null)
						{
							foreach (string file in files)
							{
								//запуск файла на выполнение на терминале
								multis[i].Run(file, memory);
							}
						}
					}
					catch (Exception exc)
					{
						_logger.ErrorException("RunFilesAllPDA :: i =" + i.ToString() + exc.Message, exc);
					}
				}

				_logger.Info("RunFilesAllPDA finish " + from);
			}//lock
		}


		//присваивание имени "abort" процессу
		public void AbortUpDownLoading(IWrapperMulti wrapperMulti)
		{
			if (wrapperMulti.Multi.Address != 0x80)
			{
				wrapperMulti.Multi.AbortUpDownLoading();
			}
		}

		
		
	}
}
		