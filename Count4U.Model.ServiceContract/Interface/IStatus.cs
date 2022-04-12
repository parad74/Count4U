using System;
namespace Count4U.Model.Interface
{
    /// <summary>
    /// Интерфейс статуса, предназначен для однотипной обработки в интерфейсе пользователя
    /// </summary>
    public interface IStatus : IClassificator
    {
		int Bit { get; set; }
    }
}
