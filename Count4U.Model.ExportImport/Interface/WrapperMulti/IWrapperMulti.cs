using System.Collections.Generic;
using Count4U.Model.Lib.MultiPoint;

namespace Count4U.Model.Count4U
{
    public interface IWrapperMulti
    {
        string ComPortStatic { get; set; } //1
		event UploadPdaUnitMPState StateChanged;
		IMulti Multi { get; }
		void Delete();
		string GetTerminalID();
		//файлы на терминале
		List<ItemDir> GetDirectory(bool with_size);
		bool UploadFiles(List<string> fullPathList);
		bool DownloadFiles(List<string> lileNameList, string folder);
		//string GetCOMPort();//0
		//byte GetAddress();//0
		//void WakeUp();//0
		//bool Start();//0
		//void Stop();//0
		//string GetTerminalID();//0
		//bool UploadFiles(List<string> list, string folder);//0
		//bool UploadFiles(List<string> fullPathList);//0
		//bool AbortUpDownLoading();//0
 		//void Close();//0
		//bool SerialIsOpen();//0
		//void MultiAbortThread();//0
    }
}