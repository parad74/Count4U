using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.SelectionParams
{
    /// <summary>
    /// Параметры выборки.
    /// 
    /// Selection parameters.
    /// </summary>
    public interface ISelectParams
    {
    //    #warning В базе количество записей Int64, но в LINQ нет перегрузок методов Skip и Take для Int64

        /// <summary>
        /// Пейджинг включен (true) / выключен (false).
        /// 
        /// Enable paging.
        /// </summary>
        bool IsEnablePaging { get; set; }

        /// <summary>
        /// Текущая страница (от 1 до n).
        /// 
        /// Current page (from 1 to n).
        /// </summary>
        int CurrentPage { get; set; }

        /// <summary>
        /// Количество записей на одной странице (больше 0).
        /// 
        /// Count of records on page (greater 0).
        /// </summary>
        int CountOfRecordsOnPage { get; set; }

        /// <summary>
        /// Количество записей, которое необходимо пропустить.
        /// 
        /// Count of skip records.
        /// </summary>
        int CountOfSkipRecords { get; }

        /// <summary>
        /// Условие сортировки.
        /// 
        /// Sort conditions.
        /// </summary>
        /// <example>
        /// Name ASC - сортировка по полю Name в порядке возрастания,
        /// Name DESC - сортировка по полю Name в порядке убывания.
        /// 
        /// Name ASC - sort by field Name in ascending order,
        /// Name DESC - sort by field Name in decreasing order.
        /// </example>
        string SortParams { get; set; }

        /// <summary>
        /// Условия простого фильтра (фильтра по полям).
        /// Коллекция пар ключ-значение:
        /// - ключом является имя поля, на которое устанавливается фильтр,
        /// - значением является параметр фильтра.
        /// Пары соединены условием AND.
        /// 
        /// Conditions of a simple filter (filter by field).
        /// Key-value pair collection:
        /// - key is filed name on which a filter is applied,
        /// - value is filter parameter.
        /// Pairs are connected by the AND condition.
        /// </summary>
        Dictionary<string, FilterParam> FilterParams { get; }

        /// <summary>
        /// Условия простого фильтра в строковом представлении.
        /// 
        /// Conditions of a simple filter in the string representation.
        /// </summary>
        string FilterParamsString { get; }

        /// <summary>
        /// Условия множественного фильтра.
        /// Коллекция пар ключ-значение:
        /// - ключом является имя поля, на которое устанавливается фильтр,
        /// - значением является коллекция значений фильтра (значения соединены условием OR).
        /// Пары соединены условием AND.
        /// 
        /// Conditions of a multiple filter.
        /// Key-value pair collection:
        /// - key is filed name on which a filter is applied,
        /// - value is a collection of filter values ​​(values ​​are connected by the condition OR).
        /// Pairs are connected by the AND condition.
        /// </summary>
        Dictionary<string, FilterListParam> FilterListParams { get; }

        /// <summary>
        /// Условия множественного фильтра в строковом представлении.
        /// 
        /// Conditions of a multiple filter in the string representation.
        /// </summary>
        string FilterListParamsString { get; }

        /// <summary>
        /// Применение параметров фильтрации к запросу.
        /// 
        /// Apply filter to query.
        /// </summary>
        /// <typeparam name="T">
        /// Тип сущностей.
        /// 
        /// Type of entity.
        /// </typeparam>
        /// <param name="query">
        /// Входящий запрос.
        /// 
        /// Input query.
        /// </param>
        /// <returns>
        /// Запрос с параметрами.
        /// 
        /// Output query.
        /// </returns>
        IQueryable<T> ApplyFilterToQuery<T>(IQueryable<T> query);

        /// <summary>
        /// Применение параметров сортировки и пейджинга к запросу.
        /// 
        /// Apply sort and paging to query.
        /// </summary>
        /// <typeparam name="T">
        /// Тип сущностей.
        /// 
        /// Type of entity.
        /// </typeparam>
        /// <param name="query">
        /// Входящий запрос.
        /// 
        /// Input query.
        /// </param>
        /// <returns>
        /// Запрос с параметрами.
        /// 
        /// Output query.
        /// </returns>
        IQueryable<T> ApplySortAndPagingToQuery<T>(IQueryable<T> query);
    }
}
