using Gsmu.Api.Commerce.ShoppingCart;
using Gsmu.Web.Areas.Public.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gsmu.Web.Areas.Adm.Controllers
{
    public class AdministratorEnrollmentController
    {
        public static void SetPrincipalStudent(int studentid)
        {
            CartController cart = new CartController();
            cart.Empty();
            CourseShoppingCart.Instance.MultipleOrder_PrincipalStudent = studentid;
            CourseShoppingCart.Instance.MultipleOrder_SelectedStudent = studentid;
            var student = Gsmu.Api.Data.School.Entities.Student.GetStudent(studentid);
            CourseShoppingCart.Instance.MultipleOrder_PrincipalStudentName = student.USERNAME +" "+ student.FIRST +" "+ student.LAST;
        }
    }
}