/*
Net20Compat.cs

Author:
      César Andrés Morgan <xds_xps_ivx@hotmail.com>

Copyright (c) 2019-2020 César Andrés Morgan

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

#if NETFX20
using System.Collections.Generic;

namespace System
{
    public delegate void Action();
}

namespace System.Linq
{
    /// <summary>
    /// Incluye un sub-set de las funciones de Linq utilizadas por Sneik.
    /// </summary>
    internal static class Enumerable
    {

        public static bool Any<TSource>(IEnumerable<TSource> source)
        {
            return source.GetEnumerator().MoveNext();          
        }

        public static bool Any<TSource>(IEnumerable<TSource> source, Predicate<TSource> predicate)
        {
            foreach (var j in source)
            {
                if (predicate.Invoke(j)) return true;
            }
            return false;
        }

        public static TSource First<TSource>(IEnumerable<TSource> source)
        {
            var e = source.GetEnumerator();
            if (e.MoveNext()) return e.Current;
            throw new InvalidOperationException();
        }

        public static TSource Last<TSource>(IEnumerable<TSource> source)
        {
            var e = source.GetEnumerator();
            TSource retVal = default;
            var done = false;
            while (e.MoveNext()) { retVal = e.Current; done = true; }
            return done ? retVal : throw new InvalidOperationException();
        }

        public static TSource FirstOrDefault<TSource>(IEnumerable<TSource> source, Predicate<TSource> predicate)
        {
            foreach (var j in source)
            {
                if (predicate.Invoke(j)) return j;
            }
            return default;
        }
    }
}
#endif