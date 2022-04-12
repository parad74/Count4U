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

	public class ExportCurrentInventoryStockSonigoXslxWriter1 : IExportCurrentInventoryWriter
	{

		public void AddRow(StreamWriter sw, CurrentInventoryAdvanced currentInventory, long countRow, string ERPNum = "",
			string INVDate = "",
			string separator = "^", Dictionary<ImportProviderParmEnum, object> parms = null, List<PreviousInventory> details = null)
		{
			string param = parms.GetStringValueFromParm(ImportProviderParmEnum.SourcePath);
			string[] newRows1 = new string[7];
			newRows1[0] = currentInventory.ItemCode;// 		"	 ItemCode	"	//	"	מק''ט	"
			newRows1[1] = currentInventory.CatalogItemName;// 		"	 ItemName	"	//	"	תיאור פריט	"
			newRows1[2] = "F=FT--N=" + currentInventory.Quantity;// 		"	Quantity	"	//	כמות שנספרה (תבנית מספר)
			newRows1[3] = currentInventory.PropertyStr18Name;// 		"	PropertyStr18"	//	שם משתמש
			newRows1[4] = "F=FT--DT=" + currentInventory.PropertyStr8;// 		"	CreatedDate	"	//	תאריך הקמה (תבנית תאריך ושעה)
			newRows1[5] = "F=FT--DT=" + currentInventory.PropertyStr7;// 		"	modifiedDate	"	//	תאריך עדכון (תבנית תאריך ושעה)
			newRows1[6] = currentInventory.LocationCode;// 		"	LocationCode	"	//	סטטוס


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

			string[] newRows = new string[7];

			newRows[0] = @"	מק''ט	";//	"	 ItemCode	"
			newRows[1] = @"	תיאור פריט	";//	"	 ItemName	"
			newRows[2] = @"	כמות שנספרה";//	"	Quantity	"
			newRows[3] = @"	שם משתמש	";//	"	PropertyStr18"
			newRows[4] = @"	תאריך הקמה 	";//	"	CreatedDate	"
			newRows[5] = @"	תאריך עדכון ";//	"	modifiedDate	"
			newRows[6] = @"	סטטוס	";//	"	LocationCode	"




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
			string[] newRows1 = new string[7];
			newRows1[0] = @"	 ItemCode";//	"	מק''ט	"
			newRows1[1] = @"	 ItemName";//	"	תיאור פריט	"
			newRows1[2] = @"	Quantity";//	כמות שנספרה (תבנית מספר)
			newRows1[3] = @"	PropertyStr18	";//	שם משתמש
			newRows1[4] = @"	CreatedDate	";//	תאריך הקמה (תבנית תאריך ושעה)
			newRows1[5] = @"	modifiedDate	";//	תאריך עדכון (תבנית תאריך ושעה)
			newRows1[6] = @"	LocationCode	";//	סטטוס


			string newRow1 = string.Join(separator, newRows1);
			sw.WriteLine(newRow1);
		}

		public void AddHeaderSum(StreamWriter sw, IEnumerable<CurrentInventory> currentInventoryList = null, string ERPNum = "", string INVDate = "", string separator = ",", Dictionary<ImportProviderParmEnum, object> parms = null)
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
