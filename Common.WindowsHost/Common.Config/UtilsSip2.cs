using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace Common.Config
{
	public class UtilsSip2
	{
		private static string _ip = "";
		private static string _port = "";
		private static string _username = "";
		private static string _password = "";
		private static string _extraNumber = "";
		private static string _sip2Liber8ProtocolFile = "Sip2Liber8Protocol.xml";//IdSip2.Common.Properties.Settings1.Default.Sip2Liber8ProtocolFile; //@"C:\idsip2\trunk\IdSip2\IdSip2.Provider\IDSip2.ServiceHostIIS\App_Data\Sip2Liber8Protocol.xml";//
		public static string _configCommunicationFile = "ConfigCommunication.xml";//IdSip2.Common.Properties.Settings1.Default.ConfigCommunicationFile;
		private static XDocument _xDocumentSip2Protocol = null;
		private static XDocument _xDocumentConfigCommunication = null;
		private static string _error = "";
		//public static string _IIS = IdSip2.Common.Properties.Settings1.Default.IIS;
		//public static string LogPath = IdSip2.Common.Properties.Settings1.Default.LogPath;
		//public static string DBPath = IdSip2.Common.Properties.Settings1.Default.AppDataFolder;
		//public static string AppDataFolder = IdSip2.Common.Properties.Settings1.Default.AppDataFolder;



		private static string AssemblyFolderPath()
		{

			string appDataFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		
			//appDataFolder = new DirectoryInfo(appDataFolder).Parent.FullName;
			//appDataFolder = new DirectoryInfo(appDataFolder).Parent.FullName;
			//appDataFolder = new DirectoryInfo(appDataFolder).Parent.FullName;
			//appDataFolder = new DirectoryInfo(appDataFolder).Parent.FullName;
			//appDataFolder = appDataFolder + @"\IdSip2.Common\App_Data";

			DirectoryInfo di = new DirectoryInfo(appDataFolder);
			return di.FullName;
		}



		public static string BuildAppDataFolderPath(string folderPath = "")
		{
			string appDataFolderPath = AssemblyFolderPath() + @"\App_Data\";
			if (folderPath != "")
			{
				appDataFolderPath = folderPath + @"App_Data\";
			}

			return appDataFolderPath;
		}

		
				//	string appData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
				//	string dbPathFile = Path.Combine(appData, AppDataLocalFolder, DbPathFile);
				//	if (File.Exists(dbPathFile))
				
		public static XDocument XDocumentSip2Protocol(string folderPath = "")
		{
			//get 
			//{
				//string appData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData); //C:\ProgramData

				UtilsSip2._error = "";

				//string basePath = BuildAppDataFolderPath();
				string ip2Liber8ProtocolPath = UtilsSip2.Sip2Liber8ProtocolPath(folderPath);

				if (UtilsSip2._xDocumentSip2Protocol == null)
				{
					try
					{
						if (File.Exists(ip2Liber8ProtocolPath) == false) UtilsSip2._error =  UtilsSip2._error + "File not Exists : "  + ip2Liber8ProtocolPath;
						UtilsSip2._xDocumentSip2Protocol = XDocument.Load(ip2Liber8ProtocolPath);
					}
					catch(Exception exp)
					{
						UtilsSip2._xDocumentSip2Protocol = null; 
						UtilsSip2._error = UtilsSip2._error + exp.Message;
					}
				}
				return UtilsSip2._xDocumentSip2Protocol; 
			//}
			//set { UtilsSip2._xDocumentSip2Protocol = value; }
		}


		public static string Sip2Liber8ProtocolPath(string folderPath = "")
		{
			string appDataFolderPath = BuildAppDataFolderPath(folderPath).Trim('\\'); 
			ConfigCommunication configCommunication = new ConfigCommunication();
			string subFolder = configCommunication.GetProtocolFilePath().Trim('\\');
			appDataFolderPath = appDataFolderPath + @"\" + subFolder + @"\";

			return appDataFolderPath + UtilsSip2._sip2Liber8ProtocolFile; 
		}



		public static XDocument XDocumentConfigCommunication(string folderPath = "")
		{
			//get
			//{
				//string appData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData); //C:\ProgramData

				UtilsSip2._error = "";

				//string basePath = BuildAppDataFolderPath();
				string configCommunicationPath = UtilsSip2.ConfigCommunicationFilePath(folderPath);

				if (UtilsSip2._xDocumentConfigCommunication == null)
				{
					try
					{
						if (File.Exists(configCommunicationPath) == false) UtilsSip2._error = UtilsSip2._error + "File not Exists : " + configCommunicationPath;
						UtilsSip2._xDocumentConfigCommunication = XDocument.Load(configCommunicationPath);
					}
					catch (Exception exp)
					{
						UtilsSip2._xDocumentConfigCommunication = null;
						UtilsSip2._error = UtilsSip2._error + exp.Message;
					}
				}
				return UtilsSip2._xDocumentConfigCommunication;
			//}
			//set { UtilsSip2._xDocumentConfigCommunication = value; }
		}

		public static string ConfigCommunicationFilePath(string folderPath = "")
		{
			//get { return UtilsSip2.AppDataFolder.TrimEnd('\\') + @"\" + UtilsSip2._configCommunicationFile; }
				//HttpContext.Current.ApplicationInstance.Server.MapPath("~/App_Data");
			//get
			//{
			//	string filePath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "App_Data", UtilsSip2._configCommunicationFile);
			//	return filePath;
			//}
			return BuildAppDataFolderPath(folderPath) + UtilsSip2._configCommunicationFile;
		}
		

		public static string Error
		{
			get { return UtilsSip2._error; }
			set { UtilsSip2._error = value; }
		}

					
		//public UtilsSip2()
		//{
		//	string sip2Liber8ProtocolPath = IdSip2.Common.Properties.Settings1.Default.Sip2Liber8ProtocolPath;
		//}

		public static string Ip
		{
			get { return _ip; }
			set { _ip = value; }
		}

		public static string Port
		{
			get { return _port; }
			set { _port = value; }
		}

		public static string Username
		{
			get { return _username; }
			set { _username = value; }
		}

		public static string Password
		{
			get { return _password; }
			set { _password = value; }
		}

		public static string ExtraNumber
		{
			get { return _extraNumber; }
			set { _extraNumber = value; }
		}

		
	}

	
}
