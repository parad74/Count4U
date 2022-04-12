namespace Count4U.Model.Count4U.Translation
{
    public partial class PropertyTranslation
    {
        private void Location()
        {
            this.Add(TypedReflection<Location>.GetPropertyInfo(r => r.Code), Localization.Resources.Property_Location_Code);
            this.Add(TypedReflection<Location>.GetPropertyInfo(r => r.Name), Localization.Resources.Property_Location_Name);
        }
    }
}