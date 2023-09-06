using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Gsmu.Api.Data.School;
using Gsmu.Api;
using Gsmu.Api.Data;
using Gsmu.Api.Data.School.Category;
using Gsmu.Api.Data.School.Course;

namespace Gsmu.Api.Data.School.Category
{
    /// <summary>
    /// Linq queries related to categories.
    /// </summary>
    public static class Queries
    {

        /*
         * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.COURSE.QUERIES!!!!
         * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.COURSE.QUERIES!!!!
         * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.COURSE.QUERIES!!!!
         */

        /// <summary>
        /// Loads the categories for public display
        /// </summary>
        public static Dictionary<string, Dictionary<string, List<string>>> CategoryTree(int subsiteid = 0, bool courseInternal = false, CourseCancelState cancelState = CourseCancelState.NotCancelled, bool ShowPastandTranscribedCourses = false, bool ShowMembershipCourses=false)
        {
            var mainConnection = Connections.GetSchoolConnection();
            mainConnection.Open();

            var query = string.Empty;

            /*
             * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.COURSE.QUERIES!!!!
             * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.COURSE.QUERIES!!!!
             * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.COURSE.QUERIES!!!!
             */

            query += "SELECT distinct M.MainCategory,mordr.mcatorder "; //should get the mcatorder of other courses using this category
            query += " FROM MainCategories M  ";
            query += " OUTER APPLY (SELECT TOP 1 mcatorder FROM MainCategories WHERE MainCategory = M.MainCategory order by MainCategoryID) AS mordr ";
            query += " inner join  ";
            query += " (select distinct courses.courseid from  ";
            query += " (((courses inner JOIN [course times] ON courses.courseid = [course times].courseid) inner JOIN ";
            query += " MainCategories M ON courses.courseid = M.courseid) inner JOIN ";
            query += " SubCategories S ON S.MainCategoryID = M.MainCategoryID) LEFT JOIN ";
            query += " SubSubCategories S2 ON S2.MainCategoryID = M.MainCategoryID ";
            query += " left join courses as ec on ec.eventid = courses.COURSEID ";
            if (ShowMembershipCourses)
            {
                query += " where 1=1 and M.MainCategory = '%~ZZZZZZ~%' ";
            }
            else
            {
                query += " where 1=1 and M.MainCategory not like '%~ZZZZZZ~%' ";
            }

            query = ExtendCategoryTreeQuery(query, subsiteid, courseInternal, cancelState, null, null, ShowPastandTranscribedCourses);

            if (!WebConfiguration.DevelopmentMode)
            {
                query += " WHERE ltrim(rtrim(M.MainCategory)) <> '' AND M.MainCategory is not null AND M.MainCategory not like '%~ZZZZZZ~%'";
            }
            query += " ORDER BY mcatorder, 1 ";

            var mainCommand = mainConnection.CreateCommand();
            mainCommand.CommandText = query;
            var tree = new Dictionary<string, Dictionary<string, List<string>>>();
            using (var mainReader = mainCommand.ExecuteReader())
            {
                while (mainReader.Read())
                {
                    var mainName = mainReader.GetString(0);
                    if (!tree.ContainsKey(mainName))
                    {
                        var subTree = GetSubCategoryTree(subsiteid, courseInternal, mainName, cancelState, ShowPastandTranscribedCourses, ShowMembershipCourses);
                        tree[mainName] = subTree;
                    }
                }
            }
            mainConnection.Close();
            return tree;
        }

        private static List<string> GetSubCategories(int subsiteid, bool internalcourse, string mainName, CourseCancelState cancelState)
        {
            var subConnection = Connections.GetSchoolConnection();
            subConnection.Open();
            var subCommand = subConnection.CreateCommand();

            /*
             * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.COURSE.QUERIES!!!!
             * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.COURSE.QUERIES!!!!
             * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.COURSE.QUERIES!!!!
             */

            var query = string.Empty;
            query += "SELECT distinct S.SubCategory FROM (MainCategories M inner join  ";
            query += " SubCategories S on S.MainCategoryID = M.MainCategoryid) inner join ";
            query += " (select distinct courses.courseid from  ";
            query += " (((courses inner JOIN [course times] ON courses.courseid = [course times].courseid) inner JOIN ";
            query += "  MainCategories M ON courses.courseid = M.courseid) inner JOIN ";
            query += "  SubCategories S ON S.MainCategoryID = M.MainCategoryID) LEFT JOIN ";
            query += "  SubSubCategories S2 ON S2.MainCategoryID = M.MainCategoryID ";
            query += " left join courses as ec on ec.eventid = courses.COURSEID ";
            query += " where 1=1 ";
            query = ExtendCategoryTreeQuery(query, subsiteid, internalcourse, cancelState);
            query += " where M.MainCategory = @MainCategory";


            query += " ORDER BY 1 ";

            subCommand.CommandText = query;
            subCommand.Parameters.Add(
                new SqlParameter("@MainCategory", mainName)
            );

            List<string> subList = new List<string>();
            using (var subReader = subCommand.ExecuteReader())
            {
                while (subReader.Read())
                {
                    subList.Add(subReader.GetString(0));
                }
            }
            subConnection.Close();

            return subList;
        }

