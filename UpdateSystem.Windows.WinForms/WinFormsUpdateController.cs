using System;
using System.Net.Http;

namespace CodeElements.UpdateSystem.Windows.WinForms
{
    /// <summary>
    ///     The <see cref="UpdateController{TEnvironmentSettings}" /> for Windows Forms applications
    /// </summary>
    public class WinFormsUpdateController : UpdateController<WinFormsPatcher>
    {
        /// <inheritdoc />
        public WinFormsUpdateController(Guid projectId, HttpMessageHandler httpMessageHandler) : base(projectId,
            httpMessageHandler)
        {
        }

        /// <inheritdoc />
        public WinFormsUpdateController(Guid projectId, HttpClient httpClient) : base(projectId, httpClient)
        {
        }

        /// <inheritdoc />
        public WinFormsUpdateController(Guid projectId) : base(projectId)
        {
        }
    }
}