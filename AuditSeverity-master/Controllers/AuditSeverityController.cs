using Audit_management_portal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AuditSeverityService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditSeverityController : ControllerBase
    {
        private readonly log4net.ILog _log4net;
        public IConfiguration _config;
        public AuditSeverityController(IConfiguration config)
        {
            _config = config;
            _log4net = log4net.LogManager.GetLogger(typeof(AuditSeverityController));
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<AuditResponse>> ProjectExecutionStatus([FromBody] AuditRequest AuditRequest)
        {
            if (AuditRequest.ProjectName == null || AuditRequest.ProjectManagerName == null || AuditRequest.AuditDetails.AuditType == null || AuditRequest.ApplicationOwnerName == null || AuditRequest.AuditDetails.AuditDate == null || AuditRequest.AuditDetails.AuditQuestions == null)
            {
                _log4net.Error("Method invoked with null input");
                return BadRequest("Please Enter All the Values");                 //check all inputs are given or not
            }
            if (!AuditRequest.AuditDetails.AuditType.Equals("Internal", StringComparison.InvariantCultureIgnoreCase) && !AuditRequest.AuditDetails.AuditType.Equals("SOX", StringComparison.InvariantCultureIgnoreCase)) //Incase of Invalid Input
            {
                _log4net.Error("Method invoked with Invalid AuditType Input");
                return BadRequest("Invalid AuditType Input");                  //check for invalid auditTYpe input
            }

            _log4net.Info("AuditSeverity Post Method For " + AuditRequest.AuditDetails.AuditType);

            List<AuditBenchmark> ListAuditBenchamark;
            AuditResponse obj = new AuditResponse();
            int AcceptableNoValue = 0;
            int CountNo = 0;

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync(_config["Links:AuditBenchmark"]);      //call benchmark
                string result = response.Content.ReadAsStringAsync().Result;
                ListAuditBenchamark = JsonConvert.DeserializeObject<List<AuditBenchmark>>(result);

            }
            foreach (var item in ListAuditBenchamark)                  //To Store Acceptable "NO" values returned from the benchmark api
            {
                if (item.AuditType.Equals(AuditRequest.AuditDetails.AuditType, StringComparison.InvariantCultureIgnoreCase))
                    AcceptableNoValue = item.BenchmarkNoAnswers;
            }

            for (int i = 0; i < AuditRequest.AuditDetails.AuditQuestions.Count; i++)                      //calculate number of "NO" entere by user
            {
                if (AuditRequest.AuditDetails.AuditQuestions.ElementAt(i).Value.Equals("NO", StringComparison.InvariantCultureIgnoreCase))
                    CountNo++;
            }

            Random r = new Random();
            obj.AuditId = r.Next(1, 99999);                         //generate random number to assign to AuditId

            if (AuditRequest.AuditDetails.AuditType.Equals("Internal", StringComparison.InvariantCultureIgnoreCase) && CountNo > AcceptableNoValue)         //enter Audit Response values based upon conditions
            {
                obj.ProjectExecutionStatus = "RED";
                obj.RemedialActionDuration = "Action to be taken in 2 weeks";
            }
            else if (AuditRequest.AuditDetails.AuditType.Equals("SOX", StringComparison.InvariantCultureIgnoreCase) && CountNo > AcceptableNoValue)
            {
                obj.ProjectExecutionStatus = "RED";
                obj.RemedialActionDuration = "Action to be taken in 1 week";
            }
            else
            {
                obj.ProjectExecutionStatus = "GREEN";
                obj.RemedialActionDuration = "No action needed";
            }

            return Created(String.Empty, obj);              //return Audit Response
        }
    }
}
