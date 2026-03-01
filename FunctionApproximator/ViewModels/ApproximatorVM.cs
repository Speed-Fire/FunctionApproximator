using CommunityToolkit.Mvvm.ComponentModel;
using FunctionApproximator.Approximators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace FunctionApproximator.ViewModels
{
	internal partial class ApproximatorVM : ObservableObject
	{
		private readonly IFunctionApproximator _approximator;
		private readonly ApproximatorSettingsVM _settings;

		[ObservableProperty]
		private IReadOnlyList<double>? _coefficients;

		[ObservableProperty]
		private string _polynomRepresentation = string.Empty;

		public ApproximatorVM(ApproximatorSettingsVM settings)
		{
			_approximator = new LeastSquareApproximator();
			_settings = settings;
		}

		public void Approximate(double[] data)
		{
			if (_settings.InvalidDegree)
				throw new Exception("Invalid polynom degree.");

			_approximator.PolynomialDegree = _settings.Degree;
			_approximator.Approximate(data);
			Coefficients = _approximator.PolynomCoefficients;
			CreatePolynomString();
		}

		public Memory<double> DrawGraph(double left, double right)
		{
			var step = _settings.GetDrawingStep(left, right);
			return _approximator.Draw(left, right, step);
		}

		public void Clear()
		{
			_approximator.Clear();
			Coefficients = [];
			PolynomRepresentation = string.Empty;
		}

		private void CreatePolynomString()
		{
			if(Coefficients is null)
				throw new ArgumentNullException(nameof(Coefficients));

			var sb = new StringBuilder();
			for (int i = 0; i < Coefficients.Count; i++)
			{
				var coef = Math.Round(Coefficients[i], 2);

				if (i == 0)
				{
					sb.Append(coef);
				}
				else
				{
					if (double.IsNegative(coef))
						sb.Append(" - ");
					else
						sb.Append(" + ");

					sb.Append(Math.Abs(coef));
				}

				if (i == 1)
					sb.Append('x');
				else if (i > 1)
					sb.Append($"x^{i}");
			}

			PolynomRepresentation = sb.ToString();
		}
	}
}
