using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;
using System.Text;

namespace Count4U.Model.SelectionParams
{
    [Serializable]
    public class SelectParams : ISelectParams
    {
        public SelectParams()
        {
            this._filterParams = new Dictionary<string, FilterParam>();
			this._filterDateTimeParams = new Dictionary<string, FilterParam>();
			this._filterListParams = new Dictionary<string, FilterListParam>();
			this._filterIntListParams = new Dictionary<string, FilterIntListParam>();
			this._filterStringListParams = new Dictionary<string, FilterStringListParam>();
			this._commentDictionary = new Dictionary<string, string>();
            this._extra = new Dictionary<string, object>();
        }

        #region IPagingParams

        private bool _isEnablePaging;
        public bool IsEnablePaging
        {
            get
            {
                this.Validate();
                return this._isEnablePaging;
            }
            set
            {
                this._isEnablePaging = value;
            }
        }

		public override string ToString()
		{
			PropertyInfo[] props = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
			StringBuilder sb = new StringBuilder();
			sb.Append("{");
			for (int i = 0; i < props.Length; i++)
			{
				if (i > 0) sb.Append(", ");
				sb.Append(props[i].Name);
				sb.Append("=");
				sb.Append(props[i].GetValue(this, null));
			}
			sb.Append("}");
			return sb.ToString();
		}

        public int CurrentPage { get; set; }

        public int CountOfRecordsOnPage { get; set; }

		public int CountOfSkipRecords { get { return (this.CurrentPage - 1) * this.CountOfRecordsOnPage; } }

        public string SortParams { get; set; }

        private Dictionary<string, FilterParam> _filterParams;
		public Dictionary<string, FilterParam> FilterParams { get { return this._filterParams; } }
		public string FilterParamsString { get { return this.GetFilterParamsString(); } }

        private Dictionary<string, FilterListParam> _filterListParams;
		public Dictionary<string, FilterListParam> FilterListParams { get { return this._filterListParams; } }
		public string FilterListParamsString { get { return this.GetFilterListParamsString(); } }

		private Dictionary<string, FilterIntListParam> _filterIntListParams;
		public Dictionary<string, FilterIntListParam> FilterIntListParams { get { return this._filterIntListParams; } }
		public string FilterIntListParamsString { get { return this.GetFilterIntListParamsString(); } }


		private Dictionary<string, FilterStringListParam> _filterStringListParams;
		public Dictionary<string,  FilterStringListParam> FilterStringListParams { get { return this._filterStringListParams; } }
		public string FilterStringListParamsString { get { return this.GetFilterStringListParamsString(); } }

		private Dictionary<string, FilterParam> _filterDateTimeParams;
		public Dictionary<string, FilterParam> FilterDateTimeParams { get { return this._filterDateTimeParams; } }

		private Dictionary<string, string> _commentDictionary;

		public Dictionary<string, string> CommentDictionary
		{
			get { return _commentDictionary; }
			set { _commentDictionary = value; }
		}

        public string FilterTemplate { get; set; }

        private readonly Dictionary<string, object> _extra;

        public Dictionary<string, object> Extra
        {
            get { return _extra; }
        }
        
