
using Audit_management_portal;
using Audit_management_portal.Models;
using AuditSeverityService;
using AuditSeverityService.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace AuditSeverityTest
{
    public class AuditSeverity

    {
        [SetUp]
        public void Setup()
        {
        }
        [Test]
        public void CheckAuditResponseObject_WhenAuditTypeIsInternal()
        {
            //Arrange
            AuditRequest obj = new AuditRequest();
            AuditDetails b = new AuditDetails();
            b.AuditDate = Convert.ToDateTime("2020-08-02");
            b.AuditType = "Internal";
            Dictionary<string, string> a = new Dictionary<string, string>();
            a.Add("Have all Change requests followed SDLC before PROD move? ", "Yes");
            a.Add("Have all Change requests been approved by the application owner?", "Yes");
            a.Add("Are all artifacts like CR document, Unit test cases available?", "Yes");
            a.Add("Is the SIT and UAT sign-off available?", "Yes");
            a.Add("Is data deletion from the system done with application owner approval?", "Yes");
            b.AuditQuestions = a;
            obj.AuditDetails = b;
            obj.ProjectManagerName = "kunal";
            obj.ProjectName = "audit";
            obj.ApplicationOwnerName = "cts";

            Mock<IConfiguration> config = new Mock<IConfiguration>();
            config.SetupGet(x => x["Links:AuditBenchmark"]).Returns("https://localhost:44315/api/AuditBenchmark");

            //Act
            AuditSeverityController Controller = new AuditSeverityController(config.Object);
            var auditResponse = Controller.ProjectExecutionStatus(obj);
            var res = auditResponse.Result;
            var c = res.Result as CreatedResult;
            var d = c.Value as AuditResponse;

            //Assert
            Assert.AreEqual("GREEN", d.ProjectExecutionStatus);
            Assert.AreEqual("No action needed", d.RemedialActionDuration);

        }

        [Test]
        public void CheckAuditResponseObject_WhenAuditTypeIsSOX()
        {
            //Arrange
            AuditRequest obj = new AuditRequest();
            AuditDetails b = new AuditDetails();
            b.AuditDate = Convert.ToDateTime("2020-08-12");
            b.AuditType = "SOX";
            Dictionary<string, string> Question = new Dictionary<string, string>();
            Question.Add("Have all Change requests followed SDLC before PROD move?", "NO");
            Question.Add("Have all Change requests been approved by the application owner?", "NO");
            Question.Add("For a major change, was there a database backup taken before and after PROD move?", "NO");
            Question.Add("Has the application owner approval obtained while adding a user to the system?", "NO");
            Question.Add("Is data deletion from the system done with application owner approval?", "NO");
            b.AuditQuestions = Question;
            obj.AuditDetails = b;
            obj.ProjectManagerName = "subhanshu";
            obj.ProjectName = "audit";
            obj.ApplicationOwnerName = "cts";
            Mock<IConfiguration> config = new Mock<IConfiguration>();
            config.SetupGet(x => x["Links:AuditBenchmark"]).Returns("https://localhost:44315/api/AuditBenchmark");

            //Act
            AuditSeverityController Controller = new AuditSeverityController(config.Object);
            var auditResponse = Controller.ProjectExecutionStatus(obj);
            var res = auditResponse.Result;
            var c = res.Result as CreatedResult;
            var d = c.Value as AuditResponse;

            //Assert
            Assert.AreEqual("RED", d.ProjectExecutionStatus);
            Assert.AreEqual("Action to be taken in 1 week", d.RemedialActionDuration);
        }


        [Test]
        public void GetAuditCheckListQuestions_WhenInvalidInputIsPassed_ReturnsBadRequest()
        {
            //Arrange
            AuditRequest obj = new AuditRequest();
            AuditDetails b = new AuditDetails();
            b.AuditType = "InvalidInput"; ;
            b.AuditDate = Convert.ToDateTime("2020-08-12");
            Dictionary<string, string> Question = new Dictionary<string, string>();
            Question.Add("Have all Change requests followed SDLC before PROD move?", "NO");
            Question.Add("Have all Change requests been approved by the application owner?", "NO");
            Question.Add("For a major change, was there a database backup taken before and after PROD move?", "NO");
            Question.Add("Has the application owner approval obtained while adding a user to the system?", "NO");
            Question.Add("Is data deletion from the system done with application owner approval?", "NO");
            b.AuditQuestions = Question;
            obj.AuditDetails = b;
            obj.ProjectManagerName = "subhanshu";
            obj.ProjectName = "audit";
            obj.ApplicationOwnerName = "cts";
            Mock<IConfiguration> config = new Mock<IConfiguration>();
            //Act
            AuditSeverityController Controller = new AuditSeverityController(config.Object);
            var ControllerOutput = Controller.ProjectExecutionStatus(obj);
            var result = ControllerOutput.Result;
            var a = result.Result as BadRequestObjectResult;



            //Assert     
            Assert.AreEqual("Invalid AuditType Input", a.Value);
        }

        [Test]
        public void GetAuditCheckListQuestions_WhenEmptyObjectIsPassed_ReturnsBadRequest()
        {
            Mock<IConfiguration> config = new Mock<IConfiguration>();
            AuditRequest obj = new AuditRequest();
            AuditSeverityController Controller = new AuditSeverityController(config.Object);
            var ControllerOutput = Controller.ProjectExecutionStatus(obj);
            var result = ControllerOutput.Result;
            var a = result.Result as BadRequestObjectResult;

            Assert.AreEqual("Please Enter All the Values", a.Value);
        }

    }
}