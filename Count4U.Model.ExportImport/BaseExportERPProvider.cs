using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using Count4U.Model.SelectionParams;
using Count4U.Model.Interface;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Count4U;
using Count4U.Model;
using System.Xml.Linq;
using Microsoft.Practices.ServiceLocation;
using System.IO;
using ClosedXML.Excel;
using System.Windows.Media;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Count4U.Model
{
	public abstract class BaseExportERPProvider : BaseProvider
	{
		private bool _refill = true;

	
		public BaseExportERPProvider(ILog log, IServiceLocator serviceLocator) : base(log, serviceLocator)
		{
		}


		public bool Refill
		{
			get { return _refill; }
			set { _refill = value; }
		}

		public string SetImportTypeExportByLocation(string locationCode, string toPathFile)
		{
			this._importTypes.Add(ImportDomainEnum.ExportInventProductByLocationCode);
			this.Parms[ImportProviderParmEnum.LocationCode] = locationCode;
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(toPathFile);
			string extension = Path.GetExtension(toPathFile);
			string directoryName = Path.GetDirectoryName(toPathFile);
			string newPathFile = directoryName + @"\" + fileNameWithoutExtension + "_LocationCode" + locationCode + extension;
			return newPathFile;
 	}

		
		public string SetImportTypeExportByModifyDate(DateTime? modifyDate, string toPathFile)
		{
			if (modifyDate == null) return toPathFile;
			this._importTypes.Add(ImportDomainEnum.ExportInventProductByModifyDate);
			string md = "";
			try
			{
				DateTime mdDateTime = Convert.ToDateTime(modifyDate);
				md = mdDateTime.ToString("yyyy-MM-dd");
			}
			catch { return toPathFile; }
			this.Parms[ImportProviderParmEnum.ModifyDate] = md; 
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(toPathFile);
			string extension = Path.GetExtension(toPathFile);
			string directoryName = Path.GetDirectoryName(toPathFile);
			string newPathFile = directoryName + @"\" + fileNameWithoutExtension + "_ModifyDate_" + md + extension;
			return newPathFile;
		}

		public string SetImportTypeExportByItur(string iturCode, string toPathFile)
		{
			this._importTypes.Add(ImportDomainEnum.ExportInventProductByIturCode);
			this.Parms[ImportProviderParmEnum.IturCode] = iturCode;
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(toPathFile);
			string extension = Path.GetExtension(toPathFile);
			string directoryName = Path.GetDirectoryName(toPathFile);
			string newPathFile = directoryName + @"\" + fileNameWithoutExtension + "_" + iturCode + extension;
			if (fileNameWithoutExtension.Contains("EEEE") == true) //добавляем папку с IturCode
			{
				fileNameWithoutExtension = fileNameWithoutExtension.Replace("EEEE", "");
				newPathFile = directoryName + @"\" + iturCode + @"\" + fileNameWithoutExtension + "_" + iturCode + extension;
			}
			if (fileNameWithoutExtension.Contains("NNNN") == true)
			{
				string iturCode1 = "0000";
				if (iturCode.Length > 4)
				{
					iturCode1 = iturCode.Substring(4).TrimStart('0');
				}
				fileNameWithoutExtension = fileNameWithoutExtension.Replace("NNNN", iturCode1);
				newPathFile = directoryName + @"\" + fileNameWithoutExtension + extension;
			}
			return newPathFile;
		}

		public void ClearImportTypeExportByItur()
		{
			this._importTypes.Remove(ImportDomainEnum.ExportInventProductByIturCode);
			this.Parms[ImportProviderParmEnum.IturCode] = "";
		}

		public void ClearImportTypeExportByLocation()
		{
			this._importTypes.Remove(ImportDomainEnum.ExportInventProductByLocationCode);
			this.Parms[ImportProviderParmEnum.LocationCode] = "";
		}

		
		public void ClearImportTypeExportByModifyDate()
		{
			this._importTypes.Remove(ImportDomainEnum.ExportInventProductByModifyDate);
			this.Parms[ImportProviderParmEnum.ModifyDate] = "";
		}

		public void Export(bool full = true, bool isFilterByLocations = false, List<string> locationCodeList = null,
			bool IsFilterByItur = false, List<string> iturCodeList = null, bool onModifyDate = false, DateTime? modifyDate = null)
			//bool IsGroupByItur = false, List<string> iturCodeFullList = null)
		{
			base.FillInfoLog(this.FromPathDB, this.GetType().Name, this._importTypes);
			if (full == true)
			{
				this.WriteToFile(this.ToPathFile);
			}

			
			this.ClearImportTypeExportByLocation();
			if (isFilterByLocations == true && locationCodeList != null)
			{
				foreach (var locationCode in locationCodeList)
				{
					string toPathFile = this.SetImportTypeExportByLocation(locationCode, this.ToPathFile);
					this.WriteToFile(toPathFile);
				}
			}

			this.ClearImportTypeExportByItur();
			if (IsFilterByItur == true && iturCodeList != null)
			{
				this.Refill = true;
				foreach (var iturCode in iturCodeList)
				{
					string toPathFile = this.SetImportTypeExportByItur(iturCode, this.ToPathFile);
					this.WriteToFile(toPathFile);
					this.Refill = false;
				}
			}


			this.ClearImportTypeExportByModifyDate();
			if (onModifyDate == true && modifyDate != null)
			{
				//this.Refill = true;

				string toPathFile = this.SetImportTypeExportByModifyDate(modifyDate, this.ToPathFile);
				this.WriteToFile(toPathFile);

				//this.Refill = false;
			}

			
		}

		public abstract void WriteToFile(string toPathFile);
	
		protected override void InitDefault()
		{
		}

		protected void WriteAllToExcel(MemoryStream ms, string toPathFile, Encoding encoding, int headerCountLine = 0,
			bool headerBold = true, XLColor[] headerBackgroungs = null,
			XLReferenceStyle referenceStyle = XLReferenceStyle.R1C1, bool rightToLeft = false, string separator = ",",
			 string SheetName = "Sheet 1", int position = 1, XLWorkbook waitWorkbook = null)
		{
			if (System.IO.Path.GetExtension(toPathFile).ToLower() != ".xlsx")
			{
				toPathFile = toPathFile + ".xlsx";
			}

			char[] ch = separator.ToCharArray();

			XLWorkbook workbook = waitWorkbook;
			if (workbook == null)
			{
				workbook = new XLWorkbook { RightToLeft = rightToLeft };            //	var workbook = new XLWorkbook(); 
			}
			if (workbook == null) return;
			var worksheet = workbook.Worksheets.Add(SheetName, position);
			// You can also change the reference notation:
			workbook.ReferenceStyle = referenceStyle; //XLReferenceStyle.R1C1;

			// And the workbook calculation mode:
			workbook.CalculateMode = XLCalculateMode.Auto;

			int i = 0;
			using (System.IO.StringReader reader = new System.IO.StringReader(encoding.GetString(ms.ToArray())))
			{
				string rowLine;
				while ((rowLine = reader.ReadLine()) != null)
				{
					try
					{
						i++;   //row
							   //string ret = line;
						List<string> rowCells = rowLine.Split(ch).ToList();
						int j = 0; //col
						foreach (string rowCell in rowCells)
						{
							j++;
							if (i == 1)        //"Hide" в первой строке - скрываем колонку
							{
								if (rowCell.StartsWith("Hide") == true)
								{
									worksheet.Column(j).Hide();
								}
								else
								{
									worksheet.Column(j).Width = 25;
								}
							}

							if (rowCell.StartsWith("F=") == false)          //НЕ Формула
							{
								worksheet.Cell(i, j).SetValue<string>(rowCell);
							}
							else if (rowCell.StartsWith("F=FT--N=") == true)            //Number
							{
								string rowCelltemp = rowCell.Replace("F=FT--N=", "");
								worksheet.Cell(i, j).SetValue<string>(rowCelltemp);
								worksheet.Cell(i, j).DataType = XLCellValues.Number;
							}
							else if (rowCell.StartsWith("F=FT--DT=") == true)           //DateTime
							{
								string rowCelltemp = rowCell.Replace("F=FT--DT=", "");
								worksheet.Cell(i, j).SetValue<string>(rowCelltemp);
								worksheet.Cell(i, j).DataType = XLCellValues.DateTime;
							}
							else
							{
								string rowCellFormula = rowCell.Replace("F=", "");  //"F=RC[-3]-RC[-2]"
								var cellWithFormulaR1C1 = worksheet.Cell(i, j);
								cellWithFormulaR1C1.FormulaR1C1 = rowCellFormula;//"RC[-3]-RC[-2]";
																				 //cellWithFormulaR1C1.DataType = XLCellValues.Number;
							}
							//header format style
							if (i <= headerCountLine)
							{
								worksheet.Cell(i, j).Style.Font.Bold = headerBold;
								if (headerBackgroungs != null)
								{
									if (i <= headerBackgroungs.Count())
									{
										try
										{
											if (i != 0)
											{
												if (headerBackgroungs[i - 1] != null)
												{
													XLColor color = headerBackgroungs[i - 1];
													worksheet.Cell(i, j).Style.Fill.BackgroundColor = color;//XLColor.Red;
												}
											}
										}
										catch { }
									}
								}
							}

						}
						//j++;
						//if (i > headerCountLine)
						//{
						//	var cellWithFormulaR1C1 = worksheet.Cell(i, j);
						//	cellWithFormulaR1C1.FormulaR1C1 = "RC[-3]-RC[-2]";
						//}


					}
					catch (Exception exp)
					{
						base.Log.Add(MessageTypeEnum.Error, "function : WriteAllToExcel, to file: " + toPathFile + ", i : " + i + ", row : " + rowLine);
					}
				}
			}

			if (waitWorkbook == null)
			{
				workbook.SaveAs(toPathFile);
			}

		}


		protected void WriteAllToExcelTemplate(MemoryStream ms, string toPathFile, Encoding encoding, int headerCountLine = 0, bool headerBold = true, XLColor headerBackgroung = null)
		{
			if (System.IO.Path.GetExtension(toPathFile).ToLower() != ".xlsx")
			{
				toPathFile = toPathFile + ".xlsx";
			}
			var workbook = new XLWorkbook(); if (workbook == null) return;
			var worksheet = workbook.Worksheets.Add("Sheet 1");
			// You can also change the reference notation:
			workbook.ReferenceStyle = XLReferenceStyle.R1C1;

			// And the workbook calculation mode:
			workbook.CalculateMode = XLCalculateMode.Auto;

			int i = 0;
			using (System.IO.StringReader reader = new System.IO.StringReader(encoding.GetString(ms.ToArray())))
			{
				string rowLine;
				while ((rowLine = reader.ReadLine()) != null)
				{
					i++;
					//string ret = line;
					List<string> rowCells = rowLine.Split(',').ToList();
					int j = 0;
					foreach (string rowCell in rowCells)
					{
						j++;
						if (rowCell.StartsWith("F=") == false)
						{
							worksheet.Cell(i, j).SetValue<string>(rowCell);
						}
						else
						{
							string rowCellFormula = rowCell.Replace("F=", "");	//"F=RC[-3]-RC[-2]"
							var cellWithFormulaR1C1 = worksheet.Cell(i, j);
							cellWithFormulaR1C1.FormulaR1C1 = rowCellFormula;//"RC[-3]-RC[-2]";
						}
						//header format style
						if (i <= headerCountLine)
						{

							worksheet.Cell(i, j).Style.Font.Bold = headerBold;
							if (headerBackgroung != null)
							{
								worksheet.Cell(i, j).Style.Fill.BackgroundColor = headerBackgroung;//XLColor.Red;
							}
						}

					}
					//j++;
					//if (i > headerCountLine)
					//{
					//	var cellWithFormulaR1C1 = worksheet.Cell(i, j);
					//	cellWithFormulaR1C1.FormulaR1C1 = "RC[-3]-RC[-2]";
					//}

				}
			}
			workbook.SaveAs(toPathFile);

		}

	}

}
