using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SideProject.Models
{
    public class Collection
    {
        [Key]//主鍵 PK
        [Display(Name = "編號")]//顯示名稱
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//自動生成編號
        public int Id { get; set; }

        [Required]//必填
        [Display(Name = "收藏人ID")]
        public int MembersId { get; set; }

        [ForeignKey("MembersId")]
        public virtual Members Member { get; set; }

        [Required]//必填
        [Display(Name = "專案ID")]
        public int ProjectId { get; set; }

        [Display(Name = "建立時間")]//顯示名稱
        public DateTime InitDate { get; set; }
    }
}