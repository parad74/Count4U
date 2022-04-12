using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
namespace Count4U.Common.Helpers.Ftp
{
public class FileDirectoryInfo
    {
        string fileSize;
        string type;
        string name;
        string date;
		long size;
        public string Adress;
		public Uri Uri;
		public DateTime DateTimeCreated;
		public bool IsDirectory;

        public string FileSize
        {
            get { return fileSize; }
            set { fileSize = value; }
        }

		public long Size
		{
			get { return size; }
			set { size = value; }
		}

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Date
        {
            get { return date; }
            set { date = value; }
        }

        public FileDirectoryInfo() 
		{ 
		    fileSize = "";
			type ="";
			name = "";
			date = "";
			Adress = "";
			IsDirectory = true;
			size = 0;
		}

        public FileDirectoryInfo(string fileSize, string type, string name, string date, string adress)
        {
            FileSize = fileSize;
            Type = type;
            Name = name;
            Date = date;
            this.Adress = adress;
        }

    }



}