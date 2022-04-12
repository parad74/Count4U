using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using Count4U.Model.Count4U.Validate;

namespace Count4U.Model.Count4U
{
	public class ExportCatalogPdaHt630FileWriter : IExportProductStreamWriter
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

		public void AddRowSimple(StreamWriter sw,
			Product product,
			Dictionary<string, ProductMakat> productMakatDictionary,
			Dictionary<string, string> productMakatBarcodesDictionary,
			Dictionary<string, Family> familyDictionary ,
			bool makatWithoutMask, bool barcodeWithoutMask,
			ExportFileType fileType,
			int maxLen, bool invertLetter, bool rt2lf, bool cutLf2Rt = false,
			string separator = ",", bool trimEndOrAddSeparator = true)
		{
			string typeMakat = TypeMakatEnum.M.ToString();
			string typeBarcode = TypeMakatEnum.B.ToString();
			if (product.TypeCode == typeBarcode && product.IsUpdateERP == true) return;

			//===========ExportCatalogPdaHt630FileProvider
			// ==========OnlyBarcodes
			if (fileType == ExportFileType.OnlyBarcodes)
			{
				if (product.TypeCode == typeBarcode)
				{   
					string barcode = "";
					if (makatWithoutMask == false) { barcode = product.MakatOriginal; }
					else { barcode = product.Makat; }

					string[] newRows1 = new string[] { barcode };
					string newRow1 = string.Join(separator, newRows1);
					sw.WriteLineAddSeparator(newRow1, trimEndOrAddSeparator, separator);
				}
			}

			else if (fileType == ExportFileType.OnlyBarcodesAndName)
			{
				if (product.TypeCode == typeBarcode)
				{
					string barcode = "";
					if (makatWithoutMask == false) { barcode = product.MakatOriginal; }
					else { barcode = product.Makat; }
					string productName = "";
					string parentMakat = product.ParentMakat;
					if (string.IsNullOrWhiteSpace(parentMakat) == false)
					{
						if (productMakatDictionary.ContainsKey(product.ParentMakat) == true) productName = productMakatDictionary[product.ParentMakat].Name;
					}
					string prodName = CompileProductName(productName, maxLen, invertLetter, rt2lf, cutLf2Rt);

					string[] newRows1 = new string[] { barcode, prodName };
					string newRow1 = string.Join(separator, newRows1);
					sw.WriteLineAddSeparator(newRow1, trimEndOrAddSeparator, separator);
				}
			}

			// ==========OnlyMakats
			else if (fileType == ExportFileType.OnlyMakats)
			{
				if (product.TypeCode == typeMakat)
				{
					string makat = "";
					if (makatWithoutMask == false) { makat = product.MakatOriginal; }
					else { makat = product.Makat; }

					string[] newRows1 = new string[] { makat };
					string newRow1 = string.Join(separator, newRows1);
					sw.WriteLineAddSeparator(newRow1, trimEndOrAddSeparator, separator);
				}
			}

		
			else if (fileType == ExportFileType.MakatAndNameAndListBarcode)
			{
				string makat = "";
				if (makatWithoutMask == false) { makat = product.MakatOriginal; }
				else { makat = product.Makat; }
				if (product.TypeCode == TypeMakatEnum.M.ToString())
				{
					string valueBarcodes = "";
					string barcodeList = "";
					if (productMakatBarcodesDictionary.TryGetValue(product.Makat, out valueBarcodes) == true) //?? add  barcodes (for this makat)
					{
						string[] barcodes = valueBarcodes.Split(',');
						//	record.JoinRecord(separator)
						barcodeList = string.Join("@", barcodes);
					}
					string productName = product.Name;
					string itemType = "";// пока пустое
					string prodName = CompileProductName(productName, maxLen, invertLetter, rt2lf, cutLf2Rt);
					//Item code & "|" Description & "|" & itemType(Not in Use today) & "|" & [List of Barcodes with @ as delimiter]
					//12345|Item Sample1||Barcode12345-1@Barcode12345-3@Barcode12345-322
					string[] newRows = new string[] { makat, productName, itemType, barcodeList };
					string newRow = newRows.JoinRecordSplitEndSeparator(separator);
					//string newRow = string.Join(separator, newRows);
					//sw.WriteLineAddSeparator(newRow, trimEndOrAddSeparator, separator);
					newRow = newRow + Environment.NewLine;										//CR/LF 
					sw.Write(newRow);
					//sw.WriteLine(newRow);
				}
			}
		
			else 	// ==========NOT OnlyBarcodes and NOT OnlyMakats
			{
				//if (productWriter == WriterEnum.ExportCatalogPdaHt630FileWriter)//ExportProviderEnum.ExportCatalogToFileProvider)
				//{
				if (product.TypeCode == typeMakat)
				{
					//CountRow++;
					string makat = "";
					//bool reverse = (exportProviderEnum == ExportProviderEnum.ExportCatalogPdaHt630FileWriter) ? true : false;
					string productName = " ";
					string familyType = " ";
					string familyColor = " ";
					if (fileType == ExportFileType.ProductCodeAndName
						|| fileType == ExportFileType.ProductCodeAndNameAndSupplierCode 
						|| fileType == ExportFileType.ProductCodeAndNameAndUnitType
						|| fileType == ExportFileType.ProductCodeAndNameAndUnitNameAndSerial
						|| fileType == ExportFileType.ProductCodeAndNameAndQuantityInPackAndUnitType)
					{
						productName = CompileProductName(product.Name, maxLen, invertLetter, rt2lf, cutLf2Rt);
					}
					else if (fileType == ExportFileType.ProductCodeAndFamilyNameAndFamilyColor)
					{
						if (familyDictionary.ContainsKey(product.Makat) == true)
						{
							Family family = familyDictionary[product.Makat];
							familyType = CompileProductName(family.Type, 25, invertLetter, rt2lf);
							familyColor = CompileProductName(family.Extra1, 25, invertLetter, rt2lf);
						}
					}
					//
					//=============== Makat ===================

					if (makatWithoutMask == false) { makat = product.Makat; }
					else { makat = product.MakatOriginal; }

					if (fileType == ExportFileType.ProductCodeAndName)//if (withoutProductName == false)
					{
						string[] newRows = new string[] { makat, productName };	  
						string newRow = string.Join(separator, newRows);
						sw.WriteLineAddSeparator(newRow, trimEndOrAddSeparator, separator);
						//sw.WriteLine(newRow);
					}
			
					else if (fileType == ExportFileType.ProductCodeAndNameAndSupplierCode)//if (withoutProductName == false)
					{

						string[] newRows = new string[] { makat, productName, product.SupplierCode };
						string newRow = string.Join(separator, newRows);
						sw.WriteLineAddSeparator(newRow, trimEndOrAddSeparator, separator);
						//sw.WriteLine(newRow);
					}
					else if (fileType == ExportFileType.ProductCodeAndFamilyNameAndFamilyColor)//if (withoutProductName == false)
					{
						string[] newRows = new string[] { makat, familyType + " " + familyColor };
						string newRow = string.Join(separator, newRows);
						sw.WriteLineAddSeparator(newRow, trimEndOrAddSeparator, separator);
						//sw.WriteLine(newRow);
					}
					else if (fileType == ExportFileType.ProductCodeAndNameAndUnitType)//if (withoutProductName == false)
					{
						string[] newRows = new string[] { makat, productName, product.UnitTypeCode };
						string newRow = string.Join(separator, newRows);
						sw.WriteLineAddSeparator(newRow, trimEndOrAddSeparator, separator);
						//sw.WriteLine(newRow);
					}
					else if (fileType == ExportFileType.ProductCodeAndNameAndUnitNameAndSerial)//if (withoutProductName == false)
					{
						//Field1: Item Code				//0
						//Field2: Item Name				//1
						//Field3: Unit Type					//2  UnitTypeCode
						//Field4: Unit Type Name		//3  SupplierCode
						//Field5: Group						//4  Family
						//Field6: Section						//5  SectionCode
						//Field7: Serial						//6  FamilyCode
						//Item Code, Name, Unit Type Name, Serial
						string serial = "";
						if (product.FamilyCode == "FALSE") serial = "F";
						if (product.FamilyCode == "TRUE") serial = "T";
						string productNameR2L = productName;//.ReverseDosHebrew(false, true);
						string supplierCodeR2L = product.SupplierCode.ReverseDosHebrew(true, false);
						string[] newRows = new string[] { makat, productNameR2L, supplierCodeR2L, serial };
						string newRow = string.Join(separator, newRows);
						sw.WriteLineAddSeparator(newRow, trimEndOrAddSeparator, separator);
						//sw.WriteLine(newRow);
					}
					else if (fileType == ExportFileType.ProductCodeAndNameAndQuantityInPackAndUnitType)//if (withoutProductName == false)
					{
						int quantityInPackInt = 1;
						if (product.BalanceQuantityPartialERP != null)
						{
							quantityInPackInt = Convert.ToInt32(product.BalanceQuantityPartialERP);
						}
						string quantityInPack = quantityInPackInt.ToString();
						string[] newRows = new string[] { makat, productName, quantityInPack, product.UnitTypeCode };
						string newRow = string.Join(separator, newRows);
						sw.WriteLineAddSeparator(newRow, trimEndOrAddSeparator, separator);
						//sw.WriteLine(newRow);
					}
					else
					{
						string[] newRows = new string[] { makat }; //" "
						string newRow = string.Join(separator, newRows);
						sw.WriteLineAddSeparator(newRow, trimEndOrAddSeparator, separator);
						//sw.WriteLine(newRow);
					}
					// end add Makat

					string value = "";
					if (productMakatBarcodesDictionary.TryGetValue(product.Makat, out value) == true) //?? add  barcodes (for this makat)
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
									sw.WriteLineAddSeparator(newRow1, trimEndOrAddSeparator, separator);
									//sw.WriteLine(newRow1.Trim(','));
								}
								else if (fileType == ExportFileType.ProductCodeAndNameAndSupplierCode)//if (withoutProductName == false)
								{
									string[] newRows1 = new string[] { makat1, productName, product.SupplierCode };
									string newRow1 = string.Join(separator, newRows1);
									sw.WriteLineAddSeparator(newRow1, trimEndOrAddSeparator, separator);
									//sw.WriteLine(newRow1.Trim(','));
								}
								else if (fileType == ExportFileType.ProductCodeAndFamilyNameAndFamilyColor)//if (withoutProductName == false)
								{
									string[] newRows1 = new string[] { makat1, familyType + " " + familyColor };
									string newRow1 = string.Join(separator, newRows1);
									sw.WriteLineAddSeparator(newRow1, trimEndOrAddSeparator, separator);
									//sw.WriteLine(newRow1.Trim(','));
								}
								else if (fileType == ExportFileType.ProductCodeAndNameAndUnitType)//if (withoutProductName == false)
								{
									string[] newRows = new string[] { makat1, productName, product.UnitTypeCode };
									string newRow = string.Join(separator, newRows);
									sw.WriteLineAddSeparator(newRow, trimEndOrAddSeparator, separator);
									//sw.WriteLine(newRow);
								}
								else if (fileType == ExportFileType.ProductCodeAndNameAndUnitNameAndSerial)//if (withoutProductName == false)
								{
									//Field1: Item Code				//0
									//Field2: Item Name				//1
									//Field3: Unit Type					//2  UnitTypeCode
									//Field4: Unit Type Name		//3  SupplierCode
									//Field5: Group						//4  Family
									//Field6: Section						//5  SectionCode
									//Field7: Serial						//6  FamilyCode
									//Item Code, Name, Unit Type Name, Serial
									string serial = "";
									if (product.FamilyCode == "FALSE") serial = "F";
									if (product.FamilyCode == "TRUE") serial = "T";
									string productNameR2L = productName;//.ReverseDosHebrew(false, true);
									string supplierCodeR2L = product.SupplierCode.ReverseDosHebrew(true, false);
									string[] newRows = new string[] { makat, productNameR2L, supplierCodeR2L, serial };

									string newRow = string.Join(separator, newRows);
									sw.WriteLineAddSeparator(newRow, trimEndOrAddSeparator, separator);
									//sw.WriteLine(newRow);
								}
								else if (fileType == ExportFileType.ProductCodeAndNameAndQuantityInPackAndUnitType)//if (withoutProductName == false)
								{
									int quantityInPackInt = 1;
									if (product.BalanceQuantityPartialERP != null)
									{
										quantityInPackInt = Convert.ToInt32(product.BalanceQuantityPartialERP);
									}
									string quantityInPack = quantityInPackInt.ToString();
									string[] newRows = new string[] { makat1, productName, quantityInPack, product.UnitTypeCode };
									string newRow = string.Join(separator, newRows);
									sw.WriteLineAddSeparator(newRow, trimEndOrAddSeparator, separator);
									//sw.WriteLine(newRow);
								}
								else
								{
									string[] newRows1 = new string[] { makat1 };
									string newRow1 = string.Join(separator, newRows1);
									sw.WriteLineAddSeparator(newRow1, trimEndOrAddSeparator, separator);
									//sw.WriteLine(newRow1.Trim(','));
								}
							}
						}
					}//add  barcodes (for this makat)
				}//if (product.TypeCode == typeMakat)
			}// not only Barcode

			//}
			//=============end ExportCatalogPdaHt630FileWriter
			

		}
		
		private static string CompileProductName(string name, int maxLen, bool invertLetter, bool rt2lf, bool cutLt2Rt= false)
		{
			string productName = "";
			//================ ProductName собираем ============
			string nameInvertLetter = String.IsNullOrEmpty(name) ? " " : name.ReverseDosHebrew(invertLetter, false);		 //(true, true);
			productName = nameInvertLetter;
			// хоть одна буква на ивритте
			bool isHebrew = productName.HaveDosHebrewCharInWord();
			if (isHebrew == true)
			{
				productName = nameInvertLetter.ReverseDosHebrew(false, rt2lf);
			}

			string productNameRevers = productName;
			//string productNameRevers = String.IsNullOrEmpty(product.Name) ? " " : product.Name.ReverseDosHebrew(invertLetter, rt2lf);		 //(true, true);
			//productName = productNameRevers;
			if (cutLt2Rt == false)		   //обрезаем справа налево
			{
				if (productNameRevers.Length > maxLen && maxLen > 0)
				{
					if (isHebrew == true)
					{
						int start = productNameRevers.Length - maxLen;
						productName = productNameRevers.Substring(start, maxLen);
					}
					else
					{
						productName = productNameRevers.Substring(0, maxLen);
					}
				}
			}
			else //cutLt2Rt == true то обрезаем слева на направо
			{
				if (productNameRevers.Length > maxLen && maxLen > 0)
				{
					//if (isHebrew == true)
					//{
					//	int start = productNameRevers.Length - maxLen;
					//	productName = productNameRevers.Substring(0, maxLen);
					//}
					//else
					//{
					productName = productNameRevers.Substring(0, maxLen);
					//}
					//}
				}
			}
			return productName;
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

	public static class SW
	{
		public static int i = 0;
		public static void WriteLineAddSeparator(this StreamWriter sw, string newRow, bool trimEndOrAddSeparator = true, string separator = ",")
		{
			//if (i == 3)
			//{
				newRow = newRow.Trim(separator.ToCharArray());
				//char[] arrayChar = newRow.ToCharArray();
				if (trimEndOrAddSeparator == false)
				{
					newRow = newRow + separator;
				}
				newRow = newRow + Environment.NewLine;										//CR/LF 
				sw.Write(newRow);
			//}
			//i++;
			//sw.WriteLine(newRow);
		}
	}

}
