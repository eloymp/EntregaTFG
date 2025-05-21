using System.Configuration;
using System.Data;
using System.Windows;
using ProyectoFinalEMP.Singleton;
using ProyectoFinalEMP.Views;

namespace ProyectoFinalEMP
{
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);// No quitar esto

            bool conectado = GlobalData.Instance.miBBDD.Conectar();
            if (!conectado)
            {
                MessageBox.Show((string)Application.Current.FindResource("ConectionFailed"));
                Current.Shutdown(); // Cierra la app si falla la conexión
            }
        }
    }

}
