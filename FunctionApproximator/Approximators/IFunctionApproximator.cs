using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApproximator.Approximators
{
    public interface IFunctionApproximator : IDisposable
    {
        IReadOnlyList<double> PolynomCoefficients { get; }
        int PolynomialDegree { get; set; }

		void Approximate(double[] data);
        Memory<double> Draw(double from, double to, double step);
        void Clear();
    }
}
