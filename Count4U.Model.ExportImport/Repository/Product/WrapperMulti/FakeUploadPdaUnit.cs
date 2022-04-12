using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using Count4U.Model.Lib.MultiPoint;
using NLog.LayoutRenderers;

namespace Count4U.Model.Count4U
{
    public class FakeUploadPdaUnit : IWrapperMulti
    {
        private static readonly Random _rnd = new Random();

        private int UploadingChunkSize = 5200; //500bytes

        private readonly DispatcherTimer _timer;

        private List<FileInfo> _uploadingFiles;
        private int? _uploadingIndex;
        private long _uploadedBytesFile;

        public FakeUploadPdaUnit()
        {
            IsFault = false;
            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
            _timer.Interval = TimeSpan.FromMilliseconds(500);
        }      

        public int Id { get; set; }

        public bool IsFault { get; set; }

        public string ComPortStatic { get; set; }

		//public string GetCOMPort() //remove
		//{
		//   // return String.Format("COM{0}", Id);
		//	return ComPortStatic;
		//}

		//public byte GetAddress()
		//{
		//	return 100;
		//}

		//public void WakeUp()
		//{

		//}

		//public bool Start()
		//{
		//	return true;
		//}

		//public void Stop()
		//{
		//	_uploadingIndex = null;
		//	_uploadedBytesFile = 0;
		//	_timer.Stop();
		//}

		//public string GetTerminalID()
		//{
		//	return "Terminal ID";
		//}

		//public void Close()
		//{

		//}

        public event UploadPdaUnitMPState StateChanged;        

        protected virtual void OnStateChanged(UploadPdaMPStage stage, int state, string text, int current)
        {
            UploadPdaUnitMPState handler = StateChanged;
            if (handler != null) handler(this, stage, state, text, current);
        }

		//public bool UploadFiles(List<string> list, string folder)
		//{
		//	return UploadFiles(list.Select(r => Path.Combine(folder, r)).ToList());
		//}

		//public bool UploadFiles(List<string> fullPathList)
		//{
		//	if (_timer.IsEnabled)
		//	{
		//		throw new InvalidOperationException();
		//	}

		//	_uploadingFiles = new List<FileInfo>();
		//	long totalSize = 0;

		//	foreach (string path in fullPathList)
		//	{
		//		if (File.Exists(path))
		//		{
		//			FileInfo fi = new FileInfo(path);
		//			_uploadingFiles.Add(fi);
		//			totalSize += fi.Length;
		//		}
		//	}

		//	UploadingChunkSize = (int)totalSize / (3 * fullPathList.Count);

		//	_timer.Start();

		//	return true; 
		//}

		//public bool AbortUpDownLoading()
		//{
		//	_uploadingIndex = null;
		//	_uploadedBytesFile = 0;
		//	_timer.Stop();

		//	return true;
		//}      

        void Timer_Tick(object sender, EventArgs e)
        {
            if (_uploadingIndex == null)
            {
                _uploadingIndex = 0;
                _uploadedBytesFile = 0;
            }

            FileInfo file = _uploadingFiles[_uploadingIndex.Value];

            long currentChunk = (long)(UploadingChunkSize * _rnd.NextDouble());

            long report = 0;
            long state = file.Length;
            bool fileDone = false;
            if (file.Length <= _uploadedBytesFile + currentChunk) //last chunk
            {
                report = file.Length;
                _uploadedBytesFile = file.Length;
                fileDone = true;
            }
            else
            {
                _uploadedBytesFile += currentChunk;
                report = _uploadedBytesFile;
            }

            if (IsFault)
            {
                if (_rnd.Next(0, 3) == 1)
                {
                    state = -1;
                }
            }

            OnStateChanged(UploadPdaMPStage.Upload, (int)state, file.Name, (int)report);

            if (fileDone)
            {
                if (_uploadingFiles.IndexOf(file) == _uploadingFiles.Count - 1) //all files done
                {
                    _timer.Stop();
                    _uploadingIndex = 0;
                    _uploadedBytesFile = 0;
                }
                else //only current file done
                {
                    _uploadingIndex++;
                    _uploadedBytesFile = 0;
                }
            }
        }


		public Multi Multi
		{
			get { throw new NotImplementedException(); }
		}


		//public bool SerialIsOpen()
		//{
		//	return true;
		//}


		//public void MultiAbortThread()
		//{
		//	throw new NotImplementedException();
		//}
	}
}