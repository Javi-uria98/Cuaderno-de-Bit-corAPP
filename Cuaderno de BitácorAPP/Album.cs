using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cuaderno_de_BitácorAPP
{
    public class Album
    {
        public Album()
        {
            this.Imagenes = new ObservableCollection<Imagen>();
        }

        public string Nombre { get; set; }

        public ObservableCollection<Imagen> Imagenes { get; set; }
    }
}

  

