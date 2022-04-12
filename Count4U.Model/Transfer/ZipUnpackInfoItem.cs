using System.Collections.Generic;
using Ionic.Zip;

namespace Count4U.Model.Transfer
{
    public class ZipUnpackInfo
    {
        public ZipUnpackInfo()
        {
            Items = new List<ZipUnpackInfoItem>();
            IsOverwrite = true;
        }

        public string ZipFilePath { get; set; }
        public IList<ZipUnpackInfoItem> Items { get; set; }
        public bool IsOverwrite { get; set; }
        public bool IsClearImportData { get; set; }
    }
    public class ZipUnpackInfoItem
    {
        public ZipRootFolder RootFolder { get; set; }
        public IList<ZipEntry> Entries { get; set; }
    }
}