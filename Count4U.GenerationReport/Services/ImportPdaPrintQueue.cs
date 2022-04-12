using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Count4U.Common.Events;
using Microsoft.Practices.Prism.Events;
using NLog;
using Count4U.Common.Extensions;

namespace Count4U.GenerationReport
{
    public class ImportPdaPrintQueue
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly IGenerateReportRepository _generateReportRepository;
        private readonly IEventAggregator _eventAggregator;


        private readonly ConcurrentBag<PrintQueueItem> _queue;
        private bool _isQueueWorkerStarted;
        private bool _isQueueWorking;

        public ImportPdaPrintQueue(
            IGenerateReportRepository generateReportRepository,
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _generateReportRepository = generateReportRepository;
            _queue = new ConcurrentBag<PrintQueueItem>();
            _isQueueWorkerStarted = false;
        }

        public void Enqueue(PrintQueueItem item)
        {
            if (_isQueueWorking == false)
            {
                _eventAggregator.GetEvent<PrintQueueRunningEvent>().Publish(true);
            }

            if (!_isQueueWorkerStarted) //start thread
            {
                _isQueueWorkerStarted = true;
                Task.Factory.StartNew(Process).LogTaskFactoryExceptions("Enqueue");
            }

            _queue.Add(item);
            _isQueueWorking = true;
        }

        public bool IsPrinting
        {
            get { return _isQueueWorking; }
        }

        private void Process()
        {
            while (true)
            {
                PrintQueueItem item;
                while (_queue.TryTake(out item))
                {
                    try
                    {
                        _generateReportRepository.RunPrintReport(item.GenerateReportArgs);
                         //Thread.Sleep(500);
                    }
                    catch (Exception exc)
                    {
                        _logger.ErrorException("Process _generateReportRepository.RunPrintReport", exc);
                    }
                }

                if (_isQueueWorking)
                {
                    _eventAggregator.GetEvent<PrintQueueRunningEvent>().Publish(false);
                    _isQueueWorking = false;
                }

                Thread.Sleep(500);
            }
        }

        public void Stop()
        {
            //PrintQueueItem item;
            //while (_queue.TryTake(out item))
            //{
            //}


            //if (_isQueueWorking)
            //{
            //    _eventAggregator.GetEvent<PrintQueueRunningEvent>().Publish(false);
            //    _isQueueWorking = false;
            //}

            PrintQueueItem item;
            while (!_queue.IsEmpty)
            {
                _queue.TryTake(out item);
            }

            //Thread.Sleep(500);
        }
    }
}
