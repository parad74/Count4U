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

namespace Count4U.Common.Web
{
	public class FtpFolderProFile
	{
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly IUserSettingsManager _userSettingsManager;
		private readonly IContextCBIRepository _contextCBIRepository;

		public FtpFolderProFile(IUserSettingsManager userSettingsManager,
			IContextCBIRepository contextCBIRepository)
		{
			this._userSettingsManager = userSettingsManager;
			this._contextCBIRepository = contextCBIRepository;
		}

		 //CUSTOMER Create folder on FTP and PROFILE on webServise
		//rootFonderOnFtp= mINV
		public FtpCommandResult CustomerProfileCreate(Customer newCustomer, string rootFonderOnFtp, ref FtpCommandResult ftpCommandResult) 
		{
			//FtpCommandResult ftpCommandResult = new FtpCommandResult();
			//ftpCommandResult.Successful = SuccessfulEnum.Successful;

			if (newCustomer == null)
			{
				_logger.Error("CustomerProfileCreate ERROR : Customer is Null  ");
				ftpCommandResult.Successful = SuccessfulEnum.NotSuccessful;
				ftpCommandResult.Error = "CustomerProfileCreate ERROR : Customer is Null  ";
				return ftpCommandResult;
			}
  			
			using (new CursorWait("ProfileBuild"))
			{
				string messageCreateFolder = "";

				try
				{
					//FTP
					string folderForCustomerObject = this._contextCBIRepository.BuildLongCodesPath(newCustomer);
					CreatePathOnFtp(rootFonderOnFtp, folderForCustomerObject, ref messageCreateFolder);
					ftpCommandResult.MessageCreateFolder = messageCreateFolder;
				}
				catch (Exception exc)
				{
					_logger.ErrorException("In process create Folder on FTP, happens ERROR : ", exc);
					ftpCommandResult.Successful = SuccessfulEnum.NotSuccessful;
					ftpCommandResult.Error = "In process create Folder on FTP, happens ERROR : " + Environment.NewLine + exc.Message;
					return ftpCommandResult;
				}

				// WEB
				
				string messageResponse = WebCreateProfileCustomer(newCustomer, ref ftpCommandResult);

				string messageText = messageCreateFolder + messageResponse;
				_logger.Info("ProfileCreate : " + messageText);

				return ftpCommandResult;
			}
		}

