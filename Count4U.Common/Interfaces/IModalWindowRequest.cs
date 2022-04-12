using System;
using System.Collections.Generic;

namespace Count4U.Common.Interfaces
{
    public interface IModalWindowRequest
    {
        event EventHandler<ModalWindowRequestPayload> ModalWindowRequest;      
    }

    public class ModalWindowRequestPayload : EventArgs
    {
        public Dictionary<string, string> Settings { get; set; }
        public Action<object> Callback { get; set; }
        public string WindowTitle { get; set; }
        public string ViewName { get; set; }

		public int Width { get; set; }
		public int Height { get; set; }
		public int MinWidth { get; set; }
		public int MinHeight { get; set; }
		public string PathInData { get; set; }
		public string PathFtp { get; set; }
		public string PathAbsolute { get; set; }
    }
}