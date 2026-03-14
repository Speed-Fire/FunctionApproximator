using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FunctionApproximator.Components;
using FunctionApproximator.Messages;
using FunctionApproximator.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApproximator.ViewModels
{
	internal partial class DataCheckerVM : ObservableObject
	{
		private readonly PointDataVM _pointData;
		private readonly ApproximatorSettingsVM _approxSettings;
		private readonly NotificationsVM _notifications;

		[ObservableProperty]
		private bool _isReady;

		public DataCheckerVM(
			PointDataVM pointData,
			ApproximatorSettingsVM approxSettings,
			NotificationsVM notifications)
		{
			this._pointData = pointData;
			this._approxSettings = approxSettings;
			_notifications = notifications;

			pointData.PropertyChanged += DataChanged;
			approxSettings.PropertyChanged += DataChanged;
		}

		private void DataChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			// validation checks calls PropertyChanged as well for HasErrors, but at that moment
			// VM's error collection is not yet updated, so it can break wished behavior 
			// if not ignored.
			if (e.PropertyName == "HasErrors")
				return;
			UpdateReadyness();
		}

		private void UpdateReadyness()
		{
			IsReady = 
				(CheckPointsCorrectness() &
				CheckPointsSameX() &
				CheckAscendendXOrder() &
				CheckApproximatorDegreeCorrectness() &
				CheckSamplingDensityCorrectness()) &&
				CheckPointCountAndDegree();
		}

		private bool CheckAscendendXOrder()
		{
			var isCorrect = !_pointData.InvalidXOrder;
			SendError(Notifications.INVALID_X_ORDER, isCorrect);
			return isCorrect;
		}

		private bool CheckPointsCorrectness()
		{
			var isCorrect = !_pointData.InvalidPointData;
			SendError(Notifications.INVALID_POINTS, isCorrect);
			return isCorrect;
		}

		private bool CheckPointsSameX()
		{
			var isCorrect = !_pointData.HasSameX;
			SendError(Notifications.POINTS_SAME_X, isCorrect);
			return isCorrect;
		}

		private bool CheckApproximatorDegreeCorrectness()
		{
			var isCorrect = !_approxSettings.InvalidDegree;
			SendError(Notifications.INVALID_DEGREE, isCorrect);
			return isCorrect;
		}

		private bool CheckPointCountAndDegree()
		{
			var isCorrect = _pointData.PointCount >= _approxSettings.Degree + 1;
			SendError(Notifications.INVALID_POINT_COUNT, isCorrect);
			return isCorrect;
		}

		private bool CheckSamplingDensityCorrectness()
		{
			var isCorrect = !_approxSettings.InvalidSamplingDensity;
			SendError(Notifications.INVALID_SAMPLING_DENSITY, isCorrect);
			return isCorrect;
		}

		private void SendError(Notification error, bool remove)
		{
			if (remove)
			{
				_notifications.RemoveError(error);
			}
			else
			{
				_notifications.AddError(error);
			}
		}
	}
}
