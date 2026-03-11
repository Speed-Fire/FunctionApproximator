using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApproximator.ViewModels
{
	internal partial class PointDataVM : ObservableObject
	{
		private readonly ObservableCollection<PointVM> _points = [];
		public ObservableCollection<PointVM> Points => _points;

		public event Action? PointsChanged;

		[ObservableProperty]
		private int _pointCount;

		[ObservableProperty]
		private bool _hasSameX;

		[ObservableProperty]
		private bool _invalidPointData;

		[ObservableProperty]
		private bool _invalidXOrder;

		public PointDataVM()
		{
			_points.CollectionChanged += (sndr, e) =>
			{
				PointCount = _points.Count;
				ClearPointsCommand.NotifyCanExecuteChanged();
			};
		}

		#region Commands

		[RelayCommand]
		private void CreatePoint()
		{
			AddPoint("0", "0");
			HasSameX = _points.DistinctBy(p => p.X).Count() != _points.Count;
			CheckXOrder();
			PointsChanged?.Invoke();
		}

		[RelayCommand]
		private void DeletePoint(PointVM point)
		{
			RemovePoint(point);
			HasSameX = _points.DistinctBy(p => p.X).Count() != _points.Count;
			CheckXOrder();
			PointsChanged?.Invoke();
		}

		[RelayCommand(CanExecute = nameof(CanClearPointsExecute))]
		private void ClearPoints()
		{
			foreach (var point in Points)
			{
				point.ErrorsChanged -= Point_ErrorsChanged;
				point.PropertyChanged -= Point_PropertyChanged;
			}
			Points.Clear();
			PointsChanged?.Invoke();
			HasSameX = InvalidPointData = InvalidXOrder = false;
		}

		private bool CanClearPointsExecute()
		{
			return Points.Count > 0;
		}

		#endregion

		#region Event hadnlers

		private void Point_ErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
		{
			InvalidPointData = _points.Any(p => p.HasErrors);
		}

		private void Point_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			HasSameX = _points.DistinctBy(p => p.X).Count() != _points.Count;
			CheckXOrder();
			PointsChanged?.Invoke();
		}

		#endregion

		#region Internal

		private void AddPoint(string x, string y)
		{
			var point = new PointVM()
			{
				Id = Points.Count + 1,
				X = x,
				Y = y
			};

			point.ErrorsChanged += Point_ErrorsChanged;
			point.PropertyChanged += Point_PropertyChanged;
			Points.Add(point);
		}

		private void RemovePoint(PointVM point)
		{
			if (Points.Count > 1)
			{
				var pos = Points.IndexOf(point);
				var id = point.Id;

				for (int i = pos + 1; i < Points.Count; i++)
					Points[i].Id = id++;
			}

			point.ErrorsChanged -= Point_ErrorsChanged;
			point.PropertyChanged -= Point_PropertyChanged;
			Points.Remove(point);
		}

		private void CheckXOrder()
		{
			if (_points.Count == 0)
				return;

			var first = _points.FirstOrDefault(p => !p.HasErrors);
			if (first is null)
				return;

			double prev = double.Parse(first.X);
			for(int i = first.Id; i < _points.Count; i++)
			{
				if (_points[i].HasErrors)
					continue;

				var cur = double.Parse(_points[i].X);
				if(cur <= prev)
				{
					InvalidXOrder = true;
					return;
				}
				prev = cur;
			}
			InvalidXOrder = false;
		}

		#endregion

		public void AddPointRange(IEnumerable<Tuple<string, string>> points)
		{
			foreach(var point in points)
			{
				AddPoint(point.Item1, point.Item2);
			}

			HasSameX = _points.DistinctBy(p => p.X).Count() != _points.Count;
			CheckXOrder();
			PointsChanged?.Invoke();
		}
	}
}
