using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Count4U.Common.Interfaces;
using Count4U.Common.ViewModel;
using Count4U.Model;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace Count4U.Common.Misc.PopupExt
{
    public class PopupExtFilter
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IRegionManager _regionManager;
        private readonly INavigationRepository _navigationRepository;
        private readonly IEventAggregator _eventAggregator;

        private string _view;
        private CBIContextBaseViewModel _viewModel;
        private string _region;
        private Button _button;
        private Window _window;
        private double _height;

        private CBIContext _cbiContext;
        private string _cbiDbContext;

        private Action<UriQuery> _applyForQuery;

        public PopupExtFilter(IUnityContainer unityContainer,
            IRegionManager regionManager,
            INavigationRepository navigationRepository,
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _navigationRepository = navigationRepository;
            _regionManager = regionManager;
            _unityContainer = unityContainer;

            _height = 0;

            _eventAggregator.GetEvent<PopupWindowCloseEvent>().Subscribe(PopupWindowClose);
        }

        public CBIContextBaseViewModel ViewModel
        {
            get { return _viewModel; }
            set { _viewModel = value; }
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

        public Action<UriQuery> ApplyForQuery
        {
            get { return _applyForQuery; }
            set { _applyForQuery = value; }
        }

        public string View
        {
            get { return _view; }
            set { _view = value; }
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

        public double Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public void Init()
        {
            if (_unityContainer == null || _button == null ||
                String.IsNullOrWhiteSpace(_region))
            {
                throw new InvalidOperationException();
            }

            _button.Click += ButtonFilter_Click;
        }

        void ButtonFilter_Click(object sender, System.Windows.RoutedEventArgs e)
        {
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

                if (_applyForQuery != null)
                {
                    _applyForQuery(query);
                }

                _window = UtilsPopup.BuildPopup(_button,
                                               this._unityContainer,
                                               _view,
                                               _region,
                                               query,
                                               PopupMode.Filter,
                                               350,
                                               _height
                    );

                _window.Closed += Popup_Closed;

                _window.Show();
            }
        }

        void Popup_Closed(object sender, EventArgs e)
        {
            if (_window != null)
            {
                _window.Closed -= Popup_Closed;
                UtilsPopup.NavigateFrom(_window, _regionManager, _region);
                _window = null;
            }
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