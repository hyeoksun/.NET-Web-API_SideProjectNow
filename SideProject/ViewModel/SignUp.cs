using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SideProject.ViewModel
{
    /// <summary>
    /// 註冊的變數
    /// </summary>

    public class SignUp
{
    /// <summary>
    /// 帳號
    /// </summary>
        public string Account { get; set; }

        public string Password { get; set; }

        public string NickName { get; set; }


        public string Gender { get; set; }

        public string ProfilePicture { get; set; }

        public string ContactTime { get; set; }

    }
}