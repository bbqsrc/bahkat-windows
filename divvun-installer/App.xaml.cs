using Castle.DynamicProxy;
using Divvun.PkgMgr.Bahkat;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;

namespace Divvun.PkgMgr
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static internal RegistryKey RegKey =
            Registry.LocalMachine.CreateSubKey(@"SOFTWARE\" + PkgMgr.Properties.Settings.Default.RegistryId);

        static internal string RepositoryKey = "RepositoryUrl";
        static internal string UpdateCheckIntervalKey = "UpdateCheckInterval";

        public static ObservableStore<State> Store = new ObservableStore<State>(new State());

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            Store.Observe().Select(state => state.RepositoryUrl)
                .DistinctUntilChanged()
                .Subscribe(url =>
                {
                    RegKey.SetValue(RepositoryKey, url);
                });

            Store.Observe().Select(state => state.UpdateCheckInterval)
                .DistinctUntilChanged()
                .Subscribe(interval =>
                {
                    RegKey.SetValue(UpdateCheckIntervalKey, interval);
                });

            Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(Store.State.SelectedPackages, "CollectionChanged")
                .Subscribe(_ => Store.ForceRefresh());
        }
    }

    public enum PeriodInterval
    {
        Never,
        Daily,
        Weekly,
        Fortnightly,
        Monthly
    }

    public class State
    {
        public virtual string RepositoryUrl { get; set; }
        public virtual PeriodInterval UpdateCheckInterval { get; set; }
        public virtual ObservableCollection<PackageIndex> SelectedPackages { get; set; }
        public virtual AsyncState<Repository, string> SelectedRepository { get; set; }

        private T InitOrDefault<T>(string key, T fallback)
        {
            var thing = App.RegKey.GetValue(key);
            if (thing is T)
            {
                return (T)thing;
            }

            return fallback;
        }

        public State()
        {
            RepositoryUrl = InitOrDefault(App.RepositoryKey, Properties.Settings.Default.Repository);
            UpdateCheckInterval = (PeriodInterval)Enum.Parse(typeof(PeriodInterval),
                InitOrDefault(App.UpdateCheckIntervalKey, Properties.Settings.Default.UpdateCheckInterval));
            SelectedPackages = new ObservableCollection<PackageIndex>();
            SelectedRepository = AsyncState<Repository, string>.NotStarted();
        }
    }

    public class StateInterceptor<T> : IInterceptor
    {
        private BehaviorSubject<T> subject;

        public StateInterceptor(BehaviorSubject<T> subject)
        {
            this.subject = subject;
        }

        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();

            if (invocation.Method.Name.StartsWith("set_", StringComparison.OrdinalIgnoreCase))
            {
                subject.OnNext((T)invocation.Proxy);
            }
        }
    }

    public class ObservableStore<TState> where TState : class
    {
        public TState State { get; private set; }
        private BehaviorSubject<TState> subject = new BehaviorSubject<TState>(null);

        public ObservableStore(TState state)
        {
            var generator = new ProxyGenerator();
            var proxy = generator.CreateClassProxyWithTarget(state, new StateInterceptor<TState>(subject));
            subject.OnNext(proxy);
            State = proxy;
        }

        public IObservable<TState> Observe()
        {
            return subject;
        }

        public void ForceRefresh()
        {
            subject.OnNext(State);
        }

        public void Register<T>(IObservable<T> obj)
        {
            obj.Subscribe(_ => subject.OnNext(State));
        }
    }

    public enum TAsyncState
    {
        NotStarted,
        Loading,
        Success,
        Failure
    }

    public class AsyncState<T, E>
    {
        public readonly TAsyncState State;
        public readonly T Value;
        public readonly E Error;

        public static AsyncState<T, E> NotStarted()
        {
            return new AsyncState<T, E>(TAsyncState.NotStarted);
        }

        public static AsyncState<T, E> Loading()
        {
            return new AsyncState<T, E>(TAsyncState.Loading);
        }

        public static AsyncState<T, E> Success(T value)
        {
            return new AsyncState<T, E>(TAsyncState.Success, value);
        }

        public static AsyncState<T, E> Failure(E error)
        {
            return new AsyncState<T, E>(TAsyncState.Failure, error);
        }

        private AsyncState(TAsyncState state)
        {
            State = state;
        }

        private AsyncState(TAsyncState state, T value)
        {
            State = state;
            Value = value;
        }

        private AsyncState(TAsyncState state, E error)
        {
            State = state;
            Error = error;
        }
    }
}
