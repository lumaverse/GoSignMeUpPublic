using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data.ViewModels.Grid;
using Gsmu.Api.Language;
using System.Web.Helpers;

namespace Gsmu.Api.Data.ViewModels.Layout
{
    public static class LayoutManager
    {
        public static void SaveContentVisibilitySetting(LayoutArea area, bool visible)
        {
            using (var db = new SchoolEntities())
            {
                var items = (from mi in db.masterinfo4 select mi);

                foreach (var mi in items)
                {
                    switch (area)
                    {
                        case LayoutArea.Header:
                            mi.PublicHeaderVisible = visible ? 1 : 0;
                            break;

                        case LayoutArea.Footer:
                            mi.PublicFooterVisible = visible ? 1 : 0;
                            break;

                        case LayoutArea.Welcome:
                            mi.PublicWelcomeMessageVisible = visible ? 1 : 0;
                            break;
                    }
                }
                db.SaveChanges();
            }
        }

        public static void SaveContent(LayoutArea area, string html)
        {
            using (var db = new SchoolEntities())
            {

                var items = (from mi in db.masterinfo4 select mi);

                foreach (var mi in items)
                {
                    switch (area)
                    {
                        case LayoutArea.Header:
                            mi.PublicHeaderContent = html;
                            break;

                        case LayoutArea.Footer:
                            mi.PublicFooterContent = html;
                            break;

                        case LayoutArea.Welcome:
                            mi.PublicWelcomeMessageContent = html;
                            break;
                    }
                }
                db.SaveChanges();
            };
        }

        public static string SaveUploadContentFile(LayoutArea area, HttpPostedFileBase file)
        {
            string pathName;

            switch (area)
            {
                case LayoutArea.Footer:
                    pathName = "footer";
                    break;

                case LayoutArea.Header:
                    pathName = "header";
                    break;

                case LayoutArea.Welcome:
                    pathName = "welcome";
                    break;

                default:
                    throw new Exception("The layout area you are uploading for is not implemented");
            }

            var webPath = "Areas/Public/Content/Layout/" + pathName + "/" + file.FileName;
            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/" + webPath);
            file.SaveAs(path);
            return webPath;
        }


        public static List<BGColorInfo> BGColorInfos()
        {

            var BGColorlist = Enum.GetValues(typeof(BGColor))
            .Cast<BGColor>()
            .Select(v => v.ToString())
            .ToList();

            var BGColorInfolist = new List<BGColorInfo>();

            foreach (var corcc in BGColorlist)
            {
                var strcolor = Gsmu.Api.Data.ViewModels.Layout.LayoutManager.GetBGColor(corcc);
                if (String.IsNullOrWhiteSpace(strcolor)) { strcolor = GetBGDefaultColor(corcc); };

                BGColorInfolist.Add(
                    new BGColorInfo
                    {
                        field = corcc,
                        color = strcolor,
                        Title = Gsmu.Api.Data.ViewModels.Layout.LayoutManager.GetBGTitle(corcc)
                    }
            );
            }

            return BGColorInfolist;
        }

        public static void SaveBGColorInfoHistory(string jsondata)
        {
            var jsonencode = Json.Encode(jsondata);

            var themes = Settings.Instance.GetMasterInfo3().UIThemes;
            if (!string.IsNullOrWhiteSpace(themes))
            {
                jsonencode = jsonencode + "|" + themes;
            }



            using (var db = new SchoolEntities())
            {
                foreach (var mi3 in db.MasterInfo3)
                {
                    mi3.UIThemes = jsonencode;
                }
                db.SaveChanges();
            }
            Settings.Instance.Refresh();
        }


