using CommunityToolkit.Mvvm.Messaging.Messages;
using FunctionApproximator.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApproximator.Messages
{
	internal class ChangeErrorMessage : ValueChangedMessage<PlotError>
	{
		public bool IsRemove { get; }

		public ChangeErrorMessage(PlotError value, bool isRemove) : base(value)
		{
			IsRemove = isRemove;
		}
	}
}
