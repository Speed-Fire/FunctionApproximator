using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FunctionApproximator.GraphSeries;
using FunctionApproximator.Helpers;
using FunctionApproximator.Messages;
using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FunctionApproximator.ViewModels
{
	internal partial class GraphPlotterVM : ObservableObject,
		IRecipient<GraphColorChanged>,
		IRecipient<GraphVisibilityChanged>,
		IRecipient<GraphGridlinesVisibilityChanged>
	{
		[ObservableProperty]
		private PlotModel _model;

		private readonly CustomLineSeries _approximatedGraphSeries;
		private readonly CustomLineSeries _inputGraphSeries;

		private readonly Axis _xAxis;
		private readonly Axis _yAxis;

		private double _currentXMin;
		private double _currentXMax;

		private readonly OxyColor _minorGridlineColor;
		private readonly OxyColor _majorGridlineColor;

		public double WindowLeftBorder { get; private set; }
		public double WindowRightBorder { get; private set; }

		public event Action<double, double>? ModelWindowBordersChanged;

		public GraphPlotterVM()
		{
			_majorGridlineColor = AppearanceHelper.FindColorOxy("Gray700");
			_minorGridlineColor = AppearanceHelper.FindColorOxy("Gray600");

			_model = CreatePlotModel();
			_xAxis = CreateAxis(AxisPosition.Bottom, -20, 80);
			_yAxis = CreateAxis(AxisPosition.Left, -20, 80);

			_model.Axes.Add(_xAxis);
			_model.Axes.Add(_yAxis);
			_approximatedGraphSeries = new CustomLineSeries();
			_inputGraphSeries = new CustomLineSeries
			{
				MarkerType = MarkerType.Diamond,
				MarkerSize = 6,
				LineStyle = LineStyle.None
			};

			_model.Series.Add(_approximatedGraphSeries);
			_model.Series.Add(_inputGraphSeries);

			WeakReferenceMessenger.Default.RegisterAll(this);
		}

		public void Initialize()
		{
			SetXWindowBorders(_xAxis.ClipMinimum, _xAxis.ClipMaximum);

#pragma warning disable CS0618 // Тип или член устарел
			_xAxis.AxisChanged += XAxis_AxisChanged;
#pragma warning restore CS0618 // Тип или член устарел
		}

		#region Model and axes

		private static LinearAxis CreateAxis(AxisPosition position, double minimum, double maximum)
		{
			var mainColor = AppearanceHelper.FindColorOxy("Gray900");
			var gridlineColor = AppearanceHelper.FindColorOxy("Gray700");
			var minorGridlineColor = AppearanceHelper.FindColorOxy("Gray600");

			return new LinearAxis()
			{
				Position = position,
				Minimum = minimum,
				Maximum = maximum,
				MinimumRange = 0.01,
				MajorGridlineStyle = LineStyle.Solid,
				MinorGridlineStyle = LineStyle.Solid,
				MinorGridlineColor = minorGridlineColor,
				MajorGridlineColor = gridlineColor,
				AxislineColor = mainColor,
				MinorTicklineColor = mainColor,
				TicklineColor = mainColor,
				TextColor = mainColor
			};
		}

		private static PlotModel CreatePlotModel()
		{
			var color = AppearanceHelper.FindColorOxy("Gray900");
			return new()
			{
				PlotAreaBorderColor = color
			};
		}

		#endregion

		#region Commands

		[RelayCommand]
		private void OnMouseUp(MouseButtonEventArgs e)
		{
			var newXMin = _xAxis.ActualMinimum;
			if (Math.Abs(newXMin - _currentXMin) < 1e-5)
				return;

			SetXWindowBorders(_xAxis.ClipMinimum, _xAxis.ClipMaximum);
			ModelWindowBordersChanged?.Invoke(WindowLeftBorder, WindowRightBorder);
		}

		#endregion

		#region Zoom

		public void Zoom(double minX, double maxX, double minY, double maxY)
		{
			double dx = (maxX - minX) * 0.1;
			double dy = (maxY - minY) * 0.1;

			var xAxis = Model.Axes.First(a => a.Position == AxisPosition.Bottom);
			var yAxis = Model.Axes.First(a => a.Position == AxisPosition.Left);

			xAxis.Zoom(minX - dx, maxX + dx);
			yAxis.Zoom(minY - dy, maxY + dy);
		}

		private void XAxis_AxisChanged(object? sender, AxisChangedEventArgs e)
		{
			if (e.ChangeType != AxisChangeTypes.Zoom)
				return;
			
			SetXWindowBorders(_xAxis.ClipMinimum, _xAxis.ClipMaximum);
			ModelWindowBordersChanged?.Invoke(WindowLeftBorder, WindowRightBorder);
		}

		#endregion

		#region Drawing

		public void DrawGraphs(ReadOnlyMemory<double> inputData,
			ReadOnlyMemory<double> approximatedData)
		{
			_approximatedGraphSeries.RawPoints = approximatedData;
			_inputGraphSeries.RawPoints = inputData;
			Model.InvalidatePlot(false);
		}

		public void DrawApproximatedGraph(ReadOnlyMemory<double> approximatedData)
		{
			_approximatedGraphSeries.RawPoints = approximatedData;
			Model.InvalidatePlot(false);
		}

		public void ClearGraphs()
		{
			_approximatedGraphSeries.RawPoints = Array.Empty<double>();
			_inputGraphSeries.RawPoints = Array.Empty<double>();

			Model.InvalidatePlot(false);
		}

		#endregion

		#region Internal

		private void SetXWindowBorders(double modelXMin, double modelXMax)
		{
			_currentXMin = modelXMin;
			_currentXMax = modelXMax;

			var interval = Math.Abs(modelXMax - modelXMin);
			WindowLeftBorder = modelXMin - interval;
			WindowRightBorder = modelXMax + interval;
		}

		#endregion

		#region Message handlers

		void IRecipient<GraphColorChanged>.Receive(GraphColorChanged message)
		{
			if (message.IsApproximatedGraph)
			{
				_approximatedGraphSeries.Color = message.Value;
			}
			else
			{
				_inputGraphSeries.Color = message.Value;
			}

			Model.InvalidatePlot(false);
		}

		void IRecipient<GraphVisibilityChanged>.Receive(GraphVisibilityChanged message)
		{
			if (message.IsApproximatedGraph)
			{
				_approximatedGraphSeries.IsVisible = message.Value;
			}
			else
			{
				_inputGraphSeries.IsVisible = message.Value;
			}

			Model.InvalidatePlot(false);
		}

		void IRecipient<GraphGridlinesVisibilityChanged>.Receive(GraphGridlinesVisibilityChanged message)
		{
			var value = message.Value;

			switch (message.Value)
			{
				case Enums.GridlineVisibility.Hidden:
					HideGridlines();
					break;
				case Enums.GridlineVisibility.MajorOnly:
					ShowMajorGridlines();
					break;
				case Enums.GridlineVisibility.MinorOnly:
					ShowMinorGridlines();
					break;
				case Enums.GridlineVisibility.Both:
					ShowBothGridlines();
					break;
			}

			Model.InvalidatePlot(false);
		}

		#endregion

		#region Gridlines

		private void HideGridlines()
		{
			_xAxis.MajorGridlineStyle = LineStyle.None;
			_xAxis.MinorGridlineStyle = LineStyle.None;
			_yAxis.MajorGridlineStyle = LineStyle.None;
			_yAxis.MinorGridlineStyle = LineStyle.None;
		}

		private void ShowMajorGridlines()
		{
			_xAxis.MajorGridlineColor = _majorGridlineColor;
			_yAxis.MajorGridlineColor = _majorGridlineColor;

			_xAxis.MajorGridlineStyle = LineStyle.Solid;
			_xAxis.MinorGridlineStyle = LineStyle.None;
			_yAxis.MajorGridlineStyle = LineStyle.Solid;
			_yAxis.MinorGridlineStyle = LineStyle.None;
		}

		private void ShowMinorGridlines()
		{
			_xAxis.MajorGridlineColor = _minorGridlineColor;
			_yAxis.MajorGridlineColor = _minorGridlineColor;

			_xAxis.MajorGridlineStyle = LineStyle.Solid;
			_xAxis.MinorGridlineStyle = LineStyle.Solid;
			_yAxis.MajorGridlineStyle = LineStyle.Solid;
			_yAxis.MinorGridlineStyle = LineStyle.Solid;
		}

		private void ShowBothGridlines()
		{
			_xAxis.MajorGridlineColor = _majorGridlineColor;
			_yAxis.MajorGridlineColor = _majorGridlineColor;

			_xAxis.MajorGridlineStyle = LineStyle.Solid;
			_xAxis.MinorGridlineStyle = LineStyle.Solid;
			_yAxis.MajorGridlineStyle = LineStyle.Solid;
			_yAxis.MinorGridlineStyle = LineStyle.Solid;
		}

		#endregion
	}
}
