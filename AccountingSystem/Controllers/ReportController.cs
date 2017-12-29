using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AccountingSystem.Models;
using AccountingSystem.Models.ViewModel;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.Web;

namespace AccountingSystem.Controllers
{
    public class ReportController : Controller
    {
        private ReportDocument report = new ReportDocument();
        public bool RenewSession()
        {
            if (ControllerContext.HttpContext.Request.Cookies["userid"] != null)
            {
                var userid = ControllerContext.HttpContext.Request.Cookies["userid"].Value;
                if (Session["loggedinUser"] == null)
                {
                    var user = AccountingDbGateway.GetAllUsers().FirstOrDefault(x => x.UserId.ToString() == userid && x.ValidUser);
                    Session["loggedinUser"] = user;
                }
                return true;
            }
            return false;
        }
        public ActionResult Index(string reportId)
        {
            AccountingDbGateway.OpenConnection();
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            if (reportId != null)
                ViewBag.ReportId = reportId;
            ViewBag.AccordionId = "collapsethree,3";
            //ViewBag.CompanyList = AccountingDbGateway.GetCompanyList();
            ViewBag.District = AccountingDbGateway.GetDistricts();
            AccountingDbGateway.CloseConnection();
            return View();
        }

        public JsonResult GetSubreports()
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetSubreports();
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetType2Report(string type, string startingDate, string endDate, string info, int num, string mainGroup = "")
        {
            AccountingDbGateway.OpenConnection();
            if (num == 0)
            {
                var ledgers = AccountingDbGateway.GetTrialBalanceReport(type, startingDate, endDate);
                Session["reportName"] = "rptTrialBalance.rpt";
                Session["Report"] = ledgers;
                var title = type == "Month"
                    ? "For the Month of " +
                      new DateTime(Convert.ToInt32(endDate), Convert.ToInt32(startingDate), 1).ToString("MMMM, yyyy")
                    : "For the date " + startingDate + " to " + endDate;
                Session["ReportTitle"] = title;
            }
            else if (num == 6 || num == 8)
            {
                object products;
                switch (num)
                {
                    case 6:
                        {
                            products = AccountingDbGateway.GetReportRpt(startingDate, endDate, type, num, 1);
                            Session["ReportTitle"] = info;
                        }
                        break;
                    default:
                        {
                            var str = info.Split('#');
                            products = AccountingDbGateway.GetReportRpt(startingDate, endDate, type, num, Convert.ToInt32(str[1]));
                            Session["ReportTitle"] = str[0];
                        }
                        break;
                }
                if (mainGroup == "Revenue" || mainGroup == "Expense")
                    Session["reportName"] = "rptGeneralLedger2.rpt";
                else
                    Session["reportName"] = "rptGeneralLedger.rpt";
                Session["Report"] = products;
                Session["ReportSubTitle"] = "For the date " + startingDate + " to " + endDate;
            }
            else if (num == 9)
            {
                var ledgers = AccountingDbGateway.GetReportRpt(startingDate, endDate, type, num, 0);
                Session["reportName"] = "rptFixedAssets.rpt";
                Session["Report"] = ledgers;
                var subtitle = "Depreciation Schedule as at " + DateTime.Parse(startingDate).ToString("MMM dd, yyyy");
                Session["ReportTitle"] = info;
                Session["ReportSubTitle"] = subtitle;
            }
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAccountReceivableReport(string type)
        {
            AccountingDbGateway.OpenConnection();
            var products = AccountingDbGateway.GetAccountReceivableReport(type);
            AccountingDbGateway.CloseConnection();
            Session["reportName"] = "rptAccountsReceivable.rpt";
            Session["Report"] = products;
            var title = type == "PRO" ? "Product wise Account Receivables Today" : "Company wise Account Receivables Today";
            Session["ReportTitle"] = title;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetReportType1(string startingDate, string endDate, string type, string info, int num, string districtId, string districtName, string districtWise, string approved, string districtSummary)
        {
            var start = startingDate.Split(',');
            var end = endDate.Split(',');
            var startDate = new DateTime(Convert.ToInt32(start[1]), Convert.ToInt32(start[0]), 1);
            var endingDate = new DateTime(Convert.ToInt32(end[1]), Convert.ToInt32(end[0]),
                DateTime.DaysInMonth(Convert.ToInt32(end[1]), Convert.ToInt32(end[0])));
            var diff = 0;
            var cp = 0;
            if (Convert.ToInt32(start[1]) == Convert.ToInt32(end[1]))
            {
                diff = Convert.ToInt32(end[0]) - Convert.ToInt32(start[0]);
                cp = diff + 1;
            }
            else
            {
                diff = (Convert.ToInt32(end[1]) - Convert.ToInt32(start[1])) * 12 -
                       (Convert.ToInt32(start[0]) - Convert.ToInt32(end[0])) + 1;
                cp = diff;
            }
            var dId = string.IsNullOrEmpty(districtId) ? "" : districtId;
            var dName = string.IsNullOrEmpty(districtName) ? "" : districtName;
            var isDistrictWise = string.IsNullOrEmpty(districtWise) ? "0" : districtWise;
            var isApproved = string.IsNullOrEmpty(approved) ? "0" : approved;
            Session["District"] = dName == "Select" || dName == "" ? "" : "District: " + dName;
            AccountingDbGateway.OpenConnection();
            var products = AccountingDbGateway.GetReportRpt(startDate.ToString("MM/dd/yyyy"),
                endingDate.ToString("MM/dd/yyyy"), type, num, cp, dId, isDistrictWise, isApproved);
            AccountingDbGateway.CloseConnection();
            districtSummary = string.IsNullOrEmpty(districtSummary) ? "0" : districtSummary;
            var name = "";
            Session["Report"] = products;
            switch (num)
            {
                case 4:
                    name = "rptProfitLoss1.rpt";
                    break;
                case 7:
                    {
                        name = type == "MB" ? "rptBalanceSheet1.rpt" : "rptOtherBalanceSheet.rpt";
                    }
                    break;
                default:
                    name = isDistrictWise == "0" ? "rptRevenue.rpt" : "rptRevenue_DistrictWise.rpt";
                    break;
            }

            Session["reportName"] = name;

            Session["diff"] = diff;
            Session["month"] = start[0] + "," + end[0];
            Session["year"] = start[1] + "," + end[1];
            Session["info"] = info;
            Session["reportSubtitle"] = "For the Month " + startDate.ToString("MMM-yy") + " to " +
                                            endingDate.ToString("MMM-yy");
            Session["num"] = num;
            var title = "";
            switch (num)
            {
                case 0:
                    title = "Monthly Account Receivable (";
                    break;
                case 1:
                    title = "Monthly Revenue Statement (";
                    break;
                case 2:
                    title = "Monthly Expense Statement (";
                    break;
                case 3:
                    title = "Monthly Cash Collection Statement (";
                    break;
                case 4:
                    title = "Monthly Income Statement (";
                    break;
                case 5:
                    title = "Monthly Cash & Bank Details(";
                    break;
                case 7:
                    title = "Monthly Balance Sheet(" + info + ")";
                    break;
            }
            switch (type)
            {
                case "PRO":
                    title += "Product wise-";
                    break;
                case "COM":
                    title += "Company wise-";
                    break;
            }
            if (num == 4 && type == "PL")
            {
                title = "Monthly Profit & Loss Statement (";
            }
            switch (info)
            {
                case "0":
                    title += "Summary)";
                    break;
                case "1":
                    title += "Detail)";
                    break;
            }
            Session["reportTitle"] = title;
            Session["type"] = type;
            Session["isDistrictWiseSummary"] = "0";
            if (districtSummary == "1")
            {
                //Session["reportName"] = "rptRevenue_DistrictWiseSummary.rpt";
                Session["isDistrictWiseSummary"] = "1";
                Session["District"] = "District Name";
                Session["reportTitle"] = "Monthly Revenue Statement(District Wise)";
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public void Show(string type)
        {
            if (Session["reportName"] != null)
            {
                var reportName = Session["reportName"].ToString();
                string reportPath = Server.MapPath("~/Reports/" + reportName);
                switch (reportName)
                {
                    case "rptInvoiceList.rpt":
                        {
                            var invoiceList = (List<InvoiceListReport>)Session["Report"];
                            report.Load(reportPath);
                            report.SetDataSource(invoiceList);

                            report.SetParameterValue("company", Session["company"]);
                            report.SetParameterValue("criteria", Session["criteria"]);

                        }
                        break;
                    case "rptInvoice.rpt":
                        {
                            var invoice = (List<InvoiceReport>)Session["Report"];
                            report.Load(reportPath);
                            report.SetDataSource(invoice);
                            //report.SetParameterValue("isRequested", (bool)Session["isRequested"]);
                            //report.SetParameterValue("isColor", (bool)Session["isColor"]);
                            if ((bool)Session["isColor"])
                            {
                                report.ReportDefinition.ReportObjects["Line5"].ObjectFormat.EnableSuppress = true;
                                report.ReportDefinition.ReportObjects["Line6"].ObjectFormat.EnableSuppress = true;
                                report.ReportDefinition.ReportObjects["Text16"].ObjectFormat.EnableSuppress = true;
                                report.ReportDefinition.ReportObjects["Text17"].ObjectFormat.EnableSuppress = true;
                                report.ReportDefinition.ReportObjects["picBlack"].ObjectFormat.EnableSuppress = true;
                            }
                            if (!(bool)Session["isRequested"])
                            {
                                report.ReportDefinition.ReportObjects["txtReminder"].ObjectFormat.EnableSuppress = true;
                            }
                        }
                        break;
                    case "rptLabel.rpt":
                        {

                            var labels = (List<LabelReport>)Session["Report"];
                            report.Load(reportPath);
                            report.SetDataSource(labels);
                            report.PrintOptions.PaperOrientation = PaperOrientation.Landscape;
                            report.PrintOptions.PaperSize = PaperSize.PaperEnvelopeDL;

                            report.SetParameterValue("companyBold", Session["companyBold"]);
                            report.SetParameterValue("contactBold", Session["contactBold"]);
                            report.SetParameterValue("fontSize", Session["fontSize"]);
                        }
                        break;
                    case "rptOutStanding.rpt":
                        {
                            var invoice = (List<OutstandingInvoiceReport>)Session["Report"];
                            report.Load(reportPath);
                            report.SetDataSource(invoice);
                        }
                        break;
                    case "rptRemarks.rpt":
                        {
                            var invoice = (List<InvoiceRemarkReport>)Session["Report"];
                            report.Load(reportPath);
                            report.SetDataSource(invoice);
                        }
                        break;
                    case "rptJournalVoucher.rpt":
                        {
                            var vouchers = (List<JournalVoucherReport>)Session["Report"];
                            report.Load(reportPath);
                            report.SetDataSource(vouchers);
                        }
                        break;
                    case "rptListOfJournal.rpt":
                        {
                            var journals = (List<JournalForReport>)Session["Report"];
                            report.Load(reportPath);
                            report.SetDataSource(journals);
                        }
                        break;
                    case "rptAuditJournal.rpt":
                        {
                            var journals = (List<Journal>)Session["Report"];
                            var temp = new Journal();
                            var count = 0;
                            foreach (Journal journal in journals)
                            {
                                count++;
                                if (count > 1)
                                {
                                    if (journal.JId == temp.JId)
                                        journal.Background = temp.Background;
                                    else
                                    {
                                        journal.Background = temp.Background == 0 ? 1 : 0;
                                    }
                                    temp = journal;
                                }
                                else
                                {
                                    journal.Background = 0;
                                    temp = journal;
                                }
                            }
                            foreach (Journal journal in journals)
                            {
                                journal.JId = journal.Background;
                            }
                            report.Load(reportPath);
                            report.SetDataSource(journals);
                            report.SummaryInfo.ReportTitle = Session["ReportTitle"].ToString();
                            report.SummaryInfo.ReportComments = Session["ReportComment"].ToString();
                        }
                        break;
                    case "rptRecceipt.rpt":
                        {
                            var receipts = (List<MoneyReceipt>)Session["Report"];
                            var str = "";
                            var str1 = "";
                            var list = new List<string>();
                            foreach (var moneyReceipt in receipts)
                            {
                                str += "\n" + moneyReceipt.Invoice_No;
                                list.Add(moneyReceipt.sbname);

                            }
                            foreach (var st in list.Distinct().ToList())
                            {
                                str1 += "\n" + st;
                            }
                            report.Load(reportPath);
                            report.SetDataSource(receipts);
                            report.SetParameterValue("InvoiceNos", str.Substring(1));
                            report.SetParameterValue("SbNames", str1.Substring(1));
                            report.SetParameterValue("Name", receipts[0].Name + " " + "Tk. " + receipts[0].TAmount + " " + Session["str"]);
                        }
                        break;
                    case "rptTrialBalance.rpt":
                        {
                            var ledgers = (List<Ledger>)Session["Report"];
                            report.Load(reportPath);
                            report.SetDataSource(ledgers);
                            report.SummaryInfo.ReportTitle = Session["ReportTitle"].ToString();
                        }
                        break;
                    case "rptAccountsReceivable.rpt":
                        {
                            var products = (List<ProductForInvoice>)Session["Report"];
                            report.Load(reportPath);
                            report.SetDataSource(products);
                            report.SummaryInfo.ReportTitle = Session["ReportTitle"].ToString();
                        }
                        break;
                    case "rptRevenue.rpt":
                    case "rptRevenue_DistrictWise.rpt":
                    //case "rptRevenue_DistrictWiseSummary.rpt":
                        {
                            var products = (List<ReportRpt>)Session["Report"];
                            report.Load(reportPath);
                            report.SetDataSource(products);
                            var det = Convert.ToInt32(Session["info"]) == 0;

                            report.SetParameterValue("isSummary", det);
                            report.SetParameterValue("DistrictName", Session["District"].ToString());
                            var diff = Convert.ToInt32(Session["diff"]);
                            var year = Session["year"].ToString().Split(',');
                            var month = Session["month"].ToString().Split(',');
                            for (var j = 1; j <= 12; j++)
                            {
                                report.SetParameterValue("tM" + j, "0");
                            }
                            
                            var c = 3000;
                            report.ReportDefinition.ReportObjects["fldSuM" + Convert.ToInt32(month[0])].Left = c;
                            report.ReportDefinition.ReportObjects["txtM" + Convert.ToInt32(month[0])].Left = c;
                            report.ReportDefinition.ReportObjects["fldGM" + Convert.ToInt32(month[0])].Left = c;
                            report.ReportDefinition.ReportObjects["fldM" + Convert.ToInt32(month[0])].Left = c;
                            report.ReportDefinition.ReportObjects["fldSM" + Convert.ToInt32(month[0])].Left = c;
                            if (reportName == "rptRevenue_DistrictWise.rpt")
                            {
                                report.ReportDefinition.ReportObjects["fldSmD" + Convert.ToInt32(month[0])].Left = c; 
                                report.ReportDefinition.ReportObjects["D" + Convert.ToInt32(month[0])].Left = c;
                                report.SetParameterValue("isDistrictWiseSummary", Session["isDistrictWiseSummary"].ToString() == "1");
                            }
                            if (Convert.ToInt32(month[0]) < Convert.ToInt32(month[1]))
                            {
                                report.SetParameterValue("tM" + Convert.ToInt32(month[0]),
                                    new DateTime(Convert.ToInt32(year[0]), Convert.ToInt32(month[0]), 1).ToString("MMM-yy"));
                                for (var i = Convert.ToInt32(month[0]) + 1; i <= Convert.ToInt32(month[1]); i++)
                                {
                                    c += 920;
                                    report.ReportDefinition.ReportObjects["fldSuM" + i].Left =
                                        report.ReportDefinition.ReportObjects["fldGM" + i].Left =
                                            report.ReportDefinition.ReportObjects["fldM" + i].Left =
                                                report.ReportDefinition.ReportObjects["fldSM" + i].Left = c;
                                    report.ReportDefinition.ReportObjects["txtM" + i].Left = c;
                                    report.SetParameterValue("tM" + i, new DateTime(Convert.ToInt32(year[0]), i, 1).ToString("MMM-yy"));
                                    if (reportName == "rptRevenue_DistrictWise.rpt")
                                    {
                                        report.ReportDefinition.ReportObjects["fldSmD" + i].Left = c;
                                        report.ReportDefinition.ReportObjects["D" + i].Left = c;
                                    }
                                }
                            }
                            else
                            {
                                report.SetParameterValue("tM" + Convert.ToInt32(month[0]),
                                    new DateTime(Convert.ToInt32(year[0]), Convert.ToInt32(month[0]), 1).ToString("MMM-yy"));
                                var z = Convert.ToInt32(month[0]);
                                var zx = Convert.ToInt32(year[0]);
                                for (var i = 1; i <= diff - 1; i++)
                                {
                                    c += 920;
                                    z++;
                                    if (z > 12)
                                    {
                                        z = 1;
                                        zx = zx + 1;
                                    }
                                    report.ReportDefinition.ReportObjects["fldSuM" + z].Left =
                                        report.ReportDefinition.ReportObjects["fldM" + z].Left =
                                            report.ReportDefinition.ReportObjects["fldGM" + z].Left =
                                                report.ReportDefinition.ReportObjects["fldSM" + z].Left = c;
                                    report.ReportDefinition.ReportObjects["txtM" + z].Left = c;
                                    report.SetParameterValue("tM" + z, new DateTime(zx, z, 1).ToString("MMM-yy"));
                                    if (reportName == "rptRevenue_DistrictWise.rpt")
                                    {
                                        report.ReportDefinition.ReportObjects["fldSmD" + z].Left = c;
                                        report.ReportDefinition.ReportObjects["D" + z].Left = c;
                                    }
                                }
                            }
                            report.ReportDefinition.ReportObjects["Text3"].Width =
                                report.ReportDefinition.ReportObjects["Text4"].Width = c + 920;
                            report.ReportDefinition.ReportObjects["txtText"].Left = c - 920;
                            //report.ReportDefinition.ReportObjects["fldSuYrToDt"].ObjectFormat.EnableSuppress =
                            //    report.ReportDefinition.ReportObjects["txtYrToDt"].ObjectFormat.EnableSuppress =
                            //        report.ReportDefinition.ReportObjects["fldGYrToDt"].ObjectFormat.EnableSuppress =
                            //            report.ReportDefinition.ReportObjects["fldYrToDt"].ObjectFormat.EnableSuppress =
                            //                report.ReportDefinition.ReportObjects["fldSYrToDt"].ObjectFormat.EnableSuppress =
                            //                    true;

                            var num = Convert.ToInt32(Session["num"]);
                            var cash = num == 5;
                            report.SetParameterValue("isCash", cash);
                            //if (num == 2 || num == 3)
                            //{
                            report.ReportDefinition.ReportObjects["Text3"].Width =
                                report.ReportDefinition.ReportObjects["Text4"].Width = (c + 920 + 1020);

                            report.ReportDefinition.ReportObjects["fldSuYrToDt"].Left =
                                report.ReportDefinition.ReportObjects["txtYrToDt"].Left =
                                    report.ReportDefinition.ReportObjects["fldGYrToDt"].Left =
                                        report.ReportDefinition.ReportObjects["fldYrToDt"].Left =
                                            report.ReportDefinition.ReportObjects["fldSYrToDt"].Left = c + 920;
                            if (reportName == "rptRevenue_DistrictWise.rpt")
                            {
                                report.ReportDefinition.ReportObjects["fldSmDYrToDt"].Left = c + 920;
                                report.ReportDefinition.ReportObjects["fldSmDYrToDt"].ObjectFormat.EnableSuppress = false;
                                report.ReportDefinition.ReportObjects["SumD"].Left = c + 920;
                                report.ReportDefinition.ReportObjects["SumD"].ObjectFormat.EnableSuppress = false;
                            }
                            report.ReportDefinition.ReportObjects["fldSuYrToDt"].ObjectFormat.EnableSuppress =
                                report.ReportDefinition.ReportObjects["txtYrToDt"].ObjectFormat.EnableSuppress =
                                    report.ReportDefinition.ReportObjects["fldGYrToDt"].ObjectFormat.EnableSuppress =
                                        report.ReportDefinition.ReportObjects["fldYrToDt"].ObjectFormat.EnableSuppress =
                                            report.ReportDefinition.ReportObjects["fldSYrToDt"].ObjectFormat.EnableSuppress
                                                = false;
                            report.ReportDefinition.ReportObjects["txtText"].Left = (c - 920 + 1020);
                            //}
                            if (det == false)
                            {
                                for (var j = 1; j <= 12; j++)
                                {
                                    report.ReportDefinition.ReportObjects["fldSuM" + j].ObjectFormat.EnableSuppress = true;
                                }
                                report.ReportDefinition.ReportObjects["fldSuYrToDt"].ObjectFormat.EnableSuppress = true;
                            }
                            
                            report.SummaryInfo.ReportTitle = Session["reportTitle"].ToString();
                            report.SummaryInfo.ReportComments = Session["reportSubtitle"].ToString();
                        }
                        break;
                    case "rptProfitLoss1.rpt":
                        {
                            var field = new[] { "15", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38" };
                            var list = new List<int>();
                            var products = (List<ReportRpt>)Session["Report"];
                            report.Load(reportPath);
                            report.SetDataSource(products);
                            var det = Convert.ToInt32(Session["info"]) == 0;
                            var pl = Session["type"].ToString() == "PL";
                            report.SetParameterValue("isAccountReceivable", det);
                            report.SetParameterValue("IsProfitLoss", pl);
                            var diff = Convert.ToInt32(Session["diff"]);
                            var year = Session["year"].ToString().Split(',');
                            var month = Session["month"].ToString().Split(',');
                            for (var j = 1; j <= 12; j++)
                            {
                                report.SetParameterValue("tM" + j, "0");
                            }
                            var c = 2400;
                            report.ReportDefinition.ReportObjects["fldSuM" + Convert.ToInt32(month[0])].Left = c;
                            report.ReportDefinition.ReportObjects["txtM" + Convert.ToInt32(month[0])].Left = c;
                            report.ReportDefinition.ReportObjects["fldM" + Convert.ToInt32(month[0])].Left = c;
                            report.ReportDefinition.ReportObjects["fldSM" + Convert.ToInt32(month[0])].Left = c;
                            report.ReportDefinition.ReportObjects["Field" + field[Convert.ToInt32(month[0]) - 1]].Left = c;
                            report.ReportDefinition.ReportObjects["Field" + (Convert.ToInt32(month[0]) + 2)].Left = c;
                            var s = (Convert.ToInt32(month[0]) + 40);
                            if (s >= 50)
                                s++;
                            report.ReportDefinition.ReportObjects["Field" + s].Left = c;
                            report.ReportDefinition.ReportObjects["fldGSM" + Convert.ToInt32(month[0])].Left = c;
                            list.Add(Convert.ToInt32(month[0]));
                            if (Convert.ToInt32(month[0]) < Convert.ToInt32(month[1]))
                            {
                                report.SetParameterValue("tM" + Convert.ToInt32(month[0]),
                                    new DateTime(Convert.ToInt32(year[0]), Convert.ToInt32(month[0]), 1).ToString("MMM-yy"));
                                for (var i = Convert.ToInt32(month[0]) + 1; i <= Convert.ToInt32(month[1]); i++)
                                {
                                    c += 960;
                                    report.ReportDefinition.ReportObjects["fldSuM" + i].Left =
                                        report.ReportDefinition.ReportObjects["fldM" + i].Left =
                                            report.ReportDefinition.ReportObjects["fldSM" + i].Left = c;
                                    report.ReportDefinition.ReportObjects["txtM" + i].Left = c;
                                    report.ReportDefinition.ReportObjects["Field" + field[i - 1]].Left = c;
                                    report.ReportDefinition.ReportObjects["fldGSM" + i].Left = c;
                                    report.ReportDefinition.ReportObjects["Field" + (i + 2)].Left = c;
                                    var s1 = i + 40;
                                    if (s1 >= 50)
                                        s1++;
                                    report.ReportDefinition.ReportObjects["Field" + s1].Left = c;
                                    list.Add(i);
                                    report.SetParameterValue("tM" + i, new DateTime(Convert.ToInt32(year[0]), i, 1).ToString("MMM-yy"));
                                }
                            }
                            else
                            {
                                report.SetParameterValue("tM" + Convert.ToInt32(month[0]),
                                    new DateTime(Convert.ToInt32(year[0]), Convert.ToInt32(month[0]), 1).ToString("MMM-yy"));
                                var z = Convert.ToInt32(month[0]);
                                var zx = Convert.ToInt32(year[0]);
                                for (var i = 1; i <= diff - 1; i++)
                                {
                                    c += 960;
                                    z++;
                                    if (z > 12)
                                    {
                                        z = 1;
                                        zx = zx + 1;
                                    }
                                    report.ReportDefinition.ReportObjects["fldSuM" + z].Left =
                                        report.ReportDefinition.ReportObjects["fldM" + z].Left =
                                            report.ReportDefinition.ReportObjects["fldSM" + z].Left = c;
                                    report.ReportDefinition.ReportObjects["txtM" + z].Left = c;
                                    report.ReportDefinition.ReportObjects["Field" + field[z - 1]].Left = c;
                                    report.ReportDefinition.ReportObjects["fldGSM" + z].Left = c;
                                    report.ReportDefinition.ReportObjects["Field" + (z + 2)].Left = c;
                                    var s2 = z + 40;
                                    if (s2 >= 50)
                                        s2++;
                                    report.ReportDefinition.ReportObjects["Field" + s2].Left = c;
                                    list.Add(z);
                                    report.SetParameterValue("tM" + z, new DateTime(zx, z, 1).ToString("MMM-yy"));
                                }
                            }
                            report.ReportDefinition.ReportObjects["Text3"].Width = c + 960 + 1040;
                            for (var p = 6; p <= 18; p++)
                            {
                                report.ReportDefinition.ReportObjects["Text" + p].Width = c + 960 + 1040 - 2400;
                            }
                            for (var t = 1; t <= 12; t++)
                            {
                                if (!list.Contains(t))
                                {
                                    report.ReportDefinition.ReportObjects["fldSuM" + t].ObjectFormat.EnableSuppress =
                                        report.ReportDefinition.ReportObjects["fldM" + t].ObjectFormat.EnableSuppress =
                                            report.ReportDefinition.ReportObjects["fldSM" + t].ObjectFormat.EnableSuppress = true;
                                    report.ReportDefinition.ReportObjects["txtM" + t].ObjectFormat.EnableSuppress = true;
                                    report.ReportDefinition.ReportObjects["Field" + field[t - 1]].ObjectFormat.EnableSuppress = true;
                                    report.ReportDefinition.ReportObjects["fldGSM" + t].ObjectFormat.EnableSuppress = true;
                                    report.ReportDefinition.ReportObjects["Field" + (t + 2)].ObjectFormat.EnableSuppress = true;
                                    var s3 = t + 40;
                                    if (s3 >= 50)
                                        s3++;
                                    report.ReportDefinition.ReportObjects["Field" + s3].ObjectFormat.EnableSuppress = true;
                                }
                            }
                            report.ReportDefinition.ReportObjects["txtText"].Left = c - 960 + 1040;
                            var num = Convert.ToInt32(Session["num"]);
                            report.ReportDefinition.ReportObjects["fldSuYrToDt"].Left =
                                report.ReportDefinition.ReportObjects["txtYrToDt"].Left =
                                    report.ReportDefinition.ReportObjects["fldGSM13"].Left =
                                        report.ReportDefinition.ReportObjects["fldYrToDt"].Left =
                                            report.ReportDefinition.ReportObjects["fldTNet"].Left =
                                                report.ReportDefinition.ReportObjects["fldTProfit"].Left =
                                                    report.ReportDefinition.ReportObjects["fldNetProfit"].Left =
                                                        report.ReportDefinition.ReportObjects["fldTCash13"].Left =
                                                            report.ReportDefinition.ReportObjects["fldSYrToDt"].Left = c + 960;
                            if (det == false)
                            {
                                for (var j = 1; j <= 12; j++)
                                {
                                    report.ReportDefinition.ReportObjects["fldSuM" + j].ObjectFormat.EnableSuppress = true;
                                }
                                report.ReportDefinition.ReportObjects["fldSuYrToDt"].ObjectFormat.EnableSuppress = true;
                            }
                            report.SummaryInfo.ReportTitle = Session["reportTitle"].ToString();
                            report.SummaryInfo.ReportComments = Session["reportSubtitle"].ToString();
                        }
                        break;
                    case "rptGeneralLedger.rpt":
                        {
                            var products = (List<ReportRpt>)Session["Report"];
                            report.Load(reportPath);
                            report.SetDataSource(products);
                            report.SummaryInfo.ReportTitle = Session["ReportTitle"].ToString();
                            report.SummaryInfo.ReportComments = Session["ReportSubtitle"].ToString();
                        }
                        break;
                    case "rptGeneralLedger2.rpt":
                        {
                            var products = (List<ReportRpt>)Session["Report"];
                            report.Load(reportPath);
                            report.SetDataSource(products);
                            report.SummaryInfo.ReportTitle = Session["ReportTitle"].ToString();
                            report.SummaryInfo.ReportComments = Session["ReportSubtitle"].ToString();
                        }
                        break;
                    case "rptFixedAssets.rpt":
                        {
                            var products = (List<ReportRpt>)Session["Report"];
                            report.Load(reportPath);
                            report.SetDataSource(products);
                            report.SummaryInfo.ReportTitle = Session["ReportTitle"].ToString();
                            report.SummaryInfo.ReportComments = Session["ReportSubtitle"].ToString();
                        }
                        break;
                    case "rptBalanceSheet1.rpt":
                    case "rptOtherBalanceSheet.rpt":
                        {
                            var products = (List<ReportRpt>)Session["Report"];
                            report.Load(reportPath);
                            report.SetDataSource(products);
                            report.SummaryInfo.ReportTitle = Session["ReportTitle"].ToString();
                            report.SummaryInfo.ReportComments = Session["ReportSubtitle"].ToString();
                            Session["info"] = 0;
                            var det = Convert.ToInt32(Session["info"]) == 0;

                            report.SetParameterValue("isSummary", det);
                            var diff = Convert.ToInt32(Session["diff"]);
                            var year = Session["year"].ToString().Split(',');
                            var month = Session["month"].ToString().Split(',');
                            for (var j = 1; j <= 12; j++)
                            {
                                report.SetParameterValue("tM" + j, "0");
                                report.ReportDefinition.ReportObjects["txtM" + j].Width = 1050;
                                report.ReportDefinition.ReportObjects["fldSuM" + j].Width = 1050;
                                report.ReportDefinition.ReportObjects["fldM" + j].Width = 1050;
                                report.ReportDefinition.ReportObjects["fldSM" + j].Width = 1050;
                                report.ReportDefinition.ReportObjects["fldGSM" + j].Width = 1050;
                                report.ReportDefinition.ReportObjects["txtNet" + j].Width = 1050;
                            }
                            var c = 3380;
                            report.ReportDefinition.ReportObjects["fldSuM" + Convert.ToInt32(month[0])].Left = c;
                            report.ReportDefinition.ReportObjects["txtM" + Convert.ToInt32(month[0])].Left = c;
                            report.ReportDefinition.ReportObjects["fldGSM" + Convert.ToInt32(month[0])].Left = c;
                            report.ReportDefinition.ReportObjects["fldM" + Convert.ToInt32(month[0])].Left = c;
                            report.ReportDefinition.ReportObjects["fldSM" + Convert.ToInt32(month[0])].Left = c;
                            report.ReportDefinition.ReportObjects["txtNet" + Convert.ToInt32(month[0])].Left = c;
                            if (Convert.ToInt32(month[0]) < Convert.ToInt32(month[1]))
                            {
                                report.SetParameterValue("tM" + Convert.ToInt32(month[0]),
                                    new DateTime(Convert.ToInt32(year[0]), Convert.ToInt32(month[0]), 1).ToString("MMM-yy"));
                                for (var i = Convert.ToInt32(month[0]) + 1; i <= Convert.ToInt32(month[1]); i++)
                                {
                                    c += 1050;
                                    report.ReportDefinition.ReportObjects["fldSuM" + i].Left =
                                        report.ReportDefinition.ReportObjects["fldGSM" + i].Left =
                                            report.ReportDefinition.ReportObjects["fldM" + i].Left =
                                                report.ReportDefinition.ReportObjects["fldSM" + i].Left =
                                                    report.ReportDefinition.ReportObjects["txtNet" + i].Left = c;
                                    report.ReportDefinition.ReportObjects["txtM" + i].Left = c;
                                    report.SetParameterValue("tM" + i, new DateTime(Convert.ToInt32(year[0]), i, 1).ToString("MMM-yy"));
                                }
                            }
                            else
                            {
                                report.SetParameterValue("tM" + Convert.ToInt32(month[0]),
                                    new DateTime(Convert.ToInt32(year[0]), Convert.ToInt32(month[0]), 1).ToString("MMM-yy"));
                                var z = Convert.ToInt32(month[0]);
                                var zx = Convert.ToInt32(year[0]);
                                for (var i = 1; i <= diff - 1; i++)
                                {
                                    c += 1050;
                                    z++;
                                    if (z > 12)
                                    {
                                        z = 1;
                                        zx = zx + 1;
                                    }
                                    report.ReportDefinition.ReportObjects["fldSuM" + z].Left =
                                        report.ReportDefinition.ReportObjects["fldM" + z].Left =
                                            report.ReportDefinition.ReportObjects["fldGSM" + z].Left =
                                                report.ReportDefinition.ReportObjects["fldSM" + z].Left =
                                                    report.ReportDefinition.ReportObjects["txtNet" + z].Left = c;
                                    report.ReportDefinition.ReportObjects["txtM" + z].Left = c;
                                    report.SetParameterValue("tM" + z, new DateTime(zx, z, 1).ToString("MMM-yy"));
                                }
                            }
                            report.ReportDefinition.ReportObjects["Text3"].Width = c + 1050;
                            report.ReportDefinition.ReportObjects["Text4"].Left =
                                report.ReportDefinition.ReportObjects["Text5"].Left = 3380;
                            report.ReportDefinition.ReportObjects["Text4"].Width =
                                report.ReportDefinition.ReportObjects["Text5"].Width = c + 1050 - 3380;
                            if (c - 1050 <= 2205)
                                report.ReportDefinition.ReportObjects["txtText"].Left = c + 500;
                            else
                                report.ReportDefinition.ReportObjects["txtText"].Left = c - 1050;
                            var num = Convert.ToInt32(Session["num"]);
                            var cash = num == 5;
                            report.SetParameterValue("num", cash.ToString());
                            if (det == false)
                            {
                                for (var j = 1; j <= 12; j++)
                                {
                                    report.ReportDefinition.ReportObjects["fldSuM" + j].ObjectFormat.EnableSuppress = true;
                                }
                            }
                            for (var a = 1; a <= 12; a++)
                            {
                                report.ReportDefinition.ReportObjects["fldSuM" + a].ObjectFormat.EnableSuppress = true;
                            }
                            report.PrintOptions.PaperOrientation = PaperOrientation.Landscape;
                            report.PrintOptions.PaperSize = PaperSize.PaperA4;
                            report.SummaryInfo.ReportTitle = Session["reportTitle"].ToString();
                            report.SummaryInfo.ReportComments = Session["reportSubtitle"].ToString();
                        }
                        break;
                }
                var exportOption = ExportFormatType.PortableDocFormat;
                switch (type)
                {
                    case "Excel":
                        exportOption = ExportFormatType.Excel;
                        break;
                    case "ExcelData":
                        exportOption = ExportFormatType.ExcelRecord;
                        break;
                    case "ExcelBook":
                        exportOption = ExportFormatType.ExcelWorkbook;
                        break;
                    case "Word":
                        exportOption = ExportFormatType.WordForWindows;
                        break;
                    case "XML":
                        exportOption = ExportFormatType.Xml;
                        break;
                }
                report.ExportToHttpResponse(exportOption, System.Web.HttpContext.Current.Response, false, "Bdjobs_Accounting");
            }
            report.Close();
            report.Dispose();
        }
    }
}