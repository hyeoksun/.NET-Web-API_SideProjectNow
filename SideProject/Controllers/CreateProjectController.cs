using SideProject.JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SideProject.Models;
using SideProject.ViewModel;
using System.Threading.Tasks;
using System.Web;
using System.IO;
using System.Text;
using System.Collections;

namespace SideProject.Controllers
{
    public class CreateProjectController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        /// <summary>
        /// 新增專案(所有欄位必填)
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddProject")]
        [JwtAuthFilter]
        public IHttpActionResult AddProject(ProjectDetail content)
        {
            var data = JwtAuthUtil.GetUserList(Request.Headers.Authorization.Parameter);


            StringBuilder builder = new StringBuilder();
            foreach (var value in content.PartnerSkills)
            {
                builder.Append(value);
                builder.Append(',');
            }
            string edit = builder.ToString();
            string editPartnerSkills = edit.Trim(',');
            var userId = data.Item1;
            Projects project = new Projects();
            project.ProjectName = content.ProjectName;
            project.ProjectContext = content.ProjectContext;
            project.GroupPhoto = content.GroupPhoto;
            project.InitDate=DateTime.Now;
            project.GroupDeadline = DateTime.Now.AddDays(7);
            project.FinishedDeadline = content.FinishedDeadline;
            project.GroupNum = content.GroupNum;
            project.PartnerCondition = content.PartnerCondition;
            project.PartnerSkills = editPartnerSkills;
            project.ProjectTypeId = content.ProjectTypeId;
            project.ProjectState = "媒合中";
            project.MembersId = userId;

            db.Projects.Add(project);
            db.SaveChanges();
            
            int pId = project.Id;
            ProjectSkills skills = new ProjectSkills();
            foreach (var skillint in content.PartnerSkills)
            {
                skills.ProjectId = pId;
                skills.SkillId = skillint;
                db.ProjectSkills.Add(skills);
                db.SaveChanges();
            }
            return Ok(new { status = "success", message = "專案發起成功" });
        }


        /// <summary>
        /// 專案組別圖片上傳
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("UploadGroupPic")]
        [JwtAuthFilter]
        public async Task<IHttpActionResult> UploadGroupPic()
        {
            // 檢查請求是否包含 multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            // 檢查資料夾是否存在，若無則建立
            string root = HttpContext.Current.Server.MapPath("~/Upload/GroupPicture");
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory("~/Upload/GroupPicture");
            }

            try
            {
                // 讀取 MIME 資料
                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                //檢查組別圖片資料夾內有無同名檔案，有就加流水號
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
                    message = "組別圖片上傳成功",
                    data = new
                    {
                        ProfilePicture = fileName,
                        PicPath = $"~/Upload/GroupPicture/{fileName}"
                    }
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message); // 400
            }
        }


        /// <summary>
        /// 編輯專案資料(參加截止日僅顯示，如有更換組別圖片，會在UploadGroupPic的api取得回傳的圖片新檔名，一起存入會員資料)
        /// </summary>
        /// <param name="Id">專案ID</param>
        /// <param name="content">專案內容</param>
        /// <returns></returns>
        [HttpPut]
        [Route("EditProject")]
        [JwtAuthFilter]
        public IHttpActionResult EditProject(int Id, ProjectDetail content)
        {

            var projectExist = db.Projects.FirstOrDefault(x => x.Id == Id);
            var projectData = db.Projects.Where(x=>x.Id==Id);
            if (projectExist == null)
            {
                return Ok(new { status = "error", message = "查無專案" });
            }

            StringBuilder builder = new StringBuilder();
            foreach (var value in content.PartnerSkills)
            {
                builder.Append(value);
                builder.Append(',');
            }
            string edit = builder.ToString();
            string editPartnerSkills = edit.Trim(',');
            foreach (var item in projectData)
            {
                item.ProjectName = content.ProjectName;
                item.ProjectContext = content.ProjectContext;
                item.GroupPhoto = content.GroupPhoto;
                item.FinishedDeadline = content.FinishedDeadline;
                item.GroupNum = content.GroupNum;
                item.PartnerCondition = content.PartnerCondition;
                item.PartnerSkills = editPartnerSkills;
                item.ProjectTypeId = content.ProjectTypeId;
            }

            var project = db.Projects.FirstOrDefault(x => x.Id == Id);
            var skill = db.ProjectSkills.Where (x=>x.ProjectId==Id);

            foreach (var item in skill)
            {
                db.ProjectSkills.Remove(item);
            }

            db.SaveChanges();

            ProjectSkills skills = new ProjectSkills();
            foreach (var skillint in content.PartnerSkills)
            {
                skills.ProjectId = Id;
                skills.SkillId = skillint;
                db.ProjectSkills.Add(skills);
                db.SaveChanges();
            }

            return Ok(new { status = "success", message = "專案資料修改成功" });
        }
    }
}