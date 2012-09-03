using System;
using System.Collections.Generic;
using System.Linq;

namespace MoneyBurnDown.Extensions
{
    public static class EnumerableExtensions
    {
         public static double Multiplication(this IEnumerable<double> source)
         {
             return source.Aggregate<double, double>(1, (current, d) => current*d);
         }
    }
}