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
		struct EqualitySolution
		{
			public IntPtr variables; // double pointer
			public int length;
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
		private EqualitySolution _polynom;

		#endregion

		#region Graph

		private bool _hasGraph = false;
		private EqualitySolution _graph;

		#endregion

		#region Approximation

		public void Approximate(double[] data)
		{
			FreePolynom();
			FreeGraph();

			_polynom = CreateLskPolynom(data, (nuint)data.Length, PolynomialDegree);
			_hasPolynom = true;
			FillPolynomRetrievePolynomCoefficients();
		}

		public Memory<double> Draw(double from, double to, double step)
		{
			FreeGraph();

			_graph = DrawGraph(ref _polynom, from, to, step);
			_hasGraph = true;

			var memManager = new UnmanagedDoubleMemoryManager(_graph.variables, _graph.length);
			var memory = memManager.Memory;

			return memory;
		}

		public void Clear()
		{
			FreePolynom();
			FreeGraph();
		}

		private void FillPolynomRetrievePolynomCoefficients()
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
				FreeEqualitySolution(ref _polynom);
				_hasPolynom = false;
			}
		}

		private void FreeGraph()
		{
			if (_hasGraph)
			{
				FreeEqualitySolution(ref _graph);
				_hasGraph = false;
			}
		}

		#endregion

		#region Native

		private const string DllName = "ApproximationLib.dll";

		[LibraryImport(DllName)]
		[UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
		private static partial EqualitySolution CreateLskPolynom(double[] data, nuint length, int degree);

		[LibraryImport(DllName)]
		[UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
		private static partial EqualitySolution DrawGraph(ref EqualitySolution solution,
			double from, double to, double step);

		[LibraryImport(DllName)]
		[UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
		static partial void FreeEqualitySolution(ref EqualitySolution solution);

		#endregion

		#region Dispose

		private bool disposedValue;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: освободить управляемое состояние (управляемые объекты)
				}

				// TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
				// TODO: установить значение NULL для больших полей
				Clear();
				disposedValue = true;
			}
		}

		// TODO: переопределить метод завершения, только если "Dispose(bool disposing)" содержит код для освобождения неуправляемых ресурсов
		~LeastSquareApproximator()
		{
			// Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
			Dispose(disposing: false);
		}

		public void Dispose()
		{
			// Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
