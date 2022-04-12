using System;
using System.Collections.Generic;
using Count4U.Model;
namespace Count4U.Model.Count4U
{
	public interface IImportProviderDomainRepository
	{
		Dictionary<int, ImportProviderEnum> BitImportProviderDomainDictionary { get; }
		List<ImportProviderDomain> GetImportProviderDomainList(ImportDomainEnum importDomainEnum);
		Dictionary<string, ImportProviderDomain> ImportProviderDomainDictionary { get; }
		string GetFolderName(string importProviderType);
		string GetFileName(string importProviderType);
		string GetFilePath(string importProviderType);
		string GetFolderName(ImportProviderEnum importProviderType);
		string GetFileName(ImportProviderEnum importProviderType);
		string GetFilePath(ImportProviderEnum importProviderType);
		string GetImportProviderType(ImportDomainEnum importDomainEnum, string folderName);
	}
}
