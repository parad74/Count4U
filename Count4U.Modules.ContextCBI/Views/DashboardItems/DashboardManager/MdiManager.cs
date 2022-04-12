using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Count4U.Common.Helpers;
using Count4U.Common.UserSettings;
using Count4U.Modules.ContextCBI.Events.Misc;
using Count4U.Modules.ContextCBI.Interfaces;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Popup;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.MdiFilter;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.MdiFilter.Items;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using NLog;
using WPF.MDI;
using WindowState = System.Windows.WindowState;

namespace Count4U.Modules.ContextCBI.Views.DashboardItems.DashboardManager
{
    public class MdiManager
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly MdiContainer _mdiContainer;
        private readonly List<MdiRegion> _regions;
        private readonly IRegionManager _regionManager;
        private readonly string _dashboardName;
        private readonly IUserSettingsManager _userSettingsManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly MdiRegionLayoutCollection _defaultLayout;

        public MdiManager(
            MdiContainer mdiContainer,
            IRegionManager regionManager,
            List<MdiRegion> regions,
            string dashboardName,
            IUserSettingsManager userSettingsManager,
            IEventAggregator eventAggregator,
            MdiRegionLayoutCollection defaultLayout = null)
        {
            _defaultLayout = defaultLayout;
            this._eventAggregator = eventAggregator;
            this._userSettingsManager = userSettingsManager;
            this._regionManager = regionManager;
            this._regions = regions;
            this._mdiContainer = mdiContainer;
            this._dashboardName = dashboardName;

            Application.Current.MainWindow.Closing += MainWindow_Closing;
            this._eventAggregator.GetEvent<MdiFilterChangedEvent>().Subscribe(MdiChanged);

            BuildContextMenu();
        }

        public List<MdiRegion> Regions
        {
            get { return _regions; }
        }

