using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Models
{
    [XmlTypeAttribute(AnonymousType = true)]
    public interface ICoverageComparable
    {
        [XmlIgnore]
        string Name { get; }
        public uint LinesCovered { get; }
        public uint LinesPartiallyCovered { get; }
        public uint LinesNotCovered { get; }
        public uint BlocksCovered { get; }
        public uint BlocksNotCovered { get; }
        [XmlIgnore]
        public ICoverageComparable[] InnerComparables { get; }
    }
}