        public static string GetBGColor(string field)
        {
            var bgclr = "";
            var mi3 = Settings.Instance.GetMasterInfo3();
            switch (field)
            {
                case "clogheaderbg":
                    bgclr = mi3.clogheaderbg;
                    break;
                case "clogheaderbar":
                    bgclr = mi3.clogheaderbar;
                    break;
                case "clogheaderbartext":
                    bgclr = mi3.clogheaderbartext;
                    break;
                case "clogshowallbg":
                    bgclr = mi3.clogshowallbg;
                    break;
                case "clogshowalltext":
                    bgclr = mi3.clogshowalltext;
                    break;
                case "clogmaincatbg":
                    bgclr = mi3.clogmaincatbg;
                    break;
                case "clogmaincatactive":
                    bgclr = mi3.clogmaincatactive;
                    break;
                case "clogmaincattext":
                    bgclr = mi3.clogmaincattext;
                    break;
                case "clogsubcatbg":
                    bgclr = mi3.clogsubcatbg;
                    break;
                case "clogsubcatactive":
                    bgclr = mi3.clogsubcatactive;
                    break;
                case "clogsubcattext":
                    bgclr = mi3.clogsubcattext;
                    break;
                case "clogsub2catbg":
                    bgclr = mi3.clogsub2catbg;
                    break;
                case "clogsub2catactive":
                    bgclr = mi3.clogsub2catactive;
                    break;
                case "clogsub2cattext":
                    bgclr = mi3.clogsub2cattext;
                    break;
                case "clogcoursetext":
                    bgclr = mi3.clogcoursetext;
                    break;
                case "clogleftnavbg":
                    bgclr = mi3.clogleftnavbg;
                    break;
                case "clogspecialmsgcolor":
                    bgclr = mi3.clogspecialmsgcolor;
                    break;
                case "clogimplinkcolor":
                    bgclr = mi3.clogimplinkcolor;
                    break;
                case "CatelogCurrentCourseColor":
                    bgclr = mi3.CatelogCurrentCourseColor;
                    break;
            }

            return bgclr;
        }

        public static string GetBGTitle(string field)
        {
            var bgclr = "";
            switch (field)
            {
                case "clogheaderbg":
                    bgclr = "Header Background Color";
                    break;
                case "clogheaderbar":
                    bgclr = "Header Bar Background Color";
                    break;
                case "clogheaderbartext":
                    bgclr = "Header Bar Background Text";
                    break;
                case "clogshowallbg":
                    bgclr = "Show All Background Color";
                    break;
                case "clogshowalltext":
                    bgclr = "Show All Background Text";
                    break;
                case "clogmaincatbg":
                    bgclr = "Main Category Background Color";
                    break;
                case "clogmaincatactive":
                    bgclr = "Main Category Background Color (active)";
                    break;
                case "clogmaincattext":
                    bgclr = "Main Category Text";
                    break;
                case "clogsubcatbg":
                    bgclr = "Sub Category Background Color";
                    break;
                case "clogsubcatactive":
                    bgclr = "Sub Category Background Color (active)";
                    break;
                case "clogsubcattext":
                    bgclr = "Sub Category Text";
                    break;
                case "clogsub2catbg":
                    bgclr = "Sub Category 2 Background Color";
                    break;
                case "clogsub2catactive":
                    bgclr = "Sub Category 2 Background Color (active)";
                    break;
                case "clogsub2cattext":
                    bgclr = "Sub Category 2 Text";
                    break;
                case "clogcoursetext":
                    bgclr = "Course Text Color";
                    break;
                case "clogleftnavbg":
                    bgclr = "Left Navigation Background";
                    break;
                case "clogspecialmsgcolor":
                    bgclr = "Special Message Text Color";
                    break;
                case "clogimplinkcolor":
                    bgclr = "Important Link Text Color";
                    break;
                case "CatelogCurrentCourseColor":
                    bgclr = "Select Color for Current Courses";
                    break;
            }

            return bgclr;
        }

