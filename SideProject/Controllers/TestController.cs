using SideProject.JWT;
using SideProject.Models;
using SideProject.PassSecurity;
using SideProject.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace SideProject.Controllers
{
    public class TestController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// 頭像上傳
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IHttpActionResult> ProfilePicture()
        {
            // 檢查請求是否包含 multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            // 檢查資料夾是否存在，若無則建立
            string root = HttpContext.Current.Server.MapPath("~/Upload/ProfilePicture");
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory("~/Upload/ProfilePicture");
            }

            //var UserAccount_ = HttpContext.Current.Request.Form.GetValues("Account");
            //string UserAccount = UserAccount_[0];
            //var dataAccount = db.Members.FirstOrDefault(x => x.Account == UserAccount);

            //if (dataAccount != null)
            //{
            //    return Ok(new {status = "error", message = "帳號已重複"});
            //}

            try
            {
                //找到formdata中key值為NickName的value值
                var name = HttpContext.Current.Request.Form.GetValues("NickName");
                string NickName = name[0];

                // 讀取 form data 資料
                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                // 取得檔案副檔名，單檔用.FirstOrDefault()直接取出，多檔需用迴圈
                string fileNameData = provider.Contents.FirstOrDefault().Headers.ContentDisposition.FileName.Trim('\"');
                string fileType = fileNameData.Remove(0, fileNameData.LastIndexOf('.')); // .jpg

                // 定義檔案名稱
                string fileName = "NickName" + "Profile" + DateTime.Now.ToString("yyyyMMddHHmmss") + fileType;

                // 儲存圖片，單檔用.FirstOrDefault()直接取出，多檔需用迴圈
                var fileBytes = await provider.Contents.FirstOrDefault().ReadAsByteArrayAsync();
                var outputPath = Path.Combine(root, fileName);
                using (var output = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                {
                    await output.WriteAsync(fileBytes, 0, fileBytes.Length);
                }

                return Ok("圖片上傳成功");

            }
            catch (Exception e)
            {
                return BadRequest(e.Message); // 400
            }
        }

        /// <summary>
        /// 發起人取得發起的專案列表(沒有分頁)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAddProjectNoPage")]
        [JwtAuthFilter]
        public IHttpActionResult GetAddProject()
        {
            var data = JwtAuthUtil.GetUserList(Request.Headers.Authorization.Parameter);

            var memberID = data.Item1;
            var user = db.Members.FirstOrDefault(x => x.Id == memberID);
            var projectList = user.Project.OrderByDescending(x=>x.InitDate).Where(x=>!(x.ProjectState.Equals("已刪除"))).ToList();
            var collects = db.Collections.Where(x => x.MembersId == memberID).ToList();

            var hasProject = db.Projects.FirstOrDefault(x => x.MembersId == memberID);
            if (hasProject == null)
            {
                return Ok(new { status = "error", message = "此會員無發起的專案" });
            }
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
                
                //讀出所選的專案類型ID與類別
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

                GetProjectList NewProjectArray = new GetProjectList();
                NewProjectArray.Id = content.Id;
                NewProjectArray.ProjectName = content.ProjectName;
                NewProjectArray.ProjectContext = content.ProjectContext;
                NewProjectArray.ProjectTypeId = projectTypeAry;
                NewProjectArray.InitDate = content.InitDate;
                NewProjectArray.FinishedDeadline = content.FinishedDeadline;
                NewProjectArray.GroupDeadline = content.GroupDeadline;
                NewProjectArray.ProjectState = content.ProjectState;
                NewProjectArray.PartnerSkills = skillListAry;
                NewProjectArray.GroupNum = content.GroupNum;
                NewProjectArray.GroupPhoto = content.GroupPhoto;
                NewProjectArray.CollectOrNot = collectResult;

                arrayList.Add(NewProjectArray);

            }

            int total = arrayList.Count;
            return Ok(new {status = "success", message = "發起的專案列表", datatotal = total, data = arrayList});
        }

        /// <summary>
        /// 會員取得參與的專案列表(沒有分頁)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAttendProjectNoPage")]
        [JwtAuthFilter]
        public IHttpActionResult GetAttendProjectNoPage()
        {
            var data = JwtAuthUtil.GetUserList(Request.Headers.Authorization.Parameter);

            var memberID = data.Item1;
            var projectIdList = db.Applicant.Where(x => x.ApplicantState.Equals("已通過") && x.MembersId == memberID && !(x.Project.ProjectState.Equals("已刪除"))).Select(x => x.ProjectsId).ToList();
            var state = db.Applicant.Where(x => x.MembersId == memberID).ToList();
            if (projectIdList == null)
            {
                return Ok(new { status = "success", message = "此會員無已通過並參與的專案" });
            }
            var collects = db.Collections.Where(x => x.MembersId == memberID).ToList();

            var projectDetail = db.Projects;
            List<Projects> arrayList = new List<Projects>();
            List<GetProjectList> arrayProjectList = new List<GetProjectList>();
            foreach (var item in projectIdList)
            {
                var projectList = projectDetail.Where(x => x.Id == item).OrderByDescending(x=>x.InitDate).ToList();

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
                        var applicantState = state.Where(x => x.ProjectsId == pId.Id).Select(x => x.ApplicantState).FirstOrDefault();

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
            return Ok(new { status = "success", message = "參與的專案列表", data = arrayProjectList });

        }

        /// <summary>
        /// 會員取得申請的專案列表(沒有分頁)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetApplyProjectNoPage")]
        [JwtAuthFilter]
        public IHttpActionResult GetApplyProjectNoPage()
        {
            var data = JwtAuthUtil.GetUserList(Request.Headers.Authorization.Parameter);

            var memberID = data.Item1;
            var projectIdList = db.Applicant.Where(x => !(x.ApplicantState.Equals("已退出")) && x.MembersId == memberID && !(x.Project.ProjectState.Equals("已刪除"))).Select(x => x.ProjectsId).ToList();
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
                        newProjectArray.CollectOrNot = collectResult;

                        arrayProjectList.Add(newProjectArray);
                    }

                }

            }

            return Ok(new { status = "success", message = "申請的專案列表", data = arrayProjectList });

        }


        /// <summary>
        /// 會員取得專案列表(沒有分頁)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllProjectNoPage")]
        [JwtAuthFilter]
        public IHttpActionResult GetAllProjectNoPage()
        {

            var projects = db.Projects.Where(x => !(x.ProjectState.Equals("已完成")) && !(x.ProjectState.Equals("已刪除")));

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
            var ProjectLists = arrayList.OrderByDescending(x => x.InitDate);
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

            return Ok(new { status = "success", message = "已完成外的專案列表", datatotal = total, data = result });

        }

        /// <summary>
        /// 非會員取得專案列表(不含已完成，沒有分頁)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllProjectGuestNoPage")]
        public IHttpActionResult GetAllProjectGuestNoPage()
        {

            var projects = db.Projects.Where(x=>!(x.ProjectState.Equals("已完成")) && !(x.ProjectState.Equals("已刪除")));

            var projectList = projects.OrderByDescending(x=>x.InitDate).ToList();
            //var ProjectList = from project in db.Projects where project.MembersId == user.Id select project;
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

                GetProjectList NewProjectArray = new GetProjectList();
                NewProjectArray.Id = content.Id;
                NewProjectArray.ProjectName = content.ProjectName;
                NewProjectArray.ProjectContext = content.ProjectContext;
                NewProjectArray.ProjectTypeId = projectTypeAry;
                NewProjectArray.InitDate = content.InitDate;
                NewProjectArray.FinishedDeadline = content.FinishedDeadline;
                NewProjectArray.GroupDeadline = content.GroupDeadline;
                NewProjectArray.ProjectState = content.ProjectState;
                NewProjectArray.PartnerSkills = skillListAry;
                NewProjectArray.GroupNum = content.GroupNum;
                NewProjectArray.GroupPhoto = content.GroupPhoto;

                arrayList.Add(NewProjectArray);

            }

            int total = arrayList.Count;
            ArrayList result = new ArrayList();
            foreach (var item in arrayList)
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
            return Ok(new { status = "success", message = "所有專案列表", datatotal = total, data = result });
        }


        /// <summary>
        /// 會員取得已完成專案列表(沒有分頁)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSuccessProjectNoPage")]
        [JwtAuthFilter]
        public IHttpActionResult GetSussessProjectNoPage()
        {

            var projects = db.Projects.Where(x => (x.ProjectState.Equals("已完成")));
            var ProjectList = projects.ToList();
            var token = JwtAuthUtil.GetUserList(Request.Headers.Authorization.Parameter);
            var userID = token.Item1;
            var user = db.Members.FirstOrDefault(x => x.Id == userID);
            var collects = db.Collections.Where(x => x.MembersId == userID).ToList();


            List<GetProjectList> arrayList = new List<GetProjectList>();
            foreach (var content in ProjectList)
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

                //讀取專案類別ID與類型
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

                GetProjectList NewProjectArray = new GetProjectList();
                NewProjectArray.Id = content.Id;
                NewProjectArray.ProjectName = content.ProjectName;
                NewProjectArray.ProjectContext = content.ProjectContext;
                NewProjectArray.ProjectTypeId = projectTypeAry;
                NewProjectArray.InitDate = content.InitDate;
                NewProjectArray.FinishedDeadline = content.FinishedDeadline;
                NewProjectArray.GroupDeadline = content.GroupDeadline;
                NewProjectArray.ProjectState = content.ProjectState;
                NewProjectArray.PartnerSkills = skillListAry;
                NewProjectArray.GroupNum = content.GroupNum;
                NewProjectArray.GroupPhoto = content.GroupPhoto;
                NewProjectArray.CollectOrNot = collectResult;

                //NewProjectArray.ProjectTypeId = content.ProjectTypeId;
                arrayList.Add(NewProjectArray);

            }

            int total = arrayList.Count;
            var ProjectLists = arrayList.OrderByDescending(x => x.InitDate);
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
            return Ok(new { status = "success", message = "完成的專案列表", datatotal = total, data = result });

        }

        /// <summary>
        /// 非會員取得已完成專案列表(沒有分頁)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSuccessProjectGuestNoPage")]
        public IHttpActionResult GetSussessProjectGuestNoPage([FromUri] SelectProject data, int page = 0)
        {

            var projects = db.Projects.Where(x => (x.ProjectState.Equals("已完成")));

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

            if (data.skill.HasValue)
            {
                projects = projects.Where(x => x.ProjectSkill.Any(y=>y.SkillId==data.skill));
            }



            if (!string.IsNullOrWhiteSpace(data.keyword))
            {
                projects = projects.Where(x => x.ProjectName.Contains(data.keyword));
            }

            var ProjectList = projects.OrderByDescending(x=>x.InitDate).ToList();

            List<GetProjectList> arrayList = new List<GetProjectList>();
            foreach (var content in ProjectList)
            {
                //讀出所選的技能列表
                string[] SkillList = content.PartnerSkills.Split(',');
                int[] SkillListInt = SkillList.Select(x => Convert.ToInt32(x.ToString())).ToArray();
                ArrayList SkillListAry = new ArrayList();
                foreach (var item in db.Skills)
                {
                    foreach (var SelectID in SkillListInt)
                    {
                        if (SelectID == item.Id)
                        {
                            var resultdata = new
                            {
                                item.Id,
                                item.skill
                            };
                            SkillListAry.Add(resultdata);
                        }

                    }
                }

                ArrayList ProjectTypeAry = new ArrayList();
                foreach (var item in db.ProjectType)
                {
                    if (item.Id == content.ProjectTypeId)
                    {
                        var resultdata = new
                        {
                            item.Id,
                            item.ProjectType
                        };
                        ProjectTypeAry.Add(resultdata);
                    }
                }

                GetProjectList NewProjectArray = new GetProjectList();
                NewProjectArray.Id = content.Id;
                NewProjectArray.ProjectName = content.ProjectName;
                NewProjectArray.ProjectContext = content.ProjectContext;
                NewProjectArray.ProjectTypeId = ProjectTypeAry;
                NewProjectArray.InitDate = content.InitDate;
                NewProjectArray.FinishedDeadline = content.FinishedDeadline;
                NewProjectArray.GroupDeadline = content.GroupDeadline;
                NewProjectArray.ProjectState = content.ProjectState;
                NewProjectArray.PartnerSkills = SkillListAry;
                NewProjectArray.GroupNum = content.GroupNum;
                NewProjectArray.GroupPhoto = content.GroupPhoto;

                arrayList.Add(NewProjectArray);

            }

            int total = arrayList.Count;
            ArrayList result = new ArrayList();
            foreach (var item in arrayList)
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
            return Ok(new { status = "success", message = "所有已完成專案列表", datatotal = total, data = result });

        }

        /// <summary>
        /// 會員取得收藏的專案列表(無分頁)
        /// </summary>
        /// <param name="page">目前所在頁數</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSaveProjectNoPage")]
        [JwtAuthFilter]
        public IHttpActionResult GetSaveProjectNoPage()
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

            return Ok(new { status = "success", message = "收藏的的專案列表", datatotal = total, data = arrayProjectList });

        }

    }
}