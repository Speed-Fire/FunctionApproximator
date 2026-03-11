using FunctionApproximator.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FunctionApproximator.Components
{
	/// <summary>
	/// Логика взаимодействия для NotificationsComponent.xaml
	/// </summary>
	public partial class NotificationsComponent : UserControl
    {
        private static readonly DoubleAnimation _turnOnAnimation = new(0, 1, new(TimeSpan.FromSeconds(0.15)));
        private static readonly DoubleAnimation _turnOffAnimation = new(1, 0, new(TimeSpan.FromSeconds(0.15)));

        private bool _currentNotificationBulbState = false;
        private bool _currentErrorBulbState = false;

        public NotificationsComponent()
        {
            InitializeComponent();

            LightBulb.Opacity = 0;
            NotificationText.Opacity = 0;
            ErrorLightBulb.Opacity = 0;
            ErrorText.Opacity = 0;

			Loaded += OnLoaded;
        }

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
            if(DataContext is NotificationsVM vm)
            {
				vm.PropertyChanged += SourceChanged;
            }
		}

		private void SourceChanged(object? sender, PropertyChangedEventArgs e)
		{
			if(e.PropertyName == nameof(NotificationsVM.CurrentError))
            {
                UpdateErrorBulb(((NotificationsVM)sender!).CurrentError?.Message);
            }
            else if(e.PropertyName == nameof(NotificationsVM.CurrentNotification))
            {
                UpdateNotificationBulb(((NotificationsVM)sender!).CurrentNotification?.Message);
            }
		}

        private static void TurnOn(UIElement[] controls)
        {
            foreach(var control in controls)
            {
                control.BeginAnimation(Control.OpacityProperty, _turnOnAnimation);
            }
        }

		private static void TurnOff(UIElement[] controls)
		{
			foreach (var control in controls)
			{
				control.BeginAnimation(Control.OpacityProperty, _turnOffAnimation);
			}
		}

        private void UpdateErrorBulb(string? error)
        {
            UpdateBulb(error, ref _currentErrorBulbState, ErrorLightBulb, ErrorText);
        }

        private void UpdateNotificationBulb(string? notification)
        {
            UpdateBulb(notification, ref _currentNotificationBulbState, LightBulb, NotificationText);
        }

        private static void UpdateBulb(string? msg, ref bool state, Image bulb, TextBlock tb)
        {
			if (msg is null)
			{
				if (state)
				{
					TurnOff([bulb, tb]);
					tb.Text = string.Empty;
					state = false;
				}
			}
			else
			{
				if (msg != tb.Text)
				{
					TurnOff([bulb, tb]);
					tb.Text = msg;
					TurnOn([bulb, tb]);
					state = true;
				}
			}
		}
	}
}
