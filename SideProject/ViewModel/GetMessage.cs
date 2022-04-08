using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SideProject.ViewModel
{
    public class GetMessage
    {
        public int ProjectsId { get; set; }
        public string ProfilePicture { get; set; }
        public string NickName { get; set; }
        public string MessageTitle { get; set; }
        public string MessageContent { get; set; }
        public DateTime InitDate { get; set; }
    }
}