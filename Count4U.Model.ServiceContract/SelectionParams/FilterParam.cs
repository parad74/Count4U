using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.SelectionParams
{
     [Serializable]
    public class FilterParam
    {
        /// <summary>
        /// Оператор фильтра.
        /// 
        /// Filter operator.
        /// </summary>
        public FilterOperator Operator { get; set; }

        /// <summary>
        /// Значение фильтра.
        /// 
        /// Filter value.
        /// </summary>
        public object Value { get; set; }

		/// <summary>
		/// Вторая граница значение фильтра.
		/// 
		/// Second Filter value.
		/// </summary>
		public object Value1 { get; set; }
    }
}
