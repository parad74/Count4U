using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Count4Mobile;

namespace Count4U.Model.Count4U
{
	public class ExportCurrentInventoryNativPlusLadpcXslxWriter2 : IExportCurrentInventoryWriter
	{

		public void AddRow(StreamWriter sw, CurrentInventoryAdvanced currentInventory, long countRow, string ERPNum = "",
			string INVDate = "",
			string separator = "^", Dictionary<ImportProviderParmEnum, object> parms = null, List<PreviousInventory> details = null)
		{

			string[] newRows1 = new string[18];

			newRows1[0] = currentInventory.PropertyStr8Code;//@"PropertyStr8";//
			newRows1[1] = currentInventory.PropertyStr15Code;//@"PropertyStr15";//
			newRows1[2] = currentInventory.LocationLevel1Code;//@"Location.Level1.Code";//
			newRows1[3] = currentInventory.LocationLevel1Name;//@"Location.Level1.Name";//
			newRows1[4] = currentInventory.ItemCode;//@"ItemCode";//
			newRows1[5] = currentInventory.CatalogItemName;//@"ItemName";//
			newRows1[6] = currentInventory.SerialNumberLocal;//@"SerialNumberLocal";//
			newRows1[7] = currentInventory.SerialNumberSupplier;//@"SerialNumberSupplier";//
			newRows1[8] = currentInventory.Quantity;//@"Quantity";//
			newRows1[9] = currentInventory.PropertyStr7Code;//@"PropertyStr7";//
			newRows1[10] = currentInventory.PropertyStr9Code;//@"PropertyStr9";//
			newRows1[11] = currentInventory.PropertyStr10Code;//@"PropertyStr10";//
			newRows1[12] = currentInventory.PropertyStr18Code;//@"PropertyStr18";//

			newRows1[13] = currentInventory.PropertyStr1; //@"modifiedDate";//
			newRows1[14] = currentInventory.LocationLevel1Code;//@"Location.Level1.Code";//
			newRows1[15] = currentInventory.LocationLevel2Code;//@"Location.Level2.Code";//
			newRows1[16] = currentInventory.LocationLevel2Name;//@"Location.Level2.Name";//
			newRows1[17] = currentInventory.PropertyStr12Code;//@"PropertyStr12	";//



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
			//row 0
			string[] newRows = new string[18];
			newRows[0] = @"	שנה	";//
			newRows[1] = @"	רשות	";//
			newRows[2] = @"	מחסן	";//
			newRows[3] = @"	מיקום1-שם	";//
			newRows[4] = @"	מק''ט	";//
			newRows[5] = @"	תיאור פריט	";//
			newRows[6] = @"	מספר טבוע מקומי	";//
			newRows[7] = @"	מספר טבוע יצרן	";//
			newRows[8] = @"	כמות שנספרה	";//
			newRows[9] = @"	סיום אחריות";//
			newRows[10] = @"	מחיר	";//
			newRows[11] = @"	הערות	";//
			newRows[12] = @"	שם הסופר	";//
			newRows[13] = @"	תאריך עדכון	";//
			newRows[14] = @"	מיקום1-קוד	";//
			newRows[15] = @"	מיקום2-קוד	";//
			newRows[16] = @"	מיקום2-שם	";//
			newRows[17] = @"	לינק לתמונה	";//


			string newRow = string.Join(separator, newRows);
			sw.WriteLine(newRow);

			//row 1
			string[] newRows1 = new string[18];
			newRows1[0] = @"PropertyStr8";//
			newRows1[1] = @"PropertyStr15";//
			newRows1[2] = @"Location.Level1.Code";//
			newRows1[3] = @"Location.Level1.Name";//
			newRows1[4] = @"ItemCode";//
			newRows1[5] = @"ItemName";//
			newRows1[6] = @"SerialNumberLocal";//
			newRows1[7] = @"SerialNumberSupplier";//
			newRows1[8] = @"Quantity";//
			newRows1[9] = @"PropertyStr7";//
			newRows1[10] = @"PropertyStr9";//
			newRows1[11] = @"PropertyStr10";//
			newRows1[12] = @"PropertyStr18";//
			newRows1[13] = @"modifiedDate";//
			newRows1[14] = @"Location.Level1.Code";//
			newRows1[15] = @"Location.Level2.Code";//
			newRows1[16] = @"Location.Level2.Name";//
			newRows1[17] = @"PropertyStr12	";//



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
