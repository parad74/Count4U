using System;
using Count4U.Model.Count4U;

namespace Count4U.Model.Interface.Count4U
{
    /// <summary>
    /// Интерфейс репозитория для доступа к AccessParentChild объектам
    /// </summary>
    public interface IAccessParentChildRepository
	{
        /// <summary>
        /// Получить все объекты AccessParentChild
        /// </summary>
        /// <returns>Список AccessParentChild</returns>
        AccessParentChilds GetAccessParentChilds();
	}
}
