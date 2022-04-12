using Count4U.Common.Helpers;
using Count4U.Model.Count4U;
using Microsoft.Practices.Prism.ViewModel;
using System;

namespace Count4U.Modules.Audit.ViewModels.Catalog
{
    public class ProductItemViewModel : NotificationObject
    {
        private Product _product;

        public ProductItemViewModel(Product product)
        {
            this._product = product;
        }

        public string TypeCode
        {
            get { return this._product.TypeCode; }
        }

        public string InputTypeCode
        {
            get { return this._product.InputTypeCode; }
        }

        public string Makat
        {
            get { return this._product.Makat; }
        }

        public string Name
        {
            get { return this._product.Name; }
        }

        public string Family
        {
            get { return this._product.Family; }
        }

		public string FamilyCode
		{
			get { return this._product.FamilyCode; }
		}

        public string CreateDate
        {
            get { return this._product.CreateDate.ToShortDateString(); }
        }

        public string ModifyDate
        {
            get { return this._product.ModifyDate == null ? String.Empty : this._product.ModifyDate.Value.ToShortDateString(); }
        }

        public string PriceSale
        {
            get { return UtilsConvert.DecimalPlaceNoRounding(this._product.PriceSale); }
        }

        public string PriceBuy
        {
            get { return UtilsConvert.DecimalPlaceNoRounding(this._product.PriceBuy); }
        }

        public string PriceExtra
        {
            get { return UtilsConvert.DecimalPlaceNoRounding(this._product.PriceExtra); }
        }

        public string PriceString
        {
            get { return this._product.PriceString; }
        }

        public string ParentMakat
        {
            get { return this._product.ParentMakat; }
        }

        public string MakatOriginal
        {
            get { return this._product.MakatOriginal; }
        }

        public string SectionCode
        {
            get { return this._product.SectionCode; }
        }

        public string SupplierCode
        {
            get { return this._product.SupplierCode; }
        }

        public string CountInParentPack
        {
            get { return this._product.CountInParentPack.ToString(); }
        }

        public string BalanceQuantityERP
        {
            //get { return this._product.BalanceQuantityERP == null ? String.Empty : UtilsConvert.HebrewDouble(this._product.BalanceQuantityERP); }
			get { return UtilsConvert.HebrewDouble(this._product.BalanceQuantityERP); }
        }

		public string UnitTypeCode
		{
			get { return this._product.UnitTypeCode; }
		}

		public string Tag
		{
			get { return this._product.Tag; }
		}

		public string IturCodeExpected
		{
			get { return this._product.IturCodeExpected; }
		}

        public string BalanceQuantityPartialERP
        {
            get { return this._product.BalanceQuantityPartialERP == null ? String.Empty : UtilsConvert.HebrewInt(this._product.BalanceQuantityPartialERP.Value); }
        }

        public Product Product
        {
            get { return this._product; }
        }

        public void ProductSet(Product product)
        {
            this._product = product;

            this.RaisePropertyChanged(() => this.TypeCode);
            this.RaisePropertyChanged(() => this.InputTypeCode);
            this.RaisePropertyChanged(() => this.Makat);
            this.RaisePropertyChanged(() => this.Name);
            this.RaisePropertyChanged(() => this.CreateDate);
            this.RaisePropertyChanged(() => this.ModifyDate);
            this.RaisePropertyChanged(() => this.PriceSale);
            this.RaisePropertyChanged(() => this.PriceBuy);
            this.RaisePropertyChanged(() => this.PriceString);
            this.RaisePropertyChanged(() => this.PriceExtra);
            this.RaisePropertyChanged(() => this.MakatOriginal);
            this.RaisePropertyChanged(() => this.SectionCode);
            this.RaisePropertyChanged(() => this.SupplierCode);
            this.RaisePropertyChanged(() => this.CountInParentPack);
            this.RaisePropertyChanged(() => this.BalanceQuantityERP);
            this.RaisePropertyChanged(() => this.Family);
			this.RaisePropertyChanged(() => this.Tag);
			
        }
    }
}