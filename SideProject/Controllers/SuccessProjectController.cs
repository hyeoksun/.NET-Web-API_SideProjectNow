using SideProject.JWT;
using SideProject.Models;
using SideProject.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace SideProject.Controllers
{
    public class SuccessProjectController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        /// <summary>
        /// 已完成專案的Banner與對應專案
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSussessProjectBanner")]
        public IHttpActionResult GetSussessProjectBanner()
        {

            var projects = from project in db.Projects where (project.ProjectState.Equals("已完成") && !project.ProjectState.Equals("已刪除")) select project;

            var ProjectList = projects.ToList();
            //List<GetProjectList> arrayList = new List<GetProjectList>();
            //foreach (var content in ProjectList)
            //{

            //    GetProjectList NewProjectArray = new GetProjectList();
            //    NewProjectArray.Id = content.Id;
            //    NewProjectArray

            //    //NewProjectArray.ProjectTypeId = content.ProjectTypeId;
            //    arrayList.Add(NewProjectArray);

            //}

            var ProjectLists = ProjectList.OrderByDescending(x => x.FinishedDeadline).Take(7);
            ArrayList result = new ArrayList();
            foreach (var item in ProjectLists)
            {
                var resultdata = new
                {
                    item.Id,
                    item.ProjectBanner
                };
                result.Add(resultdata);
            }
            return Ok(new { status = "success", message = "完成的專案Banner最新七筆", data = result });

        }


        /// <summary>
        /// 會員取得已完成專案列表(有分頁)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSussessProject")]
        [JwtAuthFilter]
        public IHttpActionResult GetSussessProject([FromUri] SelectProject data, int page)
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

            if (!string.IsNullOrWhiteSpace(data.skill))
            {
                projects = projects.Where(x => x.PartnerSkills.Contains(data.skill));
            }

            if (!string.IsNullOrWhiteSpace(data.keyword))
            {
                projects = projects.Where(x => x.ProjectName.Contains(data.keyword));
            }
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
            return Ok(new { status = "success", message = "完成的專案列表", page = page, datatotal = total, data = result });

        }

        /// <summary>
        /// 非會員取得已完成專案列表(有分頁)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSussessProjectGuest")]
        public IHttpActionResult GetSussessProjectGuest([FromUri] SelectProject data, int page)
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

            if (!string.IsNullOrWhiteSpace(data.skill))
            {
                projects = projects.Where(x => x.PartnerSkills.Contains(data.skill));
            }

            if (!string.IsNullOrWhiteSpace(data.keyword))
            {
                projects = projects.Where(x => x.ProjectName.Contains(data.keyword));
            }
            var ProjectList = projects.ToList();
            //var ProjectList = from project in db.Projects where project.MembersId == user.Id select project;
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

                //NewProjectArray.ProjectTypeId = content.ProjectTypeId;
                arrayList.Add(NewProjectArray);

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
            return Ok(new { status = "success", message = "完成的專案列表", page= page, datatotal = total, data = result });

        }
        
        /// <summary>
        /// 上傳專案Banner
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("UploadProjectBanner")]
        public async Task<IHttpActionResult> UploadProjectBanner()
        {
            // 檢查請求是否包含 multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            // 檢查資料夾是否存在，若無則建立
            string root = HttpContext.Current.Server.MapPath("~/Upload/Banner");
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory("~/Upload/Banner");
            }

            try
            {
                // 讀取 MIME 資料
                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                //檢查頭像資料夾內有無同名檔案，有就加流水號
                DirectoryInfo directoryInfo = new DirectoryInfo(root);
                // 取得檔案副檔名，單檔用.FirstOrDefault()直接取出，多檔需用迴圈
                string fileNameData = provider.Contents.FirstOrDefault().Headers.ContentDisposition.FileName.Trim('\"');
                string[] fileNameArry = fileNameData.Split('.');
                //string fileType = fileNameData.Remove(0, fileNameData.LastIndexOf('.')); // .jpg
                
                //檢查重複加流水號
                int count = 0;
                foreach (var item in directoryInfo.GetFiles())
                {
                    if (item.Name.Contains(fileNameArry[0]))
                    {
                        count++;
                    }
                }

                // 定義檔案名稱
                string fileName = fileNameArry[0] + $"({count})." + fileNameArry[1];

                // 儲存圖片，單檔用.FirstOrDefault()直接取出，多檔需用迴圈
                var fileBytes = await provider.Contents.FirstOrDefault().ReadAsByteArrayAsync();
                var outputPath = Path.Combine(root, fileName);
                using (var output = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                {
                    await output.WriteAsync(fileBytes, 0, fileBytes.Length);
                }


                return Ok(new
                {
                    status = "success",
                    message = "專案Banner上傳成功",
                    data = new
                    {
                        ProfilePicture = fileName,
                        PicPath = $"~/Upload/Banner/{fileName}"
                    }
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message); // 400
            }
        }


        /// <summary>
        /// 上傳專案完成圖片(多圖)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("UploadProjectPics")]
        public async Task<IHttpActionResult> UploadProjectPics()
        {
            // 檢查請求是否包含 multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            // 檢查資料夾是否存在，若無則建立
            string root = HttpContext.Current.Server.MapPath("~/Upload/FinishedPicture");
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory("~/Upload/FinishedPicture");
            }

            try
            {
                // 讀取 MIME 資料
                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                //檢查頭像資料夾內有無同名檔案，有就加流水號
                DirectoryInfo directoryInfo = new DirectoryInfo(root);
                // 取得檔案副檔名，單檔用.FirstOrDefault()直接取出，多檔需用迴圈

                string fileNameList = "";
                foreach (var item in provider.Contents)
                {
                    string fileNameData = item.Headers.ContentDisposition.FileName.Trim('\"');
                    string[] fileNameArry = fileNameData.Split('.');
                    
                    //檢查重複加流水號
                    int count = 0;
                    foreach (var file in directoryInfo.GetFiles())
                    {
                        if (file.Name.Contains(fileNameArry[0]))
                        {
                            count++;
                        }
                    }

                    // 定義檔案名稱
                    string fileName = fileNameArry[0] + $"({count})." + fileNameArry[1];

                    var fileBytes = await item.ReadAsByteArrayAsync();

                    var outputPath = Path.Combine(root, fileName);
                    using (var output = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                    {
                        await output.WriteAsync(fileBytes, 0, fileBytes.Length);
                    }

                    fileNameList += $"{fileName},";

                }
                return Ok(new
                {
                    status = "success",
                    message = "專案完成圖片上傳成功",
                    data = new
                    {
                        ProfilePicture = fileNameList.Trim(',')
                    }
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message); // 400
            }
        }


        /// <summary>
        /// 新增專案完成的資料(所有欄位必填，如有更換組別圖片，會在UploadGroupPic的api取得回傳的圖片新檔名，一起存入會員資料)
        /// </summary>
        /// <param name="ProjectId">專案ID</param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("CheckSussessProject")]
        public IHttpActionResult CheckSussessProject(int ProjectId, SuccessData data)
        {
            var projectData = from project in db.Projects where project.Id == ProjectId select project;
            foreach (var item in projectData)
            {
                item.GroupPhoto = data.GroupPhoto;
                item.ProjectName = data.ProjectName;
                item.ProjectBanner = data.ProjectBanner;
                item.ProjectPhotos = data.ProjectPhotos;
                item.ProjectWebsite = data.ProjectWebsite;
                item.ProjectExperience = data.ProjectExperience;
                item.ProjectState = "已完成";
            }

            db.SaveChanges();
            return Ok(new { status = "success", message = "專案完成資料送出成功" });
        }

        /// <summary>
        /// 取得完成的專案詳細內容
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSuccessProjectDetail")]
        public IHttpActionResult GetSuccessProjectDetail(int projectId)
        {
            var projectContentId = db.Projects.FirstOrDefault(x => x.Id == projectId);
            {
                //讀出技能列表
                string[] skillList = projectContentId.PartnerSkills.Split(',');
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
                    if (item.Id == projectContentId.ProjectTypeId)
                    {
                        var resultdata = new
                        {
                            item.Id,
                            item.ProjectType
                        };
                        projectTypeAry.Add(resultdata);
                    }
                }
                
                //成功圖片(多圖)
                string[] finishedPicsAry = projectContentId.ProjectPhotos.Split(',');
                ArrayList finishedPics = new ArrayList();
                foreach (var item in finishedPicsAry)
                {
                    var picturesData = new
                        {
                            item
                        };
                        finishedPics.Add(picturesData);
                }

                //發起人資訊
                ArrayList member = new ArrayList();
                var members = db.Members;
                foreach (var item in members)
                {
                    if (item.Id == projectContentId.MembersId)
                    {
                        var memberData = new
                        {
                            item.ProfilePicture,
                            item.NickName
                        };
                        member.Add(memberData);
                    }
                }

                //組員資訊
                var applicantInfo = db.Members;
                var applicantIdList = db.Applicant.Where(x => x.ProjectsId == projectContentId.Id).Select(x => x.MembersId).ToList();
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
                    projectContentId.Id,
                    projectContentId.ProjectBanner,
                    projectContentId.GroupPhoto,
                    projectContentId.ProjectName,
                    ProjectTypeId = projectTypeAry,
                    projectContentId.ProjectState,
                    projectContentId.InitDate,
                    projectContentId.FinishedDeadline,
                    projectContentId.GroupNum,
                    ProjectPictures= finishedPics,
                    projectContentId.ProjectContext,
                    projectContentId.ProjectExperience,
                    PartnerSkills = skillListAry,
                    Organizer = member,
                    Applicants = applicants
                };
                return Ok(new { status = "success", message = "已完成專案的詳細資料", userdata = result });
            }
        }
    }
}