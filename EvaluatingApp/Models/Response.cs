using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Web;

namespace EvaluatingApp.Models
{
    public class Response
    {
        public int Id { get; set; }

        public double ResponseTime { get; set; }

        public DateTime EvaluatedDate { get; set; }

        public int UrlAddressId { get; set; }

        public UrlAddress Url { get; set; }

        public Response(int UrlAddressId, double ResponseTime)
        {
            this.UrlAddressId = UrlAddressId;
            this.ResponseTime = ResponseTime;
            this.EvaluatedDate = DateTime.Now;
        }
    }
}