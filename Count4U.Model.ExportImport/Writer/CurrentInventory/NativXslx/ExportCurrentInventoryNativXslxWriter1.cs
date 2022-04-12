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
	public class ExportCurrentInventoryNativXslxWriter1 : IExportCurrentInventoryWriter
	{

		public void AddRow(StreamWriter sw, CurrentInventoryAdvanced currentInventory, long countRow, string ERPNum = "", 
			string INVDate = "",
			string separator = "^", Dictionary<ImportProviderParmEnum, object> parms = null, List<PreviousInventory> details = null)
		{
			string param = parms.GetStringValueFromParm(ImportProviderParmEnum.SourcePath);
			string[] newRows1 = new string[16];
			newRows1[0] = currentInventory.ItemCode;//@"	 ItemCode	"; //	A
			newRows1[1] = currentInventory.CatalogItemName;//@"	 ItemName	"; //	B
			newRows1[2] = currentInventory.LocationLevel1Code;//@"	Location.Level1.Code	"; //	C
			newRows1[3] = currentInventory.LocationLevel1Name;// @"	Location.Level1.Name	"; //	D
			newRows1[4] = currentInventory.LocationLevel2Code;//@"	Location.Level2.Code	"; //	E
			newRows1[5] = currentInventory.LocationLevel2Name;//@"	Location.Level2.Name	"; //	F
			newRows1[6] = currentInventory.SerialNumberLocal;//@"	SerialNumberLocal	"; //	G
			newRows1[7] = currentInventory.SerialNumberSupplier;//@"	SerialNumberSupplier	"; //	H
			newRows1[8] = currentInventory.Quantity;//@"	Quantity	"; //	I
			string property1StrValue = currentInventory.PropertyStr1Code;
			if (param == "PropertyStr1")  
			{
				if (string.IsNullOrWhiteSpace(currentInventory.PropertyStr1Name) == false)
				{
					property1StrValue = currentInventory.PropertyStr1Name;
				}
			}
			else if (param == "PropertyStr2")
			{
				if (string.IsNullOrWhiteSpace(currentInventory.PropertyStr2Name) == false)
				{
					property1StrValue = currentInventory.PropertyStr2Name;
				}
			}
			else if (param == "PropertyStr3")
			{
				if (string.IsNullOrWhiteSpace(currentInventory.PropertyStr3Name) == false)
				{
					property1StrValue = currentInventory.PropertyStr3Name;
				}
			}
			newRows1[9] = property1StrValue;//@"	PropertyStr1	"; //	J

			newRows1[10] = currentInventory.PropertyStr2Code;//@"	PropertyStr2	"; //	K
			newRows1[11] = currentInventory.PropertyStr3Code;//@"	PropertyStr3	"; //	L
			newRows1[12] = currentInventory.PropertyStr19Code;//@"	PropertyStr19	"; //	M
			newRows1[13] = currentInventory.PropertyStr2; //@"	CreatedDate	"; //	N
			newRows1[14] = currentInventory.PropertyStr1;//@"	modifiedDate	"; //	O
			newRows1[15] = currentInventory.LocationCode;//@"	LocationCode	"; //	P

			string newRow = string.Join(separator, newRows1);
			sw.WriteLine(newRow);
		}

		public void AddRowSimple(StreamWriter sw, CurrentInventory currentInventory, long countRow,
			string ERPNum = "", string INVDate = "", string separator = ",", Dictionary<ImportProviderParmEnum, object> parms = null, List<PreviousInventory> details = null)
		{
		
		}

		public void AddHeader(StreamWriter sw, string ERPNum = "", string INVDate = "", string separator = "^" ,
			Dictionary<ImportProviderParmEnum, object> parms = null, object argument = null)
		{
			//row 0
			string[] newRows = new string[16];
			newRows[0] = @"	 מפתח פריט	"; //	A
			newRows[1] = @"	 שם פריט	"; //	B
			newRows[2] = @"	מספר בניין	"; //	C
			newRows[3] = @"	שם בניין	"; //	D
			newRows[4] = @"	מספר חדר	"; //	E
			newRows[5] = @"	שם חדר	"; //	F
			newRows[6] = @"	מספר טבוע מקומי	"; //	G
			newRows[7] = @"	מספר טבוע יצרן	"; //	H
			newRows[8] = @"	כמות שנספרה	"; //	I
			newRows[9] = @"	הערות 1	"; //	J
			newRows[10] = @"	הערות 2	"; //	K
			newRows[11] = @"	הערות 3	"; //	L
			newRows[12] = @"	קוד מסופון	"; //	M
			newRows[13] = @"	תאריך הקמה	"; //	N
			newRows[14] = @"	תאריך עדכון	"; //	O
			newRows[15] = @"	קוד מיקום	"; //	P

			string newRow = string.Join(separator, newRows);
			sw.WriteLine(newRow);

		   //row 1
			string[] newRows1 = new string[16];
			newRows1[0] = @"	 ItemCode	"; //	A
			newRows1[1] = @"	 ItemName	"; //	B
			newRows1[2] = @"	Location.Level1.Code	"; //	C
			newRows1[3] = @"	Location.Level1.Name	"; //	D
			newRows1[4] = @"	Location.Level2.Code	"; //	E
			newRows1[5] = @"	Location.Level2.Name	"; //	F
			newRows1[6] = @"	SerialNumberLocal	"; //	G
			newRows1[7] = @"	SerialNumberSupplier	"; //	H
			newRows1[8] = @"	Quantity	"; //	I
			newRows1[9] = @"	PropertyStr1	"; //	J
			newRows1[10] = @"	PropertyStr2	"; //	K
			newRows1[11] = @"	PropertyStr3	"; //	L
			newRows1[12] = @"	PropertyStr20	"; //	M
			newRows1[13] = @"	CreatedDate	"; //	N
			newRows1[14] = @"	modifiedDate	"; //	O
			newRows1[15] = @"	LocationCode	"; //	P


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
