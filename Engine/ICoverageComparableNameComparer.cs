using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Engine
{
    class ICoverageComparableNameComparer : IEqualityComparer<ICoverageComparable>
    {
        public bool Equals([AllowNull] ICoverageComparable x, [AllowNull] ICoverageComparable y)
        {
            if (x != null && y != null)
            {
                return x.Name == y.Name;
            }
            return false;
        }

        public int GetHashCode([DisallowNull] ICoverageComparable obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
