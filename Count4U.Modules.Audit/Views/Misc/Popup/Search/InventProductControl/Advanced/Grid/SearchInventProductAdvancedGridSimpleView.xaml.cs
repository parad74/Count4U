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
using Count4U.Model.Count4U;
using Count4U.Model.Count4U.Translation;
using Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.InventProductControl.Grid;
using TR = Count4U.Model.Count4U.Translation.TypedReflection<Count4U.Model.Count4U.IturAnalyzes>;

namespace Count4U.Modules.Audit.Views.Misc.Popup.Search.InventProductControl.Advanced.Grid
{
    /// <summary>
    /// Interaction logic for SearchInventProductAdvancedGridSimpleView.xaml
    /// </summary>
    public partial class SearchInventProductAdvancedGridSimpleView : UserControl
    {
        private readonly IPropertyTranslation _propertyTranslation;

        public SearchInventProductAdvancedGridSimpleView(
            SearchInventProductAdvancedGridSimpleViewModel viewModel,
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
            columnLocationName.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.LocationName));
            columnIturCode.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.IturCode));
            columnIturERPCode.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.ERPIturCode));
            columnIturDisabled.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.Itur_Disabled));
            columnIturStatusGroupBit.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.Itur_StatusIturGroupBit));
            columnIturNumberPrefix.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.Itur_NumberPrefix));
            columnIturNumberSuffix.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.Itur_NumberSufix));
            columnDocNum.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.DocNum));
            columnIPNum.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.IPNum));
            columnInputTypeCode.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.InputTypeCode));
            columnQuantityDifference.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.QuantityDifference));
            columnQuantityEdit.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.QuantityEdit));
            columnValueBuyDifference.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.ValueBuyDifference));
            columnValueBuyEdit.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.ValueBuyEdit));
            columnPDA_StatusInventProductBit.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.PDA_StatusInventProductBit));
            columnTypeMakat.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.TypeMakat));
            columnMakat.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.Makat));
            columnBarcode.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.Barcode));
            columnPriceBuy.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.PriceBuy));
            columnPriceSale.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.PriceSale));
            columnProductName.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.ProductName));
            columnSupplierName.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.SupplierName));
            columnSectionName.Header = _propertyTranslation.GetTranslation(TR.GetPropertyInfo(r => r.SectionName));
        }
    }
}
