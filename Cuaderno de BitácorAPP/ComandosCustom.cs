using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Cuaderno_de_BitácorAPP
{
    public static class ComandosCustom
    {
        public static readonly RoutedUICommand AddToAlbum = new RoutedUICommand("Añadir", "AddToAlbum", typeof(ComandosCustom), new InputGestureCollection()
                                                                                {
                                                                                    new KeyGesture(Key.O, ModifierKeys.Control)
                                                                                });

        public static readonly RoutedUICommand RestoreImageZoom = new RoutedUICommand("Restaurar Zoom", "RestoreImageZoom", typeof(ComandosCustom), new InputGestureCollection()
                                                                                       {
                                                                                           new KeyGesture(Key.R, ModifierKeys.Control)
                                                                                       });

        public static readonly RoutedUICommand AddDescription = new RoutedUICommand("Mostrar Descripcion", "AddDescription", typeof(ComandosCustom), new InputGestureCollection()
                                                                             {
                                                                                    new KeyGesture(Key.T, ModifierKeys.Control)
                                                                             });
    }
}
