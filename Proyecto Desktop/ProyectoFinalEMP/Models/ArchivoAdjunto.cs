using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoFinalEMP.Models
{
    public class ArchivoAdjunto : INotifyPropertyChanged
    {
        private string _comentario;

        public string Nombre { get; set; }
        public byte[] Contenido { get; set; }


        //Cuando cambia la propiedas se comunica con donde se llama
        public string Comentario
        {
            get => _comentario;
            set
            {
                _comentario = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Comentario)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
