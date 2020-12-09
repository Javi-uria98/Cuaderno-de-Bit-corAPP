using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing.Printing;

namespace Cuaderno_de_BitácorAPP
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Album> albums = new List<Album>();
        private int indiceAlbum = -1;
        private OpenFileDialog ventanaExploradorArchivos = new OpenFileDialog();
        

        public MainWindow()
        {
            InitializeComponent();
            inicializarPropiedadesExploradorArchivos();
            crearAlbumsDelArchivoDeTexto();
        }

        /// <summary>
        /// En el explorador de archivos, establece las opciones de selección multiple de imágenes y el filtro de tipo de archivo por defecto (.jgp)
        /// </summary>
        private void inicializarPropiedadesExploradorArchivos()
        {
            ventanaExploradorArchivos.Multiselect = true;
            ventanaExploradorArchivos.Filter = "Image Files (*.jpg)|*.jpg|" +
                                    "Image Files (*.png)|*.png|" +
                                    "All Files (*.*)|*.*";
        }

        /// <summary>
        /// Al iniciar la aplicación, busca el archivo de texto Albums.txt y carga los albumes existentes
        /// </summary>
        private void crearAlbumsDelArchivoDeTexto()
        {
            string rutaEscritorio = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string rutaArchivoAlbums = rutaEscritorio + "/Albums.txt";

            if (File.Exists(rutaArchivoAlbums))
            {
                string objetoDeserializado = File.ReadAllText(rutaArchivoAlbums);

                albums = JsonConvert.DeserializeObject(objetoDeserializado, typeof(List<Album>)) as List<Album>;

                indiceAlbum = albums.Count - 1;

                if (indiceAlbum > -1)
                {
                    textBoxForEmptyUniformGrid.Visibility = Visibility.Collapsed;
                }

                actualizarTreeView();
                actualizarUniformGrid();
            }
        }

        /// <summary>
        /// Evento que se lanza cuando se hace click al boton createAlbum. Crea un nuevo album
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createAlbumButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                intentarCrearAlbum();
            }
            catch (Exception)
            {
                MessageBox.Show("Una o más imágenes no tienen el formato válido \n \t \t O \n su formato no es soportado por BitmapImage.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Crea una nueva instancia de album y le asigna un nombre, actualizando la lista de albumes y añadiendo el album al TreeView y al UniformGrid
        /// </summary>
        private void intentarCrearAlbum()
        {
            VentanaNuevoAlbum ventanaInputNombreAlbum = new VentanaNuevoAlbum();
            Album album = new Album();

            if (ventanaInputNombreAlbum.ShowDialog() == true)
            {
                textBoxForEmptyUniformGrid.Visibility = Visibility.Collapsed;

                album.Nombre = ventanaInputNombreAlbum.Answer;
                actualizarAlbum(album, indiceAlbum + 1);
                indiceAlbum++;

                albums.Add(album);

                actualizarTreeView();
                actualizarUniformGrid();
            }
        }

        /// <summary>
        /// Actualiza el album con sus imagenes
        /// </summary>
        /// <paramref name="album">
        /// Album del que actualizar su contenido
        /// </paramref>
        /// <paramref name="albumPadre">
        /// Album inmediatamente superior en la lista de albumes
        /// </paramref>
        private void actualizarAlbum(Album album, int albumPadre)
        {
            if (ventanaExploradorArchivos.ShowDialog() == true)
            {
                int i = 0;
                string[] nombresArchivosSeguros = ventanaExploradorArchivos.SafeFileNames;
                foreach (string ruta in ventanaExploradorArchivos.FileNames)
                {
                    album.Imagenes.Add(new Imagen()
                    {
                        Nombre = nombresArchivosSeguros[i],
                        RutaImagen = ruta,
                        IndiceAlbumPadre = albumPadre
                    });
                    i++;
                }
            }
        }

        /// <summary>
        /// Actualiza el TreeView y sus items
        /// </summary>
        private void actualizarTreeView()
        {
            treeViewForAlbums.ItemsSource = albums;
            treeViewForAlbums.Items.Refresh();
        }

        /// <summary>
        /// Actualiza el UniformGrid y sus items
        /// </summary>
        private void actualizarUniformGrid()
        {
            listBoxForUniformGrid.ItemsSource = albums;
            listBoxForUniformGrid.Items.Refresh();
        }

        /// <summary>
        /// Evento que se lanza cuando se cambia el tamaño del uniformGrid. Actualiza los items del uniformGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UniformGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            listBoxForUniformGrid.Items.Refresh();
        }

        /// <summary>
        /// Evento que se lanza cuando se presiona el botón izquierdo del ratón sobre la imagen fotoAlbum.jpg. Abre una nueva ventana para visualizar las imagenes del album y actualiza el TreeView y el UniformGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            VentanaImagenes imagesWindow = new VentanaImagenes(albums[listBoxForUniformGrid.SelectedIndex]);

            imagesWindow.ShowDialog();
            actualizarTreeView();
            actualizarUniformGrid();
        }

        /// <summary>
        /// Evento que se lanza cuando se quiere borrar un album. Checkea si se puede borrar del TreeView y del UniformGrid, y establece si se puede o no borrar el album
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBindingDelete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (checkearSiAlbumPuedeBorrarseUniformGrid() || checkearSiAlbumPuedeBorrarseTreeView())
            {
                e.CanExecute = true;
            }
        }

        /// <summary>
        /// Checkea si el album puede borrarse del UniformGrid
        /// </summary>
        private bool checkearSiAlbumPuedeBorrarseUniformGrid()
        {
            if (listBoxForUniformGrid.SelectedIndex >= 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checkea si el album puede borrarse del TreeView
        /// </summary>
        private bool checkearSiAlbumPuedeBorrarseTreeView()
        {
            if (treeViewForAlbums.SelectedItem != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Evento que se lanza cuando se le da a eliminar el album. Borra el album tanto del TreeView como del UniformGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBindingDelete_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (checkearSiAlbumPuedeBorrarseUniformGrid())
            {
                borrarAlbumDelUniformGrid();
                return;
            }
            if (checkearSiAlbumPuedeBorrarseTreeView())
            {
                borrarAlbumDelTreeView();
                return;
            }
        }

        /// <summary>
        /// Borra el album del UniformGrid
        /// </summary>
        private void borrarAlbumDelUniformGrid()
        {
            int indiceABorrar = listBoxForUniformGrid.SelectedIndex;
            MessageBoxResult messageBoxResult = crearMessageBoxResultadoParaBorrado("album");

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                disminuirIndiceDeAlbumPadre(indiceABorrar);
                albums.RemoveAt(indiceABorrar);
                indiceAlbum--;
                actualizarTreeView();
                actualizarUniformGrid();
            }

            if (indiceAlbum < 0)
            {
                textBoxForEmptyUniformGrid.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Disminuye el indice del album inmediantamente superior
        /// </summary>
        /// <paramref name="indiceAlbumPadreADisminuir">
        /// indice que ha de disminuirse
        /// </paramref>
        private void disminuirIndiceDeAlbumPadre(int indiceAlbumPadreADisminuir)
        {
            for (int i = indiceAlbumPadreADisminuir; i < albums.Count; i++)
            {
                for (int j = 0; j < albums[i].Imagenes.Count; j++)
                {
                    albums[i].Imagenes[j].IndiceAlbumPadre--;
                }
            }
        }

        /// <summary>
        /// Borra el album del TreeView
        /// </summary>
        private void borrarAlbumDelTreeView()
        {
            if (treeViewForAlbums.SelectedItem.ToString().Equals("PhotoAlbum.Album"))
            {
                return;
            }

            Imagen imagenSeleccionada = (Imagen)treeViewForAlbums.SelectedItem;
            int indiceAlbumPadre = imagenSeleccionada.IndiceAlbumPadre;

            for (int i = 0; i < albums[indiceAlbumPadre].Imagenes.Count; i++)
            {
                if (albums[indiceAlbumPadre].Imagenes[i].Nombre.Equals(imagenSeleccionada.Nombre))
                {
                    MessageBoxResult messageBoxResultado = crearMessageBoxResultadoParaBorrado("imagen");

                    if (messageBoxResultado == MessageBoxResult.Yes)
                    {
                        albums[indiceAlbumPadre].Imagenes.RemoveAt(i);
                        actualizarUniformGrid();
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Crea un MessageBox donde pregunta si quiere realmente borrar el album. Devuelve la respuesta
        /// </summary>
        /// <paramref name="opcionUsuario">
        /// Opcion que elige el usuario (imagen o album)
        /// </paramref>
        private MessageBoxResult crearMessageBoxResultadoParaBorrado(string opcionUsuario)
        {
            MessageBoxResult resultado = MessageBox.Show("¿Estás seguro de que quieres eliminar este " + opcionUsuario + "?", "¿Seguro?",
                                                     MessageBoxButton.YesNo,
                                                     MessageBoxImage.Warning,
                                                     MessageBoxResult.Yes);
            return resultado;
        }

        /// <summary>
        /// Evento que se lanza cuando se le da a añadir al album. Permite añadir imagenes al album
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBindingAddToAlbum_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int indiceAlbumAlQueAnadirImagenes = listBoxForUniformGrid.SelectedIndex;

            if ((uniformGridForAlbums.Children.Count > 0) && (indiceAlbumAlQueAnadirImagenes >= 0))
            {
                actualizarAlbum(albums[indiceAlbumAlQueAnadirImagenes], indiceAlbumAlQueAnadirImagenes);
            }
        }

        /// <summary>
        /// Evento que se lanza cuando se quiere añadir al album. Establece si se puede añadir imagenes al album (siempre es true)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CommandBindingAddToAlbum_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        /// <summary>
        /// Evento que se lanza cuando se hace click sobre el menuItem Mostrar Detalles. Muestra los detalles de la imagen en cuestión 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowPhotoDetails_Click(object sender, RoutedEventArgs e)
        {
            Imagen imagenSeleccionada = (Imagen)treeViewForAlbums.SelectedItem;
            BitmapImage imagenBitMap = new BitmapImage(new Uri(imagenSeleccionada.RutaImagen));
            var tipoImagen = imagenSeleccionada.Nombre.Substring(imagenSeleccionada.Nombre.LastIndexOf('.') + 1);

            MessageBox.Show("Ruta : " + imagenBitMap.UriSource + "\n" + "\n" +
                            "Nombre : " + imagenSeleccionada.Nombre + "\n" +
                            "Tipo : " + tipoImagen + "\n" +
                            "Dimensiones : " + imagenBitMap.PixelWidth + " x " + imagenBitMap.PixelHeight + "\n" +
                            "Ancho : " + imagenBitMap.PixelWidth + "\n" +
                            "Alto : " + imagenBitMap.PixelHeight,
                            "Detalles", MessageBoxButton.OK, MessageBoxImage.Information);


        }

        /// <summary>
        /// Evento que se lanza cuando se hace click sobre el menuItem Imprimir Imagen. Imprime la imagen en cuestión
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintImage_Click(object sender, RoutedEventArgs e)
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += PrintPage;
            pd.Print();

        }

        /// <summary>
        /// Evento que convierte la imagen seleccionada para la impresión en una imagen del tipo System.Drawing.Image, de manera que pueda ser impresa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            Imagen imagenSeleccionada = (Imagen)treeViewForAlbums.SelectedItem;
            System.Drawing.Image img = System.Drawing.Image.FromFile(imagenSeleccionada.RutaImagen);
            System.Drawing.Point loc = new System.Drawing.Point(100, 100);
            e.Graphics.DrawImage(img, loc);
        }

        /// <summary>
        /// Evento que se lanza cuando se presiona el botón derecho del ratón sobre un album del TeeView. Muestra menus items como el de Mostrar Detalles
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectTreeViewItem_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = visualUpwardSearch(e.OriginalSource as DependencyObject);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }

        /// <summary>
        ///  Metodo sacado de internet. Sirve para poder mostrar items a traves del arbol visual VisualTreeHelper
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        static TreeViewItem visualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
            {
                source = VisualTreeHelper.GetParent(source);
            }

            return source as TreeViewItem;
        }

        /// <summary>
        /// Evento que se lanza cuando se le da a cerrar la aplicación. Pregunta si quiere guardar los cambios hechos, y si es así, los guarda en el archivo de texto Albums.txt, en el escritorio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult messageBoxResultado = MessageBox.Show("¿Quieres guardar los cambios en un archivo en el escritorio para después?", "¿Guardar cambios?",
                                                     MessageBoxButton.YesNo,
                                                     MessageBoxImage.Warning,
                                                     MessageBoxResult.Yes);

            if (messageBoxResultado == MessageBoxResult.Yes)
            {
                string objetoSerializado = JsonConvert.SerializeObject(albums);
                string rutaEscritorio = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                File.WriteAllText(rutaEscritorio + "/Albums.txt", objetoSerializado);
            }
        }

    }
}
