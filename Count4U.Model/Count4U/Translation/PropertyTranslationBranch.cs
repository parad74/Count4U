using Count4U.Model.Main;

namespace Count4U.Model.Count4U.Translation
{
    public partial class PropertyTranslation
    {
        private void Branch()
        {
            this.Add(TypedReflection<Branch>.GetPropertyInfo(r => r.Code), Localization.Resources.Property_Branch_Code);
            this.Add(TypedReflection<Branch>.GetPropertyInfo(r => r.CustomerCode), Localization.Resources.Property_Branch_CustomerCode);
            this.Add(TypedReflection<Branch>.GetPropertyInfo(r => r.Name), Localization.Resources.Property_Branch_Name);
            this.Add(TypedReflection<Branch>.GetPropertyInfo(r => r.BranchCodeLocal), Localization.Resources.Property_Branch_CodeLocal);
            this.Add(TypedReflection<Branch>.GetPropertyInfo(r => r.BranchCodeERP), Localization.Resources.Property_Branch_CodeERP);
            this.Add(TypedReflection<Branch>.GetPropertyInfo(r => r.ContactPerson), Localization.Resources.Property_Branch_ContactPerson);
            this.Add(TypedReflection<Branch>.GetPropertyInfo(r => r.Phone), Localization.Resources.Property_Branch_Phone);            
        }
    }
}