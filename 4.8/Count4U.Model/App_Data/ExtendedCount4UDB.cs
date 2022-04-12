/*
 * Расширенный контекст БД Count4UDB для мониторинга SQL-запросов.
 * 
 * Использует бибилотеки: ..\Lib\EFCachingProvider.dll, ..\Lib\EFProviderWrapperToolkit.dll, ..\Lib\EFTracingProvider.dll.
 *   
 * В файл конфигурации необходимо прописать:
 *  <system.data>
 *    <DbProviderFactories>
 *      <add name="EF Caching Data Provider"
 *           invariant="EFCachingProvider"
 *           description="Caching Provider Wrapper"
 *           type="EFCachingProvider.EFCachingProviderFactory, EFCachingProvider, Version=1.0.0.0, Culture=neutral, PublicKeyToken=def642f226e0e59b" />
 *      <add name="EF Tracing Data Provider"
 *           invariant="EFTracingProvider"
 *           description="Tracing Provider Wrapper"
 *           type="EFTracingProvider.EFTracingProviderFactory, EFTracingProvider, Version=1.0.0.0, Culture=neutral, PublicKeyToken=def642f226e0e59b" />
 *      <add name="EF Generic Provider Wrapper"
 *           invariant="EFProviderWrapper"
 *           description="Generic Provider Wrapper"
 *           type="EFProviderWrapperToolkit.EFProviderWrapperFactory, EFProviderWrapperToolkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=def642f226e0e59b" />
 *    </DbProviderFactories>
 *  </system.data>
 *  
 *  Использование:
 *  using (TextWriter logFile = File.CreateText("D:\\sqllogfile.txt"))
 *  using (var db = new ExtendedCount4UDB(this._connectionString))
 *  {
 *     db.Log = logFile;
 *     
 *     ...
 *  }
 */

/*
using System;
using System.IO;
using EFCachingProvider;
using EFCachingProvider.Caching;
using EFProviderWrapperToolkit;
using EFTracingProvider;

namespace Count4U.Model.App_Data
{
    public partial class ExtendedCount4UDB : Count4UDB
    {
        private TextWriter logOutput;

        public ExtendedCount4UDB()
            : this("name=Count4UDB")
        {
        }

        public ExtendedCount4UDB(string connectionString)
            : base(EntityConnectionWrapperUtils.CreateEntityConnectionWithWrappers(
                    connectionString,
                    "EFTracingProvider",
                    "EFCachingProvider"
            ))
        {
        }

        #region Tracing Extensions

        private EFTracingConnection TracingConnection
        {
            get { return this.UnwrapConnection<EFTracingConnection>(); }
        }

        public event EventHandler<CommandExecutionEventArgs> CommandExecuting
        {
            add { this.TracingConnection.CommandExecuting += value; }
            remove { this.TracingConnection.CommandExecuting -= value; }
        }

        public event EventHandler<CommandExecutionEventArgs> CommandFinished
        {
            add { this.TracingConnection.CommandFinished += value; }
            remove { this.TracingConnection.CommandFinished -= value; }
        }

        public event EventHandler<CommandExecutionEventArgs> CommandFailed
        {
            add { this.TracingConnection.CommandFailed += value; }
            remove { this.TracingConnection.CommandFailed -= value; }
        }

        private void AppendToLog(object sender, CommandExecutionEventArgs e)
        {
            if (this.logOutput != null)
            {
                this.logOutput.WriteLine(e.ToTraceString().TrimEnd());
                this.logOutput.WriteLine();
            }
        }

        public TextWriter Log
        {
            get { return this.logOutput; }
            set
            {
                if ((this.logOutput != null) != (value != null))
                {
                    if (value == null)
                    {
                        CommandExecuting -= AppendToLog;
                    }
                    else
                    {
                        CommandExecuting += AppendToLog;
                    }
                }

                this.logOutput = value;
            }
        }


        #endregion

        #region Caching Extensions

        private EFCachingConnection CachingConnection
        {
            get { return this.UnwrapConnection<EFCachingConnection>(); }
        }

        public ICache Cache
        {
            get { return CachingConnection.Cache; }
            set { CachingConnection.Cache = value; }
        }

        public CachingPolicy CachingPolicy
        {
            get { return CachingConnection.CachingPolicy; }
            set { CachingConnection.CachingPolicy = value; }
        }

        #endregion
    }

}
*/