using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FunctionApproximator.Messages;
using FunctionApproximator.ViewModels;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApproximator.VMsNew
{
	internal partial class MainVM : ObservableObject
	{
		public PointDataVM PointData { get; }
		public FileLoaderVM FileLoader { get; }
		public ApproximatorSettingsVM ApproximatorSettings { get; }
		public DataCheckerVM DataChecker { get; }
		public DrawingSettingsVM DrawingSettings { get; }
		public GraphPlotterVM GraphPlotter { get; }

		[ObservableProperty]
		[NotifyCanExecuteChangedFor(nameof(ClearGraphCommand))]
		private bool _isPlotted = false;

		public MainVM()
		{
			PointData = new();
			FileLoader = new(PointData);
			ApproximatorSettings = new();
			DataChecker = new(PointData, ApproximatorSettings);
			DrawingSettings = new();
			GraphPlotter = new();
		}

		#region Commands

		#region Loaded

		[RelayCommand]
		private void OnLoaded()
		{
			GraphPlotter.Initialize();
		}

		#endregion

		#region PlotGraph

		[RelayCommand(CanExecute = nameof(CanPlotGraphExecute))]
		private void PlotGraph()
		{
			var data = PointData.Points;
			GraphPlotter.ClearGraphs();
			//Approximator.Approximate(data);

			//double left = PlotModel.WindowLeftBorder, right = PlotModel.WindowRightBorder;

			//// draw graphs
			//var step = DrawingSettings.GetDrawingStep(left, right);
			//var res = Approximator.DrawGraph(left, right, step);
			//var (minX, maxX, minY, maxY) = PointsInput.GetInputPointsWindow();
			//PlotModel.Zoom(minX, maxX, minY, maxY);
			//PlotModel.DrawGraphs(data, res);

			//IsPlotted = true;
			//WeakReferenceMessenger.Default.Send(new GraphAccordanceChanged(false));
		}

		private bool CanPlotGraphExecute()
		{
			//if (int.TryParse(Approximator.PolynomialDegree, out var polDegree))
			//{
			//	var isValidPolDegree = PointsInput.Points.Count >= polDegree;
			//	WeakReferenceMessenger.Default.Send(new ChangeErrorMessage(_polynomDegreeError,
			//		isValidPolDegree));

			//	return PointsInput.IsDataReady && !Approximator.HasErrors && isValidPolDegree;
			//}
			//else
			//	return false;
			throw new NotImplementedException();
		}

		#endregion

		#region ClearGraph

		[RelayCommand(CanExecute = nameof(CanClearGraphExecute))]
		private void ClearGraph()
		{
			//GraphPlotter.ClearGraphs();
			//Approximator.Clear();
			//WeakReferenceMessenger.Default.Send(new GraphAccordanceChanged(false));
			//IsPlotted = false;
		}

		private bool CanClearGraphExecute()
		{
			return IsPlotted;
		}

		#endregion

		#endregion
	}
}
