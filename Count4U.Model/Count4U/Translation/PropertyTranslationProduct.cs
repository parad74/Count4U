namespace Count4U.Model.Count4U.Translation
{
    public partial class PropertyTranslation
    {
        private void Product()
        {
            this.Add(TypedReflection<Product>.GetPropertyInfo(r => r.Makat), Localization.Resources.Property_Product_Makat);
            this.Add(TypedReflection<Product>.GetPropertyInfo(r => r.Barcode), Localization.Resources.Property_Product_Barcode);
            this.Add(TypedReflection<Product>.GetPropertyInfo(r => r.Name), Localization.Resources.Property_Product_ProductName);
            this.Add(TypedReflection<Product>.GetPropertyInfo(r => r.PriceSale), Localization.Resources.Property_Product_PriceSale);
            this.Add(TypedReflection<Product>.GetPropertyInfo(r => r.PriceBuy), Localization.Resources.Property_Product_PriceBuy);
            this.Add(TypedReflection<Product>.GetPropertyInfo(r => r.SupplierCode), Localization.Resources.Property_Product_Supplier);
            this.Add(TypedReflection<Product>.GetPropertyInfo(r => r.SectionCode), Localization.Resources.Property_Product_Section);
        }
    }
}