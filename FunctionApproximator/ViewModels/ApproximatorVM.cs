using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FunctionApproximator.Approximators;
using FunctionApproximator.Domain.Interfaces;
using FunctionApproximator.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FunctionApproximator.ViewModels
{
    public partial class ApproximatorVM : 
		ObservableValidator,
		IRecipient<RequestPolynomDegreeMessage>
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
			WeakReferenceMessenger.Default.RegisterAll(this);
		}

		partial void OnPolynomialDegreeChanged(string value)
		{
			if(int.TryParse(value, out var degree))
			{
				SelectedApproximator!.PolynomialDegree = degree;
			}
			else
			{
				PolynomialDegree = "1";
			}
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

		void IRecipient<RequestPolynomDegreeMessage>.Receive(RequestPolynomDegreeMessage message)
		{
			message.Reply(int.Parse(PolynomialDegree));
		}
	}
}
