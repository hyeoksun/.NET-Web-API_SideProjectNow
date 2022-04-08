using SideProject.JWT;
using SideProject.Models;
using SideProject.PassSecurity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using SideProject.ViewModel;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Web;
using Microsoft.Ajax.Utilities;
using System.Data.Entity;

namespace SideProject.Controllers
{
    /// <summary>
    /// 註冊.登入.圖片上傳.技能列表
    /// </summary>
    public class CommonController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// 會員註冊
        /// </summary>
        /// <param name="data">註冊資料</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Signup")]
        public IHttpActionResult Signup(SignUp data)
        {
            var dataInfo = db.Members.FirstOrDefault(x => x.Account == data.Account);

            if (data != null && dataInfo == null)
            {
                PasswordWithSaltHasher withSaltHasher = new PasswordWithSaltHasher();
                HashWithSaltResult resultSha256 = withSaltHasher.HashWithSalt(data.Password, 64, SHA256.Create());
                Members member = new Members();
                member.Account = data.Account;
                member.Password = resultSha256.Digest;
                member.PasswordSalt = resultSha256.Salt;
                member.NickName = data.NickName;
                member.Gender = data.Gender;
                member.ProfilePicture = data.ProfilePicture;
                member.ContactTime = data.ContactTime;
                member.InitDate = DateTime.Now;
                db.Members.Add(member);
                db.SaveChanges();
                return Ok(new { status = "success", message = "註冊成功" });
            }
            else
            {
                return Ok(new { status = "error", message = "帳號已重複" });
            }
        }


        /// <summary>
        /// 會員登入
        /// </summary>
        /// <param name="data">data包含帳密</param>
        /// <returns></returns>

        [HttpPost]
        [Route("Login")]
        public IHttpActionResult testLogin(Login data)
        {

            var user = db.Members.FirstOrDefault(x => x.Account == data.Account);
            if (user != null)
            {
                string RightPassword = HashWithSaltResult(data.Password, user.PasswordSalt, SHA256.Create()).Digest.ToString();
                if (user.Password == RightPassword)
                {
                    JwtAuthUtil jwt = new JwtAuthUtil();


                    return Ok(new { status = "success", message = "成功登入", token = jwt.GenerateToken(user.Id,user.Account,user.NickName) });
                }
                else
                {
                    return Ok(new { status = "error", message = "密碼錯誤" });
                }
            }
            else
            {
                return Ok(new { status = "error", message = "無此帳號" });
            }

        }

