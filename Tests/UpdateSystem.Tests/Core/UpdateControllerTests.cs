using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using CodeElements.UpdateSystem.Client;
using CodeElements.UpdateSystem.Core;
using CodeElements.UpdateSystem.Core.Internal;
using Moq;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using Xunit;

namespace CodeElements.UpdateSystem.Tests.Core
{
    public class UpdateControllerTests
    {
        private readonly Guid _projectId = Guid.Parse("29DF3788-9C4F-4C5D-BC15-9C48A2BB821E");
        private readonly MockHttpMessageHandler _mockHttp = new MockHttpMessageHandler();
        private readonly UpdateController<DummyEnvironmentManager> _updateController;

        public UpdateControllerTests()
        {
            _updateController = new UpdateController<DummyEnvironmentManager>(_projectId, _mockHttp);
            Assert.True(_updateController.Settings.CleanupExecuted);
        }
        
        [Fact]
        public async Task TestSearchNewUpdatePackackgesNoUpdates()
        {
            _updateController.VersionProvider = new CustomVersionProvider("0.1.4");
            _updateController.ChangelogLanguage = CultureInfo.GetCultureInfo("fr");

            _mockHttp.Expect(HttpMethod.Get, "*packages/0.1.4/check").WithHeaders("Accept-Language", "fr")
                .Respond("application/json",
                    JsonConvert.SerializeObject(
                        new JwtResponse<UpdatePackageSearchResult> {Result = new UpdatePackageSearchResult()}));
            var result = await _updateController.SearchForNewUpdatePackagesAsync();
            Assert.True(_updateController.Settings.NoUpdatesFoundCleanupExecuted);
            Assert.False(result.IsUpdateAvailable);
        }

        [Fact]
        public async Task TestSearchNewUpdatePackackgesWithVersionFilterNoUpdates()
        {
            _updateController.VersionProvider = new CustomVersionProvider("0.1.4");
            _updateController.ChangelogLanguage = CultureInfo.GetCultureInfo("fr");
            _updateController.VersionFilter = new DefaultVersionFilter {IncludeAlpha = true, IncludeBeta = true};

            _mockHttp.Expect(HttpMethod.Get, "*packages/0.1.4/check").WithHeaders("Accept-Language", "fr")
                .WithQueryString("versionFilter=[\"beta\"%2C\"alpha\"]")
                .Respond("application/json",
                    JsonConvert.SerializeObject(
                        new JwtResponse<UpdatePackageSearchResult> { Result = new UpdatePackageSearchResult() }));
            var result = await _updateController.SearchForNewUpdatePackagesAsync();
            Assert.True(_updateController.Settings.NoUpdatesFoundCleanupExecuted);
            Assert.False(result.IsUpdateAvailable);
        }

        [Fact]
        public async Task TestSearchNewUpdatePackackgesWithPlatformsNoUpdates()
        {
            _updateController.VersionProvider = new CustomVersionProvider("0.1.4");
            _updateController.ChangelogLanguage = CultureInfo.GetCultureInfo("fr");

            var platformProviderMock = new Mock<IPlatformProvider>();
            platformProviderMock.Setup(x => x.GetEncodedPlatforms()).Returns(342);
            _updateController.PlatformProvider = platformProviderMock.Object;

            _mockHttp.Expect(HttpMethod.Get, "*packages/0.1.4/check").WithHeaders("Accept-Language", "fr")
                .WithQueryString("platforms=342")
                .Respond("application/json",
                    JsonConvert.SerializeObject(
                        new JwtResponse<UpdatePackageSearchResult> { Result = new UpdatePackageSearchResult() }));
            var result = await _updateController.SearchForNewUpdatePackagesAsync();
            Assert.True(_updateController.Settings.NoUpdatesFoundCleanupExecuted);
            Assert.False(result.IsUpdateAvailable);
        }

        [Fact]
        public async Task TestSearchNewUpdatePackackgesUpdatesFound()
        {
            _updateController.VersionProvider = new CustomVersionProvider("0.1.4");
            _updateController.ChangelogLanguage = CultureInfo.GetCultureInfo("fr");

            var searchResult = new UpdatePackageSearchResult();
            typeof(UpdatePackageSearchResult).GetProperty(nameof(searchResult.UpdatePackages))
                .SetValue(searchResult, new List<UpdatePackageInfo> {new UpdatePackageInfo {Version = "1.0"}});

            _mockHttp.Expect(HttpMethod.Get, "*packages/0.1.4/check").WithHeaders("Accept-Language", "fr")
                .Respond("application/json",
                    JsonConvert.SerializeObject(new JwtResponse<UpdatePackageSearchResult> {Result = searchResult}));

            var result = await _updateController.SearchForNewUpdatePackagesAsync();
            Assert.False(_updateController.Settings.NoUpdatesFoundCleanupExecuted);
            Assert.True(result.IsUpdateAvailable);
            Assert.Equal("1.0", result.TargetPackage.Version);
        }

        private class DummyEnvironmentManager : IEnvironmentManager
        {
            public bool CleanupExecuted { get; private set; }
            public bool NoUpdatesFoundCleanupExecuted { get; private set; }

            public void Cleanup(Guid projectGuid)
            {
                CleanupExecuted = true;
            }

            public void NoUpdatesFoundCleanup(Guid projectGuid)
            {
                NoUpdatesFoundCleanupExecuted = true;
            }

            public Stream TryOpenRead(string filename)
            {
                throw new NotImplementedException();
            }

            public IFileInfo GetStackFile(Guid projectId, Hash hash)
            {
                throw new NotImplementedException();
            }

            public IFileInfo GetDeltaStackFile(Guid projectId, int patchId)
            {
                throw new NotImplementedException();
            }

            public IFileInfo GetRandomFile(Guid projectId)
            {
                throw new NotImplementedException();
            }

            public void MoveToStackFiles(Guid projectId, IFileInfo sourceFile, Hash hash)
            {
                throw new NotImplementedException();
            }

            public void ExecuteUpdater(PatcherConfig patcherConfig)
            {
                throw new NotImplementedException();
            }
        }
    }
}
