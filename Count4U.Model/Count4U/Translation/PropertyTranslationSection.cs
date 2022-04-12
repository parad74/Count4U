namespace Count4U.Model.Count4U.Translation
{
    public partial class PropertyTranslation
    {
        private void Section()
        {
            this.Add(TypedReflection<Section>.GetPropertyInfo(r => r.SectionCode), Localization.Resources.Property_Section_Code);
            this.Add(TypedReflection<Section>.GetPropertyInfo(r => r.Name), Localization.Resources.Property_Section_Name);
            this.Add(TypedReflection<Section>.GetPropertyInfo(r => r.Tag), Localization.Resources.Property_Section_Tag);
            
        }
    }
}