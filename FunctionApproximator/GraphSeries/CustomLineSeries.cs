using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

#nullable disable

namespace FunctionApproximator.GraphSeries
{
	class CustomLineSeries : CustomDataPointSeries
	{
		/// <summary>
		/// The divisor value used to calculate tolerance for line smoothing.
		/// </summary>
		private const double ToleranceDivisor = 200;

		/// <summary>
		/// The output buffer.
		/// </summary>
		private List<ScreenPoint> outputBuffer;

		/// <summary>
		/// The buffer for contiguous screen points.
		/// </summary>
		private List<ScreenPoint> contiguousScreenPointsBuffer;

		/// <summary>
		/// The buffer for decimated points.
		/// </summary>
		private List<ScreenPoint> decimatorBuffer;

		/// <summary>
		/// The default color.
		/// </summary>
		private OxyColor defaultColor;

		/// <summary>
		/// The default marker fill color.
		/// </summary>
		private OxyColor defaultMarkerFill;

		/// <summary>
		/// The default line style.
		/// </summary>
		private LineStyle defaultLineStyle;

		/// <summary>
		/// The smoothed points.
		/// </summary>
		private List<DataPoint> smoothedPoints;

		/// <summary>
		/// Initializes a new instance of the <see cref = "LineSeries" /> class.
		/// </summary>
		public CustomLineSeries()
		{
			StrokeThickness = 2;
			LineJoin = LineJoin.Bevel;
			LineStyle = LineStyle.Automatic;

			Color = OxyColors.Automatic;
			BrokenLineColor = OxyColors.Undefined;

			MarkerFill = OxyColors.Automatic;
			MarkerStroke = OxyColors.Automatic;
			MarkerResolution = 0;
			MarkerSize = 3;
			MarkerStrokeThickness = 1;
			MarkerType = MarkerType.None;

			MinimumSegmentLength = 2;

			CanTrackerInterpolatePoints = true;
			LabelMargin = 6;
			smoothedPoints = new List<DataPoint>();

			HasRenderWithRect = true;
		}

		/// <summary>
		/// Gets or sets the color of the curve.
		/// </summary>
		/// <value>The color.</value>
		public OxyColor Color { get; set; }

		/// <summary>
		/// Gets or sets the color of the broken line segments. The default is <see cref="OxyColors.Undefined"/>. Set it to <see cref="OxyColors.Automatic"/> if it should follow the <see cref="Color" />.
		/// </summary>
		/// <remarks>Add <c>DataPoint.Undefined</c> in the Points collection to create breaks in the line.</remarks>
		public OxyColor BrokenLineColor { get; set; }

		/// <summary>
		/// Gets or sets the broken line style. The default is <see cref="LineStyle.Solid" />.
		/// </summary>
		public LineStyle BrokenLineStyle { get; set; }

		/// <summary>
		/// Gets or sets the broken line thickness. The default is <c>0</c> (no line).
		/// </summary>
		public double BrokenLineThickness { get; set; }

		/// <summary>
		/// Gets or sets the dash array for the rendered line (overrides <see cref="LineStyle" />). The default is <c>null</c>.
		/// </summary>
		/// <value>The dash array.</value>
		/// <remarks>If this is not <c>null</c> it overrides the <see cref="LineStyle" /> property.</remarks>
		public double[] Dashes { get; set; }

		/// <summary>
		/// Gets or sets the decimator.
		/// </summary>
		/// <value>
		/// The decimator action.
		/// </value>
		/// <remarks>The decimator can be used to improve the performance of the rendering. See the example.</remarks>
		public Action<List<ScreenPoint>, List<ScreenPoint>> Decimator { get; set; }

		/// <summary>
		/// Gets or sets the label format string. The default is <c>null</c> (no labels).
		/// </summary>
		/// <value>The label format string.</value>
		public string LabelFormatString { get; set; }

		/// <summary>
		/// Gets or sets the label margins. The default is <c>6</c>.
		/// </summary>
		public double LabelMargin { get; set; }

		/// <summary>
		/// Gets or sets the line join. The default is <see cref="LineJoin.Bevel" />.
		/// </summary>
		/// <value>The line join.</value>
		public LineJoin LineJoin { get; set; }

