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
	public class ExportCurrentInventoryNativPlusXslxWriter1_SN : IExportCurrentInventoryWriter
	{

		public void AddRow(StreamWriter sw, CurrentInventoryAdvanced currentInventory, long countRow, string ERPNum = "",
			string INVDate = "",
			string separator = "^", Dictionary<ImportProviderParmEnum, object> parms = null, List<PreviousInventory> details = null)
		{
			string param = parms.GetStringValueFromParm(ImportProviderParmEnum.SourcePath);
			//string[] newRows1 = new string[26];

			//newRows1[0] = currentInventory.ItemCode;//@"	 ItemCode	"; //	A
			//newRows1[1] = currentInventory.CatalogItemName;//@"	 ItemName	"; //	B
			//newRows1[2] = currentInventory.LocationLevel1Code;//@"	Location.Level1.Code	"; //	C
			//newRows1[3] = currentInventory.LocationLevel1Name;// @"	Location.Level1.Name	"; //	D
			//newRows1[4] = currentInventory.LocationLevel2Code;//@"	Location.Level2.Code	"; //	E
			//newRows1[5] = currentInventory.LocationLevel2Name;//@"	Location.Level2.Name	"; //	F
			//newRows1[6] = currentInventory.SerialNumberLocal;//@"	SerialNumberLocal	"; //	G
			//newRows1[7] = currentInventory.SerialNumberSupplier;//@"	SerialNumberSupplier	"; //	H
			//newRows1[8] = currentInventory.Quantity;//@"	Quantity	"; //	I
			//newRows1[9] = currentInventory.PropertyStr1Name;//@"	"	PropertyStr1	 (LIST)"
			//newRows1[10] = currentInventory.PropertyStr2Name;//@"	"	PropertyStr2	 (LIST)"
			//newRows1[11] = currentInventory.PropertyStr3Code;//@"	"	PropertyStr3	"
			//newRows1[12] = currentInventory.PropertyStr4Name;//@"	"	PropertyStr4 (LIST)"
			//newRows1[13] = currentInventory.PropertyStr5Name;//@"	"	PropertyStr5 (LIST)"
			//newRows1[14] = currentInventory.PropertyStr6Name;//@"	"	PropertyStr6 (LIST)"
			//newRows1[15] = currentInventory.PropertyStr7Name;//@"	"	PropertyStr7 (LIST)"
			//newRows1[16] = currentInventory.PropertyStr8Code;//@"	"	PropertyStr8"
			//newRows1[17] = currentInventory.PropertyStr9Name;//@"	"	PropertyStr9 (LIST)"
			//newRows1[18] = currentInventory.PropertyStr10Code;//@"	"	PropertyStr10"
			//newRows1[19] = currentInventory.PropertyStr19Code;//@"	"	PropertyStr19	"
			//newRows1[20] = currentInventory.PropertyStr2; //@"	CreatedDate	"; //	U
			//newRows1[21] = currentInventory.PropertyStr1;//@"	modifiedDate	"; //	V
			//newRows1[22] = currentInventory.LocationCode;//@"	LocationCode	"; //	W
			//newRows1[23] = currentInventory.LocationLevel3Code;		   //X
			//newRows1[24] = currentInventory.LocationLevel3Name;		//Y
			//newRows1[25] = currentInventory.PropertyStr4Code;			 // @" PropertyStr4 ";		//Z

			string key = currentInventory.UnitTypeCode;
			if (key == "SN")
			{
				string[] newRows1 = new string[37];
				newRows1[0] = currentInventory.ItemCode;//	"	 ItemCode	"
				newRows1[1] = currentInventory.CatalogItemName;//	"	 ItemName	"
				newRows1[2] = currentInventory.SerialNumberLocal;//	"	SerialNumberLocal	"
				newRows1[3] = currentInventory.SerialNumberSupplier;//	"	SerialNumberSupplier	"
				newRows1[4] = currentInventory.Quantity;//	"	Quantity	"
				newRows1[5] = currentInventory.PropertyStr1Name;//	"	PropertyStr1	"
				newRows1[6] = currentInventory.PropertyStr2Name;//	"	PropertyStr2"
				newRows1[7] = currentInventory.PropertyStr3Name;//	"	PropertyStr3"
				newRows1[8] = currentInventory.PropertyStr4Name;//	"	PropertyStr4"
				newRows1[9] = currentInventory.PropertyStr5Name;//	"	PropertyStr5"
				newRows1[10] = currentInventory.PropertyStr6Name;//	"	PropertyStr6"
				newRows1[11] = currentInventory.PropertyStr7Name;//	"	PropertyStr7"
				newRows1[12] = currentInventory.PropertyStr8Name;//	"	PropertyStr8"
				newRows1[13] = currentInventory.PropertyStr9Name;//	"	PropertyStr9"
				newRows1[14] = currentInventory.PropertyStr10Name;//	"	PropertyStr10"
				newRows1[15] = currentInventory.PropertyStr11Name;//	"	PropertyStr11"
				newRows1[16] = currentInventory.PropertyStr12Name;//	"	PropertyStr12"
				newRows1[17] = currentInventory.PropertyStr13Name;//	"	PropertyStr13"
				newRows1[18] = currentInventory.PropertyStr14Name;//	"	PropertyStr14"
				newRows1[19] = currentInventory.PropertyStr15Name;//	"	PropertyStr15"
				newRows1[20] = currentInventory.PropertyStr16Name;//	"	PropertyStr16"
				newRows1[21] = currentInventory.PropertyStr17Name;//	"	PropertyStr17"
				newRows1[22] = currentInventory.PropertyStr18Name;//	"	PropertyStr18"
				newRows1[23] = currentInventory.PropertyStr19Name;//	"	PropertyStr19"
				newRows1[24] = currentInventory.PropertyStr20Name;//	"	PropertyStr20"
				newRows1[25] = currentInventory.PropertyStr2;//	"	CreatedDate	"
				newRows1[26] = currentInventory.PropertyStr1;//	"	modifiedDate	"
				newRows1[27] = currentInventory.LocationCode;//	"	LocationCode	"
				newRows1[28] = currentInventory.LocationLevel1Code;//	"	Location.Level1.Code	"
				newRows1[29] = currentInventory.LocationLevel1Name;//	"	Location.Level1.Name	"
				newRows1[30] = currentInventory.LocationLevel2Code;//	"	Location.Level2.Code	"
				newRows1[31] = currentInventory.LocationLevel2Name;//	"	Location.Level2.Name	"
				newRows1[32] = currentInventory.LocationLevel3Code;//	"	Location.Level3.Code	"
				newRows1[33] = currentInventory.LocationLevel3Name;//	"	Location.Level3.Name	"
				newRows1[34] = currentInventory.LocationLevel4Code;//	"	Location.Level4.Code	"
				newRows1[35] = currentInventory.LocationLevel4Name;//	"	Location.Level4.Name	"
				if (string.IsNullOrWhiteSpace(currentInventory.PropertyStr3) == false)
				{
					newRows1[36] = ERPNum + currentInventory.PropertyStr3;
				}

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
			//string[] newRows = new string[26];
			//newRows[0] = @"	 מפתח פריט	"; //	A
			//newRows[1] = @"	 שם פריט	"; //	B
			//newRows[2] = @"	מספר בניין	"; //	C
			//newRows[3] = @"	שם בניין	"; //	D
			//newRows[4] = @"	מספר חדר	"; //	E
			//newRows[5] = @"	שם חדר	"; //	F
			//newRows[6] = @"	מספר טבוע מקומי	"; //	G
			//newRows[7] = @"	מספר טבוע יצרן	"; //	H
			//newRows[8] = @"	כמות שנספרה	"; //	I
			//newRows[9] = @"	מודל	";//	"	PropertyStr1	 (LIST)"
			//newRows[10] = @"	קטגוריה 	";//	"	PropertyStr2	 (LIST)"
			//newRows[11] = @"	מודל 2	";//	"	PropertyStr3	"
			//newRows[12] = @"	עובד	";//	"	PropertyStr4 (LIST)"
			//newRows[13] = @"	סמן	";//	"	PropertyStr5 (LIST)"
			//newRows[14] = @"	אגף	";//	"	PropertyStr6 (LIST)"
			//newRows[15] = @"	מחלקה	";//	"	PropertyStr7 (LIST)"
			//newRows[16] = @"	חדר	";//	"	PropertyStr8"
			//newRows[17] = @"	שיוך	";//	"	PropertyStr9 (LIST)"
			//newRows[18] = @"	ריק	";//	"	PropertyStr10"
			//newRows[19] = @"		קוד מסופון		";//	"	PropertyStr20	"
			//newRows[20] = @"	תאריך הקמה		";//	"	CreatedDate	"
			//newRows[21] = @"	תאריך עדכון	";//	"	modifiedDate	"
			//newRows[22] = @"	קוד מיקום	";//	"	LocationCode	"
			//newRows[23] = @"	קוד קומה   ";//currentInventory.LocationLevel3Code;
			//newRows[24] = @"	תאור קומה	";//	Location.Level3.Name	"
			//newRows[25] = @" PropertyStr4 ";		//Z 

			string[] newRows = new string[37];
			newRows[0] = @"	מק''ט	"; //	"	 ItemCode	"
			newRows[1] = @"	תיאור פריט	"; //	"	 ItemName	"
			newRows[2] = @"	מספר טבוע מקומי	"; //	"	SerialNumberLocal	"
			newRows[3] = @"	מספר טבוע יצרן	"; //	"	SerialNumberSupplier	"
			newRows[4] = @"	כמות שנספרה	"; //	"	Quantity	"
			newRows[5] = @"	שדה נוסף 1	"; //	"	PropertyStr1	"
			newRows[6] = @"	שדה נוסף 2	"; //	"	PropertyStr2"
			newRows[7] = @"	שדה נוסף 3	"; //	"	PropertyStr3"
			newRows[8] = @"	שדה נוסף 4	"; //	"	PropertyStr4"
			newRows[9] = @"	שדה נוסף 5	"; //	"	PropertyStr5"
			newRows[10] = @"	שדה נוסף 6	"; //	"	PropertyStr6"
			newRows[11] = @"	שדה נוסף 7	"; //	"	PropertyStr7"
			newRows[12] = @"	שדה נוסף 8	"; //	"	PropertyStr8"
			newRows[13] = @"	שדה נוסף 9	"; //	"	PropertyStr9"
			newRows[14] = @"	שדה נוסף 10	"; //	"	PropertyStr10"
			newRows[15] = @"	שדה נוסף 11	"; //	"	PropertyStr11"
			newRows[16] = @"	שדה נוסף 12	"; //	"	PropertyStr12"
			newRows[17] = @"	שדה נוסף 13	"; //	"	PropertyStr13"
			newRows[18] = @"	שדה נוסף 14	"; //	"	PropertyStr14"
			newRows[19] = @"	שדה נוסף 15	"; //	"	PropertyStr15"
			newRows[20] = @"	שדה נוסף 16	"; //	"	PropertyStr16"
			newRows[21] = @"	שדה נוסף 17	"; //	"	PropertyStr17"
			newRows[22] = @"	שדה נוסף 18	"; //	"	PropertyStr18"
			newRows[23] = @"	שדה נוסף 19	"; //	"	PropertyStr19"
			newRows[24] = @"	שדה נוסף 20	"; //PropertyStr20"
			newRows[25] = @"	תאריך הקמה		"; //	CreatedDate	"
			newRows[26] = @"	תאריך עדכון	"; //	"	modifiedDate	"
			newRows[27] = @"	קוד מיקום	"; //	"	LocationCode	"
			newRows[28] = @"	מיקום1-קוד	"; //	"	Location.Level1.Code	"
			newRows[29] = @"	מיקום1-שם	"; //	"	Location.Level1.Name	"
			newRows[30] = @"	מיקום2-קוד	"; //	"	Location.Level2.Code	"
			newRows[31] = @"	מיקום2-שם	"; //	"	Location.Level2.Name	"
			newRows[32] = @"	מיקום3-קוד	"; //	"	Location.Level3.Code	"
			newRows[33] = @"	מיקום3-שם	"; //	"	Location.Level3.Name	"
			newRows[34] = @"	מיקום4-קוד	"; //	"	Location.Level4.Code	"
			newRows[35] = @"	מיקום4-שם	"; //	"	Location.Level4.Name	"
			newRows[36] = "";
			//string uid = propertyStr.TypeCode + "|" + propertyStr.PropertyStrCode + "|" + propertyStr.Code;
			//DocumentSheet 	  //TypeCode						0
			// Row 					 //PropertyStrCode 			1
			//Col 						//Code								    2
			//Name 					//значение 							3
			
			for (int i = 0; i < newRows.Count(); i++ )
			{
																		//DocumentSheet, Row , 	  Col
				var futureKey = new Pair<string, string, string>("1", "1", (i+1).ToString());
				if (dictionary.ContainsKey(futureKey) == true)
				{
					string title = dictionary[futureKey].Name;
					newRows[i] = title;
				}
			}

			string newRow = string.Join(separator, newRows);
			sw.WriteLine(newRow);

			//row 1
			//string[] newRows1 = new string[26];
			//newRows1[0] = @"	 ItemCode	"; //	A
			//newRows1[1] = @"	 ItemName	"; //	B
			//newRows1[2] = @"	Location.Level1.Code	"; //	C
			//newRows1[3] = @"	Location.Level1.Name	"; //	D
			//newRows1[4] = @"	Location.Level2.Code	"; //	E
			//newRows1[5] = @"	Location.Level2.Name	"; //	F
			//newRows1[6] = @"	SerialNumberLocal	"; //	G
			//newRows1[7] = @"	SerialNumberSupplier	"; //	H
			//newRows1[8] = @"	Quantity	"; //	I
			//newRows1[9] = @"	PropertyStr1	 (LIST)";//	
			//newRows1[10] = @"	PropertyStr2	 (LIST)";//	K
			//newRows1[11] = @"	PropertyStr3	";//	L
			//newRows1[12] = @"	PropertyStr4 (LIST)";//	M
			//newRows1[13] = @"	PropertyStr5 (LIST)";//	N
			//newRows1[14] = @"	PropertyStr6 (LIST)";//	O
			//newRows1[15] = @"	PropertyStr7 (LIST)";//	P
			//newRows1[16] = @"	PropertyStr8";//	Q
			//newRows1[17] = @"	PropertyStr9 (LIST)";//	R
			//newRows1[18] = @"	PropertyStr10";//	S
			//newRows1[19] = @"	PropertyStr19	";//	T
			//newRows1[20] = @"	CreatedDate	";//	U
			//newRows1[21] = @"	modifiedDate	";//	V
			//newRows1[22] = @"	LocationCode	";//	W
			//newRows1[23] = @"Location.Level3.Code";		//X
			//newRows1[24] = @"Location.Level3.Name	";		//Y
			//newRows1[25] = @" PropertyStr4 ";		//Z

			string[] newRows1 = new string[37];
			newRows1[0] = @"	 ItemCode	";
			newRows1[1] = @"	 ItemName	";
			newRows1[2] = @"	SerialNumberLocal	";
			newRows1[3] = @"	SerialNumberSupplier	";
			newRows1[4] = @"	Quantity	";
			newRows1[5] = @"	PropertyStr1	";
			newRows1[6] = @"	PropertyStr2";
			newRows1[7] = @"	PropertyStr3";
			newRows1[8] = @"	PropertyStr4";
			newRows1[9] = @"	PropertyStr5";
			newRows1[10] = @"	PropertyStr6";
			newRows1[11] = @"	PropertyStr7";
			newRows1[12] = @"	PropertyStr8";
			newRows1[13] = @"	PropertyStr9";
			newRows1[14] = @"	PropertyStr10";
			newRows1[15] = @"	PropertyStr11";
			newRows1[16] = @"	PropertyStr12";
			newRows1[17] = @"	PropertyStr13";
			newRows1[18] = @"	PropertyStr14";
			newRows1[19] = @"	PropertyStr15";
			newRows1[20] = @"	PropertyStr16";
			newRows1[21] = @"	PropertyStr17";
			newRows1[22] = @"	PropertyStr18";
			newRows1[23] = @"	PropertyStr19";
			newRows1[24] = @"	PropertyStr20";
			newRows1[25] = @"	CreatedDate	";
			newRows1[26] = @"	modifiedDate	";
			newRows1[27] = @"	LocationCode	";
			newRows1[28] = @"	Location.Level1.Code	";
			newRows1[29] = @"	Location.Level1.Name	";
			newRows1[30] = @"	Location.Level2.Code	";
			newRows1[31] = @"	Location.Level2.Name	";
			newRows1[32] = @"	Location.Level3.Code	";
			newRows1[33] = @"	Location.Level3.Name	";
			newRows1[34] = @"	Location.Level4.Code	";
			newRows1[35] = @"	Location.Level4.Name	";
			newRows1[36] = @"  Link of Photo" ;

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
