using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SideProject.Models
{
    public class ProjectClass
    {
        [Key]//主鍵 PK
        [Display(Name = "編號")]//顯示名稱
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自動生成編號
        public int Id { get; set; }

        [Required]//必填
        [MaxLength(100)]//限制最大字數，未設定為Max
        [Display(Name = "專案類別")]//顯示名稱
        public string ProjectType { get; set; }

        public virtual ICollection<Projects> Projects { get; set; }
    }
}