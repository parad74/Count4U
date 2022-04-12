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
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model
{
	public class ImportIturMultiCsvDelete9999999Provider : BaseProvider, IImportProvider
	{
		private readonly IImportIturRepository _importIturADORepository;


		public ImportIturMultiCsvDelete9999999Provider(IImportIturRepository importIturADORepository,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
		{
			if (importIturADORepository == null) throw new ArgumentNullException("importIturADORepository");
			this._importIturADORepository = importIturADORepository;
			
		}

		public void Import()
		{
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;
			if (this.IsEmptyPath(this.ToPathDB) == true) return;

			IIturRepository iturRepository = _serviceLocator.GetInstance<IIturRepository>();
			IInventProductRepository inventProductRepository = _serviceLocator.GetInstance<IInventProductRepository>();
			Itur itur = iturRepository.GetIturByCode("99999999", this.FromPathFile);
			if (itur == null)
			{
				return;
			}
			else
			{
				bool isAny = inventProductRepository.IsAnyInventProductInIturCode("99999999", this.FromPathFile);
				if (isAny == true)
				{
					return;
				}
				else
				{
					iturRepository.Delete("99999999", this.FromPathFile);
				}
			}
		}

		protected override void InitDefault()
		{
			this.ProviderEncoding = Encoding.GetEncoding("windows-1255");
			this._separators = new string[] { SeparatorField.Comma };
			this._countExcludeFirstString = 0;
		}

		public void Clear()
		{
			this._importIturADORepository.ClearIturs(this.ToPathDB);
		}

	}
}		 
	