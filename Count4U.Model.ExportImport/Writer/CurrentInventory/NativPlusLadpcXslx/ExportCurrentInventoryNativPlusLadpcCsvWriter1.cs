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
	public class ExportCurrentInventoryNativPlusLadpcCsvWriter1 : IExportCurrentInventoryWriter
	{

		public void AddRow(StreamWriter sw, CurrentInventoryAdvanced currentInventory, long countRow, string ERPNum = "", 
			string INVDate = "",
			string separator = "^", Dictionary<ImportProviderParmEnum, object> parms = null, List<PreviousInventory> details = null)
		{
			string param = parms.GetStringValueFromParm(ImportProviderParmEnum.SourcePath);
			string[] newRows1 = new string[9];
			newRows1[0] = currentInventory.PropertyStr8Code;//@"PropertyStr8";//	A
			newRows1[1] = currentInventory.PropertyStr4Code;//@"	PropertyStr4";//	B
			newRows1[2] = currentInventory.LocationLevel1Code;//@"Location.Level1.Code";//	C
			newRows1[3] = currentInventory.LocationLevel1Name;//@"Location.Level1.Name";//	D
			newRows1[4] = currentInventory.ItemCode;//@"ItemCode";//	E
			newRows1[5] = currentInventory.CatalogItemName;//@"ItemName";//	F
			newRows1[6] = currentInventory.SerialNumberLocal;//@"SerialNumberLocal";//	F
			newRows1[7] = currentInventory.Quantity;//@"Quantity";//	G
			newRows1[8] = currentInventory.PropertyStr9Code;//@"PropertyStr9";//	H

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
			string[] newRows = new string[9];
			newRows[0] = @"	שנה	";	//  PropertyStr8	A
			newRows[1] = @"רשות";	//  PropertyStr4	B
			newRows[2] = @"מחסן";	//  Location.Level1.Code	C
			newRows[3] = @"שם מחסן";	//  	Location.Level1.Name	D
			newRows[4] = @"מק''ט";	//  	ItemCode	E
			newRows[5] = @"תיאור פריט	";	//  	ItemName	F
			newRows[6] = @"	מספר טבוע מקומי";	//  	SerialNumberLocal	G
			newRows[7] = @"כמות שנספרה	";	//  	Quantity	G
			newRows[8] = @"מחיר	";	//  PropertyStr9	H

			string newRow = string.Join(separator, newRows);
			sw.WriteLine(newRow);

			//row 1
			string[] newRows1 = new string[9];
			newRows1[0] = @"PropertyStr8";//	A
			newRows1[1] = @"	PropertyStr4";//	B
			newRows1[2] = @"Location.Level1.Code";//	C
			newRows1[3] = @"Location.Level1.Name";//	D
			newRows1[4] = @"ItemCode";//	E
			newRows1[5] = @"ItemName";//	F
			newRows1[6] = @"SerialNumberLocal";//SerialNumberLocal
			newRows1[7] = @"Quantity";//	G
			newRows1[8] = @"PropertyStr9";//	H
	

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
