using SideProject.JWT;
using SideProject.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using SideProject.ViewModel;

namespace SideProject.Controllers
{
    public class MatchController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        
        /// <summary>
        /// 申請人申請參與專案(for申請人)
        /// </summary>
        /// <param name="Id">要參加的專案ID</param>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AttendProject")]
        [JwtAuthFilter]
        public IHttpActionResult AttendProject(int Id,ApplicantInfo info)
        {
            var data = JwtAuthUtil.GetUserList(Request.Headers.Authorization.Parameter);
           
            var attendID = data.Item1;
            var user = db.Members.FirstOrDefault(x => x.Id == attendID);
            var project = db.Projects.FirstOrDefault(x => x.Id == Id);

            if (project.MembersId != user.Id)
            {
                var hasAttend = db.Applicant.FirstOrDefault(x => x.MembersId == user.Id&&x.ProjectsId==Id);
                if (hasAttend != null)
                {
                    return Ok(new { status = "error", message = "您已申請過該專案" });
                }
                else
                {
                    Applicants applicant = new Applicants();
                    applicant.MembersId = user.Id;
                    applicant.ApplicantMessage = info.ApplicantMessage;
                    applicant.ApplicantSelfIntro = info.ApplicantSelfIntro;
                    applicant.ApplicantState = "審核中";
                    applicant.ProjectsId = project.Id;
                    applicant.InitDate = DateTime.Now;

                    db.Applicant.Add(applicant);
                    db.SaveChanges();
                    return Ok(new { status = "success", message = "申請專案成功" });
                }
            }
            else
            {
                return Ok(new { status = "error", message = "發起人不能申請自己專案噢"} );
            }
        }


        /// <summary>
        /// 申請人離開專案(for申請人)
        /// </summary>
        /// <param name="projectId">要離開的專案ID</param>
        /// <returns></returns>
        [HttpPost]
        [Route("LeaveProject")]
        [JwtAuthFilter]
        public IHttpActionResult LeaveProject(int projectId)
        {
            var data = JwtAuthUtil.GetUserList(Request.Headers.Authorization.Parameter);

            var userID = data.Item1;
            var user = db.Applicant.FirstOrDefault(x => x.MembersId == userID);
            var leavePojectId = db.Applicant.FirstOrDefault(x => x.ProjectsId == projectId);
            var userAndProject = from target in db.Applicant
                where (target.MembersId == user.MembersId & target.ProjectsId == leavePojectId.ProjectsId)
                select target;
            foreach (var item in userAndProject)
            {
                item.ApplicantState = "已退出";
            }

            db.SaveChanges();
            return Ok(new { status = "success", message = "退出專案成功" });
        }

        /// <summary>
        /// 取得專案申請者列表(for發起人)
        /// </summary>
        /// <param name="id">專案的ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetApplicant")]
        public IHttpActionResult GetApplicant(int id)
        {
            var applicantIdList = db.Applicant.Where(x => x.ProjectsId == id).
                Join(db.Members,a=>a.MembersId,b=>b.Id,(a,b)=>new
                {
                    ProjectsId=a.ProjectsId,
                    ApplicantState=a.ApplicantState,
                    InitDate=a.InitDate,
                    NickName=b.NickName,
                    ApplicantMessage=a.ApplicantMessage,
                    MembersId=a.MembersId
                }).ToList();

            return Ok(new {status = "success", message = "該專案申請者列表", data = applicantIdList});
        }

        /// <summary>
        /// 取得專案某申請人的詳細資料(for發起人)
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetApplicantInfo")]
        public IHttpActionResult GetApplicantInfo(int memberId, int projectId)
        {
            var applicant = db.Members.Where(x => x.Id == memberId).ToList();
            var selfIntro = db.Applicant.Where(x => x.MembersId == memberId && x.ProjectsId == projectId).Select(x => x.ApplicantSelfIntro);
            ArrayList skillListAry = new ArrayList();
            ArrayList applicantData = new ArrayList();
            foreach (var item in applicant)
            {
                if (item.Skills != null)
                {
                    var userSkills = item.Skills.Trim(',');
                    string[] skillListString = userSkills.Split(',');
                    int[] skillListInt = skillListString.Select(x => Convert.ToInt32(x.ToString())).ToArray();
                    foreach (var content in db.Skills)
                    {
                        foreach (var selectID in skillListInt)
                        {
                            if (selectID == item.Id)
                            {
                                var resultdata = new
                                {
                                    content.Id,
                                    content.skill
                                };
                                skillListAry.Add(resultdata);
                            }
                        }
                    }
                }
                else
                {
                    skillListAry = null;
                }
                
                var data = new
                {
                    item.ProfilePicture,
                    item.NickName,
                    item.Gender,
                    item.Account,
                    item.Fb,
                    item.Ig,
                    item.ProfileWebsite,
                    item.ContactTime,
                    item.WorkState,
                    item.Language,
                    item.Company,
                    item.Industry,
                    item.Position,
                    item.JobDescription,
                    Skills=skillListAry,
                    SelfIntoduction= selfIntro
                };
                applicantData.Add(data);
            }

            return Ok(new {status = "success", message = "會員資料", applicantData = applicantData});
        }

        /// <summary>
        /// 通過申請人(for發起人)
        /// </summary>
        /// <param name="memberId">申請人ID</param>
        /// <param name="projectId">申請人申請的專案ID</param>
        /// <returns></returns>
        [HttpPost]
        [Route("PassApplicant")]
        public IHttpActionResult PassApplicant(int memberId, int projectId)
        {
            var userAndProject = from target in db.Applicant
                where (target.MembersId == memberId & target.ProjectsId == projectId)
                select target;
            foreach (var item in userAndProject)
            {
                item.ApplicantState = "已通過";
            }

            db.SaveChanges();
            return Ok(new { status = "success", message = "已通過該申請人為組員" });
        }

