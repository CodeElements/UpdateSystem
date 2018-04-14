using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using CodeElements.UpdateSystem.Core;
using CodeElements.UpdateSystem.Files;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RichardSzalay.MockHttp;
using Xunit;

namespace CodeElements.UpdateSystem.Tests.Core
{
    public abstract class UpdateDownloaderTests
    {
        protected readonly MockHttpMessageHandler MockHttp = new MockHttpMessageHandler();
        protected readonly Guid ProjectId = Guid.Parse("C17840E3-16EC-4336-859C-81CED39559B5");
        protected readonly RSA Rsa = RSA.Create(1024);
        protected readonly UpdateDownloader UpdateDownloader;

        protected UpdateDownloaderTests()
        {
            var environment = new Mock<IEnvironmentManager>();
            SetupEnvironment(environment);

            var updateController = new Mock<IUpdateController>();
            updateController.SetupGet(x => x.HttpClient)
                .Returns(new HttpClient(MockHttp) {BaseAddress = new Uri("https://www.codeelements.net/")});
            updateController.SetupGet(x => x.ProjectId).Returns(ProjectId);
            updateController.SetupGet(x => x.Environment).Returns(environment.Object);
            updateController.SetupGet(x => x.JsonSerializerSettings).Returns(
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
            updateController.SetupGet(x => x.PublicKey).Returns(Rsa.ExportParameters(false));

            var downloadable = new Mock<IDownloadable>();
            downloadable.SetupGet(x => x.UpdateController).Returns(updateController.Object);
            downloadable.SetupGet(x => x.Instructions).Returns(GetInstructions);

            UpdateDownloader = new UpdateDownloader(downloadable.Object);
        }

        protected abstract void SetupEnvironment(Mock<IEnvironmentManager> envMock);
        protected abstract UpdateInstructions GetInstructions();
    }

    public abstract class UpdateDownloaderFileTests : UpdateDownloaderTests
    {
        protected readonly List<Hash> Downloads = new List<Hash>();

        protected abstract Dictionary<string, string> ExistingFiles { get; }
        protected Dictionary<string, MockTempFile> TempFiles = new Dictionary<string, MockTempFile>();
        protected abstract Dictionary<Hash, string> OnlineFiles { get; }

        public UpdateDownloaderFileTests()
        {
            MockHttp.When("*download").Respond(HttpHandler);
        }

        private async Task<HttpResponseMessage> HttpHandler(HttpRequestMessage httpRequestMessage)
        {
            Assert.Equal(HttpMethod.Get, httpRequestMessage.Method);

            var queryParams = HttpUtility.ParseQueryString(httpRequestMessage.RequestUri.Query);
            var fileParam = queryParams["file"];
            if (!string.IsNullOrWhiteSpace(fileParam))
            {
                var fileHash = Hash.Parse(fileParam);

                if (!OnlineFiles.TryGetValue(fileHash, out var fileData))
                    return new HttpResponseMessage(HttpStatusCode.NotFound);

                Downloads.Add(fileHash);
                using(var responseStream = new MemoryStream())
                using (var gzipStream = new GZipStream(responseStream, CompressionLevel.Fastest))
                {
                    var data = Encoding.ASCII.GetBytes(fileData);
                    await gzipStream.WriteAsync(data, 0, data.Length);
                    gzipStream.Close();

                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new ByteArrayContent(responseStream.ToArray())
                    };
                }
            }

            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }

        protected override void SetupEnvironment(Mock<IEnvironmentManager> envMock)
        {
            envMock.Setup(x => x.TryOpenRead(It.IsAny<string>())).Returns<string>(GetFile);
            envMock.Setup(x => x.GetStackFile(It.IsAny<Guid>(), It.IsAny<Hash>())).Returns<Guid, Hash>(GetTempFile);
        }

        private IFileInfo GetTempFile(Guid guid, Hash hash)
        {
            Assert.Equal(ProjectId, guid);
            if (!TempFiles.TryGetValue(hash.ToString(), out var mockFile))
                TempFiles.Add(hash.ToString(), mockFile = new MockTempFile());

            return mockFile;
        }

