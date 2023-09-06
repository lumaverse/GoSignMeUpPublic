using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gsmu.Api.Data;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.School.Student;
using Gsmu.Api.Authorization;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.Messaging;
using Gsmu.Api.Language;

namespace Gsmu.Api.Integration.Lti
{
    public class LtiHelper
    {
        private static ServiceProvider serviceProvider = null;

        public static ServiceProvider ServiceProvider
        {
            get
            {
                if (serviceProvider == null)
                {
                    var serviceUri = Configuration.Instance.ServiceUri;
                    var description = new ServiceProviderDescription();

                    description.AccessTokenEndpoint = new DotNetOpenAuth.Messaging.MessageReceivingEndpoint(serviceUri, DotNetOpenAuth.Messaging.HttpDeliveryMethods.PostRequest);

                    description.ProtocolVersion = ProtocolVersion.V10a;

                    description.RequestTokenEndpoint = new DotNetOpenAuth.Messaging.MessageReceivingEndpoint(serviceUri, DotNetOpenAuth.Messaging.HttpDeliveryMethods.PostRequest);

                    description.TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() };

                    serviceProvider = new DotNetOpenAuth.OAuth.ServiceProvider(description, new OAuth.TokenManager());

                }
                return serviceProvider;
            }
        }
        
        /*
        lti_version=LTI-1p0
        lti_message_type=basic-lti-launch-request
        resource_link_id=0
        resource_link_title=GoSignMeUp
        resource_link_description=
        user_id=Administrator (Default)
        user_image=http://montage.ltn.lvc.com/SAFARI/generated/customize/school/405c49e9-da0e-11de-9898-0024e87ec755/user/974d839a-7a35-11e0-a265-0024e87ec755/110x82/91902ce5-f9e1-11e2-8682-0024e87ec755.jpeg
        roles=Instructor
        lis_person_name_full=Default Administrator
        lis_person_name_family=Administrator
        lis_person_name_given=Default
        lis_person_contact_email_primary=dgappa@safarimontage.com
        context_id=dashboard-link
        context_title=Dashboard Link
        context_label=Dashboard
        tool_consumer_info_product_family_code=safarimontage
        tool_consumer_info_version=5.9.1
        tool_consumer_instance_guid=montage.ltn.lvc.com
        tool_consumer_instance_name=Advanced Development School
        tool_consumer_instance_description=Advanced Development School
        tool_consumer_instance_url=http://montage.ltn.lvc.com/SAFARI/
        tool_consumer_instance_contact_email=dgappa@safarimontage.com
        launch_presentation_locale=en-US
        launch_presentation_document_target=iframe
        launch_presentation_return_url=http://montage.ltn.lvc.com/SAFARI/montage/dashboard.php
        oauth_callback=about:blank
        oauth_version=1.0
        oauth_nonce=b1e6df674649198b12cc91f7a284311a
        oauth_timestamp=1384362323
        oauth_consumer_key=ABCDEFGHIJKLMNOPQRST
        oauth_signature_method=HMAC-SHA1
        oauth_signature=bkdxRO43qFARQf4tDDujsj/IPLk=
        ext_submit=ok
         */

