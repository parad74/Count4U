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
	public class ImportPropertyStr7MerkavaXslx2SdfProvider1 : BaseProvider, IImportProvider
	{
		private readonly IImportPropertyStrRepository _importPropertyStrADORepository;

		public ImportPropertyStr7MerkavaXslx2SdfProvider1(IImportPropertyStrRepository importPropertyStrADORepository,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
		{
			if (importPropertyStrADORepository == null) throw new ArgumentNullException("ImportPropertyStr7MerkavaXslx2SdfProvider1");
			this._importPropertyStrADORepository = importPropertyStrADORepository;
			this._importTypes.Add(ImportDomainEnum.ClearByDomainObjectType);
		}

		public void Import()
		{
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;
			if (this.IsEmptyPath(this.ToPathDB) == true) return;
			this._importPropertyStrADORepository.InsertPropertyStrs(this.FromPathFile, this.ToPathDB,
			PropertyStrParserEnum.PropertyStr6MerkavaXslx2SdfParser1,	//парсит 6 , добавляем в 7
			DomainObjectTypeEnum.PropertyStr7,	   //это правильно	 добавляем в 7
			this.ProviderEncoding, this._separators, this._countExcludeFirstString,
			this._importTypes, this.Parms);
		}

		protected override void InitDefault()
		{
			this.ProviderEncoding = Encoding.GetEncoding("windows-1255");
			this._separators = new string[] { SeparatorField.Comma };
			this._countExcludeFirstString = 1;
		}

		public void Clear()
		{
			this._importPropertyStrADORepository.ClearPropertyStrs(DomainObjectTypeEnum.PropertyStr7, this.ToPathDB);
		}

	}
}		 
	