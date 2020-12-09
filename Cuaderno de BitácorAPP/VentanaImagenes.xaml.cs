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

namespace Cuaderno_de_BitácorAPP
{
    /// <summary>
    /// Lógica de interacción para VentanaImagenes.xaml
    /// </summary>
    public partial class VentanaImagenes : Window
    {
        private Album album;

        public VentanaImagenes(Album album)
        {
            InitializeComponent();
            this.album = album;
            ListBoxForPhotos.ItemsSource = album.Imagenes;
        }

        /// <summary>
        /// Evento que se lanza al eliminar una foto. Pregunta si realmente se quiere elminar la foto, y si es así, la elimina y actualiza el listBox que las contiene
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBindingDeletePhoto_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int indiceABorrar = ListBoxForPhotos.SelectedIndex;
            MessageBoxResult resultado;

            if (indiceABorrar >= 0)
            {
                resultado = MessageBox.Show("¿Quieres eliminar esta foto?", "¿Seguro?",
                                                     MessageBoxButton.YesNo,
                                                     MessageBoxImage.Warning,
                                                     MessageBoxResult.Yes);
                if (resultado == MessageBoxResult.Yes)
                {
                    album.Imagenes.RemoveAt(indiceABorrar);
                    actualizarListBoxFotos();
                }
            }
            if (album.Imagenes.Count == 0)
            {
                this.Close();
            }
        }

        /// <summary>
        /// Evento que se lanza cuando se quiere eliminar una foto. Establece si se puede o no eliminar la foto (siempre es true)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBindingDeletePhoto_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Axtualiza el elemento ListBocForPhotos que contiene las imagenes
        /// </summary>
        private void actualizarListBoxFotos()
        {
            ListBoxForPhotos.ItemsSource = album.Imagenes;
            ListBoxForPhotos.Items.Refresh();
        }

        /// <summary>
        /// Evento que se lanza al presionar el botón izquierdo del ratón sobre una imagen. Se abre la ventana de visualizacion de la imagen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            VisualizadorImagenes visualizaImagenes = new VisualizadorImagenes(album, ListBoxForPhotos.SelectedIndex);
            visualizaImagenes.ShowDialog();
        }
    }
}
