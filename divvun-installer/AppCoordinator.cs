using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Divvun.PkgMgr
{
    //public class RegValueSubject<T>
    //{
    //    private BehaviorSubject<T> subject;
    //    private RegistryKey regKey;
    //    private string valueKey;

    //    public RegValueSubject(RegistryKey rk, string vk, T defaultValue)
    //    {
    //        regKey = rk;
    //        valueKey = vk;
    //        var val = (T)regKey.GetValue(valueKey);
    //        subject = new BehaviorSubject<T>(val == null ? defaultValue : val);
    //    }

    //    public T Value
    //    {
    //        set
    //        {
    //            try
    //            {
    //                regKey.SetValue(valueKey, value);
    //                subject.OnNext(value);
    //            }
    //            catch (Exception ex)
    //            {
    //                subject.OnError(ex);
    //            }
    //        }

    //        get
    //        {
    //            return (T)regKey.GetValue(valueKey);
    //        }
    //    }

    //    public IObservable<T> AsObservable() {
    //        return subject.AsObservable();
    //    }
    //}

    
    
    public class AppCoordinator
    {
        //static internal RegistryKey RegKey =
        //    Registry.LocalMachine.CreateSubKey(@"SOFTWARE\" + Properties.Settings.Default.RegistryId);

        //static internal string RepositoryKey = "RepositoryUrl";
        //static internal string UpdateCheckIntervalKey = "UpdateCheckInterval";

        //private RegValueSubject<string> RepositoryUrlSubject =
        //    new RegValueSubject<string>(RegKey, RepositoryKey, Properties.Settings.Default.Repository);
        //private RegValueSubject<string> UpdateCheckIntervalSubject =
        //    new RegValueSubject<string>(RegKey, UpdateCheckIntervalKey, Properties.Settings.Default.UpdateCheckInterval);

        //public IObservable<PeriodInterval> UpdateCheckInterval
        //{
        //    get
        //    {
        //        return UpdateCheckIntervalSubject
        //            .AsObservable()
        //            .DistinctUntilChanged()
        //            .Select(x => (PeriodInterval)Enum.Parse(typeof(PeriodInterval), x));
        //    }
        //}
    }
}
