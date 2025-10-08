using OxyPlot;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace FunctionApproximator.Helpers
{
	internal static class AppearanceHelper
	{
		public static object FindResource(string key)
		{
			return Application.Current.FindResource(key);
		}

		public static OxyColor FindColorOxy(string key)
		{
			return ((Color)FindResource(key)).ToOxyColor();
		}
	}
}
