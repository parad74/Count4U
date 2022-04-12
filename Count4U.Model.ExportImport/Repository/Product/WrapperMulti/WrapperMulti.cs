using System;
using System.Collections.Generic;
using System.Threading;
using Count4U.Model.Lib.MultiPoint;
using NLog;

namespace Count4U.Model.Count4U
{
    public class WrapperMulti : IWrapperMulti
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		private readonly IMulti _multi;
		public string ComPortStatic { get; set; }
		public event UploadPdaUnitMPState StateChanged;

		public WrapperMulti(IMulti multi)  //вызывается один раз при имплементации в IWrapperMultiRepository
        {
            this._multi = multi;
			this._multi.EventMPState += new MPState(Multi_EventMPState);
        }

		public IMulti Multi
		{
			get { return _multi; }
		} 

		public void Delete()
		{
			//this._multi.EventMPState -= Multi_EventMPState;
			this._multi.Delete();
		}

		public string GetTerminalID()
		{
			return this._multi.TerminalID;
		}


		//файлы на терминале
		public List<ItemDir> GetDirectory(bool with_size)
		{
			return this._multi.GetDirectory(with_size);
		}

		//Выгрузка файлов c компа на терминал
		public bool UploadFiles(List<string> fullPathList)
		{
			return this._multi.UploadFiles(fullPathList);
		}

		//Выгрузка файлов c терминала на комп
		public bool DownloadFiles(List<string> lileNameList, string folder)
		{
			return this._multi.DownloadFiles(lileNameList, folder);
		}
			

        void Multi_EventMPState(IMulti from, MPStage stage, int state, string text, int current)
        {
			//public enum MPStage { Bell = 0x07, Upload = 0x4C, Download = 0x55 };  //MPStage stage
			//public enum MPError { NoAddress = -0x80, NAK = -0x15, FailStart = -0x10, FailBlock = -0x11, FileStream = -0x12, Abort = -0x13 }; //int state

            UploadPdaMPStage stageUI;

            switch (stage)
            {
                case MPStage.Bell:
                    stageUI = UploadPdaMPStage.Bell;
                    break;
                case MPStage.Upload:
                    stageUI = UploadPdaMPStage.Upload;
                    break;
                case MPStage.Download:
                    stageUI = UploadPdaMPStage.Download;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("stage");
            }

            OnStateChanged(stageUI, state, text, current);
        }

		protected virtual void OnStateChanged(UploadPdaMPStage stage, int state, string text, int current)
		{
			UploadPdaUnitMPState handler = StateChanged;
			if (handler != null) handler(this, stage, state, text, current);
		}

		//void WaitForOneSecondsAsync(Action callback)		// Asynchronous NON-BLOCKING method
		//{
		//	new Timer(_ => callback()).Change(1000, -1);
		//}


		//public string GetCOMPort() //rem
		//{
		//	try
		//	{
		//		return _multi.Port;
		//	}
		//	catch (Exception exc)
		//	{
		//		_logger.ErrorException("GetCOMPort", exc);
		//	}

		//	return String.Empty;
		//}

		//public byte GetAddress() //rem
		//{
		//	try
		//	{
		//		return _multi.Address;
		//	}
		//	catch (Exception exc)
		//	{
		//		_logger.ErrorException("GetAddress", exc);
		//	}

		//	return 0;
		//}

		//public void WakeUp() //rem
		//{
		//	try
		//	{
		//		_multi.WakeUp();
		//	}
		//	catch (Exception exc)
		//	{
		//		_logger.ErrorException("WakeUp", exc);
		//		throw;
		//	}
		//}

		//public bool Start()  //rem
		//{
		//	if (_multi.Address != 0x80)
		//	{
		//		return _multi.Start();
		//	}
		//	else
		//	{
		//		return false;
		//	}

		//}

		//public void Stop() //rem
		//{
		//	//      Thread.Sleep(TimeSpan.FromSeconds(2));
		//	if (_multi.Address != 0x80)
		//	{
		//		_multi.Stop();
		//	}

		//}

		//public string GetTerminalID() //rem
		//{
		//	try
		//	{
		//		if (_multi.SerialIsOpen() == false) return "not started";
		//		if (_multi.Address != 0x80)
		//		{
		//			string terminalID = _multi.GetTerminalID();
		//			if (string.IsNullOrWhiteSpace(terminalID) == true) 	terminalID = "not started";
		//			return terminalID;
		//		}
		//		else
		//		{
		//			return "not started";
		//		}
		//	}
		//	catch (Exception exc)
		//	{
		//		_logger.ErrorException("GetTerminalID", exc);
		//		throw;
		//	}
		//}

	

		//public bool UploadFiles(List<string> fullPathList) //rem
		//{
		//	try
		//	{
		//		_logger.Info("UploadPdaUnit.UploadFiles(List<string> fullPathList)");
		//		return _multi.UploadFiles(fullPathList);
		//	}
		//	catch (Exception exc)
		//	{
		//		_logger.ErrorException("UploadFiles", exc);
		//		throw;
		//	}
		//}

		//public bool UploadFiles(List<string> list, string folder) //rem
		//{
		//	try
		//	{
		//		return _multi.UploadFiles(list, folder);
		//	}
		//	catch (Exception exc)
		//	{
		//		_logger.ErrorException("UploadFiles", exc);
		//		throw;
		//	}
		//}

		//public bool AbortUpDownLoading() //rem
		//{
		//	try
		//	{
		//		if (_multi.Address != 0x80)
		//		{
		//			return _multi.AbortUpDownLoading();
		//		}
		//		else return false;
		//	}
		//	catch (Exception exc)
		//	{
		//		_logger.ErrorException("AbortUpDownLoading", exc);
		//		return false;
		//	}

		//}

	  //  public void Close() //rem
	  //  {
	  ////      Thread.Sleep(TimeSpan.FromSeconds(2));
	  //	  _multi.Close();
	  //  }

		//public void MultiAbortThread() //rem
		//{
		//	_multi.Delete();
		//}


		//public bool SerialIsOpen()  //rem
		//{
		//		return _multi.SerialIsOpen();
		//}

    
    }
}