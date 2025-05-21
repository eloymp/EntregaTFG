using Microsoft.Win32;
using MongoDB.Bson;
using ProyectoFinalEMP.Models;
using ProyectoFinalEMP.Singleton;
using ProyectoFinalEMP.Views.DisplayAlerts;
using ProyectoFinalEMP.Views.Unregistered;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Media;
using ProyectoFinalEMP.Views.Common;
using ProyectoFinalEMP.Views.Admin;
using ProyectoFinalEMP.Views.SuperAdmin;

namespace ProyectoFinalEMP.Views.User.Solicitudes.AsesoriaAdministrativa
{

    public partial class IntermediacionAsesorFiscal : Page
    {
        private bool barraExpandida = false;
        private List<ArchivoAdjunto> archivosAdjuntos = new List<ArchivoAdjunto>();
        private List<ArchivoAdjunto> archivosAdjuntosAdmin = new List<ArchivoAdjunto>();

        private BsonDocument solicitud;
        public IntermediacionAsesorFiscal()
        {
            InitializeComponent();

            PanelArchivos.ItemsSource = archivosAdjuntos;

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

        public IntermediacionAsesorFiscal(BsonDocument solicitud)
        {
            InitializeComponent();
            CargarSolicitud(solicitud);
            this.solicitud = solicitud;

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

        #region Boton añadir archivo
        private void BtnAñadirArchivo_Click(object sender, MouseButtonEventArgs e)
        {
            var dialogo = new OpenFileDialog
            {
                Filter = (string)Application.Current.FindResource("FilterDocuments") + " (*.*)|*.*",
                Multiselect = false,
                Title = (string)Application.Current.FindResource("SelectDocument")

            };

            if (dialogo.ShowDialog() == true)
            {
                var archivoInfo = new FileInfo(dialogo.FileName);

                // Verificación de tamaño máximo 10 MB
                if (archivoInfo.Length > 10 * 1024 * 1024)
                {
                    MessageBox.Show((string)Application.Current.FindResource("BigDocument")
);
                    return;
                }

                var nuevoArchivo = new ArchivoAdjunto
                {
                    Nombre = Path.GetFileName(dialogo.FileName),
                    Contenido = File.ReadAllBytes(dialogo.FileName),
                    Comentario = ""
                };

                archivosAdjuntos.Add(nuevoArchivo);

                PanelArchivos.ItemsSource = null;
                PanelArchivos.ItemsSource = archivosAdjuntos;

                PanelArchivosAdmin.ItemsSource = null;
                PanelArchivosAdmin.ItemsSource = archivosAdjuntos;
            }
        }

        #endregion

        #region Boton eliminar archivo
        private void BtnEliminarArchivo_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image img && img.Tag is ArchivoAdjunto archivo)
            {
                //Elimina el archivo de la lista
                archivosAdjuntos.Remove(archivo);

                // Refrescar la vista
                PanelArchivos.ItemsSource = null;
                PanelArchivos.ItemsSource = archivosAdjuntos;

                PanelArchivosAdmin.ItemsSource = null;
                PanelArchivosAdmin.ItemsSource = archivosAdjuntos;
            }
        }
        #endregion

        #region Descargar archivo
        private void TxtNombreArchivo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var textBlock = sender as TextBlock;
            if (textBlock?.Tag is ArchivoAdjunto archivo)
            {
                var dialogo = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = archivo.Nombre,
                    Filter = "Todos los archivos (*.*)|*.*"
                };

