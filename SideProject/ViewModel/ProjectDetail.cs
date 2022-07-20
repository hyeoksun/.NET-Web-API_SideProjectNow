using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SideProject.ViewModel
{
    public class ProjectDetail
    {
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string ProjectContext { get; set; }
        public string GroupPhoto { get; set; }
        public DateTime InitDate { get; set; }
        public DateTime GroupDeadline { get; set; }
        public DateTime FinishedDeadline { get; set; }
        public int GroupNum { get; set; }
        public string PartnerCondition { get; set; }
        public List<int> PartnerSkills { get; set; }
        public int ProjectTypeId { get; set; }
        //public ArrayList ProjectTypeAry { get; set; }
        public string ProjectState { get; set; }
        public int MembersId { get; set; }

    }
}