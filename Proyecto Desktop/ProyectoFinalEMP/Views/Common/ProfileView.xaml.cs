using MongoDB.Bson;
using MongoDB.Driver;
using ProyectoFinalEMP.Singleton;
using ProyectoFinalEMP.Views.Unregistered;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace ProyectoFinalEMP.Views.Common
{
    public partial class ProfileView : Window
    {
        private string originalNombre;
        private string originalApellidos;
        private string originalNif;
        private string originalTelefono;
        private string originalDireccion;
        private string originalNombreEmpresa;
        private string originalDescripcionEmpresa;

        public ProfileView()
        {
            InitializeComponent();

            #region Cargar datos del usuario
            var usuario = GlobalData.Instance.UsuarioLogueado;

            if (usuario.Contains("email"))
                EntryEmail.Text = usuario["email"].AsString;

            if (usuario.Contains("nombre"))
                EntryName.Text = usuario["nombre"].AsString;

            if (usuario.Contains("apellidos"))
                EntryLastName.Text = usuario["apellidos"].AsString;

            if (usuario.Contains("nif"))
                EntryNIF.Text = usuario["nif"].AsString;

            if (usuario.Contains("teléfono"))
                EntryPhone.Text = usuario["teléfono"].AsString;

            if (usuario.Contains("dirección"))
                EntryAddress.Text = usuario["dirección"].AsString;

            if (usuario.Contains("rol"))
                EntryRol.Text = usuario["rol"].AsString;

            if (usuario.Contains("tipo_cliente"))
                EntryClientType.Text = usuario["tipo_cliente"].AsString;

            if (usuario.Contains("nombre_empresa"))
                EntryCompanyName.Text = usuario["nombre_empresa"].AsString;

            if (usuario.Contains("descripcion_empresa"))
                EntryCompanyDescription.Text = usuario["descripcion_empresa"].AsString;

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
                    imgFotoPerfil.ImageSource = image;
                }
            }
            #endregion
        }

        #region Lapiz editar campos
        private void EditarCampos_Click(object sender, MouseButtonEventArgs e)
        {
            //Guardar los valores originales
            originalNombre=EntryName.Text;
            originalApellidos=EntryLastName.Text;
            originalNif=EntryNIF.Text;
            originalTelefono=EntryPhone.Text;
            originalDireccion=EntryAddress.Text;
            originalNombreEmpresa=EntryCompanyName.Text;
            originalDescripcionEmpresa=EntryCompanyDescription.Text;

            //Activar los campos editables
            EntryName.IsReadOnly = false;
            EntryLastName.IsReadOnly = false;
            EntryNIF.IsReadOnly = false;
            EntryPhone.IsReadOnly = false;
            EntryAddress.IsReadOnly = false;
            EntryCompanyName.IsReadOnly=false;
            EntryCompanyDescription.IsReadOnly = false;


            EntryName.Background = Brushes.White;
            EntryLastName.Background = Brushes.White;
            EntryNIF.Background = Brushes.White;
            EntryPhone.Background = Brushes.White;
            EntryAddress.Background = Brushes.White;
            EntryCompanyName.Background = Brushes.White;
            EntryCompanyDescription.Background = Brushes.White;

            // Mostrar iconos check y uncheck
            imgEditar.Visibility = Visibility.Collapsed;
            imgGuardar.Visibility = Visibility.Visible;
            imgCancelar.Visibility = Visibility.Visible;
        }
        #endregion

        #region Tick guardar cambios
        private async void GuardarCambios_Click(object sender, MouseButtonEventArgs e)
        {
            var usuario = GlobalData.Instance.UsuarioLogueado;
            string email = usuario["email"].AsString;

            usuario["nombre"] = EntryName.Text;
            usuario["apellidos"] = EntryLastName.Text;
            usuario["nif"]=EntryNIF.Text;
            usuario["teléfono"] = EntryPhone.Text;
            usuario["dirección"] = EntryAddress.Text;
            usuario["nombre_empresa"] = EntryCompanyName.Text;
            usuario["descripcion_empresa"] = EntryCompanyDescription.Text;

            var actualizacion = Builders<BsonDocument>.Update
                .Set("nombre", EntryName.Text)
                .Set("apellidos", EntryLastName.Text)
                .Set("nif", EntryNIF.Text)
                .Set("teléfono", EntryPhone.Text)
                .Set("dirección", EntryAddress.Text)
                .Set("nombre_empresa", EntryCompanyName.Text)
                .Set("descripcion_empresa", EntryCompanyDescription.Text);


            await GlobalData.Instance.miBBDD.ActualizarUsuario(email, actualizacion);

            MessageBox.Show((string)Application.Current.FindResource("DisplayAlertChangesOk"));

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

            MessageBox.Show((string)Application.Current.FindResource("DisplayAlertDiscardChanges"));

            DesactivarEdicion();
        }
        #endregion

        #region Metodo para desactivar la edicion de las entry
        private void DesactivarEdicion()
        {
            EntryName.IsReadOnly = true;
            EntryLastName.IsReadOnly = true;
            EntryNIF.IsReadOnly = true;
            EntryPhone.IsReadOnly = true;
            EntryAddress.IsReadOnly = true;
            EntryCompanyName.IsReadOnly = true;
            EntryCompanyDescription.IsReadOnly = true;

            //Volver a poner el estilo de no editable
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
                    string emailUsuario = GlobalData.Instance.UsuarioLogueado["email"].AsString;
                    await GlobalData.Instance.miBBDD.ActualizarFotoPerfil(emailUsuario, imagenBytes);

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
            var usuario = GlobalData.Instance.UsuarioLogueado;
            string email = usuario["email"].AsString;



            string actual = ActualPassword.Password;
            string nueva = NewPassword.Password;
            string repetir = RepeatNewPassword.Password;

            // Comprobar que los 3 campos estén rellenados
            if (string.IsNullOrWhiteSpace(actual) || string.IsNullOrWhiteSpace(nueva) || string.IsNullOrWhiteSpace(repetir))
            {
                MessageBox.Show((string)Application.Current.FindResource("DisplayAlertObligatori"));
                return;
            }

            // Validar que la actual coincide con la guardada (en hash)
            string hashActual = GlobalData.Instance.administradorHash.GenerarHash(actual);

            if (hashActual != usuario["contraseña_hasheada"].AsString)
            {
                MessageBox.Show((string)Application.Current.FindResource("DisplayAlertActualPassword"));

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

    }
}
