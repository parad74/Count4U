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
	public class ExportCurrentInventoryNativPlusMisradApnimXslxWriter : IExportCurrentInventoryWriter
	{

		public void AddRow(StreamWriter sw, CurrentInventoryAdvanced currentInventory, long countRow, string ERPNum = "",
			string INVDate = "",
			string separator = "^", Dictionary<ImportProviderParmEnum, object> parms = null, List<PreviousInventory> details = null)
		{
			string param = parms.GetStringValueFromParm(ImportProviderParmEnum.SourcePath);

			string[] newRows1 = new string[17];
			newRows1[0] = currentInventory.LocationCode;//	"	LocationCode	"
			newRows1[1] = currentInventory.CatalogItemName;//	"	ItemName	"
			newRows1[2] = currentInventory.SerialNumberSupplier;//	"	SerialNumberSupplier	"
			newRows1[3] = currentInventory.SerialNumberLocal;//	"	SerialNumberLocal	"
			newRows1[4] = currentInventory.PropertyStr7Name;//	PropertyStr7
			newRows1[5] = currentInventory.PropertyStr6Name;//	PropertyStr6
			newRows1[6] = currentInventory.PropertyStr8Name;//	PropertyStr8
			newRows1[7] = currentInventory.PropertyStr9Name;//	PropertyStr9
			newRows1[8] = currentInventory.LocationLevel2Code;//	"	Location.Level2.Code	"
			newRows1[9] = currentInventory.PropertyStr3Name;//	PropertyStr3
			newRows1[10] = currentInventory.PropertyStr5Name;//	PropertyStr5
			newRows1[11] = currentInventory.PropertyStr4Name;//	PropertyStr4
			newRows1[12] = currentInventory.PropertyStr14Name;//	PropertyStr14
			newRows1[13] = currentInventory.Quantity;//	"	Quantity	"
			newRows1[14] = currentInventory.PropertyStr2Name;//	PropertyStr2
			newRows1[15] = currentInventory.PropertyStr1Name;//	PropertyStr1
			newRows1[16] = currentInventory.PropertyStr15Name;//	PropertyStr15
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
			Dictionary<Pair<string, string, string>, PropertyStr> dictionary = argument as Dictionary<Pair<string, string, string>, PropertyStr>;

			string[] newRows = new string[37];
			newRows[0] = @"	קוד מיקום	";//	"	LocationCode	"
			newRows[1] = @"	תיאור פריט	";//	"	ItemName	"
			newRows[2] = @"	סיריאלי יצרן	";//	"	SerialNumberSupplier	"
			newRows[3] = @"	מספר מרכבה	";//	"	SerialNumberLocal	"
			newRows[4] = @"	שם מחשב	";//	PropertyStr7
			newRows[5] = @"	משוייך לציוד	";//	PropertyStr6
			newRows[6] = @"	קורא כרטיס אשראי	";//	PropertyStr8
			newRows[7] = @"	קורא כרטיס עובד	";//	PropertyStr9
			newRows[8] = @"	יחידה אירגונית	";//	"	Location.Level2.Code	"
			newRows[9] = @"	קומה	";//	PropertyStr3
			newRows[10] = @"	שם חדר / עמדה	";//	PropertyStr5
			newRows[11] = @"	סטטוס	";//	PropertyStr4
			newRows[12] = @"	כתובת IP	";//	PropertyStr14
			newRows[13] = @"	כמות שנספרה	";//	"	Quantity	"
			newRows[14] = @"	דגם	";//	PropertyStr2
			newRows[15] = @"	יצרן	";//	PropertyStr1
			newRows[16] = @"	CLASS	";//	PropertyStr15


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


			string[] newRows1 = new string[17];
			newRows1[0] = @"LocationCode";//
			newRows1[1] = @"	ItemName";//
			newRows1[2] = @"SerialNumberSupplier	";//
			newRows1[3] = @"SerialNumberLocal";//
			newRows1[4] = @"PropertyStr7	";//
			newRows1[5] = @"PropertyStr6	";//
			newRows1[6] = @"PropertyStr8	";//
			newRows1[7] = @"PropertyStr9	";//
			newRows1[8] = @"Location.Level2.Code	";//
			newRows1[9] = @"PropertyStr3	";//
			newRows1[10] = @"PropertyStr5	";//
			newRows1[11] = @"PropertyStr4	";//
			newRows1[12] = @"PropertyStr14	";//
			newRows1[13] = @"Quantity";//
			newRows1[14] = @"PropertyStr2	";//
			newRows1[15] = @"PropertyStr1	";//
			newRows1[16] = @"PropertyStr15	";//


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