        /*
         * Data coming from LTI CANVAS
        
        oauth_consumer_key: ABCDEFGHIJKLMNOPQRST
        oauth_signature_method: HMAC-SHA1
        oauth_timestamp: 1406886807
        oauth_nonce: s1GnoJbuBf9JgzZqHQCROhyHN1tIr4rrnGHhfpk
        oauth_version: 1.0
        oauth_callback: about:blank
        oauth_signature: a8dkUxGgRv4qqccs+K6VTpx9u/s=
        
        lti_message_type: basic-lti-launch-request
        lti_version: LTI-1p0

        launch_presentation_document_target: iframe
        launch_presentation_height: 400
        launch_presentation_locale: en
        launch_presentation_return_url: https://mccb.test.instructure.com/about/205847?include_host=true
        launch_presentation_width: 800
         
        context_id: c104e5542320976276f72e1a1ffda63f998fc432
        context_title: MVCC Board (new)
         
        lis_person_contact_email_primary: patrik@gosignmeup.com
        lis_person_name_family:
        lis_person_name_full: gsmeup1
        lis_person_name_given: gsmeup1
         
        user_id: a6b410c4713b8e4ae47f322db1952bb89e7239d7
        user_image: https://secure.gravatar.com/avatar/1bd1932d61dde5eaaaca62e47ab10f35?s=50&d=https%3A%2F%2Fcanvas.instructure.com%2Fimages%2Fmessages%2Favatar-50.png

        roles: urn:lti:sysrole:ims/lis/None
        
        custom_canvas_account_id: 16
        custom_canvas_account_sis_id: $Canvas.account.sisSourceId
        custom_canvas_api_domain: mvcc16.instructure.com
        custom_canvas_enrollment_state: $Canvas.enrollment.enrollmentState
        custom_canvas_user_id: 205847
        custom_canvas_user_login_id: gsmeup1

        -- gsmu specific
        custom_gsmu_area_source: user_navigation
        custom_gsmu_lti_version: canvas
        -- gsmu specific
         
        resource_link_id: a6b410c4713b8e4ae47f322db1952bb89e7239d7
        resource_link_title: GoSignMeUp

        tool_consumer_info_product_family_code: canvas
        tool_consumer_info_version: cloud
        tool_consumer_instance_contact_email: notifications@instructure.com
        tool_consumer_instance_guid: cnIl3C3LZ7E4EywfsbhwJToyHU65VRIwgnDsjcK9
        tool_consumer_instance_name: MVCC Board (new)

         * * */
        public static System.Web.Mvc.ActionResult HandleSso(System.Web.Mvc.ControllerBase controller)
        {
            var server = ServiceProvider;
            var oauthRequest = server.ReadRequest();

            var request = controller.ControllerContext.HttpContext.Request;

            Student student = null;
            using (var db = new SchoolEntities())
            {
                if (!string.IsNullOrWhiteSpace(request.Form["custom_gsmu_lti_version"]))
                {
                    switch (request.Form["custom_gsmu_lti_version"])
                    {
                        case "canvas":
                            if (!Canvas.Configuration.Instance.EnableCanvasLtiAuhentication)
                            {
                                throw new Exception("This function is disabled in the Canvas GSMU config. If this is not a mistake, please contact GSMU support to enable Canvas logon via LTI authentication.");
                            }
                            return Canvas.CanvasImport.SynchronizeLtiUser(controller);

                        default:
                            throw new Exception(
                                string.Format("Invalid LTI GSMU variable \"{0}\" value \"{1}\". Valid values are empty or null or see the source code for details in the switch/case statement.")
                            );
                    }
                }
                else
                {
                    student = HandleGenericLti(controller, db);
                }

                if (student != null)
                {
                    student.lti_data =
                    StringHelper.NameValueCollectionToQueryString(request.Form, NameValueCollectionToQueryStringBehavior.SameKeyDivideValuesWithComma);
                }
                db.SaveChanges();
            }
            if (student != null)
            {
                var messages = AuthorizationHelper.LoginStudent(student.USERNAME);
                Gsmu.Api.Web.ObjectHelper.AddRequestMessages(controller, messages);
            }
            return null;
        }

        private static Student HandleGenericLti(System.Web.Mvc.ControllerBase controller, SchoolEntities db)
        {
            var request = controller.ControllerContext.HttpContext.Request;
            var userId = request.Form["user_id"];
            var firstName = request.Form["lis_person_name_given"];
            var lastName = request.Form["lis_person_name_family"];
            var email = request.Form["lis_person_contact_email_primary"];
            var roles = request.Form["role"];

            if (userId == null)
            {
                throw new NullReferenceException();
            }

            var student = (from s in db.Students where s.lti_user_id == userId select s).FirstOrDefault();

            if (student == null)
            {
                student = new Student();
                student.lti_user_id = userId;

                student.USERNAME = userId;
                student.STUDNUM = "lti-" + userId;

                student.FIRST = firstName;
                student.EMAIL = email;
                student.LAST = lastName;
                StudentHelper.RegisterStudent(student, db);
            }
            else
            {
                student.USERNAME = userId;
                student.STUDNUM = "lti-" + userId;

                student.FIRST = firstName;
                student.EMAIL = email;
                student.LAST = lastName;
            }
            
            Gsmu.Api.Web.ObjectHelper.AddRequestMessage(controller, "LTI login successful.");

            return student;
        }

    }
}
