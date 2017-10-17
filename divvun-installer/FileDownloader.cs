using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Divvun.PkgMgr
{
    [TestFixture]
    public class Tests
    {
        //[Test]
        public void TestDownload()
        {
            var uri = new Uri("https://test.example/file");
            var pubKey = new byte[] { 0 };
            var sig = "abcde==";

            var d = EcdsaSha256FileDownloader.Download(uri, pubKey, sig, (obj, e) =>
            {
                Console.WriteLine(e.ProgressPercentage);
            }).Subscribe((filePath) =>
            {
                // Do something with the verified and downloaded file.
            });
        }

        [Test]
        public void TestKey()
        {
            var d = new ECDsaCng();
            d.HashAlgorithm = CngAlgorithm.Sha256;
            Console.WriteLine(d.Key.Export(CngKeyBlobFormat.EccPublicBlob));
            
            X509Certificate2 cert = new X509Certificate2();
            ECDsaCertificateExtensions.GetECDsaPublicKey(cert);
        }
    }

    public class EcdsaSha256FileDownloader
    {
        private Uri uri;
        private CngKey publicKey;
        private byte[] signature;

        static public IObservable<string> Download(Uri uri, byte[] publicKey, string signature, DownloadProgressChangedEventHandler onProgress)
        {
           return new EcdsaSha256FileDownloader(uri, publicKey, signature).Download(onProgress);
        }

        private EcdsaSha256FileDownloader(Uri uri, byte[] publicKey, string signature)
        {
            this.uri = uri;
            this.publicKey = CngKey.Import(publicKey, CngKeyBlobFormat.EccPublicBlob);
            this.signature = Convert.FromBase64String(signature);
        }

        private IObservable<string> Download(DownloadProgressChangedEventHandler onProgress)
        {
            using (var wc = new WebClient())
            {
                var localTmpFile = Path.GetTempFileName();
                
                wc.DownloadProgressChanged += onProgress;
                wc.DownloadFileAsync(uri, localTmpFile);

                return Observable.FromEventPattern<AsyncCompletedEventHandler, AsyncCompletedEventArgs>(
                    h => wc.DownloadFileCompleted += h,
                    h => wc.DownloadFileCompleted -= h)
                    .SingleAsync()
                    .Select(x => Verify(localTmpFile));
            }
        }

        private string Verify(string path)
        {
            using (ECDsaCng ecdsa = new ECDsaCng(publicKey))
            {
                using (var file = MemoryMappedFile.CreateFromFile(path))
                {
                    if (!ecdsa.VerifyData(file.CreateViewStream(), signature, HashAlgorithmName.SHA256))
                    {
                        throw new Exception("File failed to verify.");
                    }
                }
            }

            return path;
        }
    }
}
