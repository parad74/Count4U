using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Count4Mobile;
using Count4U.Model.Common;

namespace Count4U.Model.Count4U
{
	public class ExportCurrentInventoryNativPlusYesXslxWriter2 : IExportCurrentInventoryWriter
	{

		public void AddRow(StreamWriter sw, CurrentInventoryAdvanced currentInventory, long countRow, string ERPNum = "",
			string INVDate = "",
			string separator = "^", Dictionary<ImportProviderParmEnum, object> parms = null, List<PreviousInventory> details = null)
		{
			string param = parms.GetStringValueFromParm(ImportProviderParmEnum.SourcePath);

			string[] newRows1 = new string[24];
			newRows1[0] = currentInventory.LocationCode;//	"	LocationCode	"
			newRows1[1] = currentInventory.LocationLevel1Code;//	"	Location.Level1.Code	"
			newRows1[2] = currentInventory.LocationLevel2Code;//	"	Location.Level2.Code	"
			newRows1[3] = currentInventory.LocationLevel3Code;//	"	Location.Level3.Code	"
			newRows1[4] = currentInventory.LocationLevel4Code;//	"	Location.Level4.Code	"
			newRows1[5] = currentInventory.ItemCode;//	"	 ItemCode	"
			newRows1[6] = currentInventory.LocationLevel1Name;//	"	Location.Level1.Name	"
			newRows1[7] = currentInventory.LocationLevel2Name;//	"	Location.Level2.Name	"
			newRows1[8] = currentInventory.LocationLevel3Name;//	"	Location.Level3.Name	"
			newRows1[9] = currentInventory.LocationLevel4Name;//	"	Location.Level4.Name	"
			newRows1[10] = currentInventory.CatalogItemName;//	"	 ItemName	"
			newRows1[11] = currentInventory.Quantity;//	"	Quantity	"
			newRows1[12] = currentInventory.SerialNumberLocal;//	"	SerialNumberLocal	"
			newRows1[13] = currentInventory.PropertyStr1Name;//	"	PropertyStr1	"
			newRows1[14] = currentInventory.PropertyStr4Name;//	"	PropertyStr4"
			newRows1[15] = currentInventory.PropertyStr2Name;//	"	PropertyStr2"
			newRows1[16] = currentInventory.SerialNumberSupplier;//	"	SerialNumberSupplier	"
			newRows1[17] = currentInventory.PropertyStr3Name;//	"	PropertyStr3"
			newRows1[18] = currentInventory.PropertyStr18Name;//	"	PropertyStr18"
			newRows1[19] = currentInventory.PropertyStr19Name;//	"	PropertyStr19"
			newRows1[20] = currentInventory.PropertyStr20Name;//	"	PropertyStr20"
			newRows1[21] = currentInventory.PropertyStr2;//	"	CreatedDate	"
			newRows1[22] = currentInventory.PropertyStr1;//	"	modifiedDate	"

			if (string.IsNullOrWhiteSpace(currentInventory.PropertyStr3) == false)		//	  Link of Photo
			{
				newRows1[23] = ERPNum + currentInventory.PropertyStr3;
			}

			string newRow = string.Join(separator, newRows1);
			sw.WriteLine(newRow);
		}

		public void AddRowSimple(StreamWriter sw, CurrentInventory currentInventory, long countRow,
			string ERPNum = "", string INVDate = "", string separator = ",", Dictionary<ImportProviderParmEnum, object> parms = null, List<PreviousInventory> details = null)
		{
		
		}

