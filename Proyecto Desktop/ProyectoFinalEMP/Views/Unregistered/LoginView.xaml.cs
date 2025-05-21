using MongoDB.Bson;
using ProyectoFinalEMP.Controller;
using ProyectoFinalEMP.Singleton;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ProyectoFinalEMP.Cryptography;
using MongoDB.Driver;
using ProyectoFinalEMP.Views.Admin;
using ProyectoFinalEMP.Views.SuperAdmin;
using ProyectoFinalEMP.Views.User;

namespace ProyectoFinalEMP.Views.Unregistered
{

    public partial class LoginView : Page
    {
        public LoginView()
        {
            InitializeComponent();


            CargarDiccionariosDefecto();


            var gestor = new SQLiteManager();
            string[] datos = gestor.CargarSesion();

            if (datos != null)
            {
                txtUsuario.Text = datos[0];
                txtContraseña.Password = CryptographyPassword.Descifrar(datos[1]);
            }
        }

        #region Pulsar enter y que inicie sesion
        private void Page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoginButton_Click(sender, e);
            }
        }
        #endregion

        #region Boton Login
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginButton.IsEnabled = false;

            try
            {

                string email = txtUsuario.Text.Trim().ToLower();
                string contraseña = txtContraseña.Password;

                //Comprobar que no esten vacios los campos
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(contraseña))
                {
                    MessageBox.Show((string)Application.Current.FindResource("DisplayAlertObligatori"));
                    return;
                }

                //Crear hash de la contraseña para la comparacion
                string hash = GlobalData.Instance.administradorHash.GenerarHash(contraseña);

                //Guardar los datos del usuario si existe
                BsonDocument usuario = await GlobalData.Instance.miBBDD.ObtenerUsuario(email, hash);

           

                if (usuario == null)
                {
                    MessageBox.Show((string)Application.Current.FindResource("DisplayAlertIncorrectUserOrPass"));
                    return;
                }

                //Guardar los datos del usuario de manera global
                GlobalData.Instance.UsuarioLogueado = usuario;

                //Comprobar que la cuenta este activa
                if (usuario["estado_cuenta"] != "activa")
                {
                    MessageBox.Show((string)Application.Current.FindResource("DisplayAlertAccountNotActive"));
                    return;
                }

                var preferencias = await GlobalData.Instance.miBBDD.ObtenerPreferenciasUsuario(usuario["_id"].AsObjectId);

                Application.Current.Resources.MergedDictionaries.Clear();

                if (preferencias.idioma == "español")
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
                    {
                        Source = new Uri("Resources/Lenguages/Spanish.xaml", UriKind.Relative)
                    });
                else if (preferencias.idioma == "ingles")
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
                    {
                        Source = new Uri("Resources/Lenguages/English.xaml", UriKind.Relative)
                    });

                if (preferencias.tema == "oscuro")
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
                    {
                        Source = new Uri("Resources/Themes/DarkTheme.xaml", UriKind.Relative)
                    });
                else if (preferencias.tema == "claro")
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
                    {
                        Source = new Uri("Resources/Themes/LightTheme.xaml", UriKind.Relative)
                    });


                //Dependiendo del rol cargar una interfaz u otra
                string rol = usuario["rol"].AsString;
                switch (rol)
                {
                    case "usuario":
                        NavigationService.Navigate(new UserMainView());
                        break;
                    case "administrador":
                        NavigationService.Navigate(new AdminMainView("todos"));
                        break;
                    case "superadministrador":
                        NavigationService.Navigate(new SuperAdminMainView("todos"));
                        break;
                }

                if (checkBoxRecordarme.IsChecked == true)
                {
                    string cifrada = CryptographyPassword.Cifrar(contraseña);
                    var gestor = new SQLiteManager();
                    gestor.GuardarSesion(email, cifrada);
                }

                var updateSesion = Builders<BsonDocument>.Update.Set("ultimo_inicio_sesion", DateTime.UtcNow);
                await GlobalData.Instance.miBBDD.ActualizarUsuario(email, updateSesion);

            }
            finally
            {
                LoginButton.IsEnabled = true;
            }

        }
        #endregion

        #region Hipervinculo correo
        private void HyperLinkPassword_Click(object sender, MouseButtonEventArgs e)
        {
            //Hilo
            Thread thread = new Thread(() =>
            {
                string adminEmail = "administracion@gesem.com";
                string asunto = (string)Application.Current.FindResource("SubjectLogin");

                string cuerpoSinFecha = (string)Application.Current.FindResource("Body");
                string cuerpo = string.Format(cuerpoSinFecha, DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

                string mailtoUri = $"https://mail.google.com/mail/?view=cm&fs=1&to={adminEmail}&su={Uri.EscapeDataString(asunto)}&body={Uri.EscapeDataString(cuerpo)}";

                var psi = new ProcessStartInfo(mailtoUri)
                {
                    UseShellExecute = true
                };

                Process.Start(psi);
            });

            thread.Start();
        }
        #endregion

        #region PlaceHolder Usuario
        private void txtUsuario_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsuario.Text))
            {
                placeholderUsuario.Visibility = Visibility.Visible;
            }
            else
            {
                placeholderUsuario.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region PlaceHolder Contraseña
        private void txtContraseña_TextChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtContraseña.Password))
            {
                placeholderPassword.Visibility = Visibility.Visible;
            }
            else
            {
                placeholderPassword.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region MenuAboutUs
        private void AboutUs_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AboutUsViewUnregistered());
        }
        #endregion

        #region MenuAyuda
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FAQViewUnregistered());
        }
        #endregion

        #region BotonLogo
        private void ImgLogo_Click(object sender, RoutedEventArgs e)
        {
            var usuario = GlobalData.Instance.UsuarioLogueado;

            if (usuario == null)
            {
                NavigationService.Navigate(GlobalData.Instance.Login);
            }
            else
            {
                string rol = usuario["rol"].AsString;

                switch (rol)
                {
                    case "usuario":
                        NavigationService.Navigate(new UserMainView());
                        break;
                    case "administrador":
                        NavigationService.Navigate(new AdminMainView("todos"));
                        break;
                    case "superadministrador":
                        NavigationService.Navigate(new SuperAdminMainView("todos"));
                        break;
                }
            }
        }
        #endregion

        #region MenuInicio
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            var usuario = GlobalData.Instance.UsuarioLogueado;

            if (usuario == null)
            {
                NavigationService.Navigate(GlobalData.Instance.Login);
            }
            else
            {
                string rol = usuario["rol"].AsString;

                switch (rol)
                {
                    case "usuario":
                        NavigationService.Navigate(new UserMainView());
                        break;
                    case "administrador":
                        NavigationService.Navigate(new AdminMainView("todos"));
                        break;
                    case "superadministrador":
                        NavigationService.Navigate(new SuperAdminMainView("todos"));
                        break;
                }
            }
        }
        #endregion

        #region Cargar diccionarios por defecto
        private void CargarDiccionariosDefecto()
        {
            var idioma = new ResourceDictionary
            {
                Source = new Uri("Resources/Lenguages/Spanish.xaml", UriKind.Relative)
            };

            var tema = new ResourceDictionary
            {
                Source = new Uri("Resources/Themes/DarkTheme.xaml", UriKind.Relative)
            };

            // Eliminar temas e idiomas anteriores
            var anteriores = Application.Current.Resources.MergedDictionaries
                .Where(d => d.Source != null &&
                    (d.Source.OriginalString.Contains("Spanish.xaml") ||
                     d.Source.OriginalString.Contains("English.xaml") ||
                     d.Source.OriginalString.Contains("LightTheme.xaml") ||
                     d.Source.OriginalString.Contains("DarkTheme.xaml")))
                .ToList();

            foreach (var dic in anteriores)
                Application.Current.Resources.MergedDictionaries.Remove(dic);

            // Añadir los nuevos
            Application.Current.Resources.MergedDictionaries.Add(idioma);
            Application.Current.Resources.MergedDictionaries.Add(tema);
        }
        #endregion

    }
}
