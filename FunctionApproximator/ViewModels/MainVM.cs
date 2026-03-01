using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FunctionApproximator.Approximators;
using FunctionApproximator.Extensions;
using FunctionApproximator.Helpers;
using FunctionApproximator.Messages;
using FunctionApproximator.ViewModels;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace FunctionApproximator.ViewModels
{
	internal partial class MainVM : ObservableObject
	{
		public PointDataVM PointData { get; }
		public FileLoaderVM FileLoader { get; }
		public ApproximatorSettingsVM ApproximatorSettings { get; }
		public DataCheckerVM DataChecker { get; }
		public DrawingSettingsVM DrawingSettings { get; }
		public GraphPlotterVM GraphPlotter { get; }
		public ApproximatorVM Approximator { get; }

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
			Approximator = new(ApproximatorSettings);

			#region Event listeners

			DataChecker.PropertyChanged += (s, e) =>
			{
				PlotGraphCommand.NotifyCanExecuteChanged();
			};
			PointData.PointsChanged += () =>
			{
				if (!IsPlotted)
					return;

				WeakReferenceMessenger.Default.Send(new GraphAccordanceChanged(true));
			};
			GraphPlotter.ModelWindowBordersChanged += (l, r) =>
			{
				if (!IsPlotted)
					return;

				var res = Approximator.DrawGraph(l, r);
				GraphPlotter.DrawApproximatedGraph(res);
			};

			#endregion
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
			var data = PointData.Points.FlattenPoints();
			GraphPlotter.ClearGraphs();
			Approximator.Approximate(data);

			double left = GraphPlotter.WindowLeftBorder,
				right = GraphPlotter.WindowRightBorder;

			// draw graphs
			var res = Approximator.DrawGraph(left, right);
			var (minX, maxX, minY, maxY) = FlatPointsHelper.GetMinMaxXY(data);
			GraphPlotter.Zoom(minX, maxX, minY, maxY);
			GraphPlotter.DrawGraphs(data, res);

			IsPlotted = true;
			WeakReferenceMessenger.Default.Send(new GraphAccordanceChanged(false));
		}

		private bool CanPlotGraphExecute()
		{
			return DataChecker.IsReady;
		}

		#endregion

		#region ClearGraph

		[RelayCommand(CanExecute = nameof(CanClearGraphExecute))]
		private void ClearGraph()
		{
			GraphPlotter.ClearGraphs();
			Approximator.Clear();
			WeakReferenceMessenger.Default.Send(new GraphAccordanceChanged(false));
			IsPlotted = false;
		}

		private bool CanClearGraphExecute()
		{
			return IsPlotted;
		}

		#endregion

		#region CopyToClipboard

		[RelayCommand]
		private void CopyToClipboard(object data)
		{
			Clipboard.SetText(data.ToString());
			Clipboard.Flush();
		}

		#endregion

		#endregion
	}
}
