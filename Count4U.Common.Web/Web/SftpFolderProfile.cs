using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Count4U.Common.Helpers;
using Count4U.Common.Helpers.Ftp;
using Count4U.Common.UserSettings;
using Count4U.Common.Web.Profile;
using Count4U.Model;
using Count4U.Model.Audit;
using Count4U.Model.Interface;
using Count4U.Model.Interface.Audit;
using Count4U.Model.Main;
using Count4U.Model.Transfer;
//using Ionic.Zip;
using NLog;
using System.IO;
using Renci.SshNet;

namespace Count4U.Common.Web
{
	public class SftpFolderProfile
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly IUserSettingsManager _userSettingsManager;
		private readonly IContextCBIRepository _contextCBIRepository;

		public SftpFolderProfile(IUserSettingsManager userSettingsManager,
			IContextCBIRepository contextCBIRepository)
		{
			this._userSettingsManager = userSettingsManager;
			this._contextCBIRepository = contextCBIRepository;
		}

	
		 //CUSTOMER Create folder on FTP and PROFILE on webServise
		//rootFonderOnFtp= mINV
		public void CustomerProfileCreate(Customer newCustomer, string rootFonderOnFtp) 
		{
			if (newCustomer == null)
			{
				_logger.Error("CustomerProfileCreate ERROR : Customer is Null  ");
				return;
			}
  			
			using (new CursorWait("ProfileBuild"))
			{
				string messageCreateFolder = "";

				try
				{
					//FTP
					string folderForCustomerObject = this._contextCBIRepository.BuildLongCodesPath(newCustomer);
					CreatePathOnFtp(rootFonderOnFtp, folderForCustomerObject, ref messageCreateFolder);
				}
				catch (Exception exc)
				{
					_logger.ErrorException("In process create Folder on FTP, happens ERROR : ", exc);
					return;
				}

				// WEB
				string messageResponse = WebCreateProfileCustomer(newCustomer);

				string messageText = messageCreateFolder + messageResponse;
				_logger.Info("ProfileCreate : " + messageText);
			}
		}

