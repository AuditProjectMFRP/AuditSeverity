using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Audit_management_portal.Models
{
    public class AuditDetails
    {
        public string AuditType { get; set; }
        public DateTime AuditDate { get; set; }
        public Dictionary<string, string> AuditQuestions { get; set; }
    }
}
