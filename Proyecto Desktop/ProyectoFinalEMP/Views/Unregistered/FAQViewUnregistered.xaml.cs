using ProyectoFinalEMP.Singleton;
using ProyectoFinalEMP.Views.Unregistered;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace ProyectoFinalEMP.Views.Unregistered
{

    public partial class FAQViewUnregistered : Page
    {
        public FAQViewUnregistered()
        {
            InitializeComponent();
        }

        #region HiperVinculoWeb
        private void WebHyperLink_Click(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://www.ejemplo.com") { UseShellExecute = true });
        }
        #endregion

        #region MenuAboutUs
        private void AboutUs_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AboutUsViewUnregistered());
        }
        #endregion

        #region MenuInicio
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(GlobalData.Instance.Login);
        }
        #endregion


    }
}
