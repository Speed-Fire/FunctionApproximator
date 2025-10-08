using FunctionApproximator.Helpers;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

#nullable disable

namespace FunctionApproximator.Windows
{
	/// <summary>
	/// Логика взаимодействия для ColorSelectionWindow.xaml
	/// </summary>
	public partial class ColorSelectionWindow : Window
	{
		private readonly Button[] _colorButtons;
		public SolidColorBrush SelectedBrush { get; set; }

		private readonly SolidColorBrush _selectionBrush;
		private readonly SolidColorBrush _baseButtonBorderBrush;
		private Button _checkedButton;

		public ColorSelectionWindow(SolidColorBrush selectedBrush)
		{
			InitializeComponent();

			_colorButtons = [Btn0, Btn1, Btn2, Btn3, Btn4];

			_selectionBrush = new(new() { G = 255, B = 255, A = 255 });
			_baseButtonBorderBrush = (SolidColorBrush)AppearanceHelper.FindResource("Gray900Brush");

			StartupColorSelection(selectedBrush);
		}

		private void StartupColorSelection(SolidColorBrush selectedBrush)
		{
			foreach(var btn in _colorButtons)
			{
				if(btn.Background is SolidColorBrush brush &&
					brush.Color == selectedBrush.Color)
				{
					SelectColor(btn, new());
					break;
				}
			}
		}

		private void SelectColor(object sender, RoutedEventArgs e)
		{
			var btn = (Button)sender;
			SelectedBrush = (SolidColorBrush)btn.Background;
			MarkButtonChecked(btn);
		}

		private void Save(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}

		private void Exit(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
		}

		private void MarkButtonChecked(Button button)
		{
			if (_checkedButton is not null)
				_checkedButton.Foreground = _baseButtonBorderBrush;

			button.Foreground = _selectionBrush;
			_checkedButton = button;
		}

		private void Grid_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				this.DragMove();
			}
		}
	}
}
