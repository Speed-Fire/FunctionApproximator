using CommunityToolkit.Mvvm.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApproximator.Messages
{
	class GraphAccordanceChanged : ValueChangedMessage<bool>
	{
		public GraphAccordanceChanged(bool value) : base(value)
		{
		}
	}
}
