using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SideProject.Models
{
    public class Members
    {
        [Key]//主鍵 PK
        [Display(Name = "編號")]//顯示名稱
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自動生成編號
        public int Id { get; set; }

        [Required]//必填
        [MaxLength(50)]//限制最大字數，未設定為Max
        [Display(Name = "帳號")]//顯示名稱
        public string Account { get; set; }

        [Required]//必填
        [MaxLength(100)]
        [Display(Name = "密碼")]//顯示名稱
        public string Password { get; set; }

        [Required]//必填
        [Display(Name = "密碼鹽")]//顯示名稱
        public string PasswordSalt { get; set; }

        [Required]//必填
        [Display(Name = "暱稱")]//顯示名稱
        [MaxLength(50)]
        public string NickName { get; set; }

        [Required]//必填
        [Display(Name = "性別")]//顯示名稱
        [MaxLength(20)]
        public string Gender { get; set; }

        [Required]//必填
        [Display(Name = "大頭貼")]//顯示名稱
        public string ProfilePicture { get; set; }

        [Display(Name = "IG")]//顯示名稱
        [MaxLength(200)]
        public string Ig { get; set; }

        [Display(Name = "FB")]//顯示名稱
        [MaxLength(200)]
        public string Fb { get; set; }

        [Display(Name = "個人網站")]//顯示名稱
        [MaxLength(200)]
        public string ProfileWebsite { get; set; }

        [Display(Name = "聯絡時間")]//顯示名稱
        [MaxLength(50)]
        public string ContactTime { get; set; }

        [Display(Name = "自我介紹")]//顯示名稱
        [MaxLength(1000)]
        public string SelfIntroduction { get; set; }

        [Display(Name = "目前狀態")]//顯示名稱
        [MaxLength(20)]
        public string WorkState { get; set; }

        [Display(Name = "語言")]//顯示名稱
        [MaxLength(100)]
        public string Language { get; set; }

        [Display(Name = "公司")]//顯示名稱
        [MaxLength(100)]
        public string Company { get; set; }

        [Display(Name = "產業")]//顯示名稱
        [MaxLength(100)]
        public string Industry { get; set; }

        [Display(Name = "職務")]//顯示名稱
        [MaxLength(100)]
        public string Position { get; set; }

        [Display(Name = "技能")]//顯示名稱
        public string Skills { get; set; }

        [Display(Name = "工作內容")]//顯示名稱
        [MaxLength(1000)]
        public string JobDescription { get; set; }

        [Display(Name = "建立時間")]//顯示名稱
        public DateTime InitDate { get; set; }

        public virtual ICollection<Projects> Project { get; set; }
        public virtual ICollection<Message> Message { get; set; }
        public virtual ICollection<Collection> Collection { get; set; }
    }
}