		/// <summary>
		/// Gets or sets the line style. The default is <see cref="LineStyle.Automatic" />.
		/// </summary>
		/// <value>The line style.</value>
		public LineStyle LineStyle { get; set; }

		/// <summary>
		/// Gets or sets a value specifying the position of a legend rendered on the line. The default is <c>LineLegendPosition.None</c>.
		/// </summary>
		/// <value>A value specifying the position of the legend.</value>
		public LineLegendPosition LineLegendPosition { get; set; }

		/// <summary>
		/// Gets or sets the marker fill color. The default is <see cref="OxyColors.Automatic" />.
		/// </summary>
		/// <value>The marker fill.</value>
		public OxyColor MarkerFill { get; set; }

		/// <summary>
		/// Gets or sets the a custom polygon outline for the markers. Set <see cref="MarkerType" /> to <see cref="MarkerType.Custom" /> to use this property. The default is <c>null</c>.
		/// </summary>
		/// <value>A polyline.</value>
		public ScreenPoint[] MarkerOutline { get; set; }

		/// <summary>
		/// Gets or sets the marker resolution. The default is <c>0</c>.
		/// </summary>
		/// <value>The marker resolution.</value>
		public int MarkerResolution { get; set; }

		/// <summary>
		/// Gets or sets the size of the marker. The default is <c>3</c>.
		/// </summary>
		/// <value>The size of the marker.</value>
		public double MarkerSize { get; set; }

		/// <summary>
		/// Gets or sets the marker stroke. The default is <c>OxyColors.Automatic</c>.
		/// </summary>
		/// <value>The marker stroke.</value>
		public OxyColor MarkerStroke { get; set; }

		/// <summary>
		/// Gets or sets the marker stroke thickness. The default is <c>2</c>.
		/// </summary>
		/// <value>The marker stroke thickness.</value>
		public double MarkerStrokeThickness { get; set; }

		/// <summary>
		/// Gets or sets the type of the marker. The default is <c>MarkerType.None</c>.
		/// </summary>
		/// <value>The type of the marker.</value>
		/// <remarks>If MarkerType.Custom is used, the MarkerOutline property must be specified.</remarks>
		public MarkerType MarkerType { get; set; }

		/// <summary>
		/// Gets or sets the minimum length of the segment.
		/// Increasing this number will increase performance,
		/// but make the curve less accurate. The default is <c>2</c>.
		/// </summary>
		/// <value>The minimum length of the segment.</value>
		public double MinimumSegmentLength { get; set; }

		/// <summary>
		/// Gets or sets a type of interpolation algorithm used for smoothing this <see cref = "DataPointSeries" />.
		/// </summary>
		/// <value>Type of interpolation algorithm.</value>
		public IInterpolationAlgorithm InterpolationAlgorithm { get; set; }

		/// <summary>
		/// Gets or sets the thickness of the curve.
		/// </summary>
		/// <value>The stroke thickness.</value>
		public double StrokeThickness { get; set; }

		/// <summary>
		/// Gets the actual color.
		/// </summary>
		/// <value>The actual color.</value>
		public OxyColor ActualColor
		{
			get
			{
				return Color.GetActualColor(defaultColor);
			}
		}

		/// <summary>
		/// Gets the actual marker fill color.
		/// </summary>
		/// <value>The actual color.</value>
		public OxyColor ActualMarkerFill
		{
			get
			{
				return MarkerFill.GetActualColor(defaultMarkerFill);
			}
		}

		/// <summary>
		/// Gets the actual line style.
		/// </summary>
		/// <value>The actual line style.</value>
		protected LineStyle ActualLineStyle
		{
			get
			{
				return LineStyle != LineStyle.Automatic ? LineStyle : defaultLineStyle;
			}
		}

		/// <summary>
		/// Gets the actual dash array for the line.
		/// </summary>
		protected double[] ActualDashArray
		{
			get
			{
				return Dashes ?? ActualLineStyle.GetDashArray();
			}
		}

		/// <summary>
		/// Gets the smoothed points.
		/// </summary>
		/// <value>The smoothed points.</value>
		protected List<DataPoint> SmoothedPoints
		{
			get
			{
				return smoothedPoints;
			}
		}

