using Gsmu.Api;
using Gsmu.Api.Data;
using Gsmu.Api.Data.School.CourseRoster;
using Gsmu.Api.Data.ViewModels.Layout;
using Gsmu.Api.Data.School.Student;
using Gsmu.Api.Commerce.ShoppingCart;
using Gsmu.Api.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gsmu.Web.Areas.Public.Controllers
{
    [RequireAdminModeAttribute(false)]
    public class LayoutController : Controller
    {
        public ActionResult SaveContentVisibilitySetting(LayoutArea area, bool visible)
        {
            LayoutManager.SaveContentVisibilitySetting(area, visible);
            return null;
        }

        [ActionName("button-labels-and-coloring")]
        public ActionResult ButtonLabelsAndColoring()
        {
            var BGColorlist = Enum.GetValues(typeof(BGColor))
            .Cast<BGColor>()
            .Select(v => v.ToString())
            .ToList();

            var BGColorInfolist = new List<BGColorInfo>();

            foreach (var corcc in BGColorlist)
            {
                BGColorInfolist.Add(
                    new BGColorInfo
                    {
                        field = corcc,
                        color = Gsmu.Api.Data.ViewModels.Layout.LayoutManager.GetBGColor(corcc),
                        Title = Gsmu.Api.Data.ViewModels.Layout.LayoutManager.GetBGTitle(corcc)
                    }
            );
            }

            ViewBag.BGColorInfolist = BGColorInfolist;

            return View("ButtonLabelsAndColoring");
        }

        public ActionResult ColorTheme()
        {
            ViewBag.BGColorInfolist = Gsmu.Api.Data.ViewModels.Layout.LayoutManager.BGColorInfos();

            return View("ColorTheme");
        }

        public ActionResult SaveButtonLabel(LayoutButton button, string label)
        {
            LayoutManager.SaveButtonLabel(button, label);
            return null;
        }

        public ActionResult AppendHistoryBGColor(string jsondata)
        {
            LayoutManager.SaveBGColorInfoHistory(jsondata);
            return null;
        }

        public String GetHistoryBGColor()
        {
            string jsondata = Settings.Instance.GetMasterInfo3().UIThemes;
            return jsondata;
        }

        public ActionResult resetAllBGColors()
        {

            var BGColorlist = Enum.GetValues(typeof(BGColor))
            .Cast<BGColor>()
            .Select(v => v)
            .ToList();

            foreach (BGColor corcc in BGColorlist)
            {
                string defltColor = LayoutManager.GetBGDefaultColor(corcc.ToString());
                LayoutManager.SaveBGColor(corcc, defltColor);
            }
            return null;
        }

        public ActionResult AllBGColors()
        {

            var BGColorlist = Enum.GetValues(typeof(BGColor))
            .Cast<BGColor>()
            .Select(v => v)
            .ToList();
            
            var result = new JsonResult();

            var BGColorInfolist = new List<BGColorInfo>();

            foreach (var corcc in BGColorlist)
            {
                BGColorInfolist.Add(
                    new BGColorInfo
                    {
                        field = corcc.ToString(),
                        color = Gsmu.Api.Data.ViewModels.Layout.LayoutManager.GetBGColor(corcc.ToString()),
                        Title = Gsmu.Api.Data.ViewModels.Layout.LayoutManager.GetBGTitle(corcc.ToString())
                    }
            );
            }

            result.Data = BGColorInfolist;

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SaveBGColor(BGColor field, string color)
        {
            LayoutManager.SaveBGColor(field, color);
            return null;
        }

        [ValidateInput(false)]
        public ActionResult SaveContent(LayoutArea area, string html)
        {
            var result = new JsonResult()
            {
            };
            LayoutManager.SaveContent(area, html);
            return result;
        }

        public ActionResult UploadContentFile(LayoutArea area, HttpPostedFileBase file)
        {
            var result = new JsonResult()
            {
            };           
            string fileName = LayoutManager.SaveUploadContentFile(area, file);
            result.Data = new
            {
                success = true,
                file = fileName,
                mime = file.ContentType
            };

            return result;
        }

        public ActionResult ListLayouts()
        {
            var content = new ContentResult();
            content.ContentType = "application/json";
            content.Content = LayoutManager.PublicCourseBrowserLayoutConfigurationJson;
            return content;
        }

        public ActionResult SetLayoutState(string id, string state)
        {
            LayoutManager.SetLayoutState(id, state);
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteLayout(string id)
        {
            LayoutManager.RemoveLayoutState(id);
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetEmbeddedState(bool enabled)
        {
            EmbedHelper.IsSiteEmbedded = enabled;
            return null;
        }

        [HttpPost]
        public ActionResult LayoutConfiguration(string layoutConfiguration)
        {
            LayoutManager.PublicLayoutConfigurationJson = layoutConfiguration;
            var result = new JsonResult();
            return result;
        }

        public ActionResult SetIncreaseWordTopRow(int increaseWordTopRow)
        {
            var layout = LayoutManager.PublicLayoutConfiguration;
            layout.IncreaseWordTopRow = increaseWordTopRow;
            LayoutManager.PublicLayoutConfiguration = layout;
            return null;
        }
      
    }
}
