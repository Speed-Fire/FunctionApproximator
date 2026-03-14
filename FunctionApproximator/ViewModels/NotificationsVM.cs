using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using FunctionApproximator.Messages;
using FunctionApproximator.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApproximator.ViewModels
{
	internal partial class NotificationsVM : ObservableObject
	{
		private readonly List<Notification> _notifications = [];
		private readonly List<Notification> _errors = [];

		[ObservableProperty]
		private Notification? _currentNotification;

		[ObservableProperty]
		private Notification? _currentError;

		public void AddNotification(Notification notification)
		{
			if (!_notifications.Contains(notification))
				_notifications.Add(notification);
			CurrentNotification = _notifications.FirstOrDefault();
		}

		public void RemoveNotification(Notification notification)
		{
			_notifications.Remove(notification);
			CurrentNotification = _notifications.FirstOrDefault();
		}

		public void AddError(Notification error)
		{
			if (!_errors.Contains(error))
				_errors.Add(error);
			CurrentError = error;
		}

		public void RemoveError(Notification error)
		{
			_errors.Remove(error);
			CurrentError = _errors.LastOrDefault();
		}
	}
}
