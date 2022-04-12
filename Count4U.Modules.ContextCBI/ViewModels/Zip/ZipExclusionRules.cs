using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Count4U.Model;
using Count4U.Model.Interface;
using Count4U.Model.Transfer;
using Ionic.Zip;

namespace Count4U.Modules.ContextCBI.ViewModels.Zip
{
    public class ZipPackCondition
    {
        public ZipRootFolder RootFolder { get; set; }
        public string Path { get; set; }
        public int Level { get; set; }
    }

    public class ZipUnpackCondition
    {
        public ZipEntry ZipEntry { get; set; }
    }

    public class ZipExclusionRules
    {
        private const string MainDbSdf = "MainDB.sdf";

        private readonly IDBSettings _dbSettings;

        private readonly List<Func<ZipPackCondition, bool>> _rulesPack;
        private readonly List<Func<ZipUnpackCondition, bool>> _rulesUnpack;        

        public ZipExclusionRules(IDBSettings dbSettings)
        {
            _dbSettings = dbSettings;
            _rulesPack = new List<Func<ZipPackCondition, bool>>();
            _rulesUnpack = new List<Func<ZipUnpackCondition, bool>>();

            BuildPackRules();
            BuildUnpackRules();
        }

        public bool IsAcceptableForPack(ZipRootFolder zipRootFolder, string path, int level)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            if (this._rulesPack.Any(r =>
                                    {
                                        var condition = new ZipPackCondition() { RootFolder = zipRootFolder, Path = path, Level = level};
                                        return r(condition) == false;
                                    }))
                return false;

            return true;
        }

        public bool IsAcceptableForUnpack(ZipEntry zipEntry)
        {
            if (zipEntry == null)
                return false;

            if (this._rulesUnpack.Any(r =>
            {
                var condition = new ZipUnpackCondition() { ZipEntry = zipEntry };
                return r(condition) == false;
            }))
                return false;

            return true;
        }

        private void BuildPackRules()
        {
            this._rulesPack.Add(r =>
            {
                if (r.RootFolder == ZipRootFolder.Db)
                {
                    if (File.Exists(r.Path)) //if file
                    {
                        FileInfo fi = new FileInfo(r.Path);

                        if (fi.Extension != ".sdf" &&
                            fi.Extension != ".png" &&
                            fi.Extension != ".jpg" &&
                            fi.Extension != ".jpeg" &&
                            fi.Extension != ".gif")
                            return false;
                    }
                }

                return true; //default is OK
            });

            this._rulesPack.Add(r =>
            {
                if (r.RootFolder == ZipRootFolder.Db)
                {
                    if (File.Exists(r.Path)) //if file
                    {
                        FileInfo fi = new FileInfo(r.Path);

                        if (FileSystem.IsAppRedactionOffice() == false) //laptop
                        {
                            if (fi.Name == MainDbSdf)
                            {
                                return false;
                            }
                        }
                    }
                }

                return true; //default is OK
            });


            this._rulesPack.Add(r =>
            {
                if (r.RootFolder == ZipRootFolder.Db && r.Level == 1)
                {
                    if (Directory.Exists(r.Path)) //if directory
                    {
                     DirectoryInfo di = new DirectoryInfo(r.Path);
                        string dirName = di.Name;
                        if (dirName != this._dbSettings.FolderLogoFile &&
                            dirName != this._dbSettings.FolderCustomer &&
                            dirName != this._dbSettings.FolderBranch &&
                            dirName != this._dbSettings.FolderInventor)
                            return false;
                    }
                }

                return true; //default is OK
            });

            this._rulesPack.Add(r=>
                {
                    if (r.RootFolder == ZipRootFolder.Adapters || r.RootFolder == ZipRootFolder.ExportAdapters)
                    {
                        if (File.Exists(r.Path))
                        {
                            FileInfo fi = new FileInfo(r.Path);

                            if (fi.Extension == ".pdb")
                                return false;
                        }
                    }
                    return true;
                });
        }

        private void BuildUnpackRules()
        {
            this._rulesUnpack.Add(r =>
                                 {                                     
                                       if(r.ZipEntry.IsDirectory == false &&
                                         r.ZipEntry.FileName.EndsWith(MainDbSdf) &&
                                         FileSystem.IsAppRedactionOffice()) //offfice
                                         return false;

                                     return true;
                                 });
        }
    }
}