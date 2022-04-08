using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SideProject.ViewModel
{
    public class ApplicantInfo
    {
        public string ApplicantState { get; set; }
        public string ApplicantSelfIntro { get; set; }
        public string ApplicantMessage { get; set; }
        public int MembersId { get; set; }
        public int ProjectsId { get; set; }
        public DateTime InitDate { get; set; }
    }
}