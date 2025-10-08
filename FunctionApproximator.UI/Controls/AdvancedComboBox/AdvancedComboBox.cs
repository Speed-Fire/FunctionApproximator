using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FunctionApproximator.UI.Controls
{
	public class AdvancedComboBox : ComboBox
	{
		#region Properties

		#region ItemMouseOverBrush

		public static readonly DependencyProperty ItemMouseOverBrushProperty =
			DependencyProperty.Register("ItemMouseOverBrush", typeof(Brush), typeof(AdvancedComboBox), new PropertyMetadata(Brushes.LightGray));

		public Brush ItemMouseOverBrush
		{
			get => (Brush)GetValue(ItemMouseOverBrushProperty);
			set => SetValue(ItemMouseOverBrushProperty, value);
		}

		#endregion

		#region ItemSelectedBrush

		public static readonly DependencyProperty ItemSelectedBrushProperty =
			DependencyProperty.Register("ItemSelectedBrush", typeof(Brush), typeof(AdvancedComboBox), new PropertyMetadata(Brushes.LightBlue));

		public Brush ItemSelectedBrush
		{
			get => (Brush)GetValue(ItemSelectedBrushProperty);
			set => SetValue(ItemSelectedBrushProperty, value);
		}

		#endregion

		#endregion

		static AdvancedComboBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(AdvancedComboBox),
				new FrameworkPropertyMetadata(typeof(AdvancedComboBox)));

			
		}

		public AdvancedComboBox()
		{
			
		}
	}
}
