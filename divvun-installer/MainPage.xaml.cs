using Divvun.PkgMgr.Bahkat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Divvun.PkgMgr
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            Repository repoLoading;
            
            App.Store.Observe()
                .SubscribeOnDispatcher()
                .ObserveOnDispatcher()
                .Select(state => state.SelectedRepository)
                .DistinctUntilChanged()
                .Subscribe(repo =>
                {
                    switch (repo.State)
                    {
                        case TAsyncState.NotStarted:
                            repoLoading = new Repository(App.Store.State.RepositoryUrl);
                            App.Store.State.SelectedRepository = AsyncState<Repository, string>.Loading();

                            repoLoading.Refresh().ContinueWith(result =>
                            {
                                if (result.Exception != null) throw result.Exception;
                                App.Store.State.SelectedRepository = AsyncState<Repository, string>.Success(repoLoading);
                            });
                            break;
                        case TAsyncState.Loading:
                            break;
                        case TAsyncState.Success:
                            var packages = repo.Value.PackagesIndex.Values
                                //.Where(x => x.Os.Keys.Contains("windows"))
                                .ToList();
                            var filter = repo.Value.Meta.PrimaryFilter;
                            
                            switch (filter)
                            {
                                case "category":
                                    // throw new NotSupportedException();
                                case "language":
                                    FilterByLanguage(packages);
                                    break;
                            }
                            break;
                        case TAsyncState.Failure:
                            break;
                    }
                });

         
        }

        private void FilterByLanguage(List<PackageIndex> packages)
        {
            var languages = new Dictionary<string, List<PackageIndex>>();

            foreach (var package in packages)
            {
                foreach (var lang in package.Languages)
                {
                    if (!languages.ContainsKey(lang))
                    {
                        languages.Add(lang, new List<PackageIndex>());
                    }
                    languages[lang].Add(package);
                }
            }

            var cats = new ObservableCollection<MenuCategory>();

            foreach (var entry in languages)
            {
                var cat = new MenuCategory()
                {
                    Title = entry.Key
                };
                
                foreach (var item in entry.Value)
                {
                    cat.Items.Add(new MenuItem()
                    {
                        Title = item.Name["en"],
                        RawFileSize = 12345678,
                        Version = item.Version
                    });
                }

                cats.Add(cat);
            }

            tvPackages.ItemsSource = cats;
        }

        private void btnPrimary_Click(object sender, RoutedEventArgs e)
        {
            //this.NavigationService.Navigate(new UpdatePage());
            //this.NavigationService.RemoveBackEntry();
        }

        private void tvPackages_KeyDown(object sender, KeyEventArgs e)
        {
            if (tvPackages.SelectedItem is MenuItem)
            {
                var item = (MenuItem)tvPackages.SelectedItem;

                item.IsSelected = !item.IsSelected;
            }
        }
    }

    public class MenuCategory
    {
        public MenuCategory()
        {
            this.Items = new ObservableCollection<MenuItem>();
        }

        public string Title { get; set; }

        public ObservableCollection<MenuItem> Items { get; set; }
    }

    public class MenuItem
    {
        private static String BytesToString(UInt64 bytes)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
            if (bytes == 0)
            {
                return "0 " + suf[0];
            }
            Int32 place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            Double num = Math.Round(bytes / Math.Pow(1024, place), 2);
            return num.ToString() + " " + suf[place];
        }

        public string Title { get; set; }
        public UInt64 RawFileSize { get; set; }
        public string FileSize
        {
            get
            {
                return BytesToString(RawFileSize);
            }
        }
        public string Version { get; set; }
        public string Meta
        {
            get
            {
                return Version + " (" + FileSize + ")";
            }
        }
        public string Status
        {
            get
            {
                return "Not Installed";
            }
        }
        public bool IsSelected { get; set; }
    }
}
