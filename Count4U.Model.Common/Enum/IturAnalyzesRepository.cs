using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model
{
	// как в БД пишем - то что переключаем в конфиге
	public enum IturAnalyzesRepositoryEnum
	{
		//IturAnalyzesADORepository,  // old
		IturAnalyzesBulkRepository,
		IturAnalyzesReaderADORepository
	}

	// тип источника Itur - Family - Supplier - Section  IImportIturAnalyzesBulkRepository + 
	public enum IturAnalyzesTableRepositoryTypeEnum
	{
		IturAnalyzesBulk_IturTypeRepository,
		//IturAnalyzesReaderADO_IturTypeRepository,
		IturAnalyzesBulk_FamilyTypeRepository
		//IturAnalyzesReaderADO_IturTypeRepository,
	}

	public class IturAnalyzesRepositoryCaption
	{
		//public const string IturAnalyzesADORepository = "ADO";
//		public const string IturAnalyzesBulkRepository = "Try Fast";
//		public const string IturAnalyzesReaderADORepository = "Reader ADO";

        public static string IturAnalyzesBulkRepository = Localization.Resources.ViewModel_UserSettings_cmbReportsRepositoryBulk;
        public static string IturAnalyzesReaderADORepository = Localization.Resources.ViewModel_UserSettings_cmbReportsRepositoryADO;
	}
}
