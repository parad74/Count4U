using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Core.Objects;
using Count4U.Model.Interface;
using System.Data.SqlServerCe;
using System.Data;
using System.Xml.Linq;
using Count4U.Model;
using Count4U.Model.Count4U;
using Microsoft.Practices.ServiceLocation;
using System.IO;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model
{
	public class ImportInventProductMisAndDefaultADOProvider : BaseProvider, IImportProvider
	{
		private readonly IImportInventProductRepository _importInventProductRepository;

		public ImportInventProductMisAndDefaultADOProvider(
				IImportInventProductRepository importInventProductRepository,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
		{
			if (importInventProductRepository == null) throw new ArgumentNullException("importInventProductRepository");
			this._importInventProductRepository = importInventProductRepository;
		
			this._importTypes.Add(ImportDomainEnum.ImportInventProduct);
			//this._importTypes.Add(ImportDomainEnum.ImportDocumentHeader);
			this._importTypes.Add(ImportDomainEnum.ImportItur);
			//this._importTypes.Add(ImportDomainEnum.ImportParentProductAdvanced);
			//this._importTypes.Add(ImportDomainEnum.ImportSession);
			this._importTypes.Add(ImportDomainEnum.ExistItur);
			this._importTypes.Add(ImportDomainEnum.ExistMakat);
			//this._importType.Add(ImportDomainEnum.ImportMakat);
		}

		public void Import()
		{
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (base.IsEmptyPath(this.FromPathFile) == true) return;
			if (base.IsEmptyPath(this.ToPathDB) == true) return;
			string header = FirstLineFromFile(this.FromPathFile);
			string h2 = "H,";
			if (header.Length > 1)
			{
				h2 = header.Substring(0, 2);
			}
			if (h2.ToUpper() == "H|")
			{
				this._separators = new string[] { SeparatorField.I };
				this._importInventProductRepository.InsertInventProducts(this.FromPathFile, this.ToPathDB,
				InventProductSimpleParserEnum.InventProductMisParser,
					//	InventProductSimpleParserEnum.InventProductParser,	 
				 this.ProviderEncoding, this._separators, this._countExcludeFirstString,
				 this._importTypes, this.Parms);
			}
			else 
			{
				this._separators = new string[] { SeparatorField.Comma };
				this._importInventProductRepository.InsertInventProducts(this.FromPathFile, this.ToPathDB,
			InventProductSimpleParserEnum.InventProductSimpleParser,
					//	InventProductSimpleParserEnum.InventProductParser,	 
			 this.ProviderEncoding, this._separators, this._countExcludeFirstString,
			 this._importTypes, this.Parms);
			}
		}

		protected override void InitDefault()
		{
			this.ProviderEncoding = Encoding.GetEncoding("windows-1255");
			//this._separators = new string[] { SeparatorField.I };
			this._countExcludeFirstString = 0;
		}

		public void Clear()
		{
			this._importInventProductRepository.ClearInventProducts(this.ToPathDB);
			this._importInventProductRepository.ClearDocumentHeaders(this.ToPathDB);
			this._importInventProductRepository.ClearSession(this.ToPathDB);
		}

		private string FirstLineFromFile(string filePath)
		{
			if (!String.IsNullOrEmpty(filePath) && File.Exists(filePath))
			{
				IEnumerable<string> lines;
				Encoding encoding = this.ProviderEncoding ?? Encoding.GetEncoding(1255);
				string extension = Path.GetExtension(filePath);
				//if (extension == ".xlsx")
				//{
				//	IFileParser fileParser = this._serviceLocator.GetInstance<IFileParser>(FileParserEnum.ExcelFileParser.ToString());
				//	lines = fileParser.GetRecords(filePath, encoding, 0);
				//}
				//else
				if (extension != ".xlsx")
				{
					return File.ReadLines(filePath, encoding).FirstOrDefault(); 
				}
				//if (lines == null)
				//	return String.Empty;
				//if (!lines.Any())
				//	return String.Empty;
				//int count = lines.Count();
				//if (count == 0)
				//	return String.Empty;
				//if (count == 1 && lines.First() == null)
				//	return String.Empty;
				//return lines.Select(x=>x).Take(1).ToString();
			}
			return String.Empty;
		}
	}
}	   	
