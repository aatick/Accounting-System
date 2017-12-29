using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using AccountingSystem.Models;

namespace AccountingSystem.Controllers
{
    public class SaleController : Controller
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
        public ActionResult New(int? onlinejobId, int? onlineLedgerId, int? companyid)
        {
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapsetwo,2";
            ViewBag.CompanyList = accountingDb.GetCompanyList().OrderBy(x => x.Name);
            ViewBag.Products = new SelectList(accountingDb.GetProducts(0, 0, "Revenue", "", "I", 0), "Id", "GroupName");
            ViewBag.Type = new SelectList(accountingDb.GetProducts(0, 0, "", "", "", 1), "Id", "GroupName");
            ViewBag.ClosingDate = accountingDb.GetClosingDate();
            if (onlineLedgerId != null && onlinejobId != null && companyid != null)
            {
                ViewBag.Online = new[] { onlineLedgerId, onlinejobId, companyid };
            }
            return View();
        }

        public ActionResult OnlineJobs()
        {
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapsetwo,9";
            ViewBag.Services = accountingDb.GetServices();
            //ViewBag.CompanyList = accountingDb.GetCompanyList().OrderBy(x => x.Name);
            ViewBag.Connection = accountingDb.GetOnlineConnectionString();
            return View();
        }

        public JsonResult GetLocalCompanyList()
        {
            var json = Json(accountingDb.GetCompanyList().OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        public ActionResult FixedAssets()
        {
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapsetwo,1";
            return View();
        }

        public JsonResult GetOnlineJobList()
        {
            return Json(accountingDb.GetOnlineJobList("USP_ONLINE_JOB_LIST"), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJobs(int cpId, string date)
        {
            return Json(accountingDb.GetJobs(cpId,date), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetInvoices(int cpId, string sDate, int ledgerId)
        {
            return Json(accountingDb.GetInvoices(cpId, sDate, ledgerId), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOnlineLedgerId(string onlineProduct)
        {
            return Json(accountingDb.GetOnlineLedgerId(onlineProduct), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteOnlineJob(int jpId)
        {
            accountingDb.DeleteOnlineJob(jpId);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsAllUploaded()
        {
            return Json(accountingDb.IsAllUploaded(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckOnlineConnection()
        {
            return Json(accountingDb.CheckOnlineConnection(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteTmpJobs()
        {
            accountingDb.DeleteTmpJobs();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DownloadJobs(string fromDate, string toDate)
        {
            return Json(accountingDb.DownloadJobs(fromDate, toDate), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetContactOrJob(string type, int cId)
        {
            return Json(new { ContactPersons = accountingDb.GetContactPersonsOrJobTitle("C", cId), JobTitles = accountingDb.GetContactPersonsOrJobTitle("T", cId) }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckJobTitle(int productId)
        {
            return Json(accountingDb.CheckJobTitle(productId), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetClosingDate()
        {
            return Json(accountingDb.GetClosingDate(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Save(int userId, int cId, int pCode, string fromdate, string toDate, string journalDate,
            double salesPrice, string billingPerson, string designation, string comment, int duration, int noOfInvoice,
            string refNo, int typeId, double vat, int jpId, string jobTitle, string workshopDate)
        {
            accountingDb.SaveSale(userId, cId, pCode, fromdate, toDate, journalDate, salesPrice,
                billingPerson.Replace("'", "`"), designation.Replace("'", "`"), comment.Replace("'", "`"), duration,
                noOfInvoice, refNo, typeId, vat, jpId, jobTitle.Replace("'", "`"), workshopDate);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFixedAssetInitialData()
        {
            return Json(new
            {
                AssetCodes = accountingDb.GetAssetCodes(),
                BankList = accountingDb.GetAssetBankList(),
                AssetItems = accountingDb.GetFixedAssetItem()
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGeneratedAssetCode(int id, string assettype)
        {
            return Json(accountingDb.GetGeneratedAssetCode(id, assettype), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAssetInfo(string assetCode)
        {
            return Json(accountingDb.GetAssetInfo(assetCode), JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertUpdateAsset(string action, int userId, int noDep, string assetCode, string assetNo,
            string assetType, string purchasedDate, double price, string depStartDate, string supplier, string invoiceNo,
            string labelNo, string description, double depRate, int depLife, string depEndDate)
        {
            accountingDb.InsertUpdateAsset(action, userId, noDep, assetCode, assetNo, assetType, purchasedDate, price,
                depStartDate, supplier, invoiceNo, labelNo, description, depRate, depLife, depEndDate);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckAssetNumber(string assetCode)
        {
            return Json(accountingDb.CheckAssetNumber(assetCode), JsonRequestBehavior.AllowGet);
        }

        public JsonResult MakeAssetJournal(string userId, string noDep, string assetCode, string assetNo, string purchasedId,
            double price, string description)
        {
            accountingDb.MakeJournal(userId, noDep, assetCode, assetNo, purchasedId, price, description);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertDisposalAsset(string userId, string assetNoId, string disposalDate, double amount,
            int sold, string soldId, string description)
        {
            accountingDb.InsertDisposalAsset(userId, assetNoId, disposalDate, amount, sold, soldId, description);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteAssetJournal(string assetCode)
        {
            accountingDb.DeleteAssetJournal(assetCode);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOnlineCompanyList(int radio)
        {
            return Json(accountingDb.GetOnlineCompanyList(radio), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOnlineCompanyInfo(int cpId, string connectionString)
        {
            return Json(accountingDb.GetOnlineCompanyInfo(cpId, connectionString), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLocalCompanyInfo(int cpId)
        {
            return Json(accountingDb.GetCompanyById(cpId), JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertUpdateOnlineCompany(string action, string cpId, string name, string address, string city, string phone, string email, string cPerson, string designation, string companyId)
        {
            accountingDb.InsertUpdateOnlineCompany(action, cpId, name, address, city, phone, email, cPerson, designation,
                companyId);
            accountingDb.UpdateProfile(action, name, cpId, companyId);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckCompany(string name)
        {
            var companies = new List<Company>();
            var company = accountingDb.GetCompanyByName(name);
            if (company != null)
                companies.Add(company);
            return Json(companies, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckOnlineCompany(int id)
        {
            var company = accountingDb.GetCompanyById(id);
            return Json(company, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CashCollection()
        {
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapsetwo,4";
            ViewBag.CompanyList = accountingDb.GetCompanyList();
            ViewBag.Ledgers = accountingDb.GetAllLedger().Where(x => x.Under == "1,12,410").OrderBy(x => x.GroupName).ToList();
            ViewBag.Total = accountingDb.GetTotalOnlinePost();
            ViewBag.ClosingDate = accountingDb.GetClosingDate();
            return View();
        }

        public JsonResult GetInvoicesForCashCollection(string query)
        {
            return Json(accountingDb.GetInvoicesForCashCollection(query), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSalesInfo(string invoiceNo)
        {
            return Json(accountingDb.GetSalesInfo(invoiceNo), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCashCollection(string id)
        {
            return Json(accountingDb.GetCashCollection(id), JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertCashCollection(string type, string userId, string invoiceNo, string cash, string date, string tno, string invoiceShedulerId, string ledgerId, string chequeDetails, string companyName)
        {
            accountingDb.InsertCashCollection(type, userId, invoiceNo, cash, date, tno, invoiceShedulerId, ledgerId,
                chequeDetails, companyName);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAccountReceivable(string tno)
        {
            return Json(accountingDb.GetAccountReceivable(tno), JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateCashCollection(string type, string userId, string invoiceNo, string cash, string date, string tno, string invoiceShedulerId, string ledgerId, string chequeDetails, string companyName, string cashCollectionId)
        {
            accountingDb.UpdateCashCollection(type, userId, invoiceNo, cash, date, tno, invoiceShedulerId, ledgerId,
                chequeDetails, companyName, cashCollectionId);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UnpaidCashCollection(string userId, string ledgerId, string tno, string invoiceId, string invoiceNo, string collectionid, string amount, string companyName)
        {
            accountingDb.UnpaidCashCollection(userId, ledgerId, tno, invoiceId, invoiceNo, collectionid, amount,
                companyName);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit()
        {
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapsetwo,3";
            ViewBag.CompanyList = accountingDb.GetCompanyList();
            ViewBag.ClosingDate = accountingDb.GetClosingDate();
            ViewBag.Type = accountingDb.GetProducts(0, 0, "", "", "", 1);
            return View();
        }
        public JsonResult GetSales(string pageNo, string pageSize, int cId)
        {
            return Json(accountingDb.GetSales(pageNo, pageSize, cId, 0), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteSale(string tno)
        {
            return Json(accountingDb.DeleteSale(tno), JsonRequestBehavior.AllowGet);
        }

        public JsonResult MakeJournalOfSale(string sId, string amount, string jDate, string duration, string tno, string description, string salesdate, string taxId, string tax, string userId)
        {
            accountingDb.MakeJournalOfSale(sId, amount, jDate, duration, tno, description, salesdate, taxId, tax, userId);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNumberOfId(string tno)
        {
            return Json(accountingDb.GetNumberOfId(tno), JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateSaleInfo(string salesPrice, string accReceivable, string duration, string tax, string sDate, string eDate, string tno, string contactId, string refNo)
        {
            accountingDb.UpdateSaleInfo(salesPrice, accReceivable, duration, tax, sDate, eDate, tno, contactId, refNo);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateSalesJournal(string sid, string vatId, string tno, string oldDuration,
            string newDuration, string oldAmount, string newAmount, string oldVatAmount, string newVatAmount,
            string fromDate, string description, string userId)
        {
            accountingDb.UpdateSalesJournal(sid, vatId, tno, oldDuration, newDuration, oldAmount, newAmount,
                oldVatAmount, newVatAmount, fromDate, description, userId);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateSalePosted(string sid, string newAmount, string fromdate, string newDuration, string tno, string description, string dateFrom, string vatId, string newVatAmount, string userId)
        {
            accountingDb.UpdateSalePosted(sid, newAmount, fromdate, newDuration, tno, description, dateFrom, vatId,
                newVatAmount, userId);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateProduct(string oldSid, string tno, string newSid)
        {
            accountingDb.UpdateSaleProduct(oldSid, tno, newSid);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateSaleContactPersonAndRefNo(string personId, string refNo, string tno)
        {
            accountingDb.UpdateSaleContactPersonAndRefNo(personId, refNo, tno);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMoneyReceipt(string invoices, string str)
        {
            var receipts = accountingDb.GetMoneyReceipt(invoices);
            Session["reportName"] = "rptRecceipt.rpt";
            Session["Report"] = receipts;
            Session["str"] = str;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

    }
}