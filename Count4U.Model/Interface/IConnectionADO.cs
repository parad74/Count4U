using System;
namespace Count4U.Model.Interface
{
    /// <summary>
    /// Интерфейс для подключения к БД
	/// Interface connection to DB x
    /// </summary>
    public interface IConnectionADO
    {
		string BuildPathFileADO(string subFolder, string fileDB);
		string GetConnectionString(string pathFileDB);
		string GetADOConnectionStringBySubFolder(string subFolder, string fileDB);
		string CopyEmptyMobileDB(string toFolderPath);
    }
}
