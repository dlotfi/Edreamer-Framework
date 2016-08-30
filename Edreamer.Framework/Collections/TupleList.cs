using System;
using System.Collections.Generic;

namespace Edreamer.Framework.Collections
{
    public class TupleList<T1, T2> : List<Tuple<T1, T2>>
    {
        public void Add(T1 item, T2 item2)
        {
            Add(new Tuple<T1, T2>(item, item2));
        }
    }

    public class TupleList<T> : TupleList<T, T>
    {
        public static implicit operator TupleList<T>(T[,] value)
        {
            var tupleList = new TupleList<T>();
            for (int i = value.GetLowerBound(0); i <= value.GetUpperBound(0); i++)
            {
                tupleList.Add(value[i, 0], value[i, 1]);
            }
            return tupleList;
        }
    }
}