		public void AddHeader(StreamWriter sw, string ERPNum = "", string INVDate = "", string separator = "^",
			Dictionary<ImportProviderParmEnum, object> parms = null, object argument = null)
		{
			//string uid = propertyStr.TypeCode + "|" + propertyStr.PropertyStrCode + "|" + propertyStr.Code;
			Dictionary<Pair<string, string, string>, PropertyStr> dictionary = argument as Dictionary<Pair<string, string, string>, PropertyStr>;
			//row 0
			string[] newRows = new string[24];
			newRows[0] = @"	קוד מיקום	"; //	"	LocationCode	"
			newRows[1] = @"	מיקום1-קוד	"; //	"	Location.Level1.Code	"
			newRows[2] = @"	מיקום2-קוד	"; //	"	Location.Level2.Code	"
			newRows[3] = @"	מיקום3-קוד	"; //	"	Location.Level3.Code	"
			newRows[4] = @"	מיקום4-קוד	"; //	"	Location.Level4.Code	"
			newRows[5] = @"	מק''ט	"; //	"	 ItemCode	"
			newRows[6] = @"	מיקום1-שם	"; //	"	Location.Level1.Name	"
			newRows[7] = @"	מיקום2-שם	"; //	"	Location.Level2.Name	"
			newRows[8] = @"	מיקום3-שם	"; //	"	Location.Level3.Name	"
			newRows[9] = @"	מיקום4-שם	"; //	"	Location.Level4.Name	"
			newRows[10] = @"	תיאור פריט	"; //	"	 ItemName	"
			newRows[11] = @"	כמות שנספרה	"; //	"	Quantity	"
			newRows[12] = @"	מספר טבוע מקומי	"; //	"	SerialNumberLocal	"
			newRows[13] = @"	יצרן	"; //	"	PropertyStr1	"
			newRows[14] = @"	מודל	"; //	"	PropertyStr4"
			newRows[15] = @"	דגם	"; //	"	PropertyStr2"
			newRows[16] = @"	מספר טבוע יצרן	"; //	"	SerialNumberSupplier	"
			newRows[17] = @"	הערה	"; //	"	PropertyStr3"
			newRows[18] = @"	שם משתמש	"; //	"	PropertyStr18"
			newRows[19] = @"	קוד מכשיר	"; //	"	PropertyStr19"
			newRows[20] = @"	שדה נוסף 20	"; //	"	PropertyStr20"
			newRows[21] = @"	תאריך הקמה	"; //	"	CreatedDate	"
			newRows[22] = @"	תאריך עדכון	"; //	"	modifiedDate	"
			newRows[23] = @"	נתיב לתמונה	"; //	  Link of Photo


			for (int i = 0; i < newRows.Count(); i++)
			{
				//DocumentSheet, Row , 	  Col
				var futureKey = new Pair<string, string, string>("2", "1", (i + 1).ToString());
				if (dictionary.ContainsKey(futureKey) == true)
				{
					string title = dictionary[futureKey].Name;
					newRows[i] = title;
				}
			}

			string newRow = string.Join(separator, newRows);
			sw.WriteLine(newRow);

			//row 1
			string[] newRows1 = new string[24];
			newRows1[0] = @"LocationCode	";//
			newRows1[1] = @"	Location.Level1.Code	";//
			newRows1[2] = @"Location.Level2.Code	";//
			newRows1[3] = @"Location.Level3.Code	";//
			newRows1[4] = @"Location.Level4.Code	";//
			newRows1[5] = @"ItemCode	";//
			newRows1[6] = @"Location.Level1.Name	";//
			newRows1[7] = @"Location.Level2.Name	";//
			newRows1[8] = @"Location.Level3.Name	";//
			newRows1[9] = @"Location.Level4.Name	";//
			newRows1[10] = @"ItemName	";//
			newRows1[11] = @"Quantity	";//
			newRows1[12] = @"SerialNumberLocal	";//
			newRows1[13] = @"PropertyStr1	";//
			newRows1[14] = @"PropertyStr4";//
			newRows1[15] = @"PropertyStr2";//
			newRows1[16] = @"SerialNumberSupplier";//
			newRows1[17] = @"PropertyStr3";//
			newRows1[18] = @"PropertyStr18";//
			newRows1[19] = @"PropertyStr19";//
			newRows1[20] = @"PropertyStr20";//
			newRows1[21] = @"CreatedDate	";//
			newRows1[22] = @"ModifiedDate";//
			newRows1[23] = @"Link of Photo	";//


			string newRow1 = string.Join(separator, newRows1);
			sw.WriteLine(newRow1);
		}

		public void AddHeaderSum(StreamWriter sw, IEnumerable<CurrentInventory> currentInventoryList = null, string ERPNum = "", string INVDate = "", string separator = "," , Dictionary<ImportProviderParmEnum, object> parms = null)
		{
				// заголовки 
			//string[] newRows = new string[] { "Item Code", "Item Name", "Unit Price", 
			//	 "Quantity In Pack",	 
			//	 "Total Packs Counted", "Total Units Counted",   "Units Quantity in ERP", "Quantity Difference",
			//	"Inventory Value", "ERP Value",  "Value Difference ERP"};
		

		}
		public void AddFooter(StreamWriter sw, long countRow, string ERPNum = "", string INVDate = "", string separator = ",", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
		}

		public void AddFooterSum(StreamWriter sw, long countRow, IEnumerable<CurrentInventory> currentInventoryList = null, string ERPNum = "", string INVDate = "", string separator = ",", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
		}

		//newRows[5] = "F=RC[-2]*RC[-1]";
	}
}
