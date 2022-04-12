using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;
using Count4U.Model.Count4Mobile;
using Count4U.Model.Common;
using Count4U.Model.Interface.Count4Mobile;

namespace Count4U.Model.Count4U
{
	public class ExportCurrentInventoryMerkavaXslxWriter3 : IExportCurrentInventoryWriter
	{

		public void AddRow(StreamWriter sw, CurrentInventoryAdvanced currentInventory, long countRow, string ERPNum = "", 
			string INVDate = "",
			string separator = "^", Dictionary<ImportProviderParmEnum, object> parms = null, List<PreviousInventory> details = null)
		{
			//IPreviousInventoryRepository _previousInventoryEFRepository = this._serviceLocator.GetInstance<IPreviousInventoryRepository>();
			//Dictionary<string, PreviousInventory> dictionaryPreviousInventory = _previousInventoryEFRepository.GetDictionaryPreviousInventorys(fromPathFile);
			if (details != null)
			{
				if (details.Count > 0)
				{
					string key = currentInventory.PropertyStr13Code;
					string[] newRows = new string[75];
					//newRows[0] = currentInventory.Uid;
					if (string.IsNullOrWhiteSpace(key) == true) newRows[0] = "1";
					else newRows[0] = "0";

					newRows[0] = @""; //	-1	//	
					newRows[1] = currentInventory.PropertyStr1Code;//@"PropertyStr1"; //	B	//	2
					newRows[2] = currentInventory.PropertyStr2Code;//@"PropertyStr2"; //	C	//	3
					newRows[3] = @""; //	-4	//	
					newRows[4] = @""; //	-5	//	
					newRows[5] = currentInventory.ItemCode;//@"ItemCode"; //	F	//	6
					newRows[6] = currentInventory.CatalogItemName;//@"ItemName"; //	G	//	7
					newRows[7] = currentInventory.SerialNumberLocal;//@"SerialNumberLocal"; //	H	//	8
					newRows[8] = @""; //	-9	//	
					newRows[9] = @""; //	-10	//	
					newRows[10] = @""; //	-11	//	
					newRows[11] = @"	"; //	-12	//	
					newRows[12] = @""; //	-13	//	
					newRows[13] = @""; //	-14	//	
					newRows[14] = @""; //	-15	//	
					newRows[15] = @""; //	-16	//	
					newRows[16] = currentInventory.PropertyStr3Code;//@"PropertyStr3"; //	Q	//	17
					newRows[17] = @""; //	-18	//	
					newRows[18] = currentInventory.PropertyStr4Code;//@"PropertyStr4"; //	S	//	19
					newRows[19] = currentInventory.LocationLevel3Code;//@"Location.Level3"; //	T	//	20
					newRows[20] = currentInventory.LocationLevel3Name;//@"Location.Name3"; //	U	//	21
					newRows[21] = currentInventory.PropertyStr5Code;//@"PropertyStr5"; //	V	//	22
					newRows[22] = currentInventory.LocationLevel1Code;//@"Location.Level1	"; //	W	//	23
					newRows[23] = currentInventory.LocationLevel2Code;//@"Location.Level2"; //	X	//	24
					newRows[24] = currentInventory.LocationLevel2Name;//@"Location.Name2"; //	Y	//	25
					newRows[25] = @""; //	-26	//	
					newRows[26] = @""; //	-27	//	
					newRows[27] = @""; //	-28	//	
					newRows[28] = @""; //	-29	//	
					newRows[29] = @""; //	-30	//	
					newRows[30] = @""; //	-31	//	
					newRows[31] = @""; //	-32	//	
					newRows[32] = @""; //	-33	//	
					newRows[33] = @""; //	-34	//	
					newRows[34] = currentInventory.PropertyStr6Code;//@"PropertyStr6"; //	AI	//	35
					newRows[35] = currentInventory.PropertyStr6Name;//@"PropertyStr7List.Name"; //	AJ	//	36
					newRows[36] = currentInventory.PropertyStr7Code;//@"PropertyStr7"; //	AK	//	37
					newRows[37] = currentInventory.PropertyStr7Name;//@"PropertyStr7List.Name"; //	AL	//	38
					newRows[38] = @""; //	AM	//	39
					newRows[39] = @""; //	-40	//	
					newRows[40] = @""; //	-41	//	
					newRows[41] = @""; //	-42	//	
					newRows[42] = @""; //	-43	//	
					newRows[43] = @""; //	-44	//	
					newRows[44] = @""; //	-45	//	
					newRows[45] = @""; //	-46	//	
					newRows[46] = @""; //	-47	//	
					newRows[47] = @""; //	-48	//	
					newRows[48] = currentInventory.Quantity;//@"Quantity"; //	AW	//	49
					newRows[49] = @""; //	-50	//	
					newRows[50] = @""; //	-51	//	
					newRows[51] = @""; //	-52	//	
					newRows[52] = currentInventory.SerialNumberSupplier;//@"SerialNumberSupplier"; //	BA	//	53
					newRows[53] = currentInventory.PropertyStr8Code;//@"PropertyStr8"; //	BB	//	54
					newRows[54] = currentInventory.PropertyStr9Code;//@"PropertyStr9"; //	BC	//	55
					newRows[55] = currentInventory.PropertyStr10Code;//@"PropertyStr10"; //	BD	//	56
					newRows[56] = currentInventory.PropertyStr11Code;//@"PropertyStr11"; //	BE	//	57
					//	if PropertyStr1 is empty -> "אביב"
					//if PropertyStr1 not empty -> "מרכבה"
					string propertyStr1 = "";
					if (string.IsNullOrWhiteSpace(currentInventory.PropertyStr1Code) == true) propertyStr1 = "אביב";
					else propertyStr1 = "מרכבה";
					newRows[57] = propertyStr1; //	BF	//	58
					newRows[58] = currentInventory.LocationCode;//@"LocationCode	"; //	BG	//	59
					//Merjava Agri Format, can you please add into last field - modifiedDate (DD-MM-YYYY format) its "BH" field (one after location code)
					newRows[59] = currentInventory.DateModified;			   //BH

					newRows[60] = currentInventory.PropertyStr17Code;
					string androidDateModified = currentInventory.PropertyStr17Code;
					if (string.IsNullOrWhiteSpace(androidDateModified) == false)
					{
						var longAndroidDateModified = androidDateModified.GetNullableValue<long>().GetValueOrDefault();
						DateTime modifyDateMobile1 = new DateTime(longAndroidDateModified);
						DateTime modifyDateMobile = modifyDateMobile1.ConvertFromAndroidTime();			   //получаем c андроид тикис (от  1970)
						newRows[60] = modifyDateMobile.ToShortDateString() + " " + modifyDateMobile.ToShortTimeString();//currentInventory.PropertyStr17Code;//@"PropertyStr17";							 //BI	
					}

					newRows[61] = currentInventory.PropertyStr18Name;// @"PropertyStr18List.Name";		    //	BJ	

					if (details.Count > 0)
					{
						foreach (var detail in details)
						{
							PreviousInventory pInventor = detail as PreviousInventory;
							if (pInventor != null)
							{
								if (pInventor.ItemCode.Trim().ToLower() != currentInventory.ItemCode.Trim().ToLower())
								{
									newRows[62] = pInventor.ItemCode;
									newRows[63] = pInventor.PropExtenstion9;
									newRows[64] = pInventor.PropertyStr10;
									newRows[65] = pInventor.PropertyStr11;
									newRows[66] = pInventor.SerialNumberLocal;
									newRows[67] = pInventor.SerialNumberSupplier;
									newRows[68] = currentInventory.ItemCode;
									newRows[69] = currentInventory.CatalogItemName;
									newRows[70] = currentInventory.PropertyStr10;
									newRows[71] = currentInventory.PropertyStr11;
									newRows[72] = currentInventory.SerialNumberLocal;
									newRows[73] = currentInventory.SerialNumberSupplier;
									string newRow = string.Join(separator, newRows);
									sw.WriteLine(newRow);
								}
							}
						}
					}
	
					//else
					//{
					//	string newRow = string.Join(separator, newRows);
					//	sw.WriteLine(newRow);
					//}
				}
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
			string[] newRows = new string[75];
			newRows[0] = @"Hide"; //	-1	//	
			newRows[1] = @"	קריאה בלבד (מס רץ)	"; //	B	//	2
			newRows[2] = @"	תיאור נוסף - טקסט חופשי	"; //	C	//	3
			newRows[3] = @"Hide"; //	-4	//	
			newRows[4] = @"Hide"; //	-5	//	
			newRows[5] = @"	מספר קטלוגי מרכבה	"; //	F	//	6
			newRows[6] = @"	תאור מרכבה	"; //	G	//	7
			newRows[7] = @"	סיריאלי חקלאות (SN1)	"; //	H	//	8
			newRows[8] = @"Hide"; //	-9	//	
			newRows[9] = @"Hide"; //	-10	//	
			newRows[10] = @"Hide"; //	-11	//	
			newRows[11] = @"Hide"; //	-12	//	
			newRows[12] = @"Hide"; //	-13	//	
			newRows[13] = @"Hide"; //	-14	//	
			newRows[14] = @"Hide"; //	-15	//	
			newRows[15] = @"Hide"; //	-16	//	
			newRows[16] = @"	קריאה בלבד	"; //	Q	//	17
			newRows[17] = @"Hide"; //	-18	//	
			newRows[18] = @"	קריאה בלבד	"; //	S	//	19
			newRows[19] = @"	קוד רמה 3 באיתור	"; //	T	//	20
			newRows[20] = @"	שם רמה 3 באיתור	"; //	U	//	21
			newRows[21] = @"	שם רמה 4 באיתור	"; //	V	//	22
			newRows[22] = @"	קוד רמה 1 באיתור	"; //	W	//	23
			newRows[23] = @"	קוד רמה 2 באיתור	"; //	X	//	24
			newRows[24] = @"	שם רמה 2 באיתור	"; //	Y	//	25
			newRows[25] = @"Hide"; //	-26	//	
			newRows[26] = @"Hide	"; //	-27	//	
			newRows[27] = @"Hide	"; //	-28	//	
			newRows[28] = @"Hide	"; //	-29	//	
			newRows[29] = @"Hide	"; //	-30	//	
			newRows[30] = @"Hide	"; //	-31	//	
			newRows[31] = @"Hide"; //	-32	//	
			newRows[32] = @"Hide	"; //	-33	//	
			newRows[33] = @"Hide	"; //	-34	//	
			newRows[34] = @"	מספר עובד אחראי - ניתן לשינוי (כניראה ת.ז)	"; //	AI	//	35
			newRows[35] = @"	שם עובד אחראי - ניתן לשינוי	"; //	AJ	//	36
			newRows[36] = @"	מספר עובד - האם  מושאל ? 	"; //	AK	//	37
			newRows[37] = @"	שם משפחה של השואל 	"; //	AL	//	38
			newRows[38] = @"	שם פרטי של השואל 	"; //	AM	//	39
			newRows[39] = @"Hide	"; //	-40	//	
			newRows[40] = @"Hide	"; //	-41	//	
			newRows[41] = @"Hide"; //	-42	//	
			newRows[42] = @"Hide	"; //	-43	//	
			newRows[43] = @"Hide	"; //	-44	//	
			newRows[44] = @"Hide	"; //	-45	//	
			newRows[45] = @"Hide	"; //	-46	//	
			newRows[46] = @"Hide	"; //	-47	//	
			newRows[47] = @"Hide	"; //	-48	//	
			newRows[48] = @"	כמות שנספרה (INT)	"; //	AW	//	49
			newRows[49] = @"Hide	"; //	-50	//	
			newRows[50] = @"Hide	"; //	-51	//	
			newRows[51] = @"Hide"; //	-52	//	
			newRows[52] = @"	סיריאלי יצרן (SN2)	"; //	BA	//	53
			newRows[53] = @"	סיריאלי רדיו (SN3) - יהפוך לראשי	"; //	BB	//	54
			newRows[54] = @"	טקסט חופשי, ניתן לעריכה	"; //	BC	//	55
			newRows[55] = @"	יש להחליט על צורת הכנסת הנתון	"; //	BD	//	56
			newRows[56] = @"		"; //	BE	//	57
			newRows[57] = @"	אוטומטית עי האפליקציה	"; //	BF	//	58
			newRows[58] = @"	מאגד 3 רמות	"; //	BG	//	59
			newRows[59] = @"ModifiedDate";
			newRows[60] = @"SpecialSNSupplierInsert";
			newRows[61] = @"SN Supplier Remark";
			newRows[62] = @"PreviousInventory";
			newRows[63] = @"PreviousInventory";
			newRows[64] = @"PreviousInventory";
			newRows[65] = @"PreviousInventory";
			newRows[66] = @"PreviousInventory";
			newRows[67] = @"PreviousInventory";
			newRows[68] = @"CurrentInventory";
			newRows[69] = @"CurrentInventory";
			newRows[70] = @"CurrentInventory";
			newRows[71] = @"CurrentInventory";
			newRows[72] = @"CurrentInventory";
			newRows[73] = @"CurrentInventory";
			
			//SpecialSNSupplierInsert	SN Supplier Remark


			string newRow = string.Join(separator, newRows);
			sw.WriteLine(newRow);

		   //row 1
			string[] newRows1 = new string[75];
			newRows1[0] = @"		"; //	-1	//	
			newRows1[1] = @"	העברה (לא למסופון)	"; //	B	//	2
			newRows1[2] = @"	ניתן לעריכה	"; //	C	//	3
			newRows1[3] = @"		"; //	-4	//	
			newRows1[4] = @"		"; //	-5	//	
			newRows1[5] = @"	מתוך רשימה	"; //	F	//	6
			newRows1[6] = @"	מתוך רשימה	"; //	G	//	7
			newRows1[7] = @"	מזהה ראשי/ניתן לעריכה- רק לזיהוי	"; //	H	//	8
			newRows1[8] = @"		"; //	-9	//	
			newRows1[9] = @"		"; //	-10	//	
			newRows1[10] = @"		"; //	-11	//	
			newRows1[11] = @"		"; //	-12	//	
			newRows1[12] = @"		"; //	-13	//	
			newRows1[13] = @"		"; //	-14	//	
			newRows1[14] = @"		"; //	-15	//	
			newRows1[15] = @"		"; //	-16	//	
			newRows1[16] = @"	העברה (לא למסופון)	"; //	Q	//	17
			newRows1[17] = @"		"; //	-18	//	
			newRows1[18] = @"	העברה (הצגה למסופון)	"; //	S	//	19
			newRows1[19] = @"	עדכון חובה מתוך רשימה	"; //	T	//	20
			newRows1[20] = @"	הצגה (מרשימה)	"; //	U	//	21
			newRows1[21] = @"	טקסט 8 תווים - לא חובה	"; //	V	//	22
			newRows1[22] = @"	עדכון חובה מתוך רשימה	"; //	W	//	23
			newRows1[23] = @"	עדכון חובה מתוך רשימה	"; //	X	//	24
			newRows1[24] = @"	הצגה (מרשימה)	"; //	Y	//	25
			newRows1[25] = @"		"; //	-26	//	
			newRows1[26] = @"		"; //	-27	//	
			newRows1[27] = @"		"; //	-28	//	
			newRows1[28] = @"		"; //	-29	//	
			newRows1[29] = @"		"; //	-30	//	
			newRows1[30] = @"		"; //	-31	//	
			newRows1[31] = @"		"; //	-32	//	
			newRows1[32] = @"		"; //	-33	//	
			newRows1[33] = @"		"; //	-34	//	
			newRows1[34] = @"	עדכון חובה מתוך רשימה (למרכבה עובר מספר עובד)	"; //	AI	//	35
			newRows1[35] = @"	הצגה (מרשימה)	"; //	AJ	//	36
			newRows1[36] = @"	הצגה בלבד	"; //	AK	//	37
			newRows1[37] = @"	הצגה בלבד	"; //	AL	//	38
			newRows1[38] = @"	הצגה בלבד	"; //	AM	//	39
			newRows1[39] = @"		"; //	-40	//	
			newRows1[40] = @"		"; //	-41	//	
			newRows1[41] = @"		"; //	-42	//	
			newRows1[42] = @"		"; //	-43	//	
			newRows1[43] = @"		"; //	-44	//	
			newRows1[44] = @"		"; //	-45	//	
			newRows1[45] = @"		"; //	-46	//	
			newRows1[46] = @"		"; //	-47	//	
			newRows1[47] = @"		"; //	-48	//	
			newRows1[48] = @"	חובה	"; //	AW	//	49
			newRows1[49] = @"		"; //	-50	//	
			newRows1[50] = @"		"; //	-51	//	
			newRows1[51] = @"		"; //	-52	//	
			newRows1[52] = @"	מזהה משני/ניתן לעריכה	"; //	BA	//	53
			newRows1[53] = @"	מזהה משני נוסף/ניתן לעריכה	"; //	BB	//	54
			newRows1[54] = @"	עדכון שם ו/או ת.ז עובד במידה הצורך	"; //	BC	//	55
			newRows1[55] = @"	שדה חופשי או רק שנה	"; //	BD	//	56
			newRows1[56] = @"		"; //	BE	//	57
			newRows1[57] = @"	אביב/מרכבה (מס ציוד ריק - אביב)	"; //	BF	//	58
			newRows1[58] = @"		"; //	BG	//	59

			string newRow1 = string.Join(separator, newRows1);
			sw.WriteLine(newRow1);

			//row 2
			string[] newRows2 = new string[75];
			newRows2[0] = @"		"; //	-1	//	
			newRows2[1] = @"PropertyStr1"; //	B	//	2
			newRows2[2] = @"PropertyStr2"; //	C	//	3
			newRows2[3] = @"		"; //	-4	//	
			newRows2[4] = @"		"; //	-5	//	
			newRows2[5] = @"ItemCode"; //	F	//	6
			newRows2[6] = @"ItemName"; //	G	//	7
			newRows2[7] = @"SerialNumberLocal"; //	H	//	8
			newRows2[8] = @"		"; //	-9	//	
			newRows2[9] = @"		"; //	-10	//	
			newRows2[10] = @"		"; //	-11	//	
			newRows2[11] = @"		"; //	-12	//	
			newRows2[12] = @"		"; //	-13	//	
			newRows2[13] = @"		"; //	-14	//	
			newRows2[14] = @"		"; //	-15	//	
			newRows2[15] = @"		"; //	-16	//	
			newRows2[16] = @"PropertyStr3"; //	Q	//	17
			newRows2[17] = @"		"; //	-18	//	
			newRows2[18] = @"PropertyStr4"; //	S	//	19
			newRows2[19] = @"Location.Level3"; //	T	//	20
			newRows2[20] = @"Location.Name3"; //	U	//	21
			newRows2[21] = @"PropertyStr5"; //	V	//	22
			newRows2[22] = @"Location.Level1	"; //	W	//	23
			newRows2[23] = @"Location.Level2"; //	X	//	24
			newRows2[24] = @"Location.Name2"; //	Y	//	25
			newRows2[25] = @"		"; //	-26	//	
			newRows2[26] = @"		"; //	-27	//	
			newRows2[27] = @"		"; //	-28	//	
			newRows2[28] = @"		"; //	-29	//	
			newRows2[29] = @"		"; //	-30	//	
			newRows2[30] = @"		"; //	-31	//	
			newRows2[31] = @"		"; //	-32	//	
			newRows2[32] = @"		"; //	-33	//	
			newRows2[33] = @"		"; //	-34	//	
			newRows2[34] = @"PropertyStr6"; //	AI	//	35
			newRows2[35] = @"PropertyStr7List.Name"; //	AJ	//	36
			newRows2[36] = @"PropertyStr7"; //	AK	//	37
			newRows2[37] = @"PropertyStr7List.Name"; //	AL	//	38
			newRows2[38] = @"	להתעלם	"; //	AM	//	39
			newRows2[39] = @"		"; //	-40	//	
			newRows2[40] = @"		"; //	-41	//	
			newRows2[41] = @"		"; //	-42	//	
			newRows2[42] = @"		"; //	-43	//	
			newRows2[43] = @"		"; //	-44	//	
			newRows2[44] = @"		"; //	-45	//	
			newRows2[45] = @"		"; //	-46	//	
			newRows2[46] = @"		"; //	-47	//	
			newRows2[47] = @"		"; //	-48	//	
			newRows2[48] = @"Quantity"; //	AW	//	49
			newRows2[49] = @"		"; //	-50	//	
			newRows2[50] = @"		"; //	-51	//	
			newRows2[51] = @"		"; //	-52	//	
			newRows2[52] = @"SerialNumberSupplier"; //	BA	//	53
			newRows2[53] = @"PropertyStr8"; //	BB	//	54
			newRows2[54] = @"PropertyStr9"; //	BC	//	55
			newRows2[55] = @"PropertyStr10"; //	BD	//	56
			newRows2[56] = @"PropertyStr11"; //	BE	//	57
			newRows2[57] = @"	אביב/מרכבה עמודה B ריקה - אביב)	"; //	BF	//	58
			newRows2[58] = @"LocationCode	"; //	BG	//	59
			newRows2[60] = @"PropertyStr17";
			newRows2[61] = @"PropertyStr18List.Name";
			newRows2[62] = @"PreviousInventory.ItemCode";
			newRows2[63] = @"PreviousInventory.ItemName";
			newRows2[64] = @"PreviousInventory.PropertyStr10";
			newRows2[65] = @"PreviousInventory.PropertyStr11";
			newRows2[66] = @"PreviousInventory.SerialNumberLocal";
			newRows2[67] = @"PreviousInventory.SerialNumberSupplier";
			newRows2[68] = @"CurrentInventory.ItemCode";
			newRows2[69] = @"CurrentInventory.CatalogItemName";
			newRows2[70] = @"CurrentInventory.PropertyStr10";
			newRows2[71] = @"CurrentInventory.PropertyStr11";
			newRows2[72] = @"CurrentInventory.SerialNumberLocal";
			newRows2[73] = @"CurrentInventory.SerialNumberSupplier";
	
			string newRow2 = string.Join(separator, newRows2);
			sw.WriteLine(newRow2);

			//row 3
			string[] newRows3 = new string[75];
			newRows3[0] = @"		"; //	-1	//	
			newRows3[1] = @"	ציוד	"; //	B	//	2
			newRows3[2] = @"	תאור ציוד	"; //	C	//	3
			newRows3[3] = @"D"; //	-4	//	
			newRows3[4] = @"E"; //	-5	//	
			newRows3[5] = @"	חומר	"; //	F	//	6
			newRows3[6] = @"	תיאור חומר	"; //	G	//	7
			newRows3[7] = @"	מספר סידורי	"; //	H	//	8
			newRows3[8] = @"I"; //	-9	//	
			newRows3[9] = @"J"; //	-10	//	
			newRows3[10] = @"K"; //	-11	//	
			newRows3[11] = @"	L"; //	-12	//	
			newRows3[12] = @"M	"; //	-13	//	
			newRows3[13] = @"N	"; //	-14	//	
			newRows3[14] = @"O"; //	-15	//	
			newRows3[15] = @"P"; //	-16	//	
			newRows3[16] = @"	ערך נכס מופחת	"; //	Q	//	17
			newRows3[17] = @"R"; //	-18	//	
			newRows3[18] = @"	שדה מיון	"; //	S	//	19
			newRows3[19] = @"	מיקום	"; //	T	//	20
			newRows3[20] = @"	תיאור המיקום	"; //	U	//	21
			newRows3[21] = @"	חדר	"; //	V	//	22
			newRows3[22] = @"	אתר	"; //	W	//	23
			newRows3[23] = @"	אתר אחסון	"; //	X	//	24
			newRows3[24] = @"	תיאור אתר אחסון	"; //	Y	//	25
			newRows3[25] = @"Z"; //	-26	//	
			newRows3[26] = @"AA"; //	-27	//	
			newRows3[27] = @"AB"; //	-28	//	
			newRows3[28] = @"AC"; //	-29	//	
			newRows3[29] = @"AD"; //	-30	//	
			newRows3[30] = @"AE"; //	-31	//	
			newRows3[31] = @"AF"; //	-32	//	
			newRows3[32] = @"AG"; //	-33	//	
			newRows3[33] = @"AH"; //	-34	//	
			newRows3[34] = @"	שותף	"; //	AI	//	35
			newRows3[35] = @"	שם שותף	"; //	AJ	//	36
			newRows3[36] = @"	מספר עובד	"; //	AK	//	37
			newRows3[37] = @"	שם משפחה	"; //	AL	//	38
			newRows3[38] = @"	שם פרטי	"; //	AM	//	39
			newRows3[39] = @"AN"; //	-40	//	
			newRows3[40] = @"AO"; //	-41	//	
			newRows3[41] = @"AP"; //	-42	//	
			newRows3[42] = @"AQ"; //	-43	//	
			newRows3[43] = @"AR"; //	-44	//	
			newRows3[44] = @"AS"; //	-45	//	
			newRows3[45] = @"AT"; //	-46	//	
			newRows3[46] = @"AU"; //	-47	//	
			newRows3[47] = @"AV"; //	-48	//	
			newRows3[48] = @"	כמות	"; //	AW	//	49
			newRows3[49] = @"AX"; //	-50	//	
			newRows3[50] = @"AY"; //	-51	//	
			newRows3[51] = @"AZ"; //	-52	//	
			newRows3[52] = @"SN יצרן	"; //	BA	//	53
			newRows3[53] = @"UID-RFID"; //	BB	//	54
			newRows3[54] = @"	הערות	"; //	BC	//	55
			newRows3[55] = @"	תאריך רכישה	"; //	BD	//	56
			newRows3[56] = @"	ערך רכישה	"; //	BE	//	57
			newRows3[57] = @"	מערכת מקור	"; //	BF	//	58
			newRows3[58] = @"	קוד איתור	"; //	BG	//	59
			newRows3[59] = @"Modified Date";
			newRows3[60] = @"PropertyStr17";
			newRows3[61] = @"PropertyStr18List.Name";
			newRows3[62] = @"PreviousInventory.ItemCode";
			newRows3[63] = @"PreviousInventory.ItemName";
			newRows3[64] = @"PreviousInventory.PropertyStr10";
			newRows3[65] = @"PreviousInventory.PropertyStr11";
			newRows3[66] = @"PreviousInventory.SerialNumberLocal";
			newRows3[67] = @"PreviousInventory.SerialNumberSupplier";
			newRows3[68] = @"CurrentInventory.ItemCode";
			newRows3[69] = @"CurrentInventory.CatalogItemName";
			newRows3[70] = @"CurrentInventory.PropertyStr10";
			newRows3[71] = @"CurrentInventory.PropertyStr11";
			newRows3[72] = @"CurrentInventory.SerialNumberLocal";
			newRows3[73] = @"CurrentInventory.SerialNumberSupplier";

			string newRow3 = string.Join(separator, newRows3);
			sw.WriteLine(newRow3);
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
