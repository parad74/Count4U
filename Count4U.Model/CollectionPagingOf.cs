using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model
{
    /// <summary>
    /// Коллекция сущностей предметной области с пейджингом.
    /// </summary>
    /// <typeparam name="T">Тип сущности предметной области.</typeparam>
    public class CollectionPagingOf<T> : CollectionOf<T>
    {
        public long TotalCount { get; internal set; }
    }
}