		public FtpCommandResult InventorProfileCreate(Customer currentCustomer, Branch currentBranch,  Inventor newInventor, string rootFonderOnFtp, ref FtpCommandResult ftpCommandResult) 	 //mINV
		{
			if (currentCustomer == null)
			{
				_logger.Error("InventorProfileCreate ERROR : Customer is Null  ");
				ftpCommandResult.Successful = SuccessfulEnum.NotSuccessful;
				ftpCommandResult.Error = "InventorProfileCreate ERROR : Customer is Null  ";
				return ftpCommandResult;
			}

			if (currentBranch == null)
			{
				_logger.Error("InventorProfileCreate ERROR : Branch is Null  ");
				ftpCommandResult.Successful = SuccessfulEnum.NotSuccessful;
				ftpCommandResult.Error = "InventorProfileCreate ERROR : Branch is Null  ";
				return ftpCommandResult;
			}

			if (newInventor == null)
			{
				_logger.Error("InventorProfileCreate ERROR : Inventor is Null  ");
				ftpCommandResult.Successful = SuccessfulEnum.NotSuccessful;
				ftpCommandResult.Error = "InventorProfileCreate ERROR : Inventor is Null  ";
				return ftpCommandResult;
			}

			using (new CursorWait("ProfileBuild"))
			{
				string messageCreateFolder = "";
				try
				{
					//FTP
					string folderForCustomerObject = this._contextCBIRepository.BuildLongCodesPath(currentCustomer);
					CreatePathOnFtp(rootFonderOnFtp, folderForCustomerObject, ref messageCreateFolder);
					ftpCommandResult.MessageCreateFolder = messageCreateFolder;

					string folderForInventorObject = this._contextCBIRepository.BuildLongCodesPath(newInventor).Trim(@"\".ToCharArray()) + @"\Profile";
					CreatePathOnFtp(rootFonderOnFtp, folderForInventorObject, ref messageCreateFolder);
					ftpCommandResult.MessageCreateFolder = messageCreateFolder;
				}
				catch (Exception exc)
				{
					_logger.ErrorException("In process create Folder on FTP, happens ERROR : ", exc);
					ftpCommandResult.Successful = SuccessfulEnum.NotSuccessful;
					ftpCommandResult.Error = "In process create Folder on FTP, happens ERROR : " + Environment.NewLine + exc.Message;
					return ftpCommandResult;
				}

				// WEB
				//FtpCommandResult ftpCommandResult1 = new FtpCommandResult();
				string messageResponse1 = WebCreateProfileCustomer(currentCustomer, ref ftpCommandResult);
				string messageResponse2 = WebCreateProfileInventor(currentCustomer, currentBranch, newInventor, ref ftpCommandResult);

				string messageText = messageCreateFolder + messageResponse1 + messageResponse2;
				_logger.Info("ProfileCreate : " + messageText);
				return ftpCommandResult;
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
					//ftpCommandResult.MessageCreateFolder = messageCreateFolder;
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
		//FtpCommandResult ftpCommandResult = new FtpCommandResult();
		public string WebCreateProfileCustomer(Customer newCustomer, ref FtpCommandResult ftpCommandResult)
		{
			//ftpCommandResult.Successful = SuccessfulEnum.Successful;

			string messageResponse = "Response from Web Service : " + Environment.NewLine;
			string webServiceLink = _userSettingsManager.WebServiceLinkGet();
			//DefaultWebServiceLink = @"http://api.prod.minv.dimex.co.il/v1/c4u"
			messageResponse = messageResponse + ProfileRESTRepository.RemoteCreateProfileCustomer(newCustomer, webServiceLink, _userSettingsManager, ref ftpCommandResult);
			ftpCommandResult.MessageResponse = messageResponse;

			bool useToo = _userSettingsManager.UseTooGet();
			if (useToo == true)
			{
			
				string webServiceDeveloperLink = _userSettingsManager.WebServiceDeveloperLinkGet();
				if (webServiceLink.ToLower() != webServiceDeveloperLink.ToLower())
				{
					FtpCommandResult ftpCommandResult1 = new FtpCommandResult();
					ftpCommandResult1.Successful = SuccessfulEnum.Successful;
					string messageResponse1 = ProfileRESTRepository.RemoteCreateProfileCustomer(newCustomer, webServiceDeveloperLink, _userSettingsManager, ref ftpCommandResult1);
					ftpCommandResult.Successful1 = ftpCommandResult1.Successful;
					ftpCommandResult.MessageResponse1 = messageResponse1;
				}

			}
			
			return messageResponse;
		}

		//WebServise for Inventor Profile
		public string WebCreateProfileInventor(Customer currentCustomer, Branch currentBranch, Inventor newInventor, ref FtpCommandResult ftpCommandResult)
		{
			string messageResponse = "Response from WebService : " + Environment.NewLine;
			string webServiceLink = _userSettingsManager.WebServiceLinkGet();
			//DefaultWebServiceLink = @"http://api.prod.minv.dimex.co.il/v1/c4u"

			messageResponse = messageResponse + ProfileRESTRepository.RemoteCreateProfileInventor(currentCustomer, currentBranch, newInventor, webServiceLink, _userSettingsManager, ref ftpCommandResult);
			ftpCommandResult.MessageResponse = messageResponse;

			bool useToo = _userSettingsManager.UseTooGet();
			if (useToo == true)
			{
				string webServiceDeveloperLink = _userSettingsManager.WebServiceDeveloperLinkGet();
				if (webServiceLink.ToLower() != webServiceDeveloperLink.ToLower())
				{
					FtpCommandResult ftpCommandResult1 = new FtpCommandResult();
					ftpCommandResult1.Successful = SuccessfulEnum.Successful;
					string messageResponse1 = ProfileRESTRepository.RemoteCreateProfileInventor(currentCustomer, currentBranch, newInventor, webServiceDeveloperLink, _userSettingsManager, ref ftpCommandResult1);
					ftpCommandResult.Successful1 = ftpCommandResult1.Successful;
					ftpCommandResult.MessageResponse1 = messageResponse1;
				}

			}
			return messageResponse;
		}

		//  Create folder on ftp
		//mINV\Customer\<CustomerCode>\Branch\BranchCode>\Inventor\<InventorCode>\Profile\<profile>
		//rootFonderOnFtp = "mINV" 
		public string CreatePathOnFtp(string rootFonderOnFtp, string objectFolder, ref string messageCreateFolder)
		{
			//string host = _userSettingsManager.HostGet().Trim('\\');
			bool enableSsl;
			string host = _userSettingsManager.HostFtpGet(out enableSsl);
			string user = _userSettingsManager.UserGet();
			string password = _userSettingsManager.PasswordGet();

			FtpClient client = new FtpClient(host, user, password, enableSsl);
			string folderResult = client.CreatePathOnFtp(host, rootFonderOnFtp, objectFolder, ref messageCreateFolder);
			_logger.Info(messageCreateFolder);
			return folderResult;

			//objectFolder = objectFolder.Trim(@"\".ToCharArray());
			//string[] foldersInPath = objectFolder.Split(@"\".ToCharArray());
			//foreach (var folder in foldersInPath)
			//{
			//	if (string.IsNullOrWhiteSpace(folder) == true) continue;
			//	string newfolder = folder.Trim();
			//	messageCreateFolder = messageCreateFolder + this.GreateFolderOnFtp(rootFonderOnFtp, newfolder);
			//	rootFonderOnFtp = rootFonderOnFtp + @"\" + newfolder;
			//}

			//return messageCreateFolder;
		}

		//OLD move to 	FtpClient
		//public string GreateFolderOnFtp(string rootFolder, string createFolder)
		//{
		//	string host = _userSettingsManager.HostGet().Trim('\\');
		//	string user = _userSettingsManager.UserGet();
		//	string password = _userSettingsManager.PasswordGet();
		//	string messageCreateFolder = "";

		//	FtpClient client = new FtpClient(host, user, password);
		//	//--------------- find or create folder on ftp   ----------------
		//	client.uri = host;
		//	messageCreateFolder = client.CreateFolderOnFtp(host, rootFolder, createFolder);
		//	_logger.Info(messageCreateFolder);
		//	return messageCreateFolder;


		//	//string result3 = client.ChangeWorkingDirectory(rootFolder);

		//	////string customerfolder = base.CurrentCustomer.Code;													// customerCode
		//	////createFolder
		//	//string[] listDirectory = client.ListDirectory();
		//	//bool newDir = true;
		//	//foreach (string dir in listDirectory)
		//	//{																					 // проверяем есть ли такая папка
		//	//	if (dir.ToLower() == createFolder.ToLower())		//  ToApp/customerCode
		//	//	{
		//	//		newDir = false;
		//	//	}
		//	//}

		//	////string pathFtpCustomerFolder = host + @"\" + exportToPDAFolder + @"\" + base.CurrentCustomer.Code;  
		//	//string pathFtpFolder = host + @"\" + rootFolder + @"\" + createFolder;
		//	////string pathFtpProfileFolder = pathFtpCustomerFolder + @"\profile";
		//	//messageCreateFolder = "";//"On FTP there is Folder for Profile : " + Environment.NewLine;

		//	//if (newDir == true)
		//	//{
		//	//	string result1 = client.MakeDirectory(createFolder);
		//	//	_logger.Info("Create folder on ftpServer[" + createFolder + "]");
		//	//	_logger.Info("with Result [" + result1 + "]");
		//	//	messageCreateFolder = "On FTP try crete Folder for Profile : [" + pathFtpFolder + "] with Result [" + result1 + "]" + Environment.NewLine;
		//	//	return messageCreateFolder;
		//	//}
		//	//else
		//	//{
		//	//	messageCreateFolder = messageCreateFolder + pathFtpFolder + Environment.NewLine;
		//	//	return messageCreateFolder;
		//	//}

		//}

	

		public void CopyFileFromFtp(string fromFtpFonder, string toLocalFolder, string fileName, ref string messageCreateFolder)
		{
			if (Directory.Exists(toLocalFolder) == false) return;
			bool enableSsl;
			//string host = _userSettingsManager.HostGet().Trim('\\');
			string host = _userSettingsManager.HostFtpGet(out enableSsl);
			string user = _userSettingsManager.UserGet();
			string password = _userSettingsManager.PasswordGet();

			FtpClient client = new FtpClient(host, user, password, enableSsl);
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

		public void CopyFileToFtp(string fromLocalFolder, string toFtpFolder, string fileName)
		{												 
			//string host = _userSettingsManager.HostGet().Trim('\\');
			bool enableSsl;
			string host = _userSettingsManager.HostFtpGet(out enableSsl);
			string user = _userSettingsManager.UserGet();
			string password = _userSettingsManager.PasswordGet();
			try
			{

				FtpClient client = new FtpClient(host, user, password, enableSsl);
				client.uri = host;
				string result3 = client.ChangeWorkingDirectory(toFtpFolder);

				string toFTPFilePath = fileName;
				string fromLocalFilePath = fromLocalFolder + @"\" + fileName;

				string result = client.SaveFileToFtp(fromLocalFilePath, toFTPFilePath);
				_logger.Info("Save file [" + fromLocalFilePath + "]" + " to ftp [" + toFTPFilePath + "]");
				_logger.Info("with Result [" + result + "]");

			}
			catch (Exception exc)
			{
				string message = String.Format("CopyFileFromFtp ");
				_logger.ErrorException(message, exc);
			}

		}

	}
}

 