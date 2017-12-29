using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AccountingSystem.Models;
using AccountingSystem.Models.ViewModel;

namespace AccountingSystem.Controllers
{
    public class InvoiceController : Controller
    {
        private AccountingDbGateway accountingDb = new AccountingDbGateway();

        public bool RenewSession()
        {
            if (ControllerContext.HttpContext.Request.Cookies["userid"] != null)
            {
                var userid = ControllerContext.HttpContext.Request.Cookies["userid"].Value;
                if (Session["loggedinUser"] == null)
                {
                    var user = accountingDb.GetAllUsers().FirstOrDefault(x => x.UserId.ToString() == userid && x.ValidUser);
                    Session["loggedinUser"] = user;
                }
                return true;
            }
            return false;
        }
        public ActionResult OutstandingInvoice()
        {
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapsetwo,5";
            ViewBag.CompanyList = new SelectList(accountingDb.GetCompanyList().OrderBy(x => x.Name), "Id", "Name");
            return View();
        }

        public JsonResult GetScheduledInvoicesByCompanyId(int companyId, char showAs, string date)
        {
            return Json(accountingDb.GetScheduledInvoicesByCompanyId(companyId, showAs, date),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteSchedule(int invoiceId)
        {
            accountingDb.DeleteSchedule(invoiceId);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListOfInvoices()
        {
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapsetwo,7";
            ViewBag.Products = new SelectList(accountingDb.GetProducts(0, 0, "Revenue", "All", "I", 0), "Id", "GroupName");
            return View();
        }

        public JsonResult GetInvoiceList(int pageNo, int pageSize, int productId, int validity, string Operator,
            int fDuration, int tDuration, int fullPayment, int blacklisted, string order)
        {
            return
                Json(
                    accountingDb.GetInvoices(pageNo, pageSize, productId, validity, Operator, fDuration, tDuration,
                        fullPayment, blacklisted, order), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteUndeleteInvoice(int invoiceId, bool invalid)
        {
            accountingDb.DeleteUndeleteInvoice(invoiceId, invalid);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InvoiceListReport(string productId, string validity, string Operator, string fDuration, string tDuration, string fullPayment, string blacklisted, string order, string company, string criteria)
        {
            var query = "USP_LIST_OF_INVOICE_RPT 1,1000,'" + productId + "','" + validity + "','" + Operator + "','" + fDuration + "','" + tDuration + "','" + fullPayment + "','" + blacklisted + "','" + order + "'";
            Session["reportName"] = "rptInvoiceList.rpt";
            Session["Report"] = accountingDb.GetInvoiceListReport(query);
            Session["company"] = company;
            Session["criteria"] = criteria;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InvoiceReport(string invoiceNo, bool isRequested, bool isColor)
        {
            var query = "select i1.Invoice_No,i1.InvSendDt,c.name as CName,l.sbname,i2.amount,i2.comments,b.name as bname,b.designation,s.RefNo, c.VATRegNo from InvoiceList i1,Company c,Ledger l,InvoiceSceduler i2,ContactPersons b,sales s where i1.id=i2.Invoice_Id and s.tno=i2.tno and s.pcode=l.id and s.cid=c.id and s.BillContactId=b.id and i1.Invoice_No= '" + invoiceNo + "'";
            Session["reportName"] = "rptInvoice.rpt";
            Session["Report"] = accountingDb.GetInvoiceReport(query);
            Session["isRequested"] = isRequested;
            Session["isColor"] = isColor;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateLabel()
        {
            if (Session["loggedinUser"] == null)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapsethree,2";
            ViewBag.Products = new SelectList(accountingDb.GetProducts(0, 0, "Revenue", "All", "I", 0), "Id", "GroupName");
            return View();
        }

        public JsonResult ListOfLabels(string pageNo, string pageSize, string type, string productId, string from, string to)
        {
            var query = "USP_LIST_OF_LABEL '" + pageNo + "','" + pageSize + "','" + type + "','" + productId + "','" + from + "','" + to + "'";
            return Json(accountingDb.GetInvoices(query), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PreviewLabelReport(string type, string list, string fontSize, string companyBold, string contactBold)
        {
            var query = "USP_LIST_OF_LABEL_RPT " + type + ",'" + list + "'";
            Session["reportName"] = "rptLabel.rpt";
            Session["Report"] = accountingDb.GetLabelReport(query);
            Session["fontSize"] = fontSize;
            Session["companyBold"] = companyBold;
            Session["contactBold"] = contactBold;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Remarks()
        {
            if (Session["loggedinUser"] == null)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapsetwo,8";
            ViewBag.CompanyList = new SelectList(accountingDb.GetCompanyList().OrderBy(x => x.Name), "Id", "Name");
            return View();
        }

        public JsonResult GetInvoicesForRemarks(string companyId, string fullPayment)
        {
            var query = "USP_INVOICE_LIST_FOR_REMARKS '" + companyId + "','" + fullPayment + "';";
            return Json(accountingDb.GetInvoicesForRemarks(query), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRemarksForInvoice(string invoiceId)
        {
            var query = "USP_INSERT_UPDATE_DELETE_REMARKS '',0,'','','" + invoiceId + "';";
            return Json(accountingDb.GetRemarksForInvoice(query), JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertUpdateDeleteRemark(string action, string remarkId, string date, string remark, string invoiceId, string userId)
        {
            var query = "USP_INSERT_UPDATE_DELETE_REMARKS '" + action + "','" + remarkId + "','" + date + "','" + remark + "','" + invoiceId + "','" + userId + "';";
            accountingDb.InsertUpdateDeleteRemark(query);
            return Json(accountingDb.GetRemarksForInvoice(invoiceId), JsonRequestBehavior.AllowGet);
        }

        public JsonResult PreviewOutstandingInvoiceReport(string invoiceType, string previewType, string incComId)
        {
            var query = "USP_INVOICE_REMARKS_RPT '" + invoiceType + "','" + previewType + "','" + incComId + "';";
            Session["reportName"] = "rptOutStanding.rpt";
            Session["Report"] = accountingDb.GetOutstandingInvoiceReport(query);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInvoiceReportWithRemark(string invoiceType, string previewType, string incComId)
        {
            var query = "USP_INVOICE_REMARKS_RPT '" + invoiceType + "','" + previewType + "','" + incComId + "';";
            Session["reportName"] = "rptRemarks.rpt";
            Session["Report"] = accountingDb.GetInvoiceReportWithRemark(query);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create(string companyId)
        {
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapsetwo,6";
            ViewBag.CompanyList = accountingDb.GetCompanyList().OrderBy(x => x.Name);
            if (companyId != null)
                ViewBag.CompanyId = companyId;
            return View();
        }

        public JsonResult GetProductsForInvoice(string cId, int type)
        {
            var query = "USP_GET_INVOICE_INVSCHEDULE '" + type + "','" + cId + "';";
            return Json(accountingDb.Getproducts(query, type), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductsDetails(string id)
        {
            var query = "USP_GET_INVOICE_DETAIL " + id + ";";
            return Json(accountingDb.GetProductsDetails(query), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GenerateInvoiceNumber(string cId, string issueDate)
        {
            var query = "USP_SET_INVOICE_NUMBER " + cId + ",'" + issueDate + "';";
            return Json(new { InvoiceNo = accountingDb.GenerateInvoiceNumber(query) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateDeleteComments(string action, string invSchId, string invoiceId, string amount, string comments)
        {
            var query = "USP_UPDATE_INVOICE '" + action + "','" + invSchId + "','" + invoiceId + "','" + amount + "','" + comments + "';";
            accountingDb.UpdateDeleteComments(query);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckOnlineJobs(string tnolist, string cId)
        {
            var query = "USP_CHECK_ONLINE_JOBS '" + cId + "','" + tnolist + "'";
            var jobs = accountingDb.CheckOnlineJobs(query);
            return Json(new { Online = jobs[0], Total = jobs[1] }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckInvoiceNo(string invoiceNo)
        {
            return Json(accountingDb.CheckInvoiceNo(invoiceNo), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveInvoice(string action, string cId, string invoice, string issueDate, string totalprice, string idList)
        {
            var query = "USP_INSERT_INVOICE '" + action + "','" + cId + "','" + invoice + "','" + issueDate + "','" + totalprice + "','" + idList + "','" + 0 + "';";
            accountingDb.SaveInvoice(query);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}