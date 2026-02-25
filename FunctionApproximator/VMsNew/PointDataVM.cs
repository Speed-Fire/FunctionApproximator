using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApproximator.VMsNew
{
	internal partial class PointDataVM : ObservableObject
	{
		private readonly ObservableCollection<PointVM> _points = [];
		public ObservableCollection<PointVM> Points => _points;

		[ObservableProperty]
		private int _pointCount;

		[ObservableProperty]
		private bool _hasSameX;

		[ObservableProperty]
		private bool _invalidPointData;

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
		}

		[RelayCommand]
		private void DeletePoint(PointVM point)
		{
			RemovePoint(point);
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
		}

		#endregion

		#region Internal

		public void AddPoint(string x, string y)
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

		#endregion
	}
}
