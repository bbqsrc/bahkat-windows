using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Divvun.PkgMgr.Bahkat
{
    public partial class PackageIndex
    {
        private string CachedInstallerPath
        {
            get
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(appData, Properties.Settings.Default.RegistryId);
            }
        }
        public async Task<string> DownloadInstaller()
        {
            // Use package id as the cache key
            // So caches\packages\<base url from repo>\<channel>\<package-id>\<version>_<filename>
            return null;
        }


    }
}
