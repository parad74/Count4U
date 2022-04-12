namespace Count4U.Common.Constants
{
    public class FocusValues
    {
        public class InventProductSearch
        {
            public const string Makat = "Makat";
			public const string Code = "Code";
            public const string CodeInputFromPDA = "CodeInputFromPDA";
            public const string ProductName = "ProductName";
			public const string SerialNumber = "SerialNumber";
            public const string IturCode = "IturCode";
			public const string PropertyStr = "PropertyStr";
			public const string PropertyNumber = "PropertyNumber";
			

            public static string MakatText = Localization.Resources.View_IturListDetails_tbMakat;
			public static string CodeText = Localization.Resources.View_IturListDetails_tbCode;
			public static string SerialNumberText = Localization.Resources.View_IturListDetails_tbSerialNumber;
            public static string CodeInputFromPDAText = Localization.Resources.View_IturListDetails_tbCodeInputFromPda;
            public static string ProductNameText = Localization.Resources.View_IturListDetails_tbProductName;
            public static string IturCodeText = Localization.Resources.View_SearchInventProduct_tbIturCode;
        }
    }
}