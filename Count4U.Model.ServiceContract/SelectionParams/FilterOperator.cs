using System;

namespace Count4U.Model.SelectionParams
{
    /// <summary>
    /// Оператор фильтра.
    /// 
    /// Filter operator type.
    /// </summary>    
    public enum FilterOperator
    {
        /// <summary>
        /// Не указан.
        /// </summary>
        None = 0,

        /// <summary>
        /// Множественный.
        /// </summary>
        Multiple = 1,

        /// <summary>
        /// Равно.
        /// </summary>
        Equal = 2,

        /// <summary>
        /// Меньше.
        /// </summary>
        Less = 3,

        /// <summary>
        /// Меньше или равно.
        /// </summary>
        LessOrEqual = 4,

        /// <summary>
        /// Больше.
        /// </summary>
        Greater = 5,

        /// <summary>
        /// Больше или равно.
        /// </summary>
        GreaterOrEqual = 6,

        /// <summary>
        /// Содержит.
        /// </summary>
        Contains = 7,

        /// <summary>
        /// Начинается с.
        /// </summary>
        StartsWith = 8,

        /// <summary>
        /// Оканчивается на.
        /// </summary>
        EndsWith = 9,

		/// <summary>
		/// DateTime Больше или равно.
		/// </summary>
		DateTimeGreaterOrEqual = 10,

		/// <summary>
		/// DateTime Меньше или равно.
		/// </summary>
		DateTimeLessOrEqual = 11,

		/// <summary>
		/// DateTime Меньше или равно.
		/// </summary>
		DateTimeBetween = 12,

		/// <summary>
		/// Множественный строковый
		/// </summary>
		MultipleString = 13
    }
}
