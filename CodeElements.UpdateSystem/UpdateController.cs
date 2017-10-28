using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using CodeElements.UpdateSystem.Client;
using CodeElements.UpdateSystem.Core;
using CodeElements.UpdateSystem.Core.Internal;
using Newtonsoft.Json;

namespace CodeElements.UpdateSystem
{
	/// <summary>
	///     The core class that provides the methods to search for update packages
	/// </summary>
	public class UpdateController<TEnvironmentSettings> : IUpdateController where TEnvironmentSettings : IEnvironmentManager, new()
	{
	    private readonly Lazy<HttpClient> _httpClient;
	    private readonly Uri _updateSystemApiUri;

	    /// <summary>
	    /// Initialize a new instance of <see cref="UpdateController"/>
	    /// </summary>
	    /// <param name="projectGuid">The <see cref="Guid"/> of the project</param>
	    /// <param name="cleanupUtilities">Provides utilities to cleanup redundant files created by the update system</param>
	    /// <param name="httpMessageHandler"></param>
	    public UpdateController(Guid projectGuid, HttpMessageHandler httpMessageHandler) : this(projectGuid)
		{
            _httpClient = new Lazy<HttpClient>(() => new HttpClient(httpMessageHandler));
		}

	    public UpdateController(Guid projectGuid, HttpClient httpClient) : this(projectGuid)
	    {
	        _httpClient = new Lazy<HttpClient>(() => httpClient);
	    }

        public UpdateController(Guid projectGuid)
        {
            ProjectGuid = projectGuid;
            VersionProvider = new AssemblyVersionProvider();
            ChangelogLanguage = CultureInfo.CurrentUICulture;
            Settings = new TEnvironmentSettings();
            _httpClient = new Lazy<HttpClient>(() => new HttpClient());
            _updateSystemApiUri = new Uri("https://localhost:9696/u/v1/");

            Settings?.Cleanup(projectGuid);
        }

	    public Guid ProjectGuid { get; }

	    /// <summary>
		///     The public key to validate the signatures of the files
		/// </summary>
		public RSAParameters PublicKey { get; set; }

	    IEnvironmentManager IUpdateController.Environment => Settings;
	    HttpClient IUpdateController.HttpClient => _httpClient.Value;
	    Uri IUpdateController.UpdateSystemApiUri => _updateSystemApiUri;

        /// <summary>
        ///     The current version provider that is used to retrieve the application version
        /// </summary>
        public IVersionProvider VersionProvider { get; set; }

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
        /// Search for new update packages with the given options
        /// </summary>
        /// <returns>Return the result of the update search</returns>
		public async Task<UpdatePackageSearchResult> SearchForNewUpdatePackages()
		{
			var request = new SearchUpdatePackage
			{
				CurrentVersion = VersionProvider.GetVersion(),
				VersionFilter = VersionFilter?.GetSupportedPrereleases(),
				Platforms = PlatformProvider?.GetEncodedPlatforms() ?? int.MaxValue
			};

		    if (request.VersionFilter.Length > 10)
		        throw new ArgumentException("A maximum of 10 version filters is allowed.");

			var response = await _httpClient.Value.PostAsync(new Uri(_updateSystemApiUri, $"projects/{ProjectGuid:N}/check"),
				new StringContent(JsonConvert.SerializeObject(request)));
		    if (response.StatusCode != HttpStatusCode.OK)
		        throw await GetException(response);

		    var updateSearchResult =
		        JsonConvert.DeserializeObject<UpdatePackageSearchResult>(
		            await response.Content.ReadAsStringAsync());
            updateSearchResult.Initialize(this);

		    if (!updateSearchResult.IsUpdateAvailable)
		        Settings?.NoUpdatesFoundCleanup(ProjectGuid);

		    return updateSearchResult;
		}

	    internal async Task<Exception> GetException(HttpResponseMessage responseMessage)
	    {
	        Debug.Assert(responseMessage.StatusCode != HttpStatusCode.OK);
	        if (responseMessage.StatusCode == HttpStatusCode.InternalServerError)
	        {
	            var errorResponse =
	                JsonConvert.DeserializeObject<ErrorResponse>(await responseMessage.Content.ReadAsStringAsync());

	            switch ((ErrorType)Enum.Parse(typeof(ErrorType), errorResponse.Type))
	            {
	                case ErrorType.InvalidData:
	                    return new ArgumentException(errorResponse.Message);
	                case ErrorType.ServiceError:
	                    switch ((ServiceErrorId)errorResponse.Code)
	                    {
	                        case ServiceErrorId.ProjectNotFound:
	                            return new ArgumentException("The project with the given GUID was not found.");
	                        case ServiceErrorId.ProjectDisabled:
	                            return new InvalidOperationException("The project was disabled.");
                            case ServiceErrorId.ProjectExpired:
                                return new InvalidOperationException("The project subscription expired.");
                            case ServiceErrorId.LicenseNotFound:
                                return new ArgumentException("The license was not found.");
                            case ServiceErrorId.LicenseDeactivated:
                                return new InvalidOperationException("The license is deactivated.");
                            case ServiceErrorId.LicenseExpired:
                                return new InvalidOperationException("The license expired.");
                            case ServiceErrorId.MachineNotFound:
                                return new InvalidOperationException("The current computer is not activated.");
                            case ServiceErrorId.IpLimitExhausted:
                                return new InvalidOperationException("The ip address limit is exhausted.");
                            case ServiceErrorId.FileNotFound:
                                return new FileNotFoundException("The file was not found.");
                            default:
	                            return new InvalidOperationException(
	                                $"Unknown service error id received.{Environment.NewLine}Message: " +
	                                errorResponse.Message);
	                    }
	                case ErrorType.ServerError:
	                    return new InvalidOperationException(errorResponse.Message);
	                default:
	                    throw new ArgumentOutOfRangeException("Unknown error type received.");
	            }
	        }

	        return new HttpRequestException("Invalid status code received: " + responseMessage.StatusCode);
	    }

	    internal enum ServiceErrorId
	    {
	        ProjectNotFound = 1,
	        ProjectDisabled = 2,
	        ProjectExpired = 3,
	        LicenseNotFound = 4,
	        LicenseDeactivated = 5,
	        LicenseExpired = 6,
	        MachineNotFound = 7,
	        IpLimitExhausted = 8,
	        ActivationLimitExhausted = 9,
            FileNotFound = 10
	    }

	    internal enum ErrorType
	    {
	        InvalidData,
	        ServiceError,
	        ServerError
	    }

        internal class ErrorResponse
	    {
	        [JsonProperty("message")]
	        public string Message { get; set; }

	        [JsonProperty("type")]
	        public string Type { get; set; }

	        [JsonProperty("code")]
	        public int Code { get; set; }
	    }
    }
}