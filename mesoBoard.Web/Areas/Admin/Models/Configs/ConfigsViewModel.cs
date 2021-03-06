﻿using System.Collections.Generic;
using mesoBoard.Data;

namespace mesoBoard.Web.Areas.Admin.Models
{
    public class ConfigsViewModel
    {
        public IEnumerable<Config> Configs { get; set; }
        public string[] ConfigGroups { get; set; }
    }
}