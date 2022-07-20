using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SideProject.JWT;
using SideProject.Models;
using SideProject.ViewModel;

namespace SideProject.Controllers
{
    public class UserController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// 會員資料檢視
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetUserInfo")]
        [JwtAuthFilter]
        public IHttpActionResult UserInfo()
        {
            var data = JwtAuthUtil.GetUserList(Request.Headers.Authorization.Parameter);

            var account = data.Item2;
            var user = db.Members.FirstOrDefault(x => x.Account == account);
            ArrayList skillListAry = new ArrayList();
            if (user.Skills != null)
            {
                var userSkills = user.Skills.Trim(',');
                string[] skillListString = userSkills.Split(',');
                int[] skillListInt = skillListString.Select(x => Convert.ToInt32(x.ToString())).ToArray();
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
            }
            else
            {
                skillListAry = null;
            }
            var result = new
            {
                user.Account,
                user.NickName,
                user.Gender,
                user.ProfilePicture,
                user.Ig,
                user.Fb,
                user.ProfileWebsite,
                user.ContactTime,
                user.SelfIntroduction,
                user.WorkState,
                user.Language,
                user.Company,
                user.Industry,
                user.Position,
                Skills = skillListAry,
                user.JobDescription
            };
            return Ok(new { status = "success", message = "會員資料", userdata = result });

        }