        private static Dictionary<string, List<string>> GetSubCategoryTree(int subsiteid, bool internalcourse, string mainCategory, CourseCancelState cancelState, bool ShowPastandTranscribedCourses = false, bool ShowMembershipCourses=false)
        {
            var showpastonlinecourses = Settings.Instance.GetMasterInfo(subsiteid).ShowPastOnlineCourses;
            var allowviewpastcoursesdays = Settings.Instance.GetMasterInfo3(subsiteid).allowviewpastcoursesdays;

            /*
             * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.COURSE.QUERIES!!!!
             * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.COURSE.QUERIES!!!!
             * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.COURSE.QUERIES!!!!
             */

            var query = string.Empty;
            query += "SELECT distinct ";
            query += "S.SubCategory, ";
            query += "SubQuery.SubSubCategory as subcate2 ";
            query += "FROM ";
            query += "(MainCategories M inner join SubCategories S on S.MainCategoryID = M.MainCategoryid) ";
            query += "inner join (select distinct ";

            query += "S.SubCategory, S2.SubSubCategory, M.MainCategoryID, courses.courseid ";
            //if (allowviewpastcoursesdays != null && allowviewpastcoursesdays.Value != 0)
            //{
                query += " ,courses.viewpastcoursesdays ";
            //}
            if (showpastonlinecourses != null && showpastonlinecourses.Value != 1)
            {
               
            }
            else
            {
                query += ", courses.onlinecourse ";
            }
            query += "from (((courses inner JOIN [course times] ON courses.courseid = [course times].courseid) ";
            query += "inner JOIN MainCategories M ON courses.courseid = M.courseid) ";
            query += "inner JOIN SubCategories S ON S.MainCategoryID = M.MainCategoryID) ";
            query += "LEFT JOIN SubSubCategories S2 ON S2.MainCategoryID = M.MainCategoryID ";
            query += " left join courses as ec on ec.eventid = courses.COURSEID ";
            query += "where 1 = 1 ";
            query = ExtendCategoryTreeQuery(query, subsiteid, internalcourse, cancelState, "S.SubCategory, S2.SubSubCategory, M.MainCategoryID", "S.MainCategoryID = SubQuery.MainCategoryID", ShowPastandTranscribedCourses);
            query += " where M.MainCategory = @MainCategory ";
            query += "ORDER BY 1,2";

            var connection = Connections.GetSchoolConnection();
            connection.Open();
            var cmd = connection.CreateCommand();
            cmd.CommandText = query;

            cmd.Parameters.Add(
                new SqlParameter("@MainCategory", mainCategory)
            );

            var tree = new Dictionary<string, List<string>>();
            string currentSubCat = null;
            List<string> currentList = null;
            bool initialized = false;
            using (var reader = cmd.ExecuteReader())
            {
                var hasRows = reader.Read();
                while (hasRows)
                {
                    var nextSubCat = reader.GetString(0);
                    string nextSubSubCat = null;
                    if (!reader.IsDBNull(1))
                    {
                        nextSubSubCat = reader.GetString(1);
                    }
                    if (!initialized)
                    {
                        currentSubCat = nextSubCat;
                        currentList = new List<string>();
                        initialized = true;
                    }
                    if (nextSubCat != currentSubCat)
                    {
                        if (!tree.ContainsKey(currentSubCat))
                        {
                            tree.Add(currentSubCat, currentList);
                        }
                        currentList = new List<string>();
                        currentSubCat = nextSubCat;
                    }
                    if (nextSubSubCat != null)
                    {
                        currentList.Add(nextSubSubCat);
                    }
                    hasRows = reader.Read();
                    if (!hasRows)
                    {
                        if (!tree.ContainsKey(currentSubCat))
                        {
                            tree.Add(currentSubCat, currentList);
                        }
                    }
                }
            }

            connection.Close();

            return tree;
        }

