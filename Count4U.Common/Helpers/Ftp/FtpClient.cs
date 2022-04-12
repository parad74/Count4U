using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
namespace Count4U.Common.Helpers.Ftp
{
	public class FtpClient {
		private string _password;
		private string _userName;
		private bool _enableSsl = false;
		public string uri;
	
		private int _bufferSize = 1024;
		private bool _passive = true;
		private bool _binary = true;
		private bool _hash = false;

		public FtpClient(string uri, string userName, string password, bool enableSsl = false)
		{
			this.uri = uri;
			this._userName = userName;
			this._password = password;
			this._enableSsl = enableSsl;
			if (enableSsl == true) ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

		//	request.ServerCertificateValidationCallback +=
		//(sender, cert, chain, error) =>
		//{
		//	return cert.GetCertHashString() == "xxxxxxxxxxxxxxxx";
		//};

			//HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
			//request.ServerCertificateValidationCallback = delegate { return true; };
		}

		//private static bool ValidateServerCertficate(
		//object sender,
		//X509Certificate cert,
		//X509Chain chain,
		//SslPolicyErrors sslPolicyErrors)
		//{
		//	if (sslPolicyErrors == SslPolicyErrors.None)
		//	{
		//		// Good certificate.
		//		return true;
		//	}

		//	//Common.Helpers.Logger.Log.Error(string.Format("SSL certificate error: {0}", sslPolicyErrors));
		//	try
		//	{
		//		using (X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
		//		{
		//			store.Open(OpenFlags.ReadWrite);
		//			store.Add(new X509Certificate2(cert));
		//			store.Close();
		//		}
		//		return true;
		//	}
		//	catch (Exception ex)
		//	{
		//		//Common.Helpers.Logger.Log.Error(string.Format("SSL certificate add Error: {0}", ex.Message));
		//	}

		//	return false;
		//}
 
		public string ChangeWorkingDirectory(string path) {
			uri = CombineFullPath(uri, path);
 
			return PrintWorkingDirectory();
		}

		public FtpStatusCode ChangeWorkingDirectoryReturnStatusCode(string path)
		{
			uri = CombineFullPath(uri, path);

			return GetStatusWorkingDirectory();
		}
 
		public string DeleteFile(string fileName) {
			var request = CreateRequest(CombineFullPath(uri, fileName), WebRequestMethods.Ftp.DeleteFile);
			
			return GetStatusDescription(request);
		}
 
		/// <summary>
		/// Get file from ftp
		/// </summary>
		/// <param name="fromFileFtp">from ftp</param>
		/// <param name="toFilePath">to local full path</param>
		/// <returns></returns>
		public FtpStatusCode GetFileFromFtp(string fromFileFtp, string toFilePath)
		{									 //from //скачать //загрузить
			var request = CreateRequest(CombineFullPath(uri, fromFileFtp), WebRequestMethods.Ftp.DownloadFile);
			
			byte[] buffer = new byte[_bufferSize];
 
			using (var response = (FtpWebResponse)request.GetResponse()) {
				using (var stream = response.GetResponseStream()) {
					using (var fs = new FileStream(toFilePath, FileMode.OpenOrCreate)) {
						int readCount = stream.Read(buffer, 0, _bufferSize);
 
						while (readCount > 0) {
							if (_hash)
								Console.Write("#");
 
							fs.Write(buffer, 0, readCount);
							readCount = stream.Read(buffer, 0, _bufferSize);
						}
					}
				}
 
				return response.StatusCode;
			}
		}

		/// <summary>
		/// Save file to Ftp server folder
		/// </summary>
		/// <param name="fromFilePath">from File with full Paht</param>
		/// <param name="toFileFtp">to ftp server folder</param>
		/// <returns></returns>
		public string SaveFileToFtp(string fromFilePath, string toFileFtp)
		{
			string ftpPath = CombineFullPath(uri, toFileFtp);
			//to	 //отослать // записать на сервере
			var request = CreateRequest(ftpPath, WebRequestMethods.Ftp.UploadFile);

			using (var stream = request.GetRequestStream())
			{
				using (var fileStream = System.IO.File.Open(fromFilePath, FileMode.Open))
				{
					int num;

					byte[] buffer = new byte[_bufferSize];

					while ((num = fileStream.Read(buffer, 0, buffer.Length)) > 0)
					{
						if (_hash)
							Console.Write("#");

						stream.Write(buffer, 0, num);
					}
				}
			}

			return GetStatusDescription(request);
		}


