using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SideProject.Models
{
    public class UpdateProjectState
    {
        [Key]//主鍵 PK
        [Display(Name = "編號")]//顯示名稱
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自動生成編號
        public int Id { get; set; }

        [Display(Name = "更新時間")]//顯示名稱
        public DateTime UpdateTime { get; set; }
    }
}