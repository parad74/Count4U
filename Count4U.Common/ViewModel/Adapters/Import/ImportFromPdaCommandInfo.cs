namespace Count4U.Common.ViewModel.Adapters.Import
{
    public class ImportFromPdaCommandInfo : ImportCommandInfo
    {
        public bool IsAutoPrint { get; set; }
        public object Report { get; set; }
        public bool IsContinueGrabFiles { get; set; }
    }
}