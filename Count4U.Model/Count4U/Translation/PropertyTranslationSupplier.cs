namespace Count4U.Model.Count4U.Translation
{
    public partial class PropertyTranslation
    {
         private void Supplier()
         {
             this.Add(TypedReflection<Supplier>.GetPropertyInfo(r => r.SupplierCode), Localization.Resources.Property_Supplier_Code);
             this.Add(TypedReflection<Supplier>.GetPropertyInfo(r => r.Name), Localization.Resources.Property_Supplier_Name);
         }
    }
}