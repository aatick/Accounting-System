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
        public ActionResult New(int? onlinejobId, int? onlineLedgerId, int? companyid)
        {
            AccountingDbGateway.OpenConnection();
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapsetwo,2";
            ViewBag.Products = new SelectList(AccountingDbGateway.GetProducts(0, 0, "Revenue", "", "I", 0), "Id", "GroupName");
            ViewBag.Type = new SelectList(AccountingDbGateway.GetProducts(0, 0, "", "", "", 1).Where(x => x.Id != 847), "Id", "GroupName",672);
            ViewBag.ClosingDate = AccountingDbGateway.GetClosingDate();
            if (onlineLedgerId != null && onlinejobId != null && companyid != null)
            {
                ViewBag.Online = new[] { onlineLedgerId, onlinejobId, companyid };
                var aCompany = AccountingDbGateway.GetCompanyById(companyid);
                ViewBag.CompanyName = aCompany.Name;
                ViewBag.Blacklist = aCompany.BlackListed;
            }
            AccountingDbGateway.CloseConnection();
            return View();
        }

        public ActionResult OnlineJobs()
        {
            AccountingDbGateway.OpenConnection();
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapsetwo,9";
            ViewBag.Services = AccountingDbGateway.GetServices();
            ViewBag.Districts = AccountingDbGateway.GetDistricts();
            AccountingDbGateway.CloseConnection();
            return View();
        }

        public JsonResult GetLocalCompanyList()
        {
            AccountingDbGateway.OpenConnection();
            var json = Json(AccountingDbGateway.GetCompanyList().OrderBy(x => x.Name), JsonRequestBehavior.AllowGet);
            AccountingDbGateway.CloseConnection();
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        public ActionResult FixedAssets()
        {
            AccountingDbGateway.OpenConnection();
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            AccountingDbGateway.CloseConnection();
            ViewBag.AccordionId = "collapsetwo,1";
            return View();
        }

        public JsonResult GetOnlineJobList()
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetOnlineJobList();
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJobs(int cpId, string date, int adType,int adRegion)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetJobs(cpId, date, adType, adRegion);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetInvoices(int cpId, string sDate, int ledgerId)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetInvoices(cpId, sDate, ledgerId);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOnlineLedgerId(string onlineProduct)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetOnlineLedgerId(onlineProduct);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteOnlineJob(int jpId)
        {
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.DeleteOnlineJob(jpId);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DownLoadOnlineJobs(string fromDate, string toDate)
        {
            object returnValue = 0;
            AccountingDbGateway.OpenConnection();
            var isUploaded=AccountingDbGateway.IsAllUploaded();
            AccountingDbGateway.CloseConnection();
            if (!isUploaded)
            {
                returnValue = 1;
            }
            else
            {
                var isOnlineOk = AccountingDbGateway.CheckOnlineConnection();
                if (!isOnlineOk)
                {
                    returnValue = 2;
                }
                else
                {
                    bool status;
                    returnValue = AccountingDbGateway.DownloadJobs(fromDate, toDate, out status);
                    if (!status)
                    {
                        returnValue = 3;
                    }
                }
            }
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult IsAllUploaded()
        //{
        //    AccountingDbGateway.OpenConnection();
        //    var returnValue = AccountingDbGateway.IsAllUploaded();
        //    AccountingDbGateway.CloseConnection();
        //    return Json(returnValue, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult CheckOnlineConnection()
        //{
        //    var returnValue = AccountingDbGateway.CheckOnlineConnection();
        //    return Json(returnValue, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult DeleteTmpJobs()
        //{
        //    AccountingDbGateway.OpenConnection();
        //    AccountingDbGateway.DeleteTmpJobs();
        //    AccountingDbGateway.CloseConnection();
        //    return Json(true, JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult DownloadJobs(string fromDate, string toDate)
        //{
        //    AccountingDbGateway.OpenConnection();
        //    var returnValue = AccountingDbGateway.DownloadJobs(fromDate, toDate);
        //    AccountingDbGateway.CloseConnection();
        //    return Json(returnValue, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetContactOrJob(string type, int cId)
        {
            AccountingDbGateway.OpenConnection();
            var contact = AccountingDbGateway.GetContactPersonsOrJobTitle("C", cId);
            var jobs = AccountingDbGateway.GetContactPersonsOrJobTitle("T", cId);
            AccountingDbGateway.CloseConnection();
            return Json(new { ContactPersons = contact, JobTitles = jobs }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckJobTitle(int productId)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.CheckJobTitle(productId);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetClosingDate()
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetClosingDate();
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Save(int userId, int cId, int pCode, string fromdate, string toDate, string journalDate,
            double salesPrice, string billingPerson, string designation, string comment, int duration, int noOfInvoice,
            string refNo, string typeId, double vat, int jpId, string jobTitle, string workshopDate)
        {
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.SaveSale(userId, cId, pCode, fromdate, toDate, journalDate, salesPrice,
                billingPerson.Replace("'", "`"), designation.Replace("'", "`"), comment.Replace("'", "`"), duration,
                noOfInvoice, refNo, typeId, vat, jpId, jobTitle.Replace("'", "`"), workshopDate);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFixedAssetInitialData()
        {
            AccountingDbGateway.OpenConnection();
            var json = Json(new
            {
                AssetCodes = AccountingDbGateway.GetAssetCodes(),
                BankList = AccountingDbGateway.GetAssetBankList(),
                AssetItems = AccountingDbGateway.GetFixedAssetItem()
            }, JsonRequestBehavior.AllowGet);
            AccountingDbGateway.CloseConnection();
            return json;
        }

        public JsonResult GetGeneratedAssetCode(int id, string assettype)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetGeneratedAssetCode(id, assettype);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAssetInfo(string assetCode)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetAssetInfo(assetCode);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertUpdateAsset(string action, int userId, int noDep, string assetCode, string assetNo,
            string assetType, string purchasedDate, double price, string depStartDate, string supplier, string invoiceNo,
            string labelNo, string description, double depRate, int depLife, string depEndDate)
        {
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.InsertUpdateAsset(action, userId, noDep, assetCode, assetNo, assetType, purchasedDate, price,
                depStartDate, supplier, invoiceNo, labelNo, description, depRate, depLife, depEndDate);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckAssetNumber(string assetCode)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.CheckAssetNumber(assetCode);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MakeAssetJournal(string userId, string noDep, string assetCode, string assetNo, string purchasedId,
            double price, string description)
        {
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.MakeJournal(userId, noDep, assetCode, assetNo, purchasedId, price, description);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertDisposalAsset(string userId, string assetNoId, string disposalDate, double amount,
            int sold, string soldId, string description)
        {
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.InsertDisposalAsset(userId, assetNoId, disposalDate, amount, sold, soldId, description);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteAssetJournal(string assetCode)
        {
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.DeleteAssetJournal(assetCode);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOnlineCompanyList(int radio)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetOnlineCompanyList(radio);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOnlineCompanyInfo(int cpId)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetOnlineCompanyInfo(cpId);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLocalCompanyInfo(int cpId)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetCompanyById(cpId);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertUpdateOnlineCompany(string action, string cpId, string name, string address, string city, string phone, string email, string cPerson, string designation, string companyId, string districtId)
        {
          
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.InsertUpdateOnlineCompany(action, cpId, name, address, city, phone, email, cPerson,designation,companyId,districtId);
            AccountingDbGateway.UpdateProfile(action, name, cpId, companyId);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckCompany(string name)
        {
            AccountingDbGateway.OpenConnection();
            var companies = new List<Company>();
            var company = AccountingDbGateway.GetCompanyByName(name);
            if (company != null)
                companies.Add(company);
            AccountingDbGateway.CloseConnection();
            return Json(companies, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckOnlineCompany(int id)
        {
            AccountingDbGateway.OpenConnection();
            var company = AccountingDbGateway.GetCompanyById(id);
            AccountingDbGateway.CloseConnection();
            return Json(company, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CashCollection()
        {
            AccountingDbGateway.OpenConnection();
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapsetwo,4";
            ViewBag.Ledgers = AccountingDbGateway.GetAllLedger().Where(x => x.Under == "1,12,410").OrderBy(x => x.GroupName).ToList();
            ViewBag.Total = AccountingDbGateway.GetTotalOnlinePost();
            ViewBag.ClosingDate = AccountingDbGateway.GetClosingDate();
            AccountingDbGateway.CloseConnection();
            return View();
        }

        public JsonResult GetInvoicesForCashCollection(string query)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetInvoicesForCashCollection(query);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSalesInfo(string invoiceNo)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetSalesInfo(invoiceNo);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCashCollection(string id)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetCashCollection(id);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertCashCollection(string type, string userId, string invoiceNo, string cash, string date, string tno, string invoiceShedulerId, string ledgerId, string chequeDetails, string companyName)
        {
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.InsertCashCollection(type, userId, invoiceNo, cash, date, tno, invoiceShedulerId, ledgerId,
                chequeDetails, companyName);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAccountReceivable(string tno)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetAccountReceivable(tno);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateCashCollection(string type, string userId, string invoiceNo, string cash, string date, string tno, string invoiceShedulerId, string ledgerId, string chequeDetails, string companyName, string cashCollectionId)
        {
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.UpdateCashCollection(type, userId, invoiceNo, cash, date, tno, invoiceShedulerId, ledgerId,
                chequeDetails, companyName, cashCollectionId);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UnpaidCashCollection(string userId, string ledgerId, string tno, string invoiceId, string invoiceNo, string collectionid, string amount, string companyName)
        {
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.UnpaidCashCollection(userId, ledgerId, tno, invoiceId, invoiceNo, collectionid, amount,
                companyName);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit()
        {
            AccountingDbGateway.OpenConnection();
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapsetwo,3";
            //ViewBag.CompanyList = AccountingDbGateway.GetCompanyList();
            ViewBag.ClosingDate = AccountingDbGateway.GetClosingDate();
            ViewBag.Type = AccountingDbGateway.GetProducts(0, 0, "", "", "", 1);
            AccountingDbGateway.CloseConnection();
            return View();
        }
        public JsonResult GetSales(string pageNo, string pageSize, int cId)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetSales(pageNo, pageSize, cId, 0);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteSale(string tno)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.DeleteSale(tno);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MakeJournalOfSale(string sId, string amount, string jDate, string duration, string tno, string description, string salesdate, string taxId, string tax, string userId)
        {
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.MakeJournalOfSale(sId, amount, jDate, duration, tno, description, salesdate, taxId, tax, userId);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNumberOfId(string tno)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetNumberOfId(tno);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateSaleInfo(string salesPrice, string accReceivable, string duration, string tax, string sDate, string eDate, string tno, string contactId, string refNo)
        {
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.UpdateSaleInfo(salesPrice, accReceivable, duration, tax, sDate, eDate, tno, contactId, refNo);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateSalesJournal(string sid, string vatId, string tno, string oldDuration,
            string newDuration, string oldAmount, string newAmount, string oldVatAmount, string newVatAmount,
            string fromDate, string description, string userId)
        {
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.UpdateSalesJournal(sid, vatId, tno, oldDuration, newDuration, oldAmount, newAmount,
                oldVatAmount, newVatAmount, fromDate, description, userId);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateSalePosted(string sid, string newAmount, string fromdate, string newDuration, string tno, string description, string dateFrom, string vatId, string newVatAmount, string userId)
        {
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.UpdateSalePosted(sid, newAmount, fromdate, newDuration, tno, description, dateFrom, vatId,
                newVatAmount, userId);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateProduct(string oldSid, string tno, string newSid)
        {
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.UpdateSaleProduct(oldSid, tno, newSid);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateSaleContactPersonAndRefNo(string personId, string refNo, string tno)
        {
            AccountingDbGateway.OpenConnection();
            AccountingDbGateway.UpdateSaleContactPersonAndRefNo(personId, refNo, tno);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetMoneyReceipt(string invoices, string str)
        {
            AccountingDbGateway.OpenConnection();
            var receipts = AccountingDbGateway.GetMoneyReceipt(invoices);
            AccountingDbGateway.CloseConnection();
            Session["reportName"] = "rptRecceipt.rpt";
            Session["Report"] = receipts;
            Session["str"] = str;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UploadInvoicesOnline(int? cpId, string invoiceNo, int serviceNo, string billingContact, string price, string opId,string jpIdList)
        {
            AccountingDbGateway.OpenConnection();
            var invSendDt = AccountingDbGateway.GetInvSendDt(invoiceNo);
            var cid = cpId ?? 0;
            var results = AccountingDbGateway.UploadInvoiceOnline(cid, invoiceNo, serviceNo, invSendDt, billingContact,
                price, opId, jpIdList);
            AccountingDbGateway.CloseConnection();
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateInvoice(string invoiceNo)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.UpdateInvoice(invoiceNo);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProducts()
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.GetProducts(0, 0, "Revenue", "", "I", 0);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PostToOnline(string postType,string invoiceNo, string invoiceId)
        {
            AccountingDbGateway.OpenConnection();
            var returnValue = AccountingDbGateway.PostToOnline(postType, invoiceNo, invoiceId);
            AccountingDbGateway.CloseConnection();
            return Json(returnValue, JsonRequestBehavior.AllowGet);
        }
    }
}