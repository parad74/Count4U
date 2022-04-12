using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.Ini;
using Count4U.Configuration.Interfaces;
using Count4U.Model.Count4U;
using Count4U.Model.Count4U.Translation;
using Count4U.Model.Interface;
using NLog;
using TRE = Count4U.Model.Count4U.Translation.TypedReflection<Count4U.Configuration.Dynamic.EditorTemplate>;
using TRP = Count4U.Model.Count4U.Translation.TypedReflection<Count4U.Configuration.Dynamic.PropertyLink>;
using Type = System.Type;

namespace Count4U.Configuration.Dynamic
{
    public class EditorTemplateRepository : IEditorTemplateRepository
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IDBSettings _dbSettings;
        private readonly IIniFileParser _iniFileParser;
        private readonly IFieldLinkRepository _fieldLinkRepository;

        private readonly List<PropertyInfo> _editorTemplateProperties;
        private readonly List<PropertyInfo> _propertyLinkProperties;
        private readonly IAllowedAsPropertyLinkRepository _allowedAsPropertyLinkRepository;

        public EditorTemplateRepository(
            IDBSettings dbSettings,
            IIniFileParser iniFileParser,
            IAllowedAsPropertyLinkRepository allowedAsPropertyLinkRepository,
            IFieldLinkRepository fieldLinkRepository)
        {
            _fieldLinkRepository = fieldLinkRepository;
            _allowedAsPropertyLinkRepository = allowedAsPropertyLinkRepository;
            _iniFileParser = iniFileParser;
            _dbSettings = dbSettings;

            _editorTemplateProperties = BuildEditorTemplateProperties();
            _propertyLinkProperties = BuildPropertyLinkProperties();
        }

        #region public

