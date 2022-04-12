using System;
using System.Reflection;
using Count4U.Common.Interfaces;

namespace Count4U.Services
{
    public class ApplicationVersion : IApplicationVersion
    {
        public Version Get()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }
    }
}