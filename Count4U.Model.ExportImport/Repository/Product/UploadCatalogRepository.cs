using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Count4U.Model.Interface.Count4U;
using System.Data.Objects;
using System.Data.SqlServerCe;
using System.Data;
using Count4U.Model.Interface;
using Microsoft.Practices.ServiceLocation;
using Count4U.Model;
using Count4U.Model.Count4U.Validate;
using System.IO;
using System.Threading;

namespace Count4U.Model.Count4U
{
	public class UploadCatalogRepository : BaseExportFileRepository, IUploadCatalogFileRepository
	{
		private readonly IMakatRepository _makatRepository;
		private Dictionary<string, ProductMakat> _makatDictionary;
		private IProductSimpleParser _productParser;
		private static int CountRow;
		private IExportProductStreamWriter _exportProductStreamWriter;

		public UploadCatalogRepository(
			IServiceLocator serviceLocator,
			ILog log,
			IMakatRepository makatRepository
			)
			: base(log, serviceLocator)
		{
			if (makatRepository == null) throw new ArgumentNullException("makatRepository");
			this._makatDictionary = new Dictionary<string, ProductMakat>();
			this._makatRepository = makatRepository;
		}

		public void WriteToFile(string fromPathDB, string toPathFile,
			ProductSimpleParserEnum productParserEnum,
			WriterEnum productWriter,
			//ExportProviderEnum exportProviderEnum,
			Encoding encoding, string[] separators,
			List<ImportDomainEnum> importType,
			Dictionary<ImportProviderParmEnum, object> parms = null)
		{
			CancellationToken cancellationToken = parms.GetCancellationTokenFromParm();
			if (cancellationToken == CancellationToken.None) throw new ArgumentNullException("CancellationToken.None");

			Action<long> countAction = parms.GetActionUpdateProgressFromParm();
			if (countAction == null) throw new ArgumentNullException("ActionUpdateProgress is null");

			if (cancellationToken.IsCancellationRequested == true)
			{
				//Localization.Resources.Log_TraceRepositoryResult1057%"Cancel Write to File [{0}]"
				this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1057, toPathFile));
				return;
			}

			this._exportProductStreamWriter = this._serviceLocator.GetInstance<IExportProductStreamWriter>(productWriter.ToString());
			if (this._exportProductStreamWriter == null) throw new ArgumentNullException(productWriter.ToString() + " is null");

			this._productParser = base.GetProductSimpleParserInstance(productParserEnum);
			if (this._productParser == null) return;

			//Localization.Resources.Log_TraceRepository1015%"Export to [{0}] is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1015, "Catalog", "ExportCatalogSimpleFileRepository]"));
			//Localization.Resources.Log_TraceRepository1040%"[{0}] is [{1}]"
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepository1040, "CatalogParser", productParserEnum.ToString()));

			Dictionary<string, ProductMakat> productMakatDictionary = base.GetProductMakatDictionary(fromPathDB, true);
			if (cancellationToken.IsCancellationRequested == true)
			{
				//Localization.Resources.Log_TraceRepositoryResult1057%"Cancel Write to File [{0}]"
				this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1057, toPathFile));
				return;
			}

			Dictionary<string, string> productMakatBarcodesDictionary = base.GetProductMakatBarcodesDictionary(fromPathDB, true);

			//Localization.Resources.Log_TraceRepositoryResult1016%"Start Write to File [{0}] "
			this.Log.Add(MessageTypeEnum.TraceRepository, String.Format(Localization.Resources.Log_TraceRepositoryResult1016, toPathFile));

			ExportFileType fileType = parms.GetFileTypeFromParm();
			bool barcodeWithoutMask = parms.GetBoolValueFromParm(ImportProviderParmEnum.BarcodeWithoutMask);
			bool makatWithoutMask = parms.GetBoolValueFromParm(ImportProviderParmEnum.MakatWithoutMask);
			int maxLen = parms.GetIntValueFromParm(ImportProviderParmEnum.MaxLen);
			if (maxLen == 0) maxLen = 16;
			bool invertLetter = false;
			bool rt2lf = false;
			string separator = ",";
			if (separators != null && separators.Count() > 0)
			{
				separator = separators[0];
			}
 			if (encoding == Encoding.GetEncoding(862) || encoding == Encoding.GetEncoding(1255))
			{
				invertLetter = parms.GetBoolValueFromParm(ImportProviderParmEnum.InvertLetters);
				rt2lf = parms.GetBoolValueFromParm(ImportProviderParmEnum.InvertWords);
			}

			StreamWriter sw = GetStreamWriter(toPathFile, encoding);
			try
			{
				if (importType.Contains(ImportDomainEnum.ExportCatalog) == true)
				{
					CountRow = 0;
					countAction(CountRow);
					foreach (Product product in this._productParser.GetProducts(fromPathDB,
							encoding, separators, 0, null, importType, parms))
					{
						if (cancellationToken.IsCancellationRequested == true)
						{
							break;
						}
						CountRow++;
						if (CountRow % 100 == 0)
						{
							countAction(CountRow);
						}
						CountRow++;

						this._exportProductStreamWriter.AddRowSimple(sw, product, 
							productMakatDictionary, productMakatBarcodesDictionary,
							makatWithoutMask, barcodeWithoutMask, fileType, maxLen,
							invertLetter, rt2lf, separator);
						//WriteRow(sw, product, productWriter, 
						//    productMakatDictionary, productMakatBarcodesDictionary,
						//    makatWithoutMask, barcodeWithoutMask, fileType, maxLen,
						//    invertLetter, rt2lf, separators[0]);
					}
				}
			}
			catch (Exception error)
			{
				_logger.ErrorException("WriteToFile", error);
				this.Log.Add(MessageTypeEnum.ErrorDB, error.Message + ":" + error.StackTrace);
			}

			sw.Close();
			//Localization.Resources.Log_TraceRepositoryResult1058%"Write to File [{0}]"
			this.Log.Add(MessageTypeEnum.SimpleTrace, String.Format(Localization.Resources.Log_TraceRepositoryResult1058, toPathFile));
		}

	
	}
}
