using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Count4U.Model.Count4U.Translation;
using Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.InventProductControl.Grid;
using TR = Count4U.Model.Count4U.Translation.TypedReflection<Count4U.Model.Count4U.IturAnalyzes>;


namespace Count4U.Modules.Audit.Views.Misc.Popup.Search.InventProductControl.Advanced.Grid
{
    /// <summary>
    /// Interaction logic for SearchInventProductAdvancedGridSumView.xaml
    /// </summary>
    public partial class SearchInventProductAdvancedGridSumView : UserControl
    {
        private readonly IPropertyTranslation _propertyTranslation;

        public SearchInventProductAdvancedGridSumView(
            SearchInventProductAdvancedGridSumViewModel viewModel,
            IPropertyTranslation propertyTranslation)
        {
            _propertyTranslation = propertyTranslation;
            InitializeComponent();

            this.DataContext = viewModel;

            viewModel.View = this;

            SetColumnHeader();
        }

        private void SetColumnHeader()
        {
            //columnQuantityDifference.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.QuantityDifference));
            columnQuantityEdit.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.QuantityEdit));
            //columnValueBuyDifference.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.ValueBuyDifference));
            columnValueEdit.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.ValueBuyEdit));
            //columnValueBuyEdit.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.ValueBuyEdit));
            columnMakat.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.Makat));
            columnQuantityOriginalERP.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.QuantityOriginalERP));
            columnValueOriginalERP.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.ValueOriginalERP));
            columnQuantityDifferenceOriginalERP.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.QuantityDifferenceOriginalERP));
            columnValueDifferenceOriginalERP.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.ValueDifferenceOriginalERP));
            columnPriceBuy.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.PriceBuy));
            columnPriceSale.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.PriceSale));
            columnProductName.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.ProductName));
            columnSupplierName.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.SupplierName));
            columnSectionName.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.SectionName));
			columnSupplierCode.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.SupplierCode));
			columnSectionCode.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.SectionCode));
			//columnFamilyCode.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.FamilyCode));
		}
    }
}
