using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApproximator.Misc
{
	public record Notification(string Message, bool IsError = false);

	public static class Notifications
	{
		#region Errors

		public static Notification INVALID_POINTS { get; } = new("Some points have incorrect data.", true);
		public static Notification POINTS_SAME_X { get; } = new("Some of your points have the same X.", true);
		public static Notification INVALID_DEGREE { get; } = new("Polynom degree value is invalid.", true);
		public static Notification INVALID_POINT_COUNT { get; } = new("Polynom degree can't be higher than count of points minus one.", true);
		public static Notification INVALID_X_ORDER { get; } = new("The x-values must be in ascending order.", true);

		#endregion

		#region Notifications

		public static Notification INPUT_CHANGED { get; } = new("Attention! Input data was changed. Please replot the graph.");

		#endregion
	}
}
