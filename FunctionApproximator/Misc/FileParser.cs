using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApproximator.Misc
{
    public static class FileParser
    {
        public static IEnumerable<string[]> ReadCsv(string path, char delimiter = ';')
        {
            var text = File.ReadAllText(path);
            var result = text
                .Split(Environment.NewLine, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Select(x =>
            {
                var sp = x.Split(delimiter, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (sp.Length != 2)
                    throw new InvalidOperationException("Incorrect file!");
                return sp;
            });

            return result;
        }

        public static double[] ReadCsvToDouble(string path, char delimiter = ';')
        {
            var rawValues = ReadCsv(path, delimiter);

            var values = new double[rawValues.Count() * 2];
            var i = 0;
            foreach(var arr in rawValues)
            {
                values[i] = double.Parse(arr[0]);
                values[i + 1] = double.Parse(arr[1]);
                i += 2;
            }

            return values;
        }
    }
}
