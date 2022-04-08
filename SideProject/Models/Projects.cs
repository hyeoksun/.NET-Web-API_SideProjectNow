using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SideProject.Models
{
    public class Projects
    {
        [Key]//主鍵 PK
        [Display(Name = "編號")]//顯示名稱
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自動生成編號
        public int Id { get; set; }

        [Required]//必填
        [MaxLength(100)]//限制最大字數，未設定為Max
        [Display(Name = "專案名稱")]//顯示名稱
        public string ProjectName { get; set; }

        [Required]//必填
        [MaxLength(1000)]//限制最大字數，未設定為Max
        [Display(Name = "專案內容")]//顯示名稱
        public string ProjectContext { get; set; }

        [MaxLength(30)]//限制最大字數，未設定為Max
        [Display(Name = "專案圖片")]//顯示名稱
        public string GroupPhoto { get; set; }

        [Display(Name = "建立時間")]//顯示名稱
        public DateTime InitDate { get; set; }

        [Display(Name = "媒合截止日")]//顯示名稱
        public DateTime GroupDeadline { get; set; }

        [Display(Name = "專案結束日")]//顯示名稱
        public DateTime FinishedDeadline { get; set; }

        [Display(Name = "團隊人數")]//顯示名稱
        public int GroupNum { get; set; }

        [Required]//必填
        [MaxLength(1000)]//限制最大字數，未設定為Max
        [Display(Name = "夥伴條件")]//顯示名稱
        public string PartnerCondition { get; set; }

        [Required]//必填
        [MaxLength(1000)]//限制最大字數，未設定為Max
        [Display(Name = "夥伴技能")]//顯示名稱
        public string PartnerSkills { get; set; }

        [Required]//必填
        [Display(Name = "專案類別")]//顯示名稱
        public int ProjectTypeId { get; set; }

        [Required]//必填
        [MaxLength(5)]//限制最大字數，未設定為Max
        [Display(Name = "專案狀態")]//顯示名稱
        public string ProjectState { get; set; }

        [MaxLength(100)]//限制最大字數，未設定為Max
        [Display(Name = "專案網址")]//顯示名稱
        public string ProjectWebsite { get; set; }

        [MaxLength(30)]//限制最大字數，未設定為Max
        [Display(Name = "專案封面圖")]//顯示名稱
        public string ProjectBanner { get; set; }

        [MaxLength(100)]//限制最大字數，未設定為Max
        [Display(Name = "專案內容圖")]//顯示名稱
        public string ProjectPhotos { get; set; }

        [MaxLength(1000)]//限制最大字數，未設定為Max
        [Display(Name = "專案心得")]//顯示名稱
        public string ProjectExperience { get; set; }

        [Display(Name = "發起人編號")]
        public int MembersId { get; set; }

        [ForeignKey("MembersId")]
        public virtual Members Member { get; set; }

        [ForeignKey("ProjectTypeId")]
        public virtual ProjectClass ProjectClass { get; set; }

        public virtual ICollection<Applicants> Applicant { get; set; }


    }
}