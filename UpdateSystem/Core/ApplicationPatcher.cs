using System;

namespace CodeElements.UpdateSystem.Core
{
    /// <summary>
    ///     The application patcher that invokes the patcher to apply the patch
    /// </summary>
    public class ApplicationPatcher
    {
        private readonly IEnvironmentManager _environmentManager;
        private readonly PatcherConfig _patcherConfig;
        private bool _isDisposed;

        internal ApplicationPatcher(IEnvironmentManager environmentManager, PatcherConfig patcherConfig)
        {
            _environmentManager = environmentManager;
            _patcherConfig = patcherConfig;
        }

        /// <summary>
        ///     Patch the application
        /// </summary>
        public void Patch()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(ApplicationPatcher));

            _environmentManager.ExecuteUpdater(_patcherConfig);
        }

        /// <summary>
        ///     Delete all downloaded files
        /// </summary>
        public void Dispose()
        {
            _isDisposed = true;
            _environmentManager.Cleanup(_patcherConfig.ProjectId);
        }
    }
}