        protected virtual Stream GetFile(string filename)
        {
            if (ExistingFiles.TryGetValue(filename, out var data))
                return new MemoryStream(Encoding.ASCII.GetBytes(data));

            return null;
        }

        protected class MockTempFile : IFileInfo
        {
            public string Filename { get; set; }
            public bool Exists { get; set; }
            public MemoryStream DataStream { get; set; }

            public Stream OpenRead()
            {
                DataStream?.Seek(0, SeekOrigin.Begin);
                return DataStream;
            }

            public Stream Create()
            {
                Exists = true;
                DataStream = new MemoryStream();
                return DataStream;
            }

            public void Delete()
            {
                DataStream = null;
                Exists = false;
            }
        }

        protected virtual Dictionary<Hash, byte[]> GenerateOnlineFileSignatures()
        {
            return OnlineFiles.ToDictionary(x => x.Key,
                x => Rsa.SignData(Encoding.ASCII.GetBytes(x.Value), HashAlgorithmName.SHA256, RSASignaturePadding.Pss));
        }
    }

    public class UpdateDownloaderCancellationTest : UpdateDownloaderTests
    {
        protected override void SetupEnvironment(Mock<IEnvironmentManager> envMock)
        {
        }

        protected override UpdateInstructions GetInstructions()
        {
            return null;
        }

        [Fact]
        public Task TestCancellationExceptionThrown()
        {
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            return Assert.ThrowsAsync<OperationCanceledException>(() =>
                UpdateDownloader.DownloadAsync(cancellationTokenSource.Token));
        }
    }

    public class UpdateDownloaderFileOperationsNullTest : UpdateDownloaderFileTests
    {
        protected override Dictionary<string, string> ExistingFiles { get; } = new Dictionary<string, string>
        {
            {"%basedir%\\existingFile.exe", "NORA"},
            {"%basedir%\\changedFile.txt", "NOT NORA"}
        };

        protected override Dictionary<Hash, string> OnlineFiles { get; } = new Dictionary<Hash, string>
        {
            {Hash.Parse("7324c19ad06ea51117d7ffdd01f7d030ac03651f6ea57472de1765b3dfa3a9e0"), "NORA2"},
            {Hash.Parse("ac6cb635c58754672d0e576442db9ecadd8043829916ca85330cf00bd7359df9"), "NORA3"}
        };

        protected override UpdateInstructions GetInstructions()
        {
            var instructions = new UpdateInstructions();
            typeof(UpdateInstructions).GetProperty(nameof(UpdateInstructions.TargetFiles))
                .SetValue(instructions, new List<FileInformation>
                {
                    new FileInformation
                    {
                        Filename = "%basedir%\\existingFile.exe",
                        Hash = Hash.Parse("322592d512976ad849b60a88dd34cf60e3db134974b09b5bf79c359f909a8c07"),
                        Length = 1000
                    },
                    new FileInformation
                    {
                        Filename = "%basedir%\\changedFile.txt",
                        Hash = Hash.Parse("7324c19ad06ea51117d7ffdd01f7d030ac03651f6ea57472de1765b3dfa3a9e0"),
                        Length = 1000
                    },
                    new FileInformation
                    {
                        Filename = "%basedir%\\nonexistingFile.exe",
                        Hash = Hash.Parse("ac6cb635c58754672d0e576442db9ecadd8043829916ca85330cf00bd7359df9"),
                        Length = 1000
                    }
                });

            typeof(UpdateInstructions).GetProperty(nameof(UpdateInstructions.FileSignatures))
                .SetValue(instructions, GenerateOnlineFileSignatures());

            return instructions;
        }

