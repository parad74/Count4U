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
	public class ExportCurrentInventoryClaitXslxWriter1 : IExportCurrentInventoryWriter
	{


		public void AddRow(StreamWriter sw, CurrentInventoryAdvanced currentInventory, long countRow, string ERPNum = "", 
			string INVDate = "",
			string separator = "^", Dictionary<ImportProviderParmEnum, object> parms = null, List<PreviousInventory> details = null)
		{
			string key = currentInventory.PropertyStr13Code; 
			string[] newRows = new string[50];
			//newRows[0] = currentInventory.Uid;
			if (string.IsNullOrWhiteSpace(key) == true)    newRows[0] = "1";
			else newRows[0] = "0";

			newRows[1] = currentInventory.SerialNumberLocal;//SerialNumberLocal	;
			newRows[2] = currentInventory.LocationCode; //IturCodeERP	;
			newRows[3] = currentInventory.LocationDescription.Replace(',','.');
			newRows[4] = currentInventory.PropertyStr1Code;
			newRows[5] = currentInventory.SerialNumberSupplier; //SerialNumberSupplier	;
			newRows[6] = currentInventory.PropertyStr2Code;
			newRows[7] = currentInventory.PropertyStr3Code;
			newRows[8] = currentInventory.PropertyStr4Code;
			newRows[9] = currentInventory.PropertyStr5Code;
			newRows[10] = currentInventory.PropertyStr6Code;
			string rpopStr7  = "Y";
			if (currentInventory.PropertyStr7Code == "false") rpopStr7 = "N";
			else rpopStr7 = "Y";
			newRows[11] = rpopStr7;

			if (string.IsNullOrWhiteSpace(key) == false)
			{
				newRows[12] = currentInventory.PropExtenstion1;//PreviousExtension1	;
				newRows[13] = currentInventory.PropExtenstion2;//PreviousExtension2	;
				newRows[14] = currentInventory.PropExtenstion3;//PreviousExtension3	;
				newRows[15] = currentInventory.PropExtenstion4;//PreviousExtension4	;
				newRows[16] = currentInventory.PropExtenstion5;//PreviousExtension5	;
				newRows[17] = currentInventory.PropExtenstion6;//PreviousExtension6	;
				newRows[18] = currentInventory.PropExtenstion7;//PreviousExtension7	;
				newRows[19] = currentInventory.PropExtenstion8;//PreviousExtension8	;
				newRows[20] = currentInventory.PropExtenstion9;//PreviousExtension9	;
				newRows[21] = currentInventory.PropExtenstion10;//PreviousExtension10	;
				newRows[22] = currentInventory.PropExtenstion11;//PreviousExtension11	;
				newRows[23] = currentInventory.PropExtenstion12;//PreviousExtension12	;
			}
			else
			{
				newRows[12] = "";
				newRows[13] = "";
				newRows[14] = "";
				newRows[15] = "";
				newRows[16] = "";
				newRows[17] = "";
				newRows[18] = "";
				newRows[19] = "";
				newRows[20] = "";
				newRows[21] = "";
				newRows[22] = "";
				newRows[23] = "";
			}

			newRows[24] = currentInventory.PropertyStr8Code;
			newRows[25] = currentInventory.PropertyStr9Code;

			if (string.IsNullOrWhiteSpace(key) == false)				 //red
			{
				newRows[26] = currentInventory.PropertyStr10Code;
				newRows[27] = "";
				newRows[28] = "";//currentInventory.PropExtenstion13;//PreviousExtension13	;
				newRows[29] = currentInventory.PropExtenstion14; //PreviousExtension14	;
				newRows[30] = currentInventory.PropExtenstion15;//PreviousExtension15	;
				newRows[31] = currentInventory.PropExtenstion16;//PreviousExtension16	;
				newRows[32] = currentInventory.PropExtenstion17;//PreviousExtension17	;
				newRows[33] = "";//currentInventory.PropExtenstion18;//PreviousExtension18	;
				newRows[34] = currentInventory.PropertyStr11Code;
				newRows[35] = currentInventory.DateModified;
				newRows[36] = currentInventory.PropertyStr13Code;
				newRows[37] = currentInventory.LocationLevel1Code + "0000";
				newRows[38] = "U";
				newRows[39] = "PM";
				newRows[40] = "";
				newRows[41] = currentInventory.PropertyStr19Code; 
				newRows[42] = "";
			}
			else   //green
			{
				newRows[26] = "";
				newRows[27] = "";
				newRows[28] = "";
				newRows[29] = "";
				newRows[30] = "";
				newRows[31] = "";
				newRows[32] = "";
				newRows[33] = "";
				newRows[34] = currentInventory.DateModified;
				newRows[35] = currentInventory.DateModified;
				newRows[36] = "";
				newRows[37] = currentInventory.LocationLevel1Code + "0000";
				newRows[38] = "I";
				newRows[39] = "";
				newRows[40] = "";
				newRows[41] = currentInventory.PropertyStr19Code; 
				newRows[42] = "";
			}



			string newRow = string.Join(separator, newRows);
			sw.WriteLine(newRow);
		}

		public void AddRowSimple(StreamWriter sw, CurrentInventory currentInventory, long countRow,
			string ERPNum = "", string INVDate = "", string separator = "^", Dictionary<ImportProviderParmEnum, object> parms = null, List<PreviousInventory> details = null)
		{
			//int countInParentPack = iturAnalyzes.CountInParentPack;
			//double quantityEdit = iturAnalyzes.QuantityEdit != null ? (double)iturAnalyzes.QuantityEdit : 0.0 ;
			//double price = iturAnalyzes.Price;
			//double quantityEditXcountInParentPack = countInParentPack * quantityEdit;
			//double balanceQuantityERP = iturAnalyzes.QuantityOriginalERP;
			//double quantityDifference = quantityEditXcountInParentPack - balanceQuantityERP;
			//double inventoryValue = quantityEditXcountInParentPack* price;
			//double ERPValue = balanceQuantityERP * price;
			//double valueDifference = inventoryValue - ERPValue;

			//string[] newRows = new string[11];
			//newRows[0] = iturAnalyzes.Makat;
			//newRows[1] = iturAnalyzes.ProductName;
			//newRows[2] = iturAnalyzes.Price.ToString();
			//newRows[3] = countInParentPack.ToString();

			//newRows[4] = quantityEdit.ToString();
			//newRows[5] = quantityEditXcountInParentPack.ToString();
			//newRows[6] = balanceQuantityERP.ToString();
			//newRows[7] = quantityDifference.ToString();

			//newRows[8] = inventoryValue.ToString();
			//newRows[9] = ERPValue.ToString();
			//newRows[10] = valueDifference.ToString();

			////string[] newRows = new string[] { iturAnalyzes.Makat, iturAnalyzes.ProductName, iturAnalyzes.Price.ToString(), 
			////	countInParentPack.ToString(), 
			////	quantityEdit.ToString(), quantityEditXcountInParentPack.ToString(), balanceQuantityERP.ToString(),  quantityDifference.ToString(),
			////	inventoryValue.ToString(), ERPValue.ToString(),  valueDifference.ToString()};

			//string newRow = string.Join(separator, newRows);
			//sw.WriteLine(newRow);
		}

		public void AddHeader(StreamWriter sw, string ERPNum = "", string INVDate = "", string separator = "^", 
			Dictionary<ImportProviderParmEnum, object> parms = null, object argument = null)
		{
			string[] newRows = new string[50];
			newRows[0] = @"	סימון	";
			newRows[1] = @"	מדבקה צהובה	";
			newRows[2] = @"	מיקום פונקציונלי	";
			newRows[3] = @"	תאור מיקום	";
			newRows[4] = @"	מדבקה ישנה	";
			newRows[5] = @"	סריאל יצרן	";
			newRows[6] = @"	סוג ציוד/אוביקט	";
			newRows[7] = @"	יצרן	";
			newRows[8] = @"	מודל (חופשי)	";
			newRows[9] = @"	תאור ציוד (חופשי)	";
			newRows[10] = @"	סטטוס	";
			newRows[11] = @"	ליסינג	";
			newRows[12] = @"	שם מחשב	";
			newRows[13] = @"	IP	";
			newRows[14] = @"	סוג מעבד	";
			newRows[15] = @"	מהירות מעבד	";
			newRows[16] = @"	סוג זיכרון	";
			newRows[17] = @"	כמות זכרון	";
			newRows[18] = @"	גודל דיסק	";
			newRows[19] = @"	מערכת הפעלה	";
			newRows[20] = @"	DVD	";
			newRows[21] = @"	CD	";
			newRows[22] = @"	Floppy	";
			newRows[23] = @"	CDR	";
			newRows[24] = @"	גודל צג	";
			newRows[25] = @"	הערה	";
			newRows[26] = @"	ספק	";
			newRows[27] = @"	תאור ספק	";
			newRows[28] = @"	תאריך תחילת אחריות	";
			newRows[29] = @"	תאריך התקנה	";
			newRows[30] = @"	תאריך תום אחריות	";
			newRows[31] = @"	שם המתקין	";
			newRows[32] = @"	משתמש	";
			newRows[33] = @"	סריאל  מ SMS	";
			newRows[34] = @"	תאריך עדכון אחרון	";
			newRows[35] = @"	תאריך ספירה אחרונה	";
			newRows[36] = @"	מפתח SAP	";
			newRows[37] = @"	מוסד	";
			newRows[38] = @"	סוג עדכון	";
			newRows[39] = @"	יוצר הרשומה	";
			newRows[40] = @"	 מחלקה ישן	";
			newRows[41] = @"	 	";
			newRows[42] = @"	 	";

			string newRow = string.Join(separator, newRows);
			sw.WriteLine(newRow);

			string[] newRows1 = new string[50];
			newRows1[0] = @"simun";
			newRows1[1] = @"	tagno";
			newRows1[2] = @"loc_func_location_id";
			newRows1[3] = @"location_descr";
			newRows1[4] = @"serial";
			newRows1[5] = @"it_vend_refno";
			newRows1[6] = @"prod_id";
			newRows1[7] = @"manufacturer";
			newRows1[8] = @"model_id";
			newRows1[9] = @"descr";
			newRows1[10] = @"configuration_stat_id	";
			newRows1[11] = @"leasing";
			newRows1[12] = @"pc_net_name";
			newRows1[13] = @"Network_address";
			newRows1[14] = @"pc_cpu_id";
			newRows1[15] = @"pc_cpu_speed_id";
			newRows1[16] = @"pc_ram_mb	";
			newRows1[17] = @"pc_ram_qty	";
			newRows1[18] = @"hdd_size";
			newRows1[19] = @"op_sys_id";
			newRows1[20] = @"pc_has_dvd";
			newRows1[21] = @"pc_has_cd";
			newRows1[22] = @"pc_has_fdd";
			newRows1[23] = @"pc_has_cdr";
			newRows1[24] = @"pc_screen_size";
			newRows1[25] = @"longtext";
			newRows1[26] = @"vend_id";
			newRows1[27] = @"teur_sapak";
			newRows1[28] = @"vend_warr_start";
			newRows1[29] = @"install_date";
			newRows1[30] = @"vend_warranty_end";
			newRows1[31] = @"install_username";
			newRows1[32] = @"pc_net_user";
			newRows1[33] = @"sms_serial";
			newRows1[34] = @"last_upd_date";
			newRows1[35] = @"purchase_date";
			newRows1[36] = @"Item_Id	";
			newRows1[37] = @"client_id	";
			newRows1[38] = @"sug_idcun";
			newRows1[39] = @"creator_id";
			newRows1[40] = @"cust_id";
			newRows1[41] = @"sofer";
			newRows1[42] = @"sms_ind";
			string newRow1 = string.Join(separator, newRows1);
			sw.WriteLine(newRow1);

		}

		public void AddHeaderSum(StreamWriter sw, IEnumerable<CurrentInventory> currentInventoryList = null, string ERPNum = "", string INVDate = "", string separator = "^" , Dictionary<ImportProviderParmEnum, object> parms = null)
		{
				// заголовки 
			//string[] newRows = new string[] { "Item Code", "Item Name", "Unit Price", 
			//	 "Quantity In Pack",	 
			//	 "Total Packs Counted", "Total Units Counted",   "Units Quantity in ERP", "Quantity Difference",
			//	"Inventory Value", "ERP Value",  "Value Difference ERP"};
		

		}
		public void AddFooter(StreamWriter sw, long countRow, string ERPNum = "", string INVDate = "", string separator = "^", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
		}

		public void AddFooterSum(StreamWriter sw, long countRow, IEnumerable<CurrentInventory> currentInventoryList = null, string ERPNum = "", string INVDate = "", string separator = "^", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
		}

		public void AddRowInventProduct(StreamWriter sw, InventProduct inventProduct, long countRow,
			string ERPNum = "", string INVDate = "", string separator = ",", object argument = null)
		{ 
		}

		//newRows[5] = "F=RC[-2]*RC[-1]";
	}
}
