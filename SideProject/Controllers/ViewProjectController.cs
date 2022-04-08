using SideProject.JWT;
using SideProject.Models;
using SideProject.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SideProject.Controllers
{
    public class ViewProjectController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// 會員取得專案列表(不含已完成，有分頁)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllProject")]
        [JwtAuthFilter]
        public IHttpActionResult GetAllProject([FromUri]SelectProject data, int page)
        {

            var projects = db.Projects.Where (x=>!(x.ProjectState.Equals("已完成")) && !(x.ProjectState.Equals("已刪除")));
            if (data.projectType.HasValue)
            {
                projects = projects.Where(x => x.ProjectTypeId == data.projectType);
            }

            if (data.starTime.HasValue)
            {
                projects = projects.Where(x => x.InitDate >= data.starTime);
            }

            if (data.endTime.HasValue)
            {
                projects = projects.Where(x => x.InitDate <= data.endTime);
            }

            if (data.groupNum.HasValue)
            {
                projects = projects.Where(x => x.GroupNum == data.groupNum);
            }

            if (!string.IsNullOrWhiteSpace(data.skill))
            {
                projects = projects.Where(x=>x.PartnerSkills.Contains(data.skill));
            }
            
            if (!string.IsNullOrWhiteSpace(data.keyword))
            {
                projects = projects.Where(x => x.ProjectName.Contains(data.keyword));
            }

            var projectList = projects.ToList();
            var token = JwtAuthUtil.GetUserList(Request.Headers.Authorization.Parameter);
            var userID = token.Item1;
            var user = db.Members.FirstOrDefault(x => x.Id == userID);
            var collects = db.Collections.Where(x => x.MembersId == userID).ToList();

            List<GetProjectList> arrayList = new List<GetProjectList>();
            foreach (var content in projectList)
            {
                //讀出所選的技能列表
                string[] skillList = content.PartnerSkills.Split(',');
                int[] skillListInt = skillList.Select(x => Convert.ToInt32(x.ToString())).ToArray();
                ArrayList skillListAry = new ArrayList();
                foreach (var item in db.Skills)
                {
                    foreach (var selectID in skillListInt)
                    {
                        if (selectID == item.Id)
                        {
                            var resultdata = new
                            {
                                item.Id,
                                item.skill
                            };
                            skillListAry.Add(resultdata);
                        }

                    }
                }

                ArrayList projectTypeAry = new ArrayList();
                foreach (var item in db.ProjectType)
                {
                    if (item.Id == content.ProjectTypeId)
                    {
                        var resultdata = new
                        {
                            item.Id,
                            item.ProjectType
                        };
                        projectTypeAry.Add(resultdata);
                    }
                }

                var collect = collects.Where(x => x.ProjectId == content.Id).FirstOrDefault();
                bool collectResult;
                if (collect != null)
                {
                    collectResult = true;
                }
                else
                {
                    collectResult = false;
                }
                GetProjectList newProjectArray = new GetProjectList();
                newProjectArray.Id = content.Id;
                newProjectArray.ProjectName = content.ProjectName;
                newProjectArray.ProjectContext = content.ProjectContext;
                newProjectArray.InitDate = content.InitDate;
                newProjectArray.FinishedDeadline = content.FinishedDeadline;
                newProjectArray.GroupDeadline = content.GroupDeadline;
                newProjectArray.ProjectState = content.ProjectState;
                newProjectArray.PartnerSkills = skillListAry;
                newProjectArray.GroupNum = content.GroupNum;
                newProjectArray.GroupPhoto = content.GroupPhoto;
                newProjectArray.ProjectTypeId = projectTypeAry;
                newProjectArray.CollectOrNot = collectResult;

                arrayList.Add(newProjectArray);

            }

            int total = arrayList.Count;
            var dataCount = (page - 1) * 10;
            var ProjectLists = arrayList.OrderByDescending(x => x.InitDate).Skip(dataCount).Take(10);
            ArrayList result = new ArrayList();
            foreach (var item in ProjectLists)
            {
                var resultdata = new
                {
                    item.Id,
                    item.ProjectName,
                    item.ProjectContext,
                    item.ProjectTypeId,
                    item.InitDate,
                    item.FinishedDeadline,
                    item.GroupDeadline,
                    item.ProjectState,
                    item.PartnerSkills,
                    item.GroupNum,
                    item.GroupPhoto,
                    item.CollectOrNot
                };
                result.Add(resultdata);
            }

            return Ok(new {status = "success", message = "已完成外的專案列表", page = page, datatotal = total, data = result});

        }

        /// <summary>
        /// 非會員取得專案列表(不含已完成，有分頁)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllProjectGuest")]
        public IHttpActionResult GetAllProjectGuest([FromUri] SelectProject data, int page)
        {

            var projects = db.Projects.Where(x => !(x.ProjectState.Equals("已完成")) && !(x.ProjectState.Equals("已刪除")));
            if (data.projectType.HasValue)
            {
                projects = projects.Where(x => x.ProjectTypeId == data.projectType);
            }

            if (data.starTime.HasValue)
            {
                projects = projects.Where(x => x.InitDate >= data.starTime);
            }

            if (data.endTime.HasValue)
            {
                projects = projects.Where(x => x.InitDate <= data.endTime);
            }

            if (data.groupNum.HasValue)
            {
                projects = projects.Where(x => x.GroupNum == data.groupNum);
            }

            if (!string.IsNullOrWhiteSpace(data.skill))
            {
                projects = projects.Where(x => x.PartnerSkills.Contains(data.skill));
            }

            if (!string.IsNullOrWhiteSpace(data.keyword))
            {
                projects = projects.Where(x => x.ProjectName.Contains(data.keyword));
            }

            var projectList = projects.ToList();

            List<GetProjectList> arrayList = new List<GetProjectList>();
            foreach (var content in projectList)
            {
                //讀出所選的技能列表
                string[] skillList = content.PartnerSkills.Split(',');
                int[] skillListInt = skillList.Select(x => Convert.ToInt32(x.ToString())).ToArray();
                ArrayList skillListAry = new ArrayList();
                foreach (var item in db.Skills)
                {
                    foreach (var selectID in skillListInt)
                    {
                        if (selectID == item.Id)
                        {
                            var resultdata = new
                            {
                                item.Id,
                                item.skill
                            };
                            skillListAry.Add(resultdata);
                        }

                    }
                }

                ArrayList projectTypeAry = new ArrayList();
                foreach (var item in db.ProjectType)
                {
                    if (item.Id == content.ProjectTypeId)
                    {
                        var resultdata = new
                        {
                            item.Id,
                            item.ProjectType
                        };
                        projectTypeAry.Add(resultdata);
                    }
                }

                GetProjectList newProjectArray = new GetProjectList();
                newProjectArray.Id = content.Id;
                newProjectArray.ProjectName = content.ProjectName;
                newProjectArray.ProjectContext = content.ProjectContext;
                newProjectArray.InitDate = content.InitDate;
                newProjectArray.FinishedDeadline = content.FinishedDeadline;
                newProjectArray.GroupDeadline = content.GroupDeadline;
                newProjectArray.ProjectState = content.ProjectState;
                newProjectArray.PartnerSkills = skillListAry;
                newProjectArray.GroupNum = content.GroupNum;
                newProjectArray.GroupPhoto = content.GroupPhoto;
                newProjectArray.ProjectTypeId = projectTypeAry;

                arrayList.Add(newProjectArray);

            }

            int total = arrayList.Count;
            var dataCount = (page - 1) * 10;
            var ProjectLists = arrayList.OrderByDescending(x => x.InitDate).Skip(dataCount).Take(10);
            ArrayList result = new ArrayList();
            foreach (var item in ProjectLists)
            {
                var resultdata = new
                {
                    item.Id,
                    item.ProjectName,
                    item.ProjectContext,
                    item.ProjectTypeId,
                    item.InitDate,
                    item.FinishedDeadline,
                    item.GroupDeadline,
                    item.ProjectState,
                    item.PartnerSkills,
                    item.GroupNum,
                    item.GroupPhoto
                };
                result.Add(resultdata);
            }

            return Ok(new { status = "success", message = "已完成外的專案列表", page = page, datatotal = total, data = result });

        }

        /// <summary>
        /// 取得專案詳細內容
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetProjectDetail")]
        public IHttpActionResult GetProjectDetail(int Id)
        {
            //var data = JwtAuthUtil.GetUserList(Request.Headers.Authorization.Parameter);
            //string guestState = "";
            //if (data != null)
            //{
            //    var member = data.Item1;
            //    var project = db.Projects.Where(x => x.Id == Id).ToList();
            //    int organizer=0;
            //    foreach (var item in project)
            //    {
            //        organizer = item.MembersId;
            //    }

            //    if (member == organizer)
            //    {
            //        true
            //    }
            //}
            //else
            //{
            //    guestState = "false";
            //}
            var projectId = db.Projects.FirstOrDefault(x => x.Id == Id);
            {
                //讀出技能列表
                string[] skillList = projectId.PartnerSkills.Split(',');
                int[] skillListInt = skillList.Select(x => Convert.ToInt32(x.ToString())).ToArray();
                ArrayList skillListAry = new ArrayList();
                foreach (var item in db.Skills)
                {
                    foreach (var SelectID in skillListInt)
                    {
                        if (SelectID == item.Id)
                        {
                            var resultdata = new
                            {
                                item.Id,
                                item.skill
                            };
                            skillListAry.Add(resultdata);
                        }

                    }
                }

                //取得專案類別的ID與型別
                ArrayList projectTypeAry = new ArrayList();
                foreach (var item in db.ProjectType)
                {
                    if (item.Id == projectId.ProjectTypeId)
                    {
                        var resultdata = new
                        {
                            item.Id,
                            item.ProjectType
                        };
                        projectTypeAry.Add(resultdata);
                    }
                }

                ArrayList member = new ArrayList();
                foreach (var item in db.Members)
                {
                    if (item.Id == projectId.MembersId)
                    {
                        var memberData = new
                        {
                            item.ProfilePicture,
                            item.NickName
                        };
                        member.Add(memberData);
                    }
                }

                var applicantInfo = db.Members;
                var applicantIdList = db.Applicant.Where(x => x.ProjectsId == projectId.Id).Select(x => x.MembersId)
                    .ToList();
                ArrayList applicants = new ArrayList();
                foreach (var item in applicantIdList)
                {
                    var applicantList = applicantInfo.Where(x => x.Id == item).ToList();
                    foreach (var info in applicantList)
                    {
                        var infoData = new
                        {
                            info.ProfilePicture,
                            info.NickName
                        };
                        applicants.Add(infoData);
                    }
                }

                var result = new
                {
                    projectId.Id,
                    projectId.ProjectName,
                    projectId.ProjectContext,
                    projectId.GroupPhoto,
                    projectId.InitDate,
                    projectId.GroupDeadline,
                    projectId.FinishedDeadline,
                    projectId.ProjectState,
                    projectId.GroupNum,
                    projectId.PartnerCondition,
                    PartnerSkills = skillListAry,
                    ProjectTypeId = projectTypeAry,
                    Organizer = member,
                    Applicants = applicants
                };

                return Ok(new {status = "success", message = "專案詳細資料", userdata = result});
            }
        }

        /// <summary>
        /// 會員在專案留言
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="messageContent"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SendProjectMessage")]
        public IHttpActionResult SendProjectMessage(int projectId, string title, string messageContent)
        {
            var data = JwtAuthUtil.GetUserList(Request.Headers.Authorization.Parameter);
            var memberID = data.Item1;
            Message message = new Message();
            message.ProjectsId = projectId;
            message.MembersId = memberID;
            message.MessageTitle = title;
            message.MessageContent = messageContent;
            message.InitDate = DateTime.Now;
            db.Messages.Add(message);
            db.SaveChanges();
            return Ok(new {status = "success", message = "留言成功"});
        }

        /// <summary>
        /// 取得特定專案的所有留言
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetProjectMessage")]
        public IHttpActionResult GetProjectMessage(int projectId)
        {
            var projectsId = db.Messages.Where(x => x.ProjectsId == projectId).ToList();

            List<GetMessage> data = new List<GetMessage>();
            foreach (var item in projectsId)
            {
                GetMessage message = new GetMessage();
                message.ProjectsId = item.ProjectsId;
                message.ProfilePicture = item.Member.ProfilePicture;
                message.NickName = item.Member.NickName;
                message.MessageTitle = item.MessageTitle;
                message.MessageContent = item.MessageContent;
                message.InitDate = item.InitDate;

                data.Add(message);
            }

            return Ok(new {status = "success", message = "該專案留言內容", data = data});
        }

        /// <summary>
        /// 驗證是否為發起人
        /// </summary>
        /// <param name="projectId">該專案ID</param>
        /// <returns></returns>
        [HttpPost]
        [Route("CheckUser")]
        [JwtAuthFilter]
        public IHttpActionResult CheckUser(int projectId)
        {
            var data = JwtAuthUtil.GetUserList(Request.Headers.Authorization.Parameter);
            //var data = JwtAuthUtil.GetUserList(token);
            var userID = data.Item1;
            var user = db.Members.FirstOrDefault(x => x.Id == userID);
            var organizer = db.Projects.FirstOrDefault(x => x.Id == projectId);
            if (user.Id == organizer.MembersId)
            {
                return Ok(new {status = true, message = "會員為發起人"});
            }
            else
            {
                return Ok(new {status = false, message = "會員，但非發起人"});
            }
        }
    }
}    