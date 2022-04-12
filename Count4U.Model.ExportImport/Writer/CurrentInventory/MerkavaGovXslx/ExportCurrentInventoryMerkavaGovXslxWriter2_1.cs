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
	//1 sheet  	MerkavaCurrentInventory_MerkavaFormat_
	public class ExportCurrentInventoryMerkavaGovXslxWriter2_1 : IExportCurrentInventoryWriter
	{
		//"SN"
		public void AddRow(StreamWriter sw, CurrentInventoryAdvanced currentInventory, long countRow, string ERPNum = "", 
			string INVDate = "",
			string separator = "^", Dictionary<ImportProviderParmEnum, object> parms = null, List<PreviousInventory> details = null)
		{
				string key = currentInventory.UnitTypeCode;
				if (key == "SN")
				{
					string[] newRows = new string[37];

					newRows[0] = "C"; //@"	Always C	";//	A	1
					newRows[1] = currentInventory.ItemCode; //@"	ItemCode	";//	B	2
					newRows[2] = currentInventory.Quantity;//@"	Quantity	";//	C	3
					newRows[3] = @"יח"; //@"	קבוע יחידות";//	D	4
					newRows[4] = currentInventory.SerialNumberLocal;//@"E	5	מספר סריאלי	SERNR	מספר סריאלי	SerialNumberLocal
					newRows[5] = currentInventory.CatalogItemName;//@"	ItemName	";//	F	6
					newRows[6] = currentInventory.LocationLevel1Code;//@"	Location.Level1	";//	G	7
					newRows[7] = currentInventory.LocationLevel2Code;//@"	Location.Level2	";//	H	8
					newRows[8] = "";// @"	Empty (not in use)	";//	I	9						  currentInventory.PropertyStr6Code;//
					//string propertyStr7_X = "X";

					//PropertyStr6
					//	Field1: EmployeeCode
					//Field2: EmployeeID

					string propertyStr7_X = "";
					string propertyStr7_K = currentInventory.PropertyStr6Name;	    //ID

					if (currentInventory.PropertyStr7Code == "0") currentInventory.PropertyStr7Code = "";
					if (currentInventory.PropertyStr6Code == "0") currentInventory.PropertyStr6Code = "";

  					//PropertyStr7 <> Empty & PropertyStr7 <> "0" (There is employee Code on PropertyStr7)
					//Field J ("X") = X
					//Field K ("PERID") = PropertyStr7 (take the emp code and change to emp ID)
					if (string.IsNullOrWhiteSpace(currentInventory.PropertyStr7Code) == false)	  // не пустой код	7
					{
						propertyStr7_X = "X";
						propertyStr7_K = currentInventory.PropertyStr6;		  //ID
						//if (string.IsNullOrWhiteSpace(propertyStr7_K) == true)				 //пустой ID
						//{
						//	propertyStr7_K = currentInventory.PropertyStr7Code;
						//}
					}
			

					//=============================
					//if (string.IsNullOrWhiteSpace(currentInventory.PropertyStr7Code) == false	)
					//{
					//	propertyStr7_X = "X";
					//	propertyStr7_K = currentInventory.PropertyStr7Name;
					//	if (string.IsNullOrWhiteSpace(propertyStr7_K) == true)
					//	{
					//		propertyStr7_K = currentInventory.PropertyStr7Code;
					//	}
					//}
					//else
					//{
					//	if (string.IsNullOrWhiteSpace(currentInventory.PropertyStr6Code) == false)
					//	{
					//		propertyStr7_K = currentInventory.PropertyStr6Name;
					//	}
					//}
					 //=========================
					//currentInventoryRow.PropertyStr7_X = "";	 //PropertyStr7 is Empty
					//currentInventoryRow.PropertyStr7_K = "";	//PropertyStr6 is Empty
					//if (string.IsNullOrWhiteSpace(currentInventoryRow.PropertyStr7) == false)		 //PropertyStr7 is not Empty
					//{
					//	currentInventoryRow.PropertyStr7_X = "X";
					//	if (_propertyStr6s.ContainsKey(currentInventoryRow.PropertyStr7) == true)
					//	{
					//		var propObject = _propertyStr6s[currentInventoryRow.PropertyStr7];
					//		if (propObject != null)
					//		{
					//			currentInventoryRow.PropertyStr7_K = propObject.PropertyStrName;
					//		}
					//		else
					//		{
					//			currentInventoryRow.PropertyStr7_K = currentInventoryRow.PropertyStr7;
					//		}
					//	}
					//	else
					//	{
					//		currentInventoryRow.PropertyStr7_K = currentInventoryRow.PropertyStr7;
					//	}
					//}
					//else
					//{
					//	if (string.IsNullOrWhiteSpace(currentInventoryRow.PropertyStr6) == false)		 //PropertyStr6 is not Empty
					//	{
					//		if (_propertyStr6s.ContainsKey(currentInventoryRow.PropertyStr6) == true)
					//		{
					//			var propObject = _propertyStr6s[currentInventoryRow.PropertyStr6];
					//			if (propObject != null)
					//			{
					//				currentInventoryRow.PropertyStr7_K = propObject.PropertyStrName;
					//			}
					//		}
					//	}
					//}

					newRows[9] = propertyStr7_X; // @"If PropertyStr7 not empty = X";//	J	10						
					newRows[10] = propertyStr7_K;// @"	PropertyStr7	";//	K	11
					newRows[11] = ""; //@"	Empty (not in use)	";//	L	12				 currentInventory.PropertyStr7Code; //
					newRows[12] = ""; //@"Empty (not in use)";//	M	 13	ציוד עליון	HEQUI	ציוד עליון	Empty (not in use)
			//		newRows[13] = currentInventory.LocationLevel1Code + "-" + currentInventory.LocationLevel3Code;//@Location.Level1 & "-" & Location.Level3;//	N	14
					newRows[13] = currentInventory.LocationLevel1Code + "-" + currentInventory.LocationLevel3Code;//currentInventory.LocationLevel3Code;// Location.Level3;//	N	14			=> Location.Level1 & "-" & Location.Level3.code		#1425
					newRows[14] = currentInventory.PropertyStr5Code;//@"	PropertyStr5	";//	O	15
					newRows[15] = ""; //@"	Empty (not in use)	";//	P	16
					newRows[16] = ""; //@"	Empty (not in use)	";//	Q	17
					newRows[17] = currentInventory.PropertyStr12Code;//@"	Empty (not in use)	";//	R	18
					newRows[18] = ""; //@"	Empty (not in use)	";//	S	19
					newRows[19] = ""; //@"	Empty (not in use)	";//	T	20
					newRows[20] = ""; //@"	Empty (not in use)	";//	U	21
					newRows[21] = ""; //@"	Empty (not in use)	";//	V	22
					newRows[22] = ""; // @"Empty (not in use)";//W	23	ערך הנכס בש"ח	ANSWT	ערך הנכס בש"ח	Empty (not in use)

					string propertyStr10_dt = currentInventory.PropertyStr10Code;

					if (string.IsNullOrWhiteSpace(propertyStr10_dt) == false)
					{
						DateTime dt = DateTime.Now;
						bool ret = DateTime.TryParse(propertyStr10_dt, out dt);
						if (ret == true)
						{
							//"dd/mm/yyyy"
							propertyStr10_dt = String.Format("{0:D2}", dt.Day) + @"/" + String.Format("{0:D2}", dt.Month) + @"/" + dt.Year;
						}
						else
						{
							int year = 1900;
							bool ret1 = Int32.TryParse(propertyStr10_dt, out year);
							if (ret1 == true)
							{
								if (year > 1900 && year < 2100)
								{
									propertyStr10_dt = @"01/01/" + year;
								}
								else
								{
									propertyStr10_dt = "";
								}
							}
							else
							{
								propertyStr10_dt = "";
							}
						}
					}

					newRows[23] = "";// @"	Empty (not in use)";//	X	24	תאריך רכישה	ANSDT	תאריך רכישה	Empty (not in use)

					newRows[24] = ""; //@"	Empty (not in use)	";//	Y	25
					newRows[25] = currentInventory.PropertyStr15Name; //	Z	26	יצרן	HERST	יצרן	PropertyStr15
					newRows[26] = ""; //@"	Empty (not in use)	";//	AA	27

					newRows[27] = currentInventory.PropertyStr2Name;  //@"	Empty (not in use)	";//		AB	28	מספר מודל	TYPBZ	מספר מודל	PropertyStr2
					newRows[28] = currentInventory.SerialNumberSupplier;//@"	SerialNumberSupplier	";//	AC	29
					newRows[29] = ""; //@"	Empty (not in use)	";//	AD	30
					newRows[30] = ""; //@"	Empty (not in use)	";//	AE	31
					newRows[31] = ""; //@"	Empty (not in use)	";//	AF	32
					newRows[32] = ""; //@"	Empty (not in use)	";//	AG	33
					newRows[33] = ""; // @"	Empty (not in use)	";//	AH	34
					newRows[34] = currentInventory.PropertyStr1Code;//@"	Empty (not in use)	";//	AI	35
					newRows[35] = currentInventory.PropertyStr14Name;//AJ	36	שדה מיון	EQFNR	שדה מיון	PropertyStr14
					string serialNumberLocal = "";
					if (string.IsNullOrWhiteSpace(currentInventory.PropertyStr1Code) == false)
					{
						serialNumberLocal = currentInventory.SerialNumberLocal;
					}
				
					newRows[36] = "";//AK	37	מספר סידורי ישן	SN OLD	מספר סידורי ישן	Empty (not in use)
					//newRows[37] = currentInventory.PropertyStr2Name;//@"	value from PropertyStr2	";AL 38

					string newRow = string.Join(separator, newRows);
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
			//row 0
			string[] newRows = new string[37];

			newRows[0] = @"	Always C	";//	A	1
			newRows[1] = @"	ItemCode	";//	B	2
			newRows[2] = @"	Quantity	";//	C	3
			newRows[3] = @"	קבוע יחידות";//	D	4
			newRows[4] = @"SerialNumberLocal";//	E	5	מספר סריאלי	SERNR	מספר סריאלי	SerialNumberLocal
			newRows[5] = @"	ItemName	";//	F	6
			newRows[6] = @"	Location.Level1	";//	G	7
			newRows[7] = @"	Location.Level2	";//	H	8
			newRows[8] = @"	Empty (not in use)	";//	I	9  //???										   @"PropertyStr6Code";//
			newRows[9] = @"If PropertyStr7 not empty = X";//	J	10						
			newRows[10] = @"PropertyStr7";//	K	11
			newRows[11] = @"	Empty (not in use)	";//	L	12	//???					   @"PropertyStr7Code";//
			newRows[12] = @"Empty (not in use)";//	M	 13	ציוד עליון	HEQUI	ציוד עליון	Empty (not in use)
			newRows[13] = @"Location.Level1 - Location.Level3	";//	N	14
			newRows[14] = @"PropertyStr5";//	O	15
			newRows[15] = @"Empty (not in use)	";//	P	16
			newRows[16] = @"	Empty (not in use)	";//	Q	17
			newRows[17] = @"	Empty (not in use)	";//	R	18
			newRows[18] = @"	Empty (not in use)	";//	S	19
			newRows[19] = @"	Empty (not in use)	";//	T	20
			newRows[20] = @"	Empty (not in use)	";//	U	21
			newRows[21] = @"	Empty (not in use)	";//	V	22
			newRows[22] = @"	Empty (not in use)	";//	W	23	ערך הנכס בש"ח	ANSWT	ערך הנכס בש"ח	Empty (not in use)
			newRows[23] = @"	Empty (not in use)";//	X	24	   //	X	24	תאריך רכישה	ANSDT	תאריך רכישה	Empty (not in use)
			newRows[24] = @"	Empty (not in use)	";//	Y	25
			newRows[25] = @"	PropertyStr15	";//	Z	26		  Z	26	יצרן	HERST	יצרן	PropertyStr15
			newRows[26] = @"	Empty (not in use)	";//	AA	27
			newRows[27] = @"PropertyStr2";//		AB	28	מספר מודל	TYPBZ	מספר מודל	PropertyStr2
			newRows[28] = @"	SerialNumberSupplier	";//	AC	29
			newRows[29] = @"	Empty (not in use)	";//	AD	30
			newRows[30] = @"	Empty (not in use)	";//	AE	31
			newRows[31] = @"	Empty (not in use)	";//	AF	32
			newRows[32] = @"	Empty (not in use)	";//	AG	33
			newRows[33] = @"	Empty (not in use)	";//	AH	34
			newRows[34] = @"	Empty (not in use)	";//	AI	35
			newRows[35] = @"	PropertyStr14	";//	AJ	36	 //AJ	36	שדה מיון	EQFNR	שדה מיון	PropertyStr14
			newRows[36] = @"	Empty (not in use)";//	 AK	37	מספר סידורי ישן	SN OLD	מספר סידורי ישן	Empty (not in use)
			//newRows[37] = @"	PropertyStr2	";//	AL	38

			string newRow = string.Join(separator, newRows);
			sw.WriteLine(newRow);

			//row 1
			string[] newRows1 = new string[37];
			newRows1[0] = @"	C_U	";//	A	1
			newRows1[1] = @"	MATNR	";//	B	2
			newRows1[2] = @"	ERFMG	";//	C	3
			newRows1[3] = @"	MEINS	";//	D	4
			newRows1[4] = @"	SERNR	";//		E	5	מספר סריאלי	SERNR	מספר סריאלי	SerialNumberLocal
			newRows1[5] = @"	SHTXT	";//	F	6
			newRows1[6] = @"	WERKS	";//	G	7
			newRows1[7] = @"	LGORT	";//	H	8
			newRows1[8] = @"	CHARG	";//	I	9
			newRows1[9] = @"	X	";//	J	10
			newRows1[10] = @"	PERID	";//	K	11
			newRows1[11] = @"	TPLNR	";//	L	12
			newRows1[12] = @"	HEQUI	";//	M	13	//	M	 13	ציוד עליון	HEQUI	ציוד עליון	Empty (not in use)
			newRows1[13] = @"	STORT	";//	N	14
			newRows1[14] = @"	MSGRP	";//	O	15
			newRows1[15] = @"	IWERK	";//	P	16
			newRows1[16] = @"	INGRP	";//	Q	17
			newRows1[17] = @"	ANLNR	";//	R	18
			newRows1[18] = @"	ANLKL	";//	S	19
			newRows1[19] = @"	FIPOS	";//	T	20
			newRows1[20] = @"	XBLNR	";//	U	21
			newRows1[21] = @"	KOSTL	";//	V	22
			newRows1[22] = @"	ANSWT	";//	W	23	ערך הנכס בש"ח	ANSWT	ערך הנכס בש"ח	Empty (not in use)
			newRows1[23] = @"	ANSDT	";//	X	24	  //	X	24	תאריך רכישה	ANSDT	תאריך רכישה	Empty (not in use)
			newRows1[24] = @"	GROES	";//	Y	25
			newRows1[25] = @"	HERST	";//	Z	26		 Z	26	יצרן	HERST	יצרן	PropertyStr15
			newRows1[26] = @"	HERLD	";//	AA	27
			newRows1[27] = @"	TYPBZ	";//		AB	28	מספר מודל	TYPBZ	מספר מודל	PropertyStr2
			newRows1[28] = @"	SERGE	";//	AC	29
			newRows1[29] = @"	GWLDT_I	";//	AD	30
			newRows1[30] = @"	GWLEN_I	";//	AE	31
			newRows1[31] = @"	ELIEF	";//	AF	32
			newRows1[32] = @"	GWLDT_O	";//	AG	33
			newRows1[33] = @"	GWLEN_O	";//	AH	34
			newRows1[34] = @"	TIDNR	";//	AI	35				
			newRows1[35] = @"	EQFNR	";//	AJ	36			    //AJ	36	שדה מיון	EQFNR	שדה מיון	PropertyStr14
			newRows1[36] = @"	SN OLD	";//	AK	37מספר סידורי ישן	SN OLD	מספר סידורי ישן	Empty (not in use)


			string newRow1 = string.Join(separator, newRows1);
			sw.WriteLine(newRow1);

			//row 2
			string[] newRows2 = new string[37];
			newRows2[0] = @"	יצירה/עדכון	";//	A	1
			newRows2[1] = @"item code Merkava";//	B	2
			newRows2[2] = @"	כמות 	";//	C	3
			newRows2[3] = @"	יחידת כמות	";//	D	4
			newRows2[4] = @"מספר סריאלי";//	E	5	מספר סריאלי	SERNR	מספר סריאלי	SerialNumberLocal
			newRows2[5] = @"	תיאור	";//	F	6
			newRows2[6] = @"	אתר 	";//	G	7
			newRows2[7] = @"	אתר אחסון 	";//	H	8
			newRows2[8] = @"	מספר אצווה	";//	I	9
			newRows2[9] = @"	מושאל לעובד 	";//	J	10
			newRows2[10] = @"	ת.ז. של העובד	";//	K	11
			newRows2[11] = @"	מיקום פונקציונלי 	";//	L	12
			newRows2[12] = @"	ציוד עליון	";//	M	13	 //	M	 13	ציוד עליון	HEQUI	ציוד עליון	Empty (not in use)
			newRows2[13] = @"	מיקום 	";//	N	14
			newRows2[14] = @"	חדר 	";//	O	15
			newRows2[15] = @"	אתר מתכנן	";//	P	16
			newRows2[16] = @"	קבוצת מתכנן	";//	Q	17
			newRows2[17] = @"	מס' נכס	";//	R	18
			newRows2[18] = @"	סוג נכס	";//	S	19
			newRows2[19] = @"	מספר תקנה תקציבית	";//	T	20
			newRows2[20] = @"	מספר פקודה/חשבונית	";//	U	21
			newRows2[21] = @"	מרכז עלות	";//	V	22
			newRows2[22] = @" ערך הנכס בשח ";//	W	23	ערך הנכס בשח	ANSWT	ערך הנכס בש"ח	Empty (not in use)
			newRows2[23] = @"	תאריך רכישה 	";//	X	24	 //	X	24	תאריך רכישה	ANSDT	תאריך רכישה	Empty (not in use)
			newRows2[24] = @"	גודל /מימד	";//	Y	25
			newRows2[25] = @"	יצרן 	";//	Z	26		  Z	26	יצרן	HERST	יצרן	PropertyStr15
			newRows2[26] = @"	ארץ יצרן 	";//	AA	27
			newRows2[27] = @"	מספר מודל 	";//		AB	28	מספר מודל	TYPBZ	מספר מודל	PropertyStr2
			newRows2[28] = @"	מספר סידורי יצרן  (מספר שירות)	";//	AC	29
			newRows2[29] = @"	תאריך תחילת אחריות ספק	";//	AD	30
			newRows2[30] = @"	תאריך סוף אחריות ספק	";//	AE	31
			newRows2[31] = @"	חברה מתחזקת	";//	AF	32
			newRows2[32] = @"	תחילת אחריות לקוח	";//	AG	33
			newRows2[33] = @"	סוף אחריות לקוח	";//	AH	34
			newRows2[34] = @"	מספר זיהוי טכני 	";//	AI	35
			newRows2[35] = @"	שדה מיון	";//	AJ	36			  //AJ	36	שדה מיון	EQFNR	שדה מיון	PropertyStr14
			newRows2[36] = @"	מספר סידורי ישן	";//	AK	37	מספר סידורי ישן	SN OLD	מספר סידורי ישן	Empty (not in use)
		//	newRows2[37] = @"	PropertyStr2	";//	AL	38


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
