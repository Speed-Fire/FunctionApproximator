using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FunctionApproximator.Components;
using FunctionApproximator.GraphSeries;
using FunctionApproximator.Messages;
using FunctionApproximator.ViewModels;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FunctionApproximator
{
	unsafe partial class MainVM : ObservableObject
    {
		private static readonly PlotError _polynomDegreeError = new("Your polynom degree can't be higher than count of points.");

		public AdaptivePlotModel PlotModel { get; } = new();
		public PointsInputVM PointsInput { get; } = new();
		public ApproximatorVM Approximator { get; } = new();
		public DrawingSettingsVM DrawingSettings { get; } = new();

		[ObservableProperty]
		[NotifyCanExecuteChangedFor(nameof(ClearGraphCommand))]
		private bool _isPlotted = false;

		public MainVM()
		{
			Approximator.ErrorsChanged += Approximator_ErrorsChanged;
			PlotModel.ModelWindowBordersChanged += PlotModel_ModelWindowBordersChanged;
			PointsInput.PropertyChanged += PointsInput_PropertyChanged;
			PointsInput.InputDataChanged += OnInputDataChanged;
			PointsInput.Points.CollectionChanged += (sndr, e) =>
			{
				OnDataReadynessChanged();
			};
			Approximator.PropertyChanged += (sndr, e) =>
			{
				if(e.PropertyName == nameof(Approximator.PolynomialDegree))
					OnDataReadynessChanged();
			};
		}

		#region Commands

		#region Loaded

		[RelayCommand]
		private void OnLoaded()
		{
			PlotModel.Initialize();
		}

		#endregion

		#region PlotGraph

		[RelayCommand(CanExecute = nameof(CanPlotGraphExecute))]
		private void PlotGraph()
		{
			var data = PointsInput.GetPoints();
			PlotModel.ClearGraphs();
			Approximator.Approximate(data);
			
			double left = PlotModel.WindowLeftBorder, right = PlotModel.WindowRightBorder;

			// draw graphs
			var step = DrawingSettings.GetDrawingStep(left, right);
			var res = Approximator.DrawGraph(left, right, step);
			var (minX, maxX, minY, maxY) = PointsInput.GetInputPointsWindow();
			PlotModel.Zoom(minX, maxX, minY, maxY);
			PlotModel.DrawGraphs(data, res);

			IsPlotted = true;
			WeakReferenceMessenger.Default.Send(new GraphAccordanceChanged(false));
		}

		private bool CanPlotGraphExecute()
		{
			if (int.TryParse(Approximator.PolynomialDegree, out var polDegree))
			{
				var isValidPolDegree = PointsInput.Points.Count >= polDegree;
				WeakReferenceMessenger.Default.Send(new ChangeErrorMessage(_polynomDegreeError,
					isValidPolDegree));

				return PointsInput.IsDataReady && !Approximator.HasErrors && isValidPolDegree;
			}
			else
				return false;
		}

		#endregion

		#region ClearGraph

		[RelayCommand(CanExecute = nameof(CanClearGraphExecute))]
		private void ClearGraph()
		{
			PlotModel.ClearGraphs();
			Approximator.Clear();
			WeakReferenceMessenger.Default.Send(new GraphAccordanceChanged(false));
			IsPlotted = false;
		}

		private bool CanClearGraphExecute()
		{
			return IsPlotted;
		}

		#endregion

		#endregion

		private void PointsInput_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(PointsInput.IsDataReady):
					OnDataReadynessChanged();
					break;
				default:
					break;
			}
		}

		private void Approximator_ErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
		{
			if (IsPlotted)
				WeakReferenceMessenger.Default.Send(new GraphAccordanceChanged(true));
		}

		private void PlotModel_ModelWindowBordersChanged(double left, double right)
		{
			if (!IsPlotted)
				return;

			var step = DrawingSettings.GetDrawingStep(left, right);

			var res = Approximator.DrawGraph(left, right, step);
			PlotModel.DrawApproximatedGraph(res);
		}

		private void OnInputDataChanged()
		{
			if (IsPlotted)
			{
				WeakReferenceMessenger.Default.Send(new GraphAccordanceChanged(true));
			}
		}

		private void OnDataReadynessChanged()
		{
			PlotGraphCommand.NotifyCanExecuteChanged();
		}
	}

}
