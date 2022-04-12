using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.Interfaces
{
    public interface IMdiChild
    {
        void Refresh();
        void Clear();
    }
}