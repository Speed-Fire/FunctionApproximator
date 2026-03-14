using FunctionApproximator.Misc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApproximator.Approximators
{
	public partial class LeastSquareApproximator : IFunctionApproximator
	{
		[StructLayout(LayoutKind.Sequential)]
		struct Array
		{
			public IntPtr variables; // double pointer
			public nuint length;
		}

		private int _polynomialDegree;

		public IReadOnlyList<double> PolynomCoefficients { get; private set; } = [];

		public int PolynomialDegree 
		{
			get => _polynomialDegree; 
			set => _polynomialDegree = value; 
		}

		#region Polynom

		private bool _hasPolynom = false;
		private Array _polynom;

		#endregion

		#region Graph

		private Array _graph;

		#endregion

		#region Controlling

		public void Initialize(int samplingDensity)
		{
			NativeInitialize(samplingDensity);
		}

		public void SetSamplingDensity(int samplingDensity)
		{
			NativeSetSamplingDensity(samplingDensity);
		}

		#endregion

		#region Approximation

		public unsafe void Approximate(double[] data)
		{
			FreePolynom();

			fixed(double* ptr = data)
			{
				var array = new Array()
				{
					variables = (IntPtr)ptr,
					length = (nuint)data.Length,
				};

				_polynom = NativeCreateLsPolynom(ref array, PolynomialDegree);
			}

			_hasPolynom = true;
			RetrievePolynomCoefficients();
		}

		public Memory<double> Draw(double from, double to)
		{
			_graph = NativeDrawGraph(ref _polynom, from, to);

			var memManager = new UnmanagedDoubleMemoryManager(_graph.variables, _graph.length);
			var memory = memManager.Memory;

			return memory;
		}

		public void Clear()
		{
			FreePolynom();
		}

		private void RetrievePolynomCoefficients()
		{
			if (!_hasPolynom)
				return;

			var memManager = new UnmanagedDoubleMemoryManager(_polynom.variables, _polynom.length);
			var memory = memManager.Memory;
			var span = memory.Span;

			var list = new List<double>();
			for(int i = 0; i < span.Length; i++)
			{
				list.Add(span[i]);
			}

			PolynomCoefficients = list;
		}

		private void FreePolynom()
		{
			if (_hasPolynom)
			{
				NativeFreeArray(ref _polynom);
				_hasPolynom = false;
			}
		}

		#endregion

		#region Native

		private const string DllName = "ApproximationLib.dll";

		[LibraryImport(DllName, EntryPoint = "Initialize")]
		[UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
		private static partial void NativeInitialize(int samplingDensity);

		[LibraryImport(DllName, EntryPoint = "SetSamplingDensity")]
		[UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
		private static partial void NativeSetSamplingDensity(int samplingDensity);

		[LibraryImport(DllName, EntryPoint = "Dispose")]
		[UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
		private static partial void NativeDispose();

		[LibraryImport(DllName, EntryPoint = "CreateLsPolynom")]
		[UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
		private static partial Array NativeCreateLsPolynom(ref Array data, int degree);

		[LibraryImport(DllName, EntryPoint = "DrawGraph")]
		[UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
		private static partial Array NativeDrawGraph(ref Array solution, double from, double to);

		[LibraryImport(DllName, EntryPoint = "FreeArray")]
		[UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
		static partial void NativeFreeArray(ref Array solution);

		#endregion

		#region Dispose

		private bool disposedValue;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					
				}

				Clear();
				NativeDispose();
				disposedValue = true;
			}
		}

		~LeastSquareApproximator()
		{
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
