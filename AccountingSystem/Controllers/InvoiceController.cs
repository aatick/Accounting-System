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
        public ActionResult OutstandingInvoice()
        {
            AccountingDbGateway.OpenConnection();
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapsetwo,5";
            ViewBag.CompanyList = new SelectList(AccountingDbGateway.GetCompanyList().OrderBy(x => x.Name), "Id", "Name");
            AccountingDbGateway.CloseConnection();
            return View();
        }

        public JsonResult GetScheduledInvoicesByCompanyId(int companyId, char showAs, string date)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetScheduledInvoicesByCompanyId(companyId, showAs, date);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue,JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteSchedule(int invoiceId)
        {
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.DeleteSchedule(invoiceId);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListOfInvoices()
        {
            AccountingDbGateway.OpenConnection();
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapsetwo,7";
            ViewBag.Products = new SelectList(AccountingDbGateway.GetProducts(0, 0, "Revenue", "All", "I", 0), "Id", "GroupName");
            AccountingDbGateway.CloseConnection();
            return View();
        }

        public JsonResult GetInvoiceList(int pageNo, int pageSize, int productId, int validity, string Operator,
            int fDuration, int tDuration, int fullPayment, int blacklisted, string order, string location)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetInvoices(pageNo, pageSize, productId, validity, Operator, fDuration,tDuration,fullPayment, blacklisted, order, location);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteUndeleteInvoice(int invoiceId, bool invalid)
        {
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.DeleteUndeleteInvoice(invoiceId, invalid);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InvoiceListReport(string productId, string validity, string Operator, string fDuration, string tDuration, string fullPayment, string blacklisted, string order, string company, string criteria, string location)
        {
            AccountingDbGateway.OpenConnection();
            var query = "USP_LIST_OF_INVOICE_RPT 1,1000,'" + productId + "','" + validity + "','" + Operator + "','" + fDuration + "','" + tDuration + "','" + fullPayment + "','" + blacklisted + "','" + order + "','" + location + "'";
            Session["reportName"] = "rptInvoiceList.rpt";
            Session["Report"] = AccountingDbGateway.GetInvoiceListReport(query);
            Session["company"] = company;
            Session["criteria"] = criteria;
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InvoiceReport(string invoiceNo, bool isRequested, bool isColor)
        {
            AccountingDbGateway.OpenConnection();
            var query = "select i1.Invoice_No,i1.InvSendDt,c.name as CName,l.sbname,i2.amount,i2.comments,b.name as bname,b.designation,s.RefNo, c.VATRegNo from InvoiceList i1,Company c,Ledger l,InvoiceSceduler i2,ContactPersons b,sales s where i1.id=i2.Invoice_Id and s.tno=i2.tno and s.pcode=l.id and s.cid=c.id and s.BillContactId=b.id and i1.Invoice_No= '" + invoiceNo + "'";
            Session["reportName"] = "rptInvoice.rpt";
            Session["Report"] = AccountingDbGateway.GetInvoiceReport(query);
            Session["isRequested"] = isRequested;
            Session["isColor"] = isColor;
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateLabel()
        {
            AccountingDbGateway.OpenConnection();
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapsethree,2";
            ViewBag.Products = new SelectList(AccountingDbGateway.GetProducts(0, 0, "Revenue", "All", "I", 0), "Id", "GroupName");
            AccountingDbGateway.CloseConnection();
            return View();
        }

        public JsonResult ListOfLabels(string pageNo, string pageSize, string type, string productId, string from, string to)
        {
            AccountingDbGateway.OpenConnection();
            var query = "USP_LIST_OF_LABEL '" + pageNo + "','" + pageSize + "','" + type + "','" + productId + "','" + from + "','" + to + "'";
            var returnValue = AccountingDbGateway.GetInvoices(query);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PreviewLabelReport(string type, string list, string fontSize, string companyBold, string contactBold)
        {
            AccountingDbGateway.OpenConnection();
            var query = "USP_LIST_OF_LABEL_RPT " + type + ",'" + list + "'";
            Session["reportName"] = "rptLabel.rpt";
            Session["Report"] = AccountingDbGateway.GetLabelReport(query);
            AccountingDbGateway.CloseConnection();
            Session["fontSize"] = fontSize;
            Session["companyBold"] = companyBold;
            Session["contactBold"] = contactBold;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Remarks()
        {
            AccountingDbGateway.OpenConnection();
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapsetwo,8";
            AccountingDbGateway.CloseConnection();
            //ViewBag.CompanyList = new SelectList(AccountingDbGateway.GetCompanyList().OrderBy(x => x.Name), "Id", "Name");
            return View();
        }

        public JsonResult GetInvoicesForRemarks(string companyId, string fullPayment)
        {
            var query = "USP_INVOICE_LIST_FOR_REMARKS '" + companyId + "','" + fullPayment + "';";
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetInvoicesForRemarks(query);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRemarksForInvoice(string invoiceId)
        {
            var query = "USP_INSERT_UPDATE_DELETE_REMARKS '',0,'','','" + invoiceId + "';";
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetRemarksForInvoice(query);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertUpdateDeleteRemark(string action, string remarkId, string date, string remark, string invoiceId, string userId)
        {
            var query = "USP_INSERT_UPDATE_DELETE_REMARKS '" + action + "','" + remarkId + "','" + date + "','" + remark + "','" + invoiceId + "','" + userId + "';";
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.InsertUpdateDeleteRemark(query);
            var returnValue = AccountingDbGateway.GetRemarksForInvoice(invoiceId);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PreviewOutstandingInvoiceReport(string invoiceType, string previewType, string incComId)
        {
            var query = "USP_INVOICE_REMARKS_RPT '" + invoiceType + "','" + previewType + "','" + incComId + "';";
            Session["reportName"] = "rptOutStanding.rpt";
            AccountingDbGateway.OpenConnection();
            Session["Report"] = AccountingDbGateway.GetOutstandingInvoiceReport(query);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInvoiceReportWithRemark(string invoiceType, string previewType, string incComId)
        {
            var query = "USP_INVOICE_REMARKS_RPT '" + invoiceType + "','" + previewType + "','" + incComId + "';";
            Session["reportName"] = "rptRemarks.rpt";
            AccountingDbGateway.OpenConnection();
            Session["Report"] = AccountingDbGateway.GetInvoiceReportWithRemark(query);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create(string companyId)
        {
            AccountingDbGateway.OpenConnection();
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapsetwo,6";
            if (companyId != null)
            {
                var aCompany = AccountingDbGateway.GetCompanyList().FirstOrDefault(x => x.Id.ToString() == companyId);
                ViewBag.CompanyId = companyId;
                ViewBag.CompanyName = aCompany.Name;
                ViewBag.BlackList = aCompany.BlackListed;
            }
            AccountingDbGateway.CloseConnection();
            return View();
        }

        public JsonResult GetProductsForInvoice(string cId, int type)
        {
            var query = "USP_GET_INVOICE_INVSCHEDULE '" + type + "','" + cId + "';";
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.Getproducts(query, type);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductsDetails(string id)
        {
            var query = "USP_GET_INVOICE_DETAIL " + id + ";";
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetProductsDetails(query);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GenerateInvoiceNumber(string cId, string issueDate)
        {
            var query = "USP_SET_INVOICE_NUMBER " + cId + ",'" + issueDate + "';";
            AccountingDbGateway.OpenConnection();
            var invoiceNo = AccountingDbGateway.GenerateInvoiceNumber(query);
            AccountingDbGateway.CloseConnection();
            return Json(new { InvoiceNo = invoiceNo }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateDeleteComments(string action, string invSchId, string invoiceId, string amount, string comments, string sendDate)
        {
            var query = "USP_UPDATE_INVOICE '" + action + "','" + invSchId + "','" + invoiceId + "','" + amount + "','" + comments + "','" + sendDate + "';";
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.UpdateDeleteComments(query);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckOnlineJobs(string tnolist, string cId)
        {
            var query = "USP_CHECK_ONLINE_JOBS '" + cId + "','" + tnolist + "'";
            AccountingDbGateway.OpenConnection();
            var jobs = AccountingDbGateway.CheckOnlineJobs(query);
            AccountingDbGateway.CloseConnection();
            return Json(new { Online = jobs[0], Total = jobs[1] }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckInvoiceNo(string invoiceNo)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.CheckInvoiceNo(invoiceNo);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveInvoice(string action, string cId, string invoice, string issueDate, string totalprice, string idList)
        {
            var query = "USP_INSERT_INVOICE '" + action + "','" + cId + "','" + invoice + "','" + issueDate + "','" + totalprice + "','" + idList + "','" + 0 + "';";
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.SaveInvoice(query);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}