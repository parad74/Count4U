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
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model
{
	public class ImportPreviousInventoryClalitSdf2SqliteProvider : BaseProvider, IImportProvider
	{
		private readonly IImportPreviousInventorysSQLiteADORepository _importPreviousInventorysSQLiteADORepository;

		public ImportPreviousInventoryClalitSdf2SqliteProvider(
				IImportPreviousInventorysSQLiteADORepository importPreviousInventoryRepository,
				IServiceLocator serviceLocator,
				ILog log)
			: base(log, serviceLocator)
	    {
			this._importPreviousInventorysSQLiteADORepository = importPreviousInventoryRepository;
			this._countExcludeFirstString = 3;
		}

		public void Import()
		{
			this.FillInfoLog(this.FromPathFile, this.GetType().Name, this._importTypes);
			if (this.IsEmptyPath(this.FromPathFile) == true) return;
			if (this.IsEmptyPath(this.ToPathDB) == true) return;
			this._importPreviousInventorysSQLiteADORepository.InsertPreviousInventorys(this.FromPathFile, this.ToPathDB,
			PreviousInventorySQLiteParserEnum.PreviousInventoryClalitSdf2SqliteParser,
			this.ProviderEncoding, this._separators, this._countExcludeFirstString,
			this._importTypes, this.Parms);
		}

		protected override void InitDefault()
		{
			this.ProviderEncoding = Encoding.GetEncoding("windows-1255");
			this._separators = new string[] { SeparatorField.Comma };
			this._countExcludeFirstString = 3;
		}

		public void Clear()
		{
//			ICurrentInventoryAdvancedSourceRepository currentInventoryAdvancedSourceRepository =
//this._serviceLocator.GetInstance<ICurrentInventoryAdvancedSourceRepository>();
//			currentInventoryAdvancedSourceRepository.AlterTableCurrentInventoryAdvanced(this.FromPathFile);

			this._importPreviousInventorysSQLiteADORepository.ClearPreviousInventory(this.ToPathDB);
			this._importPreviousInventorysSQLiteADORepository.VacuumPreviousInventory(this.ToPathDB);
		}



	}
}

