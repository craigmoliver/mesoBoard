﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using mesoBoard.Framework.Core;
using mesoBoard.Services;
using mesoBoard.Web.Areas.Admin.ViewModels;
using System.IO;
using mesoBoard.Framework;

namespace mesoBoard.Web.Areas.Admin.Controllers
{
    public class UpgradeController : BaseAdminController
    {
        private UpgradeServices _upgradeServices;

        public UpgradeController(UpgradeServices upgradeServices)
        {
            _upgradeServices = upgradeServices;
            SetCrumb("Upgrade");
        }

        [DefaultAction]
        public ActionResult Upgrade()
        {
            var upgrades = _upgradeServices.GetAvailableUpgrades();
            var model = new UpgradeViewModel()
            {
                AvailableUpgrades = upgrades
            };
            return View(model);
        }

        public ActionResult Confirm(string version)
        {
            UpgradeDetailsViewModel model = new UpgradeDetailsViewModel();
            model.Version = version;
            string partialFileName = string.Format("{0}.cshtml", version);
            string partialPath = Path.Combine(Server.MapPath(DirectoryPaths.Upgrade), version, partialFileName);
            model.HasDetails = System.IO.File.Exists(partialPath);
            if (model.HasDetails)
                model.Details = System.IO.File.ReadAllText(partialPath);
            return View(model);
        }

        [HttpPost]
        public ActionResult Confirm(string version, string Action)
        {
            if (Action == "Cancel")
            {
                SetSuccess("Upgrade cancelled");
                return RedirectToAction("Index");
            }

            switch (version)
            {
                case "0.9.3":
                    _upgradeServices.To093();
                    break;
                case "0.9.2":
                    _upgradeServices.To092();
                    break;

                default:
                    SetError("Select a version to upgrade to");
                    return RedirectToAction("Index");
            }
            SetSuccess(string.Format("mesoBoard was successfully upgrade to version {0}. Please verify the changes mentioned in the upgrade details.", version));
            return RedirectToAction("Upgrade");
        }
    }
}
