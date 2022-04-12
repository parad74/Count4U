using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
		None = 9 , 
		Temp = 10 ,
		Shift = 11
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
		//AnyDocIsNotApprove = 4,
		SomeDocIsNotApprove = 4,						//0 0 1 0 0
		//SomeDocIsApprove = 5,   AllDocIsApprove
		SomeDocIsApprove = 5,									//0 0 1 0 1
		DisableAndNoOneDoc = 16,							//1 0 0 0 0
		//None = 17,												//1 0 0 0 1
		DisableAndOneDocIsNotApprove = 18,		//1 0 0 1 1
		DisableAndOneDocIsApprove = 19,				//1 0 0 1 0
		DisableAndSomeDocIsNotApprove = 20,		//1 0 1 0 0
		//DisableAndAnyDocIsNotApprove = 20,		//1 0 1 0 0
		DisableAndSomeDocIsApprove = 21,				//1 0 1 0 1
		//DisableAndAllDocIsApprove = 21,				//1 0 1 0 1
		WarningConvert = 32								//1 0 0 0 0 0
	}

	public class IturStatusTitle
	{
		//Localization.Resources.Status_IturStatusTitle_NoOneDoc%"No One Document"
		public static string NoOneDoc = Localization.Resources.Status_IturStatusTitle_NoOneDoc;
		//Localization.Resources.Status_IturStatusTitle_None%"None"
		public static string None = Localization.Resources.Status_IturStatusTitle_None;
		//Localization.Resources.Status_IturStatusTitle_OneDocIsNotApprove%"One Document Is Not Approve"
		public static string OneDocIsNotApprove = Localization.Resources.Status_IturStatusTitle_OneDocIsNotApprove;
		//Localization.Resources.Status_IturStatusTitle_OneDocIsApprove%"One Document Is Approve"
		public static string OneDocIsApprove = Localization.Resources.Status_IturStatusTitle_OneDocIsApprove;
		//Localization.Resources.Status_IturStatusTitle_AnyDocIsNotApprove%"Some of Documents Is Not Approve"
		//public static string AnyDocIsNotApprove = Localization.Resources.Status_IturStatusTitle_AnyDocIsNotApprove;
		//Localization.Resources.Status_IturStatusTitle_AllDocIsApprove%"Some of Documents Is Approve"
		//public static string AllDocIsApprove = Localization.Resources.Status_IturStatusTitle_AllDocIsApprove;
		//Localization.Resources.Status_IturStatusTitle_SomeDocIsNotApprove%"Some of Documents Is Not Approve"
		public static string SomeDocIsNotApprove = Localization.Resources.Status_IturStatusTitle_SomeDocIsNotApprove;
		//Localization.Resources.Status_IturStatusTitle_SomeDocIsApprove%"Some of Documents Is Approve"
		public static string SomeDocIsApprove = Localization.Resources.Status_IturStatusTitle_SomeDocIsApprove;
		//Localization.Resources.Status_IturStatusTitle_DisableAndNoOneDoc%"Disable And No One Document"
		public static string DisableAndNoOneDoc = Localization.Resources.Status_IturStatusTitle_DisableAndNoOneDoc;
		//Localization.Resources.Status_IturStatusTitle_DisableAndOneDocIsNotApprove%"Disable And One Document Is Not Approve"
		public static string DisableAndOneDocIsNotApprove = Localization.Resources.Status_IturStatusTitle_DisableAndOneDocIsNotApprove;
		//Localization.Resources.Status_IturStatusTitle_DisableAndOneDocIsApprove%"Disable And One Document Is Approve"
		public static string DisableAndOneDocIsApprove = Localization.Resources.Status_IturStatusTitle_DisableAndOneDocIsApprove;
		//Localization.Resources.Status_IturStatusTitle_DisableAndAnyDocIsNotApprove%"Disable And Some of Documents Is Not Approve"
		//public static string DisableAndAnyDocIsNotApprove = Localization.Resources.Status_IturStatusTitle_DisableAndAnyDocIsNotApprove;
		//Localization.Resources.Status_IturStatusTitle_DisableAndAllDocIsApprove%"Disable And Some of Documents Is Approve"
		//public static string DisableAndAllDocIsApprove = Localization.Resources.Status_IturStatusTitle_DisableAndAllDocIsApprove;
		//Localization.Resources.Status_IturStatusTitle_DisableAndSomeDocIsNotApprove%"Disable And Some of Documents Is Not Approve"
		public static string DisableAndSomeDocIsNotApprove = Localization.Resources.Status_IturStatusTitle_DisableAndSomeDocIsNotApprove;
		//Localization.Resources.Status_IturStatusTitle_DisableAndSomeDocIsApprove%"Disable And Some of Documents Is Approve"
		public static string DisableAndSomeDocIsApprove = Localization.Resources.Status_IturStatusTitle_DisableAndSomeDocIsApprove;
		//Localization.Resources.Status_IturStatusTitle_WarningConvert%"Error"
		public static string WarningConvert = Localization.Resources.Status_IturStatusTitle_WarningConvert;
	}

	public static class IturDocValidate
	{
		public static string ConvertIturStatusBit2Message(int statusBit)
		{
			switch ((IturStatusEnum)statusBit)
			{
				case IturStatusEnum.NoOneDoc:
					return IturStatusTitle.NoOneDoc;
				case IturStatusEnum.OneDocIsNotApprove:
					return IturStatusTitle.OneDocIsNotApprove;
				case IturStatusEnum.OneDocIsApprove:
					return IturStatusTitle.OneDocIsApprove;
				case IturStatusEnum.SomeDocIsNotApprove:
					return IturStatusTitle.SomeDocIsNotApprove;
				case IturStatusEnum.SomeDocIsApprove:
					return IturStatusTitle.SomeDocIsApprove;
				case IturStatusEnum.DisableAndNoOneDoc:
					return IturStatusTitle.DisableAndNoOneDoc;
				case IturStatusEnum.DisableAndOneDocIsNotApprove:
					return IturStatusTitle.DisableAndOneDocIsNotApprove;
				case IturStatusEnum.DisableAndOneDocIsApprove:
					return IturStatusTitle.DisableAndOneDocIsApprove;
				case IturStatusEnum.DisableAndSomeDocIsNotApprove:
					return IturStatusTitle.DisableAndSomeDocIsNotApprove;
				case IturStatusEnum.DisableAndSomeDocIsApprove:
					return IturStatusTitle.DisableAndSomeDocIsApprove;
				case IturStatusEnum.WarningConvert:
					return IturStatusTitle.WarningConvert;
				}
			return "";
		}
	}

	public enum IturStatusGroupEnum
	{
		Empty = 0,
		OneDocIsApprove = 1,
		AllDocsIsApprove = 2,
		NotApprove = 3,
		DisableAndNoOneDoc = 4,
		DisableWithDocs = 5,
		Error = 6 , 
		None = 7
	}

	public class IturStatusGroupTitle
	{
		//Localization.Resources.Status_IturStatusGroupTitle_Empty%"Empty"
		public static string Empty = Localization.Resources.Status_IturStatusGroupTitle_Empty;
		//Localization.Resources.Status_IturStatusGroupTitle_OneDocIsApprove%"One Doc Is Approve"
		public static string OneDocIsApprove = Localization.Resources.Status_IturStatusGroupTitle_OneDocIsApprove;
		//Localization.Resources.Status_IturStatusGroupTitle_AllDocsIsApprove%"All Docs Are Approve"
		public static string AllDocsIsApprove = Localization.Resources.Status_IturStatusGroupTitle_AllDocsIsApprove;
		//Localization.Resources.Status_IturStatusGroupTitle_NotApprove%"Not Approve"
		public static string NotApprove = Localization.Resources.Status_IturStatusGroupTitle_NotApprove;
		//Localization.Resources.Status_IturStatusGroupTitle_DisableAndNoOneDoc%"Disable And No One Doc"
		public static string DisableAndNoOneDoc = Localization.Resources.Status_IturStatusGroupTitle_DisableAndNoOneDoc;
		//Localization.Resources.Status_IturStatusGroupTitle_DisableWithDocs%"Disable With Docs"
		public static string DisableWithDocs = Localization.Resources.Status_IturStatusGroupTitle_DisableWithDocs;
		//Localization.Resources.Status_IturStatusGroupTitle_Error%"Error"
		public static string Error = Localization.Resources.Status_IturStatusGroupTitle_Error;
		public static string None = Localization.Resources.Status_IturStatusGroupTitle_None;
		


	}

	public class IturStatusGroupString
	{
		//Localization.Resources.Status_IturStatusGroupTitle_Empty%"Empty"
		public static string Empty = "Empty";
		//Localization.Resources.Status_IturStatusGroupTitle_OneDocIsApprove%"One Doc Is Approve"
		public static string OneDocIsApprove = "One Doc Is Approve";
		//Localization.Resources.Status_IturStatusGroupTitle_AllDocsIsApprove%"All Docs Are Approve"
		public static string AllDocsIsApprove = "All Docs Are Approve";
		//Localization.Resources.Status_IturStatusGroupTitle_NotApprove%"Not Approve"
		public static string NotApprove = "Not Approve";
		//Localization.Resources.Status_IturStatusGroupTitle_DisableAndNoOneDoc%"Disable And No One Doc"
		public static string DisableAndNoOneDoc = "Disable And No One Doc";
		//Localization.Resources.Status_IturStatusGroupTitle_DisableWithDocs%"Disable With Docs"
		public static string DisableWithDocs = "Disable With Docs";
		//Localization.Resources.Status_IturStatusGroupTitle_Error%"Error"
		public static string Error = "Error";
		public static string None = "None";

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
		//Localization.Resources.Status_DocumentStatusTitle_NoOneDoc%"No One Document"
		public static string NoOneDoc = Localization.Resources.Status_DocumentStatusTitle_NoOneDoc;
		//Localization.Resources.Status_DocumentStatusTitle_None%"None"
		public static string None = Localization.Resources.Status_DocumentStatusTitle_None;
		//Localization.Resources.Status_DocumentStatusTitle_OneDocIsNotApprove%"One Document Is Not Approve"
		public static string OneDocIsNotApprove = Localization.Resources.Status_DocumentStatusTitle_OneDocIsNotApprove;
		//Localization.Resources.Status_DocumentStatusTitle_OneDocIsApprove%"One Document Is Approve"
		public static string OneDocIsApprove = Localization.Resources.Status_DocumentStatusTitle_OneDocIsApprove;
		//Localization.Resources.Status_DocumentStatusTitle_SomeDocsIsNotApprove%"Some Documents And This Is Not Approve"
		public static string SomeDocsIsNotApprove = Localization.Resources.Status_DocumentStatusTitle_SomeDocsIsNotApprove;
		//Localization.Resources.Status_DocumentStatusTitle_SomeDocIsApprove%"Some Documents And This Is Approve"
		public static string SomeDocIsApprove = Localization.Resources.Status_DocumentStatusTitle_SomeDocIsApprove;
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
		//Localization.Resources.Status_ConvertDataErrorMessage_InvalidValue%"Invalid Value in Data Row [{0}]"
		public static string InvalidValue = Localization.Resources.Status_ConvertDataErrorMessage_InvalidValue;
		//Localization.Resources.Status_ConvertDataErrorMessage_SameCodeExist%"Object with the Same Code [{0}] Exist in DB"
		public static string SameCodeExist = Localization.Resources.Status_ConvertDataErrorMessage_SameCodeExist;
		//Localization.Resources.Status_ConvertDataErrorMessage_FKCodeIsEmpty%"FK Code Is Empty in Data Row [{0}] "
		public static string FKCodeIsEmpty = Localization.Resources.Status_ConvertDataErrorMessage_FKCodeIsEmpty;
		//Localization.Resources.Status_ConvertDataErrorMessage_CodeIsEmpty%"Code Is Empty in Data Row [{0}] "
		public static string CodeIsEmpty = Localization.Resources.Status_ConvertDataErrorMessage_CodeIsEmpty;
	}
}
