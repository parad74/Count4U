using System;
using Count4U.Model.Interface.Count4U;

namespace Count4U.Model.Count4U
{
    public class SectionRepository : ISectionRepository
	{
        private Sections _list;

        #region ISectionRepository Members

		public Sections GetSections(string pathDB)
        {
            if (this._list == null)
            {
                this._list = new Sections {
                    new Section() { ID = 1, Name = "Section1", Description = "Section1" },
                    new Section() { ID = 2, Name = "Section2", Description = "Section2" },
                };
            }
            return this._list;
        }

        #endregion

		#region ISectionRepository Members


		public Sections GetSections(SelectionParams.SelectParams selectParams, string pathDB)
		{
			throw new NotImplementedException();
		}

		public System.Collections.Generic.Dictionary<string, Section> GetSectionDictionary(string pathDB, bool refill = false)
		{
			throw new NotImplementedException();
		}

		public void ClearSectionDictionary()
		{
			throw new NotImplementedException();
		}

		public void FillSectionDictionary(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ISectionRepository Members


		public string[] GetSectionCodes(string pathDB)
		{
			throw new NotImplementedException();
		}

		public void Delete(Section section, string pathDB)
		{
			throw new NotImplementedException();
		}

		public void Insert(Section section, string pathDB)
		{
			throw new NotImplementedException();
		}

		public void Insert(Sections sections, string pathDB)
		{
			throw new NotImplementedException();
		}

		public void Update(Section section, string pathDB)
		{
			throw new NotImplementedException();
		}

		public void Update(Sections sections, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ISectionRepository Members


		public Section GetSectionByCode(string sectionCode, string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ISectionRepository Members


		public System.Collections.Generic.List<string> GetSectionCodeList(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ISectionRepository Members


		public void RepairCodeFromDB(string pathDB)
		{
			throw new NotImplementedException();
		}

		#endregion


		public System.Collections.Generic.Dictionary<string, Section> GetSectionDictionary_DescriptionKey(string pathDB)
		{
			throw new NotImplementedException();
		}

		public System.Collections.Generic.Dictionary<string, Section> GetSectionDictionary_NameKey(string pathDB)
		{
			throw new NotImplementedException();
		}


		#region ISectionRepository Members


		public System.Collections.Generic.List<string> GetSectionCodeListByTag(string pathDB, string tag)
		{
			throw new NotImplementedException();
		}

		public System.Collections.Generic.List<string> GetSectionCodeListIncludedTag(string pathDB, string tag)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
