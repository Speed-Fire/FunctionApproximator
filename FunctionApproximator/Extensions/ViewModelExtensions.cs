using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FunctionApproximator.Extensions
{
    public static class ViewModelExtensions
    {
        public static void ShowError(this ObservableObject _, string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static MessageBoxResult ShowQuestion(this ObservableObject _, string title, string message)
        {
            return
                MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
        }

        public static void ShowError(this ObservableObject obj, string message)
        {
            ShowError(obj, "Error", message);
        }

		public static MessageBoxResult ShowQuestion(this ObservableObject obj, string message)
		{
			return ShowQuestion(obj, "Question", message);
		}
	}
}
