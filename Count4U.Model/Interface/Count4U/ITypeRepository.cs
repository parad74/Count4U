using System;
using Count4U.Model.Count4U;

namespace Count4U.Model.Interface.Count4U
{
    /// <summary>
    /// Интерфейс репозитория для доступа к Type объектам
    /// </summary>
    public interface ITypeRepository
	{
		Types GetTypes(string pathDB);
	}
}
