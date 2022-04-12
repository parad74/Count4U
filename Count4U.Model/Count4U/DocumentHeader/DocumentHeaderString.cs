using System;
//using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
	public class DocumentHeaderString
	{
		public string Name { get; set; }   //Device
		public string Approve { get; set; }
		public string Code { get; set; }
		public string DocumentCode { get; set; }
		//public string Itur { get; set; }
		public string SessionCode { get; set; }
		//public string StatusDocHeader { get; set; }

		public string CreateDate { get; set; }
		public string CreateTime { get; set; }
		public string ModifyDate { get; set; }
		public string IturCode { get; set; }
		//public string StatusDocHeaderCode   { get; set; }
		//public string StatusDocHeaderBit { get; set; }
		public string WorkerGUID { get; set; }					 //Worker
		public string QuantityEdit { get; set; }
		public string Total { get; set; }
		public string FromTime { get; set; }
		public string ToTime { get; set; }
		public string TicksTimeSpan { get; set; }
		public string PeriodFromTo { get; set; }


		public DocumentHeaderString()
		{
			Name = "";
			Approve = "";
			Code = "";
			DocumentCode = "";
			//Itur = "";
			SessionCode = "";
			//StatusDocHeader = "";
			CreateDate = "";
			ModifyDate = "";
			IturCode = "";
			//StatusDocHeaderCode  = "";
			//StatusDocHeaderBit = "";
			WorkerGUID = "";
			QuantityEdit = "";
			Total = "";
			FromTime = "";
			ToTime = "";
			TicksTimeSpan = "";
			PeriodFromTo = "";
		}
	}
}
