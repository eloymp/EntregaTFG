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

namespace ProyectoFinalEMP.Views.SuperAdmin
{
    public partial class AddUserView : Window
    {
        private byte[] imagenPerfilBytes;
        public AddUserView()
        {
            InitializeComponent();

        }
        #region Cambiar foto de perfil
        private void ChangeProfileImage(object sender, MouseButtonEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Imágenes (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                Title = "Seleccionar foto de perfil"
            };

            if (openFileDialog.ShowDialog() != true) return;

            try
            {                var image = new BitmapImage(new Uri(openFileDialog.FileName));
                imgFotoPerfil.ImageSource = image;

                imagenPerfilBytes = File.ReadAllBytes(openFileDialog.FileName);
            }
            catch (Exception)
            {
                MessageBox.Show((string)Application.Current.FindResource("DisplayAlertImageChangeError"));
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

        #region Boton crear usuario
        private async void CrearUsuario_Click(object sender, RoutedEventArgs e)
        {
            string email = EntryEmail.Text.Trim();
            string nombre = EntryName.Text;
            string apellidos = EntryLastName.Text;
            string nif = EntryNIF.Text;
            string telefono = EntryPhone.Text;
            string direccion = EntryAddress.Text;
            string tipoCliente="autonomo";
            if (LstTipoCliente.SelectedItem is ComboBoxItem itemTipo)
            {
                tipoCliente = itemTipo.Content.ToString().ToLower();
            }

            string rol = "usuario";
            if (LstRolCliente.SelectedItem is ComboBoxItem itemRol)
            {
                rol = itemRol.Content.ToString().ToLower();
            }

            string nombreEmpresa = EntryCompanyName.Text;
            string descripcionEmpresa = EntryCompanyDescription.Text;
            string password = NewPassword.Password;
            string repeatPassword = RepeatNewPassword.Password;

            if (!EsCorreoValido(EntryEmail.Text))
            {

                MessageBox.Show((string)Application.Current.FindResource("FormatoCorreo"));
                return;
            }

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || password != repeatPassword)
            {
                MessageBox.Show((string)Application.Current.FindResource("MessageBoxRepeatPassword")
);
                return;
            }

            var hash = GlobalData.Instance.administradorHash.GenerarHash(password);

            if (await GlobalData.Instance.miBBDD.ExisteUsuario(email))
            {
                MessageBox.Show((string)Application.Current.FindResource("MessageBoxUserDuplicated")
);
                return;
            }

            var nuevoUsuario = new BsonDocument{
                { "email", email },
                { "nombre", nombre },
                { "apellidos", apellidos },
                { "nif", nif },
                { "teléfono", telefono },
                { "dirección", direccion },
                { "rol", rol },
                { "tipo_cliente", tipoCliente },
                { "estado_cuenta", "activa" },
                { "nombre_empresa", nombreEmpresa },
                { "descripcion_empresa", descripcionEmpresa },
                { "contraseña_hasheada", hash },
                { "foto_perfil", imagenPerfilBytes != null
                ? new BsonBinaryData(imagenPerfilBytes)
                    : BsonNull.Value
                }
            };


            

            await GlobalData.Instance.miBBDD.InsertarUsuario(nuevoUsuario);

            MessageBox.Show((string)Application.Current.FindResource("MessageBoxUserCreatedOk")
);
            this.Close();
        }
        #endregion

        private bool EsCorreoValido(string correo)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(correo,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
    }
}
