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
	public class ExportCurrentInventoryNativPlusYesXslxWriter1_Q : IExportCurrentInventoryWriter
	{

		public void AddRow(StreamWriter sw, CurrentInventoryAdvanced currentInventory, long countRow, string ERPNum = "",
			string INVDate = "",
			string separator = "^", Dictionary<ImportProviderParmEnum, object> parms = null, List<PreviousInventory> details = null)
		{
			string param = parms.GetStringValueFromParm(ImportProviderParmEnum.SourcePath);
			//newRows1[0] = @" ItemCode";//
			//newRows1[1] = @" ItemName";//
			//newRows1[2] = @"Quantity";//
			//newRows1[3] = @"PropertyStr1";//
			//newRows1[4] = @"Location.Level1.Code";//
			//newRows1[5] = @"Location.Level2.Code";//
			//newRows1[6] = @"Location.Level2.Name";//
			string key = currentInventory.UnitTypeCode;
			if (key == "Q")
			{
				string[] newRows1 = new string[7];
				newRows1[0] = currentInventory.ItemCode;//	"	 ItemCode	"
				newRows1[1] = currentInventory.CatalogItemName;//	"	 ItemName	"
				newRows1[2] = currentInventory.Quantity;//	"	Quantity	"
				newRows1[3] = currentInventory.PropertyStr1Name;//	"	PropertyStr1	"
				newRows1[4] = currentInventory.LocationLevel1Code;//	"	Location.Level1.Code	"
				newRows1[5] = currentInventory.LocationLevel1Name;//	"	Location.Level1.Name	"
				newRows1[6] = currentInventory.LocationLevel2Code;//	"	Location.Level2.Code	"
			
				
				string newRow = string.Join(separator, newRows1);
				sw.WriteLine(newRow);
			}
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
			string[] newRows = new string[7];
			newRows[0] = @"	מק''ט	";// ItemCode	
			newRows[1] = @"	תיאור פריט	";//	 ItemName	
			newRows[2] = @" 	כמות שנספרה	 - תקין";//		Quantity	
			newRows[3] = @"כמות שנספרה - תקול	";//		PropertyStr1	
			newRows[4] = @"	מיקום1-אתר";//		Location.Level1.Code	
			newRows[5] = @"	מיקום2-קוד אתר איחסון";//		Location.Level2.Code	
			newRows[6] = @"מיקום2-תאור אתר איחסון	";//		Location.Level2.Name	

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

			string[] newRows1 = new string[7];

			newRows1[0] = @" ItemCode";//
			newRows1[1] = @" ItemName";//
			newRows1[2] = @"Quantity";//
			newRows1[3] = @"PropertyStr1";//
			newRows1[4] = @"Location.Level1.Code";//
			newRows1[5] = @"Location.Level2.Code";//
			newRows1[6] = @"Location.Level2.Name";//


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