		public string CopyFileOnFtp(string fromFileFtp, string toFileFtp)
		{						//to UploadFile
			string toFtpPath = CombineFullPath(uri, toFileFtp);
			var requestToFtp = CreateRequest(toFtpPath, WebRequestMethods.Ftp.UploadFile);
			string fromFtpPath = CombineFullPath(uri, fromFileFtp);
			var requestFromFtp = CreateRequest(fromFtpPath, WebRequestMethods.Ftp.DownloadFile);
			//var request = CreateRequest(ftpPath, WebRequestMethods.Ftp.UploadFile);
			//using (var stream = request.GetRequestStream())

			using (var streamToFtp = requestToFtp.GetRequestStream())
			{
				using (var responseFromFtp = (FtpWebResponse)requestFromFtp.GetResponse())
				{
					using (var streamFromFtp = responseFromFtp.GetResponseStream())
					{
						int num;

						byte[] buffer = new byte[_bufferSize];

						while ((num = streamFromFtp.Read(buffer, 0, buffer.Length)) > 0)
						{
							if (_hash)
								Console.Write("#");

							streamToFtp.Write(buffer, 0, num);
						}
					}
				}

			}
	 
			return GetStatusDescription(requestToFtp);
		}
 

		/// <summary>
		/// Save file to Ftp server folder
		/// </summary>
		/// <param name="fromFilePath">from File with full Paht</param>
		/// <param name="toFileFtp">to ftp server folder</param>
		/// <returns></returns>
		//public string MoveFileToFtp(string fromFileFtp, string toFileFtp)
		//{
		//	string toFtpPath = CombineFullPath(uri, toFileFtp);
		//	string fromFtpPath = CombineFullPath(uri, fromFileFtp);
		//	//to	 //отослать // записать на сервере
		//	var request = CreateRequest(toFtpPath, WebRequestMethods.Ftp.DownloadFile);

		//	using (var stream = request.GetRequestStream())
		//	{
		//		using (var fileStream = System.IO.File.Open(fromFtpPath, FileMode.Open))
		//		{
		//			int num;

		//			byte[] buffer = new byte[bufferSize];

		//			while ((num = fileStream.Read(buffer, 0, buffer.Length)) > 0)
		//			{
		//				if (Hash)
		//					Console.Write("#");

		//				stream.Write(buffer, 0, num);
		//			}
		//		}
		//	}

		//	return GetStatusDescription(request);
		//}
 
		public DateTime GetDateTimestamp(string fileName) {
			var request = CreateRequest(CombineFullPath(uri, fileName), WebRequestMethods.Ftp.GetDateTimestamp);
			
			using (var response = (FtpWebResponse)request.GetResponse()) {
				return response.LastModified;
			}
		}
 
		public long GetFileSize(string fileName) {
			var request = CreateRequest(CombineFullPath(uri, fileName), WebRequestMethods.Ftp.GetFileSize);
			
			using (var response = (FtpWebResponse)request.GetResponse()) {
				return response.ContentLength;
			}
		}
 
		public string[] ListDirectory() {
			var list = new List<string>();
 
			var request = CreateRequest(WebRequestMethods.Ftp.ListDirectory);
			
			using (var response = (FtpWebResponse)request.GetResponse()) {
				using (var stream = response.GetResponseStream()) {
					using (var reader = new StreamReader(stream, true)) {
						while (!reader.EndOfStream) {
							list.Add(reader.ReadLine());
						}
					}
				}
			}
 
			return list.ToArray();
		}
 
