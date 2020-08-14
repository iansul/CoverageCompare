using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Engine
{
    public class CompareFiles : Comparison
    {
        public CompareFiles(CoverageFile one, CoverageFile two, CompareBy compareBy) : base(one.Modules.ToArray(), two.Modules.ToArray(), compareBy)
        {
        }
    }
}
