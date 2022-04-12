using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductRetalixPosHOERPFileWriter : IExportInventProductFileWriter
	{

		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "",
			string separator = ",", object argument = null)
		{
			//string quantityEdit = (Convert.ToInt32(iturAnalyzes.QuantityEdit)).ToString().LeadingZero7();
			////Const “2” (col 1)
			////Makat (col 2-14)
			////Qty (col 15-21)
			////Const “00000000000”  (col 22-32)
			//string newRow = "2" +													//Const “2” (col 1)
			//String.Format("{0,13}", iturAnalyzes.Makat) +				//Makat (col 2-14)
			//String.Format("{0,7}", quantityEdit) +							//Qty (col 15-21)
			//"00000000000" +														//Const “00000000000”  (col 22-32)
			//Environment.NewLine;												//CR/LF  (col 33)
			//sw.Write(newRow);

		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{
			int num71 = (int)countRow / 71 + 1;
			int num70 = (int)countRow / 70 + 1;
			string numDiv71 = num71.ToString().LeadingZero6();
			string numDiv70 = num70.ToString().LeadingZero6();
			string seq = numDiv70;

			long rest70 = countRow % 70;
			if (rest70 == 0)
			{
				rest70 = 70;
				seq = numDiv71;
			}
			long rest71 = countRow % 71;
			string numRest71 = (rest71).ToString().LeadingZero6();
			string numRest70 = (rest70).ToString().LeadingZero6();

			if (rest70 == 1)
			{
				//string newRow1 =
				//seq +
				//"0000" +
				//String.Format("{0,10}", INVDate) +
				//"000000000000";
				//sw.WriteLine(newRow1);


				string newRow1 = "1" +							//	Const “1” (col 1)
		String.Format("{0,6}", seq) +							//seq (col 2-7)
		String.Format("{0,6}", INVDate) +					//Date “YYMMDD” (col 8-13)
		"0001               " +											//Const “0001”  (col 14-17)//Const Space (col 18-32)
		Environment.NewLine;										//CR/LF  (col 33)
				sw.Write(newRow1);
			}


			//-------------
			string quantityEdit = (Convert.ToInt32(iturAnalyzes.QuantityEdit)).ToString().LeadingZero7();
			//Const “2” (col 1)
			//Makat (col 2-14)
			//Qty (col 15-21)
			//Const “00000000000”  (col 22-32)
			string newRow = "2" +													//Const “2” (col 1)
			String.Format("{0,13}", iturAnalyzes.Makat) +				//Makat (col 2-14)
			String.Format("{0,7}", quantityEdit) +							//Qty (col 15-21)
			"00000000000" +														//Const “00000000000”  (col 22-32)
			Environment.NewLine;												//CR/LF  (col 33)
			sw.Write(newRow);
		}

		public void AddHeader(StreamWriter sw, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
//            Header:
//Const “1” (col 1)
//Itur Code (col 2-7)
//Date “YYMMDD” (col 8-13)
//Const “0001”  (col 14-17)
//Const Space (col 18-32)
//CR/LF  (col 33)

	//		string iturCode = parms.GetStringValueFromParm(ImportProviderParmEnum.IturCode);
	//		string iturCode6 = iturCode.LeadingZero6();
	//		string inventorDate = parms.GetStringValueFromParm(ImportProviderParmEnum.InventorDate);

	//		string newRow = "1" +							//	Const “1” (col 1)
	//String.Format("{0,6}", iturCode6) +				//Itur Code (col 2-7)
	//String.Format("{0,6}", inventorDate) +			//Date “YYMMDD” (col 8-13)
	//"0001               " +											//Const “0001”  (col 14-17)//Const Space (col 18-32)
	//Environment.NewLine;										//CR/LF  (col 33)
	//		sw.Write(newRow);
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