		public string[] ListDirectoryDetails() {
			var list = new List<string>();
 
			var request = CreateRequest(WebRequestMethods.Ftp.ListDirectoryDetails);
			
			using (var response = (FtpWebResponse)request.GetResponse()) {
				using (var stream = response.GetResponseStream()) {
					using (var reader = new StreamReader(stream, true)) {
						while (!reader.EndOfStream) {
							list.Add(reader.ReadLine());
						}
					}
				}
			}
 
			return list.ToArray();
		}
 
		public string MakeDirectory(string directoryName)
		{
			string pathFolder = CombineFullPath(uri, directoryName);
			var request = CreateRequest(pathFolder, WebRequestMethods.Ftp.MakeDirectory);
			
			return GetStatusDescription(request);
		}
 
		public string PrintWorkingDirectory() {
			var request = CreateRequest(WebRequestMethods.Ftp.PrintWorkingDirectory);
 
			return GetStatusDescription(request);
		}

		public FtpStatusCode GetStatusWorkingDirectory()
		{
			var request = CreateRequest(WebRequestMethods.Ftp.PrintWorkingDirectory);
							    
			return GetStatusCode(request);
		}
 
		public string RemoveDirectory(string directoryName) {
			var request = CreateRequest(CombineFullPath(uri, directoryName), WebRequestMethods.Ftp.RemoveDirectory);
			
			return GetStatusDescription(request);
		}
 
		public string Rename(string currentName, string newName) {
			var request = CreateRequest(CombineFullPath(uri, currentName), WebRequestMethods.Ftp.Rename);
			
			request.RenameTo = newName;
 
			return GetStatusDescription(request);
		}
 
	
	
		public string UploadFileWithUniqueName(string source) {
			var request = CreateRequest(WebRequestMethods.Ftp.UploadFileWithUniqueName);
			
			using (var stream = request.GetRequestStream()) {
				using (var fileStream = System.IO.File.Open(source, FileMode.Open)) {
					int num;
 
					byte[] buffer = new byte[_bufferSize];
 
					while ((num = fileStream.Read(buffer, 0, buffer.Length)) > 0) {
						if (_hash)
							Console.Write("#");
 
						stream.Write(buffer, 0, num);
					}
				}
			}
 
			using (var response = (FtpWebResponse)request.GetResponse()) {
				return Path.GetFileName(response.ResponseUri.ToString());
			}
		}
 
		private FtpWebRequest CreateRequest(string method) {
			return CreateRequest(uri, method);
		}
 
		private FtpWebRequest CreateRequest(string uri, string method) {
			var r = (FtpWebRequest)WebRequest.Create(uri);
 
			r.Credentials = new NetworkCredential(_userName, _password);
			r.Method = method;
			r.UseBinary = _binary;
			r.EnableSsl = _enableSsl;
			r.UsePassive = _passive;
			
 
			return r;
		}
 
		private string GetStatusDescription(FtpWebRequest request) {
			using (var response = (FtpWebResponse)request.GetResponse()) {
				return response.StatusDescription;
			}
		}

		private FtpStatusCode GetStatusCode(FtpWebRequest request)
		{
			using (var response = (FtpWebResponse)request.GetResponse())
			{
				return response.StatusCode;
			}
		}
 
		private string CombineFullPath(string path1, string path2) 
		{
			string path = Path.Combine(path1, path2);
			return path.Replace("\\", "/");
		}

