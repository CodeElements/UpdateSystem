using System;
using System.Net.Http;

namespace CodeElements.UpdateSystem.Windows.Wpf
{
    /// <summary>
    ///     The <see cref="UpdateController{TEnvironmentSettings}" /> for Wpf applications
    /// </summary>
    public class WpfUpdateController : UpdateController<WpfPatcher>
    {
        /// <inheritdoc />
        public WpfUpdateController(Guid projectId, HttpMessageHandler httpMessageHandler) : base(projectId,
            httpMessageHandler)
        {
        }

        /// <inheritdoc />
        public WpfUpdateController(Guid projectId, HttpClient httpClient) : base(projectId, httpClient)
        {
        }

        /// <inheritdoc />
        public WpfUpdateController(Guid projectId) : base(projectId)
        {
        }
    }
}