namespace Count4U.Model.Count4U.Translation
{
    public partial class PropertyTranslation
    {
        private void InventProduct()
        {
            this.Add(TypedReflection<InventProduct>.GetPropertyInfo(r => r.Makat), Localization.Resources.Property_InventProduct_Makat);
            this.Add(TypedReflection<InventProduct>.GetPropertyInfo(r => r.Barcode), Localization.Resources.Property_InventProduct_Barcode);            
            this.Add(TypedReflection<InventProduct>.GetPropertyInfo(r => r.ImputTypeCodeFromPDA), Localization.Resources.Property_InventProduct_CodeFromPda);
            this.Add(TypedReflection<InventProduct>.GetPropertyInfo(r => r.ProductName), Localization.Resources.Property_InventProduct_ProductName);
            this.Add(TypedReflection<InventProduct>.GetPropertyInfo(r => r.SectionCode), Localization.Resources.Property_InventProduct_SectionCode);
            this.Add(TypedReflection<InventProduct>.GetPropertyInfo(r => r.PriceBuy), Localization.Resources.Property_InventProduct_PriceBuy);
            this.Add(TypedReflection<InventProduct>.GetPropertyInfo(r => r.PriceSale), Localization.Resources.Property_InventProduct_PriceSell);
            this.Add(TypedReflection<InventProduct>.GetPropertyInfo(r => r.IturCode), Localization.Resources.Property_InventProduct_IturCode);
			this.Add(TypedReflection<InventProduct>.GetPropertyInfo(r => r.ERPIturCode), Localization.Resources.Property_InventProduct_ERPIturCode);
           
        }
    }
}