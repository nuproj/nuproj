using B3;
using ServiceModel.Composition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A3
{
    [ExportService]
    public class A3Class
    {
        public A3Class()
        {
            new B3Class();
        }

        public int Three()
        {
            return 3;
        }
    }
}
