using System;
using Count4U.Common.Events;
using Count4U.Common.Helpers;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.ViewModels
{
    public class BackForwardViewModel : INavigationAware, IRegionMemberLifetime
    {       
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;

        private readonly DelegateCommand _goBackCommand;
        private readonly DelegateCommand _goForwardCommand;        

        public BackForwardViewModel(
            IRegionManager regionManager,
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            this._regionManager = regionManager;

            this._goBackCommand = new DelegateCommand(GoBackCommandExecuted, GoBackCommandCanExecute);
            this._goForwardCommand = new DelegateCommand(GoForwardCommandExecuted, GoForwardCommandCanExecuted);
        }

        public DelegateCommand GoBackCommand
        {
            get { return this._goBackCommand; }
        }

        public DelegateCommand GoForwardCommand
        {
            get { return this._goForwardCommand; }
        }

        private bool GoForwardCommandCanExecuted()
        {
            return UtilsNavigate.CanGoForward(this._regionManager);
        }

        private void GoForwardCommandExecuted()
        {
            using (new CursorWait("GoForwardCommandExecuted"))
            {
                UtilsNavigate.GoForward(this._regionManager);
            }
        }

        private bool GoBackCommandCanExecute()
        {
            return UtilsNavigate.CanGoBack(this._regionManager);
        }

        private void GoBackCommandExecuted()
        {            
            using (CursorWait mw = new CursorWait("GoBackCommandCanExecute"))
            {
                UtilsNavigate.GoBack(this._regionManager);
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<SpecialKeyEvent>().Subscribe(SpecialKey);
        }

        private void SpecialKey(SpecialKey specialKey)
        {
            switch (specialKey)
            {
                case Common.Events.SpecialKey.Back:
                    if (_goBackCommand.CanExecute())
                        _goBackCommand.Execute();
                    break;
                case Common.Events.SpecialKey.Forward:
                    if (_goForwardCommand.CanExecute())
                        _goForwardCommand.Execute();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("specialKey");
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<SpecialKeyEvent>().Unsubscribe(SpecialKey);
        }

        #region Implementation of IRegionMemberLifetime

        public bool KeepAlive
        {
            get { return false; }
        }

        #endregion
    }
}