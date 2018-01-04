using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;
using CodeElements.UpdateSystem.Client;
using CodeElements.UpdateSystem.Core;
using CodeElements.UpdateSystem.Core.Internal;
using CodeElements.UpdateSystem.Extensions;
using CodeElements.UpdateSystem.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CodeElements.UpdateSystem
{
    /// <summary>
    ///     The core class that provides the methods to search for update packages
    /// </summary>
    public class UpdateController<TEnvironmentSettings> : IUpdateController
        where TEnvironmentSettings : IEnvironmentManager, new()
    {
        private readonly Lazy<HttpClient> _httpClient;
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private readonly Uri _updateSystemApiUri;

        /// <summary>
        ///     Initialize a new instance of <see cref="UpdateController{TEnvironmentSettings}" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the project</param>
        /// <param name="httpMessageHandler"></param>
        public UpdateController(Guid projectId, HttpMessageHandler httpMessageHandler) : this(projectId)
        {
            _httpClient = new Lazy<HttpClient>(() => new HttpClient(httpMessageHandler));
        }

        public UpdateController(Guid projectId, HttpClient httpClient) : this(projectId)
        {
            _httpClient = new Lazy<HttpClient>(() => httpClient);
        }

        public UpdateController(Guid projectId)
        {
            ProjectId = projectId;
            VersionProvider = new AssemblyVersionProvider();
            ChangelogLanguage = CultureInfo.CurrentUICulture;
            Settings = new TEnvironmentSettings();
            _httpClient = new Lazy<HttpClient>(() => new HttpClient());
            _updateSystemApiUri = new Uri($"http://localhost:63195/v1/projects/{projectId:N}/u");
            _jsonSerializerSettings =
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};

            Settings?.Cleanup(projectId);
        }

        /// <summary>
        ///     Filter the versions that should be found and updated to. If set to null, only release versions will be found
        /// </summary>
        public IVersionFilter VersionFilter { get; set; }

        /// <summary>
        ///     The current platform to determine which files are relevant for this install
        /// </summary>
        public IPlatformProvider PlatformProvider { get; set; }

        public TEnvironmentSettings Settings { get; set; }

        /// <summary>
        ///     The desired changelog language. The fallback language is always english
        /// </summary>
        /// <example>
        ///     ChangelogLanguage: de-de
        ///     1. check: is a changelog with de-de available
        ///     2. check: is a changelog with de available
        ///     3. check: is a changelog with en available
        ///     4. check: is a changelog available
        /// </example>
        public CultureInfo ChangelogLanguage { get; set; }

        /// <summary>
        ///     If the update system is connected to a license system, this property must be set to the hardware id of the license
        ///     system for the authentication
        /// </summary>
        public byte[] LicenseSystemHardwareId { get; set; }

        /// <summary>
        ///     The id of the project
        /// </summary>
        public Guid ProjectId { get; }

        /// <summary>
        ///     The public key to validate the signatures of the files
        /// </summary>
        public RSAParameters PublicKey { get; set; }

        IEnvironmentManager IUpdateController.Environment => Settings;
        HttpClient IUpdateController.HttpClient => _httpClient.Value;
        JsonSerializerSettings IUpdateController.JsonSerializerSettings => _jsonSerializerSettings;
        Uri IUpdateController.UpdateSystemApiUri => _updateSystemApiUri;

        /// <summary>
        ///     The current version provider that is used to retrieve the application version
        /// </summary>
        public IVersionProvider VersionProvider { get; set; }

        /// <summary>
        ///     Search for new update packages with the given options
        /// </summary>
        /// <returns>Return the result of the update search</returns>
        public async Task<UpdatePackageSearchResult> SearchForNewUpdatePackages()
        {
            var versionFilter = VersionFilter?.GetSupportedPrereleases();
            if (versionFilter?.Length > 10)
                throw new ArgumentException("A maximum of 10 version filters is allowed.");

            var httpClient = _httpClient.Value;
            httpClient.DefaultRequestHeaders.Remove("VersionFilter");
            if (versionFilter?.Length > 0)
                _httpClient.Value.DefaultRequestHeaders.Add("VersionFilter",
                    JsonConvert.SerializeObject(versionFilter));

            httpClient.DefaultRequestHeaders.Remove("UserSession");
            httpClient.DefaultRequestHeaders.Add("UserSession",
                JsonConvert.SerializeObject(new UserSessionDto
                {
                    OperatingSystem = OperatingSystemProvider.GetOperatingSystemType(),
                    UserLanguage = ChangelogLanguage.TwoLetterISOLanguageName
                }));

            var uri = new Uri(_updateSystemApiUri,
                $"packages/{Uri.EscapeDataString(VersionProvider.GetVersion().ToString())}/check");

            var platforms = PlatformProvider?.GetEncodedPlatforms();
            if (platforms != null)
                uri = uri.AddQueryParameters("platforms", platforms.Value.ToString());
            uri = uri.AddQueryParameters("hwid",
                new Hash(LicenseSystemHardwareId ?? HardwareIdGenerator.GenerateHardwareId()).ToString());

            var response = await _httpClient.Value.GetAsync(uri);
            if (!response.IsSuccessStatusCode)
                throw await GetException(response);

            var updateSearchResult =
                JsonConvert.DeserializeObject<UpdatePackageSearchResult>(
                    await response.Content.ReadAsStringAsync(), _jsonSerializerSettings);

            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", updateSearchResult.Jwt);
            updateSearchResult.Initialize(this);

            if (!updateSearchResult.IsUpdateAvailable)
                Settings?.NoUpdatesFoundCleanup(ProjectId);

            return updateSearchResult;
        }

        internal async Task<Exception> GetException(HttpResponseMessage response)
        {
            var result = await response.Content.ReadAsStringAsync();

            RestError[] errors;
            try
            {
                errors = JsonConvert.DeserializeObject<RestError[]>(result, _jsonSerializerSettings);
            }
            catch (Exception)
            {
                throw new HttpRequestException(result);
            }

            if (errors == null)
                throw new HttpRequestException($"Invalid response (status code: {response.StatusCode}): {result}");

            var error = errors[0];
            switch (error.Type)
            {
                case ErrorTypes.ValidationError:
                    return new ArgumentException(error.Message);
                default:
                    return new UpdateSystemRequestException(error);
            }
        }
    }
}