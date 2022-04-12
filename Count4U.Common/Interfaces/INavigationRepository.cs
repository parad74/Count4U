namespace Count4U.Common.Interfaces
{
    public interface INavigationRepository
    {
        string Add(object obj);
        object Get(string key);
        void Remove(string key);
    }
}