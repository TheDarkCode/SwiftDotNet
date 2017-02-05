using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Swift.Net.Extensions
{
    public static class ListExtensions
    {
        public static void ReplaceOrAdd<T>(this List<T> list, Predicate<T> selector, T newItem)
        {
            var index = list.FindIndex(selector);
            if (index != -1)
            {
                list[index] = newItem;
            }
            else
            {
                list.Add(newItem);
            }
        }
        public static void ReplaceOrAddRange<T>(this List<T> list, Predicate<T> selector, List<T> newItems)
        {
            foreach (var item in newItems)
            {
                var index = list.FindIndex(selector);
                if (index != -1)
                {
                    list[index] = item;
                }
                else
                {
                    list.Add(item);
                }
            }
        }
    }
}
