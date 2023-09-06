using Gsmu.Api.Data.School.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gsmu.Api.Data.ViewModels
{
    public class CreditCardPaymentModel
    {
        public CreditCardPaymentModel() { }
        public IEnumerable<Payment_Option> AcceptedCreditCardTypes { get; set; }
        public string Url
        {
            get;
            set;
        }
        public string LoginID
        {
            get;
            set;
        }
        public string LoginKey
        {
            get;
            set;
        }
        public string CardNumber
        {
            get;
            set;
        }
        public string CardType
        {
            get;
            set;
        }
        public string ExpiryMonth
        {
            get;
            set;
        }
        public string ExpiryYear
        {
            get;
            set;
        }
        public string FirstName
        {
            get;
            set;
        }
        public string LastName
        {
            get;
            set;
        }
        public string Address
        {
            get;
            set;
        }
        public string Address2
        {
            get;
            set;
        }
        public string City
        {
            get;
            set;
        }

        public string State
        {
            get;
            set;
        }

        public string Zip
        {
            get;
            set;
        }
        public string Country
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public string Email
        {
            get;
            set;
        }
        public string CCV
        {
            get;
            set;
        }
        public string PaymentType
        {
            get;
            set;
        }
        public string Telephone
        {
            get;
            set;
        }
        public string OrderNumber
        {
            get;
            set;
        }
        public string PaymentNumber
        {
            get;
            set;
        }

        public double TotalPaid
        {
            get;
            set;
        }

        public string[] CurrentUrl
        {
            get;
            set;
        }

        public string LongOrderId
        {
            get;
            set;
        }

        public double CreditCardFee
        {
            get;
            set;
        }

        public string ReceiptUrl { get; set; }

        public string RespMsg { get; set; }

        public string AuthNum { get; set; }

        public string RefNumber { get; set; }

        public string Result { get; set; }

        public string ActiveCCPayMethod { get; set; }
    }
}