        private HashWithSaltResult HashWithSaltResult(string password, string salt, HashAlgorithm hashAlgo)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] passwordAsBytes = Encoding.UTF8.GetBytes(password);
            List<byte> passwordWithSaltBytes = new List<byte>();
            passwordWithSaltBytes.AddRange(passwordAsBytes);
            passwordWithSaltBytes.AddRange(saltBytes);
            byte[] digestBytes = hashAlgo.ComputeHash(passwordWithSaltBytes.ToArray());
            return new HashWithSaltResult(Convert.ToBase64String(saltBytes), Convert.ToBase64String(digestBytes));
        }

 

        /// <summary>
        /// 大頭貼上傳
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("UploadProfile")]
        public async Task<IHttpActionResult> UploadProfile()
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
                    message = "頭像上傳成功",
                    data = new
                    {
                        ProfilePicture = fileName,
                        PicPath=$"~/Upload/ProfilePicture/{fileName}"
                    }
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message); // 400
            }
        }

        /// <summary>
        /// 收藏專案
        /// </summary>
        /// <param name="projectId">專案ID</param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddFavoriteProject")]
        [JwtAuthFilter]
        public IHttpActionResult AddFavoriteProject(int projectId)
        {
            var data = JwtAuthUtil.GetUserList(Request.Headers.Authorization.Parameter);
            var userID = data.Item1;
            var user = db.Members.FirstOrDefault(x => x.Id == userID);
            var project = db.Projects.FirstOrDefault(x => x.Id == projectId);
            Collection collect = new Collection();
            collect.MembersId = user.Id;
            collect.ProjectId = project.Id;
            collect.InitDate=DateTime.Now;

            db.Collections.Add(collect);
            db.SaveChanges();
            return Ok(new {status = "success", message = "收藏專案成功"});
        }

        /// <summary>
        /// 取消收藏專案
        /// </summary>
        /// <param name="projectId">專案ID</param>
        /// <returns></returns>
        [HttpPost]
        [Route("CancelFavoriteProject")]
        [JwtAuthFilter]
        public IHttpActionResult CancelFavoriteProject(int projectId)
        {
            var data = JwtAuthUtil.GetUserList(Request.Headers.Authorization.Parameter);
            var userID = data.Item1;
            var user = db.Members.FirstOrDefault(x => x.Id == userID);
            var project = db.Projects.FirstOrDefault(x => x.Id == projectId);
            var collect = from collectProject in db.Collections
                where (collectProject.MembersId == user.Id && collectProject.ProjectId == projectId)
                select collectProject;


            foreach (var item in collect)
            {
                db.Collections.Remove(item);
            }

            db.SaveChanges();
            return Ok(new { status = "success", message = "取消收藏專案" });
        }


        /// <summary>
        /// 顯示是否收藏專案(如果他有收藏，收藏按鈕用這支來判斷，會長不一樣)
        /// </summary>
        /// <param name="projectId">專案ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("PresentFavoriteProject")]
        [JwtAuthFilter]
        public IHttpActionResult PresentFavoriteProject(int projectId)
        {
            var data = JwtAuthUtil.GetUserList(Request.Headers.Authorization.Parameter);
            var userID = data.Item1;
            var user = db.Members.FirstOrDefault(x => x.Id == userID);
            var collect = db.Collections.FirstOrDefault(x => x.MembersId == user.Id && x.ProjectId==projectId);

            if (collect == null)
            {

                return Ok(new { status = false, message = "未收藏該專案" });
            }
            else
            {
                return Ok(new { status = true, message = "有收藏該專案" });
            }


        }

        /// <summary>
        /// 判斷時間更改狀態(update表單時間至少要前一天才會變動)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("UpdateProjectState")]
        public IHttpActionResult UpdateProjectState()
        {
            var updateTime = db.UpdateProjectStates.FirstOrDefault();
            var now = DateTime.Now.Date;
            var project = db.Projects.Where(x => x.FinishedDeadline.Year <= now.Year && x.FinishedDeadline.Month <= now.Month && x.FinishedDeadline.Day <= now.Day && x.ProjectState.Equals("進行中")).ToList();
            var group = db.Projects.Where(x => x.GroupDeadline.Year <= now.Year && x.GroupDeadline.Month <= now.Month && x.GroupDeadline.Day <= now.Day && x.ProjectState.Equals("媒合中")).ToList();
            var applicants = db.Applicant
                .Where(x => x.Project.GroupDeadline.Year <= now.Year && x.Project.GroupDeadline.Month<=now.Month&& x.Project.GroupDeadline.Day<=now.Day && x.ApplicantState.Equals("審核中")).ToList();
            if (updateTime == null)
            {
                UpdateProjectState update = new UpdateProjectState(); 
                update.UpdateTime=DateTime.Now;
                db.UpdateProjectStates.Add(update);
                db.SaveChanges();
                return Ok((new { status = "success", message = "建立第一次更新時間" }));
            }
            else
            {
                if (updateTime.UpdateTime.Date < now)
                {
                    updateTime.UpdateTime = now.AddDays(-1);
                    db.SaveChanges();
                }
                if (updateTime.UpdateTime.Date.AddDays(1)==now)
                {
                    updateTime.UpdateTime = now;
                    foreach (var item in project)
                    {
                        item.ProjectState = "已廢棄";
                    }
                    db.SaveChanges();
                    foreach (var content in group)
                    {
                        content.ProjectState = "已關閉";
                    }
                    db.SaveChanges();
                    foreach (var applicant in applicants)
                    {
                        applicant.ApplicantState = "未通過";
                    }
                    db.SaveChanges();
                    return Ok((new { status = "success", message = "更新專案與申請人的狀態成功" }));
                }
                else
                {
                    return Ok((new { status = "false", message = "沒有要更新的狀態" }));
                }
            }

        }


        /// <summary>
        /// 新增技能列表
        /// </summary>
        /// <param name="skills"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddSkill")]
        public IHttpActionResult AddSkills(string skills)
        {
            Models.Skills skill = new Skills();
            var repeat = db.Skills.FirstOrDefault(x => x.skill == skills);

            if (repeat != null)
            {
                return Ok(new { status = "error", message = "技能已存在" });
            }
            else
            {
                skill.skill = skills;
            }
            db.Skills.Add(skill);
            db.SaveChanges();
            return Ok(new {status = "success", message = "新增技能成功"});
        }
        
        /// <summary>
        /// 技能列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetSkills")]
        public IHttpActionResult Skills()
        {
            ArrayList result = new ArrayList();
            foreach (var item in db.Skills)
            {
                var resultdata = new
                {
                    item.Id,
                    item.skill,
                };
                result.Add(resultdata);
            }

            return Ok(new { status = "success", message = "技能列表", Skilldata = result });
        }

        /// <summary>
        /// 新增專案類別列表
        /// </summary>
        /// <param name="ProjectClass"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("AddProjectClass")]
        public IHttpActionResult AddProjectClass(string projectClass)
        {
            Models.ProjectClass Type = new ProjectClass();
            var repeat = db.ProjectType.FirstOrDefault(x => x.ProjectType == projectClass);

            if (repeat != null)
            {
                return Ok(new { status = "error", message = "專案類別已存在" });
            }
            else
            {
                Type.ProjectType = projectClass;
            }
            db.ProjectType.Add(Type);
            db.SaveChanges();
            return Ok(new { status = "success", message = "新增專案類別成功" });
        }

        /// <summary>
        /// 專案類別列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetProjectClass")]
        public IHttpActionResult GetProjectClass()
        {
            ArrayList result = new ArrayList();
            foreach (var item in db.ProjectType)
            {
                var resultdata = new
                {
                    item.Id,
                    item.ProjectType,
                };
                result.Add(resultdata);
            }

            return Ok(new { status = "success", message = "專案類別列表", Classdata = result });
        }
    }
    
}