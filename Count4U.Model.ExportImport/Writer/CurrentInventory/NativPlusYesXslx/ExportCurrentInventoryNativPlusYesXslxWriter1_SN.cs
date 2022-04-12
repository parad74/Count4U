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
	public class ExportCurrentInventoryNativPlusYesXslxWriter1_SN : IExportCurrentInventoryWriter
	{

		public void AddRow(StreamWriter sw, CurrentInventoryAdvanced currentInventory, long countRow, string ERPNum = "",
			string INVDate = "",
			string separator = "^", Dictionary<ImportProviderParmEnum, object> parms = null, List<PreviousInventory> details = null)
		{
			string param = parms.GetStringValueFromParm(ImportProviderParmEnum.SourcePath);
			//newRows1[0] = "	 ItemCode	";//
			//newRows1[1] = "	 ItemName	";//
			//newRows1[2] = "	SerialNumberLocal	";//
			//newRows1[3] = "	Location.Level1.Code	";//
			//newRows1[4] = "	Location.Level2.Code	";//
			//newRows1[5] = "	Location.Level2.Name	";//


			string key = currentInventory.UnitTypeCode;
			if (key == "SN")
			{
				string[] newRows1 = new string[37];
				newRows1[0] = currentInventory.ItemCode;//	"	 ItemCode	"
				newRows1[1] = currentInventory.CatalogItemName;//	"	 ItemName	"
				newRows1[2] = currentInventory.SerialNumberLocal;//	"	SerialNumberLocal	"
				newRows1[3] = currentInventory.LocationLevel1Code;//	"	Location.Level1.Code	"
				newRows1[4] = currentInventory.LocationLevel2Code;//	"	Location.Level2.Code	"
				newRows1[5] = currentInventory.LocationLevel2Name;//	"	Location.Level2.Name	"
		
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
			//row 0
			string[] newRows = new string[6];
			newRows[0] = @"מק''ט	";//	 ItemCode
			newRows[1] = @"	תיאור פריט	";//	 ItemName	
			newRows[2] = @"מספר טבוע מקומי	";//	"	SerialNumberLocal	"
			newRows[3] = @"	מיקום1-קוד	";//	"	Location.Level1.Code	"
			newRows[4] = @"	מיקום2-קוד	";//	"	Location.Level2.Code	"
			newRows[5] = @"	מיקום2-אתר איחסון	";//	"	Location.Level2.Name	"



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


			string[] newRows1 = new string[6];
			newRows1[0] = @"	 ItemCode";//
			newRows1[1] = @" ItemName";//
			newRows1[2] = @"SerialNumberLocal";//
			newRows1[3] = @"Location.Level1.Code";//
			newRows1[4] = @"Location.Level2.Code";//
			newRows1[5] = @"Location.Level2.Name";//


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
