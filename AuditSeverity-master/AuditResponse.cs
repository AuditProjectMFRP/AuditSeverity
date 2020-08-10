using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuditSeverityService
{
    public class AuditResponse
    {
        public int AuditId { get; set; }
        public string ProjectExecutionStatus { get; set; }
        public string RemedialActionDuration { get; set; }

    }
}
