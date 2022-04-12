using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace Common.Utility.Constant
{
	public static class GlogalConstantStatic
	{
		public static UnityContainer UnityContainerStatic{ get ; set; }
		public static IServiceLocator ServiceLocatorStatic { get ; set; }
	}
}
