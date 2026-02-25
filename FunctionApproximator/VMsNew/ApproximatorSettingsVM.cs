using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FunctionApproximator.VMsNew
{
	internal partial class ApproximatorSettingsVM : ObservableValidator
	{
		[ObservableProperty]
		[MinLength(1)]
		[CustomValidation(typeof(ApproximatorSettingsVM), nameof(ValidateInt))]
		private string _polynomialDegree = "1";

		[ObservableProperty]
		private bool _invalidDegree;

		public int Degree => int.Parse(PolynomialDegree);

		public ApproximatorSettingsVM()
		{
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
	}
}
