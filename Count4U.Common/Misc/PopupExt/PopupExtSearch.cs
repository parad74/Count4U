using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.Services.Navigation.Data.SearchData;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace Count4U.Common.Misc.PopupExt
{
    public class PopupExtSearch
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IRegionManager _regionManager;
        private readonly INavigationRepository _navigationRepository;
        private readonly IEventAggregator _eventAggregator;

        private CBIContextBaseViewModel _viewModel;
        private object _navigationData;
        private string _region;
        private Button _button;
        private Window _window;

        private CBIContext _cbiContext;
        private string _cbiDbContext;
        private Action<UriQuery> _applyForQuery;

        public PopupExtSearch(IUnityContainer unityContainer,
            IRegionManager regionManager,
            INavigationRepository navigationRepository,
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _navigationRepository = navigationRepository;
            _regionManager = regionManager;
            _unityContainer = unityContainer;

            _eventAggregator.GetEvent<PopupWindowCloseEvent>().Subscribe(PopupWindowClose);
        }

        public CBIContextBaseViewModel ViewModel
        {
            get { return _viewModel; }
            set { _viewModel = value; }
        }

        public object NavigationData
        {
            get { return _navigationData; }
            set { _navigationData = value; }
        }

        public string Region
        {
            get { return _region; }
            set { _region = value; }
        }

        public Button Button
        {
            get { return _button; }
            set { _button = value; }
        }

        public CBIContext CBIContext
        {
            get { return _cbiContext; }
            set { _cbiContext = value; }
        }

        public string CBIDbContext
        {
            get { return _cbiDbContext; }
            set { _cbiDbContext = value; }
        }

        public Action<UriQuery> ApplyForQuery
        {
            get { return _applyForQuery; }
            set { _applyForQuery = value; }
        }

        public void Init()
        {
            if (_unityContainer == null || _button == null ||
                String.IsNullOrWhiteSpace(_region))
            {
                throw new InvalidOperationException();
            }

            _button.Click += SearchButton_Click;
        }

        void SearchButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {           
            double width = Application.Current.MainWindow.ActualWidth - 40;

            if (_window != null)
            {
                _window.Close();
                _window = null;
            }
            else
            {
                UriQuery query = new UriQuery();
                if (_viewModel != null)
                {
                    Utils.AddContextToQuery(query, _viewModel.Context);
                    Utils.AddDbContextToQuery(query, _viewModel.CBIDbContext);
                }
                else
                {
                    Utils.AddContextToQuery(query, _cbiContext);
                    Utils.AddDbContextToQuery(query, _cbiDbContext);
                }

                if (ApplyForQuery != null)
                {
                    ApplyForQuery(query);
                }

                if (_navigationData != null)
                    UtilsConvert.AddObjectToQuery(query, _navigationRepository, _navigationData, Common.NavigationObjects.PopupSearch);

                _window = UtilsPopup.BuildPopup(_button, this._unityContainer, Common.ViewNames.SearchView, _region, query, PopupMode.Search, width);
                _window.Closed += Window_Closed;
                // !!!! Изменила по просьбе Иран а _window.Show();		Eran
				if (UtilsPopup.PikUnPik == true)
				{
					_window.ShowDialog();
				}
				else
				{
					_window.Show();
				}
            }
        }

        void Window_Closed(object sender, EventArgs e)
        {
            if (_window != null)
            {
                _window.Closed -= Window_Closed;

                if (_eventAggregator != null)
                {
                    using (new CursorWait())
                    {
                        _eventAggregator.GetEvent<IturQuantityEditChangedEvent>().Publish(
                            new IturQuantityEditChangedEventPayload()
                            );
                    }
                }
                UtilsPopup.NavigateFrom(_window, _regionManager, _region);
                _window = null;
            }
        }

        public void OnNavigatedFrom()
        {
            if (_window == null) return;
            UtilsPopup.NavigateFrom(_window, _regionManager, _region);
            _window = null;
        }

        private void PopupWindowClose(object o)
        {
            if (_window != null)
            {
                _window.Close();
            }
        }
    }
}