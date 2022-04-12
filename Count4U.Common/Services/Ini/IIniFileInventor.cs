namespace Count4U.Common.Services.Ini
{
    public interface IIniFileInventor
    {
        void Save(IniFileInventorData data);
        string BuildParamsFolderPath();
    }
}