        private List<EditorTemplate> Get()
        {
            List<EditorTemplate> result = new List<EditorTemplate>();

            try
            {
                string confFolder = _dbSettings.UIPropertySetFolderPath();

                if (!Directory.Exists(confFolder))
                    return null;

                DirectoryInfo di = new DirectoryInfo(confFolder);

                foreach (FileInfo fi in di.GetFiles("*.ini"))
                {
					if (fi.Name.StartsWith("~")) continue;
                    List<EditorTemplate> editorTemplateList = ProcessIniFile(fi.FullName);

                    if (editorTemplateList != null)
                    {
                        foreach (EditorTemplate editorTemplate in editorTemplateList)
                        {
                            result.Add(editorTemplate);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("Get", exc);
            }

            return result;
        }

        public List<EditorTemplate> Get(string viewName)
        {
            List<EditorTemplate> list = Get();

            if (list == null) return null;

            var result = list.Where(r => r.ViewName.ToLower() == viewName.ToLower()).OrderBy(r => r.NN).ToList();

            return result;
        }

        public EditorTemplate GetCurrent(string viewName, string dbPath)
        {
            List<EditorTemplate> editorTemplates = Get(viewName);

            if (editorTemplates == null) return null;

            FieldLink fieldLink = _fieldLinkRepository.GetFieldLinkByViewName(viewName, dbPath);
            if (fieldLink == null)
                return editorTemplates.FirstOrDefault();

            return editorTemplates.FirstOrDefault(r => r.Code == fieldLink.EditorTemplate);
        }

        public void SetCurrent(string viewName, string dbPath, EditorTemplate editorTemplate)
        {
            FieldLink fieldLink = _fieldLinkRepository.GetFieldLinkByViewName(viewName, dbPath);

            if (fieldLink == null)
            {
                fieldLink = new FieldLink();
                fieldLink.ViewName = viewName;
                fieldLink.EditorTemplate = editorTemplate.Code;

                _fieldLinkRepository.Insert(fieldLink, dbPath);
            }
            else
            {
                fieldLink.EditorTemplate = editorTemplate.Code;
                _fieldLinkRepository.Update(fieldLink, dbPath);
            }
        }

        #endregion

        private List<EditorTemplate> ProcessIniFile(string filePath)
        {
            List<EditorTemplate> result = new List<EditorTemplate>();

            List<IniFileData> iniData = _iniFileParser.Get(filePath);

            IniFileData globalSection = iniData.FirstOrDefault(r => r.SectionName == "Global"); //global section
            if (globalSection == null)
                return null; //file missing [Global] section    

            EditorTemplate editorTemplate = new EditorTemplate();
            FillObjectFromIniSection(editorTemplate, _editorTemplateProperties, globalSection.Data);

            if (String.IsNullOrWhiteSpace(editorTemplate.Code) ||
                String.IsNullOrWhiteSpace(editorTemplate.DomainType) ||
                String.IsNullOrWhiteSpace(editorTemplate.ViewName)
                )
            {
                return null;
            }

            Type type = System.Type.GetType(String.Format("Count4U.Model.Count4U.{0},Count4U.Model", editorTemplate.DomainType));
            if (type == null)
                return null;
            editorTemplate.Type = type;

			if (String.IsNullOrWhiteSpace(editorTemplate.DetailDomainType) == false)
			{
				Type dType = System.Type.GetType(String.Format("Count4U.Model.Count4U.{0},Count4U.Model", editorTemplate.DetailDomainType));
				if (type == null)
					return null;
				editorTemplate.DetailType = dType;
			}

            string localization = UtilsMisc.LocalizationFromLocalizationKey(editorTemplate.CodeLocalizationEditorLabel);
            editorTemplate.Title = String.IsNullOrWhiteSpace(localization) ? editorTemplate.DefaultEditorLabel : localization;
            if (String.IsNullOrWhiteSpace(editorTemplate.Title))
            {
                editorTemplate.Title = editorTemplate.Code;
            }

            iniData.Remove(globalSection);

            FillEditorTemplatePropertyLinks(editorTemplate, iniData);

            //split view name with ","

            foreach (string split in editorTemplate.ViewName.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string viewName = split.Trim();

                EditorTemplate copy = UtilsConvert.CreateClone(editorTemplate) as EditorTemplate;
                if (copy != null)
                {
                    copy.ViewName = viewName;
                    result.Add(copy);
                }
            }

            return result;
        }

        private void FillEditorTemplatePropertyLinks(EditorTemplate editorTemplate, IEnumerable<IniFileData> iniData)
        {
            List<PropertyInfo> allowed = _allowedAsPropertyLinkRepository.Get(editorTemplate.Type);
			List<PropertyInfo> allowedDetailType = _allowedAsPropertyLinkRepository.Get(editorTemplate.DetailType);

            foreach (IniFileData iniSection in iniData)//sections == property links
            {
                PropertyLink propertyLink = new PropertyLink();

                FillObjectFromIniSection(propertyLink, _propertyLinkProperties, iniSection.Data);

                if (String.IsNullOrWhiteSpace(propertyLink.PropertyNameInDomainType) ||
                    String.IsNullOrWhiteSpace(propertyLink.DefaultEditorLabel))
                    continue;

                if (allowed != null)
                {
                    if (allowed.Any(r => r.Name.ToLower() == propertyLink.PropertyNameInDomainType.ToLower()) == true)					   //false continue
                    {
						editorTemplate.PropertyLinkList.Add(propertyLink);
                        //continue;
                    }
                }

				if (allowedDetailType != null)
				{
					if (allowedDetailType.Any(r => r.Name.ToLower() == propertyLink.PropertyNameInDomainType.ToLower()) == true)					   //false continue
					{
						propertyLink.PropertyCode = iniSection.SectionName;
						editorTemplate.DetailPropertyLinkList.Add(propertyLink);
						editorTemplate.DetailPropertyLinkDictionary[iniSection.SectionName] = propertyLink;
						//continue;
					}
				}
				//editorTemplate.PropertyLinkList.Add(propertyLink);
               
            }

		

			//editorTemplate.DetailPropertyLinkList.Add(propertyLink);
        }

        private List<PropertyInfo> BuildEditorTemplateProperties()
        {
            List<PropertyInfo> result = new List<PropertyInfo>();

            result.Add(TRE.GetPropertyInfo(r => r.Code));
            result.Add(TRE.GetPropertyInfo(r => r.CodeLocalizationEditorLabel));
            result.Add(TRE.GetPropertyInfo(r => r.DefaultEditorLabel));
            result.Add(TRE.GetPropertyInfo(r => r.DomainType));
			result.Add(TRE.GetPropertyInfo(r => r.DetailDomainType));
            result.Add(TRE.GetPropertyInfo(r => r.ViewName));
			result.Add(TRE.GetPropertyInfo(r => r.DetailPart));
            result.Add(TRE.GetPropertyInfo(r => r.NN));

            return result;
        }

        private List<PropertyInfo> BuildPropertyLinkProperties()
        {
            List<PropertyInfo> result = new List<PropertyInfo>();

            result.Add(TRP.GetPropertyInfo(r => r.CodeLocalizationEditorLabel));
            result.Add(TRP.GetPropertyInfo(r => r.DefaultEditorLabel));
            result.Add(TRP.GetPropertyInfo(r => r.NN));
            result.Add(TRP.GetPropertyInfo(r => r.PropertyNameInDomainType));
			result.Add(TRP.GetPropertyInfo(r => r.PropertyCode));
            return result;
        }

        private void FillObjectFromIniSection(object obj, List<PropertyInfo> propertyList, Dictionary<string, string> data)
        {
            foreach (KeyValuePair<string, string> keyValue in data)
            {
                string key = keyValue.Key;
                string value = keyValue.Value;

                PropertyInfo pi = propertyList.FirstOrDefault(r => r.Name.ToLower() == key.ToLower());
                if (pi == null)
                    continue;

                if (pi.PropertyType.FullName == typeof(string).FullName)
                {
                    pi.SetValue(obj, value, null);
                }
                else if (pi.PropertyType.FullName == typeof(int).FullName)
                {
                    int i;
                    if (int.TryParse(value, out i))
                    {
                        pi.SetValue(obj, i, null);
                    }
                }
                else if (pi.PropertyType.FullName == typeof(long).FullName)
                {
                    long l;
                    if (long.TryParse(value, out l))
                    {
                        pi.SetValue(obj, l, null);
                    }
                }
                else if (pi.PropertyType.FullName == typeof(double).FullName)
                {
                    double d;
                    if (double.TryParse(value, out d))
                    {
                        pi.SetValue(obj, d, null);
                    }
                }
            }
        }
    }
}
