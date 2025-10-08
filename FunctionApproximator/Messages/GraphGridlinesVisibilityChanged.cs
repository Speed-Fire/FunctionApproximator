using CommunityToolkit.Mvvm.Messaging.Messages;
using FunctionApproximator.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApproximator.Messages
{
	internal class GraphGridlinesVisibilityChanged : ValueChangedMessage<GridlineVisibility>
	{
		public GraphGridlinesVisibilityChanged(GridlineVisibility value) : base(value)
		{
		}
	}
}
