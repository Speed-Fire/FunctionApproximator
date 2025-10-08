using System.Configuration;
using System.Data;
using System.Globalization;
using System.Text;
using System.Windows;

namespace FunctionApproximator;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
	}
}

