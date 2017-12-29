using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AccountingSystem.Models;

namespace AccountingSystem.Controllers
{
    public class JournalController : Controller
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
        public ActionResult Close()
        {
            AccountingDbGateway.OpenConnection();
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            AccountingDbGateway.CloseConnection();
            ViewBag.AccordionId = "collapseOne,6";
            return View();
        }

        public JsonResult GetJournalClosing()
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetJournalClosing();
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetJournalClosing(string closingDate, string setDate, int userId)
        {
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.SetJournalClosing(closingDate, setDate, userId);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Make(string info,string page)
        {
            AccountingDbGateway.OpenConnection();
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapseOne,3";
            ViewBag.Info = info;
            ViewBag.ClosingDate = AccountingDbGateway.GetClosingDate();
            AccountingDbGateway.CloseConnection();
            ViewBag.Page = page;
            return View();
        }

        public ActionResult Vouchers()
        {
            AccountingDbGateway.OpenConnection();
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapseOne,5";
            ViewBag.VoucherList = new SelectList(AccountingDbGateway.GetVoucherList(DateTime.Now.Year, DateTime.Now.Month), "Id", "InvoiceNo");
            AccountingDbGateway.CloseConnection();
            return View();
        }

        public JsonResult GetVouchers(int year, int month)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetVoucherList(year, month);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetVoucherReport(int voucherId)
        {
            Session["reportName"] = "rptJournalVoucher.rpt";
            AccountingDbGateway.OpenConnection();
            Session["Report"] = AccountingDbGateway.GetVoucherReport(voucherId);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMaxJournalId()
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetMaxJournalId();
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveJournalUpdateLedgerMakeVoucher(List<Journal> journals, List<Ledger> ledgers)
        {
            var isSuccess = true;
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.BeginTransaction();
            try
            {
                AccountingDbGateway.SaveJournals(journals);
                AccountingDbGateway.UpdateLedger(ledgers);
                AccountingDbGateway.MakeJournalVoucher(journals[0].JId, journals[0].JDate.ToShortDateString());
                AccountingDbGateway.CommitTransaction();
            }
            catch(Exception ex)
            {
                AccountingDbGateway.RollBackTransaction();
                isSuccess = false;
            }
            AccountingDbGateway.CloseConnection();
            return Json(isSuccess, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLedgersWithBalance()
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetLedgersWithBalance().OrderBy(x => x.GroupName);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListOfJournals()
        {
            AccountingDbGateway.OpenConnection();
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            AccountingDbGateway.CloseConnection();
            ViewBag.AccordionId = "collapseOne,4";
            return View();
        }

        public JsonResult GetInformation()
        {
            AccountingDbGateway.OpenConnection();
            var user = (User)Session["loggedinUser"];
            var json = Json(
                new
                {
                    Ledgers = AccountingDbGateway.GetAllLedger(user.CanModifyAdmin.ToString(), user.AccountDep.ToString()),
                    Approvers = AccountingDbGateway.GetApprovers(),
                    Users = AccountingDbGateway.GetSpecificusers()
                }, JsonRequestBehavior.AllowGet);
            AccountingDbGateway.CloseConnection();
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        public JsonResult GetJournals(string pageNo, string pageSize, string isPreview, string dateType,
            string startDate, string endDate, string ledgerId, string ledgerName, string companyId, string approvedBy, string postedBy,
            string isApproved)
        {
            AccountingDbGateway.OpenConnection();
            var journals = AccountingDbGateway.GetJournals(pageNo, pageSize, isPreview, dateType, startDate, endDate, ledgerId,
                ledgerName, companyId, approvedBy, postedBy, isApproved);
            AccountingDbGateway.CloseConnection();
            if (isPreview == "1")
            {
                Session["reportName"] = "rptListOfJournal.rpt";
                Session["Report"] = journals;
                
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            var jsonData = Json(journals, JsonRequestBehavior.AllowGet);
            jsonData.MaxJsonLength = int.MaxValue;
            return jsonData;
        }

        public JsonResult ApprovedJournals(string userId, string dateType, string startDate, string endDate, string ledgerId, string ledgerName, string companyId, string approvedBy, string postedBy, string approved)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.ApprovedJournals(userId, dateType, startDate, endDate, ledgerId,ledgerName, companyId,approvedBy, postedBy, approved);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSales(string pageNo, string pageSize, string tno)
        {
            var tnoNo = tno == "" ? 0 : Convert.ToInt32(tno);
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetSales(pageNo, pageSize, 0, tnoNo);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateJournal(string id, string sid, string description, string debt, string credit,
            string jdate, string updatedBy, string updatedDate, string notify)
        {
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.UpdateJournal(id, sid, description, debt, credit, jdate, updatedBy, updatedDate, notify);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteJournal(string id, string jid)
        {
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.DeleteJournal(id, jid);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJournalsForTrialBalance(string pageNo, string pageSize, string tno, string fromDate, string endDate)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetJournalsForTrialBalance(pageNo, pageSize, tno, fromDate, endDate);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue,JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPreviewReport(string type, string fromdate, string endDate)
        {
            AccountingDbGateway.OpenConnection();
            var journals = AccountingDbGateway.GetPreviewReport(type, fromdate, endDate);
            AccountingDbGateway.CloseConnection();
            Session["reportName"] = "rptAuditJournal.rpt";
            Session["Report"] = journals;
            Session["ReportTitle"] = "Group: " + type;
            Session["ReportComment"] = "From " + fromdate + " to " + endDate;
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}