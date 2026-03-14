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
	public partial class ApproximatorSettingsVM : ObservableValidator
	{
		[ObservableProperty]
		[NotifyDataErrorInfo]
		[CustomValidation(typeof(ApproximatorSettingsVM), nameof(ValidateInt))]
		private string _polynomialDegree = "1";

		[ObservableProperty]
		private bool _invalidDegree;

		[ObservableProperty]
		[NotifyDataErrorInfo]
		[CustomValidation(typeof(ApproximatorSettingsVM), nameof(ValidateInt))]
		private string _samplingDensity = "400";

		[ObservableProperty]
		private bool _invalidSamplingDensity;

		public int Degree => int.Parse(PolynomialDegree);
		public int Density => int.Parse(SamplingDensity);

		public event Action<int>? SamplingDensityChanged;

		public ApproximatorSettingsVM()
		{
			this.ErrorsChanged += (s, e) =>
			{
				InvalidDegree = this.GetErrors(nameof(PolynomialDegree)).Any();
				InvalidSamplingDensity = this.GetErrors(nameof(SamplingDensity)).Any();
			};
		}

		public static ValidationResult ValidateInt(string value)
		{
			if (!int.TryParse(value, out var degree))
				return new("Incorrect format.");
			if (degree < 1)
				return new("Value must be higher than 0.");
			return ValidationResult.Success!;
		}

		partial void OnSamplingDensityChanged(string value)
		{
			if (InvalidSamplingDensity)
				return;

			SamplingDensityChanged?.Invoke(Density);
		}
	}
}
