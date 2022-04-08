using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SideProject.Models
{
    public class Applicants
    {
        [Key]//主鍵 PK
        [Display(Name = "編號")]//顯示名稱
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自動生成編號
        public int Id { get; set; }

        [Required]//必填
        [MaxLength(5)]//限制最大字數，未設定為Max
        [Display(Name = "申請人狀態")]//顯示名稱
        public string ApplicantState { get; set; }
        
        [Required]//必填
        [Display(Name = "申請人自我介紹")]//顯示名稱
        public string ApplicantSelfIntro { get; set; }

        [Required]//必填
        [Display(Name = "申請人留言")]//顯示名稱
        public string ApplicantMessage { get; set; }

        [Required]//必填
        [Display(Name = "申請人ID")]
        public int MembersId { get; set; }

        [Display(Name = "專案ID")]
        public int ProjectsId { get; set; }

        [ForeignKey("ProjectsId")]
        public virtual Projects Project { get; set; }

        [Display(Name = "建立時間")]//顯示名稱
        public DateTime InitDate { get; set; }
    }
}