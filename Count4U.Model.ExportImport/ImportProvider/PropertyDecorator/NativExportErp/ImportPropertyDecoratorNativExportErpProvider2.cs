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
	public class ImportPropertyDecoratorNativExportErpProvider2 : BaseProvider, IImportProvider
	{
	//	private readonly IImportBuildingConfigADORepository _importBuildingConfigADORepository;
		private readonly IImportPropertyStrRepository _importPropertyStrADORepository;

		public ImportPropertyDecoratorNativExportErpProvider2(
				//IImportBuildingConfigADORepository importBuildingConfigRepository,
				IImportPropertyStrRepository importPropertyStrADORepository,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
	    {
			//if (importBuildingConfigRepository == null) throw new ArgumentNullException("importBuildingConfigRepository");
			//this._importBuildingConfigADORepository = importBuildingConfigRepository;
			if (importPropertyStrADORepository == null) throw new ArgumentNullException("ImportPropertyDecoratorNativExportErpProvider");
			this._importPropertyStrADORepository = importPropertyStrADORepository;
			this._importTypes.Add(ImportDomainEnum.ClearByDomainObjectType);

		}

		public void Import()
		{
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;
			if (this.IsEmptyPath(this.ToPathDB) == true) return;
	
		this._importPropertyStrADORepository.InsertPropertyStrs(this.FromPathFile, this.ToPathDB,
		PropertyStrParserEnum.PropertyStrPropertyDecoratorNativExportErpParser2,
		DomainObjectTypeEnum.PropertyExportErpDecorator2,
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
		//	this._importBuildingConfigADORepository.ClearBuildingConfig(this.ToPathDB);
			this._importPropertyStrADORepository.ClearPropertyStrs(DomainObjectTypeEnum.PropertyExportErpDecorator2, this.ToPathDB);
		}



	}
}

