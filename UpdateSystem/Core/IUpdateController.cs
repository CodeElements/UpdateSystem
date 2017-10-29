using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CodeElements.UpdateSystem.Core
{
    public interface IUpdateController
    {
        Guid ProjectGuid { get; }
        RSAParameters PublicKey { get; }
        IEnvironmentManager Environment { get; }
        HttpClient HttpClient { get; }
        Uri UpdateSystemApiUri { get; }
        IVersionProvider VersionProvider { get; }

        Task<UpdatePackageSearchResult> SearchForNewUpdatePackages();
    }
}