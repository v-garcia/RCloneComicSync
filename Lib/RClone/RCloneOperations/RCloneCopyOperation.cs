using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.RClone.RCloneOperations
{
    public class RCloneCopyOperation : IRCloneCopyOperation
    {
        public int jobId { get; set; }

        public IRClonePath destination { get; set; }

        public IRClonePath source { get; set; }

        public override string ToString()
        {
            return $"Job id:{jobId} from {source.ToString()} to {destination.ToString()}";
        }
    }
}
