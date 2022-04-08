using Jose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace SideProject.JWT
{
    /// <summary>
    /// 生成JwtToken
    /// </summary>
    public class JwtAuthUtil
    {
        /// <summary>
        /// 產生token
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="account">帳號</param>
        /// <param name="nickName">暱稱</param>
        /// <returns></returns>
        public string GenerateToken(int id,string account, string nickName)
        {
            string secret = "NowSideProject";//加解密的key,如果不一樣會無法成功解密
            Dictionary<string, Object> claim = new Dictionary<string, Object>();//payload 需透過token傳遞的資料
            claim.Add("Id", id);
            claim.Add("Account",account);
            claim.Add("NickName",nickName);
            claim.Add("iat", DateTime.Now.ToString());//建立時間
            claim.Add("Exp", DateTime.Now.AddSeconds(Convert.ToInt32("86400")).ToString());//Token 時效設定一天 
            var payload = claim;
            var token = Jose.JWT.Encode(payload, Encoding.UTF8.GetBytes(secret), JwsAlgorithm.HS512);//產生token
            return token;
        }
        /// <summary>
        /// 查詢使用者ID
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        public static int GetId(string Token)
        {
            string secert = "NowSideProject";//加解密的key,不一樣會無法成功解密
            var jwtObject = Jose.JWT.Decode<Dictionary<string, Object>>(
                Token,
                Encoding.UTF8.GetBytes(secert),
                JwsAlgorithm.HS512);
            return Convert.ToInt32(jwtObject["Id"]);
        }
        
        /// <summary>
        /// 查詢使用者Token內容
        /// </summary>
        /// <param name="Token">Token</param>
        /// <returns></returns>
        public static Tuple<int, string, string> GetUserList(string Token)
        {
            string secret = "NowSideProject";
            var jwtObject = Jose.JWT.Decode<Dictionary<string, Object>>(
                Token,
                Encoding.UTF8.GetBytes(secret),
                JwsAlgorithm.HS512);

            return Tuple.Create<int, string, string>(Convert.ToInt32(jwtObject["Id"]), jwtObject["Account"].ToString(), jwtObject["NickName"].ToString());


        }

    }
}