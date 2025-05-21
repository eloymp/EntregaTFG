using MongoDB.Bson;
using MongoDB.Driver;
using ProyectoFinalEMP.Singleton;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProyectoFinalEMP.Views.SuperAdmin
{
    public partial class ModifyUserAdminView : Window
    {
        private string originalEmail;
        private string originalNombre;
        private string originalApellidos;
        private string originalNif;
        private string originalTelefono;
        private string originalDireccion;
        private string originalNombreEmpresa;
        private string originalDescripcionEmpresa;
        private string originalEstadoCuenta;
        private string originalTipoCliente;
        private string originalRolCliente;

        BsonDocument usuarioDoc;
        public ModifyUserAdminView(BsonDocument doc)
        {
            InitializeComponent();

            #region Cargar datos del usuario
            usuarioDoc = doc;

            if (doc.Contains("email"))
                EntryEmail.Text = doc["email"].AsString;

            if (doc.Contains("nombre"))
                EntryName.Text = doc["nombre"].AsString;

            if (doc.Contains("apellidos"))
                EntryLastName.Text = doc["apellidos"].AsString;

            if (doc.Contains("nif"))
                EntryNIF.Text = doc["nif"].AsString;

            if (doc.Contains("teléfono"))
                EntryPhone.Text = doc["teléfono"].AsString;

            if (doc.Contains("dirección"))
                EntryAddress.Text = doc["dirección"].AsString;

            if (doc.Contains("estado_cuenta"))
            {
                string estado = doc["estado_cuenta"].AsString.ToLower();
                foreach (ComboBoxItem item in LstEstadoCuenta.Items)
                {
                    if ((item.Tag as string)?.ToLower() == estado)
                    {
                        LstEstadoCuenta.SelectedItem = item;
                        break;
                    }
                }
            }

            if (doc.Contains("tipo_cliente"))
            {
                string tipo = doc["tipo_cliente"].AsString.ToLower();
                foreach (ComboBoxItem item in LstTipoCliente.Items)
                {
                    if ((item.Tag as string)?.ToLower() == tipo)
                    {
                        LstTipoCliente.SelectedItem = item;
                        break;
                    }
                }
            }

            if (doc.Contains("rol"))
            {
                string tipo = doc["rol"].AsString.ToLower();
                foreach (ComboBoxItem item in LstRolCliente.Items)
                {
                    if ((item.Tag as string)?.ToLower() == tipo)
                    {
                        LstRolCliente.SelectedItem = item;
                        break;
                    }
                }
            }

            if (doc.Contains("nombre_empresa"))
                EntryCompanyName.Text = doc["nombre_empresa"].AsString;

            if (doc.Contains("descripcion_empresa"))
                EntryCompanyDescription.Text = doc["descripcion_empresa"].AsString;

            if (doc.Contains("foto_perfil") && !doc["foto_perfil"].IsBsonNull)
            {
                var imagenBytes = doc["foto_perfil"].AsBsonBinaryData.Bytes;
                using (var stream = new MemoryStream(imagenBytes))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.StreamSource = stream;
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    imgFotoPerfil.ImageSource = image;
                }
            }
            #endregion
        }

        #region Lapiz editar campos
        private void EditarCampos_Click(object sender, MouseButtonEventArgs e)
        {
            //Guardar los valores originales
            originalEmail = EntryEmail.Text;
            originalNombre =EntryName.Text;
            originalApellidos=EntryLastName.Text;
            originalNif=EntryNIF.Text;
            originalTelefono=EntryPhone.Text;
            originalDireccion=EntryAddress.Text;
            originalNombreEmpresa=EntryCompanyName.Text;
            originalDescripcionEmpresa=EntryCompanyDescription.Text;
            originalTipoCliente = LstTipoCliente.Text;
            originalEstadoCuenta = LstEstadoCuenta.Text;
            originalRolCliente=LstRolCliente.Text;

            //Activar los campos editables
            EntryEmail.IsReadOnly = false;
            EntryName.IsReadOnly = false;
            EntryLastName.IsReadOnly = false;
            EntryNIF.IsReadOnly = false;
            EntryPhone.IsReadOnly = false;
            EntryAddress.IsReadOnly = false;
            EntryCompanyName.IsReadOnly=false;
            EntryCompanyDescription.IsReadOnly = false;
            LstTipoCliente.IsEnabled = true;
            LstEstadoCuenta.IsEnabled = true;
            LstRolCliente.IsEnabled = true;

            EntryEmail.Background = Brushes.White;
            EntryName.Background = Brushes.White;
            EntryLastName.Background = Brushes.White;
            EntryNIF.Background = Brushes.White;
            EntryPhone.Background = Brushes.White;
            EntryAddress.Background = Brushes.White;
            EntryCompanyName.Background = Brushes.White;
            EntryCompanyDescription.Background = Brushes.White;
            LstTipoCliente.Background = Brushes.White;
            LstEstadoCuenta.Background = Brushes.White;
            LstRolCliente.Background = Brushes.White;

            // Mostrar iconos check y uncheck
            imgEditar.Visibility = Visibility.Collapsed;
            imgGuardar.Visibility = Visibility.Visible;
            imgCancelar.Visibility = Visibility.Visible;
        }
        #endregion

        #region Tick guardar cambios
        private async void GuardarCambios_Click(object sender, MouseButtonEventArgs e)
        {
            string email = usuarioDoc["email"].AsString;

            usuarioDoc["email"] = EntryEmail.Text;
            usuarioDoc["nombre"] = EntryName.Text;
            usuarioDoc["apellidos"] = EntryLastName.Text;
            usuarioDoc["nif"]=EntryNIF.Text;
            usuarioDoc["teléfono"] = EntryPhone.Text;
            usuarioDoc["dirección"] = EntryAddress.Text;
            usuarioDoc["nombre_empresa"] = EntryCompanyName.Text;
            usuarioDoc["descripcion_empresa"] = EntryCompanyDescription.Text;

            if (LstEstadoCuenta.SelectedItem is ComboBoxItem itemEstado && itemEstado.Tag is string estadoTag)
            {
                usuarioDoc["estado_cuenta"] = estadoTag;
            }

            if (LstTipoCliente.SelectedItem is ComboBoxItem itemTipo && itemTipo.Tag is string tipoTag)
            {
                usuarioDoc["tipo_cliente"] = tipoTag;
            }

            if (LstRolCliente.SelectedItem is ComboBoxItem itemRol && itemRol.Tag is string rolTag)
            {
                usuarioDoc["rol"] = rolTag;
            }

            var actualizacion = Builders<BsonDocument>.Update
                .Set("email", EntryEmail.Text)
                .Set("nombre", EntryName.Text)
                .Set("apellidos", EntryLastName.Text)
                .Set("nif", EntryNIF.Text)
                .Set("teléfono", EntryPhone.Text)
                .Set("dirección", EntryAddress.Text)
                .Set("nombre_empresa", EntryCompanyName.Text)
                .Set("descripcion_empresa", EntryCompanyDescription.Text)
                .Set("estado_cuenta", usuarioDoc["estado_cuenta"])
                .Set("tipo_cliente", usuarioDoc["tipo_cliente"])
                .Set("rol", usuarioDoc["rol"]);



            await GlobalData.Instance.miBBDD.ActualizarUsuario(email, actualizacion);

            MessageBox.Show((string)Application.Current.FindResource("DisplayAlertChangesOk"));

            //Devolver true para que se actualice la tabla
            this.DialogResult = true;


            DesactivarEdicion();

        }
        #endregion

        #region Cross cancelar cambios
        private void CancelarCambios_Click(object sender, MouseButtonEventArgs e)
        {
            EntryName.Text = originalNombre;
            EntryLastName.Text = originalApellidos;
            EntryNIF.Text = originalNif;
            EntryPhone.Text = originalTelefono;
            EntryAddress.Text = originalDireccion;
            EntryCompanyName.Text = originalNombreEmpresa;
            EntryCompanyDescription.Text = originalDescripcionEmpresa;

            foreach (ComboBoxItem item in LstEstadoCuenta.Items)
            {
                if ((item.Tag as string) == originalEstadoCuenta)
                {
                    LstEstadoCuenta.SelectedItem = item;
                    break;
                }
            }

            foreach (ComboBoxItem item in LstTipoCliente.Items)
            {
                if ((item.Tag as string) == originalTipoCliente)
                {
                    LstTipoCliente.SelectedItem = item;
                    break;
                }
            }

            MessageBox.Show((string)Application.Current.FindResource("DisplayAlertDiscardChanges"));

            DesactivarEdicion();
        }
        #endregion

        #region Metodo para desactivar la edicion de las entry
        private void DesactivarEdicion()
        {
            EntryEmail.IsReadOnly = true;
            EntryName.IsReadOnly = true;
            EntryLastName.IsReadOnly = true;
            EntryNIF.IsReadOnly = true;
            EntryPhone.IsReadOnly = true;
            EntryAddress.IsReadOnly = true;
            EntryCompanyName.IsReadOnly = true;
            EntryCompanyDescription.IsReadOnly = true;
            LstTipoCliente.IsEnabled = false;
            LstEstadoCuenta.IsEnabled = false;


            //Volver a poner el estilo de no editable
            EntryEmail.ClearValue(TextBox.BackgroundProperty);
            EntryEmail.ClearValue(TextBox.BorderBrushProperty);

            EntryName.ClearValue(TextBox.BackgroundProperty);
            EntryName.ClearValue(TextBox.BorderBrushProperty);

            EntryLastName.ClearValue(TextBox.BackgroundProperty);
            EntryLastName.ClearValue(TextBox.BorderBrushProperty);

            EntryNIF.ClearValue(TextBox.BackgroundProperty);
            EntryNIF.ClearValue(TextBox.BorderBrushProperty);

            EntryPhone.ClearValue(TextBox.BackgroundProperty);
            EntryPhone.ClearValue(TextBox.BorderBrushProperty);

            EntryAddress.ClearValue(TextBox.BackgroundProperty);
            EntryAddress.ClearValue(TextBox.BorderBrushProperty);

            EntryCompanyName.ClearValue(TextBox.BackgroundProperty);
            EntryCompanyName.ClearValue(TextBox.BorderBrushProperty);

            EntryCompanyDescription.ClearValue(TextBox.BackgroundProperty);
            EntryCompanyDescription.ClearValue(TextBox.BorderBrushProperty);

            LstEstadoCuenta.ClearValue(TextBox.BackgroundProperty);
            LstEstadoCuenta.ClearValue(TextBox.BorderBrushProperty);

            LstTipoCliente.ClearValue(TextBox.BackgroundProperty);
            LstTipoCliente.ClearValue(TextBox.BorderBrushProperty);

            //Mostrar el lapiz
            imgEditar.Visibility = Visibility.Visible;
            imgGuardar.Visibility = Visibility.Collapsed;
            imgCancelar.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region Cambiar foto de perfil
        private async void ChangeProfileImage(object sender, MouseButtonEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Imágenes (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                Title = "Seleccionar nueva foto de perfil"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    // Cargar imagen visualmente
                    var image = new BitmapImage(new Uri(openFileDialog.FileName));
                    imgFotoPerfil.ImageSource = image;

                    // Leer bytes
                    byte[] imagenBytes = File.ReadAllBytes(openFileDialog.FileName);

                    // Actualizar en memoria
                    GlobalData.Instance.UsuarioLogueado["foto_perfil"] = new BsonBinaryData(imagenBytes);

                    // Guardar en la base de datos
                    string emailUsuario = usuarioDoc["email"].AsString;
                    await GlobalData.Instance.miBBDD.ActualizarFotoPerfil(emailUsuario, imagenBytes);

                    MessageBox.Show((string)Application.Current.FindResource("DisplayAlertImageChange"));
                }
                catch (Exception ex)
                {
                    MessageBox.Show((string)Application.Current.FindResource("DisplayAlertImageChangeError"));
                }
            }
        }
        #endregion

        #region Solo permitir numeros en el telefono
        private void EntryPhone_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Solo permite números
            e.Handled = !e.Text.All(char.IsDigit);
        }
        #endregion

        #region Boton cambiar contraseña
        private void ChangeProfilePassword(object sender, MouseButtonEventArgs e)
        {
            PanelCambiarContraseña.Visibility = Visibility.Visible;
            imgEditar.Visibility = Visibility.Collapsed;

        }
        #endregion

        #region Boton guardar contraseña nueva
        private async void BtnGuardarContraseña_Click(object sender, RoutedEventArgs e)
        {
            var usuario = usuarioDoc;
            string email = usuario["email"].AsString;

            string nueva = NewPassword.Password;
            string repetir = RepeatNewPassword.Password;

            // Comprobar que los 3 campos estén rellenados
            if (string.IsNullOrWhiteSpace(nueva) || string.IsNullOrWhiteSpace(repetir))
            {
                MessageBox.Show((string)Application.Current.FindResource("DisplayAlertObligatori"));
                return;
            }

            // Validar que las nuevas coincidan
            if (nueva != repetir)
            {
                MessageBox.Show((string)Application.Current.FindResource("DisplayAlertNewPasswords"));
                return;
            }

            usuario["contraseña_hasheada"] = GlobalData.Instance.administradorHash.GenerarHash(RepeatNewPassword.Password);

            var actualizacion = Builders<BsonDocument>.Update
                .Set("contraseña_hasheada", GlobalData.Instance.administradorHash.GenerarHash(RepeatNewPassword.Password));


            await GlobalData.Instance.miBBDD.ActualizarUsuario(email, actualizacion);

            PanelCambiarContraseña.Visibility = Visibility.Collapsed;


            MessageBox.Show((string)Application.Current.FindResource("DisplayAlertChangesOk"));

            imgEditar.Visibility = Visibility.Visible;


        }
        #endregion

        #region No permitir numeros en el nombre y apellidos
        private void SoloLetras_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Acepta letras, espacios, tildes y ñ/Ñ
            e.Handled = !e.Text.All(c =>
                char.IsLetter(c) ||
                c == ' ' ||
                c == 'á' || c == 'é' || c == 'í' || c == 'ó' || c == 'ú' ||
                c == 'Á' || c == 'É' || c == 'Í' || c == 'Ó' || c == 'Ú' ||
                c == 'ñ' || c == 'Ñ'
            );
        }
        #endregion

        #region Boton borrar cuenta
        private async void DeleteAccount(object sender, MouseButtonEventArgs e)
        {
            var result = MessageBox.Show(
                (string)Application.Current.FindResource("DisplayAlertDeleteAccountConfirm"),
                (string)Application.Current.FindResource("DisplayAlertDeleteTitle"),
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                string email = usuarioDoc["email"].AsString;

                await GlobalData.Instance.miBBDD.EliminarUsuario(email);

                MessageBox.Show((string)Application.Current.FindResource("DisplayAlertAccountDeleted"));

                this.DialogResult = true;

                this.Close(); 
            }
        }
        #endregion
    }
}
