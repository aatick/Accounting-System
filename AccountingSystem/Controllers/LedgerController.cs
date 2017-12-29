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
        public ActionResult Create(string page)
        {
            AccountingDbGateway.OpenConnection();
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapseOne,1";
            var allLedger = AccountingDbGateway.GetAllLedger();
            ViewBag.MainGroupList = new SelectList(allLedger.OrderBy(p => p.Id).Take(5).OrderBy(x => x.GroupName).ToList(), "Id", "GroupName");
            AccountingDbGateway.CloseConnection();
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
            AccountingDbGateway.OpenConnection();
            if (this.RenewSession() == false)
                return RedirectToAction("Index", "Home");
            ViewBag.AccordionId = "collapseOne,2";
            var allLedger = AccountingDbGateway.GetAllLedger();
            AccountingDbGateway.CloseConnection();
            return View(allLedger.OrderBy(p => p.Id).ToList());
        }

        public JsonResult GetGroup(int? groupId)
        {
            AccountingDbGateway.OpenConnection();
            var allLedger = AccountingDbGateway.GetAllLedger();
            AccountingDbGateway.CloseConnection();
            var selectedLedger = allLedger.FirstOrDefault(x => x.Id == groupId);
            return Json(selectedLedger, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGroupByName(string name)
        {
            AccountingDbGateway.OpenConnection();
            var allLedger = AccountingDbGateway.GetAllLedger();
            AccountingDbGateway.CloseConnection();
            var selectedLedger = allLedger.OrderBy(x => x.GroupName).FirstOrDefault(x => x.GroupName.ToLower().StartsWith(name.ToLower()));
            return Json(selectedLedger, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubgroups(string mainGroup)
        {
            AccountingDbGateway.OpenConnection();
            var subgroupList =
                AccountingDbGateway.GetAllLedger()
                    .Where(x => x.MaingroupName == mainGroup && !x.IsLedgerAccount)
                    .OrderBy(x => x.GroupName)
                    .ToList();
            AccountingDbGateway.CloseConnection();
            return Json(subgroupList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Save(string group, string mainGroup, int? subGroupId, bool isLedger)
        {
            AccountingDbGateway.OpenConnection();
            var aLedger = new Ledger();
            var allLedger = AccountingDbGateway.GetAllLedger();
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
            AccountingDbGateway.SaveLedger(aLedger);
            allLedger.Add(aLedger);
            AccountingDbGateway.CloseConnection();
            return
                Json(
                    allLedger.Where(x => x.MaingroupName == mainGroup && !x.IsLedgerAccount)
                        .OrderBy(x => x.GroupName)
                        .ToList(), JsonRequestBehavior.AllowGet);
        }
        public JsonResult Update(string group, string mainGroup, int groupId, int underId, bool isLedger)
        {
            AccountingDbGateway.OpenConnection();
            var allLedger = AccountingDbGateway.GetAllLedger();
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
            AccountingDbGateway.UpdateLedger(selectedGroup);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int groupId, string gName)
        {
            AccountingDbGateway.OpenConnection();
            if (AccountingDbGateway.GetJournalBySId(groupId) != null)
                return Json("You can not delete this ledger. One or more journal(s) exist for this selected ledger.", JsonRequestBehavior.AllowGet);
            if (AccountingDbGateway.GetAllLedger().FirstOrDefault(x => x.Under.Contains(groupId.ToString())) != null)
                return Json("This Group has one or more Ledger(s). You first delete those Ledgers then delete this group.", JsonRequestBehavior.AllowGet);
            AccountingDbGateway.DeleteLedger(groupId);
            AccountingDbGateway.CloseConnection();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubGroupsWithLedger(int? groupId)
        {
            AccountingDbGateway.OpenConnection();
            var allLedger = AccountingDbGateway.GetAllLedger();
            AccountingDbGateway.CloseConnection();
            return Json(allLedger.OrderBy(x => x.GroupName), JsonRequestBehavior.AllowGet);
        }
    }
}