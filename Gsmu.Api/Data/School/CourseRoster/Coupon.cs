using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.School.Entities
{
    public partial class Coupon
    {
        public bool IsValidForShoppingCart
        {
            get
            {
                var cart = Gsmu.Api.Commerce.ShoppingCart.CourseShoppingCart.Instance;

                if (this.CouponCourseId.HasValue && this.CouponCourseId > 0)
                {
                    var result = cart.ContainsCourse(this.CouponCourseId.Value);
                    if (result)
                    {
                        return true;
                    }
                }

                if (!string.IsNullOrWhiteSpace(this.additionalcourseid))
                {
                    if (this.additionalcourseid.EndsWith(","))
                    {
                        this.additionalcourseid = this.additionalcourseid.Remove(this.additionalcourseid.Length - 1);

                    }
                    try
                    {
                        var ids = (from s in this.additionalcourseid.Split(',') select int.Parse(s)).ToList();
                        foreach (int id in ids)
                        {
                            if (id > 0 && cart.ContainsCourse(id))
                            {
                                return true;
                            }
                        }
                    }
                    catch
                    {
                        var ids = (from s in this.additionalcourseid.Split(',') select s).ToList();
                        foreach (var id in ids)
                        {
                            foreach (Gsmu.Api.Commerce.ShoppingCart.CourseShoppingCartItem cartitem in Gsmu.Api.Commerce.ShoppingCart.CourseShoppingCart.Instance.Items)
                            {
                                try
                                {
                                    if (id == cartitem.Course.CustomCourseField5.Replace(" ", ""))
                                    {
                                        return true;
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }

                if (!(this.CouponCourseId.HasValue && this.CouponCourseId > 0) && (string.IsNullOrWhiteSpace(this.additionalcourseid)) && this.CouponDollarAmount.Value <= float.Parse(Gsmu.Api.Commerce.ShoppingCart.CourseShoppingCart.Instance.SubTotal.ToString()))
                {
                    return true;
                }
                return false;
            }
        }

        public static int NextId
        {
            get
            {
                using (var db = Connections.GetSchoolConnection())
                {
                    db.Open();
                    var query = "select max(CouponId) + 1 from Coupons";
                    var cmd = db.CreateCommand();
                    cmd.CommandText = query;
                    var result = cmd.ExecuteScalar();
                    db.Close();
                    if (result == System.DBNull.Value)
                    {
                        return 1;
                    }
                    return (int)result;
                }
            }
        }

    }
}
