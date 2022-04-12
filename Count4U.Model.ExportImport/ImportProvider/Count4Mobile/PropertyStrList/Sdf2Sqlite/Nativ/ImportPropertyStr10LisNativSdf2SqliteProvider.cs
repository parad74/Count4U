using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Core.Objects;
using System.Data;
using Count4U.Model.Interface;
using System.Xml.Linq;
using Count4U.Model;
using Count4U.Model.Count4U;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model
{
	public class ImportPropertyStr10ListNativSdf2SqliteProvider : BaseProvider, IImportProvider
	{
		private readonly IImportPropertyStrListSQLiteADORepository _importPropertyStrListADORepository;

		public ImportPropertyStr10ListNativSdf2SqliteProvider(
				IImportPropertyStrListSQLiteADORepository importPropertyStrListRepository,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
	    {
				this._importPropertyStrListADORepository = importPropertyStrListRepository;
			this._countExcludeFirstString = 1;
		}

		public void Import()
		{
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;	 //GetDbPath					
			if (this.IsEmptyPath(this.ToPathDB) == true) return;		//db3Path
			this._importPropertyStrListADORepository.InsertPropertyStrList(
			this.FromPathFile, 		  //GetDbPath		Count4U
				this.ToPathDB,				 //db3Path		  sqlite
  			PropertyStrListSQLiteTableNameEnum.PropertyStr10List,
			PropStrCodeEnum.PropStr10Code,
 			PropStrNameEnum.PropStr10Name,
			PropertyStrListSQLiteParserEnum.PropertyStrListNativSdf2SqliteParser,
			DomainObjectTypeEnum.PropertyStr10,
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
			this._importPropertyStrListADORepository.ClearPropertyStrList(
				PropertyStrListSQLiteTableNameEnum.PropertyStr10List.ToString(), this.ToPathDB);
		}
	}
}

