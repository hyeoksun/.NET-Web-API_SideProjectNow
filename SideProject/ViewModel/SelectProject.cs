using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SideProject.ViewModel
{
    public class SelectProject
    {
        public int? projectType { get; set; }
        public DateTime? starTime { get; set; }
        public DateTime? endTime { get; set; }
        public int? groupNum { get; set; }
        public int? skill { get; set; }
        public string keyword { get; set; }
    }
}