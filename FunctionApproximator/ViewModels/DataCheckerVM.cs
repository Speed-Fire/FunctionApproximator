using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FunctionApproximator.Components;
using FunctionApproximator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApproximator.ViewModels
{
	internal partial class DataCheckerVM : ObservableObject
	{
		#region Errors

		private static readonly PlotError INVALID_POINTS = new("Some points have incorrect data.");
		private static readonly PlotError POINTS_SAME_X = new("Some of your points have the same X.");
		private static readonly PlotError INVALID_DEGREE = new("Polynom degree value is invalid.");
		private static readonly PlotError INVALID_POINT_COUNT = new("Polynom degree can't be higher than count of points minus one.");

		#endregion

		private readonly PointDataVM _pointData;
		private readonly ApproximatorSettingsVM _approxSettings;

		[ObservableProperty]
		private bool _isReady;

		public DataCheckerVM(PointDataVM pointData, ApproximatorSettingsVM approxSettings)
		{
			this._pointData = pointData;
			this._approxSettings = approxSettings;

			pointData.PropertyChanged += DataChanged;
			approxSettings.PropertyChanged += DataChanged;
		}

		private void DataChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			UpdateReadyness();
		}

		private void UpdateReadyness()
		{
			IsReady = 
				CheckPointsCorrectness() &
				CheckPointsSameX() &
				CheckApproximatorDegreeCorrectness() &
				CheckPointCountAndDegree();
		}

		private bool CheckPointsCorrectness()
		{
			var isCorrect = !_pointData.InvalidPointData;
			SendError(INVALID_POINTS, isCorrect);
			return isCorrect;
		}

		private bool CheckPointsSameX()
		{
			var isCorrect = !_pointData.HasSameX;
			SendError(POINTS_SAME_X, isCorrect);
			return isCorrect;
		}

		private bool CheckApproximatorDegreeCorrectness()
		{
			var isCorrect = !_approxSettings.InvalidDegree;
			SendError(INVALID_DEGREE, isCorrect);
			return isCorrect;
		}

		private bool CheckPointCountAndDegree()
		{
			var isCorrect = _pointData.PointCount >= _approxSettings.Degree + 1;
			SendError(INVALID_POINT_COUNT, isCorrect);
			return isCorrect;
		}

		private static void SendError(PlotError message, bool remove)
		{
			WeakReferenceMessenger.Default
				.Send(new ChangeErrorMessage(message, remove));
		}
	}
}
