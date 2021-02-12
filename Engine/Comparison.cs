using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Engine
{
    public class Comparison
    {
        protected Dictionary<string, ICoverageComparable> leftInnerComparablesByName;
        protected Dictionary<string, ICoverageComparable> rightInnerComparablesByName;
        private List<(ICoverageComparable innerComparableFromOne, ICoverageComparable innerComparableFromTwo, double delta)> innerComparablesWithDifferentCoverage;
        public ICoverageComparable[] LeftComparables { get; }
        public ICoverageComparable[] RightComparables { get; }
        public CompareBy ComparisonType { get; }

        private static readonly ICoverageComparableNameComparer NameComparer = new ICoverageComparableNameComparer();

        public Comparison(ICoverageComparable one, ICoverageComparable two, CompareBy comparisonType) :this(one.InnerComparables, two.InnerComparables, comparisonType)
        {
        }

        protected Comparison(ICoverageComparable[] leftComparables, ICoverageComparable[] rightComparables, CompareBy comparisonType)
        {
            LeftComparables = leftComparables;
            RightComparables = rightComparables;
            ComparisonType = comparisonType;
            // Seeing duplicate named comparables in these lists, for now, ensuring the list is distinct, by Name, before creating Dictionary.
            leftInnerComparablesByName = leftComparables?.DistinctBy(ic => ic.Name).ToDictionary(ic => ic.Name);
            rightInnerComparablesByName = rightComparables?.DistinctBy(ic => ic.Name).ToDictionary(ic => ic.Name);
        }

        public IEnumerable<ICoverageComparable> InnerComparablesExclusiveToLeft
        {
            get
            {
                if(RightComparables == null)
                {
                    return LeftComparables;
                }
                return LeftComparables?.Except(RightComparables, NameComparer);
            }
        }

        public IEnumerable<ICoverageComparable> InnerComparablesExclusiveToRight
        {
            get
            {
                if(LeftComparables == null)
                {
                    return RightComparables;
                }
                return RightComparables?.Except(LeftComparables, NameComparer);
            }
        }

        public IEnumerable<(ICoverageComparable innerComparableFromLeft, ICoverageComparable innerComparableFromRight, double delta)> InnerComparablesWithDifferentCoverage
        {
            get
            { 
                if(innerComparablesWithDifferentCoverage == null)
                {
                    innerComparablesWithDifferentCoverage = new List<(ICoverageComparable moduleFromLeft, ICoverageComparable moduleFromRight, double delta)>();
                    if(LeftComparables != null && RightComparables != null)
                    {
                        foreach (var innerComparableFromLeft in LeftComparables)
                        {
                            if (rightInnerComparablesByName.TryGetValue(innerComparableFromLeft.Name, out var innerComparableFromRight))
                            {
                                var diff = CompareValue(innerComparableFromRight) - CompareValue(innerComparableFromLeft);
                                if (diff != 0)
                                {
                                    innerComparablesWithDifferentCoverage.Add((innerComparableFromLeft, innerComparableFromRight, diff));
                                }
                            }
                        }
                    }
                }
                return innerComparablesWithDifferentCoverage;
            }
        }

        public double CompareValue(ICoverageComparable c)
        {
            switch (ComparisonType)
            {
                case CompareBy.BlocksCovered:
                    return c.BlocksCovered;
                case CompareBy.BlocksCoveredPercentage:
                    return ((double)c.BlocksCovered / (double)(c.BlocksCovered + c.BlocksNotCovered)) * 100;
                case CompareBy.BlocksNotCovered:
                    return c.BlocksNotCovered;
                case CompareBy.BlocksNotCoveredPercentage:
                    return ((double)c.BlocksNotCovered / (double)(c.BlocksCovered + c.BlocksNotCovered)) * 100;
                case CompareBy.LinesCovered:
                    return c.LinesCovered;
                case CompareBy.LinesCoveredPercentage:
                    return ((double)c.LinesCovered / (double)(c.LinesCovered + c.LinesPartiallyCovered + c.LinesNotCovered)) * 100;
                case CompareBy.LinesNotCovered:
                    return c.LinesNotCovered;
                case CompareBy.LinesNotCoveredPercentage:
                    return ((double)c.LinesNotCovered / (double)(c.LinesCovered + c.LinesPartiallyCovered + c.LinesNotCovered)) * 100;
            }
            throw new InvalidOperationException("Unsupported ComparisonType: " + ComparisonType);
        }
    }
}
