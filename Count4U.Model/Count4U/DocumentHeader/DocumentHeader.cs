using System;
using Count4U.Model.Extensions;
//using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
    [Serializable]
    public class DocumentHeader
	{
		#region IDocumentHeader Members
		
        public long ID { get; set; }
         public string Name { get; set; }			   //Device
  
        public bool? Approve { get; set; }
        public string Code { get; set; }
        public string DocumentCode { get; set; }
		[PropertyNotBulk]
        public string Itur { get; set; }
		public string SessionCode { get; set; }			  //!
		[PropertyNotBulk]
        public string StatusDocHeader { get; set; }

		public DateTime CreateDate  { get; set; }
		public DateTime? ModifyDate	 { get; set; }
 		public string IturCode  { get; set; }
		public string StatusDocHeaderCode   { get; set; }     //name + "|" + worker
		public int StatusDocHeaderBit   { get; set; }
		public int StatusInventProductBit   { get; set; }
		public int StatusApproveBit { get; set; }
		public string WorkerGUID   { get; set; }		 //!  Worker
		public int DocNum { get; set; }

		public string Restore  { get; set; }
		 public bool RestoreBit { get; set; }

		public double QuantityEdit { get; set; }
		public long Total { get; set; }
		public DateTime FromTime { get; set; }
		public DateTime ToTime { get; set; }
		public long TicksTimeSpan { get; set; }
		public string  PeriodFromTo{ get; set; }

		
		//public int Num { get; set; }
	
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(DocumentHeader)) return false;
			return Equals((DocumentHeader)obj);
		}

		public bool Equals(DocumentHeader other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.DocumentCode, this.DocumentCode);	  
		}

		public override int GetHashCode()
		{
			return (DocumentCode != null ? DocumentCode.GetHashCode() : 0);
		}

		public DocumentHeader()
		{
			this.Name = "";
			this.Approve = null;
			this.Code = "";
			this.DocumentCode = "";
			this.Itur = "";
			this.SessionCode = "";
			this.StatusDocHeader = "";

			this.CreateDate = DateTime.Now;
			this.ModifyDate = DateTime.Now;
			this.IturCode = "";
			this.StatusDocHeaderCode = "";
			this.StatusDocHeaderBit = 0;
			this.StatusInventProductBit = 0;
			this.StatusApproveBit = 0;
			this.WorkerGUID = "";
			this.DocNum = 0;

			this.Restore  = "";
			this.RestoreBit = false;

			this.QuantityEdit = 0;
			this.Total = 0;
			this.FromTime = DateTime.Now;
			this.ToTime = DateTime.Now;
			this.TicksTimeSpan = 0;
			this.PeriodFromTo = "";
		}

        #endregion
	}
}
