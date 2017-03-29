using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication1.Controllers
{
    public class LineChatController : ApiController
    {
        [HttpPost]
        public IHttpActionResult POST()
        {
            string ChannelAccessToken = "FXOh8n...請用你自己的 bot 的 channel access token...ilFU=";
            string replyToken = "";

            try
            {
                //取得 http Post RawData(should be JSON)
                string postData = Request.Content.ReadAsStringAsync().Result;
                //剖析JSON
                var ReceivedMessage = isRock.LineBot.Utility.Parsing(postData);
                replyToken = ReceivedMessage.events[0].replyToken;
                //回覆訊息
                string Message = "無言以對";

                //如果是圖片、音樂、影片類型的資料
                if (ReceivedMessage.events.FirstOrDefault().type == "message" &&
                    ReceivedMessage.events.FirstOrDefault().message.type.Trim().ToLower() == "image")
                {
                    //取得contentid
                    var LineContentID = ReceivedMessage.events.FirstOrDefault().message.id.ToString();
                    var filebody = isRock.LineBot.Utility.GetUserUploadedContent(LineContentID, ChannelAccessToken);
                    //建立唯一名稱
                    var filename = "/tempFolder/" + Guid.NewGuid() + ".png";
                    var path = System.Web.HttpContext.Current.Request.MapPath(filename);
                    //save
                    System.IO.File.WriteAllBytes(path, filebody);
                    //回覆訊息
                    Message = $"你上傳了檔案，位於 http://{System.Web.HttpContext.Current.Request.Url.Host}{filename}";
                }
                else
                {
                    //回覆訊息
                    Message = "你just說了:" + ReceivedMessage.events[0].message.text;
                }
                //回覆用戶
                isRock.LineBot.Utility.ReplyMessage(replyToken, Message, ChannelAccessToken);

                //回覆API OK
                return Ok();
            }
            catch (Exception ex)
            {
                //回覆訊息
                string Message = "我錯了，錯在 " + ex.Message;

                //回覆用戶
                isRock.LineBot.Utility.ReplyMessage(replyToken, Message, ChannelAccessToken);

                return Ok();
            }
        }
    }
}
