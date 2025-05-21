using ProyectoFinalEMP.Singleton;
using ProyectoFinalEMP.Views.Unregistered;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace ProyectoFinalEMP.Views.Unregistered
{

    public partial class AboutUsViewUnregistered : Page
    {
        public AboutUsViewUnregistered()
        {
            InitializeComponent();
        }


        #region HiperVinculoMail
        private void MailHyperLink_Click(object sender, MouseButtonEventArgs e)
        {
            string adminEmail = "administracion@gesem.com";
            string asunto = (string)Application.Current.FindResource("SubjectAboutUs");

            //URL con el mensaje
            string mailtoUri = $"https://mail.google.com/mail/?view=cm&fs=1&to={adminEmail}&su={Uri.EscapeDataString(asunto)}";

            var psi = new ProcessStartInfo(mailtoUri)
            {
                UseShellExecute = true //programa predeterminado
            };

            Process.Start(psi);
        }
        #endregion

        #region HiperVinculoWeb
        private void WebHyperLink_Click(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://www.gesem.com") { UseShellExecute = true });
        }
        #endregion

        #region HiperVinculoDireccion
        private void AddressHyperLink_Click(object sender, MouseButtonEventArgs e)
        {
            string direccion = "C/ Hermanos Becerril, 3, \r\n16004, Cuenca";
            string url = $"https://www.google.com/maps/search/?api=1&query={Uri.EscapeDataString(direccion)}";

            var psi = new ProcessStartInfo(url)
            {
                UseShellExecute = true //programa predeterminado
            };

            Process.Start(psi);
        }
        #endregion

        #region MenuAyuda
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FAQViewUnregistered());
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
