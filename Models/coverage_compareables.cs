using Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

public partial class CoverageDSPrivModule : ICoverageComparable
{
    [XmlIgnore]
    public string Name => ModuleName;

    [XmlIgnore]
    public ICoverageComparable[] InnerComparables => namespaceTableField;
}

public partial class CoverageDSPrivModuleNamespaceTable : ICoverageComparable
{
    [XmlIgnore]
    public string Name => NamespaceName;

    [XmlIgnore]
    public ICoverageComparable[] InnerComparables => classField;
}

public partial class CoverageDSPrivModuleNamespaceTableClass : ICoverageComparable
{
    [XmlIgnore]
    public string Name => ClassName;

    [XmlIgnore]
    public ICoverageComparable[] InnerComparables => methodField;
}

public partial class CoverageDSPrivModuleNamespaceTableClassMethod : ICoverageComparable
{
    [XmlIgnore]
    public string Name => MethodName;

    [XmlIgnore]
    public ICoverageComparable[] InnerComparables => null;
}
