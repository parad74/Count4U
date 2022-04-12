using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using Count4U.Model.SelectionParams;
using Count4U.Model.Interface;
using Count4U.Model.Count4U;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model.Interface.Count4U;
using System.IO;
using NLog;
using ClosedXML.Excel;

namespace Count4U.Model
{
    public abstract class BaseExportFileRepository
    {
		private readonly ILog _log;
		public readonly IServiceLocator _serviceLocator;
		public static readonly Logger _logger = LogManager.GetCurrentClassLogger();

		public readonly IConnectionADO _connectionADO;

		public BaseExportFileRepository(ILog log,
			IServiceLocator serviceLocator)
		{
			if (log == null) throw new ArgumentNullException("log");
			this._log = log;
			this._serviceLocator = serviceLocator;
		}

		public ILog Log
		{
			get { return this._log; }
		}

	
		public string BuildFolderPath(string subFolder)
		{
			return "";
		}

		public string BuildFilePath(string subFolder, string fileName)
		{
			return "";
		}

		protected IProductSimpleParser GetProductSimpleParserInstance(string productParserName)
		{
			IProductSimpleParser productParser = this._serviceLocator.GetInstance<IProductSimpleParser>(productParserName);
			if (productParser == null)
			{
				//Localization.Resources.Log_Error1001%"In  ProductParserList {0} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1001, productParserName));
			}
			return productParser;
		}

		protected IProductSimpleParser GetProductSimpleParserInstance(ProductSimpleParserEnum productParserEnum)
		{
			IProductSimpleParser productParser = this._serviceLocator.GetInstance<IProductSimpleParser>(productParserEnum.ToString());
			if (productParser == null)
			{
				//Localization.Resources.Log_Error1001%"In  ProductParserList {0} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1001, productParserEnum.ToString()));
			}
			return productParser;
		}

		protected IInventProductSimpleParser GetInventProductSimpleParserInstance(InventProductSimpleParserEnum inventProductSimpleParserEnum)
		{
			IInventProductSimpleParser productParser = this._serviceLocator.GetInstance
				<IInventProductSimpleParser>(inventProductSimpleParserEnum.ToString());
			if (productParser == null)
			{
				//Localization.Resources.Log_Error1003%"In  InventProductSimpleParserList {0} - Is Not Exists"
				this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1003, inventProductSimpleParserEnum.ToString()));
			}
			return productParser;
		}

		public void DeleteFile(string pathFile)
		{
			if (File.Exists(pathFile) == true)
			{
				try
				{
					File.Delete(pathFile);
					//Localization.Resources.Log_TraceRepositoryResult1056%"Delete File  [{0}]"
					this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1056, pathFile));
				}
				catch (Exception error)
				{
					_logger.ErrorException("DeleteFile", error);
					//Localization.Resources.Log_Error1004%"Try Delete File  [{0}]"
					this.Log.Add(MessageTypeEnum.Error, String.Format(Localization.Resources.Log_Error1004, pathFile));
					this.Log.Add(MessageTypeEnum.Error, error.Message + " : " + error.StackTrace);
				}
			}
		}

		protected Dictionary<string, ProductMakat> GetProductMakatDictionary(string pathDB,
			bool refill = false)
		{
			IMakatRepository makatRepository = this._serviceLocator.GetInstance<IMakatRepository>();
			if (makatRepository == null) return new Dictionary<string, ProductMakat>();
			if (refill == true)
			{
				//Localization.Resources.Log_TraceRepositoryResult1020%"Start Fill [{0}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1020, "ProductMakatDictionary"));
				Dictionary<string, ProductMakat> productMakatDictionary =
				makatRepository.GetProductBarcodeDictionary(pathDB, true);
				//Localization.Resources.Log_TraceRepositoryResult1021%"End Fill[{0}]"
				this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1021, "ProductMakatDictionary"));
				return productMakatDictionary;
			}
			else
			{
				return makatRepository.GetProductBarcodeDictionary(pathDB, false);
			}
		}

		protected Dictionary<string, string> GetProductMakatBarcodesDictionary(string fromPathDB,
			bool refill = false)
		{
			IMakatRepository makatRepository = this._serviceLocator.GetInstance<IMakatRepository>();
			if (makatRepository == null) return new Dictionary<string, string>();
			if (refill == true)
			{
				//Localization.Resources.Log_TraceRepositoryResult1020%"Start Fill [{0}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1020, "ProductMakatBarcodesDictionary"));
				Dictionary<string, string> productMakatBarcodesDictionary =
							makatRepository.GetProductMakatBarcodesDictionary(fromPathDB, true);
				//Localization.Resources.Log_TraceRepositoryResult1021%"Fill [{0}]"
				this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1021, "ProductMakatBarcodesDictionary"));
				return productMakatBarcodesDictionary;
			}
			else
			{
				return makatRepository.GetProductMakatBarcodesDictionary(fromPathDB, false);
			}
		}

		protected Dictionary<string, Family> GetFamilyDictionary(string pathDB,  bool refill = false)
		{
			IFamilyRepository familyRepository = this._serviceLocator.GetInstance<IFamilyRepository>();
			if (familyRepository == null) return new Dictionary<string, Family>();
			if (refill == true)
			{
				//Localization.Resources.Log_TraceRepositoryResult1020%"Start Fill [{0}]"
				this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1020, "familyDictionary"));
				Dictionary<string, Family> familyDictionary =
				familyRepository.GetFamilyDictionary(pathDB, true);
				//Localization.Resources.Log_TraceRepositoryResult1021%"End Fill[{0}]"
				this.Log.Add(MessageTypeEnum.TraceRepositoryResult, String.Format(Localization.Resources.Log_TraceRepositoryResult1021, "familyDictionary"));
				return familyDictionary;
			}
			else
			{
				return familyRepository.GetFamilyDictionary(pathDB, false);
			}
		}

		protected StreamWriter GetStreamWriter(string toPathFile, Encoding encoding)
		{
			this.DeleteFile(toPathFile);
			string folder = Path.GetDirectoryName(toPathFile);
			if (Directory.Exists(folder) == false) Directory.CreateDirectory(folder);
			StreamWriter sw = new StreamWriter(toPathFile, false, encoding);
			return sw;
		}

		protected void WriteAllToExcel(MemoryStream ms, string toPathFile, Encoding encoding)
		{
			if (System.IO.Path.GetExtension(toPathFile).ToLower() != ".xlsx")
			{
				toPathFile = toPathFile + ".xlsx";
				var workbook = new XLWorkbook(); if (workbook == null) return;
				var worksheet = workbook.Worksheets.Add("Sheet 1");
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
							worksheet.Cell(i, j).SetValue<string>(rowCell);
						}
					}
				}
				workbook.SaveAs(toPathFile);
			}
		}
	}
}
