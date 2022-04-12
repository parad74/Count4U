using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Count4U.Modules.ContextCBI.ViewModels.Pack.Unpack
{
    public class EntryComparer : IComparer, IComparer<string>
    {
        private static Dictionary<char, int> _weights = new Dictionary<char, int>()
                {
                   {'m', 10},  {'c', 20}, {'b', 30}, {'i', 40},
                };

        private int GetWeight(string s)
        {
            char key = s.ToLower().First();
            if (_weights.Keys.Contains(key))
            {
                return _weights[key];
            }

            return -1;
        }

        public int Compare(string x, string y)
        {
            if (x.ToLower() == y.ToLower())
                return 0;

            int xWeight = GetWeight(x);
            int yWeight = GetWeight(y);

            if (xWeight != -1 || yWeight != -1)
                return xWeight.CompareTo(yWeight);

            return x.CompareTo(y);
        }

        public int Compare(object x, object y)
        {
            return Compare(((UnpackBaseItem)x).Path, ((UnpackBaseItem)y).Path);
        }
    }
}