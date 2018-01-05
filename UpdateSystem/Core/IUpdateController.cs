using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CodeElements.UpdateSystem.Core
{
    public interface IUpdateController
    {
        Guid ProjectId { get; }
        RSAParameters PublicKey { get; }
        IEnvironmentManager Environment { get; }
        HttpClient HttpClient { get; }
        JsonSerializerSettings JsonSerializerSettings { get; }
        Uri UpdateSystemApiUri { get; }
        IVersionProvider VersionProvider { get; }

        Task<UpdatePackageSearchResult> SearchForNewUpdatePackagesAsync();
    }
}