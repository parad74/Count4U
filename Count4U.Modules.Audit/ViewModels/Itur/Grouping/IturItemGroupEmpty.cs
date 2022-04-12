namespace Count4U.Modules.Audit.ViewModels
{
    public class IturItemGroupEmpty
    {
        public IturItemGroupEmpty()
        {
        }

        public string Value
        {
            get { return string.Empty; }
        }

        public bool IsVisible
        {
            get { return false; }
        }

        public override bool Equals(object obj)
        {
            return true;
        }     

        public override int GetHashCode()
        {
            return 0;
        }
    }
}