using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
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
using System.Windows.Shapes;

namespace Divvun.PkgMgr
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private CompositeDisposable dispose = new CompositeDisposable();
        private bool repoUriChanged = false;

        public SettingsWindow()
        {
            InitializeComponent();

            dispose.Add(App.Store.Observe()
                .Select(state => state.RepositoryUrl)
                .Subscribe(url => txtRepoUri.Text = url));

            txtRepoUri.TextChanged += (sender, e) => repoUriChanged = true;
        }
        
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (repoUriChanged)
            {
                App.Store.State.RepositoryUrl = txtRepoUri.Text.Trim();
            }

            Close();
        }
    }
}
