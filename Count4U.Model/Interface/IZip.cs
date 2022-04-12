using System;
using System.Collections.Generic;
using System.Threading;
using Count4U.Model.Transfer;
using Ionic.Zip;

namespace Count4U.Model.Interface
{
    public interface IZip
    {
        string AppData { get; }
        string ImportData { get; }
        string ImportModules { get; }
        string ReportTemplate { get; }
        string ExportAdapters { get; }
        string ExportData { get; }

        ZipRootFolderInfo GetRootFolder(ZipRootFolder rootFolder);
        string Pack(IList<ZipPackInfo> packages, CancellationToken cancellationToken, Action<double, string, string> progress, String outPackageFolder);
        bool IsZipInCorrectFormat(string path);
        void Unpack(ZipUnpackInfo info, CancellationToken cancellationToken, Action<double, string, string> progress);
        List<string> ValidExtensionsForDbFolder { get; }

        void CreateZip(List<ZipRelativePath> files, string outZipPath);
        string BuildFileName(string prefix = null);
		//Marina
		void DoZipFile(List<string> filesPathList, string outZipPath/*, string directory*/);
		void ReadDb3FromZipFile(string zipFileName, string outputDirectory);
    }
}