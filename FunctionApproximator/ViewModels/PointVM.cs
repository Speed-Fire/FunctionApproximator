using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApproximator.ViewModels
{
	public partial class PointVM : ObservableValidator
	{
		[ObservableProperty]
		private int _id;

		[ObservableProperty]
		[NotifyDataErrorInfo]
		[CustomValidation(typeof(PointVM), nameof(ValidateDouble))]
		public string _x = "0";

		[ObservableProperty]
		[NotifyDataErrorInfo]
		[CustomValidation(typeof(PointVM), nameof(ValidateDouble))]
		public string _y = "0";

		public static ValidationResult ValidateDouble(string value)
		{
			if (!double.TryParse(value, out _))
				return new("Incorrect format.");
			return ValidationResult.Success!;
		}
	}
}
