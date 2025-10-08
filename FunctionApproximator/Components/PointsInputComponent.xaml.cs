using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для PointsInputComponent.xaml
    /// </summary>
    public partial class PointsInputComponent : UserControl
    {
        public PointsInputComponent()
        {
            InitializeComponent();
        }

		private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Space)
				e.Handled = true;
		}

		private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			var tb = (TextBox)sender;
			var actualText = tb.Text;
			var text = e.Text;

			e.Handled = !DoubleRegex().IsMatch(actualText + text);
		}

		[GeneratedRegex(@"^[+-]?(\d+(\.\d*)?|\.\d*)?([eE][+-]?\d*)?$")]
		private static partial Regex DoubleRegex();
	}
}
