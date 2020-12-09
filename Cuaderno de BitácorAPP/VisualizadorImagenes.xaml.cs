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
    /// Lógica de interacción para VisualizadorImagenes.xaml
    /// </summary>
    public partial class VisualizadorImagenes : Window
    {

        private Album album;
        private int indice;
        private Point origen;
        private Point inicio;
        private int contadorRuedaRatonArriba = 0;
        private int contadorRuedaRatonAbajo = 0;

        public VisualizadorImagenes(Album album, int indice)
        {
            InitializeComponent();
            this.album = album;
            this.indice = indice;
            BitmapImage imagenBitmapSeleccionada = new BitmapImage(new Uri(album.Imagenes[indice].RutaImagen));
            slotForImage.Source = imagenBitmapSeleccionada;
            textBlockDescription.Text = album.Imagenes[indice].RutaImagen;
        }

        /// <summary>
        /// Evento que se lanza al presionar el boton izquierdo del raton sobre la flecha izquierda. Muestra la imagen anterior del album
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LeftPolygon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (indice > 0)
            {
                indice--;
            }
            else
            {
                indice = album.Imagenes.Count - 1;
            }
            BitmapImage imagenBitmapSeleccionada = new BitmapImage(new Uri(album.Imagenes[indice].RutaImagen));
            slotForImage.Source = imagenBitmapSeleccionada;
            textBlockDescription.Text = album.Imagenes[indice].RutaImagen;
        }

        /// <summary>
        /// Evento que se lanza al presionar el boton izquierdo del raton sobre la flecha derecha. Muestra la imagen siguiente del album
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RightPolygon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (indice < album.Imagenes.Count - 1)
            {
                indice++;
            }
            else
            {
                indice = 0;
            }
            BitmapImage imagenBitmapSeleccionada = new BitmapImage(new Uri(album.Imagenes[indice].RutaImagen));
            slotForImage.Source = imagenBitmapSeleccionada;
            textBlockDescription.Text = album.Imagenes[indice].RutaImagen;
        }

        /// <summary>
        /// Evento que se lanza cuando se mueve la rueda del ratón sobre la imagen. Hace zoom hacia afuera o hacia dentro en función de si se movió la rueda hacia abajo o hacia arriba
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void slotForImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point coordenada = e.MouseDevice.GetPosition(slotForImage);

            Matrix matriz = slotForImage.RenderTransform.Value;
            if (e.Delta > 0)
            {
                matriz.ScaleAtPrepend(1.1, 1.1, coordenada.X, coordenada.Y);
                contadorRuedaRatonArriba++;
            }
            else
            {
                matriz.ScaleAtPrepend(1.0 / 1.1, 1.0 / 1.1, coordenada.X, coordenada.Y);
                contadorRuedaRatonAbajo++;
            }

            slotForImage.RenderTransform = new MatrixTransform(matriz);
        }


        /// <summary>
        /// Evento que se lanza cuando se presiona el botón izquierdo del ratón sobre la imagen. Se captura la imagen para poder arrastrarla en caso de que se mueva el ratón mientras se siga presionando el bótón
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void slotForImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (slotForImage.IsMouseCaptured) return;
            slotForImage.CaptureMouse();

            inicio = e.GetPosition(borderForSlotForImage);
            origen.X = slotForImage.RenderTransform.Value.OffsetX;
            origen.Y = slotForImage.RenderTransform.Value.OffsetY;
        }

        /// <summary>
        /// Evento que se lanza cuando, teniendo una imagen presionada con el botón izquierdo, se suelta dicho botón. Se deja de capturar la imagen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void slotForImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            slotForImage.ReleaseMouseCapture();
        }

        /// <summary>
        /// Evento que se lanza cuando se mueve el ratón. Arrastra la imagen hacia la dirección a donde se mueva el ratón
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void slotForImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (!slotForImage.IsMouseCaptured) return;

            Point coordenada = e.MouseDevice.GetPosition(borderForSlotForImage);
            Matrix matriz = slotForImage.RenderTransform.Value;

            matriz.OffsetX = origen.X + (coordenada.X - inicio.X);
            matriz.OffsetY = origen.Y + (coordenada.Y - inicio.Y);

            slotForImage.RenderTransform = new MatrixTransform(matriz);
        }

        /// <summary>
        /// Evento que se lanza cuando se restaura el zoom de la imagen. Deja la imagen con el zoom que tiene por defecto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBindingRestoreImageZoom_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Matrix matriz = slotForImage.RenderTransform.Value;
            int diferenciaContadoresRuedaRaton = contadorRuedaRatonArriba - contadorRuedaRatonAbajo;

            if (diferenciaContadoresRuedaRaton >= 0)
            {
                for (int i = 0; i < diferenciaContadoresRuedaRaton; i++)
                {
                    matriz.ScaleAtPrepend(1.0 / 1.1, 1.0 / 1.1, borderForSlotForImage.ActualWidth / 2, borderForSlotForImage.ActualHeight / 2);
                }
            }
            else
            {
                for (int i = 0; i > diferenciaContadoresRuedaRaton; i--)
                {
                    matriz.ScaleAtPrepend(1.1, 1.1, borderForSlotForImage.ActualWidth / 2, borderForSlotForImage.ActualHeight / 2);
                }
            }
            matriz.OffsetX = 0;
            matriz.OffsetY = 0;

            slotForImage.RenderTransform = new MatrixTransform(matriz);

            contadorRuedaRatonArriba = 0;
            contadorRuedaRatonAbajo = 0;
        }

        /// <summary>
        /// Evento que se lanza cuando se trata de restaurar el zoom de la imagen. Siempre permite restaurar el zoom (siempre es true)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBindingRestoreImageZoom_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Evento que se lanza cuando añade una descripción a la imagen. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBindingAddDescription_Executed(object sender, ExecutedRoutedEventArgs e)
        {
         
            if (textBlockDescription.Visibility == Visibility.Visible)
            {
                textBlockDescription.Visibility = Visibility.Hidden;
            }
            else
            {
                textBlockDescription.Visibility = Visibility.Visible;
            }

        }

        /// <summary>
        /// Evento que se lanza cuando se trata de añadir una descripción a la imagen. Siempre permite añadir la descripción (siempre es true)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandoBindingAddDescription_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}