                if (dialogo.ShowDialog() == true)
                {
                    File.WriteAllBytes(dialogo.FileName, archivo.Contenido);
                }
            }
        }
        #endregion

        #region Botón enviar
        private async void BtnEnviar_Click(object sender, RoutedEventArgs e)
        {
            // Validar campos obligatorios
            if (string.IsNullOrWhiteSpace(TxtDescripcionCliente.Text) ||
            string.IsNullOrWhiteSpace(TxtNombreAsesor.Text) ||
            string.IsNullOrWhiteSpace(TxtCorreoAsesor.Text) ||
            string.IsNullOrWhiteSpace(TxtTelefonoAsesor.Text) ||
            cmbPeriodoFiscal.SelectedItem == null ||
            ((ComboBoxItem)cmbPeriodoFiscal.SelectedItem).Tag.ToString() == "trimestral" && cmbTrimestre.SelectedItem == null)
            {
                MessageBox.Show((string)Application.Current.FindResource("DisplayAlertObligatori"));
                return;
            }

            BtnEnviar.IsEnabled = false;


            // Recoger datos del formulario
            var usuarioId = GlobalData.Instance.UsuarioLogueado["_id"].AsObjectId;
            string descripcion = TxtDescripcionCliente.Text.Trim();
            string nombreAsesor = TxtNombreAsesor.Text.Trim();
            string correoAsesor = TxtCorreoAsesor.Text.Trim();
            string telefonoAsesor = TxtTelefonoAsesor.Text.Trim();
            string tipoPeriodo = ((ComboBoxItem)cmbPeriodoFiscal.SelectedItem).Tag.ToString();
            string trimestre = tipoPeriodo == "trimestral" ? ((ComboBoxItem)cmbTrimestre.SelectedItem).Tag.ToString() : null;

            // Validar formato del correo
            bool correoValido = System.Text.RegularExpressions.Regex.IsMatch(
                correoAsesor,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"
            );

            if (!correoValido)
            {
                MessageBox.Show((string)Application.Current.FindResource("FormatoCorreo"));
                BtnEnviar.IsEnabled = true;
                return;
            }


            // Enviar solicitud a MongoDB con los archivos completos
            await GlobalData.Instance.miBBDD.InsertarSolicitudIntermediacionAsesorFiscal(
             usuarioId,
             descripcion,
             nombreAsesor,
             correoAsesor,
             telefonoAsesor,
             tipoPeriodo,
             trimestre,
             archivosAdjuntos
            );

            // Bloquear campos tras el envío
            TxtDescripcionCliente.IsEnabled = false;
            TxtNombreAsesor.IsEnabled = false;
            TxtCorreoAsesor.IsEnabled = false;
            TxtTelefonoAsesor.IsEnabled = false;
            cmbPeriodoFiscal.IsEnabled = false;
            cmbTrimestre.IsEnabled = false;
            BtnAñadirArchivo.IsEnabled = false;
            BtnEnviar.IsEnabled = false;

            // Aplicar bloqueo y estilos a los campos manualmente
            TxtDescripcionCliente.Background = (Brush)Application.Current.FindResource("ColorFondoBloqueado");
            TxtDescripcionCliente.Foreground = (Brush)Application.Current.FindResource("ColorTextoBloqueado");

            TxtNombreAsesor.Background = (Brush)Application.Current.FindResource("ColorFondoBloqueado");
            TxtNombreAsesor.Foreground = (Brush)Application.Current.FindResource("ColorTextoBloqueado");

            TxtCorreoAsesor.Background = (Brush)Application.Current.FindResource("ColorFondoBloqueado");
            TxtCorreoAsesor.Foreground = (Brush)Application.Current.FindResource("ColorTextoBloqueado");

            TxtTelefonoAsesor.Background = (Brush)Application.Current.FindResource("ColorFondoBloqueado");
            TxtTelefonoAsesor.Foreground = (Brush)Application.Current.FindResource("ColorTextoBloqueado");

            cmbPeriodoFiscal.Background = (Brush)Application.Current.FindResource("ColorFondoBloqueado");
            cmbPeriodoFiscal.Foreground = (Brush)Application.Current.FindResource("ColorTextoBloqueado");

            cmbTrimestre.Background = (Brush)Application.Current.FindResource("ColorFondoBloqueado");
            cmbTrimestre.Foreground = (Brush)Application.Current.FindResource("ColorTextoBloqueado");

            // Bloquear edición de comentarios en los archivos
            foreach (var item in PanelArchivos.Items)
            {
                if (PanelArchivos.ItemContainerGenerator.ContainerFromItem(item) is FrameworkElement container)
                {
                    foreach (var textBox in container.FindVisualChildren<TextBox>())
                    {
                        textBox.IsReadOnly = true;
                        textBox.Background = (Brush)Application.Current.FindResource("ColorFondoBloqueado");
                        textBox.Foreground = (Brush)Application.Current.FindResource("ColorTextoBloqueado");
                    }
                }
            }

            // Cambiar visibilidad de botones
            BtnEnviar.Visibility = Visibility.Collapsed;
            BtnAñadirArchivo.IsEnabled = false;

            foreach (var item in PanelArchivos.Items)
            {
                if (PanelArchivos.ItemContainerGenerator.ContainerFromItem(item) is FrameworkElement container)
                {
                    foreach (var imagen in container.FindVisualChildren<Image>())
                    {
                        imagen.IsEnabled = false;
                        imagen.Opacity = 0.4;
                    }
                }
            }

        }
        #endregion

        #region Boton enviar admin
        private async void BtnEnviarRespuesta_Click(object sender, RoutedEventArgs e)
        {
            if (solicitud == null) return;

            string comentario = TxtComentariosAdmin.Text.Trim();

            // Validación: el comentario no puede estar vacío
            if (string.IsNullOrWhiteSpace(comentario))
            {
                MessageBox.Show((string)Application.Current.FindResource("DisplayAlertObligatori"));
                return;
            }

            var id = solicitud.GetValue("_id").AsObjectId;

            await GlobalData.Instance.miBBDD.InsertarRespuestaAdmin(
                id,
                comentario,
                archivosAdjuntosAdmin
            );


            // Bloquear campos tras enviar
            BtnEnviarRespuesta.Visibility = Visibility.Collapsed;

            PanelAdminCompleto.IsEnabled = false;
        }
        #endregion

        #region Boton añadir archivo
        private void BtnAñadirArchivoAdmin_Click(object sender, MouseButtonEventArgs e)
        {
            var dialogo = new OpenFileDialog
            {
                Filter = (string)Application.Current.FindResource("FilterDocuments") + " (*.*)|*.*",
                Multiselect = false,
                Title = (string)Application.Current.FindResource("SelectDocument")

            };

            if (dialogo.ShowDialog() == true)
            {
                var archivoInfo = new FileInfo(dialogo.FileName);

                // Verificación de tamaño máximo 10 MB
                if (archivoInfo.Length > 10 * 1024 * 1024)
                {
                    MessageBox.Show((string)Application.Current.FindResource("BigDocument")
);
                    return;
                }

                var nuevoArchivo = new ArchivoAdjunto
                {
                    Nombre = Path.GetFileName(dialogo.FileName),
                    Contenido = File.ReadAllBytes(dialogo.FileName),
                    Comentario = ""
                };

                archivosAdjuntosAdmin.Add(nuevoArchivo);

                PanelArchivosAdmin.ItemsSource = null;
                PanelArchivosAdmin.ItemsSource = archivosAdjuntosAdmin;
            }
        }

        #endregion

        #region Boton eliminar archivo
        private void BtnEliminarArchivoAdmin_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image img && img.Tag is ArchivoAdjunto archivo)
            {
                //Elimina el archivo de la lista
                archivosAdjuntosAdmin.Remove(archivo);

                // Refrescar la vista
                PanelArchivosAdmin.ItemsSource = null;
                PanelArchivosAdmin.ItemsSource = archivosAdjuntosAdmin;
            }
        }
        #endregion

        #region Boton eliminar solicitud que se muestra al meterse a una solicitud
        private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            var confirmar = MessageBox.Show(
            (string)Application.Current.FindResource("DeleteRequestAdvert"),
            (string)Application.Current.FindResource("ConfirmElimination")
,
        MessageBoxButton.YesNo,
        MessageBoxImage.Warning
        );

            if (confirmar == MessageBoxResult.Yes && solicitud != null)
            {
                var id = solicitud.GetValue("_id", null)?.AsObjectId;
                if (id != null)
                {
                    await GlobalData.Instance.miBBDD.EliminarSolicitud(id.Value);
                    MessageBox.Show((string)Application.Current.FindResource("EliminationOk")
        );


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
                                NavigationService.Navigate(new UserManageRequests("todos"));
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
            }
        }

        #endregion

        #region Si es trimestral que se muestre el combobox de los trimestres
        private void cmbPeriodoFiscal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbPeriodoFiscal.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag != null)
            {
                string tipo = selectedItem.Tag?.ToString();

                if (tipo == "trimestral")
                {
                    cmbTrimestre.Visibility = Visibility.Visible;
                    cmbTrimestre.SelectedIndex = 0;
                }
                else
                {
                    cmbTrimestre.Visibility = Visibility.Collapsed;
                    cmbTrimestre.SelectedItem = null;
                }
            }
        }

        #endregion

        #region Cargar solicitud
        private async void CargarSolicitud(BsonDocument doc)
        {

            BloquearCamposEdicion();

            // Obtener info del usuario actual
            string rol = GlobalData.Instance.UsuarioLogueado["rol"].AsString;
            var usuarioId = GlobalData.Instance.UsuarioLogueado["_id"].AsObjectId;
            var creadorId = doc.GetValue("usuario_id").AsObjectId;
            string estado = doc.GetValue("estado").AsString;

            // Mostrar botón eliminar si corresponde
            if (rol == "usuario" && usuarioId == creadorId && estado == "pendiente")
                BtnEliminar.Visibility = Visibility.Visible;
            else if (rol == "administrador" || rol == "superadministrador")
                BtnEliminar.Visibility = Visibility.Visible;

            // Rellenar campos
            TxtDescripcionCliente.Text = doc["detalles"]["descripcion_cliente"].AsString;
            TxtNombreAsesor.Text = doc["detalles"]["nombre_asesor"].AsString;
            TxtCorreoAsesor.Text = doc["detalles"]["correo_asesor"].AsString;
            TxtTelefonoAsesor.Text = doc["detalles"]["telefono_asesor"].AsString;
            string tipoPeriodo = doc["detalles"]["tipo_periodo"].AsString;
            foreach (ComboBoxItem item in cmbPeriodoFiscal.Items)
            {
                if ((string)item.Tag == tipoPeriodo)
                {
                    cmbPeriodoFiscal.SelectedItem = item;
                    break;
                }
            }

            if (tipoPeriodo == "trimestral")
            {
                cmbTrimestre.Visibility = Visibility.Visible;
                string trimestre = doc["detalles"]["trimestre"].AsString;

                foreach (ComboBoxItem item in cmbTrimestre.Items)
                {
                    if ((string)item.Tag == trimestre)
                    {
                        cmbTrimestre.SelectedItem = item;
                        break;
                    }
                }
            }

            // Mostrar archivos adjuntos del cliente
            archivosAdjuntos = doc["archivos_adjuntos"].AsBsonArray.Select(a => new ArchivoAdjunto
            {
                Nombre = a["nombre"].AsString,
                Comentario = a["comentario"].AsString,
                Contenido = a["contenido"].AsBsonBinaryData.Bytes
            }).ToList();

            PanelArchivosClienteView.ItemsSource = null;
            PanelArchivosClienteView.ItemsSource = archivosAdjuntos;

            PanelArchivosCliente.Visibility = Visibility.Visible;

            // Mostrar respuesta admin si está finalizada
            if (estado == "finalizada")
            {

                PanelRespuestaAdmin.Visibility = Visibility.Visible;

                // Mostrar comentario del administrador
                TxtComentariosAdminView.Text = doc["respuesta_admin"]["comentario"].AsString;
                TxtComentariosAdminView.IsReadOnly = true;
                TxtComentariosAdminView.Background = (Brush)Application.Current.FindResource("ColorFondoBloqueado");
                TxtComentariosAdminView.Foreground = (Brush)Application.Current.FindResource("ColorTextoBloqueado");

                // Cargar archivos enviados por el administrador
                var archivosRespuestaAdmin = doc["respuesta_admin"]["archivos"].AsBsonArray.Select(a => new ArchivoAdjunto
                {
                    Nombre = a["nombre"].AsString,
                    Comentario = a["comentario"].AsString,
                    Contenido = a["contenido"].AsBsonBinaryData.Bytes
                }).ToList();

                PanelRespuestaAdminView.ItemsSource = null;
                PanelRespuestaAdminView.ItemsSource = archivosRespuestaAdmin;

                // Mostrar fecha de respuesta
                if (doc.Contains("fecha_actualizacion"))
                {
                    var fecha = doc["fecha_actualizacion"].ToUniversalTime().ToLocalTime();
                    LblFechaRespuesta.Content = $"{Application.Current.FindResource("FechaRespuesta")} {fecha:dd/MM/yyyy HH:mm}";
                    LblFechaRespuesta.Visibility = Visibility.Visible;
                }

            }


            // Si es admin y la solicitud está pendiente, actualizar estado a en_proceso
            if ((rol == "administrador" || rol == "superadministrador") && (estado == "pendiente" || estado == "en_proceso"))
            {
                var id = doc["_id"].AsObjectId;
                await GlobalData.Instance.miBBDD.ActualizarEstadoSolicitud(id, "en_proceso");

                PanelAdminCompleto.Visibility = Visibility.Visible;
                BtnEnviarRespuesta.Visibility = Visibility.Visible;
            }



        }
        #endregion

        #region Bloquear campos de edicion y ponerlos oscuros
        public void BloquearCamposEdicion()
        {
            // Desactivar campos
            TxtDescripcionCliente.IsReadOnly = true;
            TxtNombreAsesor.IsReadOnly = true;
            TxtCorreoAsesor.IsReadOnly = true;
            TxtTelefonoAsesor.IsReadOnly = true;
            cmbPeriodoFiscal.IsEnabled = false;
            cmbTrimestre.IsEnabled = false;
            BtnEnviar.Visibility = Visibility.Collapsed;
            BtnAñadirArchivo.IsEnabled = false;
            PanelArchivos.IsEnabled = false;
            AdjuntarArchivos.Visibility = Visibility.Collapsed;

            // Colores bloqueados
            TxtDescripcionCliente.Background = (Brush)Application.Current.FindResource("ColorFondoBloqueado");
            TxtDescripcionCliente.Foreground = (Brush)Application.Current.FindResource("ColorTextoBloqueado");

            TxtNombreAsesor.Background = (Brush)Application.Current.FindResource("ColorFondoBloqueado");
            TxtNombreAsesor.Foreground = (Brush)Application.Current.FindResource("ColorTextoBloqueado");

            TxtCorreoAsesor.Background = (Brush)Application.Current.FindResource("ColorFondoBloqueado");
            TxtCorreoAsesor.Foreground = (Brush)Application.Current.FindResource("ColorTextoBloqueado");

            TxtTelefonoAsesor.Background = (Brush)Application.Current.FindResource("ColorFondoBloqueado");
            TxtTelefonoAsesor.Foreground = (Brush)Application.Current.FindResource("ColorTextoBloqueado");

            cmbPeriodoFiscal.Background = (Brush)Application.Current.FindResource("ColorFondoBloqueado");
            cmbPeriodoFiscal.Foreground = (Brush)Application.Current.FindResource("ColorTextoBloqueado");

            cmbTrimestre.Background = (Brush)Application.Current.FindResource("ColorFondoBloqueado");
            cmbTrimestre.Foreground = (Brush)Application.Current.FindResource("ColorTextoBloqueado");
        }

        #endregion

        #region Permite descargar el archivo al darle clic
        private void TxtNombreArchivoRespuesta_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock && textBlock.Tag is ArchivoAdjunto archivo)
            {
                var dialogo = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = archivo.Nombre,
                    Filter = "Todos los archivos (*.*)|*.*",
                    Title = "Guardar archivo de respuesta del administrador"
                };

                if (dialogo.ShowDialog() == true)
                {
                    File.WriteAllBytes(dialogo.FileName, archivo.Contenido);
                }
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
            TextoUltimaConexion.Visibility = Visibility.Collapsed;

            barraExpandida = false;
        }
        #endregion

        #region MenuAboutUs
        private void AboutUs_Click(object sender, RoutedEventArgs e)
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
                        NavigationService.Navigate(new AboutUsViewUser());
                        break;
                    case "administrador":
                        NavigationService.Navigate(new AboutUsViewAdmin());
                        break;
                    case "superadministrador":
                        NavigationService.Navigate(new AboutUsViewSuperAdmin());
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

        #region MenuAyuda
        private void Help_Click(object sender, RoutedEventArgs e)
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
                        NavigationService.Navigate(new FAQViewUser());
                        break;
                    case "administrador":
                        NavigationService.Navigate(new FAQViewAdmin());
                        break;
                    case "superadministrador":
                        NavigationService.Navigate(new FAQViewSuperAdmin());
                        break;
                }
            }
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

        #region MenuIdioma
        private void Lenguage_Click(object sender, RoutedEventArgs e)
        {
            DisplayAlertLenguage displayAlert = new DisplayAlertLenguage();
            displayAlert.Owner = Window.GetWindow(this); // Esto hace que se centre en esta ventana
            displayAlert.ShowDialog();
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

        #region Eventos del menu izquierdo para mostrar solicitudes
        private void VerTodasSolicitudes(object sender, MouseButtonEventArgs e)
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
            NavigationService.Navigate(new UserManageRequests("todos"));
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

        private void VerPendientesSolicitudes(object sender, MouseButtonEventArgs e)
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
                        NavigationService.Navigate(new UserManageRequests("pendiente"));
                        break;
                    case "administrador":
                        NavigationService.Navigate(new AdminMainView("pendiente"));
                        break;
                    case "superadministrador":
                        NavigationService.Navigate(new SuperAdminMainView("pendiente"));
                        break;
                }
            }

        }

        private void VerEnProcesoSolicitudes(object sender, MouseButtonEventArgs e)
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
                        NavigationService.Navigate(new UserManageRequests("en_proceso"));
                        break;
                    case "administrador":
                        NavigationService.Navigate(new AdminMainView("en_proceso"));
                        break;
                    case "superadministrador":
                        NavigationService.Navigate(new SuperAdminMainView("en_proceso"));
                        break;
                }
            }

        }

        private void VerFinalizadasSolicitudes(object sender, MouseButtonEventArgs e)
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
                        NavigationService.Navigate(new UserManageRequests("finalizada"));
                        break;
                    case "administrador":
                        NavigationService.Navigate(new AdminMainView("finalizada"));
                        break;
                    case "superadministrador":
                        NavigationService.Navigate(new SuperAdminMainView("finalizada"));
                        break;
                }
            }

        }
        #endregion

        #region nombre de solo letras
        private void SoloLetras_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsLetter);
        }
        #endregion

        #region telefono de solo numeros
        private void SoloNumeros_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(char.IsDigit);
        }
        #endregion
    }
}