		public void InventorProfileCreate(Customer currentCustomer, Branch currentBranch,  Inventor newInventor, string rootFonderOnFtp) 	 //mINV
		{
			if (currentCustomer == null)
			{
				_logger.Error("InventorProfileCreate ERROR : Customer is Null  ");
				return;
			}

			if (currentBranch == null)
			{
				_logger.Error("InventorProfileCreate ERROR : Branch is Null  ");
				return;
			}

			if (newInventor == null)
			{
				_logger.Error("InventorProfileCreate ERROR : Inventor is Null  ");
				return;
			}

			using (new CursorWait("ProfileBuild"))
			{
				string messageCreateFolder = "";
				try
				{
					//FTP
					string folderForCustomerObject = this._contextCBIRepository.BuildLongCodesPath(currentCustomer);
					CreatePathOnFtp(rootFonderOnFtp, folderForCustomerObject, ref messageCreateFolder);

					string folderForInventorObject = this._contextCBIRepository.BuildLongCodesPath(newInventor).Trim(@"\".ToCharArray()) + @"\Profile";
					CreatePathOnFtp(rootFonderOnFtp, folderForInventorObject, ref messageCreateFolder);
				}
				catch (Exception exc)
				{
					_logger.ErrorException("In process create Folder on FTP, happens ERROR : ", exc);
					return;
				}

				// WEB
				string messageResponse1 = WebCreateProfileCustomer(currentCustomer);
				string messageResponse2 = WebCreateProfileInventor(newInventor);

				string messageText = messageCreateFolder + messageResponse1 + messageResponse2;
				_logger.Info("ProfileCreate : " + messageText);
			}
		}

		public void InventorProfileGet(Customer currentCustomer, Branch currentBranch, Inventor newInventor, string rootFonderOnFtp) 	 //mINV
		{
			if (currentCustomer == null)
			{
				_logger.Error("InventorProfileCreate ERROR : Customer is Null  ");
				return;
			}

			if (currentBranch == null)
			{
				_logger.Error("InventorProfileCreate ERROR : Branch is Null  ");
				return;
			}

			if (newInventor == null)
			{
				_logger.Error("InventorProfileCreate ERROR : Inventor is Null  ");
				return;
			}

			using (new CursorWait("ProfileBuild"))
			{
				string messageCreateFolder = "";
				try
				{
					string folderForInventorObject = this._contextCBIRepository.BuildLongCodesPath(newInventor).Trim(@"\".ToCharArray()) + @"\Profile";
					//CreateFoldersOnFtpForProfile(rootFonderOnFtp, folderForInventorObject, fileName, ref messageCreateFolder);
				}
				catch (Exception exc)
				{
					_logger.ErrorException("In process create Folder on FTP, happens ERROR : ", exc);
					return;
				}

				//string messageText = messageCreateFolder + messageResponse1 + messageResponse2;
				//_logger.Info("ProfileCreate : " + messageText);
			}
		}

		//rootFonderOnFtp = "mINV" 
		//string rootFonderOnFtp = this._customerRepository.Connection.RootFolderFtp(); //mINV
		public void InventorProfileSendFromLocalPathToFtp(Inventor inventor, string rootFonderOnFtp, string fromLocalPath, string fileName = "profile.xml") 	 //mINV
		{
			if (inventor == null)
			{
				_logger.Error("InventorProfileCreate ERROR : Inventor is Null  ");
				return;
			}

			using (new CursorWait("ProfileBuild"))
			{
				string messageCreateFolder = "";
				try
				{
					string folderForInventorObject = this._contextCBIRepository.BuildLongCodesPath(inventor).Trim(@"\".ToCharArray()) + @"\Profile";
					//rootFonderOnFtp = "mINV" 

					string ftpFolder = this.CreatePathOnFtp(rootFonderOnFtp, folderForInventorObject, ref messageCreateFolder);

					this.CopyFileToFtp(fromLocalPath, ftpFolder, fileName);
				}
				catch (Exception exc)
				{
					_logger.ErrorException("In process create Folder on FTP, happens ERROR : ", exc);
					return;
				}


			}
		}

		//WebServise for Customer Profile
		public string WebCreateProfileCustomer(Customer newCustomer)
		{
			string messageResponse = "Response : " + Environment.NewLine;
			messageResponse = messageResponse + ProfileRESTRepository.RemoteCreateProfileCustomer(newCustomer);
			return messageResponse;
		}

		//WebServise for Inventor Profile
		public string WebCreateProfileInventor(Inventor newInventor)
		{
			string messageResponse = "Response : " + Environment.NewLine;
			messageResponse = messageResponse + ProfileRESTRepository.RemoteCreateProfileInventor(newInventor);
			return messageResponse;
		}

		//  Create folder on ftp
		//mINV\Customer\<CustomerCode>\Branch\BranchCode>\Inventor\<InventorCode>\Profile\<profile>
		//rootFonderOnFtp = "mINV" 
		public string CreatePathOnFtp(string rootFonderOnFtp, string objectFolder, ref string messageCreateFolder)
		{
			string host = _userSettingsManager.HostGet().Trim('\\');
			string user = _userSettingsManager.UserGet();
			string password = _userSettingsManager.PasswordGet();

			FtpClient client = new FtpClient(host, user, password);
			string folderResult = client.CreatePathOnFtp(host, rootFonderOnFtp, objectFolder, ref messageCreateFolder);
			_logger.Info(messageCreateFolder);
			return folderResult;
		}

		//======================  SFTP  ============================
		public static void UploadSFTPFile(string host, string username,
		string password, string sourcefile, string destinationpath, int port)
		{
			//using (var sshclient = new SshClient(ConnNfo))
			//{
			//	sshclient.Connect();
			//	using (var cmd = sshclient.CreateCommand("mkdir -p " + parentDirectory + " && chmod +rw " + parentDirectory))
			//	{
			//		cmd.Execute();
			//	}
			//	sshclient.Disconnect();
			//}

			using (SftpClient client = new SftpClient(host, port, username, password))
			{
				client.Connect();
				client.ChangeDirectory(destinationpath);
				using (FileStream fs = new FileStream(sourcefile, FileMode.Open))
				{
					client.BufferSize = 4 * 1024;
					client.UploadFile(fs, Path.GetFileName(sourcefile));
				}
			}
		}

		public static void UploadSFTPFile1(string host, string username,
		string password, string sourcefile, string destinationpath, int port)
		{

			//const string host = "domainna.me";
			//const string username = "chucknorris";
			//const string password = "norrischuck";
			const string workingdirectory = "/highway/hell";
			const string uploadfile = @"c:\yourfilegoeshere.txt";

			//Console.WriteLine("Creating client and connecting");
			using (var client = new SftpClient(host, port, username, password))
			{
				client.Connect();
				Console.WriteLine("Connected to {0}", host);

				client.ChangeDirectory(workingdirectory);
				Console.WriteLine("Changed directory to {0}", workingdirectory);

				var listDirectory = client.ListDirectory(workingdirectory);
				Console.WriteLine("Listing directory:");
				foreach (var fi in listDirectory)
				{
					Console.WriteLine(" - " + fi.Name);
				}

				using (var fileStream = new FileStream(uploadfile, FileMode.Open))
				{
					Console.WriteLine("Uploading {0} ({1:N0} bytes)", uploadfile, fileStream.Length);
					client.BufferSize = 4 * 1024; // bypass Payload error large files
					client.UploadFile(fileStream, Path.GetFileName(uploadfile));
				}
			}
		}

		public void DownloadFile(string localFile)
		{
			string host = "";
			string username = "";
			string password = "";
			string localFileName = System.IO.Path.GetFileName(localFile);
			string remoteFileName = "";

			using (var sftp = new SftpClient(host, username, password))
			{
				sftp.Connect();

				using (var file = File.OpenWrite(localFileName))
				{
					sftp.DownloadFile(remoteFileName, file);
				}

				sftp.Disconnect();
			}
		}

		private void UploadProfileImage(string TargetFileName, string TargetDestinationPath, string FiletoUpload)
		{
			//Get the Image Destination path
			string imageName = TargetFileName; //you can comment this
			string imgPath = TargetDestinationPath;

			string ftpurl = "ftp://downloads.abc.com/downloads.abc.com/MobileApps/SystemImages/ProfileImages/" + imgPath;
			string ftpusername = "krayknot_DAL.clsGlobal.FTPUsername";
			string ftppassword = "krayknot_DAL.clsGlobal.FTPPassword";
			string fileurl = FiletoUpload;

			FtpWebRequest ftpClient = (FtpWebRequest)FtpWebRequest.Create(ftpurl);
			ftpClient.Credentials = new System.Net.NetworkCredential(ftpusername, ftppassword);
			ftpClient.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
			ftpClient.UseBinary = true;
			ftpClient.KeepAlive = true;
			System.IO.FileInfo fi = new System.IO.FileInfo(fileurl);
			ftpClient.ContentLength = fi.Length;
			byte[] buffer = new byte[4097];
			int bytes = 0;
			int total_bytes = (int)fi.Length;
			System.IO.FileStream fs = fi.OpenRead();
			System.IO.Stream rs = ftpClient.GetRequestStream();
			while (total_bytes > 0)
			{
				bytes = fs.Read(buffer, 0, buffer.Length);
				rs.Write(buffer, 0, bytes);
				total_bytes = total_bytes - bytes;
			}
			//fs.Flush();
			fs.Close();
			rs.Close();
			FtpWebResponse uploadResponse = (FtpWebResponse)ftpClient.GetResponse();
			string value = uploadResponse.StatusDescription;
			uploadResponse.Close();
		}

//		IEnumerable<string> files = Directory.EnumerateFiles(@"C:\local\folder");

//using (WebClient client = new WebClient())
//{
//	client.Credentials = new NetworkCredential("username", "password");

//	foreach (string file in files)
//	{
//		client.UploadFile(
//			"ftp://example.com/remote/folder/" + Path.GetFileName(file), file);
//	}
//}

		/// <summary>
		/// This will list the contents of the current directory.
		/// </summary>
		public void ListDirectory()
		{
			string host = "";
			string username = "";
			string password = "";
			string remoteDirectory = "."; // . always refers to the current directory.

			using (var sftp = new SftpClient(host, username, password))
			{
				sftp.Connect();
				var files = sftp.ListDirectory(remoteDirectory);
				foreach (var file in files)
				{
					Console.WriteLine(file.FullName);
				}
				sftp.Disconnect();
			}
		}

//		tring source = @"FilePath and FileName of Local File to Upload";
//string destination = @"SFTP Server File Destination Folder";
//string host = "SFTP Host";
//string username = "User Name";
//string password = "password";
//int port = 22;  //Port 22 is defaulted for SFTP upload

//sftp.UploadSFTPFile(host, username, password, source, destination, port);

	
		public void CopyFileFromFtp(string fromFtpFonder, string toLocalFolder, string fileName, ref string messageCreateFolder)
		{
			if (Directory.Exists(toLocalFolder) == false) return;

			string host = _userSettingsManager.HostGet().Trim('\\');
			string user = _userSettingsManager.UserGet();
			string password = _userSettingsManager.PasswordGet();

			FtpClient client = new FtpClient(host, user, password);
			client.uri = host;

			try
			{
				using (new CursorWait())
				{
					FtpStatusCode statusCode = client.ChangeWorkingDirectoryReturnStatusCode(fromFtpFonder);
					if (statusCode == FtpStatusCode.PathnameCreated)
					{
						List<FileDirectoryInfo> listInfoDirectory = client.GetDirectoryInformation(fromFtpFonder, host, user, password);
						if (listInfoDirectory.Count < 1) return;
						string targetPath = "none";

						targetPath = toLocalFolder + @"\" + fileName; //Inventor
						DateTime dt = DateTime.Now;
						string backupTargetPath = toLocalFolder + @"\backup";
						if (Directory.Exists(backupTargetPath) == false) Directory.CreateDirectory(backupTargetPath);
						string extension = Path.GetExtension(targetPath);
						backupTargetPath = backupTargetPath + @"\" + Path.GetFileNameWithoutExtension(targetPath)
							+ dt.Year + dt.Month + dt.Day + dt.Hour + dt.Minute + extension;
						string fromFtpPathProfile = fromFtpFonder;
							try
							{   //Inventor
								if (File.Exists(targetPath) == true) File.Move(targetPath, backupTargetPath);				 //Count4U

								fromFtpPathProfile = fromFtpFonder + @"\" + fileName;		  //  на ftp	  (from)
								FtpStatusCode result = client.GetFileFromFtp(fileName, targetPath);
								_logger.Info("Copy profile.xml from ftpServer[" + fromFtpPathProfile + "]");
								_logger.Info("with Result [" + result.ToString() + "]");
								messageCreateFolder = messageCreateFolder + "Copy profile.xml from ftpServer[" + fromFtpPathProfile + "]"
									+ " with Result [" + result.ToString() + "]";

								//if (haveBackupFolder == true)
								//{
								//	//string result1 = client.SaveFileToFtp(targetPath, @"backup\" + fileName);
								//	string result1 = client.Rename(fileName, @"backup\" + fileName);
								//}

								//string result2 = client.DeleteFile(fileName);
							}
							catch (Exception exc)
							{
								string message = String.Format("GetFromFtpCommandExecuted (File.Move to unsurePath) {0} => {1}", fromFtpPathProfile, targetPath);
								messageCreateFolder = messageCreateFolder + message;
								_logger.ErrorException(message, exc);
							}
					}
				}
			}
			catch (Exception exc)
			{
				_logger.ErrorException("GetFromFtpCommandExecuted", exc);
				messageCreateFolder = messageCreateFolder + exc.Message;
			}

		}
										 
		private void CopyFileToFtp(string fromLocalFolder, string toFtpFolder, string fileName)
		{												 
			string Host = _userSettingsManager.HostGet().Trim('\\');
			string User = _userSettingsManager.UserGet();
			string Password = _userSettingsManager.PasswordGet();
			try
			{
				using (new CursorWait())
				{
					FtpClient client = new FtpClient(Host, User, Password);
					client.uri = Host;
					string result3 = client.ChangeWorkingDirectory(toFtpFolder);

					string toFTPFilePath = fileName;
					string fromLocalFilePath = fromLocalFolder + @"\" + fileName;

					string result = client.SaveFileToFtp(fromLocalFilePath, toFTPFilePath);
					_logger.Info("Save file [" + fromLocalFilePath + "]" + " to ftp [" + toFTPFilePath + "]");
					_logger.Info("with Result [" + result + "]");
				}
			}
			catch (Exception exc)
			{
				string message = String.Format("CopyFileFromFtp ");
				_logger.ErrorException(message, exc);
			}

		}

	}
}

 