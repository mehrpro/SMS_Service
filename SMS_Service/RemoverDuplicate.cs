using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS_Service
{
    public static class RemoverDuplicate
    {
        public static List<T> RemoveDuplicates<T>(this List<T> items)
        {
            return (from s in items select s).Distinct().ToList();
        }
    }
}
