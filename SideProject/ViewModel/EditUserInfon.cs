using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SideProject.ViewModel
{
    /// <summary>
    /// 會員資料編輯data
    /// </summary>
    public class EditUserInfon
    {
        public string Account { get; set; }
        public string NickName { get; set; }
        public string Gender { get; set; }
        public string ProfilePicture { get; set; }
        public string Ig { get; set; }
        public string Fb { get; set; }
        public string ProfileWebsite { get; set; }
        public string ContactTime { get; set; }
        public string SelfIntroduction { get; set; }
        public string WorkState { get; set; }
        public string Language { get; set; }
        public string Company { get; set; }
        public string Industry { get; set; }
        public string Position { get; set; }
        public List<string> Skills { get; set; }
        public string JobDescription { get; set; }
    }
}