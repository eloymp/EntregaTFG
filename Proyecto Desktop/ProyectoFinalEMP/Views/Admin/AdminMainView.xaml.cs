using MongoDB.Bson;
using ProyectoFinalEMP.Models;
using ProyectoFinalEMP.Singleton;
using ProyectoFinalEMP.Views.Common;
using ProyectoFinalEMP.Views.DisplayAlerts;
using ProyectoFinalEMP.Views.Unregistered;
using ProyectoFinalEMP.Views.User;
using ProyectoFinalEMP.Views.User.Solicitudes.AsesoriaAdministrativa;
using ProyectoFinalEMP.Views.User.Solicitudes.ServiciosTecnicos;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace ProyectoFinalEMP.Views.Admin
{

    public partial class AdminMainView : Page
    {
        private bool barraExpandida = false;

        public AdminMainView(string tipoSolicitud)
        {
            InitializeComponent();
            CargarSolicitudes(tipoSolicitud);

            // Buscar y seleccionar el item cuyo Tag coincide con el tipo de solicitud
            foreach (ComboBoxItem item in ComboFiltroEstado.Items)
            {
                if ((string)item.Tag == tipoSolicitud)
                {
                    ComboFiltroEstado.SelectedItem = item; // Esto cambia tanto el Tag como el texto visible
                    break;
                }
            }

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

        #region Carga las solicitudes
        private async void CargarSolicitudes(string estado)
        {
            var solicitudes = await GlobalData.Instance.miBBDD.ObtenerTodasLasSolicitudes(estado);

            var datosTabla = new List<FilaSolicitud>();

            foreach (var solicitud in solicitudes)
            {
                string email = "";

                var usuarioId = solicitud.GetValue("usuario_id", BsonNull.Value);
                if (!usuarioId.IsBsonNull)
                {
                    email = await GlobalData.Instance.miBBDD.ObtenerEmailUsuario(usuarioId.AsObjectId);
                }


                datosTabla.Add(new FilaSolicitud
                {
                    EmailUsuario = email,
                    Documento = solicitud,
                    TipoServicio = solicitud.GetValue("tipo_servicio", "").AsString,
                    Subtipo = solicitud.GetValue("subtipo", "").AsString,
                    Estado = solicitud.GetValue("estado", "").AsString,
                    DescripcionCorta = solicitud.GetValue("detalles", new BsonDocument())
                                       .AsBsonDocument.GetValue("descripcion_cliente", "").AsString,
                    FechaCreacion = solicitud.GetValue("fecha_creacion", BsonNull.Value)
                     .ToUniversalTime().ToLocalTime().ToString("dd/MM/yyyy HH:mm")
                });
            }

            TablaSolicitudes.ItemsSource = datosTabla;
        }
        #endregion

        #region Filtrar las solicitudes con el combobox
        private void ComboFiltroEstado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboFiltroEstado.SelectedItem is ComboBoxItem item && item.Tag is string estado)
            {
                CargarSolicitudes(estado);
            }
        }
        #endregion

        #region Dependiendo de que subtipo tenga la soliciutd carga una vista u otra
        private void TablaSolicitudes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (TablaSolicitudes.SelectedItem is FilaSolicitud fila && fila.Documento is BsonDocument doc)
            {
                string subtipo = fila.Subtipo;

                switch (subtipo)
                {
                    // Subtipos de servicios técnicos
                    case "asistencia_tecnica":
                        NavigationService.Navigate(new AsistenciaTecnica(doc));
                        break;

                    // Subtipos de servicios técnicos
                    case "intermediacion_asesor_fiscal":
                        NavigationService.Navigate(new IntermediacionAsesorFiscal(doc));
                        break;

                }
            }
        }
        #endregion

        #region Boton gestionar usuarios
        private void GestionarUsuarios_Click(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new ManageUsers());
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
            NavigationService.Navigate(new AboutUsViewAdmin());
        }
        #endregion

        #region MenuAyuda
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new FAQViewAdmin());
        }
        #endregion

        #region BotonLogo
        private void ImgLogo_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminMainView("todos"));

        }
        #endregion

        #region MenuInicio
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminMainView("todos"));

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

        #region Eventos del menu izquierdo para mostrar solicitudes
        private void VerTodasSolicitudes(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new AdminMainView("todos"));
        }

        private void VerPendientesSolicitudes(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new AdminMainView("pendiente"));
        }

        private void VerEnProcesoSolicitudes(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new AdminMainView("en_proceso"));
        }

        private void VerFinalizadasSolicitudes(object sender, MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new AdminMainView("finalizada"));
        }
        #endregion


    }
}