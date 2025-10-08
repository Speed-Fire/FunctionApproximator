using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApproximator.Domain.Interfaces
{
    public interface IFunctionApproximator : IDisposable
    {
        string Name { get; }
        string Representation { get; }
        int PolynomialDegree { get; set; }

		void Approximate(double[] data);
        Memory<double> Draw(double from, double to, double step);
        void Clear();

        string? ToString() => Name;
    }
}
