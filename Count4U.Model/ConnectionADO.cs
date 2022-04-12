using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Count4U.Model.Interface;
using System.Reflection;
using System.IO;
using NLog;

namespace Count4U.Model
{
    public class ConnectionADO : IConnectionADO
    {
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IDBSettings _settings;

        public ConnectionADO(IDBSettings settings)
        {
            this._settings = settings;
        }

		public string BuildPathFileADO(string subFolder, string fileDB)
        {
			return this._settings.BuildPathFileADO(subFolder, fileDB);
        }


        public string GetConnectionString(string pathFileDB)
        {
			//return @"Data Source=" + pathFileDB + ";Default Lock Timeout=60000;  Max Database Size = 1024; Max Buffer Size = 4096"; //Max Database Size = 512; 

			int maxDatabaseSize = 512;
			bool ret = int.TryParse( this._settings.ConnectionEFMaxDatabaseSize, out maxDatabaseSize);
			int maxBufferSize = 1024;
			ret = int.TryParse( this._settings.ConnectionEFMaxBufferSize, out maxBufferSize);

			string connectionString = @"Data Source=" + pathFileDB + String.Format(";Default Lock Timeout=60000;  Max Database Size = {0}; Max Buffer Size = {1}", maxDatabaseSize, maxBufferSize);
			return connectionString;
			//return @"Data Source=" + pathFileDB + ";Default Lock Timeout=60000;  Max Database Size = 1024; Max Buffer Size = 4096"; //Max Database Size = 512; 
        }
		//metadata=res://*/App_Data.Count4UDB.csdl|res://*/App_Data.Count4UDB.ssdl|res://*/App_Data.Count4UDB.msl;provider=System.Data.SqlServerCe.4.0;provider connection string="Data Source={0}"

		public string GetADOConnectionStringBySubFolder(string subFolder, string fileDB)
        {
			string pahtDB = this.BuildPathFileADO(subFolder, fileDB);
            return this.GetConnectionString(pahtDB);
        }

		public string CopyEmptyMobileDB(string toFilePath)
		{
			if (string.IsNullOrWhiteSpace(toFilePath) == true) return "";
			if (File.Exists(toFilePath) == true) return toFilePath;

			string fromFilePath = String.Empty;
			try
			{
				fromFilePath = this._settings.EmptyCount4MobileFilePath();		  //EmptyCount4MobileDBFilePath

				string folder = Path.GetDirectoryName(toFilePath);
				if (String.IsNullOrWhiteSpace(folder)) return String.Empty;

				if (Directory.Exists(folder) == false)
				{
					try
					{
						Directory.CreateDirectory(folder);
					}
					catch (Exception exp)
					{
						_logger.ErrorException("CopyEmptyMobileDB", exp);
						return "";
					}
				}

				if (File.Exists(toFilePath) == false)
				{
					File.Copy(fromFilePath, toFilePath, false);
				}
				return toFilePath;
			}
			catch (Exception exp)
			{
				_logger.ErrorException("CopyEmptyMobileDB", exp);
				_logger.Error("CopyEmptyMobileDB, fromFilePath: {0}, toFolderPath: {1}", fromFilePath, toFilePath);
				throw;
			}

		}
    }
}
