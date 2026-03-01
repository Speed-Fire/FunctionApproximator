using FunctionApproximator.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApproximator.Extensions
{
	internal static class ICollectionExtensions
	{
		public static double[] FlattenPoints(this ICollection<PointVM> points)
		{
			var res = new double[points.Count * 2];

			var i = 0;
			foreach(var point in points)
			{
				res[i] = double.Parse(point.X);
				res[i + 1] = double.Parse(point.Y);
				i += 2;
			}

			return res;
		}
	}
}
