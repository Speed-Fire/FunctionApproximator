using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FunctionApproximator.Enums;
using FunctionApproximator.Messages;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FunctionApproximator.ViewModels
{
	internal partial class DrawingSettingsVM : ObservableObject
	{
		[ObservableProperty]
		private bool _showInputGraph = true;

		[ObservableProperty]
		private bool _showApproximatedGraph = true;

		[ObservableProperty]
		private Brush _inputGraphColor;

		[ObservableProperty]
		private Brush _approximatedGraphColor;

		[ObservableProperty]
		private bool _showMajorGridlines = true;

		[ObservableProperty]
		private bool _showMinorGridlines = true;

		public DrawingSettingsVM()
		{
			ApproximatedGraphColor = new SolidColorBrush(Colors.LightGreen);
			InputGraphColor = new SolidColorBrush(Colors.OrangeRed);
		}

		#region Property changes

		partial void OnApproximatedGraphColorChanged(Brush value)
		{
			WeakReferenceMessenger.Default
				.Send(new GraphColorChanged(((SolidColorBrush)value).Color.ToOxyColor(), true));
		}

		partial void OnInputGraphColorChanged(Brush value)
		{
			WeakReferenceMessenger.Default
				.Send(new GraphColorChanged(((SolidColorBrush)value).Color.ToOxyColor(), false));
		}

		partial void OnShowApproximatedGraphChanged(bool value)
		{
			WeakReferenceMessenger.Default
				.Send(new GraphVisibilityChanged(value, true));
		}

		partial void OnShowInputGraphChanged(bool value)
		{
			WeakReferenceMessenger.Default
				.Send(new GraphVisibilityChanged(value, false));
		}

		partial void OnShowMajorGridlinesChanged(bool value)
		{
			WeakReferenceMessenger.Default
				.Send(new GraphGridlinesVisibilityChanged(GetGridlineVisibility()));
		}

		partial void OnShowMinorGridlinesChanged(bool value)
		{
			WeakReferenceMessenger.Default
				.Send(new GraphGridlinesVisibilityChanged(GetGridlineVisibility()));
		}

		#endregion

		#region Internal

		private GridlineVisibility GetGridlineVisibility()
		{
			return (GridlineVisibility)((Unsafe.BitCast<bool, byte>(ShowMajorGridlines) & 1) + 2 * (Unsafe.BitCast<bool, byte>(ShowMinorGridlines) & 1));
		}

		#endregion
	}
}