        public static string GetBGDefaultColor(string field)
        {
            var bgclr = "";
            switch (field)
            {
                case "clogheaderbg":
                    bgclr = "FFFFFF";
                    break;
                case "clogheaderbar":
                    bgclr = "FFFFFF";
                    break;
                case "clogheaderbartext":
                    bgclr = "aeaeae";
                    break;
                case "clogshowallbg":
                    bgclr = "000000";
                    break;
                case "clogshowalltext":
                    bgclr = "FFFFFF";
                    break;
                case "clogmaincatbg":
                    bgclr = "7F7F7F";
                    break;
                case "clogmaincatactive":
                    bgclr = "7F7F7F";
                    break;
                case "clogmaincattext":
                    bgclr = "FFFFFF";
                    break;
                case "clogsubcatbg":
                    bgclr = "E7E7E7";
                    break;
                case "clogsubcatactive":
                    bgclr = "E7E7E7";
                    break;
                case "clogsubcattext":
                    bgclr = "4D4D4D";
                    break;
                case "clogsub2catbg":
                    bgclr = "E7E7E7";
                    break;
                case "clogsub2catactive":
                    bgclr = "E7E7E7";
                    break;
                case "clogsub2cattext":
                    bgclr = "4D4D4D";
                    break;
                case "clogcoursetext": //not used in ruby yet
                    bgclr = "";
                    break;
                case "clogleftnavbg":
                    bgclr = "FFFFFF";
                    break;
                case "clogspecialmsgcolor":
                    bgclr = "FF0000";
                    break;
                case "clogimplinkcolor": //not used in ruby yet
                    bgclr = "";
                    break;
                case "CatelogCurrentCourseColor": //not used in ruby yet
                    bgclr = "339933";
                    break;
            }

            return bgclr;
        }

 
        public static void SaveBGColor(BGColor field, string color)
        {

                using (var db = new SchoolEntities())
                {
                    var items = (from mi in db.MasterInfo3 select mi);

                    foreach (var mi in items)
                    {
                        switch (field)
                        {
                            case BGColor.clogheaderbg:
                                mi.clogheaderbg = color;
                                break;

                            case BGColor.clogheaderbar:
                                mi.clogheaderbar = color;
                                break;

                            case BGColor.clogheaderbartext:
                                mi.clogheaderbartext = color;
                                break;

                            case BGColor.clogmaincatbg:
                                mi.clogmaincatbg = color;
                                break;

                            case BGColor.clogshowallbg:
                                mi.clogshowallbg = color;
                                break;

                            case BGColor.clogshowalltext:
                                mi.clogshowalltext = color;
                                break;

                            //case BGColor.clogmaincatactive:
                            //    mi.clogmaincatactive = color;
                            //    break;

                            case BGColor.clogmaincattext:
                                mi.clogmaincattext = color;
                                break;

                            case BGColor.clogsubcatbg:
                                mi.clogsubcatbg = color;
                                break;

                            case BGColor.clogsubcatactive:
                                mi.clogsubcatactive = color;
                                break;

                            case BGColor.clogsubcattext:
                                mi.clogsubcattext = color;
                                break;

                            case BGColor.clogsub2catbg:
                                mi.clogsub2catbg = color;
                                break;

                            case BGColor.clogsub2catactive:
                                mi.clogsub2catactive = color;
                                break;

                            case BGColor.clogsub2cattext:
                                mi.clogsub2cattext = color;
                                break;

                            //case BGColor.clogcoursetext:
                            //    mi.clogcoursetext = color;
                            //    break;

                            //case BGColor.clogleftnavbg:
                            //    mi.clogleftnavbg = color;
                            //    break;

                            //case BGColor.clogspecialmsgcolor:
                            //    mi.clogspecialmsgcolor = color;
                            //    break;

                            //case BGColor.clogimplinkcolor:
                            //    mi.clogimplinkcolor = color;
                            //    break;

                            //case BGColor.CatelogCurrentCourseColor:
                            //    mi.CatelogCurrentCourseColor = color;
                            //    break;        


                        }
                    }
                    db.SaveChanges();
                };
                return;
        }

