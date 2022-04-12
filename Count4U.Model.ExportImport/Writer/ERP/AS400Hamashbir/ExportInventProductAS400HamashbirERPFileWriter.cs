using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductAS400HamashbirERPFileWriter : IExportInventProductFileWriter
	{


		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
			//ERPBranchCode(col 8-10)
			//Makat (col 12-26)
			//Qty (col 44-51)

			//ERP Branch Code
			//Align to the left 
			//Align to the right 
			//Type- 5.3 without the dot

			//Record Sample:
			//	   012 127027010048                       15000

				//NEW
			//BranchCodeERP (col 1-5)				 Add leading zeros
			//Inventory Date (col 6-13)
			//Makat (col 14-26)									  Add leading zeros
			//Itur (col 27-34)
			//Const (col 33-37)													 Const: "00011"
			//Quantity expected (col 38-49)							   Const: "00011"	Const: "00000001.000"
			//Quantity Edit (col 50-61)										  Format: 8.3 (with decimal point)
			//Const (col 54-56)													   Const: "01P"

//Record Sample:
//003532016112272900104727650011900006400000009.00000000006.00001P


			//string erpCode = ERPNum.Trim();
			//int len = erpCode.Length;
			//if (len > 5) erpCode = ERPNum.Substring(len - 5, 5);
			//erpCode = erpCode.PadLeft(5, '0');

			//double quantityEditDouble = Convert.ToDouble(iturAnalyzes.QuantityEdit);
			//int quantityEdit00 = (int)(quantityEditDouble * 100);

			//string quantityEdit = quantityEdit00.ToString().Trim();
			//if (quantityEdit == "0") quantityEdit = "0000000000";
			//else quantityEdit = quantityEdit00.ToString().PadLeft(10, '0');

			//string quantityEdit = quantityEditDouble.ToString("F3"); //String.Format("{0:0.###}", quantityEditDouble);
			//quantityEdit = quantityEdit.PadLeft(12, '0');

			//string makatOriginal = iturAnalyzes.Makat;  //Without Mask!
			//makatOriginal = makatOriginal.PadLeft(13,'0');

			//Inventory Date (col 6-13)
			//Makat (col 14-26)									  Add leading zeros
			//Itur (col 27-34)
			//Const (col 33-37)													 Const: "00011"
			//Quantity expected (col 38-49)							   Const: "00011"	Const: "00000001.000"
			//Quantity Edit (col 50-61)										  Format: 8.3 (with decimal point)
			//Const (col 54-56)													   Const: "01P"
			//string newRow =
			//	String.Format("{0,5}", erpCode) + 				//BranchCodeERP (col 1-5)				 Add leading zeros
			//	INVDate	+															//Inventory Date (col 6-13)
			//	makatOriginal +													 //Makat (col 14-26)									  Add leading zeros
			//	iturAnalyzes.IturCode + 										//Itur (col 27-34)
			//	"00011"+ 																// Const: "00011"
			//	"00000001.000" +													//Const: "00000001.000"
			//	quantityEdit +														//Quantity Edit (col 50-61)										  Format: 8.3 (with decimal point)
			//	"01P"	 +																//Const: "01P"
			//	Environment.NewLine;										//CR/LF  
			//sw.Write(newRow);
			//003532016112272900104727650011900006400000009.00000000006.00001P
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
			//CompanyName (col 1-2)				   01 
			//Branch ERP_Code (col 3-6)		   ERPNum
			//InventoryCode (col 7-14)			   INVDate

			//PDA Code (col 15-18)					  0001
			//IturCode (col 19-22)					 IturCode 4 digits suffix
            //FamilyCode (col 23-26)  liding zeros           //SectionIID (col 23-26)			  liding zeros

			//Makat (col 27-34)						 Makat liding zeros
			//Barcode (col 35-47)					Barcode	 liding zeros
			//Quantity Edit (col 48-54)			 integet
			//Date (col 55-62)						   separator
			//Const Spaces (col 63-102)		  "                                        "
			//Date (col 103-110)						separator
			//Hour (col 111-116)					"220000"

	   		if (ERPNum.Length > 4) ERPNum = ERPNum.Substring(0, 4);
			string iturcode = iturAnalyzes.IturCode;									 //IturCode (col 19-22)					 IturCode 4 digits suffix
			int lenIturCode = iturcode.Length;
			if (lenIturCode > 4) iturcode = iturcode.Substring(4);
			iturcode = iturcode.PadLeft(4,'0');

            //string sectionCode = iturAnalyzes.SectionCode;	//!!!	здесь и сейчас	   //FamilyCode (col 23-26)  liding zeros      //SectionIID (col 23-26)			  liding zeros
            //int lenSectionCode = sectionCode.Length;
            //if (lenSectionCode > 4) sectionCode = sectionCode.Substring(0, 4);
            //else sectionCode = sectionCode.PadLeft(4,'0');

            string familyCode = iturAnalyzes.FamilyCode;	//!!!	здесь и сейчас	   //FamilyCode (col 23-26)  liding zeros      //SectionIID (col 23-26)			  liding zeros
            int lenFamilyCode = familyCode.Length;
            if (lenFamilyCode > 4) familyCode = familyCode.Substring(0, 4);
            else familyCode = familyCode.PadLeft(4, '0');

			string makat = iturAnalyzes.Makat;								//Makat (col 27-34)			//8			 Makat liding zeros
			int lenMakat = makat.Length;
			if (lenMakat > 8) makat = makat.Substring(0, 8);
			else makat = makat.PadLeft(8,'0');

			string barcode = iturAnalyzes.Barcode;					  	//Barcode (col 35-47)		//13			Barcode	 liding zeros
			int lenBarcode = barcode.Length;
			if (lenBarcode > 13) barcode = barcode.Substring(0, 13);
			else barcode = barcode.PadLeft(13, '0');

			int quantityEdit = Convert.ToInt32(iturAnalyzes.QuantityEdit);	//Quantity Edit (col 48-54)	//7	 integet  //Add leading zeros
			string quantityEditString = quantityEdit.ToString().PadLeft(7, '0');	 

			string INVDate1	=  separator;									 //Date (col 55-62)						   separator

			string newRow =
				"01" +																// CompanyName (col 1-2)				   01 
				String.Format("{0,4}", ERPNum) + 					//Branch ERP_Code (col 3-6)			
				INVDate1 +														//InventoryCode (col 7-14)			   INVDate
				 "0001" +															//PDA Code (col 15-18)					  0001
				String.Format("{0,4}", iturcode) +						//IturCode (col 19-22)					 IturCode 4 digits suffix
                String.Format("{0,4}", familyCode) + 				//familyCode (col 23-26)			  liding zeros
				String.Format("{0,8}", makat) +							//Makat (col 27-34)						 Makat liding zeros
				String.Format("{0,13}", barcode) +					//Barcode (col 35-47)			13		Barcode	 liding zeros
				String.Format("{0,7}", quantityEditString) +		//Quantity Edit (col 48-54)			 integet			
				INVDate + 														//Date (col 55-62)						   separator
				 "                                        " +						   //Const Spaces (col 63-102)		  "                                        "
				INVDate + 													   	//Date (col 103-110)						separator
				"220000" +														//Hour (col 111-116)					"220000"
				Environment.NewLine;										//CR/LF  
			sw.Write(newRow);
			}

		public void AddHeader(StreamWriter sw, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
		}

		public void AddHeaderSum(StreamWriter sw, IEnumerable<IturAnalyzesSimple> iturAnalyzesList = null, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
		}
		public void AddFooter(StreamWriter sw, long countRow, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
		}

		public void AddFooterSum(StreamWriter sw, long countRow, IEnumerable<IturAnalyzesSimple> iturAnalyzesList = null, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
		}
		public void AddRowInventProduct(StreamWriter sw, InventProduct inventProduct, long countRow,
		   string ERPNum = "", string INVDate = "", string separator = ",", object argument = null)
		{
		}
	}
}
