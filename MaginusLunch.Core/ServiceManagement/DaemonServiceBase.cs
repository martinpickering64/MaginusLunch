using MaginusLunch.Logging;
using System;
using System.Threading;

namespace MaginusLunch.Core.ServiceManagement
{
    public abstract class DaemonServiceBase : IServiceControl
    {
        private readonly ILog _logger;
        protected CancellationTokenSource StopServiceTokenSrc { get; set; }

        protected DaemonServiceBase(ILog logger)
        {
            _logger = logger;
        }

        public bool Start()
        {
            _logger.Info($"{GetType().Name} has been asked to start.");
            StopServiceTokenSrc = new CancellationTokenSource();
            try
            {
                return Start(StopServiceTokenSrc);
            }
            catch
            {
                _logger.Fatal("Failed to start the Daemon Process.");
                return false;
            }
        }

        public bool Stop()
        {
            _logger.Info($"{GetType().Name} has been asked to stop.");
            try
            {
                return Stop(StopServiceTokenSrc);
            }
            catch
            {
                _logger.Fatal("Failed to stop the Daemon Process.");
                return false;
            }
        }

        /// <summary>
        /// Actually start the service.
        /// </summary>
        /// <param name="useThisCancellationTokenSrc">A CancellationTokenSource that provides a 
        /// Token to pass into any async operations that can then be informed to cancel should the 
        /// Service be asked to Stop.</param>
        protected abstract bool Start(CancellationTokenSource useThisCancellationTokenSrc);

        /// <summary>
        /// Actually stop the service.
        /// </summary>
        /// <param name="useThisCancellationTokenSrc">The CancellationTokenSource that should be
        /// cancelled to request any async operations to cease.
        /// </param>
        protected abstract bool Stop(CancellationTokenSource useThisCancellationTokenSrc);
    }
}
