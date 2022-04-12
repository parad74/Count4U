using System;
using System.Collections.Generic;
using System.Linq;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Audit;
using Count4U.Model.Audit.MappingEF;
using Count4U.Model.Interface;
using System.Data.Objects;
using Count4U.Model.SelectionParams;
using System.IO;
using System.Security.AccessControl;

namespace Count4U.Model
{
	public static class DBFileUtils 
	{
		private static ConnectionDB _connection = new ConnectionDB(null);

		private static ConnectionDB Connection
		{
			get { return _connection; }
			set { _connection = value; }
		}

   		public static string RemoveDB(string dbPath,  string folder , bool full = false)
		{
		//	string folderInventor = Properties.Settings.Default.FolderInventor.Trim('\\') + @"\";
			dbPath = folder.Trim('\\') + @"\" + dbPath.Trim('\\');
			if (String.IsNullOrWhiteSpace(dbPath) == true ) return "";
			if (full == false)
			{
				string toFolderPath = Connection.BuildCount4UDBFolderPath(dbPath + @"\removed");
				if (Directory.Exists(toFolderPath) == false)
				{
					try
					{
						Directory.CreateDirectory(toFolderPath);
					}
					catch { return ""; }
				}

				string fromFilePath = Connection.BuildCount4UDBFilePath(dbPath);
				string toFilePath = Connection.BuildCount4UDBFilePath(dbPath + @"\removed");

				try
				{
					File.Copy(fromFilePath, toFilePath);
					GC.Collect();
					File.Delete(fromFilePath);
				}
				catch 
				{ 
					//Удалить не удалось
				}

				return toFilePath;
			}

			else  //full delete
			{
				string fromFilePath = Connection.BuildCount4UDBFilePath(dbPath);
				if (File.Exists(fromFilePath) == true)
				{
					File.Delete(fromFilePath);
				}
				return "";
			}
		}

		public static string CopyEmptyDB(string dbPath, string folder)
		{
			if (string.IsNullOrWhiteSpace(dbPath) == true) return "";
			//string folderInventor = Properties.Settings.Default.FolderInventor.Trim('\\') + @"\";
			dbPath = folder.Trim('\\') + @"\" + dbPath.Trim('\\');
			string toFolderPath = Connection.BuildCount4UDBFolderPath(dbPath);
			if (Directory.Exists(toFolderPath) == false)
			{
				try
				{
					Directory.CreateDirectory(toFolderPath);
				}
				catch  {	return "";	}
			}

			string fromFilePath = Connection.EmptyCount4UDBFilePath();
			string toFilePath = Connection.BuildCount4UDBFilePath(dbPath);
			File.Copy(fromFilePath, toFilePath, true);

			return dbPath;
		}
	
	}
}