        /// <summary>
        /// Adds common filtering for the public of listing categories into a query - used by main and subcategories, so there is
        /// no code copying.
        /// </summary>
        /// <param name="query">The query to extend</param>
        /// <param name="subsiteid">The current subsite or 0</param>
        /// <param name="internalcourse">Internalcourse value</param>
        /// <returns>The new extended query.</returns>
        private static string ExtendCategoryTreeQuery(string query, int subsiteid, bool internalcourse, CourseCancelState cancelState, string groupByFields = null, string subQueryCriteria = null, bool ShowPastandTranscribedCourses = false)
        {

            /*
             * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.COURSE.QUERIES!!!!
             * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.COURSE.QUERIES!!!!
             * ANY CHANGES YOU MAKE HERE MUST BE REFLECTED IN GSMU.API.DATA.SCHOOL.COURSE.QUERIES!!!!
             */

            var showpastonlinecourses = Settings.Instance.GetMasterInfo(subsiteid).ShowPastOnlineCourses;
            var allowviewpastcoursesdays = Settings.Instance.GetMasterInfo3(subsiteid).allowviewpastcoursesdays;

            CourseInternalState internalState = CourseInternalState.Public;
            if (internalcourse)
            {
                internalState = InternalCourseSettings.InternalCourseResultTypes;
            }

            switch (internalState)
            {
                case CourseInternalState.Internal:
                    query += " and (courses.InternalClass = -1 or courses.InternalClass =1) ";
                    break;

                case CourseInternalState.Public:
                    query += " and courses.InternalClass <> 1 and courses.InternalClass <> -1 ";
                    break;
            }

            switch (cancelState)
            {
                case CourseCancelState.Cancelled:
                    query += " and (courses.cancelcourse = -1 or courses.cancelcourse = 1) ";
                    break;

                case CourseCancelState.NotCancelled:
                    query += " and courses.cancelcourse <> -1 and courses.cancelcourse <> 1 ";
                    break;
            }

            query += " and courses.courseid is not null ";// and courses.SubSiteId = " + subsiteid.ToString();            
            query += " and (courses.coursetype = 1 OR courses.coursetype=0 OR courses.coursetype is null) ";
            if (!internalcourse)
            {
                query += " and (ec.courseid is null or ec.courseid =courses.COURSEID or ec.internalclass is null or ec.internalclass = 0) ";
            }
            query += " group by ";
            if (groupByFields != null)
            {
                query += groupByFields + ", ";
            }
            query += "courses.courseid, courses.CourseCloseDays";
            //if (allowviewpastcoursesdays != null && allowviewpastcoursesdays.Value != 0)
           // {
                query += " ,courses.viewpastcoursesdays ";
            //}

            if (showpastonlinecourses != null && showpastonlinecourses.Value != 1)
            {
                
            }
            else
            {
                query += ", courses.onlinecourse";
            }
            var now = System.DateTime.Now.ToShortDateString();
            if (ShowPastandTranscribedCourses)
            {
                now = System.DateTime.Now.AddYears(-1).ToShortDateString();
                query += " having datediff(DAY, getdate(), min([course times].coursedate)) >= -365 ";
            }
            else
            {
                query += " having datediff(DAY, getdate(), min([course times].coursedate)) >= courses.CourseCloseDays ";
            }
            
            if (Settings.Instance.GetMasterInfo3().allowviewpastcoursesdays != 0)
            {
                query += " or (datediff(DAY, getdate(), min([course times].coursedate)) >= (courses.viewpastcoursesdays*-1)) ";
            }

            if (showpastonlinecourses != null && showpastonlinecourses.Value != 1)
            {
                query += " or (datediff(DAY, getdate(), min([course times].coursedate)) >= (courses.viewpastcoursesdays*-1)) ";
            }
            else
            {
                query += " or ((courses.onlinecourse = 1 or courses.onlinecourse = -1) and getdate() between min([course times].coursedate) and max([course times].coursedate )) ";
            }

            query += " ) as SubQuery on ";
            query += subQueryCriteria == null ? "M.Courseid = SubQuery.courseid " : "S.MainCategoryID = SubQuery.MainCategoryID";
            return query;
        }

    }
}
