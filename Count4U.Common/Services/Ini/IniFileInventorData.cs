namespace Count4U.Common.Services.Ini
{
    public class IniFileInventorData
    {
        //customer
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }

        //branch
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string BranchCodeLocal { get; set; }
        public string BranchCodeERP { get; set; }

        //global
        public string InventorCode { get; set; }
        public string SDFPath { get; set; }
        public string InDataFolderPath { get; set; }
        public string ExportToPDAPath { get; set; }
        public string ExportToERPPath { get; set; }
        public string ProgramType { get; set; }
    }
}