        ///// <summary>
        ///// 編輯大頭貼照片
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("EditProfilePic")]
        //[JwtAuthFilter]
        //public async Task<IHttpActionResult> EditProfilePic()
        //{
        //    // 檢查請求是否包含 multipart/form-data.
        //    if (!Request.Content.IsMimeMultipartContent())
        //    {
        //        throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
        //    }

        //    // 檢查資料夾是否存在，若無則建立
        //    string root = HttpContext.Current.Server.MapPath("~/Upload/ProfilePicture");
        //    if (!Directory.Exists(root))
        //    {
        //        Directory.CreateDirectory("~/Upload/ProfilePicture");
        //    }

        //    var data = JwtAuthUtil.GetUserList(Request.Headers.Authorization.Parameter);
        //    if (data == null)
        //    {
        //        return Ok(new { status = "error", message = "請重新登入" });
        //    }

        //    var account = data.Item2;
        //    var user = db.Members.FirstOrDefault(x => x.Account == account);

        //    ////刪除原有圖檔
        //    //string ProfilePic = user.ProfilePicture;
        //    //string savePath = System.Web.HttpContext.Current.Server.MapPath($"~/Upload/ProfilePicture/{ProfilePic}");

        //    //switch (ProfilePic)
        //    //{
        //    //    case "defalt-01.png":
        //    //    case "defalt-02.png":
        //    //    case "defalt-03.png":
        //    //    case "defalt-04.png":
        //    //    case "defalt-05.png":
        //    //    case "defalt-06.png":
        //    //    case "defalt-07.png":
        //    //    case "defalt-08.png":
        //    //        break;
        //    //    default:
        //    //        File.Delete(savePath);
        //    //        break;

        //    //}


        //    try
        //    {
        //        // 讀取 MIME 資料
        //        var provider = new MultipartMemoryStreamProvider();
        //        await Request.Content.ReadAsMultipartAsync(provider);

        //        // 取得檔案副檔名，單檔用.FirstOrDefault()直接取出，多檔需用迴圈
        //        string fileNameData = provider.Contents.FirstOrDefault().Headers.ContentDisposition.FileName.Trim('\"');
        //        string fileType = fileNameData.Remove(0, fileNameData.LastIndexOf('.')); // .jpg

        //        // 定義檔案名稱
        //        string fileName = user.NickName + "Profile" + DateTime.Now.ToString("yyyyMMddHHmmss") + fileType;

        //        // 儲存圖片，單檔用.FirstOrDefault()直接取出，多檔需用迴圈
        //        var fileBytes = await provider.Contents.FirstOrDefault().ReadAsByteArrayAsync();
        //        var outputPath = Path.Combine(root, fileName);
        //        using (var output = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
        //        {
        //            await output.WriteAsync(fileBytes, 0, fileBytes.Length);
        //        }


        //        return Ok(new
        //        {
        //            status = "success",
        //            message = "頭像更新成功",
        //            data = new
        //            {
        //                ProfilePicture = fileName
        //            }
        //        });
        //    }
        //    catch (Exception e)
        //    {
        //        return BadRequest(e.Message); // 400
        //    }
        //}

        /// <summary>
        /// 編輯會員資料(如有更換頭像，會在EditProfilePic的api取得回傳的頭貼新檔名，一起存入會員資料)
        /// </summary>
        /// <param name="Info">會員資料所需欄位</param>
        /// <returns></returns>
        [HttpPut]
        [Route("EditUserInfo")]
        [JwtAuthFilter]
        public IHttpActionResult EditUserInfo(EditUserInfon Info)
        {
            var data = JwtAuthUtil.GetUserList(Request.Headers.Authorization.Parameter);

            var account = data.Item2;
            var user = db.Members.FirstOrDefault(x => x.Account == account);
            var editUser = db.Members.FirstOrDefault(x=>x.Id==user.Id);

            string edit = "";
            string editSkills = "";
            if (Info.Skills != null)
            {
                StringBuilder builder = new StringBuilder();
                foreach (var value in Info.Skills)
                {
                    builder.Append(value);
                    builder.Append(',');
                }
                edit = builder.ToString();
                editSkills = edit.Trim(',');
            }
            else
            {
                editSkills = null;
            }

            editUser.NickName = Info.NickName;
            editUser.Gender = Info.Gender;
            editUser.ProfilePicture = Info.ProfilePicture;
            editUser.Ig = Info.Ig;
            editUser.Fb = Info.Fb;
            editUser.ProfileWebsite = Info.ProfileWebsite;
            editUser.ContactTime = Info.ContactTime;
            editUser.SelfIntroduction = Info.SelfIntroduction;
            editUser.WorkState = Info.WorkState;
            editUser.Language = Info.Language;
            editUser.Company = Info.Company;
            editUser.Industry = Info.Industry;
            editUser.Position = Info.Position;
            editUser.Skills = editSkills;
            editUser.JobDescription = Info.JobDescription;

            //foreach (var member in editUser)
            //{
            //    member.NickName = Info.NickName;
            //    member.Gender = Info.Gender;
            //    member.ProfilePicture = Info.ProfilePicture;
            //    member.Ig = Info.Ig;
            //    member.Fb = Info.Fb;
            //    member.ProfileWebsite = Info.ProfileWebsite;
            //    member.ContactTime = Info.ContactTime;
            //    member.SelfIntroduction = Info.SelfIntroduction;
            //    member.WorkState = Info.WorkState;
            //    member.Language = Info.Language;
            //    member.Company = Info.Company;
            //    member.Industry = Info.Industry;
            //    member.Position = Info.Position;
            //    member.Skills = editSkills;
            //    member.JobDescription = Info.JobDescription;
            //}

            db.SaveChanges();
            return Ok(new { status = "success", message = "會員資料成功修改" });
        }

        /// <summary>
        /// 發起人取得發起的專案列表(有分頁)
        /// </summary>
        /// <param name="page">目前所在頁數</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAddProject")]
        [JwtAuthFilter]
        public IHttpActionResult GetAddProject(int page)
        {
            var data = JwtAuthUtil.GetUserList(Request.Headers.Authorization.Parameter);

            var memberID = data.Item1;
            var user = db.Members.FirstOrDefault(x => x.Id == memberID);
            var projectList = user.Project.Where(x => x.ProjectState != "已刪除").ToList();
            var collects = db.Collections.Where(x => x.MembersId == memberID).ToList();

            List<GetProjectList> arrayList = new List<GetProjectList>();
            foreach (var content in projectList)
            {
                //讀出所選的技能列表
                string[] skillList = content.PartnerSkills.Split(',');
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
                //專案類型包含ID與類型別
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
                newProjectArray.ProjectTypeId = projectTypeAry;
                newProjectArray.InitDate = content.InitDate;
                newProjectArray.FinishedDeadline = content.FinishedDeadline;
                newProjectArray.GroupDeadline = content.GroupDeadline;
                newProjectArray.ProjectState = content.ProjectState;
                newProjectArray.PartnerSkills = skillListAry;
                newProjectArray.GroupNum = content.GroupNum;
                newProjectArray.GroupPhoto = content.GroupPhoto;
                newProjectArray.CollectOrNot = collectResult;

                arrayList.Add(newProjectArray);

            }

            int total = arrayList.Count;
            var dataCount = (page - 1) * 10;
            var ProjectLists = arrayList.OrderByDescending(x => x.InitDate).Skip(dataCount).Take(10);

            return Ok(new { status = "success", message = "發起的專案列表", page = page, datatotal = total, data = ProjectLists });

        }

        /// <summary>
        /// 會員取得參與的專案列表(有分頁)
        /// </summary>
        /// <param name="page">目前所在頁數</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAttendProject")]
        [JwtAuthFilter]
        public IHttpActionResult GetAttendProject(int page)
        {
            var data = JwtAuthUtil.GetUserList(Request.Headers.Authorization.Parameter);

            var memberID = data.Item1;
            var projectIdList = db.Applicant.Where(x => x.ApplicantState == "已通過" && x.MembersId == memberID && x.Project.ProjectState != "已刪除").Select(x => x.ProjectsId).ToList();
            var state = db.Applicant.Where(x => x.MembersId == memberID).ToList();
            var collects = db.Collections.Where(x => x.MembersId == memberID).ToList();

            var projectDetail = db.Projects;
            List<Projects> arrayList = new List<Projects>();
            List<GetProjectList> arrayProjectList = new List<GetProjectList>();
            foreach (var item in projectIdList)
            {
                var projectList = projectDetail.Where(x => x.Id == item).ToList();

                foreach (var content in projectList)
                {
                    //讀出所選的技能列表
                    string[] skillList = content.PartnerSkills.Split(',');
                    int[] skillListInt = skillList.Select(x => Convert.ToInt32(x.ToString())).ToArray();
                    ArrayList skillListAry = new ArrayList();
                    foreach (var skill in db.Skills)
                    {
                        foreach (var selectID in skillListInt)
                        {
                            if (selectID == skill.Id)
                            {
                                var resultdata = new
                                {
                                    skill.Id,
                                    skill.skill
                                };
                                skillListAry.Add(resultdata);
                            }
                        }
                    }
                    //專案類型包含ID與類型別
                    ArrayList projectTypeAry = new ArrayList();
                    foreach (var type in db.ProjectType)
                    {
                        if (type.Id == content.ProjectTypeId)
                        {
                            var resultdata = new
                            {
                                type.Id,
                                type.ProjectType
                            };
                            projectTypeAry.Add(resultdata);
                        }
                    }

                    //取得是否有收藏專案
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

                    foreach (var pId in projectList)
                    {
                        GetProjectList newProjectArray = new GetProjectList();

                        newProjectArray.Id = content.Id;
                        newProjectArray.ProjectName = content.ProjectName;
                        newProjectArray.ProjectContext = content.ProjectContext;
                        newProjectArray.ProjectTypeId = projectTypeAry;
                        newProjectArray.InitDate = content.InitDate;
                        newProjectArray.FinishedDeadline = content.FinishedDeadline;
                        newProjectArray.GroupDeadline = content.GroupDeadline;
                        newProjectArray.ProjectState = content.ProjectState;
                        newProjectArray.PartnerSkills = skillListAry;
                        newProjectArray.GroupNum = content.GroupNum;
                        newProjectArray.GroupPhoto = content.GroupPhoto;
                        newProjectArray.CollectOrNot = collectResult;
                        
                        arrayProjectList.Add(newProjectArray);
                    }

                }

            }

            int total = arrayProjectList.Count;
            var dataCount = (page - 1) * 10;
            var projectLists = arrayProjectList.OrderByDescending(x => x.InitDate).Skip(dataCount).Take(10);

            return Ok(new { status = "success", message = "參與中的專案列表", page = page, datatotal = total, data = projectLists });

        }

        /// <summary>
        /// 會員取得申請的專案列表(有分頁)
        /// </summary>
        /// <param name="page">目前所在頁數</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetApplyProject")]
        [JwtAuthFilter]
        public IHttpActionResult GetApplyProject(int page)
        {
            var data = JwtAuthUtil.GetUserList(Request.Headers.Authorization.Parameter);

            var memberID = data.Item1;
            var projectIdList = db.Applicant.Where(x => x.ApplicantState != "已退出" && x.MembersId == memberID && x.Project.ProjectState != "已刪除").Select(x => x.ProjectsId).ToList();
            var state = db.Applicant.Where(x => x.MembersId == memberID).ToList();
            var collects = db.Collections.Where(x => x.MembersId == memberID).ToList();
            
            if (projectIdList == null)
            {
                return Ok(new { status = "success", message = "此會員無申請的專案" });
            }


            var projectDetail = db.Projects;
            List<Projects> arrayList = new List<Projects>();
            List<GetApplyProjectList> arrayProjectList = new List<GetApplyProjectList>();
            foreach (var item in projectIdList)
            {
                var projectList = projectDetail.Where(x => x.Id == item).ToList();

                foreach (var content in projectList)
                {
                    //讀出所選的技能列表
                    string[] skillList = content.PartnerSkills.Split(',');
                    int[] skillListInt = skillList.Select(x => Convert.ToInt32(x.ToString())).ToArray();
                    ArrayList skillListAry = new ArrayList();
                    foreach (var skill in db.Skills)
                    {
                        foreach (var selectID in skillListInt)
                        {
                            if (selectID == skill.Id)
                            {
                                var resultdata = new
                                {
                                    skill.Id,
                                    skill.skill
                                };
                                skillListAry.Add(resultdata);
                            }
                        }
                    }
                    //專案類型包含ID與類型別
                    ArrayList projectTypeAry = new ArrayList();
                    foreach (var type in db.ProjectType)
                    {
                        if (type.Id == content.ProjectTypeId)
                        {
                            var resultdata = new
                            {
                                type.Id,
                                type.ProjectType
                            };
                            projectTypeAry.Add(resultdata);
                        }
                    }

                    //取得是否有收藏專案
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

                    foreach (var pId in projectList)
                    {
                        GetApplyProjectList newProjectArray = new GetApplyProjectList();
                        var applicantState = state.Where(x => x.ProjectsId == pId.Id).Select(x => x.ApplicantState).FirstOrDefault();

                        newProjectArray.Id = content.Id;
                        newProjectArray.ProjectName = content.ProjectName;
                        newProjectArray.ProjectContext = content.ProjectContext;
                        newProjectArray.ProjectTypeId = projectTypeAry;
                        newProjectArray.InitDate = content.InitDate;
                        newProjectArray.FinishedDeadline = content.FinishedDeadline;
                        newProjectArray.GroupDeadline = content.GroupDeadline;
                        newProjectArray.ApplicantState = applicantState;
                        newProjectArray.PartnerSkills = skillListAry;
                        newProjectArray.GroupNum = content.GroupNum;
                        newProjectArray.GroupPhoto = content.GroupPhoto;
                        newProjectArray.ProjectState = content.ProjectState;
                        newProjectArray.CollectOrNot = collectResult;
                        
                        arrayProjectList.Add(newProjectArray);
                    }

                }

            }

            int total = arrayProjectList.Count;
            var dataCount = (page - 1) * 10;
            var projectLists = arrayProjectList.OrderByDescending(x => x.InitDate).Skip(dataCount).Take(10);

            return Ok(new { status = "success", message = "申請的專案列表", page = page, datatotal = total, data = projectLists });

        }


        /// <summary>
        /// 會員取得收藏的專案列表(有分頁)
        /// </summary>
        /// <param name="page">目前所在頁數</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSaveProject")]
        [JwtAuthFilter]
        public IHttpActionResult GetSaveProject(int page)
        {
            var data = JwtAuthUtil.GetUserList(Request.Headers.Authorization.Parameter);

            var memberID = data.Item1;
            var projectIdList = db.Collections.Where(x => x.MembersId == memberID).Select(x => x.ProjectId).ToList();
            var collects = db.Collections.Where(x => x.MembersId == memberID).ToList();

            var projectDetail = db.Projects;
            List<Projects> arrayList = new List<Projects>();
            List<GetProjectList> arrayProjectList = new List<GetProjectList>();
            foreach (var item in projectIdList)
            {
                var projectList = projectDetail.Where(x => x.Id == item && !(x.ProjectState.Equals("已刪除"))).ToList();

                foreach (var content in projectList)
                {
                    //讀出所選的技能列表
                    string[] skillList = content.PartnerSkills.Split(',');
                    int[] skillListInt = skillList.Select(x => Convert.ToInt32(x.ToString())).ToArray();
                    ArrayList skillListAry = new ArrayList();
                    foreach (var skill in db.Skills)
                    {
                        foreach (var selectID in skillListInt)
                        {
                            if (selectID == skill.Id)
                            {
                                var resultdata = new
                                {
                                    skill.Id,
                                    skill.skill
                                };
                                skillListAry.Add(resultdata);
                            }
                        }
                    }
                    //專案類型包含ID與類型別
                    ArrayList projectTypeAry = new ArrayList();
                    foreach (var type in db.ProjectType)
                    {
                        if (type.Id == content.ProjectTypeId)
                        {
                            var resultdata = new
                            {
                                type.Id,
                                type.ProjectType
                            };
                            projectTypeAry.Add(resultdata);
                        }
                    }

                    //取得是否有收藏專案
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

                    foreach (var pId in projectList)
                    {
                        GetProjectList newProjectArray = new GetProjectList();

                        newProjectArray.Id = content.Id;
                        newProjectArray.ProjectName = content.ProjectName;
                        newProjectArray.ProjectContext = content.ProjectContext;
                        newProjectArray.ProjectTypeId = projectTypeAry;
                        newProjectArray.InitDate = content.InitDate;
                        newProjectArray.FinishedDeadline = content.FinishedDeadline;
                        newProjectArray.GroupDeadline = content.GroupDeadline;
                        newProjectArray.ProjectState = content.ProjectState;
                        newProjectArray.PartnerSkills = skillListAry;
                        newProjectArray.GroupNum = content.GroupNum;
                        newProjectArray.GroupPhoto = content.GroupPhoto;
                        newProjectArray.CollectOrNot = collectResult;
                        
                        arrayProjectList.Add(newProjectArray);
                    }

                }

            }

            int total = arrayProjectList.Count;
            var dataCount = (page - 1) * 10;
            var projectLists = arrayProjectList.OrderByDescending(x => x.InitDate).Skip(dataCount).Take(10);

            return Ok(new { status = "success", message = "收藏的的專案列表", page = page, datatotal = total, data = projectLists });

        }


        /// <summary>
        /// 會員取得通知的專案列表(有分頁)
        /// </summary>
        /// <param name="page">目前所在頁數</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetNotification")]
        [JwtAuthFilter]
        public IHttpActionResult GetNotification(int page)
        {
            var data = JwtAuthUtil.GetUserList(Request.Headers.Authorization.Parameter);

            var memberID = data.Item1;

            List<NoticeId> projectIdList = new List<NoticeId>();
            var applicantList = db.Applicant.Where(x => x.MembersId == memberID && x.ApplicantState.Equals("已通過")).ToList();
            foreach (var list in applicantList)
            {
                NoticeId pid = new NoticeId();
                pid.Id = list.ProjectsId;
                projectIdList.Add(pid);
            }

            var projectList = db.Projects.Where(x =>
                x.MembersId == memberID && !(x.ProjectState.Equals("已刪除")) && !(x.ProjectState.Equals("媒合中")) &&
                !(x.ProjectState.Equals("已關閉"))).ToList();
            foreach (var content in projectList)
            {
                NoticeId pid = new NoticeId();
                pid.Id = content.Id;
                projectIdList.Add(pid);
            }

            var projectDetail = db.Projects;
            List<GetNoticeList> arrayProjectList = new List<GetNoticeList>();

            foreach (var item in projectIdList)
            {
                GetNoticeList projectDeteil = new GetNoticeList();
                
                var arrayProjectLists = projectDetail.FirstOrDefault(x => x.Id == item.Id);

                projectDeteil.Id = arrayProjectLists.Id;
                projectDeteil.ProjectName = arrayProjectLists.ProjectName;
                projectDeteil.GroupPhoto = arrayProjectLists.GroupPhoto;

                arrayProjectList.Add(projectDeteil);
            }

            int total = arrayProjectList.Count;
            var dataCount = (page - 1) * 10;
            var projectLists = arrayProjectList.OrderByDescending(x => x.InitDate).Skip(dataCount).Take(10);

            return Ok(new { status = "success", message = "通知的專案列表", page = page, datatotal = total, data = projectLists });

        }

        /// <summary>
        /// 會員取得組員的聯絡方式
        /// </summary>
        /// <param name="projectId">專案ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetGroupMembers")]
        [JwtAuthFilter]
        public IHttpActionResult GetGroupMembers(int projectId)
        {
            var data = JwtAuthUtil.GetUserList(Request.Headers.Authorization.Parameter);
            var project = db.Projects.FirstOrDefault(x => x.Id == projectId);
            ArrayList member = new ArrayList();
            foreach (var item in db.Members)
            {
                if (item.Id == project.MembersId)
                {
                    var memberData = new
                    {
                        item.ProfilePicture,
                        item.NickName,
                        item.Account
                    };
                    member.Add(memberData);
                }
            }

            var applicantInfo = db.Members;
            var applicantIdList = db.Applicant.Where(x => x.ProjectsId == projectId && x.ApplicantState.Equals("已通過")).Select(x => x.MembersId)
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
                        info.NickName,
                        info.Account
                    };
                    applicants.Add(infoData);
                }
            }

            var result = new
            {
                Organizer = member,
                Applicants = applicants
            };

            return Ok(new { status = "success", message = "申請的專案列表", data = result });

        }
    }
}