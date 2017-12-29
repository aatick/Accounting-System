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
        public ActionResult Close()
        {
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapseOne,6";
            return View();
        }

        public JsonResult GetJournalClosing()
        {
            return Json(accountingDb.GetJournalClosing(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetJournalClosing(string closingDate, string setDate, int userId)
        {
            accountingDb.SetJournalClosing(closingDate, setDate, userId);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Make(string info)
        {
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapseOne,3";
            ViewBag.Info = info;
            ViewBag.ClosingDate = accountingDb.GetClosingDate();
            return View();
        }

        public ActionResult Vouchers()
        {
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapseOne,5";
            ViewBag.VoucherList = new SelectList(accountingDb.GetVoucherList(DateTime.Now.Year, DateTime.Now.Month), "Id", "InvoiceNo");
            return View();
        }

        public JsonResult GetVouchers(int year, int month)
        {
            return Json(accountingDb.GetVoucherList(year, month), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetVoucherReport(int voucherId)
        {
            Session["reportName"] = "rptJournalVoucher.rpt";
            Session["Report"] = accountingDb.GetVoucherReport(voucherId);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMaxJournalId()
        {
            return Json(accountingDb.GetMaxJournalId(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveJournalUpdateLedgerMakeVoucher(List<Journal> journals, List<Ledger> ledgers)
        {
            accountingDb.SaveJournals(journals);
            accountingDb.UpdateLedger(ledgers);
            accountingDb.MakeJournalVoucher(journals[0].JId, journals[0].JDate.ToShortDateString());
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLedgersWithBalance()
        {
            return Json(accountingDb.GetLedgersWithBalance().OrderBy(x => x.GroupName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListOfJournals()
        {
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapseOne,4";
            return View();
        }

        public JsonResult GetInformation()
        {
            var user = (User)Session["loggedinUser"];
            var json = Json(
                new
                {
                    Ledgers = accountingDb.GetAllLedger(user.CanModifyAdmin.ToString(), user.AccountDep.ToString()),
                    Companies = accountingDb.GetCompanyList(),
                    Approvers = accountingDb.GetApprovers(),
                    Users = accountingDb.GetSpecificusers()
                }, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        public JsonResult GetJournals(string pageNo, string pageSize, string isPreview, string dateType,
            string startDate, string endDate, string ledgerId, string ledgerName, string companyId, string approvedBy, string postedBy,
            string isApproved)
        {
            var journals = accountingDb.GetJournals(pageNo, pageSize, isPreview, dateType, startDate, endDate, ledgerId,
                ledgerName, companyId, approvedBy, postedBy, isApproved);
            if (isPreview == "1")
            {
                Session["reportName"] = "rptListOfJournal.rpt";
                Session["Report"] = journals;
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(journals, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ApprovedJournals(string userId, string dateType, string startDate, string endDate, string ledgerId, string ledgerName, string companyId, string approvedBy, string postedBy, string approved)
        {
            return
                Json(
                    accountingDb.ApprovedJournals(userId, dateType, startDate, endDate, ledgerId, ledgerName, companyId,
                        approvedBy, postedBy, approved), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSales(string pageNo, string pageSize, string tno)
        {
            var tnoNo = tno == "" ? 0 : Convert.ToInt32(tno);
            return Json(accountingDb.GetSales(pageNo, pageSize, 0, tnoNo), JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateJournal(string id, string sid, string description, string debt, string credit,
            string jdate, string updatedBy, string updatedDate, string notify)
        {
            accountingDb.UpdateJournal(id, sid, description, debt, credit, jdate, updatedBy, updatedDate, notify);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteJournal(string id, string jid)
        {
            accountingDb.DeleteJournal(id, jid);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJournalsForTrialBalance(string pageNo, string pageSize, string tno, string fromDate, string endDate)
        {
            return Json(accountingDb.GetJournalsForTrialBalance(pageNo, pageSize, tno, fromDate, endDate),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPreviewReport(string type, string fromdate, string endDate)
        {
            var journals = accountingDb.GetPreviewReport(type, fromdate, endDate);
            Session["reportName"] = "rptAuditJournal.rpt";
            Session["Report"] = journals;
            Session["ReportTitle"] = "Group: " + type;
            Session["ReportComment"] = "From " + fromdate + " to " + endDate;
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}