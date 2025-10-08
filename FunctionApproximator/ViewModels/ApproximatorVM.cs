using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FunctionApproximator.Approximators;
using FunctionApproximator.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FunctionApproximator.ViewModels
{
    public partial class ApproximatorVM : ObservableValidator
    {
        public List<IFunctionApproximator> Approximators { get; } = [
			new LeastSquareApproximator()
			];

		[ObservableProperty]
		[MinLength(1)]
		private string _polynomialDegree = "1";

        [ObservableProperty]
		[NotifyCanExecuteChangedFor(nameof(CopyRepresentationCommand))]
        private string _polynomRepresentation = string.Empty;

        [ObservableProperty]
        private IFunctionApproximator _selectedApproximator;

		public ApproximatorVM()
		{
			SelectedApproximator = Approximators[0];
		}

		partial void OnPolynomialDegreeChanged(string value)
		{
			SelectedApproximator!.PolynomialDegree = int.Parse(value);
		}

		partial void OnSelectedApproximatorChanged(
			IFunctionApproximator? oldValue, IFunctionApproximator newValue)
		{
			oldValue?.Clear();
			newValue.PolynomialDegree = int.Parse(PolynomialDegree);
			PolynomRepresentation = string.Empty;
		}

		[RelayCommand(CanExecute = nameof(CanCopyRepresentationExecute))]
		private void CopyRepresentation()
		{
			Clipboard.SetText(PolynomRepresentation);
			Clipboard.Flush();
		}

		private bool CanCopyRepresentationExecute()
		{
			return !string.IsNullOrEmpty(PolynomRepresentation);
		}

		public void Approximate(double[] data)
		{
			SelectedApproximator!.Approximate(data);
			PolynomRepresentation = SelectedApproximator.Representation;
		}

		public Memory<double> DrawGraph(double from, double to, double step)
		{
			return SelectedApproximator.Draw(from, to, step);
		}

		public void Clear()
		{
			SelectedApproximator.Clear();
			PolynomRepresentation = string.Empty;
		}
	}
}
