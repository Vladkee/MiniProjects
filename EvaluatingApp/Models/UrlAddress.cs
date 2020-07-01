using EvaluatingApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace EvaluatingApp.Models
{
    public class UrlAddress
    {
        public int Id { get; set; }

        public string urlAddress { get; set; }

        public List<Response> ResponsesList { get; set; }

        public UrlAddress(string urlAddress)
        {
            this.urlAddress = urlAddress;
        }
    }
}