		/// <summary>
		/// Gets the point on the series that is nearest the specified point.
		/// </summary>
		/// <param name="point">The point.</param>
		/// <param name="interpolate">Interpolate the series if this flag is set to <c>true</c>.</param>
		/// <returns>A TrackerHitResult for the current hit.</returns>
		public override TrackerHitResult GetNearestPoint(ScreenPoint point, bool interpolate)
		{
			if (interpolate)
			{
				// Cannot interpolate if there is no line
				if (ActualColor.IsInvisible() || StrokeThickness.Equals(0))
				{
					return null;
				}

				if (!CanTrackerInterpolatePoints)
				{
					return null;
				}
			}

			if (interpolate && InterpolationAlgorithm != null)
			{
				var span = CollectionsMarshal.AsSpan(SmoothedPoints);
				var result = GetNearestInterpolatedPointInternal(span, point);
				if (result != null)
				{
					result.Text = StringHelper.Format(
						ActualCulture,
						TrackerFormatString,
						result.Item,
						Title,
						XAxis.Title ?? DefaultXAxisTitle,
						XAxis.GetValue(result.DataPoint.X),
						YAxis.Title ?? DefaultYAxisTitle,
						YAxis.GetValue(result.DataPoint.Y));
				}

				return result;
			}

			return base.GetNearestPoint(point, interpolate);
		}

		/// <summary>
		/// Renders the series on the specified rendering context.
		/// </summary>
		/// <param name="rc">The rendering context.</param>
		public override void Render(IRenderContext rc)
		{
			//var actualPoints = ActualPoints;
			//if (actualPoints == null || actualPoints.Length == 0)
			//{
			//	return;
			//}

			//VerifyAxes();

			//RenderPoints(rc, actualPoints);

			//if (LabelFormatString != null)
			//{
			//	// render point labels (not optimized for performance)
			//	RenderPointLabels(rc);
			//}

			//if (LineLegendPosition != LineLegendPosition.None && !string.IsNullOrEmpty(Title))
			//{
			//	// renders a legend on the line
			//	RenderLegendOnLine(rc);
			//}
		}

		public override void Render(IRenderContext rc, ref OxyRect rect)
		{
			var actualPoints = ActualPoints;
			if (actualPoints == null || actualPoints.Length == 0)
			{
				return;
			}

			VerifyAxes();

			RenderPoints(rc, actualPoints, ref rect);

			if (LabelFormatString != null)
			{
				// render point labels (not optimized for performance)
				RenderPointLabels(rc);
			}

			if (LineLegendPosition != LineLegendPosition.None && !string.IsNullOrEmpty(Title))
			{
				// renders a legend on the line
				RenderLegendOnLine(rc);
			}
		}

		/// <summary>
		/// Renders the legend symbol for the line series on the
		/// specified rendering context.
		/// </summary>
		/// <param name="rc">The rendering context.</param>
		/// <param name="legendBox">The bounding rectangle of the legend box.</param>
		public override void RenderLegend(IRenderContext rc, OxyRect legendBox)
		{
			double xmid = (legendBox.Left + legendBox.Right) / 2;
			double ymid = (legendBox.Top + legendBox.Bottom) / 2;
			var pts = new[] { new ScreenPoint(legendBox.Left, ymid), new ScreenPoint(legendBox.Right, ymid) };
			rc.DrawLine(
				pts,
				GetSelectableColor(ActualColor),
				StrokeThickness,
				EdgeRenderingMode,
				ActualDashArray);
			var midpt = new ScreenPoint(xmid, ymid);
			rc.DrawMarker(
				midpt,
				MarkerType,
				MarkerOutline,
				MarkerSize,
				ActualMarkerFill,
				MarkerStroke,
				MarkerStrokeThickness,
				EdgeRenderingMode);
		}

		/// <summary>
		/// Sets default values from the plot model.
		/// </summary>
		protected override void SetDefaultValues()
		{
			if (LineStyle == LineStyle.Automatic)
			{
				defaultLineStyle = PlotModel.GetDefaultLineStyle();
			}

			if (Color.IsAutomatic())
			{
				defaultColor = PlotModel.GetDefaultColor();
			}

			if (MarkerFill.IsAutomatic())
			{
				// No color was explicitly provided. Use the line color if it was set, else use default.
				defaultMarkerFill = Color.IsAutomatic() ? defaultColor : Color;
			}
		}

