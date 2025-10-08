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
    /// <summary>
    /// Логика взаимодействия для InputChangedNotifierComponent.xaml
    /// </summary>
    public partial class InputChangedNotifierComponent : UserControl, IRecipient<GraphAccordanceChanged>
    {
        private static DoubleAnimation _turnOnAnimation = new(0, 1, new(TimeSpan.FromSeconds(0.15)));
        private static DoubleAnimation _turnOffAnimation = new(1, 0, new(TimeSpan.FromSeconds(0.15)));

        private bool _currentBulbState = false;

        public InputChangedNotifierComponent()
        {
            InitializeComponent();

            WeakReferenceMessenger.Default.RegisterAll(this);

            LightBulb.Opacity = 0;
            AttentionText.Opacity = 0;
        }

        private void TurnLight(bool isOn)
        {
            if (isOn == _currentBulbState)
                return;

            if (isOn)
            {
                LightBulb.BeginAnimation(Control.OpacityProperty, _turnOnAnimation);
                AttentionText.BeginAnimation(Control.OpacityProperty, _turnOnAnimation);
			}
            else
            {
				LightBulb.BeginAnimation(Control.OpacityProperty, _turnOffAnimation);
				AttentionText.BeginAnimation(Control.OpacityProperty, _turnOffAnimation);
			}

            _currentBulbState = isOn;
        }

		void IRecipient<GraphAccordanceChanged>.Receive(GraphAccordanceChanged message)
		{
			TurnLight(message.Value);
		}
	}
}
