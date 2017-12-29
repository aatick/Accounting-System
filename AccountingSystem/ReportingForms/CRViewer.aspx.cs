using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AccountingSystem.Models;
using AccountingSystem.Models.ViewModel;
using AccountingSystem.Reports;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace AccountingSystem.ReportingForms
{
    public partial class CRInvoice : System.Web.UI.Page
    {
        ReportDocument report = new ReportDocument();
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void CRReportViewer_Init(object sender, EventArgs e)
        {

        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["reportName"] != null)
            {
                var reportName = Session["reportName"].ToString();
                if (reportName == "rptInvoiceList.rpt")
                {
                    var invoiceList = (List<InvoiceListReport>)Session["Report"];
                    string reportPath = Server.MapPath("~/Reports/" + reportName);
                    report.Load(reportPath);
                    //invoiceReport.SetDatabaseLogon("atik", "atik", "ROUTER", "Accounting_Web");
                    report.SetDataSource(invoiceList);
                    //invoiceReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, true, Guid.NewGuid().ToString());
                    //invoiceReport.ExportToHttpResponse(ExportFormatType.PortableDocFormat, HttpContext.Current.Response,
                    //    false, "Redeemed");
                    CRReportViewer.ReportSource = report;
                    CRReportViewer.DisplayGroupTree = false;

                    var paramFields = new ParameterFields();

                    var companyField = new ParameterField();
                    companyField.ParameterFieldName = "company"; //company is Crystal Report Parameter name.
                    var companyValue = new ParameterDiscreteValue();
                    companyValue.Value = Session["company"];
                    companyField.CurrentValues.Add(companyValue);
                    paramFields.Add(companyField);

                    var criteriaField = new ParameterField();
                    criteriaField.ParameterFieldName = "criteria"; //criteria is Crystal Report Parameter name.
                    var criteriaValue = new ParameterDiscreteValue();
                    criteriaValue.Value = Session["criteria"];
                    criteriaField.CurrentValues.Add(criteriaValue);
                    paramFields.Add(criteriaField);

                    CRReportViewer.ParameterFieldInfo = paramFields;

                }
                else if (reportName == "rptInvoice.rpt")
                {
                    var invoice = (List<InvoiceReport>)Session["Report"];
                    string reportPath = Server.MapPath("~/Reports/" + reportName);
                    report.Load(reportPath);
                    report.SetDataSource(invoice);
                    var isColor = (bool)Session["isColor"];
                    if (isColor)
                    {
                        report.ReportDefinition.ReportObjects["Line6"].ObjectFormat.EnableSuppress = true;
                        report.ReportDefinition.ReportObjects["Line5"].ObjectFormat.EnableSuppress = true;
                    }
                    report.SetParameterValue("isRequested", (bool)Session["isRequested"]);
                    report.SetParameterValue("isColor", isColor);
                    CRReportViewer.ReportSource = report;
                    CRReportViewer.DisplayGroupTree = false;

                }
                else if (reportName == "rptLabel.rpt")
                {

                    var labels = (List<LabelReport>)Session["Report"];
                    string reportPath = Server.MapPath("~/Reports/" + reportName);
                    report.Load(reportPath);
                    report.SetDataSource(labels);

                    var paramFields = new ParameterFields();

                    var companyField = new ParameterField();
                    companyField.ParameterFieldName = "companyBold"; //company is Crystal Report Parameter name.
                    var companyValue = new ParameterDiscreteValue();
                    companyValue.Value = Session["companyBold"];
                    companyField.CurrentValues.Add(companyValue);
                    paramFields.Add(companyField);

                    var contactField = new ParameterField();
                    contactField.ParameterFieldName = "contactBold"; //criteria is Crystal Report Parameter name.
                    var contactValue = new ParameterDiscreteValue();
                    contactValue.Value = Session["contactBold"];
                    contactField.CurrentValues.Add(contactValue);
                    paramFields.Add(contactField);

                    var criteriaField = new ParameterField();
                    criteriaField.ParameterFieldName = "fontSize"; //criteria is Crystal Report Parameter name.
                    var criteriaValue = new ParameterDiscreteValue();
                    criteriaValue.Value = Session["fontSize"];
                    criteriaField.CurrentValues.Add(criteriaValue);
                    paramFields.Add(criteriaField);
                    CRReportViewer.ReportSource = report;
                    CRReportViewer.DisplayGroupTree = false;
                    CRReportViewer.ParameterFieldInfo = paramFields;
                }
                else if (reportName == "rptOutStanding.rpt")
                {
                    var invoice = (List<OutstandingInvoiceReport>)Session["Report"];
                    string reportPath = Server.MapPath("~/Reports/" + reportName);
                    report.Load(reportPath);
                    report.SetDataSource(invoice);
                    CRReportViewer.ReportSource = report;
                    CRReportViewer.DisplayGroupTree = false;
                }
                else if (reportName == "rptRemarks.rpt")
                {
                    var invoice = (List<InvoiceRemarkReport>)Session["Report"];
                    string reportPath = Server.MapPath("~/Reports/" + reportName);
                    report.Load(reportPath);
                    report.SetDataSource(invoice);
                    CRReportViewer.ReportSource = report;
                    CRReportViewer.DisplayGroupTree = false;
                }
                else if (reportName == "rptJournalVoucher.rpt")
                {
                    var vouchers = (List<JournalVoucherReport>)Session["Report"];
                    string reportPath = Server.MapPath("~/Reports/" + reportName);
                    report.Load(reportPath);
                    report.SetDataSource(vouchers);
                    CRReportViewer.ReportSource = report;
                    CRReportViewer.DisplayGroupTree = false;
                }
                else if (reportName == "rptListOfJournal.rpt")
                {
                    var journals = (List<JournalForReport>)Session["Report"];
                    string reportPath = Server.MapPath("~/Reports/" + reportName);
                    report.Load(reportPath);
                    report.SetDataSource(journals);
                    CRReportViewer.ReportSource = report;
                    CRReportViewer.DisplayGroupTree = false;
                }
                else if (reportName == "rptAuditJournal.rpt")
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
                    string reportPath = Server.MapPath("~/Reports/" + reportName);
                    report.Load(reportPath);
                    report.SetDataSource(journals);
                    report.SummaryInfo.ReportTitle = Session["ReportTitle"].ToString();
                    report.SummaryInfo.ReportComments = Session["ReportComment"].ToString();
                    CRReportViewer.ReportSource = report;
                    CRReportViewer.DisplayGroupTree = false;
                }
                else if (reportName == "rptRecceipt.rpt")
                {
                    var receipts = (List<MoneyReceipt>)Session["Report"];
                    string str = "";
                    string str1 = "";
                    var list = new List<string>();
                    foreach (MoneyReceipt moneyReceipt in receipts)
                    {
                        str += "\n" + moneyReceipt.Invoice_No;
                        list.Add(moneyReceipt.sbname);

                    }
                    foreach (string st in list.Distinct().ToList())
                    {
                        str1 += "\n" + st;
                    }
                    string reportPath = Server.MapPath("~/Reports/" + reportName);
                    report.Load(reportPath);
                    report.SetDataSource(receipts);
                    report.SetParameterValue("InvoiceNos", str.Substring(1));
                    report.SetParameterValue("SbNames", str1.Substring(1));
                    report.SetParameterValue("Name", receipts[0].Name + " " + "Tk. " + receipts[0].TAmount + " " + Session["str"]);
                    CRReportViewer.ReportSource = report;
                    CRReportViewer.DisplayGroupTree = false;
                }
                else if (reportName == "rptTrialBalance.rpt")
                {
                    var ledgers = (List<Ledger>)Session["Report"];
                    string reportPath = Server.MapPath("~/Reports/" + reportName);
                    report.Load(reportPath);
                    report.SetDataSource(ledgers);
                    report.SummaryInfo.ReportTitle = Session["ReportTitle"].ToString();
                    CRReportViewer.ReportSource = report;
                    CRReportViewer.DisplayGroupTree = false;
                }
                else if (reportName == "rptAccountsReceivable.rpt")
                {
                    var products = (List<ProductForInvoice>)Session["Report"];
                    string reportPath = Server.MapPath("~/Reports/" + reportName);
                    report.Load(reportPath);
                    report.SetDataSource(products);
                    report.SummaryInfo.ReportTitle = Session["ReportTitle"].ToString();
                    CRReportViewer.ReportSource = report;
                    CRReportViewer.DisplayGroupTree = false;
                }
                else if (reportName == "rptRevenue.rpt")
                {
                    var products = (List<ReportRpt>)Session["Report"];
                    string reportPath = Server.MapPath("~/Reports/" + reportName);
                    report.Load(reportPath);
                    report.SetDataSource(products);
                    var det = Convert.ToInt32(Session["info"]) == 0;

                    report.SetParameterValue("isSummary", det);
                    var diff = Convert.ToInt32(Session["diff"]);
                    var year = Session["year"].ToString().Split(',');
                    var month = Session["month"].ToString().Split(',');
                    for (var j = 1; j <= 12; j++)
                    {
                        report.SetParameterValue("tM" + j, "0");
                    }
                    var c = 3380;
                    report.ReportDefinition.ReportObjects["fldSuM" + Convert.ToInt32(month[0])].Left = c;
                    report.ReportDefinition.ReportObjects["txtM" + Convert.ToInt32(month[0])].Left = c;
                    report.ReportDefinition.ReportObjects["fldGM" + Convert.ToInt32(month[0])].Left = c;
                    report.ReportDefinition.ReportObjects["fldM" + Convert.ToInt32(month[0])].Left = c;
                    report.ReportDefinition.ReportObjects["fldSM" + Convert.ToInt32(month[0])].Left = c;
                    if (Convert.ToInt32(month[0]) < Convert.ToInt32(month[1]))
                    {
                        report.SetParameterValue("tM" + Convert.ToInt32(month[0]),
                            new DateTime(Convert.ToInt32(year[0]), Convert.ToInt32(month[0]), 1).ToString("MMM-yy"));
                        for (var i = Convert.ToInt32(month[0]) + 1; i <= Convert.ToInt32(month[1]); i++)
                        {
                            c += 822;
                            report.ReportDefinition.ReportObjects["fldSuM" + i].Left =
                                report.ReportDefinition.ReportObjects["fldGM" + i].Left =
                                    report.ReportDefinition.ReportObjects["fldM" + i].Left =
                                        report.ReportDefinition.ReportObjects["fldSM" + i].Left = c;
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
                            c += 822;
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
                        }
                    }
                    report.ReportDefinition.ReportObjects["Text3"].Width =
                        report.ReportDefinition.ReportObjects["Text4"].Width = c + 822;
                    report.ReportDefinition.ReportObjects["txtText"].Left = c - 822;
                    report.ReportDefinition.ReportObjects["fldSuYrToDt"].ObjectFormat.EnableSuppress =
                        report.ReportDefinition.ReportObjects["txtYrToDt"].ObjectFormat.EnableSuppress =
                            report.ReportDefinition.ReportObjects["fldGYrToDt"].ObjectFormat.EnableSuppress =
                                report.ReportDefinition.ReportObjects["fldYrToDt"].ObjectFormat.EnableSuppress =
                                    report.ReportDefinition.ReportObjects["fldSYrToDt"].ObjectFormat.EnableSuppress =
                                        true;
                    var num = Convert.ToInt32(Session["num"]);
                    var cash = num == 5;
                    report.SetParameterValue("isCash", cash);
                    if (num == 2 || num == 3)
                    {
                        report.ReportDefinition.ReportObjects["Text3"].Width =
                            report.ReportDefinition.ReportObjects["Text4"].Width = (c + 822 + 1020);

                        report.ReportDefinition.ReportObjects["fldSuYrToDt"].Left =
                            report.ReportDefinition.ReportObjects["txtYrToDt"].Left =
                                report.ReportDefinition.ReportObjects["fldGYrToDt"].Left =
                                    report.ReportDefinition.ReportObjects["fldYrToDt"].Left =
                                        report.ReportDefinition.ReportObjects["fldSYrToDt"].Left = c + 822;

                        report.ReportDefinition.ReportObjects["fldSuYrToDt"].ObjectFormat.EnableSuppress =
                            report.ReportDefinition.ReportObjects["txtYrToDt"].ObjectFormat.EnableSuppress =
                                report.ReportDefinition.ReportObjects["fldGYrToDt"].ObjectFormat.EnableSuppress =
                                    report.ReportDefinition.ReportObjects["fldYrToDt"].ObjectFormat.EnableSuppress =
                                        report.ReportDefinition.ReportObjects["fldSYrToDt"].ObjectFormat.EnableSuppress
                                            = false;
                        report.ReportDefinition.ReportObjects["txtText"].Left = (c - 822 + 1020);
                    }
                    if (det == false)
                    {
                        for (var j = 1; j <= 12; j++)
                        {
                            report.ReportDefinition.ReportObjects["fldSuM" + j].ObjectFormat.EnableSuppress = true;
                        }
                        CRReportViewer.DisplayGroupTree = true;
                        report.ReportDefinition.ReportObjects["fldSuYrToDt"].ObjectFormat.EnableSuppress = true;
                    }
                    else
                        CRReportViewer.DisplayGroupTree = false;
                    report.SummaryInfo.ReportTitle = Session["reportTitle"].ToString();
                    report.SummaryInfo.ReportComments = Session["reportSubtitle"].ToString();
                    CRReportViewer.ReportSource = report;

                }
                else if (reportName == "rptProfitLoss1.rpt")
                {
                    var field = new[] { "15", "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38" };
                    var list = new List<int>();
                    var products = (List<ReportRpt>)Session["Report"];
                    string reportPath = Server.MapPath("~/Reports/" + reportName);
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
                        CRReportViewer.DisplayGroupTree = true;
                        report.ReportDefinition.ReportObjects["fldSuYrToDt"].ObjectFormat.EnableSuppress = true;
                    }
                    else
                        CRReportViewer.DisplayGroupTree = false;
                    report.SummaryInfo.ReportTitle = Session["reportTitle"].ToString();
                    report.SummaryInfo.ReportComments = Session["reportSubtitle"].ToString();
                    CRReportViewer.ReportSource = report;
                }
                else if (reportName == "rptGeneralLedger.rpt")
                {
                    var products = (List<ReportRpt>)Session["Report"];
                    string reportPath = Server.MapPath("~/Reports/" + reportName);
                    report.Load(reportPath);
                    report.SetDataSource(products);
                    report.SummaryInfo.ReportTitle = Session["ReportTitle"].ToString();
                    report.SummaryInfo.ReportComments = Session["ReportSubtitle"].ToString();
                    CRReportViewer.ReportSource = report;
                    CRReportViewer.DisplayGroupTree = false;
                }
                else if (reportName == "rptFixedAssets.rpt")
                {
                    var products = (List<ReportRpt>)Session["Report"];
                    string reportPath = Server.MapPath("~/Reports/" + reportName);
                    report.Load(reportPath);
                    report.SetDataSource(products);
                    report.SummaryInfo.ReportTitle = Session["ReportTitle"].ToString();
                    report.SummaryInfo.ReportComments = Session["ReportSubtitle"].ToString();
                    CRReportViewer.ReportSource = report;
                    CRReportViewer.DisplayGroupTree = true;
                }
                else if (reportName == "rptBalanceSheet1.rpt")
                {
                    var products = (List<ReportRpt>)Session["Report"];
                    string reportPath = Server.MapPath("~/Reports/" + reportName);
                    report.Load(reportPath);
                    report.SetDataSource(products);
                    report.SummaryInfo.ReportTitle = Session["ReportTitle"].ToString();
                    report.SummaryInfo.ReportComments = Session["ReportSubtitle"].ToString();
                    CRReportViewer.ReportSource = report;
                    CRReportViewer.DisplayGroupTree = false;
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
                    }
                    var c = 3380;
                    report.ReportDefinition.ReportObjects["fldSuM" + Convert.ToInt32(month[0])].Left = c;
                    report.ReportDefinition.ReportObjects["txtM" + Convert.ToInt32(month[0])].Left = c;
                    report.ReportDefinition.ReportObjects["fldGSM" + Convert.ToInt32(month[0])].Left = c;
                    report.ReportDefinition.ReportObjects["fldM" + Convert.ToInt32(month[0])].Left = c;
                    report.ReportDefinition.ReportObjects["fldSM" + Convert.ToInt32(month[0])].Left = c;
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
                                        report.ReportDefinition.ReportObjects["fldSM" + i].Left = c;
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
                                        report.ReportDefinition.ReportObjects["fldSM" + z].Left = c;
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
                        CRReportViewer.DisplayGroupTree = true;
                    }
                    else
                        CRReportViewer.DisplayGroupTree = false;
                    for (var a = 1; a <= 12; a++)
                    {
                        report.ReportDefinition.ReportObjects["fldSuM" + a].ObjectFormat.EnableSuppress = true;
                    }
                    report.PrintOptions.PaperOrientation = PaperOrientation.Landscape;
                    report.PrintOptions.PaperSize = PaperSize.PaperA4;
                    report.SummaryInfo.ReportTitle = Session["reportTitle"].ToString();
                    report.SummaryInfo.ReportComments = Session["reportSubtitle"].ToString();
                    CRReportViewer.ReportSource = report;
                }
            }
        }
        protected void Page_Unload(object sender, EventArgs e)
        {
            report.Close();
            report.Dispose();
        }

    }
}