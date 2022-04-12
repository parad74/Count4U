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

namespace Count4U.Model
{
	public class ImportStatusInventProductNativPlusSqlite2SdfProvider : BaseProvider, IImportProvider
	{

		private readonly IImportStatusInventProductBlukRepository _importstatusInventProductRepository;
	//	private readonly IImportInventProductRepository _importInventProductRepository;

		public ImportStatusInventProductNativPlusSqlite2SdfProvider(
			//	IImportInventProductRepository importInventProductRepository,
				//IImportStatusInventProductBlukRepository importStatusInventProductRepository,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
		{
			//if (_importstatusInventProductRepository == null) throw new ArgumentNullException("importStatusInventProductRepository");
			//this._importstatusInventProductRepository = importStatusInventProductRepository;
			_importstatusInventProductRepository = this._serviceLocator.GetInstance<IImportStatusInventProductBlukRepository>();
		
		}

		public void Import()
		{
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (base.IsEmptyPath(this.FromPathFile) == true) return;
			if (base.IsEmptyPath(this.ToPathDB) == true) return;
			this._importstatusInventProductRepository.InsertStatusInventProducts(this.FromPathFile, this.ToPathDB,
			StatusInventProductSimpleParserEnum.StatusInventProductNativPlusSqlite2SdfParser,
			//	InventProductSimpleParserEnum.InventProductParser,	 
			 this.ProviderEncoding, this._separators, this._countExcludeFirstString,
			 this._importTypes, this.Parms);

		}

		protected override void InitDefault()
		{
			this.ProviderEncoding = Encoding.GetEncoding("windows-1255");
			this._separators = new string[] { SeparatorField.I };
			this._countExcludeFirstString = 0;
		}

		public void Clear()
		{
			this._importstatusInventProductRepository.ClearStatusInventProducts(this.ToPathDB);
	
		}
	}
}	   	
