using System;
using System.Data.Entity.Core.Objects;
using System.Linq;
using Count4U.Model.Count4U.MappingEF;
using Count4U.Model.Interface.Count4U;
using Count4U.Model.Interface;
using System.Collections.Generic;
using Count4U.Model.SelectionParams;
using Microsoft.Practices.ServiceLocation;

namespace Count4U.Model.Count4U
{
    public class SectionEFRepository : BaseEFRepository, ISectionRepository
    {
		private Dictionary<string, Section> _sectionDictionary;
		private readonly IServiceLocator _serviceLocator;

		public SectionEFRepository(IConnectionDB connection,
			IServiceLocator serviceLocator)
			: base(connection)
        {
			this._sectionDictionary = new Dictionary<string, Section>();
			this._serviceLocator = serviceLocator;
		}

        #region BaseEFRepository Members
	

        public override IQueryable<TEntity> AsQueryable<TEntity>(ObjectSet<TEntity> objectSet)
        {
            return objectSet.AsQueryable();
        }

		public Sections GetSections(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var domainObjects = db.Sections.ToList().Select(e => e.ToDomainObject());
				return Sections.FromEnumerable(domainObjects);
			}
		}


		public Sections GetSections(SelectParams selectParams, string pathDB)
		{
			if (selectParams == null)
				return GetSections(pathDB);

			long totalCount = 0;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entities = GetEntities(db, AsQueryable(db.Sections), db.Sections.AsQueryable(),
				  selectParams, out totalCount);
				var domainObjects = entities.Select(e => e.ToDomainObject());
				var result = Sections.FromEnumerable(domainObjects);
				result.TotalCount = totalCount;
				return result;
			}
		}

		public string[] GetSectionCodes(string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				string[] codes = db.Sections.ToList().Select(e => e.SectionCode).ToArray();
				return codes;
			}
		}

		public Section GetSectionByCode(string sectionCode, string pathDB)
		{
			if (string.IsNullOrEmpty(sectionCode) == true) return null;

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByCode(db, sectionCode);
				if (entity == null) return null;
				return entity.ToDomainObject();
			}
		}

		public void Delete(Section section, string pathDB)
		{
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var sectionEntities = db.Sections.Where(e => e.SectionCode == section.SectionCode).ToList();
				if (sectionEntities != null)
				{
					sectionEntities.ForEach(e => db.Sections.DeleteObject(e));
				}

				db.SaveChanges();
			}
		}



		public List<string> GetSectionCodeListByTag(string pathDB, string tag)
		{
			List<string> ret = new List<string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var sections = db.Sections.Where(x => x.Tag.CompareTo(tag) == 0).ToList();
					ret = sections.Select(e => e.SectionCode).Distinct().ToList();
					return ret;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetSectionCodeListByTag", exp);
				}
			}
			return ret;
		}


		public List<string> GetSectionCodeListIncludedTag(string pathDB, string tag)
		{
			List<string> ret = new List<string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var sections = db.Sections.Where(x => x.Tag.Contains(tag) == true).ToList();
					ret = sections.Select(e => e.SectionCode).Distinct().ToList();
					return ret;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetSectionCodeListIncludedTag", exp);
				}
			}
			return ret;
		}

		public void Insert(Section section, string pathDB)
		{
			if (section == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = section.ToEntity();
				db.Sections.AddObject(entity);
				db.SaveChanges();
			}
		}

		public void Insert(Sections sections, string pathDB)
		{
			if (sections == null) return;
			Dictionary<string, Section> sectionFromDBDictionary = GetSectionDictionary(pathDB, true);
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				foreach (var section in sections)
				{
					if (section != null)
					{
						if (sectionFromDBDictionary.ContainsKey(section.SectionCode) == false)
						{
							var entity = section.ToEntity();
							db.Sections.AddObject(entity);
						}
					}
				}
				db.SaveChanges();
			}
		}

		public void Update(Section section, string pathDB)
		{
			if (section == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				var entity = this.GetEntityByCode(db, section.SectionCode);
				if (entity == null) return;
				entity.ApplyChanges(section);
				db.SaveChanges();
			}
		}

		public void Update(Sections sections, string pathDB)
		{
			if (sections == null) return;
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				foreach (Section section in sections)
				{
					if (section == null) continue;
					var entity = this.GetEntityByCode(db, section.SectionCode);
					if (entity == null) continue;
					entity.ApplyChanges(section);
				}
				db.SaveChanges();
			}
		}

		public Dictionary<string, Section> GetSectionDictionary_DescriptionKey(string pathDB)
		{
			Dictionary<string, Section> sectionDictionary = new Dictionary<string, Section>();

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					Sections sections = this.GetSections(pathDB);

					sectionDictionary = sections.Select(e => e).Distinct().ToDictionary(k => k.Description);
				}
				catch { }
			}

			return sectionDictionary;
		}

		public Dictionary<string, Section> GetSectionDictionary_NameKey(string pathDB)
		{
			Dictionary<string, Section> sectionDictionary = new Dictionary<string, Section>();

			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					Sections sections = this.GetSections(pathDB);

					sectionDictionary = sections.Select(e => e).Distinct().ToDictionary(k => k.Name);
				}
				catch { }
			}

			return sectionDictionary;
		}

		public Dictionary<string, Section> GetSectionDictionary(string pathDB,	bool refill = false)
		{
			if (refill == true)
			{
				this.FillSectionDictionary(pathDB);
			}
			return this._sectionDictionary;
		}

		public void ClearSectionDictionary()
		{
			this._sectionDictionary.Clear();
			GC.Collect();
		}

		public void FillSectionDictionary(string pathDB)
		{
			this.ClearSectionDictionary();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					Sections sections = this.GetSections(pathDB);

					this._sectionDictionary = sections.Select(e => e).Distinct().ToDictionary(k => k.SectionCode);
				}
				catch { }
			}
		}

		public List<string> GetSectionCodeList(string pathDB)
		{
			List<string> ret = new List<string>();
			using (var db = new App_Data.Count4UDB(this.BuildCount4UConnectionString(pathDB)))
			{
				try
				{
					var entitys = db.Sections.Select(e => e.SectionCode).Distinct().ToList();
					return entitys;
				}
				catch (Exception exp)
				{
					_logger.ErrorException("GetSectionCodeList", exp);
				}
			}
			return ret;
		}

		public void RepairCodeFromDB(string pathDB)
		{
			IProductRepository productRepository = this._serviceLocator.GetInstance<IProductRepository>();
			List<string> sectionCodeListFromProduct = productRepository.GetSectionCodeList(pathDB);			//из
			List<string> sectionCodeListFromSection = this.GetSectionCodeList(pathDB); //в
			Dictionary<string, string> difference = new Dictionary<string, string>();

			foreach (var sectionCodeFromProduct in sectionCodeListFromProduct)			   //из
			{
				if (sectionCodeListFromSection.Contains(sectionCodeFromProduct) == false)		 //в
				{
					difference[sectionCodeFromProduct] = sectionCodeFromProduct;
				}
			}

			foreach (KeyValuePair<string, string> keyValuePair in difference)
			{
				if (String.IsNullOrWhiteSpace(keyValuePair.Value) == false)
				{
					Section sectionNew = new Section();
					sectionNew.SectionCode = keyValuePair.Value;
					sectionNew.Name = keyValuePair.Value;
					//if (sectionNew.SectionCode == DomainUnknownCode.UnknownSection)
					//{
					//    sectionNew.Name = DomainUnknownName.UnknownSection;
					//}
					sectionNew.Description = "Repair from Product";
					this.Insert(sectionNew, pathDB);
				}
			}

			Section unknownSection = GetSectionByCode(DomainUnknownCode.UnknownSection,  pathDB);
			if (unknownSection == null)
			{
				Section sectionNew = new Section();
				sectionNew.SectionCode = DomainUnknownCode.UnknownSection;
				sectionNew.Name = DomainUnknownName.UnknownSection;
				sectionNew.Description = "Repair";
				this.Insert(sectionNew, pathDB);
			}
			else
			{
				unknownSection.Name = DomainUnknownName.UnknownSection;
				this.Update(unknownSection, pathDB);
			}
		}
  
        #endregion

		private App_Data.Section GetEntityByCode(App_Data.Count4UDB db, string sectionCode)
		{
			var entity = AsQueryable(db.Sections).FirstOrDefault(e => e.SectionCode.CompareTo(sectionCode) == 0);
			return entity;
		}

	
    }
}
