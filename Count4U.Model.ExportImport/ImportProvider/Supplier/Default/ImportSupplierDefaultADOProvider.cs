using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Core.Objects;
using System.Data.SqlServerCe;
using System.Data;
using Count4U.Model.Interface;
using System.Xml.Linq;
using Count4U.Model;
using Count4U.Model.Count4U;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model
{
	public class ImportSupplierDefaultADOProvider : BaseProvider, IImportProvider
	{
		private readonly IImportSupplierRepository _importSupplierRepository;

		public ImportSupplierDefaultADOProvider(IImportSupplierRepository importSupplierRepository,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
		{
			if (importSupplierRepository == null) throw new ArgumentNullException("importSupplierRepository");
			this._importSupplierRepository = importSupplierRepository;
			this._importTypes.Add(ImportDomainEnum.ImportSupplier);
		}

		public void Import()
		{
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;
			if (this.IsEmptyPath(this.ToPathDB) == true) return;
			this._importSupplierRepository.InsertSuppliers(this.FromPathFile, this.ToPathDB,
				SupplierParserEnum.SupplierDefaultParser,
			this.ProviderEncoding, this._separators, this._countExcludeFirstString,
			this._importTypes, this.Parms);
		}

		protected override void InitDefault()
		{
			this.ProviderEncoding = Encoding.GetEncoding("windows-1255");
			this._separators = new string[] { SeparatorField.Comma };
		}

		public void Clear()
		{
			this._importSupplierRepository.ClearSuppliers(this.ToPathDB);
		}

	}
}		 
	