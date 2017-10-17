using JsonLD.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Divvun.PkgMgr.Models
{
    public class CryptographicSignature
    {
        public Uri PublicKey { get; set; }
        public string SignatureMethod { get; set; }
        public string Base64 { get; set; }
    }

    public class SoftwarePackage
    {
        public Uri Id { get; set; }
        public string InLanguage { get; set; }
        public string ApplicationCategory { get; set; }
        public Dictionary<string, string> Name { get; set; }
        public Dictionary<string, string> Description { get; set; }
        public string OperatingSystem { get; set; }
        public double OperatingSystemMinimumVersion { get; set; }
        public Dictionary<string, Uri> ReleaseNotes { get; set; }
        public Uri DownloadUrl { get; set; }
        public string FileFormat { get; set; }
        public string FileSize { get; set; }
        public long ExactFileSize { get; set; }
        public DateTime DateCreated { get; set; }
        public string ApplicationVersion { get; set; }
        public CryptographicSignature CryptographicSignature { get; set; }
        public Guid InstallationGuid { get; set; }
        public string SilentInstallCommand { get; set; }
    }

    [TestFixture]
    public class Tests
    {
        [Test]
        public void TestCultureMap()
        {
            var json = @"
            {
              '@context': [
                'http://schema.org',
                {
                  'bahkat': 'http://bahkat.org/',
                  'SoftwarePackage': {
                    '@id': 'bahkat:SoftwarePackage',
                    '@type': 'schema:SoftwareApplication',
                    'name': 'SoftwarePackage',
                    'description': 'A software application package.'
                  },
                  'WindowsSoftwarePackage': {
                    '@id': 'bahkat:WindowsSoftwarePackage',
                    '@type': 'bahkat:SoftwarePackage',
                    'name': 'WindowsSoftwarePackage',
                    'description': 'A software application package for Windows operating systems.'
                  },
                  'name': {
                    '@id': 'schema:name',
                    '@container': '@language'
                  },
                  'description': {
                    '@id': 'schema:description',
                    '@container': '@language'
                  },
                  'releaseNotes': {
                    '@id': 'schema:releaseNotes',
                    '@container': '@language'
                  },
                  'exactFileSize': {
                    '@id': 'bahkat:exactFileSize',
                    '@type': 'xsd:integer',
                    '@value': '@id'
                  },
                  'fileFormat': {
                    '@value': '@id'
                  },
                  'applicationCategory': {
                    '@value': '@id'
                  },
                  'dateCreated': {
                    '@type': 'schema:DateTime'
                  },
                  'operatingSystemMinimumVersion': {
                    '@id': 'bahkat:minimumVersion',
                    '@type': 'xsd:double',
                    'schema:domainIncludes': 'bakhat:SoftwarePackage'
                  },
                  'operatingSystemMaximumVersion': {
                    '@id': 'bahkat:maximumVersion',
                    '@type': 'xsd:double',
                    'schema:domainIncludes': 'bakhat:SoftwarePackage'
                  },
                  'cryptographicSignature': {
                    '@id': 'bahkat:cryptographicSignature',
                    '@type': 'schema:Property',
                    'schema:domainIncludes': 'bakhat:SoftwarePackage',
                    'schema:rangeIncludes': 'bahkat:CryptographicSignature'
                  },
                  'Base64': {
                    '@id': 'bahkat:Base64',
                    '@type': 'schema:Text',
                    'name': 'Base64',
                    'description': 'Base64-encoded binary data.'
                  },
                  'CryptographicSignature': {
                    '@id': 'bahkat:CryptographicSignature',
                    '@type': 'schema:Intangible',
                    'name': 'CryptographicSignature'
                  },
                  'base64': {
                    '@id': 'bahkat:base64',
                    '@type': 'bahkat:Base64',
                    'name': 'base64',
                    'schema:domainIncludes': 'bahkat:CryptographicSignature',
                    'schema:rangeIncludes': 'bahkat:Base64'
                  },
                  'signatureMethod': {
                    '@id': 'bahkat:signatureMethod',
                    '@value': '@id',
                    'name': 'signatureMethod',
                    'description': 'Cryptographic method used for signature verification',
                    'schema:domainIncludes': 'bahkat:CryptographicSignature'
                  },
                  'publicKey': {
                    '@id': 'bahkat:publicKey',
                    '@type': '@id',
                    'name': 'publicKey',
                    'description': 'URL to public key to verify signature.'
                  },
                  'silentInstallCommand': {
                    '@id': 'bahkat:installCommand',
                    '@value': '@id',
                    'name': 'installCommand',
                    'description': 'The command required to install this package silently.'
                  },
                  'Guid': {
                    '@id': 'bahkat:Guid',
                    '@type': 'schema:Text',
                    'name': 'Guid',
                    'description': 'A globally-unique identifier, e.g. {27DBA885-6212-58E8-82FF-95CF3DFDE154}.'
                  },
                  'installationGuid': {
                    '@id': 'bahkat:installationGuid',
                    '@type': 'bahkat:Guid',
                    'name': 'installationGuid',
                    'description': 'The GUID referring to the installation key of this package in the Windows registry.'
                  },
                  'installedRegistryValue': {
                    '@id': 'bahkat:installedRegistryValue',
                    '@type': 'bahkat:RegistryValue',
                    'name': 'installedRegistryValue',
                    'description': 'The registry value that indicates whether or not this application is currently installed.'
                  },
                  'RegistryValue': {
                    '@id': 'bahkat:RegistryValue',
                    '@type': 'schema:StructuredValue',
                    'name': 'RegistryValue',
                    'description': 'A value within a registry key.'
                  },
                  'registryPath': {
                    '@id': 'bahkat:registryPath',
                    '@value': '@id'
                  },
                  'value': {
                    '@id': 'bahkat:value',
                    '@value': '@id'
                  }
                }
              ],
              'type': 'WindowsSoftwarePackage',
              'id': 'https://gtsvn.uit.no/divvun/voro-keyboard',
              'inLanguage': 'vro',
              'applicationCategory': 'Keyboard Layout',
              'name': {
                'en': 'Võro Kiil Keyboard Layout',
                'nb': 'Võro Kiil Tastaturer',
                'vro': 'Võro Kiil Keyboard (vro)'
              },
              'description': {
                'en': 'A keyboard for the Võro people',
                'nb': 'Voro shit m8'
              },
              'operatingSystem': 'Windows',
              'operatingSystemMinimumVersion': 8.1,
              'releaseNotes': {
                'en': 'https://pkg.example/release-notes/en.index'
              },
              'downloadUrl': 'https://pkg.example/pkg.tar.xz',
              'fileFormat': 'application/x-xz',
              'fileSize': '2.45MB',
              'exactFileSize': 2450000,
              'dateCreated': '2017-01-01T00:00:23Z',
              'applicationVersion': '0.1.0',
              'cryptographicSignature': {
                'publicKey': 'https://pkg.example/pubkey.gpg',
                'signatureMethod': 'sha256',
                'base64': 'a8hfaeHaga22=='
              },
              'installationGuid': '{27DBA885-6212-58E8-82FF-95CF3DFDE154}',
              'silentInstallCommand': '/VERYSILENT',
              'installedRegistryValue': {
                'registryPath': 'HKCU\\Software\\PkgMgr',
                'name': 'IsInstalled',
                'value': 1
              }
            }";

            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            var thing = JsonConvert.DeserializeObject<SoftwarePackage>(json, serializerSettings);

            Console.WriteLine(JsonConvert.SerializeObject(thing));
        }
    }
}
