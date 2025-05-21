using MongoDB.Bson;
using ProyectoFinalEMP.Models;
using ProyectoFinalEMP.Singleton;
using ProyectoFinalEMP.Views.Admin;
using ProyectoFinalEMP.Views.Common;
using ProyectoFinalEMP.Views.DisplayAlerts;
using ProyectoFinalEMP.Views.Unregistered;
using ProyectoFinalEMP.Views.User;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace ProyectoFinalEMP.Views.SuperAdmin
{

    public partial class ManageUsersAdmins : Page
    {
        private bool barraExpandida = false;
        private List<BsonDocument> todosLosUsuarios;


        public ManageUsersAdmins()
        {
            InitializeComponent();

            CargarUsuarios();

            #region Cargar datos del usuario en el menu
            var usuario = GlobalData.Instance.UsuarioLogueado;

            if (usuario.Contains("email"))
                MenuProfileEmail.Text = (string)Application.Current.FindResource("MenuProfileEmail") + " " + usuario["email"].AsString;

            if (usuario.Contains("nombre"))
                MenuProfileName.Text = (string)Application.Current.FindResource("MenuProfileName") + " " + usuario["nombre"].AsString;

            if (usuario.Contains("teléfono"))
                MenuProfileNumber.Text = (string)Application.Current.FindResource("MenuProfileNumber") + " " + usuario["teléfono"].AsString;

            if (usuario.Contains("nif"))
                MenuProfileAddress.Text = (string)Application.Current.FindResource("MenuProfileNif") + " " + usuario["nif"].AsString;

            if (usuario.Contains("foto_perfil") && !usuario["foto_perfil"].IsBsonNull)
            {
                var imagenBytes = usuario["foto_perfil"].AsBsonBinaryData.Bytes;
                using (var stream = new MemoryStream(imagenBytes))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = stream;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    MenuImage.ImageSource = image;
                }
            }

            #endregion
        }

        #region Cargar usuarios
        private async Task CargarUsuarios()
        {
            var documentos = await GlobalData.Instance.miBBDD.ObtenerUsuariosSinSuperadmin();

            TablaUsuarios.ItemsSource = documentos.Select(d => new UsersDataGrid
            {
                Email = d.GetValue("email", "").AsString,
                Rol = d.GetValue("rol", "").AsString,
                EstadoCuenta = d.GetValue("estado_cuenta", "").AsString
            }).ToList();
        }
        #endregion

        #region Seleccion combobox para filtrar los usuarios que se muestran
        private async void ComboFiltroRol_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = ComboFiltroRol.SelectedItem as ComboBoxItem;
            string filtro = item?.Content.ToString().ToLower();

            List<BsonDocument> documentos = new();

            if (filtro == "usuarios")
            {
                documentos = await GlobalData.Instance.miBBDD.ObtenerUsuarios();
            }
            else if (filtro == "administradores")
            {
                documentos = await GlobalData.Instance.miBBDD.ObtenerAdministradores(); 
            }
            else 
            {
                documentos = await GlobalData.Instance.miBBDD.ObtenerUsuariosSinSuperadmin(); 
            }

            TablaUsuarios.ItemsSource = documentos.Select(d => new UsersDataGrid
            {
                Email = d.GetValue("email", "").AsString,
                Rol = d.GetValue("rol", "").AsString,
                EstadoCuenta = d.GetValue("estado_cuenta", "").AsString
            }).ToList();
        }
        #endregion

        #region Boton gestionar usuarios
        private void GestionarUsuarios_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new ManageUsersAdmins());
        }
        #endregion

        #region Expandir barra lateral
        private void PanelLateral_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            double anchoDestino;
            Visibility visibilidad;


            if (barraExpandida)
            {

                visibilidad = Visibility.Collapsed;
                anchoDestino = 65;
            }
            else
            {
                anchoDestino = 200;
                visibilidad = Visibility.Visible;
            }

            var animacion = new DoubleAnimation
            {
                To = anchoDestino,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new QuadraticEase()
            };

            PanelLateral.BeginAnimation(WidthProperty, animacion);

            TextoSolicitudes.Visibility = visibilidad;
            TextoVerTodas.Visibility = visibilidad;
            TextoPendientes.Visibility = visibilidad;
            TextoEnProceso.Visibility = visibilidad;
            TextoFinalizadas.Visibility = visibilidad;
            TextoGestionarUsuarios.Visibility = visibilidad;

            barraExpandida = !barraExpandida;

            //mostrar ultimo inicio de sesion

            var usuario = GlobalData.Instance.UsuarioLogueado;

            if (usuario.Contains("ultimo_inicio_sesion"))
            {
                var fecha = usuario["ultimo_inicio_sesion"].ToUniversalTime().ToLocalTime().ToString("dd/MM/yyyy HH:mm");
                TextoUltimaConexion.Text = (string)Application.Current.FindResource("LeftMenuLastConnection") + "\n" + fecha;

                TextoUltimaConexion.Visibility = visibilidad;
            }

        }
        #endregion

        #region clicar fuera de la barra para que se minimice
        private void MainGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (barraExpandida)
            {
                // Comprobamos si se hizo clic FUERA de la barra lateral
                if (!PanelLateral.IsMouseOver)
                {
                    MinimizarBarraLateral();
                }
            }
        }
        #endregion

        #region Minimizar barra lateral
        private void MinimizarBarraLateral()
        {
            double anchoDestino = 65;
            var animacion = new DoubleAnimation
            {
                To = anchoDestino,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new QuadraticEase()
            };

            PanelLateral.BeginAnimation(WidthProperty, animacion);

            // Ocultar los textos
            TextoSolicitudes.Visibility = Visibility.Collapsed;
            TextoVerTodas.Visibility = Visibility.Collapsed;
            TextoPendientes.Visibility = Visibility.Collapsed;
            TextoEnProceso.Visibility = Visibility.Collapsed;
            TextoFinalizadas.Visibility = Visibility.Collapsed;
            TextoGestionarUsuarios.Visibility = Visibility.Collapsed;
            TextoUltimaConexion.Visibility = Visibility.Collapsed;

            barraExpandida = false;
        }
        #endregion

        #region MenuAboutUs
        private void AboutUs_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AboutUsViewSuperAdmin());
        }
        #endregion

        #region MenuAyuda
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FAQViewSuperAdmin());
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

        #region MenuTema
        private void Theme_Click(object sender, RoutedEventArgs e)
        {
            DisplayAlertTheme displayAlert = new DisplayAlertTheme();
            displayAlert.Owner = Window.GetWindow(this); // Esto hace que se centre en esta ventana
            displayAlert.ShowDialog();
        }
        #endregion

        #region MenuIdioma
        private void Lenguage_Click(object sender, RoutedEventArgs e)
        {
            DisplayAlertLenguage displayAlert = new DisplayAlertLenguage();
            displayAlert.Owner = Window.GetWindow(this); // Esto hace que se centre en esta ventana
            displayAlert.ShowDialog();
        }
        #endregion

        #region MenuCerrarSesion
        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            var confirmar = MessageBox.Show(
                (string)Application.Current.FindResource("LogOutConfirmation"),
                (string)Application.Current.FindResource("MenuItemLogOut"),
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (confirmar == MessageBoxResult.Yes)
            {
                GlobalData.Instance.UsuarioLogueado = null;

                NavigationService?.Navigate(new LoginView());
            }
        }
        #endregion

        #region EditarPerfil
        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {
            ProfileView perfil = new ProfileView();
            perfil.Owner = Window.GetWindow(this); // Esto hace que se centre en esta ventana
            perfil.ShowDialog();

        }
        #endregion

        #region Boton añadir usuario
        private void BtnAñadirUsuario_Click(object sender, MouseButtonEventArgs e)
        {
            AddUserView perfil = new AddUserView();
            perfil.Owner = Window.GetWindow(this); // Esto hace que se centre en esta ventana
            perfil.ShowDialog();

            // recargar la tabla si ha cambiado
            _ = CargarUsuarios();
        }
        #endregion

        #region Clicar dos veces en la tabla
        private async void  TablaUsuarios_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TablaUsuarios.SelectedItem is UsersDataGrid fila)
            {
                var doc = await GlobalData.Instance.miBBDD.ObtenerUsuarioPorEmail(fila.Email);
                if (doc == null) return;

                var ventana = new ModifyUserAdminView(doc);
                ventana.Owner = Window.GetWindow(this);
                if (ventana.ShowDialog() == true)
                {
                    _ = CargarUsuarios();
                }
            }
        }
        #endregion

        #region Eventos del menu izquierdo para mostrar solicitudes
        private void VerTodasSolicitudes(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new SuperAdminMainView("todos"));
        }

        private void VerPendientesSolicitudes(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new SuperAdminMainView("pendiente"));
        }

        private void VerEnProcesoSolicitudes(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new SuperAdminMainView("en_proceso"));
        }

        private void VerFinalizadasSolicitudes(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new SuperAdminMainView("finalizada"));
        }
        #endregion
    }
}