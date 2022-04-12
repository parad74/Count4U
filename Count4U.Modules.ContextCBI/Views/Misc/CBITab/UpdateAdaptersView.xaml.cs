using System.Windows;
using System.Windows.Controls;
using Count4U.Modules.ContextCBI.ViewModels.Misc.CBITab;

namespace Count4U.Modules.ContextCBI.Views.Misc.CBITab
{
    /// <summary>
    /// Interaction logic for UpdateAdaptersView.xaml
    /// </summary>
    public partial class UpdateAdaptersView : UserControl
    {
        public UpdateAdaptersView(UpdateAdaptersViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;
			this.IsVisibleChanged += UpdateAdaptersView_IsVisibleChanged;
        }

		void UpdateAdaptersView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			UpdateAdaptersViewModel vm = this.DataContext as UpdateAdaptersViewModel;
			if (vm.IsShowConfig == true)
			{
				if ((bool)e.NewValue == true && (bool)e.OldValue == false) //открываем закладку
				{
					vm.GotNewFofusConfig();
				}
				else if ((bool)e.NewValue == false && (bool)e.OldValue == true) //закрываем закладку
				{
					vm.LostFocusConfig();
				}
			}
		}
    }
}
