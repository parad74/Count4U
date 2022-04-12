using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportInventProductNimrodAvivERPFileWriter1 : IExportInventProductFileWriter
	{
		public void AddRow(StreamWriter sw, IturAnalyzes iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", 
			string separator = ",", object argument = null)
		{
			//Date (Of the first Item in the Itur) //Format: YYYY-MM-DDTHH:MM:00
			//Total Makats in the Itur					//8 Chars - Add zero to the left
			//Total Quantity in the Itur				//8 Chars - Add zero to the left
			//Itur CodeItur Suffix –					// 4 Chars – Add zero to the left
			//Worker_ID (Cut 4 digits)				//4 Chars - Add zero to the left
			//Date (Of the last Item in the Itur)				//	Format: YYYY-MM-DDTHH:MM:00

			DateTime dt1 = iturAnalyzes.FromTime;
			string dt1String = dt1.ToString("yyyy") + @"-" + dt1.ToString("MM") + @"-" + dt1.ToString("dd") + @"T" + dt1.ToString("HH") + @":" + dt1.ToString("mm") + @":00";
			DateTime dt2 = iturAnalyzes.ToTime;
			string dt2String = dt2.ToString("yyyy") + @"-" + dt2.ToString("MM") + @"-" + dt2.ToString("dd") + @"T" + dt2.ToString("HH") + @":" + dt2.ToString("mm") + @":00";

			if (iturAnalyzes.Total > 0)
			{
				string distinctMakat = iturAnalyzes.Total.ToString().PadLeft(8, '0');
				int quantityEditInt = (int)iturAnalyzes.QuantityEdit;
				string quantityEdit = quantityEditInt.ToString().PadLeft(8, '0');

				string iturCodeSuffix = "0000";
				if (iturAnalyzes.IturCode.Length > 4)
				{
					iturCodeSuffix = iturAnalyzes.IturCode.Substring(4).PadLeft(4, '0');//4 chars – Add zero to the left
				}

				string workerID = "0000";
				if (iturAnalyzes.WorkerID.Length > 4)
				{
					string workerId4 = iturAnalyzes.WorkerID.TrimStart('0');
					workerID = workerId4.PadLeft(4, '0');//4 chars – Add zero to the left
				}

				string[] newRows = new string[] { dt1String, distinctMakat, quantityEdit, iturCodeSuffix, workerID, dt2String };
				string newRow = string.Join("|", newRows);
				sw.WriteLine(newRow);
			}
		}

		public void AddRowSimple(StreamWriter sw, IturAnalyzesSimple iturAnalyzes, long countRow, string ERPNum = "", string INVDate = "", string separator = ",")
		{

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
