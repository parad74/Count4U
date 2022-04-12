using System.Collections.Generic;
using System.IO;

namespace Count4U.Model.Transfer
{
    public class ZipPackInfo
    {
        public ZipPackInfo()
        {
            Files = new List<FileInfo>();
            EmptyFolders = new List<DirectoryInfo>();
        }

        public ZipRootFolder RootFolder { get; set; }
        public IList<FileInfo> Files { get; set; }
        public IList<DirectoryInfo> EmptyFolders { get; set; }
    }
}