        [Fact]
        public async Task Test()
        {
            await UpdateDownloader.DownloadAsync(CancellationToken.None);

            Assert.Equal(2, Downloads.Count);
            Assert.Contains(Hash.Parse("7324c19ad06ea51117d7ffdd01f7d030ac03651f6ea57472de1765b3dfa3a9e0"), Downloads);
            Assert.Contains(Hash.Parse("ac6cb635c58754672d0e576442db9ecadd8043829916ca85330cf00bd7359df9"), Downloads);

            Assert.Equal(2, TempFiles.Count);

            Assert.True(TempFiles.TryGetValue("7324c19ad06ea51117d7ffdd01f7d030ac03651f6ea57472de1765b3dfa3a9e0",
                out var tempFile));
            Assert.Equal("NORA2", Encoding.ASCII.GetString(tempFile.DataStream.ToArray()));

            Assert.True(TempFiles.TryGetValue("ac6cb635c58754672d0e576442db9ecadd8043829916ca85330cf00bd7359df9",
                out tempFile));
            Assert.Equal("NORA3", Encoding.ASCII.GetString(tempFile.DataStream.ToArray()));
        }
    }

    public class UpdateDownloaderInvalidSignatureTest : UpdateDownloaderFileTests
    {
        protected override Dictionary<string, string> ExistingFiles { get; } = new Dictionary<string, string>
        {
            {"%basedir%\\existingFile.exe", "NORA"},
            {"%basedir%\\changedFile.txt", "NOT NORA"}
        };

        protected override Dictionary<Hash, string> OnlineFiles { get; } = new Dictionary<Hash, string>
        {
            {Hash.Parse("7324c19ad06ea51117d7ffdd01f7d030ac03651f6ea57472de1765b3dfa3a9e0"), "NORA2"},
            {Hash.Parse("ac6cb635c58754672d0e576442db9ecadd8043829916ca85330cf00bd7359df9"), "NORA3"}
        };

        protected override UpdateInstructions GetInstructions()
        {
            var instructions = new UpdateInstructions();
            typeof(UpdateInstructions).GetProperty(nameof(UpdateInstructions.TargetFiles))
                .SetValue(instructions, new List<FileInformation>
                {
                    new FileInformation
                    {
                        Filename = "%basedir%\\existingFile.exe",
                        Hash = Hash.Parse("322592d512976ad849b60a88dd34cf60e3db134974b09b5bf79c359f909a8c07"),
                        Length = 1000
                    },
                    new FileInformation
                    {
                        Filename = "%basedir%\\changedFile.txt",
                        Hash = Hash.Parse("7324c19ad06ea51117d7ffdd01f7d030ac03651f6ea57472de1765b3dfa3a9e0"),
                        Length = 1000
                    },
                    new FileInformation
                    {
                        Filename = "%basedir%\\nonexistingFile.exe",
                        Hash = Hash.Parse("ac6cb635c58754672d0e576442db9ecadd8043829916ca85330cf00bd7359df9"),
                        Length = 1000
                    }
                });

            typeof(UpdateInstructions).GetProperty(nameof(UpdateInstructions.FileSignatures))
                .SetValue(instructions, GenerateOnlineFileSignatures());

            return instructions;
        }

        protected override Dictionary<Hash, byte[]> GenerateOnlineFileSignatures()
        {
            var signatures = base.GenerateOnlineFileSignatures();
            signatures[Hash.Parse("7324c19ad06ea51117d7ffdd01f7d030ac03651f6ea57472de1765b3dfa3a9e0")][46] = 0xFF;
            return signatures;
        }

        [Fact]
        public Task Test()
        {
            return Assert.ThrowsAsync<InvalidOperationException>(() =>
                UpdateDownloader.DownloadAsync(CancellationToken.None));
        }
    }

    public class UpdateDownloaderWrongDownloadDataTest : UpdateDownloaderFileTests
    {
        protected override Dictionary<string, string> ExistingFiles { get; } = new Dictionary<string, string>
        {
            {"%basedir%\\existingFile.exe", "NORA"},
            {"%basedir%\\changedFile.txt", "NOT NORA"}
        };

        protected override Dictionary<Hash, string> OnlineFiles { get; } = new Dictionary<Hash, string>
        {
            {Hash.Parse("7324c19ad06ea51117d7ffdd01f7d030ac03651f6ea57472de1765b3dfa3a9e0"), "Malicious File"},
            {Hash.Parse("ac6cb635c58754672d0e576442db9ecadd8043829916ca85330cf00bd7359df9"), "NORA3"}
        };

