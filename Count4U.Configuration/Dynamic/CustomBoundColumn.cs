using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Count4U.Configuration.Dynamic
{
    public class CustomBoundColumn : DataGridBoundColumn
    {
        public string TemplateName { get; set; }
        public string TemplateEditingName { get; set; }
        public DynamicPropertyInfo Info { get; set; }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var binding = new Binding(((Binding)Binding).Path.Path);
            binding.Source = dataItem;

            var content = new ContentControl();
            content.ContentTemplate = (DataTemplate)cell.FindResource(TemplateName);
            content.SetBinding(ContentControl.ContentProperty, binding);
            return content;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            if (string.IsNullOrWhiteSpace(TemplateEditingName))
            {
                return GenerateElement(cell, dataItem);
            }
            else
            {
                var binding = new Binding(((Binding)Binding).Path.Path);
                binding.Source = dataItem;

                var content = new ContentControl();
                content.ContentTemplate = (DataTemplate)cell.FindResource(TemplateEditingName);
                content.SetBinding(ContentControl.ContentProperty, binding);
                return content;
            }
        }
    }
}