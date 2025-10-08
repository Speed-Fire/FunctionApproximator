using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FunctionApproximator.UI.Controls
{
    /// <summary>
    /// More customizable button.
    /// </summary>
    public class NormalButton : Button
    {
        #region Properties

        #region CornerRadius

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(NormalButton), new PropertyMetadata(new CornerRadius(0)));

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        #endregion

        #endregion

        static NormalButton()
        {
            PaddingProperty.OverrideMetadata(typeof(NormalButton),
                new FrameworkPropertyMetadata(new Thickness(0)));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NormalButton),
                new FrameworkPropertyMetadata(typeof(NormalButton)));
        }

        public NormalButton()
        {
            DefaultStyleKey = typeof(NormalButton);
        }

    }
}
