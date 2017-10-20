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
using System.Globalization;

namespace Divvun.PkgMgr
{
    abstract class ModelFilter : IValueConverter
    {
        abstract protected object ProcessModel(PackageIndex model);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (DependencyProperty.UnsetValue == value)
            {
                return value;
            }
            return ProcessModel((PackageIndex)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class CategoryFilter : ModelFilter
    {
        override protected object ProcessModel(PackageIndex model)
        {
            return model.Category;
        }
    }

    class LanguageFilter : ModelFilter
    {
        override protected object ProcessModel(PackageIndex model)
        {
            return model.Languages
                .Select(tag => new CultureInfo(tag).DisplayName)
                .ToList();
        }
    }

    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            BindPrimaryButton();
            BindPackageList();
        }

        private void BindPrimaryButton()
        {
            App.Store.Observe()
                .SubscribeOnDispatcher()
                .ObserveOnDispatcher()
                .Select(state => state.SelectedPackages)
                .Subscribe(packages =>
                {
                    if (packages.Count > 0)
                    {
                        btnPrimary.IsEnabled = true;
                        btnPrimary.Content = "Install " + packages.Count + " Packages";
                    }
                    else
                    {
                        btnPrimary.IsEnabled = false;
                        btnPrimary.Content = "No Packages Selected";
                    }
                });
        }

        private void BindPackageList()
        {
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
                                    FilterBy<CategoryFilter>(packages);
                                    break;
                                case "language":
                                    FilterBy<LanguageFilter>(packages);
                                    break;
                            }
                            break;
                        case TAsyncState.Failure:
                            break;
                    }
                });
        }

        private void FilterBy<T>(List<PackageIndex> packages) where T : class, IValueConverter, new()
        {
            var pkgs = packages.Select(x => new PackageMenuItem(x)).ToList();
            lvPackages.ItemsSource = pkgs;

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvPackages.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("Model", new T());
            view.GroupDescriptions.Add(groupDescription);
        }

        private void btnMenu_Click(object sender, RoutedEventArgs e)
        {
            if (btnMenu.ContextMenu.IsOpen) {
                btnMenu.ContextMenu.IsOpen = false;
                return;
            }

            btnMenu.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            btnMenu.ContextMenu.PlacementTarget = btnMenu;
            btnMenu.ContextMenu.IsOpen = true;
        }
    }

    public class PackageMenuItem
    {
        public PackageIndex Model { get; private set; }

        public PackageMenuItem(PackageIndex model)
        {
            Model = model;
        }

        public string Title => Model.Name["en"];
        public string Version => Model.Version;
        public string Meta => Version + " (" + FileSize + ")";
        public string Status => "Not Installed";

        public string FileSize
        {
            get
            {
                if (Model.Installer != null)
                {
                    return Util.BytesToString(Model.Installer.Value.InstalledSize);
                }
                return "N/A";
            }
        }

        public bool IsSelected
        {
            get
            {
                return App.Store.State.SelectedPackages.Contains(Model);
            }
        
            set
            {
                if (value)
                {
                    if (!App.Store.State.SelectedPackages.Contains(Model))
                    {
                        App.Store.State.SelectedPackages.Add(Model);
                    }
                }
                else
                {
                    App.Store.State.SelectedPackages.Remove(Model);
                }
            }
        }
    }
}
