using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AccountingSystem.Models;

namespace AccountingSystem.Controllers
{
    public class LedgerController : Controller
    {
        private AccountingDbGateway AccountingDb = new AccountingDbGateway();

        public bool RenewSession()
        {
            if (ControllerContext.HttpContext.Request.Cookies["userid"] != null)
            {
                var userid = ControllerContext.HttpContext.Request.Cookies["userid"].Value;
                if (Session["loggedinUser"] == null)
                {
                    var user = AccountingDb.GetAllUsers().FirstOrDefault(x => x.UserId.ToString() == userid && x.ValidUser);
                    Session["loggedinUser"] = user;
                }
                return true;
            }
            return false;
        }
        public ActionResult Create(string page)
        {
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapseOne,1";
            var allLedger = AccountingDb.GetAllLedger();
            ViewBag.MainGroupList = new SelectList(allLedger.OrderBy(p => p.Id).Take(5).OrderBy(x => x.GroupName).ToList(), "Id", "GroupName");
            ViewBag.SubgroupList =
                new SelectList(
                    allLedger.Where(x => !x.IsLedgerAccount).OrderBy(x => x.GroupName).ToList(),
                    "Id",
                    "GroupName");
            if (page != null)
                ViewBag.Page = page;
            return View();
        }

        public ActionResult Edit()
        {
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapseOne,2";
            var allLedger = AccountingDb.GetAllLedger();
            return View(allLedger.OrderBy(p => p.Id).ToList());
        }

        public JsonResult GetGroup(int? groupId)
        {
            var allLedger = AccountingDb.GetAllLedger();
            var selectedLedger = allLedger.FirstOrDefault(x => x.Id == groupId);
            return Json(selectedLedger, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGroupByName(string name)
        {
            var allLedger = AccountingDb.GetAllLedger();
            var selectedLedger = allLedger.OrderBy(x => x.GroupName).FirstOrDefault(x => x.GroupName.ToLower().StartsWith(name.ToLower()));
            return Json(selectedLedger, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubgroups(string mainGroup)
        {
            var subgroupList =
                AccountingDb.GetAllLedger()
                    .Where(x => x.MaingroupName == mainGroup && !x.IsLedgerAccount)
                    .OrderBy(x => x.GroupName)
                    .ToList();
            return Json(subgroupList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Save(string group, string mainGroup, int? subGroupId, bool isLedger)
        {
            var aLedger = new Ledger();
            var allLedger = AccountingDb.GetAllLedger();
            aLedger.GroupName = group;
            var ledger = allLedger.FirstOrDefault(x => x.Id == subGroupId);
            aLedger.Under = ledger.Under + "," + subGroupId;
            if (ledger.Under == "0")
            {
                aLedger.Under = ledger.Id.ToString();
            }
            aLedger.MaingroupName = mainGroup;
            aLedger.LevelNo = ledger.LevelNo + 1;
            aLedger.IsLedgerAccount = isLedger;
            AccountingDb.SaveLedger(aLedger);
            allLedger.Add(aLedger);
            return
                Json(
                    allLedger.Where(x => x.MaingroupName == mainGroup && !x.IsLedgerAccount)
                        .OrderBy(x => x.GroupName)
                        .ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Update(string group, string mainGroup, int groupId, int underId, bool isLedger)
        {
            var allLedger = AccountingDb.GetAllLedger();
            var selectedGroup = allLedger.FirstOrDefault(x => x.Id == groupId);
            selectedGroup.GroupName = group;
            var ledger = allLedger.FirstOrDefault(x => x.Id == underId);
            selectedGroup.Under = ledger.Under + "," + ledger.Id;
            if (ledger.Under == "0")
            {
                selectedGroup.Under = ledger.Id.ToString();
            }
            selectedGroup.MaingroupName = mainGroup;
            selectedGroup.LevelNo = ledger.LevelNo + 1;
            selectedGroup.IsLedgerAccount = isLedger;
            AccountingDb.UpdateLedger(selectedGroup);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int groupId, string gName)
        {
            if (AccountingDb.GetJournalBySId(groupId) != null)
                return Json("You can not delete this ledger. One or more journal(s) exist for this selected ledger.", JsonRequestBehavior.AllowGet);
            if (AccountingDb.GetAllLedger().FirstOrDefault(x => x.Under.Contains(groupId.ToString())) != null)
                return Json("This Group has one or more Ledger(s). You first delete those Ledgers then delete this group.", JsonRequestBehavior.AllowGet);
            AccountingDb.DeleteLedger(groupId);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubGroupsWithLedger(int? groupId)
        {
            var allLedger = AccountingDb.GetAllLedger();
            return Json(allLedger.OrderBy(x => x.GroupName), JsonRequestBehavior.AllowGet);
        }
    }
}