		public List<FileDirectoryInfo> GetDirectoryInformation(string folder, string host, string username, string password)
		{
			string path = CombineFullPath(host, folder);
			FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(path);
			request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
			request.Credentials = new NetworkCredential(username, password);
			request.UsePassive = true;
			request.UseBinary = true;
			request.KeepAlive = false;

			List<FileDirectoryInfo> returnValue = new List<FileDirectoryInfo>();
			try
			{
				string[] list = null;

				using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
				using (StreamReader reader = new StreamReader(response.GetResponseStream()))
				{
					list = reader.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
				}

				foreach (string line in list)
				{
					// Windows FTP Server Response Format
					// DateCreated    IsDirectory    Name
					string data = line;

					// Parse date
					string date = data.Substring(0, 17);
					//DateTime.ParseExact("April 16, 2011 4:27 pm", "MMMM d, yyyy h:mm tt", null);
					string dateTimeStringFrom = date;
					string dateTimeString = date;
					DateTime dtTime = DateTime.Now;
					DateTime dtDate = DateTime.Now;

					string[] dts = dateTimeString.Split(' ');
					string dateString = dts[0];
					string timeString = dts[2];

					if (DateTime.TryParseExact(dateString, "MM-dd-yy", null,
						System.Globalization.DateTimeStyles.None, out dtDate))
					{
						string test = String.Format("Parsed \"{0}\" as {1}", dateTimeString, dtDate);
					}

					if (DateTime.TryParseExact(timeString, "hh:mmtt", new CultureInfo("en-US"),
						System.Globalization.DateTimeStyles.None, out dtTime))
					{
						string test = String.Format("Parsed \"{0}\" as {1}", dateTimeString, dtTime);
					}
					DateTime dateTime = new DateTime(
						dtDate.Year,
						dtDate.Month,
						dtDate.Day,
						dtTime.Hour,
						dtTime.Minute,
						0);
						//DateTimeKind.Local); //Local		   Utc
					//DateTime dateTime1 = DateTime.Now;

					//if (DateTime.TryParseExact(dateTimeString, "MM-dd-yy hh:mmtt", new CultureInfo("en-US"),
					//	System.Globalization.DateTimeStyles.None, out dateTime1))
					//{
					//	string test = String.Format("Parsed \"{0}\" as {1}", dateTimeString, dateTime1);
					//}

					//bool pm = dateTimeString.Contains("PM");
					//bool am = dateTimeString.Contains("AM");

					//timeString = timeString.Replace("PM", "");
					//timeString = timeString.Replace("AM", "");
					//if (DateTime.TryParseExact(timeString, "hh:mm", null,
					//		System.Globalization.DateTimeStyles.None, out dateTime))
					//{
					//	string test = String.Format("Parsed \"{0}\" as {1}", dateTimeString, dateTime);
					//}

					//if (pm == true) timeString = timeString + " PM";
					//if (DateTime.TryParseExact(timeString, "hh:mm tt", new CultureInfo("en-US"), 
					//System.Globalization.DateTimeStyles.None, out dateTime))
					//{
					//	string test = String.Format("Parsed \"{0}\" as {1}", dateTimeString, dateTime);
					//}
					//try
					//{
					//	bool pm = dateTimeString.Contains("PM");
					//	bool am = dateTimeString.Contains("AM");
					//	dateTimeString = dateTimeString.Replace("PM", "");
					//	dateTimeString = dateTimeString.Replace("AM", "");
					//	string[] dts = dateTimeString.Split(' ');
					//	string dateString = dts[0];
					//	string timeString = dts[2];
					//	string[] dateStrings = dateString.Split('-');
					//	string[] timeStrings = timeString.Split(':');
					//	int plus12 = 0;
					//	if (pm == true) plus12 = 12;

					//	int yyyy = Int32.Parse(dateStrings[2].TrimStart('0')) + 2000;		 //yyyy
					//	int MM =	Int32.Parse(dateStrings[0].TrimStart('0'));					 //MM
					//	int dd = 	Int32.Parse(dateStrings[1].TrimStart('0'));						//dd
					//	int hh = 	Int32.Parse(timeStrings[0].TrimStart('0')) + plus12;		//hh
					//	int mm = Int32.Parse(timeStrings[1].TrimStart('0'));			 //mm
					//	int ss = 0;
			
					//	//dateTime =  yyyy, MM, 	dd, hh, mm , ss;

		
						
					//}
					//catch { }
					
					data = data.Remove(0, 24);

					// Parse <DIR>
					string dir = data.Substring(0, 5);
					bool isDirectory = dir.Equals("<dir>", StringComparison.InvariantCultureIgnoreCase);
					data = data.Remove(0, 5);
					data = data.Remove(0, 10);

					// Parse name
					string name = data;
					FileDirectoryInfo item = new FileDirectoryInfo();
					item.Name = name;
					item.IsDirectory = isDirectory;

					if (isDirectory == false)
					{
						long size = GetFileSize(data);
						item.Size = size;
						item.FileSize = "0";
						try
						{
							item.FileSize = ((int)(size / 1024) + 1).ToString() + " Kb";
						}
						catch { }

						DateTime dt = GetDateTimestamp(data);
						// Create directory info
						//item.Uri = new Uri(address);
						//item.DateTimeCreated = dateTime;
						item.Date = dateTimeStringFrom;
						item.DateTimeCreated = dateTime;

						//Debug.WriteLine(item.AbsolutePath);
						//item.Items = item.IsDirectory ? GetDirectoryInformation(item.AbsolutePath, username, password) : null;
					}
					returnValue.Add(item);
				}

			}
			catch (Exception ex)
			{
				return returnValue;
			}
			return returnValue;
		}

