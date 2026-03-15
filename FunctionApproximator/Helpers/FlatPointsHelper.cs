using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApproximator.Helpers
{
	internal static class FlatPointsHelper
	{
		public static (double minX, double maxX, double minY, double maxY)
			GetMinMaxXY(double[] points)
		{
			var minX = double.MaxValue;
			var maxX = double.MinValue;
			var minY = double.MaxValue;
			var maxY = double.MinValue;

			for(int i = 0; i < points.Length; i += 2)
			{
				var x = points[i];
				var y = points[i + 1];

				minX = Math.Min(minX, x);
				maxX = Math.Max(maxX, x);
				minY = Math.Min(minY, y);
				maxY = Math.Max(maxY, y);
			}

			return (minX, maxX, minY, maxY);
		}
	}
}
