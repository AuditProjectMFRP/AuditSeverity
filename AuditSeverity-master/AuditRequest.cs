using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Audit_management_portal.Models;

namespace Audit_management_portal
{
    public class AuditRequest
    {
        public string ProjectName { get; set; }
        public string ProjectManagerName { get; set; }
        public string ApplicationOwnerName { get; set; }

        public AuditDetails AuditDetails { get; set; }
    }
}