		//public List<FileDirectoryInfo> GetListFileDirectoryInfo( )
		//{
		//	try
		//	{
		
		//		// Регулярное выражение, которое ищет информацию о папках и файлах 
		//		// в строке ответа от сервера
		//		Regex regex = new Regex(@"^([d-])([rwxt-]{3}){3}\s+\d{1,}\s+.*?(\d{1,})\s+(\w+\s+\d{1,2}\s+(?:\d{4})?)(\d{1,2}:\d{2})?\s+(.+?)\s?$",
		//			RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);

		//		// Получаем список корневых файлов и папок
		//		// Используется LINQ to Objects и регулярные выражения
		//		List<FileDirectoryInfo> list = this.ListDirectoryDetails()
		//											 .Select(s =>
		//											 {
		//												 Match match = regex.Match(s);
		//												 if (match.Length > 5)
		//												 {
		//													 // Устанавливаем тип, чтобы отличить файл от папки (используется также для установки рисунка)
		//													 string type = match.Groups[1].Value == "d" ? "DIR.png" : "FILE.png";

		//													 // Размер задаем только для файлов, т.к. для папок возвращается
		//													 // размер ярлыка 4кб, а не самой папки
		//													 string size = "";
		//													 if (type == "FILE.png")
		//														 size = (Int32.Parse(match.Groups[3].Value.Trim()) / 1024).ToString() + " кБ";

