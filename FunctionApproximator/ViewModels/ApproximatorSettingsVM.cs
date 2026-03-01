using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FunctionApproximator.ViewModels
{
	internal partial class ApproximatorSettingsVM : ObservableValidator
	{
		[ObservableProperty]
		[MinLength(1)]
		[CustomValidation(typeof(ApproximatorSettingsVM), nameof(ValidateInt))]
		private string _polynomialDegree = "1";

		[ObservableProperty]
		private string _drawingStep = "0.1";

		[ObservableProperty]
		private bool _isAutoDrawingStep = false;

		[ObservableProperty]
		private bool _invalidDegree;

		public int Degree => int.Parse(PolynomialDegree);

		public ApproximatorSettingsVM()
		{
			IsAutoDrawingStep = true;

			this.ErrorsChanged += (s, e) =>
			{
				InvalidDegree = this.GetErrors(nameof(PolynomialDegree)).Any();
			};
		}

		public static ValidationResult ValidateInt(string value)
		{
			if (!int.TryParse(value, out _))
				return new("Incorrect format.");
			return ValidationResult.Success!;
		}

		#region Drawing step

		public double GetDrawingStep(double left, double right)
		{
			if (IsAutoDrawingStep)
				return GetStep(left, right);
			else
				return double.Parse(DrawingStep);
		}

		private static double GetStep(double left, double right)
		{
			var diff = (long)Math.Ceiling(Math.Abs(left - right));
			var step = 0.1;

			while (diff > 100)
			{
				diff /= 10;
				step *= 10;
			}

			return step;
		}

		#endregion
	}
}
