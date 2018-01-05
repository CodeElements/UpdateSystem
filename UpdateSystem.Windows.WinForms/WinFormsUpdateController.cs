using System;
using System.Net.Http;

namespace CodeElements.UpdateSystem.Windows.WinForms
{
    public class WinFormsUpdateController : UpdateController<WinFormsPatcher>
    {
        public WinFormsUpdateController(Guid projectId, HttpMessageHandler httpMessageHandler) : base(projectId, httpMessageHandler)
        {
        }

        public WinFormsUpdateController(Guid projectId, HttpClient httpClient) : base(projectId, httpClient)
        {
        }

        public WinFormsUpdateController(Guid projectId) : base(projectId)
        {
        }
    }
}