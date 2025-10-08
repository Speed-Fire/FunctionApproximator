using CommunityToolkit.Mvvm.Messaging.Messages;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApproximator.Messages
{
	public class GraphColorChanged : ValueChangedMessage<OxyColor>
	{
		public bool IsApproximatedGraph { get; }

		public GraphColorChanged(OxyColor value, bool isApproximatedGraph) : base(value)
		{
			IsApproximatedGraph = isApproximatedGraph;
		}
	}
}
