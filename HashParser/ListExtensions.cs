using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace HashParser
{
    /// <summary>
    /// Helper methods.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Count - 1.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LastIndex(this IList col) => col.Count - 1;

        /// <summary>
        /// Fast removal from a list. Only use this if the order of items in <paramref name="src"/>
        /// doesn't matter.
        /// </summary>
        public static void QuickRemove<T>(this List<T> src, T item)
        {
            int index = src.IndexOf(item);

            if (index >= 0)
                QuickRemove(src, index);
        }

        /// <summary>
        /// Fast removal from a list. Only use this if the order of items in <paramref name="src"/>
        /// doesn't matter.
        /// </summary>
        public static void QuickRemove<T>(this List<T> src, int index)
        {
            src.Swap(index, src.LastIndex());
            src.RemoveLast();
        }

        /// <summary>
        /// Swap element at index a with element at index b.
        /// </summary>
        public static void Swap<T>(this IList<T> list, int a, int b)
        {
            //perform swap
            T tmp = list[a];
            list[a] = list[b];
            list[b] = tmp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveFirst<T>(this List<T> list)
            => list.RemoveAt(0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveLast<T>(this List<T> list)
            => list.RemoveAt(list.Count - 1);
    }
}
