using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SideProject.Models
{
    public class Message
    {
        [Key]//主鍵 PK
        [Display(Name = "編號")]//顯示名稱
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自動生成編號
        public int MessageId { get; set; }

        [Required]//必填
        [Display(Name = "留言標題")]//顯示名稱
        public string MessageTitle { get; set; }

        [Required]//必填
        [Display(Name = "留言內容")]//顯示名稱
        public string MessageContent { get; set; }

        [Required]//必填
        [Display(Name = "留言人ID")]
        public int MembersId { get; set; }

        [ForeignKey("MembersId")]
        public virtual Members Member { get; set; }

        [Display(Name = "專案ID")]
        public int ProjectsId { get; set; }

        [Display(Name = "建立時間")]//顯示名稱
        public DateTime InitDate { get; set; }
    }
}