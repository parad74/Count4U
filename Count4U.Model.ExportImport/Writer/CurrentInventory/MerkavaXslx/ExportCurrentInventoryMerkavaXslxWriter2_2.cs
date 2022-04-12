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
	//"Q"  //2 sheet		 MerkavaCurrentInventory_MerkavaFormat_
	public class ExportCurrentInventoryMerkavaXslxWriter2_2 : IExportCurrentInventoryWriter
	{

		public void AddRow(StreamWriter sw, CurrentInventoryAdvanced currentInventory, long countRow, string ERPNum = "", 
			string INVDate = "",
			string separator = "^", Dictionary<ImportProviderParmEnum, object> parms = null, List<PreviousInventory> details = null)
		{
			string key = currentInventory.UnitTypeCode;
			//if (key == "Q")
			//{
				string[] newRows = new string[40];

				newRows[0] = @"C";//	A	1
				newRows[1] = currentInventory.ItemCode; //@"	ItemCode	";//	B	2
				newRows[2] = currentInventory.QuantityDouble.ToString();//@"	Quantity	";//	C	3
				newRows[3] = @"יח";//	D	4		
				newRows[4] = currentInventory.CatalogItemName;//@"	ItemName	";//	E	5
				newRows[5] = currentInventory.LocationLevel1Code;//@"	Location.Level1	";//	F	6
				newRows[6] = currentInventory.LocationLevel2Code;//@"	Location.Level2	";//	G	7
				newRows[7] = ""; //@"	Empty (not in use)	";//	H	8
				string propertyStr7_X = "X";
				if (string.IsNullOrWhiteSpace(currentInventory.PropertyStr7Code) == true)
				{
					propertyStr7_X = "";
				}
				//if (currentInventory.PropertyStr7.Trim() == "-1")
				//{
				//	propertyStr7_X = "";
				//}

				string propertyStr7_K = currentInventory.PropertyStr7Code;
				if (currentInventory.PropertyStr7Code.Trim() == "-1")
				{
					propertyStr7_K = "";
				}


				newRows[8] = propertyStr7_X; //@"	If PropertyStr7 not empty = X";//	I	9
				newRows[9] = propertyStr7_K; // @"	PropertyStr7	";//	J	10
				newRows[10] = ""; //@"	Empty (not in use)	";//	K	11
				newRows[11] = currentInventory.PropertyStr1Code; // @"	PropertyStr1	";//	L	12
				newRows[12] = currentInventory.LocationLevel1Code + "-" + currentInventory.LocationLevel3Code;//Location.Level1 & "-" & Location.Level3//	M	13
				newRows[13] = currentInventory.PropertyStr5Code; //@"	PropertyStr5	";//	N	14
				newRows[14] = ""; //@"	Empty (not in use)	";//	O	15
				newRows[15] = ""; //@"	Empty (not in use)	";//	P	16
				newRows[16] = ""; //@"	Empty (not in use)	";//	Q	17
				newRows[17] = ""; //@"	Empty (not in use)	";//	R	18
				newRows[18] = ""; // @"	Empty (not in use)	";//	S	19
				newRows[19] = ""; //@"	Empty (not in use)	";//	T	20
				newRows[20] = ""; //@"	Empty (not in use)	";//	U	21
				newRows[21] = currentInventory.PropertyStr11Code; //@"	PropertyStr11	";//	V	22
				newRows[22] = currentInventory.PropertyStr10Code; //@"	PropertyStr10	";//	W	23
				newRows[23] = ""; //@"	Empty (not in use)	";//	X	24
				newRows[24] = ""; //@"	Empty (not in use)	";//	Y	25
				newRows[25] = ""; //@"	Empty (not in use)	";//	Z	26
				newRows[26] = ""; //@"	Empty (not in use)	";//	AA	27
				newRows[27] = currentInventory.SerialNumberSupplier;//@"	SerialNumberSupplier	";//	AB	28
				newRows[28] = ""; //@"	Empty (not in use)	";//	AC	29
				newRows[29] = ""; //@"	Empty (not in use)	";//	AD	30
				newRows[30] = ""; //@"	Empty (not in use)	";//	AE	31
				newRows[31] = ""; //@"	Empty (not in use)	";//	AF	32
				newRows[32] = ""; //@"	Empty (not in use)	";//	AG	33
				newRows[33] = ""; //@"	Empty (not in use)	";//	AH	34
				newRows[34] = currentInventory.PropertyStr4Code; //@"	PropertyStr4	";//	AI	35
				newRows[35] = currentInventory.PropertyStr1Code; //@"	PropertyStr1	";//	AJ	36

				string newRow = string.Join(separator, newRows);
				sw.WriteLine(newRow);
			//}
		}

		public void AddRowSimple(StreamWriter sw, CurrentInventory currentInventory, long countRow,
			string ERPNum = "", string INVDate = "", string separator = ",", Dictionary<ImportProviderParmEnum, object> parms = null, List<PreviousInventory> details = null)
		{
		
		}

		public void AddHeader(StreamWriter sw, string ERPNum = "", string INVDate = "", string separator = "^",
			Dictionary<ImportProviderParmEnum, object> parms = null, object argument = null)
		{
			//row 0
			string[] newRows = new string[36];
			newRows[0] = @"	Always C	";//	A	1
			newRows[1] = @"	ItemCode	";//	B	2
			newRows[2] = @"	Quantity	";//	C	3
			newRows[3] = @"	קבוע יחידות";//	D	4
			newRows[4] = @"	ItemName	";//	E	5
			newRows[5] = @"	Location.Level1	";//	F	6
			newRows[6] = @"	Location.Level2	";//	G	7
			newRows[7] = @"	Empty (not in use)	";//	H	8
			newRows[8] = @"	If PropertyStr7 not empty = X";//	I	9
			newRows[9] = @"	PropertyStr7	";//	J	10
			newRows[10] = @"	Empty (not in use)	";//	K	11
			newRows[11] = @"	PropertyStr1	";//	L	12
			newRows[12] = @"	Location.Level3	";//	M	13
			newRows[13] = @"	PropertyStr5	";//	N	14
			newRows[14] = @"	Empty (not in use)	";//	O	15
			newRows[15] = @"	Empty (not in use)	";//	P	16
			newRows[16] = @"	Empty (not in use)	";//	Q	17
			newRows[17] = @"	Empty (not in use)	";//	R	18
			newRows[18] = @"	Empty (not in use)	";//	S	19
			newRows[19] = @"	Empty (not in use)	";//	T	20
			newRows[20] = @"	Empty (not in use)	";//	U	21
			newRows[21] = @"	PropertyStr11	";//	V	22
			newRows[22] = @"	PropertyStr10	";//	W	23
			newRows[23] = @"	Empty (not in use)	";//	X	24
			newRows[24] = @"	Empty (not in use)	";//	Y	25
			newRows[25] = @"	Empty (not in use)	";//	Z	26
			newRows[26] = @"	Empty (not in use)	";//	AA	27
			newRows[27] = @"	SerialNumberSupplier	";//	AB	28
			newRows[28] = @"	Empty (not in use)	";//	AC	29
			newRows[29] = @"	Empty (not in use)	";//	AD	30
			newRows[30] = @"	Empty (not in use)	";//	AE	31
			newRows[31] = @"	Empty (not in use)	";//	AF	32
			newRows[32] = @"	Empty (not in use)	";//	AG	33
			newRows[33] = @"	Empty (not in use)	";//	AH	34
			newRows[34] = @"	PropertyStr4	";//	AI	35
			newRows[35] = @"	PropertyStr1	";//	AJ	36


			string newRow = string.Join(separator, newRows);
			sw.WriteLine(newRow);

			//row 1
			string[] newRows1 = new string[36];
			newRows1[0] = @"	C_U	";//	A	1
			newRows1[1] = @"	MATNR	";//	B	2
			newRows1[2] = @"	ERFMG	";//	C	3
			newRows1[3] = @"	MEINS	";//	D	4
			newRows1[4] = @"	SHTXT	";//	E	5
			newRows1[5] = @"	WERKS	";//	F	6
			newRows1[6] = @"	LGORT	";//	G	7
			newRows1[7] = @"	CHARG	";//	H	8
			newRows1[8] = @"	X	";//	I	9
			newRows1[9] = @"	PERID	";//	J	10
			newRows1[10] = @"	TPLNR	";//	K	11
			newRows1[11] = @"	HEQUI	";//	L	12
			newRows1[12] = @"	STORT	";//	M	13
			newRows1[13] = @"	MSGRP	";//	N	14
			newRows1[14] = @"	IWERK	";//	O	15
			newRows1[15] = @"	INGRP	";//	P	16
			newRows1[16] = @"	ANLNR	";//	Q	17
			newRows1[17] = @"	ANLKL	";//	R	18
			newRows1[18] = @"	FIPOS	";//	S	19
			newRows1[19] = @"	XBLNR	";//	T	20
			newRows1[20] = @"	KOSTL	";//	U	21
			newRows1[21] = @"	ANSWT	";//	V	22
			newRows1[22] = @"	ANSDT	";//	W	23
			newRows1[23] = @"	GROES	";//	X	24
			newRows1[24] = @"	HERST	";//	Y	25
			newRows1[25] = @"	HERLD	";//	Z	26
			newRows1[26] = @"	TYPBZ	";//	AA	27
			newRows1[27] = @"	SERGE	";//	AB	28
			newRows1[28] = @"	GWLDT_I	";//	AC	29
			newRows1[29] = @"	GWLEN_I	";//	AD	30
			newRows1[30] = @"	ELIEF	";//	AE	31
			newRows1[31] = @"	GWLDT_O	";//	AF	32
			newRows1[32] = @"	GWLEN_O	";//	AG	33
			newRows1[33] = @"	TIDNR	";//	AH	34
			newRows1[34] = @"	EQFNR	";//	AI	35
			newRows1[35] = @"	PropertyStr1	";//	AJ	36


			string newRow1 = string.Join(separator, newRows1);
			sw.WriteLine(newRow1);

			//row 2
			string[] newRows2 = new string[36];
			newRows2[0] = @"	יצירה/עדכון	";//	A	1
			newRows2[1] = @"item code Merkava";//	B	2
			newRows2[2] = @"	כמות 	";//	C	3
			newRows2[3] = @"	יחידת כמות	";//	D	4
			newRows2[4] = @"	תיאור	";//	E	5
			newRows2[5] = @"	אתר 	";//	F	6
			newRows2[6] = @"	אתר אחסון 	";//	G	7
			newRows2[7] = @"	מספר אצווה	";//	H	8
			newRows2[8] = @"	מושאל לעובד 	";//	I	9
			newRows2[9] = @"	ת.ז. של העובד	";//	J	10
			newRows2[10] = @"	מיקום פונקציונלי 	";//	K	11
			newRows2[11] = @"	ציוד עליון	";//	L	12
			newRows2[12] = @"	מיקום 	";//	M	13
			newRows2[13] = @"	חדר 	";//	N	14
			newRows2[14] = @"	אתר מתכנן	";//	O	15
			newRows2[15] = @"	קבוצת מתכנן	";//	P	16
			newRows2[16] = @"	מס' נכס	";//	Q	17
			newRows2[17] = @"	סוג נכס	";//	R	18
			newRows2[18] = @"	מספר תקנה תקציבית	";//	S	19
			newRows2[19] = @"	מספר פקודה/חשבונית	";//	T	20
			newRows2[20] = @"	מרכז עלות	";//	U	21
			newRows2[21] = @"	Value in NIS	";//	V	22
			newRows2[22] = @"	תאריך רכישה 	";//	W	23
			newRows2[23] = @"	גודל /מימד	";//	X	24
			newRows2[24] = @"	יצרן 	";//	Y	25
			newRows2[25] = @"	ארץ יצרן 	";//	Z	26
			newRows2[26] = @"	מספר מודל 	";//	AA	27
			newRows2[27] = @"	מספר סידורי יצרן  (מספר שירות)	";//	AB	28
			newRows2[28] = @"	תאריך תחילת אחריות ספק	";//	AC	29
			newRows2[29] = @"	תאריך סוף אחריות ספק	";//	AD	30
			newRows2[30] = @"	חברה מתחזקת	";//	AE	31
			newRows2[31] = @"	תחילת אחריות לקוח	";//	AF	32
			newRows2[32] = @"	סוף אחריות לקוח	";//	AG	33
			newRows2[33] = @"	מספר זיהוי טכני 	";//	AH	34
			newRows2[34] = @"	שדה מיון	";//	AI	35
			newRows2[35] = @"	PropertyStr1";//	AJ	36

			string newRow2 = string.Join(separator, newRows2);
			sw.WriteLine(newRow2);


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
