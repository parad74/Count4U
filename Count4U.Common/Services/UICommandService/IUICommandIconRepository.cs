namespace Count4U.Common.Services.UICommandService
{
    public interface IUICommandIconRepository
    {
        string GetIcon(enUICommand type, enIconSize size);
    }
}