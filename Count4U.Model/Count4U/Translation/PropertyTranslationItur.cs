namespace Count4U.Model.Count4U.Translation
{
    public partial class PropertyTranslation
    {
        private void Itur()
        {
            this.Add(TypedReflection<Itur>.GetPropertyInfo(r => r.IturCode), Localization.Resources.Property_Itur_Code);
            this.Add(TypedReflection<Itur>.GetPropertyInfo(r => r.Number), Localization.Resources.Property_Itur_Number);
            this.Add(TypedReflection<Itur>.GetPropertyInfo(r => r.NumberPrefix), Localization.Resources.Property_Itur_NumberPrefix);
            this.Add(TypedReflection<Itur>.GetPropertyInfo(r => r.NumberSufix), Localization.Resources.Property_Itur_NumberSuffix);
            this.Add(TypedReflection<Itur>.GetPropertyInfo(r => r.LocationCode), Localization.Resources.Property_Itur_LocationCode);
            this.Add(TypedReflection<Itur>.GetPropertyInfo(r => r.ERPIturCode), Localization.Resources.Property_Itur_ERPCode);
            this.Add(TypedReflection<Itur>.GetPropertyInfo(r => r.StatusIturGroupBit), Localization.Resources.Property_Itur_StatusGroupBit);            
        }
    }
}