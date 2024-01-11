using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmAnalysisSemesterProject
{
    internal class Request
    {
        public int sourceNodeId;
        public int targetNodeId;

        public Request(int sourceNodeId, int targetNodeId)
        {
            this.sourceNodeId = sourceNodeId;
            this.targetNodeId = targetNodeId;
        }
    }
}
