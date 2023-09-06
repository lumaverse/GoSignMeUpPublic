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
using Gsmu.Api.Data.School.Student;

namespace Gsmu.Api.Data.School.EmailAuditTrail
{
    public class Queries
    {
        public static GridModel<entities.EmailAuditTrail> GetUserEmails(QueryState state,int userid)
        {
            var currentUser = AuthorizationHelper.CurrentUser;
            string emailto = string.Empty;
            if (AuthorizationHelper.CurrentSupervisorUser != null || !currentUser.IsLoggedIn)
            {
                var studentuser = StudentHelper.GetStudent(userid);
                if (studentuser != null)
                {
                    emailto = studentuser.EMAIL;
                }
                else
                {
                    emailto = AuthorizationHelper.CurrentSupervisorUser.EMAIL;
                }
            }
            else
            {
                emailto = currentUser.SiteUserEmailAddress;
            }

            if (string.IsNullOrEmpty(emailto))
            {
                return null;
            }
            using (var db = new entities.SchoolEntities())
            {
                var query = from e in db.EmailAuditTrails where e.EmailTo.Trim() == emailto || e.EmailCC.Contains(emailto) || e.EmailBCC.Contains(emailto) select e;

                if (state.Filters != null)
                {
                    if (state.Filters.ContainsKey("keyword"))
                    {
                        var keyword = state.Filters["keyword"];
                        query = query.Where(e => e.EmailTo.Contains(keyword) || e.EmailCC.Contains(keyword) || e.EmailBCC.Contains(keyword) || e.EmailSubject.Contains(keyword) || e.EmailBody.Contains(keyword) || e.EmailFrom.Contains(keyword));
                    }
                }

                if (state.OrderFieldString != null)
                {
                    switch (state.OrderFieldString)
                    {
                        case "EmailSubject":
                            if (state.OrderByDirection == OrderByDirection.Ascending)
                            {
                                query = query.OrderBy(e => e.EmailSubject);
                            }
                            else
                            {
                                query = query.OrderByDescending(e => e.EmailSubject);
                            }
                            break;

                        case "AuditDate":
                            if (state.OrderByDirection == OrderByDirection.Ascending)
                            {
                                query = query.OrderBy(e => e.AuditDate);
                            }
                            else
                            {
                                query = query.OrderByDescending(e => e.AuditDate);
                            }
                            break;
                    }
                }
                else
                {
                    query = query.OrderByDescending(e => e.AuditDate);
                }

                var model = new GridModel<entities.EmailAuditTrail>(query.Count(), state);
                query = model.Paginate(query);
                model.Result = query.ToList();
                return model;
            }
        }
    }
}
