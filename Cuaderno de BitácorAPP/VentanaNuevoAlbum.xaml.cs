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
    /// Lógica de interacción para VentanaNuevoAlbum.xaml
    /// </summary>
    public partial class VentanaNuevoAlbum : Window
    {
        public VentanaNuevoAlbum()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Evento que se lanza al renderizarse la ventana. Inizializa el TextBox textAnswer (nombre del album), con todo el texto predeterminado seleccionado y el focus sobre él
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            textAnswer.SelectAll();
            textAnswer.Focus();
        }

        /// <summary>
        /// Evento que se lanza al hacer click sobre el botón dialogOkButton. Establece a true el valor resultado de la ventana, estableciendo el nombre del album
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dialogOkButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }


        /// <summary>
        /// Establece el nombre del textBox answer y lo devuelve.
        /// </summary>
        public string Answer
        {
            get { return textAnswer.Text; }
        }

    }
}