		/// <summary>
		/// 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="query"></param>
		/// <returns></returns>
        public IQueryable<T> ApplyFilterToQuery<T>(IQueryable<T> query)
        {
            // Применение простого фильтра (фильтра по полям).
            // Apply the simple filter (filter by the fields).
			if (String.IsNullOrWhiteSpace(this.FilterParamsString) == false)
			{
				query = query.Where(this.FilterParamsString);
			}
			
            // Применение списочного фильтра (множественный фильтр с long).
            // Apply the listed filter (multiple filter).
            if (String.IsNullOrWhiteSpace(this.FilterListParamsString) == false)
                query = query.Where(this.FilterListParamsString);

			// Применение списочного фильтра (множественный фильтр с int).
			// Apply the listed filter (multiple filter).
			if (String.IsNullOrWhiteSpace(this.FilterIntListParamsString) == false)
				query = query.Where(this.FilterIntListParamsString);

			// Применение списочного фильтра (множественный фильтр со строками).
			// Apply the listed filter (multiple filter).
			if (String.IsNullOrWhiteSpace(this.FilterStringListParamsString) == false)
				query = query.Where(this.FilterStringListParamsString);

			// Применение  фильтра для DateTime параметров
			// Apply DateTime filter 
			if (this._filterDateTimeParams.Count != 0)
			{
				foreach (KeyValuePair<string, FilterParam> keyValuePair in this._filterDateTimeParams)
				{
					if (keyValuePair.Value.Operator == FilterOperator.DateTimeBetween)
					{
						//>=	Value
						DateTime from = (DateTime)keyValuePair.Value.Value;
						from = from.AddHours((-1) * from.Hour);
						from = from.AddMinutes((-1) * from.Minute);
						from = from.AddSeconds((-1) * from.Second);
						query = query.Where(string.Format(ConvertFilterOperatorToSharp( 
							FilterOperator.DateTimeGreaterOrEqual), 
						keyValuePair.Key),
						from);
						//(DateTime)keyValuePair.Value.Value);

						//<=  Value1
						DateTime to = (DateTime)keyValuePair.Value.Value1;
						to = to.AddHours((-1) * to.Hour + 23);
						to = to.AddMinutes((-1) * to.Minute + 59);
						to = to.AddSeconds((-1) * to.Second + 59);
						query = query.Where(string.Format(ConvertFilterOperatorToSharp(
							FilterOperator.DateTimeLessOrEqual),
							keyValuePair.Key),
							to);
							//(DateTime)keyValuePair.Value.Value1);

						//        query.Where("Date >= @0 && Date <= @1", 
						//searchStartDate.Date.ToUniversalTime(), 
						//searchEndDate.Date.ToUniversalTime()) 
					}
					else
					{
						query = query.Where(string.Format(ConvertFilterOperatorToSharp(keyValuePair.Value.Operator), 
						keyValuePair.Key),
						(DateTime)keyValuePair.Value.Value);
					}
				}

				//string parm = string.Format("{0} >= @0", "CreateDate");   //"CreateDate >= @0
				//query = query.Where(
				//    string.Format("{0} >= @0", "CreateDate"),
				//    (DateTime)FilterParams["CreateDate"].Value
				//									);

			}
		    // Возврат результата.
            // Return result.
            return query;
        }
	

        public IQueryable<T> ApplySortAndPagingToQuery<T>(IQueryable<T> query)
        {
            // Применение сортировки по параметру, либо по умолчанию.
            // Apply the sorting by the parameter or by default.
            if (String.IsNullOrWhiteSpace(this.SortParams) == false)
                query = query.OrderBy(this.SortParams);
            else
                query = query.OrderBy("ID ASC");

            // Применение пейджинга.
            // Apply the paging.
            if (this.IsEnablePaging == true)
            {
                query = query.Skip(this.CountOfSkipRecords)
                             .Take(this.CountOfRecordsOnPage);
            }

            // Возврат результата.
            // Return result.
            return query;
        }

        #endregion

        private void Validate()
        {
			if (this._isEnablePaging)
            {
                if (this.CurrentPage <= 0)
                    throw new ArgumentException("Invalid parameter", "CurrentPage");
                if (this.CountOfRecordsOnPage <= 0)
                    throw new ArgumentException("Invalid parameter", "CountOfRecordsOnPage");
            }
        }
				 
        private string GetFilterParamsString()
        {
            if (this._filterParams.Count == 0)
                return String.Empty;

			foreach (KeyValuePair<string, FilterParam> keyValuePair in this._filterParams)
			{
				if (keyValuePair.Value.Value.ToString() == "[All]")
				{
					this._filterParams.Remove(keyValuePair.Key.ToString());
				}
			}

			var filterStrings = this._filterParams.Select(p => 
                String.Format(ConvertFilterOperatorToSharp(p.Value.Operator),
                    p.Key, ConvertValueToSharp(p.Value.Value))).ToArray();
            return String.Format("({0})", String.Join(" && ", filterStrings));
        }

	
        private string GetFilterListParamsString()
        {
			if (this._filterListParams.Count == 0)
                return String.Empty;

			var filterStrings = this._filterListParams.Select(p =>
            {
                var valuesString = p.Value.Values.Select(v =>
                    String.Format(ConvertFilterOperatorToSharp(FilterOperator.Multiple), 
                        p.Key, v)).ToArray();
                return String.Format("({0})", String.Join(" || ", valuesString));
            }).ToArray();

			var filterStrings1 = filterStrings.Where(v => v != "()").ToArray();
			if (filterStrings1.Length > 0)
			{
				return String.Format("({0})", String.Join(" && ", filterStrings1));
			}
			else return "";
            //return String.Format("({0})", String.Join(" && ", filterStrings));
        }