        protected override UpdateInstructions GetInstructions()
        {
            var instructions = new UpdateInstructions();
            typeof(UpdateInstructions).GetProperty(nameof(UpdateInstructions.TargetFiles))
                .SetValue(instructions, new List<FileInformation>
                {
                    new FileInformation
                    {
                        Filename = "%basedir%\\existingFile.exe",
                        Hash = Hash.Parse("322592d512976ad849b60a88dd34cf60e3db134974b09b5bf79c359f909a8c07"),
                        Length = 1000
                    },
                    new FileInformation
                    {
                        Filename = "%basedir%\\changedFile.txt",
                        Hash = Hash.Parse("7324c19ad06ea51117d7ffdd01f7d030ac03651f6ea57472de1765b3dfa3a9e0"),
                        Length = 1000
                    },
                    new FileInformation
                    {
                        Filename = "%basedir%\\nonexistingFile.exe",
                        Hash = Hash.Parse("ac6cb635c58754672d0e576442db9ecadd8043829916ca85330cf00bd7359df9"),
                        Length = 1000
                    }
                });

            typeof(UpdateInstructions).GetProperty(nameof(UpdateInstructions.FileSignatures))
                .SetValue(instructions, GenerateOnlineFileSignatures());

            return instructions;
        }

        [Fact]
        public Task Test()
        {
            return Assert.ThrowsAsync<InvalidOperationException>(() =>
                UpdateDownloader.DownloadAsync(CancellationToken.None));
        }
    }

    public class UpdateDownloaderExistingTempFilesTest : UpdateDownloaderFileTests
    {
        public UpdateDownloaderExistingTempFilesTest()
        {
            TempFiles.Add("7324c19ad06ea51117d7ffdd01f7d030ac03651f6ea57472de1765b3dfa3a9e0",
                new MockTempFile {Exists = true, DataStream = new MemoryStream(Encoding.ASCII.GetBytes("NORA2"))});
        }

        protected override Dictionary<string, string> ExistingFiles { get; } = new Dictionary<string, string>
        {
            {"%basedir%\\existingFile.exe", "NORA"},
            {"%basedir%\\changedFile.txt", "NOT NORA"}
        };

        protected override Dictionary<Hash, string> OnlineFiles { get; } = new Dictionary<Hash, string>
        {
            {Hash.Parse("7324c19ad06ea51117d7ffdd01f7d030ac03651f6ea57472de1765b3dfa3a9e0"), "NORA2"},
            {Hash.Parse("ac6cb635c58754672d0e576442db9ecadd8043829916ca85330cf00bd7359df9"), "NORA3"}
        };

        protected override UpdateInstructions GetInstructions()
        {
            var instructions = new UpdateInstructions();
            typeof(UpdateInstructions).GetProperty(nameof(UpdateInstructions.TargetFiles))
                .SetValue(instructions, new List<FileInformation>
                {
                    new FileInformation
                    {
                        Filename = "%basedir%\\existingFile.exe",
                        Hash = Hash.Parse("322592d512976ad849b60a88dd34cf60e3db134974b09b5bf79c359f909a8c07"),
                        Length = 1000
                    },
                    new FileInformation
                    {
                        Filename = "%basedir%\\changedFile.txt",
                        Hash = Hash.Parse("7324c19ad06ea51117d7ffdd01f7d030ac03651f6ea57472de1765b3dfa3a9e0"),
                        Length = 1000
                    },
                    new FileInformation
                    {
                        Filename = "%basedir%\\nonexistingFile.exe",
                        Hash = Hash.Parse("ac6cb635c58754672d0e576442db9ecadd8043829916ca85330cf00bd7359df9"),
                        Length = 1000
                    }
                });

            typeof(UpdateInstructions).GetProperty(nameof(UpdateInstructions.FileSignatures))
                .SetValue(instructions, GenerateOnlineFileSignatures());

            return instructions;
        }

