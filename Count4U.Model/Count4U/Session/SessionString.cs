using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
	public class SessionString
    {
        public string PDAID { get; set; }
		public string SessionCode { get; set; }
		public string CreateDate { get; set; }
		public string PDADate { get; set; }
        public string WorkerGUID { get; set; }
		public string CountItem { get; set; }
		public string CountDocument { get; set; }
		public string CountItur { get; set; }

		public SessionString()
		{
			PDAID = "";
			SessionCode = "";
			CreateDate = "";
			PDADate = "";
			WorkerGUID = "";
			CountItem = "0";
			CountDocument = "0";
			CountItur = "0";
		}

	}
}
