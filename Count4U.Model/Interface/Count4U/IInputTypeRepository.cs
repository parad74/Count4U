using System;
using Count4U.Model.Count4U;

namespace Count4U.Model.Interface.Count4U
{
    /// <summary>
    /// Интерфейс репозитория для доступа к InputType объектам
    /// </summary>
    public interface IInputTypeRepository
	{
		/// <summary>
		/// Получить список всех  InputType
		/// </summary>
		/// <returns></returns>
		InputTypes GetInputTypes(string pathDB);
         
	}
}