        [Fact]
        public async Task Test()
        {
            await UpdateDownloader.DownloadAsync(CancellationToken.None);

            Assert.Single(Downloads);
            Assert.Contains(Hash.Parse("ac6cb635c58754672d0e576442db9ecadd8043829916ca85330cf00bd7359df9"), Downloads);

            Assert.Equal(2, TempFiles.Count);

            Assert.True(TempFiles.TryGetValue("7324c19ad06ea51117d7ffdd01f7d030ac03651f6ea57472de1765b3dfa3a9e0",
                out var tempFile));
            Assert.Equal("NORA2", Encoding.ASCII.GetString(tempFile.DataStream.ToArray()));

            Assert.True(TempFiles.TryGetValue("ac6cb635c58754672d0e576442db9ecadd8043829916ca85330cf00bd7359df9",
                out tempFile));
            Assert.Equal("NORA3", Encoding.ASCII.GetString(tempFile.DataStream.ToArray()));
        }
    }

    public class UpdateDownloaderMaliciousExistingTempFilesTest : UpdateDownloaderFileTests
    {
        public UpdateDownloaderMaliciousExistingTempFilesTest()
        {
            TempFiles.Add("7324c19ad06ea51117d7ffdd01f7d030ac03651f6ea57472de1765b3dfa3a9e0",
                new MockTempFile { Exists = true, DataStream = new MemoryStream(Encoding.ASCII.GetBytes("NORA??")) });
        }

        protected override Dictionary<string, string> ExistingFiles { get; } = new Dictionary<string, string>
        {
            {"%basedir%\\existingFile.exe", "NORA"},
            {"%basedir%\\changedFile.txt", "NOT NORA"}
        };

        protected override Dictionary<Hash, string> OnlineFiles { get; } = new Dictionary<Hash, string>
        {
            {Hash.Parse("7324c19ad06ea51117d7ffdd01f7d030ac03651f6ea57472de1765b3dfa3a9e0"), "NORA2"},
            {Hash.Parse("ac6cb635c58754672d0e576442db9ecadd8043829916ca85330cf00bd7359df9"), "NORA3"}
        };

        protected override UpdateInstructions GetInstructions()
        {
            var instructions = new UpdateInstructions();
            typeof(UpdateInstructions).GetProperty(nameof(UpdateInstructions.TargetFiles))
                .SetValue(instructions, new List<FileInformation>
                {
                    new FileInformation
                    {
                        Filename = "%basedir%\\existingFile.exe",
                        Hash = Hash.Parse("322592d512976ad849b60a88dd34cf60e3db134974b09b5bf79c359f909a8c07"),
                        Length = 1000
                    },
                    new FileInformation
                    {
                        Filename = "%basedir%\\changedFile.txt",
                        Hash = Hash.Parse("7324c19ad06ea51117d7ffdd01f7d030ac03651f6ea57472de1765b3dfa3a9e0"),
                        Length = 1000
                    },
                    new FileInformation
                    {
                        Filename = "%basedir%\\nonexistingFile.exe",
                        Hash = Hash.Parse("ac6cb635c58754672d0e576442db9ecadd8043829916ca85330cf00bd7359df9"),
                        Length = 1000
                    }
                });

            typeof(UpdateInstructions).GetProperty(nameof(UpdateInstructions.FileSignatures))
                .SetValue(instructions, GenerateOnlineFileSignatures());

            return instructions;
        }

        [Fact]
        public async Task Test()
        {
            await UpdateDownloader.DownloadAsync(CancellationToken.None);

            Assert.Equal(2, Downloads.Count);
            Assert.Contains(Hash.Parse("7324c19ad06ea51117d7ffdd01f7d030ac03651f6ea57472de1765b3dfa3a9e0"), Downloads);
            Assert.Contains(Hash.Parse("ac6cb635c58754672d0e576442db9ecadd8043829916ca85330cf00bd7359df9"), Downloads);

            Assert.Equal(2, TempFiles.Count);

            Assert.True(TempFiles.TryGetValue("7324c19ad06ea51117d7ffdd01f7d030ac03651f6ea57472de1765b3dfa3a9e0",
                out var tempFile));
            Assert.Equal("NORA2", Encoding.ASCII.GetString(tempFile.DataStream.ToArray()));

            Assert.True(TempFiles.TryGetValue("ac6cb635c58754672d0e576442db9ecadd8043829916ca85330cf00bd7359df9",
                out tempFile));
            Assert.Equal("NORA3", Encoding.ASCII.GetString(tempFile.DataStream.ToArray()));
        }
    }
}