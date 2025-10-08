using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApproximator.Messages
{
	class GraphVisibilityChanged : ValueChangedMessage<bool>
	{
		public bool IsApproximatedGraph { get; }

		public GraphVisibilityChanged(bool value, bool isApproximatedGraph) : base(value)
		{
			IsApproximatedGraph = isApproximatedGraph;
		}
	}
}
