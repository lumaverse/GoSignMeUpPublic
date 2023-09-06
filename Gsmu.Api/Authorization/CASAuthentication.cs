using Gsmu.Api.Data;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.School.Student;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Gsmu.Api.Authorization
{
    public class CASAuthentication
    {
        public void ParseResponse()
        {

        }

        public static string ValidateCASAUthentication(string post_string)
        {
            string directory = AppDomain.CurrentDomain.BaseDirectory + @"Temp\";
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(Settings.Instance.GetMasterInfo2().CASAuthURL + "samlValidate" + "?TARGET=" + "https://dev250.gosignmeup.com");
            objRequest.Method = "POST";
            objRequest.ContentType = "application/soap+xml; charset=utf-8";
            objRequest.UserAgent = "classicAspCasSaml 0.1";

            StreamWriter myWriter = null;
            myWriter = new StreamWriter(objRequest.GetRequestStream());
            myWriter.Write(post_string);
            myWriter.Close();
           
            // returned values are returned as a stream, then read into a string
            String post_response ="";
            string status = "";
            string username = "";
            HttpWebResponse objHttpWebResponse = (HttpWebResponse)objRequest.GetResponse();
            System.IO.File.WriteAllText(directory +"test_cas.txt", objHttpWebResponse.StatusCode.ToString() +post_string);
            if (objHttpWebResponse.StatusCode == HttpStatusCode.OK)
            {
                var fields =HttpUtility.ParseQueryString( Settings.Instance.GetMasterInfo4().CASStudentIntegrationFields);
                Stream objResponseStream = objHttpWebResponse.GetResponseStream();
                XmlTextReader objXMLReader = new XmlTextReader(objResponseStream);
                XmlDocument xmldoc = new XmlDocument();
                System.IO.File.WriteAllText(directory +"test_cas2.txt", objXMLReader.ReadInnerXml() + objXMLReader.ReadOuterXml());
              xmldoc.Load(objXMLReader);
                XmlNodeList listNodesforStatus = xmldoc.GetElementsByTagName("StatusCode");
                foreach (XmlNode node in listNodesforStatus)
                {
                    foreach (string bbField in fields.Keys)
                    {
                        var gsmuField = fields[bbField];
                    }
                    status = node.Attributes["Value"].Value;
                }
                if (status == "samlp:Success")
                {
                    XmlNodeList listNodesforOId = xmldoc.GetElementsByTagName("NameIdentifier");
                    foreach (XmlNode node in listNodesforOId)
                    {

                        username = node.InnerText;

                    }

                    var student = StudentHelper.GetStudent(username);
                    if (student == null)
                    {
                        student = new Student
                           {
                               USERNAME = username,
                               FIRST = "",
                               LAST = "",
                               EMAIL = "",
                               STUDNUM = "CAS Maintained",
                               CITY = "",
                               STATE = "",
                               ZIP = "",
                               COUNTRY = "",
                               HOMEPHONE = "",
                               WORKPHONE = "",
                               FAX = "",
                               SS = "",
                               StudRegField1 = "",
                               StudRegField2 = "",
                               StudRegField3 = "",
                               StudRegField4 = "",
                               StudRegField5 = "",
                               StudRegField6 = "",
                               StudRegField7 = "",
                               StudRegField8 = "",
                               StudRegField9 = "",
                               StudRegField10 = "",

                               StudRegField11 = "",
                               StudRegField12 = "",
                               StudRegField13 = "",
                               StudRegField14 = "",
                               StudRegField15 = "",
                               StudRegField16 = "",
                               StudRegField17 = "",
                               StudRegField18 = "",
                               StudRegField19 = "",
                               StudRegField20 = "",
                               SCHOOL = 0,
                               DISTRICT = 0,
                               GRADE = 0,
                               DateAdded = DateTime.Now,
                               SAPLastPendingReason = "Passing from CAS",
                               AuthFromLDAP = 0,
                               loginTally = 0,
                               google_user = 0
                           };
                        StudentHelper.RegisterStudent(student);

                    }
                    AuthorizationHelper.LoginStudent(student.USERNAME, student.STUDNUM);

                }


                objXMLReader.Close();
            }
             return status;
        }

        public static string ValidateString(string requestID, string issueInstant, string ticket)
        {
            string str = "<?xml version='1.0'?>" +
                         "<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\">" +
                         "<SOAP-ENV:Header/>" +
                            "<SOAP-ENV:Body>" +
                            "<samlp:Request " +
                            "xmlns:samlp=\"urn:oasis:names:tc:SAML:1.0:protocol\" " +
                            "MajorVersion=\"1\" " +
                            "MinorVersion=\"1\" " +
                            "RequestID=\"" + requestID + "\"" +
                            "IssueInstant=\"" + issueInstant + "\">" +
                            "<samlp:AssertionArtifact>" + ticket + "</samlp:AssertionArtifact>" +
                            "</samlp:Request>" +
                            "</SOAP-ENV:Body>" +
                            "</SOAP-ENV:Envelope>";

            return str;
        }
    }
}
