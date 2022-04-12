using Count4U.Model.Audit;

namespace Count4U.Model.Count4U.Translation
{
    public partial class PropertyTranslation
    {
        private void Inventor()
        {
            this.Add(TypedReflection<Inventor>.GetPropertyInfo(r => r.Code), Localization.Resources.Property_Inventor_Code);
            this.Add(TypedReflection<Inventor>.GetPropertyInfo(r => r.CustomerCode), Localization.Resources.Property_Inventor_CustomerCode);
            this.Add(TypedReflection<Inventor>.GetPropertyInfo(r => r.BranchCode), Localization.Resources.Property_Inventor_BranchCode);
            this.Add(TypedReflection<Inventor>.GetPropertyInfo(r => r.Description), Localization.Resources.Property_Inventor_Description);
            this.Add(TypedReflection<Inventor>.GetPropertyInfo(r => r.InventorDate), Localization.Resources.Property_Inventor_InventorDate);
        }
    }
}