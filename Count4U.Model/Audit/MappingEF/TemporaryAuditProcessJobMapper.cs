using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.Audit.MappingEF
{
	public static class TemporaryAuditProcessJobMapper
	{

		public static ProcessJob ToTemporaryAuditDomainObject(this App_Data.TemporaryAuditProcessJob entity)
		{
			if (entity == null) return null;
			return new ProcessJob()
			{
				ID = entity.ID,
				ProcessJobCode = entity.ProcessJobCode != null ? entity.ProcessJobCode : "",
				ProcessCode = entity.ProcessCode != null ? entity.ProcessCode : "",
				SyncCode = entity.SyncCode != null ? entity.SyncCode : "",
				PoolCode = entity.PoolCode != null ? entity.PoolCode : "",
				ParentProcessCode = entity.ParentProcessCode != null ? entity.ParentProcessCode : "",
				FirstProcessCode = entity.FirstProcessCode != null ? entity.FirstProcessCode : "",
				NextProcessCode = entity.NextProcessCode != null ? entity.NextProcessCode : "",
				PrevProcessCode = entity.PrevProcessCode != null ? entity.PrevProcessCode : "",
				LastProcessCode = entity.LastProcessCode != null ? entity.LastProcessCode : "",
				DomainType = entity.DomainType != null ? entity.DomainType : "",
				NN = entity.NN != null ? Convert.ToInt32(entity.NN) : 0,
				Description = entity.Description != null ? entity.Description : "",
				JobTypeCode = entity.JobTypeCode != null ? entity.JobTypeCode : "",
				JobServiceCode = entity.JobServiceCode != null ? entity.JobServiceCode : "",
				StatusCode = entity.StatusCode != null ? entity.StatusCode : "",

				CreateDate = entity.CreateDate,
				GetDate = entity.GetDate,
				ResentDate = entity.ResentDate,
				StartDate = entity.StartDate,
				FinishDate = entity.FinishDate,
				ClosedDate = entity.ClosedDate,
				ModifiedDate = entity.ModifiedDate,

				Owner = entity.Owner != null ? entity.Owner : "",
				Device = entity.Device != null ? entity.Device : "",
				DbFileName = entity.DbFileName != null ? entity.DbFileName : "",
				Tag = entity.Tag != null ? entity.Tag : "",
				Tag1 = entity.Tag1 != null ? entity.Tag1 : "",
				Tag2 = entity.Tag2 != null ? entity.Tag2 : "",
				Tag3 = entity.Tag3 != null ? entity.Tag3 : "",
				TagIP = entity.TagIP != null ? entity.TagIP : "",
				TagHost = entity.TagHost != null ? entity.TagHost : "",
				Operation = entity.Operation != null ? entity.Operation : "",
				OperationResult = entity.OperationResult != null ? entity.OperationResult : "",
				ContextCBI = entity.ContextCBI != null ? entity.ContextCBI : "",
				CurrentAuditConfigCode = entity.CurrentAuditConfigCode != null ? entity.CurrentAuditConfigCode : "",
				CurrentCBIObjectType = entity.CurrentCBIObjectType != null ? entity.CurrentCBIObjectType : "",
				CurrentCBIObjectCode = entity.CurrentCBIObjectCode != null ? entity.CurrentCBIObjectCode : "",
				CurrentCustomerCode = entity.CurrentCustomerCode != null ? entity.CurrentCustomerCode : "",
				CurrentBranchCode = entity.CurrentBranchCode != null ? entity.CurrentBranchCode : "",
				CurrentInventorCode = entity.CurrentInventorCode != null ? entity.CurrentInventorCode : "",
			};
		}

		public static ProcessJob ToTemporaryAuditSimpleDomainObject(this App_Data.TemporaryAuditProcessJob entity)
		{
			throw new NotImplementedException();
		}

		public static App_Data.TemporaryAuditProcessJob ToTemporaryAuditEntity(this ProcessJob domainObject)
		{
			if (domainObject == null) return null;
			return new App_Data.TemporaryAuditProcessJob()
			{
				ID = domainObject.ID,
				ProcessJobCode = domainObject.ProcessJobCode,
				ProcessCode = domainObject.ProcessCode,
				SyncCode = domainObject.SyncCode,
				PoolCode = domainObject.PoolCode,
				ParentProcessCode = domainObject.ParentProcessCode,
				FirstProcessCode = domainObject.FirstProcessCode,
				NextProcessCode = domainObject.NextProcessCode,
				PrevProcessCode = domainObject.PrevProcessCode,
				LastProcessCode = domainObject.LastProcessCode,
				DomainType = domainObject.DomainType,
				NN = domainObject.NN,
				Description = domainObject.Description,
				JobTypeCode = domainObject.JobTypeCode,
				JobServiceCode = domainObject.JobServiceCode,
				StatusCode = domainObject.StatusCode,

				CreateDate = domainObject.CreateDate,
				GetDate = domainObject.GetDate,
				ResentDate = domainObject.ResentDate,
				StartDate = domainObject.StartDate,
				FinishDate = domainObject.FinishDate,
				ClosedDate = domainObject.ClosedDate,
				ModifiedDate = domainObject.ModifiedDate,

				Owner = domainObject.Owner,
				Device = domainObject.Device,
				DbFileName = domainObject.DbFileName,
				Tag = domainObject.Tag,
				Tag1 = domainObject.Tag1,
				Tag2 = domainObject.Tag2,
				Tag3 = domainObject.Tag3,
				TagIP = domainObject.TagIP,
				TagHost = domainObject.TagHost,
				Operation = domainObject.Operation,
				OperationResult = domainObject.OperationResult,
				ContextCBI = domainObject.ContextCBI,
				CurrentAuditConfigCode = domainObject.CurrentAuditConfigCode,
				CurrentCBIObjectType = domainObject.CurrentCBIObjectType,
				CurrentCBIObjectCode = domainObject.CurrentCBIObjectCode,
				CurrentCustomerCode = domainObject.CurrentCustomerCode,
				CurrentBranchCode = domainObject.CurrentBranchCode,
				CurrentInventorCode = domainObject.CurrentInventorCode,

			};
		}


		public static void ApplyTemporaryAuditChanges(this App_Data.TemporaryAuditProcessJob entity, ProcessJob domainObject)
		{
			if (domainObject == null) return;
			entity.ID = domainObject.ID;
			entity.ProcessJobCode = domainObject.ProcessJobCode;
			entity.ProcessCode = domainObject.ProcessCode;
			entity.SyncCode = domainObject.SyncCode;
			entity.PoolCode = domainObject.PoolCode;
			entity.ParentProcessCode = domainObject.ParentProcessCode;
			entity.FirstProcessCode = domainObject.FirstProcessCode;
			entity.NextProcessCode = domainObject.NextProcessCode;
			entity.PrevProcessCode = domainObject.PrevProcessCode;
			entity.LastProcessCode = domainObject.LastProcessCode;
			entity.DomainType = domainObject.DomainType;
			entity.NN = domainObject.NN;
			entity.Description = domainObject.Description;
			entity.JobTypeCode = domainObject.JobTypeCode;
			entity.JobServiceCode = domainObject.JobServiceCode;
			entity.StatusCode = domainObject.StatusCode;

			entity.CreateDate = domainObject.CreateDate;
			entity.GetDate = domainObject.GetDate;
			entity.ResentDate = domainObject.ResentDate;
			entity.StartDate = domainObject.StartDate;
			entity.FinishDate = domainObject.FinishDate;
			entity.ClosedDate = domainObject.ClosedDate;
			entity.ModifiedDate = domainObject.ModifiedDate;

			entity.Owner = domainObject.Owner;
			entity.Device = domainObject.Device;
			entity.DbFileName = domainObject.DbFileName;
			entity.Tag = domainObject.Tag;
			entity.Tag1 = domainObject.Tag1;
			entity.Tag2 = domainObject.Tag2;
			entity.Tag3 = domainObject.Tag3;
			entity.TagIP = domainObject.TagIP;
			entity.TagHost = domainObject.TagHost;
			entity.Operation = domainObject.Operation;
			entity.OperationResult = domainObject.OperationResult;
			entity.ContextCBI = domainObject.ContextCBI;
			entity.CurrentAuditConfigCode = domainObject.CurrentAuditConfigCode;
			entity.CurrentCBIObjectType = domainObject.CurrentCBIObjectType;
			entity.CurrentCBIObjectCode = domainObject.CurrentCBIObjectCode;
			entity.CurrentCustomerCode = domainObject.CurrentCustomerCode;
			entity.CurrentBranchCode = domainObject.CurrentBranchCode;
			entity.CurrentInventorCode = domainObject.CurrentInventorCode;
		}
	}
}
