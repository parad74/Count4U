using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model
{

	public enum PriceCodeEnum
	{
		PriceBuy,
		PriceSale,
		PriceExtra
	}

	public static class PriceCodeCaption
	{
        public static string PriceBuy = Localization.Resources.Property_PriceType_PriceBuy;
        public static string PriceSale = Localization.Resources.Property_PriceType_PriceSale;
        public static string PriceExtra = Localization.Resources.Property_PriceType_PriceExtra;

		public static double GetPriceByPriceCode(this PriceCodeEnum priceCode, double priceBuy, double priceSale, double priceExtra)
		{
			if (priceCode == PriceCodeEnum.PriceBuy) return priceBuy;
			else if (priceCode == PriceCodeEnum.PriceSale) return priceSale;
			else if (priceCode == PriceCodeEnum.PriceExtra) return priceExtra;
			else return priceBuy;
		}
	}

		
}
