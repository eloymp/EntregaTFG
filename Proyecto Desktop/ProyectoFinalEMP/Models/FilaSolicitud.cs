using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinalEMP.Models
{
    public class FilaSolicitud
    {
        public BsonDocument Documento { get; set; }
        public string TipoServicio { get; set; }
        public string Subtipo { get; set; }
        public string Estado { get; set; }
        public string DescripcionCorta { get; set; }
        public string EmailUsuario { get; set; }
        public string FechaCreacion { get; set; }

    }
}