		/// <summary>
		/// Updates the maximum and minimum values of the series.
		/// </summary>
		protected override void UpdateMaxMin()
		{
			if (InterpolationAlgorithm != null)
			{
				// Update the max/min from the control points
				base.UpdateMaxMin();

				// Make sure the smooth points are re-evaluated.
				ResetSmoothedPoints();

				if (SmoothedPoints.All(x => double.IsNaN(x.X)))
				{
					return;
				}

				// Update the max/min from the smoothed points
				MinX = SmoothedPoints.Where(x => !double.IsNaN(x.X)).Min(x => x.X);
				MinY = SmoothedPoints.Where(x => !double.IsNaN(x.Y)).Min(x => x.Y);
				MaxX = SmoothedPoints.Where(x => !double.IsNaN(x.X)).Max(x => x.X);
				MaxY = SmoothedPoints.Where(x => !double.IsNaN(x.Y)).Max(x => x.Y);
			}
			else
			{
				base.UpdateMaxMin();
			}
		}

		/// <summary>
		/// Renders the points as line, broken line and markers.
		/// </summary>
		/// <param name="rc">The rendering context.</param>
		/// <param name="points">The points to render.</param>
		protected void RenderPoints(IRenderContext rc, ReadOnlySpan<DataPoint> points, ref OxyRect rect)
		{
			var lastValidPoint = new ScreenPoint?();
			var areBrokenLinesRendered = BrokenLineThickness > 0 && BrokenLineStyle != LineStyle.None;
			var dashArray = areBrokenLinesRendered ? BrokenLineStyle.GetDashArray() : null;
			var broken = areBrokenLinesRendered ? new List<ScreenPoint>(2) : null;

			if (contiguousScreenPointsBuffer == null)
			{
				contiguousScreenPointsBuffer = new List<ScreenPoint>(points.Length);
			}

			int startIdx = 0;
			double xmax = double.MaxValue;

			if (IsXMonotonic)
			{
				// determine render range
				var xmin = XAxis.ClipMinimum;
				xmax = XAxis.ClipMaximum;
				WindowStartIndex = UpdateWindowStartIndex<DataPoint>(points, point => point.X, xmin, WindowStartIndex);

				startIdx = WindowStartIndex;
			}

			for (int i = startIdx; i < points.Length; i++)
			{
				if (!ExtractNextContiguousLineSegment(points, ref i, ref lastValidPoint, xmax, broken, contiguousScreenPointsBuffer))
				{
					break;
				}

				if (areBrokenLinesRendered)
				{
					if (broken.Count > 0)
					{
						var actualBrokenLineColor = BrokenLineColor.IsAutomatic()
														? ActualColor
														: BrokenLineColor;

						rc.DrawLineSegments(
							broken,
							actualBrokenLineColor,
							BrokenLineThickness,
							EdgeRenderingMode,
							dashArray,
							LineJoin);
						broken.Clear();
					}
				}
				else
				{
					lastValidPoint = null;
				}

				if (Decimator != null)
				{
					if (decimatorBuffer == null)
					{
						decimatorBuffer = new List<ScreenPoint>(contiguousScreenPointsBuffer.Count);
					}
					else
					{
						decimatorBuffer.Clear();
					}

					Decimator(contiguousScreenPointsBuffer, decimatorBuffer);
					RenderLineAndMarkers(rc, decimatorBuffer, ref rect);
				}
				else
				{
					RenderLineAndMarkers(rc, contiguousScreenPointsBuffer, ref rect);
				}

				contiguousScreenPointsBuffer.Clear();
			}
		}

