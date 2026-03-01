using CommunityToolkit.Mvvm.Messaging;
using FunctionApproximator.Messages;
using System;
using System.Collections.Generic;
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
    public record PlotError(string Message);

	/// <summary>
	/// Логика взаимодействия для NotificationsComponent.xaml
	/// </summary>
	public partial class NotificationsComponent : 
        UserControl,
        IRecipient<GraphAccordanceChanged>,
		IRecipient<ChangeErrorMessage>
    {
        private static readonly DoubleAnimation _turnOnAnimation = new(0, 1, new(TimeSpan.FromSeconds(0.15)));
        private static readonly DoubleAnimation _turnOffAnimation = new(1, 0, new(TimeSpan.FromSeconds(0.15)));

        private bool _currentBulbState = false;
        private bool _currentErrorBulbState = false;

        private readonly List<PlotError> _plotErrors = [];

        public NotificationsComponent()
        {
            InitializeComponent();

            WeakReferenceMessenger.Default.RegisterAll(this);

            LightBulb.Opacity = 0;
            AttentionText.Opacity = 0;
            ErrorLightBulb.Opacity = 0;
            ErrorText.Opacity = 0;
        }

        private void TurnLight(bool isOn)
        {
            if (isOn == _currentBulbState)
                return;

            if (isOn)
            {
                TurnOn([LightBulb, AttentionText]);
			}
            else
            {
				TurnOff([LightBulb, AttentionText]);
			}

            _currentBulbState = isOn;
        }

        private void TurnOn(UIElement[] controls)
        {
            foreach(var control in controls)
            {
                control.BeginAnimation(Control.OpacityProperty, _turnOnAnimation);
            }
        }

		private void TurnOff(UIElement[] controls)
		{
			foreach (var control in controls)
			{
				control.BeginAnimation(Control.OpacityProperty, _turnOffAnimation);
			}
		}

		private void AddError(PlotError error)
        {
            if (!_plotErrors.Contains(error))
                _plotErrors.Add(error);
            UpdateErrorBulb();
		}

        private void RemoveError(PlotError error)
        {
            _plotErrors.Remove(error);
            UpdateErrorBulb();

		}

        private void UpdateErrorBulb()
        {
            var error = _plotErrors.FirstOrDefault();
            if(error is null)
            {
                if (_currentErrorBulbState)
                {
                    TurnOff([ErrorLightBulb, ErrorText]);
                    ErrorText.Text = string.Empty;
                    _currentErrorBulbState = false;
                }
            }
            else
            {
                if(error.Message != ErrorText.Text)
                {
					TurnOff([ErrorLightBulb, ErrorText]);
                    ErrorText.Text = error.Message;
					TurnOn([ErrorLightBulb, ErrorText]);
                    _currentErrorBulbState = true;
				}
            }
        }

		void IRecipient<GraphAccordanceChanged>.Receive(GraphAccordanceChanged message)
		{
			TurnLight(message.Value);
		}

		void IRecipient<ChangeErrorMessage>.Receive(ChangeErrorMessage message)
		{
			if(message.IsRemove)
                RemoveError(message.Value);
            else
                AddError(message.Value);
		}
	}
}
