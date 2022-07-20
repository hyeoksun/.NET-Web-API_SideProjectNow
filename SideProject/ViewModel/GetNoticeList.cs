using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SideProject.ViewModel
{
    public class GetNoticeList
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string GroupPhoto { get; set; }
        public DateTime InitDate { get; set; }
    }
}