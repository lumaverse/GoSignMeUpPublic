using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.SessionState;
using System.Web;

namespace Gsmu.Api.Web
{
    public static class ObjectHelper
    {
        public static T GetSessionObject<T>(WebContextObject type) where T : class
        {
            var id = type.ToString();
            T item = HttpContext.Current.Session[id] as T;
            return item;
        }

        public static void SetSessionObject<T>(WebContextObject type, T item) where T : class
        {
            var id = type.ToString();
            HttpContext.Current.Session[id] = item;
        }

        public static void ClearSessionObject(WebContextObject type)
        {
            var id = type.ToString();
            HttpContext.Current.Session.Remove(id);
        }

        public static T GetRequestObject<T>(WebContextObject type) where T : class
        {
            var id = type.ToString();
            T item = HttpContext.Current.Items[id] as T;
            return item;
        }

        public static void SetRequestObject<T>(WebContextObject type, T item) where T : class
        {
            var id = type.ToString();
            HttpContext.Current.Items[id] = item;
        }

        public static void ClearRequestObject(WebContextObject type)
        {
            var id = type.ToString();
            HttpContext.Current.Items.Remove(id);
        }

        public static void AddRequestMessages(System.Web.Mvc.ControllerBase controller, List<string> messages)
        {
            var result = (List<string>)controller.TempData[WebContextObject.RequestResultMessages.ToString()];
            if (result == null)
            {
                controller.TempData[WebContextObject.RequestResultMessages.ToString()] = messages;
            }
            else
            {
                result.AddRange(messages);
                controller.TempData[WebContextObject.RequestResultMessages.ToString()] = result;
            }
        }

        public static void AddRequestMessage(System.Web.Mvc.ControllerBase controller, string message)
        {
            var result = (List<string>)controller.TempData[WebContextObject.RequestResultMessages.ToString()];
            if (result == null)
            {
                result = new List<string>();
                result.Add(message);
                controller.TempData[WebContextObject.RequestResultMessages.ToString()] = result;
            }
            else
            {
                result.Add(message);
                controller.TempData[WebContextObject.RequestResultMessages.ToString()] = result;
            }
        }

        public static List<string> GetRequestMessages(System.Web.Mvc.ControllerBase controller)
        {
            var result = (List<string>)controller.TempData[WebContextObject.RequestResultMessages.ToString()];
            if (result == null)
            {
                result = new List<string>();
            }
            return result;
        }

        public static void AddRequestDebugMessage(System.Web.Mvc.ControllerBase controller, List<string> messages)
        {
            var result = (List<string>)controller.TempData[WebContextObject.RequestDebugMessages.ToString()];
            if (result == null)
            {
                controller.TempData[WebContextObject.RequestDebugMessages.ToString()] = messages;
            }
            else
            {
                result.AddRange(messages);
                controller.TempData[WebContextObject.RequestDebugMessages.ToString()] = result;
            }
        }

        public static void AddRequestDebugMessage(System.Web.Mvc.ControllerBase controller, string message)
        {
            var result = (List<string>)controller.TempData[WebContextObject.RequestDebugMessages.ToString()];
            if (result == null)
            {
                result = new List<string>();
                result.Add(message);
                controller.TempData[WebContextObject.RequestDebugMessages.ToString()] = result;
            }
            else
            {
                result.Add(message);
                controller.TempData[WebContextObject.RequestDebugMessages.ToString()] = result;
            }
        }

        public static List<string> GetRequestDebugMessages(System.Web.Mvc.ControllerBase controller)
        {
            var result = (List<string>)controller.TempData[WebContextObject.RequestDebugMessages.ToString()];
            if (result == null)
            {
                result = new List<string>();
            }
            return result;
        }

        public static void SetShowLoginInUi(System.Web.Mvc.ControllerBase controllerBase, bool showlogin)
        {
            controllerBase.TempData[WebContextObject.ShowLoginUi.ToString()] = showlogin;
        }

        public static bool GetShowLoginUi(System.Web.Mvc.ControllerBase controller)
        {
            var result = controller.TempData[WebContextObject.ShowLoginUi.ToString()] == null ? false : (bool)controller.TempData[WebContextObject.ShowLoginUi.ToString()];
            return result;
        }
    }
}
