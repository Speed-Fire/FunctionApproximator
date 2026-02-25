using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FunctionApproximator.Extensions;
using FunctionApproximator.Misc;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApproximator.VMsNew
{
	internal partial class FileLoaderVM : ObservableObject
	{
		private readonly PointDataVM _pointData;

		public FileLoaderVM(PointDataVM pointData)
		{
			_pointData = pointData;
		}

		[RelayCommand]
		private void LoadFile()
		{
			if (!SelectFile(out var file))
				return;

			try
			{
				var data = FileParser.ReadCsv(file);

				_pointData.ClearPointsCommand.Execute(null);
				foreach (var arr in data)
				{
					_pointData.AddPoint(arr[0], arr[1]);
				}
			}
			catch (Exception ex)
			{
				this.ShowError(ex.Message);
			}
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
	}
}
