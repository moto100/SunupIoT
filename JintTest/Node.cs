using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JintTest
{
    public class Node : Dictionary<string, Node> 
    {
        public string Name; 
        public dynamic Value;

    }
}
