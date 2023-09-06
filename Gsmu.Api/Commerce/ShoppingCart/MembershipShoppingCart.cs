using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.SessionState;
using System.Web;
using Gsmu.Api.Data.School.Course;
using Gsmu.Api.Web;
using Gsmu.Api.Authorization;
using Gsmu.Api.Data.School.Entities;
using Gsmu.Api.Data;
using Gsmu.Api.Data.School.Student;
using Gsmu.Api.Data.School.Supervisor;

namespace Gsmu.Api.Commerce.ShoppingCart
{
    public class MembershipShoppingCart
    {
        #region Static

        public static MembershipShoppingCart Instance
        {
            get
            {
                var cart = ObjectHelper.GetSessionObject<MembershipShoppingCart>(WebContextObject.MembershipShoppingCart);
                if (cart == null)
                {
                    cart = new MembershipShoppingCart();
                    ObjectHelper.SetSessionObject<MembershipShoppingCart>(WebContextObject.MembershipShoppingCart, cart);
                }
                return cart;
            }
        }

        #endregion


        internal List<MembershipShoppingCartItem> memberships = new List<MembershipShoppingCartItem>();

        /// <summary>
        /// The count of items in the cart.
        /// </summary>
        public int Count
        {
            get
            {
                return memberships.Count;
            }
        }

        public IEnumerable<MembershipShoppingCartItem> Items
        {
            get
            {
                return memberships.AsEnumerable();
            }

        }

        //public int MembershipID
        //{
        //    get;
        //    set;
        //}

        public void AddMembership(int membershipID)
        {
            memberships.Add(new MembershipShoppingCartItem() { MembershipID = membershipID });

        }

        public decimal SubTotal
        {
            get
            {
                decimal subTotal = decimal.Parse((from i in memberships select i.Membership.PurchasePrice).Sum().ToString());
                return subTotal;
            }
        }

        public string Status
        {
            get
            {
                int membrspCount = memberships.Count;
                int courseCount = CourseShoppingCart.Instance.Count;
                int totalCount = membrspCount + courseCount;

                if (totalCount == 0)
                {
                    return "Empty";
                }
                else if (totalCount == 1)
                {
                    return "1 item";
                }
                else
                {
                    return totalCount + " items";
                }

            }
        }

    }
}
