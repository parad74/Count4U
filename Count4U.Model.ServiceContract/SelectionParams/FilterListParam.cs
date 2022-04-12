using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Count4U.Model.SelectionParams
{
    [Serializable]
    public class FilterListParam
    {
        /// <summary>
        /// Значение фильтра.
        /// 
        /// List filter value.
        /// </summary>
        public List<long> Values { get; set; }
    }

    [Serializable]
	public class FilterStringListParam
	{
		/// <summary>
		/// Значение фильтра.
		/// 
		/// List filter value.
		/// </summary>
		public List<string> Values { get; set; }
	}

    [Serializable]
	public class FilterIntListParam
	{
		/// <summary>
		/// Значение фильтра.
		/// 
		/// List filter value.
		/// </summary>
		public List<int> Values { get; set; }
	}
}
