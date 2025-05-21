using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using ProyectoFinalEMP.Controller;
using ProyectoFinalEMP.Cryptography;
using System.Windows.Controls;

namespace ProyectoFinalEMP.Singleton
{
    public class GlobalData
    {
        private static GlobalData _instance;
        public static GlobalData Instance => _instance ??= new GlobalData();
        private GlobalData() { }


        //Conexion con la bd
        public DatabaseConnection miBBDD = new DatabaseConnection();

        //Conexion con la clase que permite crear hashes
        public HashAdministrator administradorHash = new HashAdministrator();

        //Pagina login para mostrar siempre la misma y que no se borren los datos
        public Page Login;

        //Documento BSON con la info del usuario
        public BsonDocument UsuarioLogueado { get; set; }
    }
}
