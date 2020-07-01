using EvaluatingApp.DatabaseContext;
using EvaluatingApp.Models;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace EvaluatingApp.Controllers
{
    public class HomeController : Controller
    {
        private PerformanceDbContext webDbContext;

        public HomeController()
        {
            this.webDbContext = new PerformanceDbContext();
        }
        public async Task<ActionResult> Index()
        {
            if (HttpContext.Request.HttpMethod == "POST")
            {
                var urlAddressInputBox = Request.Form["urlAddress"];
                var linkLimitInputBox = Int32.Parse(Request.Form["linkLimit"]);
                DataTable dt = await StartCrawlerAsync(urlAddressInputBox, linkLimitInputBox);

                return View(dt);
            }
            return View("Index");
        }

        public async Task<DataTable> StartCrawlerAsync(string urlAddress, int linkLimit)
        {
            var httpClient = new HttpClient();
            var html = await httpClient.GetStringAsync(urlAddress);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var allLinkList = htmlDocument.DocumentNode.SelectNodes("//a[@href]");

            DataTable urlResponseTable = CreateDataTable();

            for (int i = 0; i < allLinkList.Count; i++)
            {
                HtmlAttribute attribute = allLinkList[i].Attributes["href"];

                if ((attribute.Value.Contains(urlAddress) || attribute.Value.StartsWith("/")) && urlResponseTable.Rows.Count < linkLimit)
                {
                    var urlStr = attribute.Value;

                    string urlPattern = $@"({Regex.Escape(urlAddress)}[^?#]*)(\?|#)?";

                    if (urlStr.StartsWith("/"))
                    {
                        urlStr = urlAddress + attribute.Value;
                    }

                    var matchedUrl = Regex.Replace(urlStr, urlPattern, "$1");

                    if (urlResponseTable.Select($"Url = '{matchedUrl}'").Length == 0)
                    {
                        try
                        {
                            var existingUrl = this.webDbContext.UrlAdresses.Where(url => url.urlAddress == matchedUrl).FirstOrDefault();

                            if (existingUrl == null)
                            {
                                existingUrl = new UrlAddress(matchedUrl);
                                this.webDbContext.UrlAdresses.Add(existingUrl).State = EntityState.Added;
                                this.webDbContext.SaveChanges();
                            }

                            Response reponseTimeModel = new Response(existingUrl.Id, this.GetResponseTime(matchedUrl));
                            this.webDbContext.Responses.Add(reponseTimeModel);

                            DataRow row = urlResponseTable.NewRow();
                            row["Url"] = matchedUrl;
                            row["Response Time"] = reponseTimeModel.ResponseTime + " s";
                            row["UrlLength"] = matchedUrl.Length;
                            row["id"] = existingUrl.Id;
                            urlResponseTable.Rows.Add(row);
                        }
                        catch (WebException)
                        {

                        }
                    }
                }
            }
            //
            // Sorting.
            //

            var sortedView = urlResponseTable.DefaultView;
            sortedView.Sort = "UrlLength";
            var sortedTable = sortedView.ToTable();

            for (int i = 0, row = 1; i < sortedTable.Rows.Count; i++, row++)
            {
                sortedTable.Rows[i]["№"] = row;
            }
            this.webDbContext.SaveChanges();

            return sortedTable;
        }

        private double GetResponseTime(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            System.Diagnostics.Stopwatch timer = new Stopwatch();

            timer.Start();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            response.Close();

            timer.Stop();

            return timer.Elapsed.TotalSeconds;
        }

        private DataTable CreateDataTable()
        {
            DataTable urlResponseTable = new DataTable("Performance");

            DataColumn urlLength = new DataColumn("UrlLength");
            urlLength.DataType = System.Type.GetType("System.Int32");
            urlLength.ReadOnly = true;
            urlLength.Unique = false;
            urlResponseTable.Columns.Add(urlLength);

            DataColumn urlId = new DataColumn("id");
            urlId.DataType = System.Type.GetType("System.Int32");
            urlId.ReadOnly = true;
            urlId.Unique = true;
            urlResponseTable.Columns.Add(urlId);

            DataColumn orderId = new DataColumn("№");
            orderId.DataType = System.Type.GetType("System.Int32");
            orderId.ReadOnly = false;
            orderId.Unique = false;
            urlResponseTable.Columns.Add(orderId);

            DataColumn url = new DataColumn("Url");
            url.DataType = System.Type.GetType("System.String");
            url.ReadOnly = false;
            url.Unique = true;
            urlResponseTable.Columns.Add(url);

            DataColumn responseTime = new DataColumn("Response Time");
            responseTime.DataType = System.Type.GetType("System.String");
            responseTime.ReadOnly = true;
            urlResponseTable.Columns.Add(responseTime);

            return urlResponseTable;
        }
    }
}