        #region events

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            this.Close();
        }

        private void Mdi_Closing(object sender, RoutedEventArgs e)
        {
            MdiChild child = sender as MdiChild;
            if (child == null) return;

            MdiClose(child);
        }

        #endregion

        #region public methods

        public void BuildRegions()
        {
            this._mdiContainer.Children.Clear();

            foreach (MdiRegion region in this.Regions)
            {
                MdiCreate(region);
            }

            OnMdiListChanged();
        }

        public void MdiAllAppearanceApply()
        {
            foreach (MdiRegion region in this.Regions)
            {
                MdiAppearanceApply(region);
            }
        }

        public void Save()
        {
            AllMdiSettingsSave();

            this._userSettingsManager.AdminSave();

            //commit in userSettingsManager
            foreach (MdiRegion region in this.Regions)
            {
                string sectionName = SectionNameBuild(region);
                this._userSettingsManager.AdminCommitSave(sectionName);
            }
        }

        public void Clear()
        {
            foreach (MdiChild mdi in this._mdiContainer.Children)
                MdiUnsubscribe(mdi);

            foreach (MdiRegion region in this.Regions)
            {
                RegionClear(region);
            }
        }

        public void Close()
        {
            Save();

            Clear();

            Application.Current.MainWindow.Closing -= MainWindow_Closing;
            _eventAggregator.GetEvent<MdiFilterChangedEvent>().Unsubscribe(MdiChanged);
        }

        #endregion

        #region private methods

        private void MdiClose(MdiChild mdi)
        {
            MdiRegion dashboardRegion = mdi.Tag as MdiRegion;
            if (dashboardRegion == null)
                return;

            dashboardRegion.IsOpen = false;

            MdiPositionSave(dashboardRegion, false);

            MdiUnsubscribe(mdi);
            RegionClear(dashboardRegion);

            OnMdiListChanged();
        }

        private void AllMdiSettingsSave()
        {
            foreach (MdiRegion region in this.Regions)
            {
                MdiChild mdiChild = this._mdiContainer.Children.FirstOrDefault(r => r.Tag == region);
                if (mdiChild == null) continue;

                MdiPositionSave(region, true);
            }
        }

        private void MdiCreate(MdiRegion dashboardRegion)
        {
            RegionElement regionElement = RegionElementGet(dashboardRegion);

            //mdi saved in db and saved in closed state
            if (regionElement != null && !regionElement.IsOpen)
            {
                dashboardRegion.IsOpen = false;
                return;
            }

            dashboardRegion.IsOpen = true;

            MdiChild mdi = new MdiChild();
            mdi.Name = dashboardRegion.ViewName;
            mdi.Title = dashboardRegion.Title;
            mdi.Focusable = false;
            mdi.Width = 100;
            mdi.Height = 100;
            mdi.Position = new Point(0, 0);
            mdi.Tag = dashboardRegion;
            mdi.IsHebrew = _userSettingsManager.LanguageGet() == enLanguage.Hebrew;

            if (dashboardRegion.MinHeight.HasValue)
            {
                mdi.MinHeight = dashboardRegion.MinHeight.Value;
            }

            if (dashboardRegion.MinWidth.HasValue)
            {
                mdi.MinWidth = dashboardRegion.MinWidth.Value;
            }

            ContentControl contentControl = new ContentControl();

            RegionManager.SetRegionManager(contentControl, this._regionManager);
            RegionManager.SetRegionName(contentControl, dashboardRegion.RegionName);

            //take into account dictionary settings
            Uri uri;
            if (dashboardRegion.Settings == null)
            {
                uri = new Uri(dashboardRegion.ViewName, UriKind.Relative);
            }
            else
            {
                UriQuery uriQuery = new UriQuery();
                foreach (var kvp in dashboardRegion.Settings)
                {
                    uriQuery.Add(kvp.Key, kvp.Value);
                }

                uri = new Uri(dashboardRegion.ViewName + uriQuery, UriKind.Relative);
            }

            IRegion region = this._regionManager.Regions[dashboardRegion.RegionName];
            region.RequestNavigate(uri);

            this._mdiContainer.Children.Add(mdi);
            mdi.Content = contentControl;
            mdi.Closing += Mdi_Closing;

            if (mdi.HeaderGridContextMenu != null)
            {
                foreach (MenuItem mdiHeaderMnu in mdi.HeaderGridContextMenu.Items)
                {
                    mdiHeaderMnu.Click += MdiHeaderGridMenuItem_Click;
                }
            }
        }

        private string SectionNameBuild(MdiRegion dashboardRegion)
        {
            return String.Format("{0}_{1}_{2}", this._dashboardName, dashboardRegion.ViewName, dashboardRegion.RegionName);
        }

        private RegionElement RegionElementGet(MdiRegion dashboardRegion)
        {
            string sectionName = SectionNameBuild(dashboardRegion);
            return this._userSettingsManager.RegionElementGet(sectionName);
        }

        private void MdiHeaderGridMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                var tuple = menuItem.Tag as Tuple<string, MdiChild>;
                if (tuple == null)
                    return;

                string theme = tuple.Item1;
                MdiChild mdi = tuple.Item2;

                mdi.Theme = theme;
            }
        }

        private void MdiUnsubscribe(MdiChild mdi)
        {
            if (mdi == null)
                return;

            mdi.Closing -= Mdi_Closing;

            if (mdi.HeaderGridContextMenu != null)
                foreach (MenuItem menuItem in mdi.HeaderGridContextMenu.Items)
                    menuItem.Click -= MdiHeaderGridMenuItem_Click;

            var cc = mdi.Content as ContentControl;
            if (cc != null)
            {
                var uc = cc.Content as UserControl;
                if (uc != null)
                {
                    var viewModel = uc.DataContext as IMdiChild;
                    if (viewModel != null)
                    {
                        viewModel.Clear();
                    }
                }
            }
        }

        private void MdiAppearanceApply(MdiRegion dashboardRegion)
        {
            if (dashboardRegion == null) return;

            RegionElement regionElement = RegionElementGet(dashboardRegion);

            MdiChild mdiChild = this._mdiContainer.Children.FirstOrDefault(r => r.Tag == dashboardRegion);
            if (mdiChild != null)
            {
                mdiChild.Position = regionElement == null ? dashboardRegion.Position : new Point(regionElement.X, regionElement.Y);
                mdiChild.Width = regionElement == null ? dashboardRegion.Width : regionElement.Width;
                mdiChild.Height = regionElement == null ? dashboardRegion.Height : regionElement.Height;

                string theme = "Theme1";
                if (regionElement != null && !String.IsNullOrEmpty(regionElement.Theme))
                    theme = regionElement.Theme;

                mdiChild.Theme = theme;
            }
        }

        private void RegionClear(MdiRegion dashboardRegion)
        {
            if (dashboardRegion == null) return;

            string regionName = dashboardRegion.RegionName;

            if (String.IsNullOrEmpty(regionName) == false)
            {
                if (this._regionManager.Regions.Any(r => r.Name == regionName))
                {
                    bool b = this._regionManager.Regions.Remove(regionName);
                }
            }
        }

        private void MdiPositionSave(MdiRegion dashboardRegion, bool isOpen)
        {
            if (dashboardRegion == null) return;

            MdiChild mdiChild = this._mdiContainer.Children.FirstOrDefault(r => r.Tag == dashboardRegion);
            if (mdiChild == null) return;

            string sectionName = SectionNameBuild(dashboardRegion);

            RegionElement element = new RegionElement();
            element.Theme = mdiChild.Theme;

            if (mdiChild.WindowState == WindowState.Normal)
            {
                element.Width = (int)mdiChild.ActualWidth == 0 ? dashboardRegion.Width : mdiChild.ActualWidth;
                element.Height = (int)mdiChild.ActualHeight == 0 ? dashboardRegion.Height : mdiChild.ActualHeight;
                element.X = mdiChild.Position.X;
                element.Y = mdiChild.Position.Y;
            }
            else
            {
                element.Width = dashboardRegion.Width;
                element.Height = dashboardRegion.Height;
                element.X = dashboardRegion.Position.X;
                element.Y = dashboardRegion.Position.Y;
            }

            element.IsOpen = isOpen;

            this._userSettingsManager.RegionElementSet(sectionName, element);
        }

        #endregion

        private void MdiChanged(MdiFilterState state)
        {
            foreach (MdiFilterItem filterItem in state.Mdis)
            {
                MdiChild mdiChild = this._mdiContainer.Children.FirstOrDefault(r =>
                                                                                   {
                                                                                       MdiRegion md = r.Tag as MdiRegion;
                                                                                       if (md == null)
                                                                                           return false;

                                                                                       return md.DashboardName == filterItem.DashboardName &&
                                                                                              md.RegionName == filterItem.RegionName &&
                                                                                              md.ViewName == filterItem.ViewName;
                                                                                   });

                if (filterItem.IsOpen && mdiChild != null) //already open
                    continue;

                if (!filterItem.IsOpen && mdiChild == null) //already closed
                    continue;

                MdiRegion mdiRegionDashboard = _regions.FirstOrDefault(r =>
                                                                           {
                                                                               return r.DashboardName == filterItem.DashboardName &&
                                                                                      r.RegionName == filterItem.RegionName &&
                                                                                      r.ViewName == filterItem.ViewName;
                                                                           });

                if (mdiRegionDashboard == null)
                    continue;

                mdiRegionDashboard.IsOpen = filterItem.IsOpen; //copy to local element

                if (mdiRegionDashboard.IsOpen)
                {
                    RegionElement regionElement = RegionElementGet(mdiRegionDashboard);
                    regionElement.IsOpen = true;

                    MdiCreate(mdiRegionDashboard);
                    MdiAppearanceApply(mdiRegionDashboard);
                }
                else
                {
                    if (mdiChild == null)
                        continue; //error, must be open

                    MdiClose(mdiChild);
                    mdiChild.Close();    //close actual gui                                   
                }
            }

            OnMdiListChanged();
        }

        public delegate void MdiChangedHandler(object sender, bool isAllMdiOpen);

        public event MdiChangedHandler MdiListChanged = delegate { };

        private void OnMdiListChanged()
        {
            MdiChangedHandler handler = MdiListChanged;
            if (handler != null) handler(this, IsAllMdi());
        }

        public bool IsAllMdi()
        {
            return _regions.All(r => r.IsOpen);
        }

        private void BuildContextMenu()
        {
            ContextMenu menu = new ContextMenu();

            MenuItem item = new MenuItem();
            item.Header = Localization.Resources.View_MdiManager_tbReset;
            item.Click += mnuResetLayout_Click;

            menu.Items.Add(item);

            _mdiContainer.ContextMenu = menu;
        }

        void mnuResetLayout_Click(object sender, RoutedEventArgs e)
        {
            using (new CursorWait())
            {
                try
                {
                    if (_defaultLayout == null) return;

                    foreach (MdiRegion region in this.Regions)
                    {
                        string sectionName = SectionNameBuild(region);

                        MdiRegionLayout layout = _defaultLayout.Get(region);

                        if (layout == null) continue;                      

                        MdiChild mdiChild = this._mdiContainer.Children.FirstOrDefault(r => r.Tag == region);
                        if (mdiChild == null)
                        {
                            RegionElement configElement = _userSettingsManager.RegionElementGet(sectionName);
                            configElement.Width = layout.Width;
                            configElement.Height = layout.Height;
                            configElement.X = layout.X;
                            configElement.Y = layout.Y;
                            _userSettingsManager.RegionElementSet(sectionName, configElement);
                        }
                        else
                        {
                            mdiChild.Width = layout.Width;
                            mdiChild.Height = layout.Height;
                            mdiChild.Position = new Point(layout.X, layout.Y);
                        }                        
                    }

                    Save();
                }
                catch (Exception exc)
                {
                    _logger.ErrorException("mnuResetLayout_Click", exc);
                }
            }
        }
    }
}