using System;
using Count4U.Model.SelectionParams;

namespace Count4U.Common.Constants
{
    public class ComboValues
    {
        public class FindInventProduct
        {
            public static string FilterMakat = Localization.Resources.Constant_Makat;
            public static string FilterBarcode = Localization.Resources.Constant_Barcode;
            public static string FilterProduct = Localization.Resources.Constant_Product;
        }

        public class FindItur
        {
            public static string IturNumber = Localization.Resources.Constant_Number;
            public static string IturERP = Localization.Resources.Constant_ERP;

            public const string FilterIturNumber = "IturNumber";
            public const string FilterIturERP = "IturERP";
        }
		public class FindIturSortAZ
        {
			public static string ASC = Localization.Resources.Constant_AZ;
			public static string DESC = Localization.Resources.Constant_ZA;

			public const string SortASC = "ASC";
            public const string SortDESC = "DESC";
        }

		public class FindIturSort
		{
			public static string Empty = "";
			public static string Number = Localization.Resources.Property_Itur_Number;
			public static string IturCode = Localization.Resources.Property_Itur_Code;
			public static string NumberPrefix = Localization.Resources.Property_Itur_NumberPrefix;
			public static string NumberSufix = Localization.Resources.Property_Itur_NumberSuffix;
			public static string LocationCode = Localization.Resources.Property_Itur_LocationCode;
			public static string ERPIturCode = Localization.Resources.Property_Itur_ERPCode;
			public static string StatusIturGroupBit = Localization.Resources.Property_Itur_StatusGroupBit;

			public const string FilterEmpty = "Empty";
			public const string FilterNumber = "Number";
			public const string FilterIturCode = "IturCode";
			public const string FilterNumberPrefix = "NumberPrefix";
			public const string FilterNumberSufix = "NumberSufix";
			public const string FilterLocationCode = "LocationCode";
			public const string FilterERPIturCode = "ERPIturCode";
			public const string FilterStatusIturGroupBit = "StatusIturGroupBit";
		}

		  //sortProperties.Add(TypedReflection<Count4U.Model.Count4U.Itur>.GetPropertyInfo(r => r.IturCode));
		  //sortProperties.Add(TypedReflection<Count4U.Model.Count4U.Itur>.GetPropertyInfo(r => r.Number));
		  //sortProperties.Add(TypedReflection<Count4U.Model.Count4U.Itur>.GetPropertyInfo(r => r.NumberPrefix));
		  //sortProperties.Add(TypedReflection<Count4U.Model.Count4U.Itur>.GetPropertyInfo(r => r.NumberSufix));
		  //sortProperties.Add(TypedReflection<Count4U.Model.Count4U.Itur>.GetPropertyInfo(r => r.LocationCode));
		  //sortProperties.Add(TypedReflection<Count4U.Model.Count4U.Itur>.GetPropertyInfo(r => r.ERPIturCode));
		  //sortProperties.Add(TypedReflection<Count4U.Model.Count4U.Itur>.GetPropertyInfo(r => r.StatusIturGroupBit));

		public class SortFileGrid
		{
			public static string DateAndTime = "Date and Time";
			public static string FileName ="File";	
            public static string Description ="Description";	
            public static string Date ="Date";	
            
            
		}

        public class ImportExportLogLevel
        {
            public const string Simple = "Simple";
            public const string Info = "Info";
            public const string Advanced = "Advanced";
            public const string Configurable = "Configurable";
        }

        public class SortItur
        {
            public static string LocationText = Localization.Resources.ViewModel_IturListDetails_SortLocation;
            public static string StatusText = Localization.Resources.ViewModel_IturListDetails_SortStatusGroup;
            public static string NumberText = Localization.Resources.ViewModel_IturListDetails_SortNumber;
			public static string IturERPCodeText = Localization.Resources.ViewModel_IturListDetails_SortIturERPCode;

            public const string LocationValue = "Location";
            public const string StatusValue = "Status";
            public const string NumberValue = "Number";
			public const string IturERPCodeValue = "IturERPCode";
			
        }

        public class GroupItur
        {
            public static string EmptyText = "";
            public static string LocationText = Localization.Resources.ViewModel_IturListDetails_GroupLocation;
			public static string TagText = Localization.Resources.ViewModel_IturListDetails_GroupTag;
            public static string StatusText = Localization.Resources.ViewModel_IturListDetails_GroupStatus;
			//public static string StatusNotEmptyText = Localization.Resources.ViewModel_IturListDetails_GroupStatusNotEmpty;

            public const string EmptyValue = "Empty";
            public const string LocationValue = "Location";
			public const string TagValue = "Tag";
            public const string StatusValue = "Status";
			//public const string StatusNotEmptyValue = "StatusNotEmpty";
        }

        public class AllValue
        {
            public const string All = "All";
            public static readonly string AllName = Localization.Resources.ViewModel_ContextCBIChoose_All;
        }

		
        public class IturListDetailsMode
        {
            public const string ModePaged = "ModePaged";
            public const string ModeLocation = "ModeLocation";
			public const string ModeTag = "ModeTag";
			public const string ModeStatus = "ModeStatus";

            public static string ModePagedText = Localization.Resources.View_IturListDetails_tbModePaging;
            public static string ModeLocationText = Localization.Resources.View_IturListDetails_tbModeLocation;
			public static string ModeTagText = Localization.Resources.View_IturListDetails_tbModeTag;
			public static string ModeStatusText = Localization.Resources.View_IturListDetails_tbModeStatus;
        }

        public static class Equality
        {
            public const string Equal = "=";
            public const string Greater = ">";
            public const string Less = "<";
            public const string GreaterOrEqual = ">=";
            public const string LessOrEqual = "<=";

            public static FilterOperator ToOperator(string str)
            {
                switch (str)
                {
                    case Equal:
                        return FilterOperator.Equal;
                    case Greater:
                        return FilterOperator.Greater;
                    case Less:
                        return FilterOperator.Less;
                    case GreaterOrEqual:
                        return FilterOperator.GreaterOrEqual;
                    case LessOrEqual:
                        return FilterOperator.LessOrEqual;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            public static bool IsAvailableForAbsoluteValue(string strOperator)
            {
                FilterOperator op = ToOperator(strOperator);

                switch (op)
                {
                    case FilterOperator.Less:
                    case FilterOperator.LessOrEqual:
                    case FilterOperator.Greater:
                    case FilterOperator.GreaterOrEqual:
                        return true;

                    case FilterOperator.None:
                    case FilterOperator.Multiple:
                    case FilterOperator.Equal:
                    case FilterOperator.Contains:
                    case FilterOperator.StartsWith:
                    case FilterOperator.EndsWith:
                    case FilterOperator.DateTimeGreaterOrEqual:
                    case FilterOperator.DateTimeLessOrEqual:
                    case FilterOperator.DateTimeBetween:
                    case FilterOperator.MultipleString:
                    default:
                        return false;
                }
            }
        }
    }

	public class WarningAdapter
	{
		public static readonly string AdapterInCustomerNotSet = Localization.Resources.Warning_AdapterInCustomerNotSet;
	}
}