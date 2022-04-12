using System;
using Count4U.Model.Audit;
	
namespace Count4U.Model.Audit
{
	public class AuditConfig //: IAuditConfig
	{
		#region IAuditConfig Members

		public long ID { get; set; }
		public string Code { get; set; }
		public string DirtyCode { get; set; }
		public string BranchCode { get; set; }
		public string BranchName { get; set; }
		public string CustomerCode { get; set; }
		public string CustomerName { get; set; }
		public string Description { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime InventorDate { get; set; }
		public string InventorCode { get; set; }
		public string InventorName { get; set; }

		public string StatusInventorCode { get; set; }
		public string StatusInventor { get; set; }

		public bool IsDirty { get; set; }
		public string DirtyInventorCode { get; set; }
		public string DirtyBranchCode { get; set; }

		public string StatusAuditConfig { get; set; }
		public string DBPath { get; set; }

		public AuditConfig()
		{
			this.CreateDate = DateTime.Now;
		}

		public AuditConfig(AuditConfig auditConfig)
		{
			this.Code = auditConfig.Code;
			this.Description = auditConfig.Description;
			this.BranchCode = auditConfig.BranchCode;
			this.BranchName = auditConfig.BranchName;
			this.CreateDate = auditConfig.CreateDate;
			this.CustomerCode = auditConfig.CustomerCode;
			this.CustomerName = auditConfig.CustomerName;
			this.InventorCode = auditConfig.InventorCode;
			this.InventorDate = auditConfig.InventorDate;
			this.InventorName = auditConfig.InventorName;
			this.IsDirty = auditConfig.IsDirty;
			this.DirtyBranchCode = auditConfig.DirtyBranchCode;
			this.DirtyInventorCode = auditConfig.DirtyInventorCode;
			this.StatusInventor = auditConfig.StatusInventor;
			this.StatusInventorCode = auditConfig.StatusInventorCode;
			this.StatusAuditConfig = auditConfig.StatusAuditConfig;
			this.DBPath = auditConfig.DBPath;
			
		}

		//public AuditConfig(AuditConfig inventor)
		//{
		//    this.Code = auditConfig.Code;
		//    this.Description = auditConfig.Description;
		//    this.BranchCode = auditConfig.BranchCode;
		//    this.BranchName = auditConfig.BranchName;
		//    this.CreateDate = auditConfig.CreateDate;
		//    this.CustomerCode = auditConfig.CustomerCode;
		//    this.CustomerName = auditConfig.CustomerName;
		//    this.InventorCode = auditConfig.InventorCode;
		//    this.InventorDate = auditConfig.InventorDate;
		//    this.InventorName = auditConfig.InventorName;
		//    this.IsDirty = auditConfig.IsDirty;
		//    this.DirtyBranchCode = auditConfig.DirtyBranchCode;
		//    this.DirtyInventorCode = auditConfig.DirtyInventorCode;
		//    this.StatusInventor = auditConfig.StatusInventor;
		//    this.StatusInventorCode = auditConfig.StatusInventorCode;
		//    this.StatusAuditConfig = auditConfig.StatusAuditConfig;
		//    this.DBPath = auditConfig.DBPath;

		//}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(AuditConfig)) return false;
			return Equals((AuditConfig)obj);
		}

		public bool Equals(AuditConfig other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return (Equals(other.CustomerCode, this.CustomerCode)
				&& Equals(other.BranchCode, this.BranchCode)
				&& Equals(other.InventorDate, this.InventorDate));
		}

		public override int GetHashCode()
		{
			return (Code != null ? Code.GetHashCode() : 0);				   //TODO:
		}


		
		public void Clear()
		{
			this.Description = "";
			this.BranchCode = "";
			this.BranchName = "";
			this.CustomerCode = "";
			this.CustomerName = "";
			this.InventorCode = "";
			this.InventorName = "";
			this.DirtyBranchCode = "";
			this.DBPath = "";
			this.DirtyInventorCode = "";
			this.StatusAuditConfig = "";
			this.StatusInventor = "";
			this.StatusInventorCode = "";
//			return this;
		}

		public void ClearBranch()
		{
			this.BranchCode = "";
			this.BranchName = "";
			this.InventorCode = "";
			this.InventorName = "";
			this.DirtyBranchCode = "";
			this.DirtyInventorCode = "";
			this.StatusAuditConfig = "";
			this.StatusInventor = "";
			this.StatusInventorCode = "";
//			return this;
		}

		public void ClearInventor()
		{
			this.InventorCode = "";
			this.InventorName = "";
			this.DirtyInventorCode = "";
			this.StatusAuditConfig = "";
			this.StatusInventor = "";
			this.StatusInventorCode = "";
//			return this;
		}


		
	}
		#endregion
}
 
