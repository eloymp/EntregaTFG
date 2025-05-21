using ProyectoFinalEMP.Singleton;
using System;
using System.Collections.Generic;
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

namespace ProyectoFinalEMP.Views.DisplayAlerts
{
    public partial class DisplayAlertLenguage : Window
    {
        public DisplayAlertLenguage()
        {
            InitializeComponent();
        }

        #region Boton tema claro
        private async void BtnEspañol_Click(object sender, RoutedEventArgs e)
        {
            AplicarIdioma("Resources/Lenguages/Spanish.xaml");
            await GlobalData.Instance.miBBDD.ActualizarIdiomaUsuario(
                GlobalData.Instance.UsuarioLogueado["_id"].AsObjectId,
                "español"
            );
            this.Close();
        }
        #endregion

        #region Boton tema oscuro
        private async void BtnIngles_Click(object sender, RoutedEventArgs e)
        {
            AplicarIdioma("Resources/Lenguages/English.xaml");
            await GlobalData.Instance.miBBDD.ActualizarIdiomaUsuario(
                GlobalData.Instance.UsuarioLogueado["_id"].AsObjectId,
                "ingles"
            );
            this.Close();
        }
        #endregion

        #region Metodo aplicar tema
        private void AplicarIdioma(string ruta)
        {
            
            var diccionario = new ResourceDictionary
            {
                Source = new Uri(ruta, UriKind.Relative)
            };

            // Eliminar solo el tema actual, no las fuentes ni los idiomas
            var temasExistentes = Application.Current.Resources.MergedDictionaries
                .Where(d => d.Source != null &&
                        (d.Source.OriginalString.Contains("Spanish.xaml") ||
                        d.Source.OriginalString.Contains("English.xaml")))
                .ToList();

            foreach (var tema in temasExistentes)
            {
                Application.Current.Resources.MergedDictionaries.Remove(tema);
            }

            // Añadir el nuevo recurso de diccionario
            Application.Current.Resources.MergedDictionaries.Add(diccionario);
        }
        #endregion



    }
}
