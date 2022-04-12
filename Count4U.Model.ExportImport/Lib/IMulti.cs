using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace Count4U.Model.Lib.MultiPoint
{
	public interface IMulti
	{
		bool AbortUpDownLoading();
		byte Address { get; }
		void Close();
		void Delete();
		//Удаление файлов на терминале
		bool DeleteFiles();
		bool DeleteFiles(List<ItemDir> exclude);
		bool DeleteFiles(List<string> exclude);
		bool DeleteFiles(string[] exclude);
		//Загрузка файлов c терминала на компьютер
		bool DownloadFiles(List<ItemDir> list, string folder);
		bool DownloadFiles(List<string> list, string folder);
		bool DownloadFiles(string[] list, string folder);
		void ErrorReceivedPort(object _sender, SerialErrorReceivedEventArgs _ea);
		event MPState EventMPState;
		List<ItemDir> GetDirectory(bool with_size);
		string GetTerminalID();
		string GetTerminalVersion();
		string Port { get; }
		int Baudrate { get; set; }
        int[] Baud_rates { get; set; }
		
		//запуск файла на выполнение на терминале
		bool Run(string file, MPMemory memory);
		bool SerialIsOpen();
		bool SetDateTime(DateTime dt);
		bool Start(string from = "");
		void Stop();
		string TerminalID { get; set; }
		//Выгрузка файлов c компа на терминал
		bool UploadFiles(List<ItemDir> list, string folder);
		bool UploadFiles(List<string> fullPathList);
		bool UploadFiles(List<string> list, string folder);
		bool UploadFiles(string[] list, string folder);

		void WakeUp();
		bool WarmStart();
	}
}
