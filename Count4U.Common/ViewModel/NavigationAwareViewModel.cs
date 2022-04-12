using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Count4U.Common.Events;
using Count4U.Common.Interfaces;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Count4U.Common.Helpers;
using System.Windows.Media;
using System.IO;
using System.Windows.Media.Imaging;

namespace Count4U.Common.ViewModel
{
    public class NavigationAwareViewModel : NotificationObject, INavigationAware, IModalWindowRequest
    {
        private NavigationContext _navigationContext;
		

		public NavigationAwareViewModel()
        {

        }

        protected NavigationContext NavigationContext
        {
            get { return _navigationContext; }
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (navigationContext == null) return;

            this._navigationContext = navigationContext;
        }

        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public static void AddNavigationContextSettings(NavigationContext navigationContext, Dictionary<string, string> settings)
        {
            if (navigationContext == null) return;
            foreach (var kvp in navigationContext.Parameters)
            {
                if (settings.ContainsKey(kvp.Key) == false)
                {
                    settings.Add(kvp.Key, kvp.Value);
                }
            }
        }

        #region Implementation of IModalWindowRequest

        public event EventHandler<ModalWindowRequestPayload> ModalWindowRequest;

        protected void OnModalWindowRequest(ModalWindowRequestPayload e)
        {
            EventHandler<ModalWindowRequestPayload> handler = ModalWindowRequest;
            if (handler != null) handler(this, e);
        }

        #endregion

		
    }
}