		/// <summary>
		/// Extracts a single contiguous line segment beginning with the element at the position of the enumerator when the method
		/// is called. Initial invalid data points are ignored.
		/// </summary>
		/// <param name="pointIdx">Current point index</param>
		/// <param name="previousContiguousLineSegmentEndPoint">Initially set to null, but I will update I won't give a broken line if this is null</param>
		/// <param name="xmax">Maximum visible X value</param>
		/// <param name="broken">place to put broken segment</param>
		/// <param name="contiguous">place to put contiguous segment</param>
		/// <param name="points">Points collection</param>
		/// <returns>
		///   <c>true</c> if line segments are extracted, <c>false</c> if reached end.
		/// </returns>
		protected bool ExtractNextContiguousLineSegment(
			ReadOnlySpan<DataPoint> points,
			ref int pointIdx,
			ref ScreenPoint? previousContiguousLineSegmentEndPoint,
			double xmax,
			// ReSharper disable SuggestBaseTypeForParameter
			List<ScreenPoint> broken,
			List<ScreenPoint> contiguous)
		// ReSharper restore SuggestBaseTypeForParameter
		{
			DataPoint currentPoint = default;
			bool hasValidPoint = false;

			// Skip all undefined points
			for (; pointIdx < points.Length; pointIdx++)
			{
				currentPoint = points[pointIdx];
				if (currentPoint.X > xmax)
				{
					return false;
				}

				// ReSharper disable once AssignmentInConditionalExpression
				if (hasValidPoint = IsValidPoint(currentPoint))
				{
					break;
				}
			}

			if (!hasValidPoint)
			{
				return false;
			}

			// First valid point
			var screenPoint = Transform(currentPoint);

			// Handle broken line segment if exists
			if (previousContiguousLineSegmentEndPoint.HasValue)
			{
				broken.Add(previousContiguousLineSegmentEndPoint.Value);
				broken.Add(screenPoint);
			}

			// Add first point
			contiguous.Add(screenPoint);

			// Add all points up until the next invalid one
			int clipCount = 0;
			for (pointIdx++; pointIdx < points.Length; pointIdx++)
			{
				currentPoint = points[pointIdx];
				clipCount += currentPoint.X > xmax ? 1 : 0;
				if (clipCount > 1)
				{
					break;
				}
				if (!IsValidPoint(currentPoint))
				{
					break;
				}

				screenPoint = Transform(currentPoint);
				contiguous.Add(screenPoint);
			}

			previousContiguousLineSegmentEndPoint = screenPoint;

			return true;
		}

		/// <summary>
		/// Renders the point labels.
		/// </summary>
		/// <param name="rc">The render context.</param>
		protected void RenderPointLabels(IRenderContext rc)
		{
			int index = -1;
			foreach (var point in ActualPoints)
			{
				index++;

				if (!IsValidPoint(point))
				{
					continue;
				}

				var pt = Transform(point) + new ScreenVector(0, -LabelMargin);

				var item = GetItem(index);
				var s = StringHelper.Format(ActualCulture, LabelFormatString, item, point.X, point.Y);

#if SUPPORTLABELPLACEMENT
                    switch (this.LabelPlacement)
                    {
                        case LabelPlacement.Inside:
                            pt = new ScreenPoint(rect.Right - this.LabelMargin, (rect.Top + rect.Bottom) / 2);
                            ha = HorizontalAlignment.Right;
                            break;
                        case LabelPlacement.Middle:
                            pt = new ScreenPoint((rect.Left + rect.Right) / 2, (rect.Top + rect.Bottom) / 2);
                            ha = HorizontalAlignment.Center;
                            break;
                        case LabelPlacement.Base:
                            pt = new ScreenPoint(rect.Left + this.LabelMargin, (rect.Top + rect.Bottom) / 2);
                            ha = HorizontalAlignment.Left;
                            break;
                        default: // Outside
                            pt = new ScreenPoint(rect.Right + this.LabelMargin, (rect.Top + rect.Bottom) / 2);
                            ha = HorizontalAlignment.Left;
                            break;
                    }
#endif

				rc.DrawText(
					pt,
					s,
					ActualTextColor,
					ActualFont,
					ActualFontSize,
					ActualFontWeight,
					0,
					HorizontalAlignment.Center,
					VerticalAlignment.Bottom);
			}
		}

