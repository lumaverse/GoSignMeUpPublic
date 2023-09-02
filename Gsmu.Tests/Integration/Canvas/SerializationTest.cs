using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

using Newtonsoft.Json;
using Gsmu.Api.Data;
using Gsmu.Api.Integration.Canvas;
using Gsmu.Api.Integration.Canvas.Entities;


namespace Gsmu.Tests.Web.Json
{
    [TestClass]
    public class SerializationTest
    {

        public SerializationTest()
        {
            var httpContext = HttpHelper.FakeHttpContext("http://localhost/");
            System.Web.HttpContext.Current = httpContext;
        }

        [TestMethod]
        public void MainTest()
        {
            var data = new List<object>();

            var channel = new CommunicationChannel();
            channel.Id = 1;
            channel.Address = "mike@gosignmeup.com";
            channel.Position = 4;
            channel.Type = CommunicationChannelType.email;
            channel.UserId = 12;
            channel.WorkflowState = CommunicationChannelWorkflowState.active;

            data.Add(channel);

            var course = new Course();
            course.AccountId = 16;
            course.AllowStudentAssignmentEdits = true;
            course.Calendar = new Calendar()
            {
                Ics = "http://www.yahoo.com/test.ics"
            };
            course.CourseCode = "21382K";
            course.DefaultView = PageType.feed;
            course.EndAt = DateTime.Now.AddDays(323);
            course.Id = 1234;
            course.Name = "Well such a cold weather";
            course.PublicDescription = "This is the main description";
            course.StartAt = DateTime.Now;
            course.StorageQuotaMb = 200;
            course.SyllabusBody = "Secret!";
            course.WorkflowState = CourseWorkflowState.available;

            data.Add(course);
            var result = JsonConvert.SerializeObject(data, Formatting.Indented);

            Console.WriteLine(result);
        }

        [TestMethod]
        public void ConfigSerializationTest()
        {
            var value = Gsmu.Api.Data.Settings.Instance.GetMasterInfo4().CanvasConfiguration;

            Console.WriteLine(value);

            var configuration = JsonConvert.DeserializeObject<Configuration>(value);
            var encoded = JsonConvert.SerializeObject(configuration);

            Console.WriteLine(encoded);

        }
    }
}