		private string GetFilterIntListParamsString()
		{
			if (this._filterIntListParams.Count == 0)
				return String.Empty;

			var filterStrings = this._filterIntListParams.Select(p =>
			{
				var valuesString = p.Value.Values.Select(v =>
					String.Format(ConvertFilterOperatorToSharp(FilterOperator.Multiple),
						p.Key, v)).ToArray();
				return String.Format("({0})", String.Join(" || ", valuesString));
			}).ToArray();

			var filterStrings1 = filterStrings.Where(v => v != "()").ToArray();
			if (filterStrings1.Length > 0)
			{
				return String.Format("({0})", String.Join(" && ", filterStrings1));
			}
			else return "";
			//return String.Format("({0})", String.Join(" && ", filterStrings));
		}

		private string GetFilterStringListParamsString()
		{
			if (this._filterStringListParams.Count == 0)
				return String.Empty;

			foreach (KeyValuePair<string, FilterStringListParam> keyValuePair in this._filterStringListParams)
			{
				//if (string.IsNullOrWhiteSpace(keyValuePair.Value.ToString()) ||
				//    string.IsNullOrWhiteSpace(keyValuePair.Key.ToString()))
				//{
				//    this._filterStringListParams.Remove(keyValuePair.Key.ToString());
				//}
			}

			var filterStrings = this._filterStringListParams.Select(p =>
			{
				var valuesString = p.Value.Values.Select(v =>
					String.Format(ConvertFilterOperatorToSharp(FilterOperator.MultipleString),
						p.Key, ConvertValueToSharp(v))).ToArray();
				return String.Format("({0})", String.Join(" || ", valuesString));
			}).ToArray();

			//(() && ((InputTypeCode = "K") || (InputTypeCode = "B")) && ((TypeMakat = "M") || (TypeMakat = "B") || (TypeMakat = "W")) && ())
			var filterStrings1 =  filterStrings.Where(v => v != "()").ToArray();
			if (filterStrings1.Length > 0)
			{
				return String.Format("({0})", String.Join(" && ", filterStrings1));
			}
			else return "";

			//return String.Format("({0})", String.Join(" && ", filterStrings));
		}

	    private string ConvertFilterOperatorToSharp(FilterOperator filterOperator)
        {
            switch (filterOperator)
            {
                case FilterOperator.Multiple:
                    return "({0} = {1})";
				case FilterOperator.MultipleString:
					return "({0} = {1})";
                case FilterOperator.Equal:
                    return "({0}.Equals({1}))";
                case FilterOperator.Less:
                    return "({0} < {1})";
                case FilterOperator.LessOrEqual:
                    return "({0} <= {1})";
                case FilterOperator.Greater:
                    return "({0} > {1})";
                case FilterOperator.GreaterOrEqual:
                    return "({0} >= {1})";
                case FilterOperator.Contains:
                    return "({0}.Contains({1}))";
                case FilterOperator.StartsWith:
                    return "({0}.StartsWith({1}))";
                case FilterOperator.EndsWith:
                    return "{0}.EndsWith({1})";
				case FilterOperator.DateTimeGreaterOrEqual:
					return "({0} >= @0)";
				case FilterOperator.DateTimeLessOrEqual:
					return "({0} <= @0)";
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        private object ConvertValueToSharp(object value)
        {
            if (value is string)
            {
                // Строку берем в двойные кавычки.
                // Get string in double quotes.
                return String.Format("\"{0}\"", value);
            }

            return value;
        }       
    }
}
