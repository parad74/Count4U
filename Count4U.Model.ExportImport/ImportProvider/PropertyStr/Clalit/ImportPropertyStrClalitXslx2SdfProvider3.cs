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
	public class ImportPropertyStrClalitXslx2SdfProvider3 : BaseProvider, IImportProvider
	{
		private readonly IImportPropertyStrRepository _importPropertyStrADORepository;

		public ImportPropertyStrClalitXslx2SdfProvider3(
				IImportPropertyStrRepository importPropertyStrADORepository,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
		{
			if (importPropertyStrADORepository == null) throw new ArgumentNullException("ImportEmployeeMerkavaSqliteXslxProvider");
			this._importPropertyStrADORepository = importPropertyStrADORepository;
			this._importTypes.Add(ImportDomainEnum.ClearByDomainObjectType);
		}

		public void Import()
		{
			//Field3: model_id
			//Field4: model_name
			// PropertyStr4		  
			//CountExcludeFirstRow = 8	 // col =  3
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;
			if (this.IsEmptyPath(this.ToPathDB) == true) return;
			this._importPropertyStrADORepository.InsertPropertyStrs(this.FromPathFile, this.ToPathDB,
			PropertyStrParserEnum.PropertyStrClalitXslx2SdfParser3,
			DomainObjectTypeEnum.PropertyStr4,
			this.ProviderEncoding, this._separators, this._countExcludeFirstString,
			this._importTypes, this.Parms);
		}

		protected override void InitDefault()
		{
			this.ProviderEncoding = Encoding.GetEncoding("windows-1255");
			this._separators = new string[] { SeparatorField.Comma };
			this._countExcludeFirstString = 8;
		}

		public void Clear()
		{
			this._importPropertyStrADORepository.ClearPropertyStrs(DomainObjectTypeEnum.PropertyStr4, this.ToPathDB);
		}

	}
}		 
	