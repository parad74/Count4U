using System;
using System.Collections.Generic;
using System.Windows.Documents;
using Count4U.Model.Lib.MultiPoint;

namespace Count4U.Model.Count4U
{
    public enum UploadPdaMPStage
  {
    Bell,
    Upload,
    Download,
  }

    public delegate void UploadPdaUnitMPState(IWrapperMulti unit, UploadPdaMPStage stage, int state, string text, int current);

    public interface IWrapperMultiRepository
    {
		List<IWrapperMulti> GetPortsAndWakeUP(int baudrate = 57600, string from = "", bool reWakeUp = true);
		//void WakeUpAllPorts(List<IUploadPdaUnit> units, int baudrate = 57600, string from = "");
		void GetTerminalIDAllPDA(List<IWrapperMulti> wrapperMultis, string from = "");
		void WakeUpAllPorts(List<IWrapperMulti> wrapperMultis, int baudrate = 57600, string from = "");
		void TryWakeUpAllNotStartPorts(List<IWrapperMulti> wrapperMultis, int baudrate = 57600, string from = "");
        void ResetBaudrate(List<IWrapperMulti> wrapperMultis, int baudrate = 57600, string from = "");
		void AbortThreadAllThreads(List<IWrapperMulti> wrapperMultis, string from = "");
		void SetDateTimeAllPDA(List<IWrapperMulti> wrapperMultis, DateTime dateTime, string from = "");
		void DeleteFilesAllPDA(List<IWrapperMulti> wrapperMultis, List<string> exclude, string from = "");
		void WarmStartAllPDA(List<IWrapperMulti> wrapperMultis, string from = "");
		void RunFilesAllPDA(List<IWrapperMulti> wrapperMultis, List<string> files, MPMemory memory, string from = "");
		//присваивание имени "abort" процессу
		void AbortUpDownLoading(IWrapperMulti wrapperMulti);
	}
}