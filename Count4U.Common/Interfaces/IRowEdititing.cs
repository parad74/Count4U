namespace Count4U.Common.Interfaces
{
    public interface IRowEdititing
    {
        void BeginEditing();
        void CancelEditing();
        void CommitEditing();
    }
}