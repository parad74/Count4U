using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using Count4U.Common.Converters;
using Count4U.Common.Helpers;
using Count4U.Common.ViewModel;
using Count4U.Configuration.Interfaces;
using NLog;
using Type = System.Type;
using System.Linq;

namespace Count4U.Configuration.Dynamic
{
    public class DynamicRepository
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IPropertyLinkRepository _propertyLinkRepository;
        private readonly IEditorTemplateRepository _editorTemplateRepository;

        public DynamicRepository(
            IPropertyLinkRepository propertyLinkRepository,
            IEditorTemplateRepository editorTemplateRepository)
        {
            _editorTemplateRepository = editorTemplateRepository;
            _propertyLinkRepository = propertyLinkRepository;
        }

        public CBIState CBIState { get; set; }
        public EditorTemplate EditorTemplate { get; set; }

        public void InsertDynamicColumns(DataGrid dataGrid)
        {
            if (dataGrid == null || EditorTemplate == null) return;

            //Type type = System.Type.GetType("Count4U.Model.Count4U.InventProduct,Count4U.Model");

            try
            {
                foreach (DataGridColumn dataGridColumn in dataGrid.Columns.ToList())
                {
                    CustomBoundColumn bound = dataGridColumn as CustomBoundColumn;
                    if (bound != null)
                    {
                        dataGrid.Columns.Remove(bound);
                    }
                }

                List<DynamicPropertyInfo> infoList = GetDynamicPropertyList();
                if (infoList == null) return;

                foreach (DynamicPropertyInfo info in infoList.OrderBy(r => r.PropertyLink.NN))
                {
                    string columnTemplateName = String.Empty;
                    string columnTemplateEditingName = String.Empty;

                    var binding = new Binding(string.Format("DynamicList[{0}]", info.Index));

                    if (info.IsBool())
                        columnTemplateName = "dynamicColumnBoolTemplate";
                    else if (info.IsDouble())
                    {
                        columnTemplateName = "dynamicColumnDoubleTemplate";
                    }
                    else if (info.IsInt())
                    {
                        columnTemplateName = "dynamicColumnIntTemplate";
                    }
                    else if (info.IsString())
                        columnTemplateName = "dynamicColumnStringTemplate";

                    if (info.PropertyInfo.Name == "QuantityInPackEdit")
                    {
                        columnTemplateEditingName = "dynamicColumnIntEditableTemplate";
                    }                  
                  
                    CustomBoundColumn uiColumn = new CustomBoundColumn()
                        {
                            Header = info.Header,
                            Binding = binding,
                            TemplateName = columnTemplateName,
                            TemplateEditingName = columnTemplateEditingName,
                            Info = info,
                            Width = 70
                        };
                    dataGrid.Columns.Insert(info.PropertyLink.NN, uiColumn);
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("InsertDynamicColumns", exc);
            }
        }

        public void FillObjectListWithDynamicProperties(IEnumerable<IDynamicObject> objectList, Func<IDynamicObject, object> getDbObject)
        {
            if (EditorTemplate == null || objectList == null) return;

            try
            {
                List<DynamicPropertyInfo> infoList = GetDynamicPropertyList();
                if (infoList == null) return;

                foreach (DynamicPropertyInfo info in infoList.OrderBy(r => r.Index))
                {
                    foreach (IDynamicObject dynObj in objectList)
                    {
                        object dbObject = getDbObject(dynObj);
                        if (dbObject == null) continue;

                        FillObjectWithDynamicProperty(info, dynObj, dbObject);
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("FillObjectListWithDynamicProperties", exc);
            }
        }

        public void FillObjectWithDynamicProperties(IDynamicObject dynObj, object dbObject)
        {
            if (EditorTemplate == null || dynObj == null || dbObject == null) return;

            List<DynamicPropertyInfo> infoList = GetDynamicPropertyList();
            if (infoList == null) return;

            foreach (DynamicPropertyInfo info in infoList.OrderBy(r => r.Index))
            {
                FillObjectWithDynamicProperty(info, dynObj, dbObject);
            }
        }

        private void FillObjectWithDynamicProperty(DynamicPropertyInfo info, IDynamicObject dynObject, object dbObject)
        {
            object dbValue = info.PropertyInfo.GetValue(dbObject, null);

            DynamicProperty dp = null;
            if (info.IsBool() && dbValue is bool)
            {
                dp = new DynamicPropertyBool((bool)dbValue);
            }
            else if (info.IsDouble() && dbValue is double)
            {
                dp = new DynamicPropertyDouble((double)dbValue);
            }
            else if (info.IsInt() && dbValue is int)
            {
                dp = new DynamicPropertyInt((int)dbValue);
            }
            else if (info.IsString() && dbValue is string)
            {
                dp = new DynamicPropertyString((string)dbValue);
            }

            if (dp != null)
            {
                dp.Info = info;
                dynObject.DynamicList.Add(dp);
            }
        }

        public void FillDbPropertiesForGrid(IDynamicObject obj, object dbObject)
        {
            if (EditorTemplate == null || obj == null || dbObject == null) return;

            try
            {
                List<DynamicPropertyInfo> infoList = GetDynamicPropertyList();
                if (infoList == null) return;

                foreach (DynamicProperty dp in obj.DynamicList)
                {
                    if (dp.Info == null) continue;

                    if (dbObject == null) continue;

                    object dynValue = null;

                    DynamicPropertyBool boolValue = dp as DynamicPropertyBool;
                    if (boolValue != null)
                    {
                        dynValue = boolValue.Value;
                    }
                    DynamicPropertyDouble doubleValue = dp as DynamicPropertyDouble;
                    if (doubleValue != null)
                    {
                        dynValue = doubleValue.Value;
                    }
                    DynamicPropertyInt intValue = dp as DynamicPropertyInt;
                    if (intValue != null)
                    {
                        dynValue = intValue.Value;
                    }
                    DynamicPropertyString stringValue = dp as DynamicPropertyString;
                    if (stringValue != null)
                    {
                        dynValue = stringValue.Value;
                    }

                    if (dynValue != null)
                    {
                        dp.Info.PropertyInfo.SetValue(dbObject, dynValue, null);
                    }
                }
            }
            catch (Exception exc)
            {
                _logger.ErrorException("FillDbPropertiesForGrid", exc);
            }
        }

        private List<DynamicPropertyInfo> GetDynamicPropertyList()
        {
            List<PropertyLink> links = EditorTemplate.PropertyLinkList;
            if (links == null) return null;

            List<DynamicPropertyInfo> result = new List<DynamicPropertyInfo>();
            int i = 0;
            foreach (PropertyLink link in links)
            {
                if (String.IsNullOrWhiteSpace(link.PropertyNameInDomainType)) continue;

                DynamicPropertyInfo info = new DynamicPropertyInfo(link);
                info.Type = EditorTemplate.Type;
                info.PropertyInfo = EditorTemplate.Type.GetProperty(link.PropertyNameInDomainType);

                string header;

                if (!string.IsNullOrWhiteSpace(link.CodeLocalizationEditorLabel))
                {
                    header = UtilsMisc.LocalizationFromLocalizationKey(link.CodeLocalizationEditorLabel);
                }
                else if (!String.IsNullOrWhiteSpace(link.DefaultEditorLabel))
                {
                    header = link.DefaultEditorLabel;
                }
                else
                {
                    header = link.PropertyNameInDomainType;
                }

                info.Header = header;
                info.Index = i++;

                result.Add(info);
            }

            return result;
        }

        public bool IsDynamicPropertyExist(string name)
        {
            if (EditorTemplate == null || String.IsNullOrWhiteSpace(name)) return false;

            bool result = false;

            try
            {
                List<DynamicPropertyInfo> list = GetDynamicPropertyList();

                return list.Any(r => r.PropertyInfo.Name.ToLower() == name.ToLower());
            }
            catch (Exception exc)
            {
                _logger.ErrorException("IsDynamicPropertyExist", exc);
            }

            return false;
        }

        public void RaisePropertyChanged(IDynamicObject dynObject)
        {
            foreach (DynamicProperty dp in dynObject.DynamicList)
            {
                dp.RaisePropertyChanged();
            }
        }
    }
}