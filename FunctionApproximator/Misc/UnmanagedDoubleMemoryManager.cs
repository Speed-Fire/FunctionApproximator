using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApproximator.Misc
{
	public class UnmanagedDoubleMemoryManager : MemoryManager<double>
	{
		private readonly IntPtr _handle;
		private readonly nuint _length;

		public UnmanagedDoubleMemoryManager(nint handle, nuint length)
		{
			_handle = handle;
			_length = length;
		}

		public override Span<double> GetSpan()
		{
			unsafe
			{
				return new Span<double>((void*)_handle, (int)_length);
			}
		}

		public override MemoryHandle Pin(int elementIndex = 0)
		{
			if(elementIndex < 0 || elementIndex >= (int)_length)
				throw new ArgumentOutOfRangeException(nameof(elementIndex));

			IntPtr pointer = _handle + elementIndex * sizeof(double);
			unsafe
			{
				return new MemoryHandle((void*)pointer);
			}
		}

		public override void Unpin()
		{
			
		}

		protected override void Dispose(bool disposing)
		{
			
		}
	}
}
