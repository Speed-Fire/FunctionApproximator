using FunctionApproximator.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FunctionApproximator.UI.Themes
{
	public partial class Styles : ResourceDictionary
	{
		private void TextBox_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var tb = sender as AdvancedTextBox;

			if(tb is not null && !tb.IsKeyboardFocusWithin)
			{
				e.Handled = true;
				tb.Focus();
			}
		}

		private void TextBox_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
		{
			var tb = sender as AdvancedTextBox;

			if (!tb.IsReadOnly)
				tb?.SelectAll();
		}
	}
}