        /// <summary>
        /// 不通過申請人(for發起人)
        /// </summary>
        /// <param name="memberId">申請人ID</param>
        /// <param name="projectId">申請人申請的專案ID</param>
        /// <returns></returns>
        [HttpPost]
        [Route("RejectApplicant")]
        public IHttpActionResult RejectApplicant(int memberId, int projectId)
        {
            var userAndProject = from target in db.Applicant
                where (target.MembersId == memberId & target.ProjectsId == projectId)
                select target;
            foreach (var item in userAndProject)
            {
                item.ApplicantState = "未通過";
            }

            db.SaveChanges();
            return Ok(new { status = "success", message = "已拒絕該申請人為組員" });
        }


        /// <summary>
        /// 發起人選定成員，開始專案，審核中申請人皆轉為未通過(for發起人)
        /// </summary>
        /// <param name="projectId">要開始的專案ID</param>
        /// <returns></returns>
        [HttpPost]
        [Route("StartProject")]
        [JwtAuthFilter]
        public IHttpActionResult StartProject(int projectId)
        {
            var data = JwtAuthUtil.GetUserList(Request.Headers.Authorization.Parameter);

            var userID = data.Item1;
            var StartProject = from target in db.Projects
                where (target.MembersId == userID & target.Id == projectId)
                select target;
            var Applicants = db.Applicant.Where(x => x.ProjectsId == projectId && x.ApplicantState=="審核中").ToList();
            foreach (var item in StartProject)
            {
                item.ProjectState = "進行中";
            }

            foreach (var applicant in Applicants)
            {
                applicant.ApplicantState = "未通過";
            }
            db.SaveChanges();
            return Ok(new { status = "success", message = "專案狀態為進行中" });
        }

        /// <summary>
        /// 廢棄專案(for發起人)
        /// </summary>
        /// <param name="projectId">專案ID</param>
        /// <returns></returns>
        [HttpPost]
        [Route("GiveUpProject")]
        [JwtAuthFilter]
        public IHttpActionResult GiveUpProject(int projectId)
        {
            var data = JwtAuthUtil.GetUserList(Request.Headers.Authorization.Parameter);

            var attendID = data.Item1;
            var user = db.Members.FirstOrDefault(x => x.Id == attendID);

            var userAndProject = from target in db.Projects
                where (target.Id == projectId && target.MembersId==user.Id)
                select target;
            foreach (var item in userAndProject)
            {
                item.ProjectState = "已廢棄";
            }
            db.SaveChanges();
            return Ok(new { status = "success", message = "已廢棄該專案" });
        }

        /// <summary>
        /// 刪除專案(for發起人)
        /// </summary>
        /// <param name="projectId">專案ID</param>
        /// <returns></returns>
        [HttpPost]
        [Route("DeleteProject")]
        public IHttpActionResult DeleteProject(int projectId)
        {
            var data = JwtAuthUtil.GetUserList(Request.Headers.Authorization.Parameter);

            var attendID = data.Item1;
            var user = db.Members.FirstOrDefault(x => x.Id == attendID);

            var userAndProject = from target in db.Projects
                where (target.Id == projectId && target.MembersId == user.Id)
                select target;
            
            foreach (var item in userAndProject)
            {
                item.ProjectState = "已刪除";
            }
            db.SaveChanges();
            return Ok(new { status = "success", message = "已刪除該專案" });
        }

        /// <summary>
        /// 關閉專案(for發起人)
        /// </summary>
        /// <param name="projectId">專案ID</param>
        /// <returns></returns>
        [HttpPost]
        [Route("CloseProject")]
        public IHttpActionResult CloseProject(int projectId)
        {
            var data = JwtAuthUtil.GetUserList(Request.Headers.Authorization.Parameter);

            var attendID = data.Item1;
            var user = db.Members.FirstOrDefault(x => x.Id == attendID);

            var userAndProject = from target in db.Projects
                where (target.Id == projectId && target.MembersId == user.Id)
                select target;

            var applicant = db.Applicant.Where(x => x.ProjectsId == projectId).ToList();
            foreach (var member in applicant)
            {
                member.ApplicantState = "未通過";
            }
            
            foreach (var item in userAndProject)
            {
                item.ProjectState = "已關閉";
            }
            db.SaveChanges();
            return Ok(new { status = "success", message = "已關閉該專案" });
        }

        /// <summary>
        /// 重啟專案(for發起人)
        /// </summary>
        /// <param name="projectId">專案ID</param>
        /// <returns></returns>
        [HttpPost]
        [Route("RestartProject")]
        public IHttpActionResult RestartProject(int projectId)
        {
            var data = JwtAuthUtil.GetUserList(Request.Headers.Authorization.Parameter);

            var attendID = data.Item1;
            var user = db.Members.FirstOrDefault(x => x.Id == attendID);

            var userAndProject = from target in db.Projects
                where (target.Id == projectId && target.MembersId == user.Id)
                select target;
            
            foreach (var item in userAndProject)
            {
                item.ProjectState = "媒合中";
                item.InitDate=DateTime.Now;
                item.GroupDeadline=DateTime.Now.AddDays(7);
            }
            db.SaveChanges();
            return Ok(new { status = "success", message = "已重啟該專案" });
        }
    }
}