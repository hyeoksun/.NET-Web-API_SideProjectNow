using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SideProject.Models
{
    public class ProjectSkills
    {
        [Key]//主鍵 PK
        [Display(Name = "編號")]//顯示名稱
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自動生成編號
        public int Id { get; set; }

        [Required]//必填
        [Display(Name = "專案ID")]//顯示名稱
        public int ProjectId { get; set; }
        
        [Required]//必填
        [Display(Name = "技能ID")]//顯示名稱
        public int SkillId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Projects Project { get; set; }

    }
}