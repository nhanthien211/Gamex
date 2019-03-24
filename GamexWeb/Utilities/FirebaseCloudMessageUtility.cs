using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using Newtonsoft.Json;

namespace GamexWeb.Utilities
{
    public class FirebaseCloudMessageUtility
    {
        private readonly string _endPoint = WebConfigurationManager.AppSettings.Get("FCMEndpoint");
        private readonly string _serverKey = WebConfigurationManager.AppSettings.Get("FirebaseKey");
        private readonly string _senderId = WebConfigurationManager.AppSettings.Get("FirebaseSenderId");

        public bool Successful { get; set; }
        public string Response { get; set; }
        public Exception Error { get; set; }

        public FirebaseCloudMessageUtility()
        {

        }


        public FirebaseCloudMessageUtility SendNotification(string exhibitionId, string exhibitionName, string exhibitionImage)
        {
            var result = new FirebaseCloudMessageUtility {Successful = true};
            try
            {
                WebRequest webRequest = WebRequest.Create(_endPoint);
                webRequest.Method = "post";
                //serverKey - Key from Firebase cloud messaging server  
                webRequest.Headers.Add($"Authorization: key={_serverKey}");
                //Sender Id - From firebase project setting  
                webRequest.Headers.Add($"Sender: id={_senderId}");

                webRequest.ContentType = "application/json";

                var body = new
                {
                    to = "/topics/NEW_EXHIBITION",
                    priority = "high",
                    content_available = true,
                    data = new
                    {
                        EXTRA_EX_NAME = exhibitionName,
                        EXTRA_EX_ID = exhibitionId,
                        EXTRA_EX_IMG = exhibitionImage
                    },
                    notification = new
                    {
                        title = "New exhibition: " + exhibitionName,
                        body = "Check it out now",
                        click_action = "openExhibitionDetail"
                    }
                };

                var postBody = JsonConvert.SerializeObject(body);
                var byteArray = Encoding.UTF8.GetBytes(postBody);
                webRequest.ContentLength = byteArray.Length;
                using (var dataStream = webRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (var webResponse = webRequest.GetResponse())
                    {
                        using (var dataStreamResponse = webResponse.GetResponseStream())
                        {
                            if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                string sResponseFromServer = tReader.ReadToEnd();
                                result.Response = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Successful = false;
                result.Response = null;
                result.Error = ex;
            }
            return result;
        }
    }
}