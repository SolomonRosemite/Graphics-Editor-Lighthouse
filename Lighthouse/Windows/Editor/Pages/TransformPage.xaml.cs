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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lighthouse.Windows.Editor.Pages
{
    /// <summary>
    /// Interaction logic for TransformPage.xaml
    /// </summary>
    public partial class TransformPage : Page
    {
        private bool isChained = false;

        public TransformPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void OnChainClick(object sender, RoutedEventArgs e)
        {
            if (isChained)
            {
                IsChainedImg.Opacity = 0;
                IsNotChainedImg.Opacity = 1;
            }
            else
            {
                IsChainedImg.Opacity = 1;
                IsNotChainedImg.Opacity = 0;
            }

            isChained = !isChained;
        }
    }
}
