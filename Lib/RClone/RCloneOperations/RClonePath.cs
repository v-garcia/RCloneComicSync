using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.RClone.RCloneOperations
{
    public class RClonePath : IRClonePath
    {
        public String RemoteName { get; set; }

        public String Path { get; set; }

        public override string ToString()
        {
            return $"{RemoteName}:{Path}";
        }
    }
}
