using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class TokenType
    {
        public string identifier { get { return "Identifier"; } }
        public string integer { get { return "Integer"; } }
        public string real { get { return "Real"; } }
        public string lexString { get { return "String"; } }
        public string lexOperator { get { return "Operator"; } }
        public string separator { get { return "Separator"; } }
        public string comment { get { return "Comment"; } }
        public string reserved { get { return "Reserved word"; } }
        public string predefined { get { return "Predefined word"; } }
        public string assignment { get { return "Assignment"; } }
    }
}
