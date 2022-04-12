using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Count4U;

namespace Count4U.Model
{
	public enum StatusInventorEnum
	{
		New = 1,
		Open = 2,
		Reopen = 3,
		Process = 4,
		Dirty = 5,
		Complete = 6,
		Send = 7,
		Exclude = 8, 
		None = 9
	}

	public enum StatusAuditConfigEnum
	{
		InProcess = 1,
		NotCurrent = 2
	}

	public enum SwitchObjectEnum
	{
		DocumentHeader = 1,			
		InventProduct = 2,
		Item = 3,
		None = 4

	}

	public enum CodeIturStatusEnum
	{
		NotApprove = 0,											//0 0 0 0 0
		Approve = 1,												//0 0 0 0 1
		OneDoc = 2,												//0 0 0 1 0
		SomeDocs = 4,											//0 0 1 0 0
		DisableItur = 16											//1 0 0 0 0
	}

	public enum IturStatusEnum
	{
		NoOneDoc = 0,											//0 0 0 0 0
		//None = 1,												//0 0 0 0 1
		OneDocIsNotApprove = 2,							//0 0 0 1 1
		OneDocIsApprove = 3,								//0 0 0 1 0
		AnyDocIsNotApprove = 4,							//0 0 1 0 0
		AllDocIsApprove = 5,									//0 0 1 0 1
		DisableAndNoOneDoc = 16,							//1 0 0 0 0
		//None = 17,												//1 0 0 0 1
		DisableAndOneDocIsNotApprove = 18,		//1 0 0 1 1
		DisableAndOneDocIsApprove = 19,				//1 0 0 1 0
		DisableAndAnyDocIsNotApprove = 20,		//1 0 1 0 0
		DisableAndAllDocIsApprove = 21,				//1 0 1 0 1
		WarningConvert = 64									//???
	}

	public class IturStatusTitle
	{
		public static string NoOneDoc = "No One Document";
		public static string None = "None";
		public static string OneDocIsNotApprove = "One Document Is Not Approve";
		public static string OneDocIsApprove = "One Document Is Approve";
		public static string AnyDocIsNotApprove = "Any of Documents Is Not Approve";
		public static string AllDocIsApprove = "All Documents Is Approve";
		public static string DisableAndNoOneDoc = "Disable And No One Document";
		public static string DisableAndOneDocIsNotApprove = "Disable And One Document Is Not Approve";
		public static string DisableAndOneDocIsApprove = "Disable And One Document Is Approve";
		public static string DisableAndAnyDocIsNotApprove = "Disable And Any of Documents Is Not Approve";
		public static string DisableAndAllDocIsApprove = "Disable And All Documents Is Approve";
	    public static string WarningConvert = "Error";
	}

	public enum IturStatusGroupEnum
	{
		Empty = 0,
		OneDocIsApprove = 1,
		AllDocsIsApprove = 2,
		NotApprove = 3,
		DisableAndNoOneDoc = 4,
		DisableWithDocs = 5,
		Error = 6
	}

	public class IturStatusGroupTitle
	{
		public static string Empty = "Empty";
		public static string OneDocIsApprove = "One Doc Is Approve";
		public static string AllDocsIsApprove = "All Docs Are Approve";
		public static string NotApprove = "Not Approve";
		public static string DisableAndNoOneDoc = "Disable And No One Doc";
		public static string DisableWithDocs = "Disable With Docs";
		public static string Error = "Error";
	}

	public enum DocumentStatusEnum
	{
		NoOneDoc = 0,							// 0 0 0
		None = 1,									// 0 0 1
		OneDocIsNotApprove = 2,			// 0 1 1
		OneDocIsApprove = 3,				// 0 1 0
		SomeDocsIsNotApprove = 4,		// 1 0 0
		SomeDocIsApprove = 5				// 1 0 1
	}

	public class DocumentStatusTitle
	{
		public static string NoOneDoc = "No One Document";
		public static string None = "None";
		public static string OneDocIsNotApprove = "One Document Is Not Approve";
		public static string OneDocIsApprove = "One Document Is Approve";
		public static string SomeDocsIsNotApprove = "Some Documents And This Is Not Approve";
		public static string SomeDocIsApprove = "Some Documents And This Is Approve";
	}

	public enum ConvertDataErrorCodeEnum
	{
		NoError = 0,												//0 0 0 0 0 0 0 0 0 0 
		InvalidValue = 64,										 //0 0 0 1 0 0 0 0 0 0
		SameCodeExist = 128,								//0 0 1 0 0 0 0 0 0 0
		FKCodeIsEmpty = 256,								//0 1 0 0 0 0 0 0 0 0
		CodeIsEmpty = 512									//1 0 0 0 0 0 0 0 0 0
	}

	public static class ConvertDataErrorMessage
	{
		public static string InvalidValue = "Invalid Value in Data Row [ {0} ]";
		public static string SameCodeExist = "Object with the Same Code [ {0} ] Exist in DB";
		public static string FKCodeIsEmpty = " FK Code Is Empty in Data Row [ {0} ] ";
		public static string CodeIsEmpty = "Code Is Empty in Data Row [ {0} ] ";
	}
}
