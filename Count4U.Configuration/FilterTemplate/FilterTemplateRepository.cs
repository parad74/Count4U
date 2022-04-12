using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Count4U.Common.Interfaces;
using Count4U.Model.Interface;

namespace Count4U.Configuration.FilterTemplate
{
    public class FilterTemplateRepository : IFilterTemplateRepository
    {
        private readonly IDBSettings _dbSettings;
        private readonly string _rootFolder;

        public FilterTemplateRepository(IDBSettings dbSettings)
        {
            _dbSettings = dbSettings;
            _rootFolder = _dbSettings.UIFilterTemplateSetFolderPath();
        }

        public List<FileInfo> GetFiles(string context)
        {
            List<FileInfo> result = new List<FileInfo>();

            string path = GetPathByContext(context);

            foreach (string file in Directory.GetFiles(path, "*.xml"))
            {
                result.Add(new FileInfo(file));
            }

            return result;
        }

        public object GetData(FileInfo file, Type type)
        {
			if (file == null) return null;
            if (file.Exists == false) return null;

            XmlSerializer serializer = new XmlSerializer(type);
            using (var reader = new StreamReader(file.FullName))
            {
                return serializer.Deserialize(reader);
            }
        }

        public FileInfo Add(string name, object content, string context)
        {
            string path = GetPathByContext(context);
            
            path = Path.Combine(path, String.Format("{0}.xml", name));

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            XmlSerializer serializer = new XmlSerializer(content.GetType());
            using (var writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, content);
            }

            return new FileInfo(path);
        }

        public void Update(FileInfo file, object content)
        {
			if (file == null) return;
            if (!file.Exists) return ;

            XmlSerializer serializer = new XmlSerializer(content.GetType());
            using (var writer = new StreamWriter(file.FullName))
            {
                serializer.Serialize(writer, content);
            }
        }

        private string GetPathByContext(string context)
        {
            string path = Path.Combine(_rootFolder, context);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return path;
        }

        public FileInfo Rename(FileInfo oldFi, string name, object content, string context)
        {
            if (oldFi.Exists)
            {
                File.Delete(oldFi.FullName);    
            }

            return Add(name, content, context);
        }

        public void Delete(FileInfo file)
        {
            if (!file.Exists)
                return;

            file.Delete();
        }
    }
}