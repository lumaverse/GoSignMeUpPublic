using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using entities = Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Authorization;
using Gsmu.Api.Data;
using Gsmu.Api.Data.ViewModels.Grid;
using Gsmu.Api.Data.School.Entities;

namespace Gsmu.Api.Data.School.User
{
    public class Queries
    {
        public static GridModel<UserQueryListModel> GetStudentListQuery(QueryState state)
        {
            using (var db = new entities.SchoolEntities())
            {
                var query = (from u in db.Students
                             select new UserQueryListModel()
                             {
                                 userid = u.STUDENTID,
                                 last = u.LAST,
                                 first = u.FIRST,
                                 email = u.EMAIL,
                                 username = u.USERNAME,
                                 dateadded = u.DateAdded,
                                 date_modified = u.date_modified,
                                 date_bb_integrated = u.date_bb_integrated,

                             }
                            );
                if (state.Query != null)
                {
                    var keyword = state.Query;
                    int useridkeyword = -1;
                    if (int.TryParse(keyword, out useridkeyword)){}
                    query = query.Where(u => u.username.Contains(keyword) || u.userid == useridkeyword || u.first.Contains(keyword) || u.last.Contains(keyword) || u.email.Contains(keyword) || u.dateadded.ToString().Contains(keyword) || u.date_modified.ToString().Contains(keyword) || u.date_bb_integrated.ToString().Contains(keyword));
                }
                    query = query.OrderBy(u => u.last);
                    var model = new GridModel<UserQueryListModel>(query.Count(), state);
                query = model.Paginate(query);
                model.Result = query.ToList();
                return model;
            }
        }

        public static GridModel<UserQueryListModel> GetInstructorListQuery(QueryState state)
        {
            using (var db = new entities.SchoolEntities())
            {
                var query = (from u in db.Instructors
                             select new UserQueryListModel()
                             {
                                 userid = u.INSTRUCTORID,
                                 last = u.LAST,
                                 first = u.FIRST,
                                 email = u.EMAIL,
                                 username = u.USERNAME
                             }
                            );
                if (state.Query != null)
                {
                    var keyword = state.Query;
                    int useridkeyword = -1;
                    if (int.TryParse(keyword, out useridkeyword)) { }
                    query = query.Where(u => u.username.Contains(keyword) || u.userid == useridkeyword || u.first.Contains(keyword) || u.last.Contains(keyword) || u.email.Contains(keyword));
                }
                query = query.OrderBy(u => u.last);
                var model = new GridModel<UserQueryListModel>(query.Count(), state);
                query = model.Paginate(query);
                model.Result = query.ToList();
                return model;
            }
        }


        public static GridModel<UserQueryListModel> GetSupervisorListQuery(QueryState state)
        {
            using (var db = new entities.SchoolEntities())
            {
                var query = (from u in db.Supervisors
                             select new UserQueryListModel()
                             {
                                 userid = u.SUPERVISORID,
                                 last = u.LAST,
                                 first = u.FIRST,
                                 email = u.EMAIL,
                                 username = u.UserName
                             }
                            );
                if (state.Query != null)
                {
                    var keyword = state.Query;
                    int useridkeyword = -1;
                    if (int.TryParse(keyword, out useridkeyword)) { }
                    query = query.Where(u => u.username.Contains(keyword) || u.userid == useridkeyword || u.first.Contains(keyword) || u.last.Contains(keyword) || u.email.Contains(keyword));
                }
                query = query.OrderBy(u => u.last);
                var model = new GridModel<UserQueryListModel>(query.Count(), state);
                query = model.Paginate(query);
                model.Result = query.ToList();
                return model;
            }
        }


    }
}
