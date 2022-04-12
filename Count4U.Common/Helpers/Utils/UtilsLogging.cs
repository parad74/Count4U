using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using NLog;

namespace Count4U.Common.Helpers
{
    public static class UtilsLogging
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static void WriteNavigationInfoToLog(IUnityContainer container)
        {
            IRegionManager regionManager = container.Resolve<IRegionManager>();
            if (regionManager == null) return;
            IRegion region = regionManager.Regions[Common.RegionNames.ApplicationWindow];
            if (region == null) return;
            IRegionNavigationJournalEntry entry = region.NavigationService.Journal.CurrentEntry;
            if (entry == null) return;

            _logger.Error("--- START OF EXCEPTION NAVIGATION INFO ---");

            _logger.Error("Current: ");
            PrintEntry(entry);

            Type type = typeof(RegionNavigationJournal);
            FieldInfo fi = type.GetField("backStack", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fi != null)
            {

                Stack<IRegionNavigationJournalEntry> backStack = fi.GetValue(region.NavigationService.Journal) as Stack<IRegionNavigationJournalEntry>;
                if (backStack != null)
                {
                    IRegionNavigationJournalEntry[] list = backStack.ToArray();
                    if (list.Any())
                    {
                        _logger.Error("Back stack: ");
                        for (int i = 0; i < list.Count(); i++)
                        {
                            _logger.Error("#{0}", i);
                            entry = list[i];

                            PrintEntry(entry);
                        }
                    }
                }
            }

			//if (container != null)
			//{
			//	try
			//	{
			//		IRegionManager regionManager1 = container.Resolve<IRegionManager>();
			//		List<string> regionNames = regionManager1.Regions.Select(x => x.Name).ToList();
			//	}
			//	catch (Exception ex)
			//	{
			//		string messsage = ex.Message;
			//	}
			//}
            _logger.Error("--- END OF EXCEPTION NAVIGATION INFO ---");
        }

        private static void PrintEntry(IRegionNavigationJournalEntry entry)
        {
            string[] splitUri = entry.Uri.ToString().Split(new[] { '?' });
            if (!splitUri.Any()) return;

            _logger.Error("\tView: {0}", splitUri[0]);

            if (splitUri.Count() > 1)
            {
                string query = splitUri[1];
                string[] splitQuery = query.Split(new[] { '&' });
                foreach (var p in splitQuery)
                {
                    _logger.Error("\t" + p);
                }
            }
        } 
    }
}