		//													 return new FileDirectoryInfo(size, type, match.Groups[6].Value, match.Groups[4].Value, "");
		//												 }
		//												 else return new FileDirectoryInfo();
		//											 }).ToList();
		//		list.Reverse();
		//		return list;
		//	}
		//	catch (Exception ex)
		//	{
		//		return new List<FileDirectoryInfo>();
		//	}
		//}

		
		//  Create folder on ftp
		// host - url	  //string host = _userSettingsManager.HostGet().Trim('\\');
		//objectFolder = mINV\Customer\<CustomerCode>\Branch\BranchCode>\Inventor\<InventorCode>\Profile\<profile>
		//	string folderForInventorObject = base._contextCBIRepository.BuildLongCodesPath(base.CurrentInventor).Trim(@"\".ToCharArray()) + @"\Profile";
		//rootFonderOnFtp = "mINV" 
		public string CreatePathOnFtp(string host, string rootFonderOnFtp, string objectFolder, ref string messageCreateFolder)
		{
			objectFolder = objectFolder.Trim(@"\".ToCharArray());
			string[] foldersInPath = objectFolder.Split(@"\".ToCharArray());
			int i = 1;
			foreach (var folder in foldersInPath)
			{
				if (string.IsNullOrWhiteSpace(folder) == true) continue;
				string newfolder = folder.Trim();
				bool isTerminalFolder = false;
				if (i == foldersInPath.Count()) isTerminalFolder = true;
				messageCreateFolder = messageCreateFolder + this.CreateFolderOnFtp(host, rootFonderOnFtp, newfolder, isTerminalFolder);
				rootFonderOnFtp = rootFonderOnFtp + @"\" + newfolder;
				i++;
			}

			//return messageCreateFolder;
			return rootFonderOnFtp;
		}



		public string CreateFolderOnFtp(string host, string rootFolder, string createFolder, bool isTerminalFolder)
		{
			//string host = _userSettingsManager.HostGet().Trim('\\');
			//string user = _userSettingsManager.UserGet();
			//string password = _userSettingsManager.PasswordGet();
			string messageCreateFolder = "";

			//FtpClient client = new FtpClient(host, user, password);
			//--------------- find or create folder on ftp ----------------
			this.uri = host;
			string result3 = this.ChangeWorkingDirectory(rootFolder);

			//string customerfolder = base.CurrentCustomer.Code;													// customerCode
			//createFolder
			string[] listDirectory = this.ListDirectory();
			bool newDir = true;
			foreach (string dir in listDirectory)
			{																					 // проверяем есть ли такая папка
				if (dir.ToLower() == createFolder.ToLower())		//  ToApp/customerCode
				{
					newDir = false;
				}
			}

			//string pathFtpCustomerFolder = host + @"\" + exportToPDAFolder + @"\" + base.CurrentCustomer.Code;  
			string pathFtpFolder = this.uri + @"/" + createFolder;
			//string pathFtpProfileFolder = pathFtpCustomerFolder + @"\profile";
			messageCreateFolder = "";//"On FTP there is Folder for Profile : " + Environment.NewLine;

			if (newDir == true)
			{
				string result1 = this.MakeDirectory(createFolder);
				//messageCreateFolder = "On FTP try crete Folder  : [" + pathFtpFolder + "]  with Result [" + result1 + "]" + Environment.NewLine;
				if (isTerminalFolder)
				{
					messageCreateFolder = "On FTP tried to create Folder with Result : " + result1;
				}
				return messageCreateFolder;
			}
			else
			{
				//messageCreateFolder = messageCreateFolder + pathFtpFolder + Environment.NewLine;
				return messageCreateFolder;
			}

		}

		public string CreateThreadCodeFolderOnFtp(string host, string rootFolder, string createFolder, string inventorCode, ref string messageCreateFolder)
		{
			//--------------- find or create folder on ftp ----------------
			this.uri = host;
			string result3 = this.ChangeWorkingDirectory(rootFolder);

			string[] listDirectory = this.ListDirectory();
			bool newDir = false;
	

			while (newDir == false)
			{
				newDir = true;

				foreach (string dir in listDirectory)
				{																					 // проверяем есть ли такая папка
					if (dir.ToLower() == createFolder.ToLower())			
					{
						newDir = false;														// такая папка есть, надо наращивать префикс
						break;
					}
				}

				if (newDir == false)
				{
					string prefix = inventorCode.Substring(0, 1);
					bool ret = createFolder.Contains('~');
					if (ret == true)
					{
						string[] folders = createFolder.Split('~');
						if (folders.Length >= 2)
						{

							if (string.IsNullOrWhiteSpace(folders[1]) == false)
							{
								string prefix1 = folders[1].Trim();
								int let = prefix1.Length + 1;
								prefix = inventorCode.Substring(0, let);
							}
						}
					}

					DateTime today = DateTime.Now;
					string ThreadCode = today.Year + "-" + today.DayOfYear + "~" + prefix;
					createFolder = ThreadCode;
				}
			}

			//string pathFtpCustomerFolder = host + @"\" + exportToPDAFolder + @"\" + base.CurrentCustomer.Code;  
			string pathFtpFolder = this.uri + @"/" + createFolder;
			//string pathFtpProfileFolder = pathFtpCustomerFolder + @"\profile";
			messageCreateFolder = "";//"On FTP there is Folder for Profile : " + Environment.NewLine;

			if (newDir == true)
			{
				string result1 = this.MakeDirectory(createFolder);
				messageCreateFolder = "On FTP try crete Folder  : [" + pathFtpFolder + "] with Result [" + result1 + "]" + Environment.NewLine;
				return createFolder;
			}
			else
			{
				messageCreateFolder = messageCreateFolder + pathFtpFolder + Environment.NewLine;
				return createFolder;
			}

		}
	}


}