namespace Count4U.Model.Count4U.Translation
{
    public partial class PropertyTranslation
    {
         private void Family()
         {
			 this.Add(TypedReflection<Family>.GetPropertyInfo(r => r.FamilyCode), Localization.Resources.Property_Family_Code);
			 this.Add(TypedReflection<Family>.GetPropertyInfo(r => r.Name), Localization.Resources.Property_Family_Name);
         }
    }
}