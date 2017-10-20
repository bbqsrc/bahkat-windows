using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Divvun.PkgMgr.Bahkat
{
    class Helpers
    {
        struct Progress
        {
            internal long TotalBytes { get; set; }
            internal long Current { get; set; }
        }

        internal static async Task<T> DownloadAsync<T>(Uri uri, DownloadProgressChangedEventHandler onProgress)
        {
            using (var client = new WebClient())
            {
                if (onProgress != null)
                {
                    client.DownloadProgressChanged += onProgress;
                }
          
                var jsonString = await client.DownloadStringTaskAsync(uri);

                var serializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                Console.WriteLine(jsonString);
                return JsonConvert.DeserializeObject<T>(jsonString, serializerSettings);
            }
        }
    }

    public struct RepoIndex
    {
        public Uri Base { get; set; }
        public Dictionary<string, string> Name { get; set; }
        public Dictionary<string, string> Description { get; set; }
        public string PrimaryFilter { get; set; }
        public List<string> Channels { get; set; }
    }

    public struct PackageIndexInstallerSignature
    {

    }

    public struct PackageIndexInstaller
    {
        public Uri Url;
        public string SilentArgs;
        public string Guid;
        public ulong Size;
        public ulong InstalledSize;
        public PackageIndexInstallerSignature? Signature;
    }

    public partial class PackageIndex
    {
        public string Id;
        public Dictionary<string, string> Name;
        public Dictionary<string, string> Description;
        public string Version;
        public string Category;
        public List<string> Languages;
        public Dictionary<string, string> Os;
        public Dictionary<string, string> Dependencies;
        public Dictionary<string, string> VirtualDependencies;
        public PackageIndexInstaller? Installer;
    }

    public struct VirtualIndexTarget
    {

    }

    public struct VirtualIndex
    {
        public string Id;
        public Dictionary<string, string> Name;
        public Dictionary<string, string> Description;
        public string Version;
        public Uri Url;
        public VirtualIndexTarget Target;
    }

    public class Repository
    {
        private Uri repoUri;

        public RepoIndex Meta { get; private set; }
        public Dictionary<string, PackageIndex> PackagesIndex { get; private set; }
        public Dictionary<string, List<string>> VirtualsIndex { get; private set; }

        public Repository(Uri uri)
        {
            repoUri = uri;
        }

        public Repository(string uri)
        {
            repoUri = new Uri(uri);
        }

        public async Task Refresh()
        {
            Meta = await Helpers.DownloadAsync<RepoIndex>(
                new Uri(repoUri, "index.json"), null);
            PackagesIndex = await Helpers.DownloadAsync<Dictionary<string, PackageIndex>>(
                new Uri(repoUri, "packages/index.json"), null);
            VirtualsIndex = await Helpers.DownloadAsync<Dictionary<string, List<string>>>(
                new Uri(repoUri, "virtuals/index.json"), null);
        }

        public async Task<VirtualIndex> DownloadVirtualIndex(string virtualName, string virtualVersion)
        {
            return await Helpers.DownloadAsync<VirtualIndex>(new Uri(repoUri,
                "virtuals/" + virtualName + "/" + virtualVersion + "/index.json"), null);
        }
    }

    [TestFixture]
    public class Tests
    {
        [Test]
        public void TestThing()
        {
            var repo = new Repository("http://192.168.0.17:8000/");
            repo.Refresh().ContinueWith(_ =>
            {
                return repo.DownloadVirtualIndex("msoffice", "16.0.0");
            }).ContinueWith(index =>
            {
                Console.WriteLine(JsonConvert.SerializeObject(index));
            })
            .Wait();
        }
    }
}
