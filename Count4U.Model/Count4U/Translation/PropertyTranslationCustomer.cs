using Count4U.Model.Main;

namespace Count4U.Model.Count4U.Translation
{
    public partial class PropertyTranslation
    {
        private void Customer()
        {
            this.Add(TypedReflection<Customer>.GetPropertyInfo(r => r.Code), Localization.Resources.Property_Customer_Code);
            this.Add(TypedReflection<Customer>.GetPropertyInfo(r => r.Name), Localization.Resources.Property_Customer_Name);
            this.Add(TypedReflection<Customer>.GetPropertyInfo(r => r.Description), Localization.Resources.Property_Customer_Description);
            this.Add(TypedReflection<Customer>.GetPropertyInfo(r => r.ContactPerson), Localization.Resources.Property_Customer_ContactPerson);
            this.Add(TypedReflection<Customer>.GetPropertyInfo(r => r.Phone), Localization.Resources.Property_Customer_Phone);
            this.Add(TypedReflection<Customer>.GetPropertyInfo(r => r.Address), Localization.Resources.Property_Customer_Address);
        }
    }
}