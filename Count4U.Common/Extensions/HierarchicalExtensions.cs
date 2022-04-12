using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLog;

namespace Count4U.Common.Extensions
{
	public static class HierarchicalExtensions
    {
		private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        public static IEnumerable<T> FlattenHierarchyNode<T>(this T node, Func<T, IEnumerable<T>> getChildEnumerator)
        {
            yield return node;

            if (getChildEnumerator(node) != null)
            {
                foreach (T child in getChildEnumerator(node))
                {
                    foreach (T childOrDescendant in child.FlattenHierarchyNode(getChildEnumerator))
                    {
                        yield return childOrDescendant;
                    }
                }
            }
        }

        public static IEnumerable<T> FlattenHierarchyNodes<T>(this IEnumerable<T> nodes, Func<T, IEnumerable<T>> getChildEnumerator)
        {
            foreach (T xNode in nodes)
                foreach (T xxNode in xNode.FlattenHierarchyNode(getChildEnumerator))
                    yield return xxNode;
        }


		public static void LogTaskFactoryExceptions(this Task task, string from = "")
		{
			//task.ContinueWith(t => { var ignore = t.Exception;},	TaskContinuationOptions.OnlyOnFaulted);
			task.ContinueWith(t => {_logger.Error("Task Factory Exception - " + from  + " : " + t.Exception.Message + " :: "  + t.Exception.InnerExceptions.ToString()); }, 
				TaskContinuationOptions.OnlyOnFaulted);
		}
    }

}