        public static void SaveButtonLabel(LayoutButton button, string label)
        {
            if (button == LayoutButton.AddToCart || button == LayoutButton.Checkout || button == LayoutButton.Enrolled || button == LayoutButton.MultipleEnrollment)
            {
                using (var db = new SchoolEntities())
                {
                    var items = (from mi in db.masterinfo4 select mi);

                    foreach (var mi in items)
                    {
                        switch (button)
                        {
                            case LayoutButton.AddToCart:
                                mi.PublicButtonLabelAddToCart = label;
                                break;

                            case LayoutButton.Checkout:
                                mi.PublicButtonLabelCheckout = label;
                                break;

                            case LayoutButton.Enrolled:
                                mi.PublicButtonLabelEnrolled = label;
                                break;
                            case LayoutButton.MultipleEnrollment:
                                mi.MultipleEnrollment = label;
                                break;
                        }
                    }
                    db.SaveChanges();
                };
                return;
            }

            var layoutConfig = LayoutManager.PublicLayoutConfiguration;
            switch (button)
            {
                case LayoutButton.ClassFull:
                    layoutConfig.LayoutButtons.ClassFull = label;
                    break;

                case LayoutButton.WaitSpaceAvailable:
                    layoutConfig.LayoutButtons.WaitSpaceAvailable = label;
                    break;

                case LayoutButton.EmptyCart:
                    layoutConfig.LayoutButtons.EmptyCart = label;
                    break;

                case LayoutButton.Login:
                    layoutConfig.LayoutButtons.Login = label;
                    break;

                case LayoutButton.CreateAccount:
                    layoutConfig.LayoutButtons.CreateAccount = label;
                    break;

                case LayoutButton.Search:
                    layoutConfig.LayoutButtons.Search = label;
                    break;

                case LayoutButton.ClosedEnrollment:
                    layoutConfig.LayoutButtons.ClosedEnrollment = label;
                    break;

                case LayoutButton.Confirmation:
                    layoutConfig.LayoutButtons.Confirmation = label;
                    break;
                
                case LayoutButton.RegisterInstructor:
                    layoutConfig.LayoutButtons.RegisterInstructor = label;
                    break;
                case LayoutButton.Shibb_Login:
                    layoutConfig.LayoutButtons.Shibb_Login = label;
                    break;
                case LayoutButton.Cas_Login:
                    layoutConfig.LayoutButtons.Cas_Login = label;
                    break;
                case LayoutButton.Canvas_Login:
                    layoutConfig.LayoutButtons.Canvas_Login = label;
                    break;
                case LayoutButton.OnWaitList:
                    layoutConfig.LayoutButtons.OnWaitList = label;
                    break;
                case LayoutButton.SearchFrom:
                    layoutConfig.LayoutButtons.SearchFrom = label;
                    break;
                case LayoutButton.SearchTo:
                    layoutConfig.LayoutButtons.SearchTo = label;
                    break;
                case LayoutButton.ContinueShoppingCourse:
                    layoutConfig.LayoutButtons.ContinueShoppingCourse = label;
                    break;
            }
            LayoutManager.PublicLayoutConfiguration = layoutConfig;
        }

        public static string PublicCourseBrowserLayoutConfigurationJson
        {
            get
            {
                var layout = Settings.Instance.GetMasterInfo4().PublicCourseBrowserLayoutConfiguration;
                if (layout == null)
                {
                    layout = "{}";
                }
                return layout;
            }
        }

        public static Dictionary<string, string> PublicCourseBrowserLayoutConfiguration
        {
            get
            {
                var options = Json.Decode<Dictionary<string, string>>(PublicCourseBrowserLayoutConfigurationJson);
                return options;
            }
        }

        public static void SavePublicCourseBrowserLayoutConfiguration(Dictionary<string, string> layoutConfig)
        {
            var layout = Json.Encode(layoutConfig);
            using (var db = new SchoolEntities())
            {
                foreach (var mi4 in db.masterinfo4)
                {
                    mi4.PublicCourseBrowserLayoutConfiguration = layout;
                }
                db.SaveChanges();
            }
            Settings.Instance.Refresh();
        }

        public static void SetLayoutState(string id, string state)
        {
            var config = PublicCourseBrowserLayoutConfiguration;
            config[id] = state;
            SavePublicCourseBrowserLayoutConfiguration(config);
        }

        public static void RemoveLayoutState(string id)
        {
            var config = PublicCourseBrowserLayoutConfiguration;
            config.Remove(id);
            SavePublicCourseBrowserLayoutConfiguration(config);
        }

        public static string GetLayoutById(string id)
        {
            if (PublicCourseBrowserLayoutConfiguration.ContainsKey(id))
            {
                return PublicCourseBrowserLayoutConfiguration[id];
            }
            else
            {
                return string.Empty;
            }
        }

        public static string PublicLayoutConfigurationJson
        {
            get
            {
                var result = Settings.Instance.GetMasterInfo4().PublicLayoutConfiguration;
                if (result == null)
                {
                    result = "{}";
                }
                return result;
            }
            set
            {
                using (var db = new SchoolEntities())
                {
                    foreach (var mi4 in db.masterinfo4)
                    {
                        mi4.PublicLayoutConfiguration = value;
                    }
                    db.SaveChanges();
                }
                Settings.Instance.Refresh();
            }
        }

        public static LayoutConfiguration PublicLayoutConfiguration
        {
            get
            {
                LayoutConfiguration config = Json.Decode<LayoutConfiguration>(PublicLayoutConfigurationJson);
                return config;
            }
            set
            {
                PublicLayoutConfigurationJson = Json.Encode(value);
            }
        }
    }
}