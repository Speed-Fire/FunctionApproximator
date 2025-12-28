using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FunctionApproximator.Components;
using FunctionApproximator.Extensions;
using FunctionApproximator.Messages;
using FunctionApproximator.Misc;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FunctionApproximator.ViewModels
{
    public partial class Point : ObservableValidator
    {
        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [CustomValidation(typeof(Point), nameof(ValidateDouble))]
        public string _x = "0";

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [CustomValidation(typeof(Point), nameof(ValidateDouble))]
        public string _y = "0";

		public static ValidationResult ValidateDouble(string value)
        {
            if (!double.TryParse(value, out _))
                return new("Incorrect format.");
            return ValidationResult.Success!;
        }
	}

    public partial class PointsInputVM : ObservableObject
    {
		private static readonly PlotError _sameXError = new("You have points with the same X.");

        [ObservableProperty]
        private ObservableCollection<Point> _points = [];

		[ObservableProperty]
		private bool _isDataReady = false;

		public event Action? InputDataChanged;

		public PointsInputVM()
		{
            _points.CollectionChanged += (sender, e) =>
            {
				ClearPointsCommand.NotifyCanExecuteChanged();
                UpdateDataReadyness();
				InputDataChanged?.Invoke();
            };
		}

		#region Commands

		[RelayCommand]
        private void CreatePoint()
        {
            AddPoint("0", "0");
        }

		[RelayCommand]
        private void DeletePoint(Point point)
        {
            if (Points.Count > 1)
            {
                var pos = Points.IndexOf(point);
                var id = point.Id;

                for(int i = pos + 1; i < Points.Count; i++)
                    Points[i].Id = id++;
            }

			point.ErrorsChanged -= Point_ErrorsChanged;
			point.PropertyChanged -= Point_PropertyChanged;
			Points.Remove(point);
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

		[RelayCommand]
		private void LoadFile()
		{
			if (!SelectFile(out var file))
				return;

			try
			{
				var data = FileParser.ReadCsv(file);

				ClearPointsCommand.Execute(null);
				foreach (var arr in data)
				{
					AddPoint(arr[0], arr[1]);
				}
			}
			catch (Exception ex)
			{
				this.ShowError(ex.Message);
			}
		}

		#endregion

		#region Internal

		public void AddPoint(string x, string y)
        {
			var point = new Point()
			{
				Id = Points.Count + 1,
                X = x,
                Y = y
			};

			point.ErrorsChanged += Point_ErrorsChanged;
			point.PropertyChanged += Point_PropertyChanged;
			Points.Add(point);
		}

		public double[] GetPoints()
        {
            var points = new double[Points.Count * 2];

            var i = 0;
            foreach (var point in Points.OrderBy(p => p.X))
            {
                points[i] = double.Parse(point.X);
                points[i + 1] = double.Parse(point.Y);
                i += 2;
            }

            return points;
        }

		public (double minX, double maxX, double minY, double maxY) GetInputPointsWindow()
		{
			var minX = Points.Select(p => double.Parse(p.X)).Min();
			var maxX = Points.Select(p => double.Parse(p.X)).Max();
			var minY = Points.Select(p => double.Parse(p.Y)).Min();
			var maxY = Points.Select(p => double.Parse(p.Y)).Max();

			return (minX, maxX, minY, maxY);
		}
			
        private void UpdateDataReadyness()
        {
			var val =
				Points.Count >= 2 &&
				!Points.Any(x => x.HasErrors);

			var hasSameX = Points.DistinctBy(p => p.X).Count() != Points.Count;
			WeakReferenceMessenger.Default.Send(new ChangeErrorMessage(_sameXError, !hasSameX));

			IsDataReady = val && !hasSameX;
        }

		private bool SelectFile(out string file)
		{
			file = string.Empty;

			var fl = new OpenFileDialog()
			{
				Multiselect = false,
				Title = "Select data file"
			};

			if (fl.ShowDialog() != true)
				return false;

			var f = fl.FileName;
			if (!File.Exists(f))
			{
				this.ShowError("File does not exist!");
				return false;
			}

			file = f;
			return true;
		}

		#endregion

		#region Event hadnlers

		private void Point_ErrorsChanged(object? sender, DataErrorsChangedEventArgs e)
		{
			UpdateDataReadyness();
		}

		private void Point_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			UpdateDataReadyness();
			InputDataChanged?.Invoke();
		}

		#endregion
	}
}
