using FunctionApproximator.Windows;
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

namespace FunctionApproximator.Components
{
    /// <summary>
    /// Логика взаимодействия для DrawingSettingsComponents.xaml
    /// </summary>
    public partial class DrawingSettingsComponents : UserControl
    {
        public DrawingSettingsComponents()
        {
            InitializeComponent();
        }

		private void ChangeApproximatedGraphColor(object sender, RoutedEventArgs e)
		{
            var btn = (Button)sender;
            var selector = new ColorSelectionWindow((SolidColorBrush)btn.Background)
            {
                Title = "Approximated graph color"
            };
            if(selector.ShowDialog() == true)
            {
                btn.Background = selector.SelectedBrush;
            }
        }

		private void ChangeInputGraphColor(object sender, RoutedEventArgs e)
		{
			var btn = (Button)sender;
            var selector = new ColorSelectionWindow((SolidColorBrush)btn.Background)
            {
                Title = "Input values color"
            };
			if (selector.ShowDialog() == true)
			{
				btn.Background = selector.SelectedBrush;
			}
		}
    }
}
