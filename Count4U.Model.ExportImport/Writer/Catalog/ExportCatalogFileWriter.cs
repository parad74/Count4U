using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportCatalogFileWriter : IExportProductStreamWriter
	{

		#region IExportProductStreamWriter Members

		public void AddRow(StreamWriter sw, 
			Product product, 
			Dictionary<string, ProductMakat> productMakatDictionary, 
			Dictionary<string, string> productMakatBarcodesDictionary, 
			bool makatWithoutMask, 
			bool barcodeWithoutMask,
			ExportFileType fileType, 
			int maxLen, 
			bool invertLetter, bool rt2lf, string separator = ",")
		{
			throw new NotImplementedException();
		}

		//? не используется
		public void AddRowSimple(StreamWriter sw,
			Product product,
			Dictionary<string, ProductMakat> productMakatDictionary,
			Dictionary<string, string> productMakatBarcodesDictionary,
			bool makatWithoutMask, bool barcodeWithoutMask,
			ExportFileType fileType,
			int maxLen, bool invertLetter, bool rt2lf,
			string separator = ",")
		{
			string typeMakat = TypeMakatEnum.M.ToString();
			//===========ExportCatalogPdaHt630FileProvider

			//if (productWriter == WriterEnum.ExportCatalogPdaHt630FileWriter)//ExportProviderEnum.ExportCatalogToFileProvider)
			//{
			if (product.TypeCode == typeMakat)
			{
				//CountRow++;
				string makat = "";
				//bool reverse = (exportProviderEnum == ExportProviderEnum.ExportCatalogPdaHt630FileWriter) ? true : false;
				string productName = " ";
				if (fileType == ExportFileType.ProductCodeAndName)
				{
					string productNameRevers = String.IsNullOrEmpty(product.Name) ? " " : product.Name.ReverseDosHebrew(invertLetter, rt2lf);		 //(true, true);
					productName = productNameRevers;
					if (productNameRevers.Length > maxLen && maxLen > 0)
					{
						//productName = productNameRevers.Substring(0, maxLen);
						int start = productNameRevers.Length - maxLen;
						productName = productNameRevers.Substring(start, maxLen);
					}
				}

				if (makatWithoutMask == false) { makat = product.Makat; }
				else { makat = product.MakatOriginal; }

				if (fileType == ExportFileType.ProductCodeAndName)//if (withoutProductName == false)
				{
					string[] newRows = new string[] { makat, productName };
					string newRow = string.Join(separator, newRows);
					sw.WriteLine(newRow);
				}
				else
				{
					string[] newRows = new string[] { makat }; //" "
					string newRow = string.Join(separator, newRows);
					sw.WriteLine(newRow);
				}


				string value = "";
				if (productMakatBarcodesDictionary.TryGetValue(product.Makat, out value) == true) //??
				{
					string[] barcodes = value.Split(',');
					foreach (string barcode in barcodes)
					{
						ProductMakat productMakat1;
						string makat1 = "";
						if (productMakatDictionary.TryGetValue(barcode, out productMakat1) == true)
						{
							if (barcodeWithoutMask == false) { makat1 = productMakat1.Makat; }
							else { makat1 = productMakat1.MakatOriginal; }

							if (fileType == ExportFileType.ProductCodeAndName)//if (withoutProductName == false)
							{
								string[] newRows1 = new string[] { makat1, productName };
								string newRow1 = string.Join(separator, newRows1);
								sw.WriteLine(newRow1.Trim(','));
							}
							else
							{
								string[] newRows1 = new string[] { makat1 };
								string newRow1 = string.Join(separator, newRows1);
								sw.WriteLine(newRow1.Trim(','));
							}
						}
					}
				}
			}
			//}
			//=============end ExportCatalogPdaHt630FileWriter

		}

		public void AddHeader(StreamWriter sw, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			throw new NotImplementedException();
		}

		public void AddHeaderSum(StreamWriter sw, string ERPNum = "", string INVDate = "", Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
