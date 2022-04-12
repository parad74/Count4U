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

	public class ExportCurrentInventoryNativPlusMateAsherXslxWriter1 : IExportCurrentInventoryWriter
	{

		public void AddRow(StreamWriter sw, CurrentInventoryAdvanced currentInventory, long countRow, string ERPNum = "",
			string INVDate = "",
			string separator = "^", Dictionary<ImportProviderParmEnum, object> parms = null, List<PreviousInventory> details = null)
		{
			string param = parms.GetStringValueFromParm(ImportProviderParmEnum.SourcePath);
			string[] newRows1 = new string[12];
			newRows1[0] = currentInventory.ItemCode;//	"	 ItemCode	"
			newRows1[1] =  currentInventory.CatalogItemName;//	"	 ItemName	"
			newRows1[2] = currentInventory.SerialNumberLocal;//	"	SerialNumberLocal	"
			newRows1[3] = currentInventory.SerialNumberSupplier;//	"	SerialNumberSupplier	"
			newRows1[4] = currentInventory.Quantity;//	"	Quantity	"
			newRows1[5] = currentInventory.PropertyStr1Name;//	"	PropertyStr1	"
			newRows1[6] = currentInventory.PropertyStr2;//	"	CreatedDate	"
			newRows1[7] =  currentInventory.PropertyStr1;//	"	modifiedDate	"
			newRows1[8] = currentInventory.LocationLevel2Code;//	"	Location.Level2.Code	"
			newRows1[9] = currentInventory.LocationLevel2Name;//	"	Location.Level2.Name	"
			newRows1[10] = currentInventory.LocationLevel3Code;//	"	Location.Level3.Code	"
			newRows1[11] = currentInventory.LocationLevel3Name;//	"	Location.Level3.Name	"
		
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

			string[] newRows = new string[12];

			newRows[0] = @"	מק''ט	";//	"	 ItemCode	"
			newRows[1] = @"תיאור פריט	";//	"	 ItemName	"
			newRows[2] = @"מספר טבוע מקומי	";//	"	SerialNumberLocal	"
			newRows[3] = @"מספר טבוע יצרן	";//	"	SerialNumberSupplier	"
			newRows[4] = @"כמות שנספרה	";//	"	Quantity	"
			newRows[5] = @"הערות	";//	"	PropertyStr1	"
			newRows[6] = @"תאריך הקמה		";//	"	CreatedDate	"
			newRows[7] = @"תאריך עדכון	";//	"	modifiedDate	"
			newRows[8] = @"קוד מחסן	";//	"	Location.Level2.Code	"
			newRows[9] = @"שם המחסן	";//	"	Location.Level2.Name	"
			newRows[10] = @"קוד תת מחסן	";//	"	Location.Level3.Code	"
			newRows[11] = @"שם תת מחסן	";//	"	Location.Level3.Name	"


			//string uid = propertyStr.TypeCode + "|" + propertyStr.PropertyStrCode + "|" + propertyStr.Code;
			//DocumentSheet 	  //TypeCode						0
			// Row 					 //PropertyStrCode 			1
			//Col 						//Code								    2
			//Name 					//значение 							3

			for (int i = 0; i < newRows.Count(); i++)
			{
				//DocumentSheet, Row , 	  Col
				var futureKey = new Pair<string, string, string>("1", "1", (i + 1).ToString());
				if (dictionary.ContainsKey(futureKey) == true)
				{
					string title = dictionary[futureKey].Name;
					newRows[i] = title;
				}
			}

			string newRow = string.Join(separator, newRows);
			sw.WriteLine(newRow);

			//row 1
			string[] newRows1 = new string[12];
			newRows1[0] = @"ItemCode";//	"	מק''ט	"
			newRows1[1] = @"ItemName";//	"	תיאור פריט	"
			newRows1[2] = @"SerialNumberLocal";//	"	מספר טבוע מקומי	"
			newRows1[3] = @"SerialNumberSupplier";//	"	מספר טבוע יצרן	"
			newRows1[4] = @"Quantity";//	"	כמות שנספרה	"
			newRows1[5] = @"PropertyStr1	";//	הערות
			newRows1[6] = @"CreatedDate	";//	"	תאריך הקמה		"
			newRows1[7] = @"modifiedDate";//	"	תאריך עדכון	"
			newRows1[8] = @"Location.Level2.Code	";//	"	קוד מחסן	"
			newRows1[9] = @"Location.Level2.Name";//	שם המחסן
			newRows1[10] = @"Location.Level3.Code";//	קוד תת מחסן
			newRows1[11] = @"Location.Level3.Name";//	שם תת מחסן


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