		/// <summary>
		/// Renders a legend on the line.
		/// </summary>
		/// <param name="rc">The render context.</param>
		protected void RenderLegendOnLine(IRenderContext rc)
		{
			// Find the position
			DataPoint point;
			HorizontalAlignment ha;
			var va = VerticalAlignment.Middle;
			double dx = 4;

			switch (LineLegendPosition)
			{
				case LineLegendPosition.Start:
					point = ActualPoints[0];
					ha = HorizontalAlignment.Right;
					dx = -dx;
					break;
				case LineLegendPosition.End:
					point = ActualPoints[ActualPoints.Length - 1];
					ha = HorizontalAlignment.Left;
					break;
				case LineLegendPosition.None:
					return;
				default:
					throw new ArgumentOutOfRangeException();
			}

			this.Orientate(ref ha, ref va);
			var pt = Transform(point) + this.Orientate(new ScreenVector(dx, 0));

			// Render the legend
			rc.DrawText(
				pt,
				Title,
				ActualTextColor,
				ActualFont,
				ActualFontSize,
				ActualFontWeight,
				0,
				ha,
				va);
		}

		/// <summary>
		/// Renders the transformed points as a line (smoothed if <see cref="InterpolationAlgorithm"/> isn’t <c>null</c>) and markers (if <see cref="MarkerType"/> is not <c>None</c>).
		/// </summary>
		/// <param name="rc">The render context.</param>
		/// <param name="pointsToRender">The points to render.</param>
		protected virtual void RenderLineAndMarkers(IRenderContext rc, IList<ScreenPoint> pointsToRender, ref OxyRect rect)
		{
			var screenPoints = pointsToRender;
			if (InterpolationAlgorithm != null)
			{
				// spline smoothing (should only be used on small datasets)
				var resampledPoints = ScreenPointHelper.ResamplePoints(pointsToRender, MinimumSegmentLength);
				screenPoints = InterpolationAlgorithm.CreateSpline(resampledPoints, false, 0.25);
			}

			// clip the line segments with the clipping rectangle
			if (StrokeThickness > 0 && ActualLineStyle != LineStyle.None)
			{
				RenderLine(rc, screenPoints, ref rect);
			}

			if (MarkerType != MarkerType.None)
			{
				var markerBinOffset = MarkerResolution > 0 ? this.Transform(MinX, MinY) : default;

				rc.DrawMarkers(
					pointsToRender,
					MarkerType,
					MarkerOutline,
					new[] { MarkerSize },
					ActualMarkerFill,
					MarkerStroke,
					MarkerStrokeThickness,
					EdgeRenderingMode,
					MarkerResolution,
					markerBinOffset);
			}
		}

		/// <summary>
		/// Renders a continuous line.
		/// </summary>
		/// <param name="rc">The render context.</param>
		/// <param name="pointsToRender">The points to render.</param>
		protected virtual void RenderLine(IRenderContext rc, IList<ScreenPoint> pointsToRender, ref OxyRect rect)
		{
			var dashArray = ActualDashArray;

			if (outputBuffer == null)
			{
				outputBuffer = new List<ScreenPoint>(pointsToRender.Count);
			}

			rc.DrawReducedLine(
				pointsToRender,
				MinimumSegmentLength * MinimumSegmentLength,
				GetSelectableColor(ActualColor),
				StrokeThickness,
				EdgeRenderingMode,
				dashArray,
				LineJoin,
				ref rect,
				outputBuffer);
		}

		/// <summary>
		/// Force the smoothed points to be re-evaluated.
		/// </summary>
		protected virtual void ResetSmoothedPoints()
		{
			double tolerance = Math.Abs(Math.Max(MaxX - MinX, MaxY - MinY) / ToleranceDivisor);
			smoothedPoints = InterpolationAlgorithm.CreateSpline(ActualPoints, false, tolerance);
		}

		/// <summary>
		/// Represents a line segment.
		/// </summary>
		protected class Segment
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="Segment" /> class.
			/// </summary>
			/// <param name="point1">The first point of the segment.</param>
			/// <param name="point2">The second point of the segment.</param>
			public Segment(DataPoint point1, DataPoint point2)
			{
				Point1 = point1;
				Point2 = point2;
			}

			/// <summary>
			/// Gets the first point1 of the segment.
			/// </summary>
			public DataPoint Point1 { get; private set; }

			/// <summary>
			/// Gets the second point of the segment.
			/// </summary>
			public DataPoint Point2 { get; private set; }
		}
	}
}
