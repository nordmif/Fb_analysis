using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Facebook;
using Aylien.TextApi;
namespace FacebookPosts3
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthorization();
            HttpRuntime.UnloadAppDomain();
            
        }
        class Posts //class for Facebook posts
        {
            public string PostId { get; set; }
            public string PostStory { get; set; }
            public string PostMessage { get; set; }
            public string PostPicture { get; set; }

            public string UserId { get; set; }
            public string Time { get; set; }

        }
        private void CheckAuthorization()
        {
            string app_id = "468781663508248"; //facebook appid
            string app_secret = "9ea8499466e88a4802b3d316243eb03f"; //facebook app secret key
            string scope = "user_posts,manage_pages,publish_actions"; //facebook app scope (what it can do)
            string alyen_appid = "4089c763"; //alyen appid
            string alyen_key = "6267e37e267fc601f9ddc50405237f35"; //alyen app key
            if (Request["code"] == null) //if user is not authorised
            {
                Response.Redirect(string.Format(
                    "https://graph.facebook.com/oauth/authorize?client_id={0}&redirect_uri={1}&scope={2}",
                    app_id, Request.Url.AbsoluteUri, scope));
            }
            else
            {
                Dictionary<string, string> tokens = new Dictionary<string, string>();
                string url = string.Format("https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&scope={2}&code={3}&client_secret={4}",
                   app_id, Request.Url.AbsoluteUri, scope, Request["code"].ToString(), app_secret);
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                string access_token, vals2;
                int end;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string vals = reader.ReadToEnd();
                    //foreach (string token in vals.Split('&'))
                    //{
                    vals2 = vals;
                    end = vals.IndexOf("\"", 150); 
                        access_token = vals.Substring(17, end-17); //cutting token out of response
                    //}
                }
                //string access_token = tokens["access_token"];
                //string access_token1 = "EAAGqWryH1xgBABrHu3ZBFZBMwYluVxdy32a7jZCz3x4cQ0jVfWVfYOZA4871NrIED9sEZBA2ZCkZCVf1UhRC778mhJJVQ16fkoXU8sdZAg3eS0Gu5WoyMS5NYIasLCiGPcQEU8k7IRibZCe74jew9Fsg0cna1n8qwwcmkdEcV7N5yJwZDZD";
                var client = new FacebookClient(access_token); 

                //client.Post("/me/feed", new { message = "Hi" });
                dynamic result = client.Get("/cnn"); //address where data is got from
                List<Posts> postsList = new List<Posts>();
                Client alyen_client = new Client(alyen_appid, alyen_key);
                Sentiment sentiment;
                //all the posts and their information (like pictures and links) is strored in result.data not in result
                StreamWriter sw = new StreamWriter(HttpRuntime.BinDirectory + "out_cnn.txt");
                for (int i = 0; i < result.data.Count; i++)
                {
                    Posts posts = new Posts();
                    posts.PostId = result.data[i].id;
                    if (object.ReferenceEquals(result.data[i].story, null))
                        posts.PostStory = "this story is null";
                    else
                        posts.PostStory = result.data[i].story;
                    if (object.ReferenceEquals(result.data[i].message, null))
                        posts.PostMessage = "this message is null";
                    else
                        posts.PostMessage = result.data[i].message;
                    posts.Time = Convert.ToString(result.data[i].updated_time);
                    postsList.Add(posts);
                    if (posts.PostMessage != "this message is null")
                    {
                        sentiment = alyen_client.Sentiment(text: posts.PostMessage);
                        sw.WriteLine(@"" + posts.PostMessage + @"" + ",");
                    }
                }
                sw.Close();
                //try
                //{
                //    dynamic result = client.Get("/me/posts");
                //    List<Posts> postsList = new List<Posts>();

                //    //all the posts and their information (like pictures and links) is strored in result.data not in result

                //    for (int i = 0; i < result.data.Count; i++)
                //    {
                //        Posts posts = new Posts();

                //        posts.PostId = result.data[i].id;
                //        if (object.ReferenceEquals(result.data[i].story, null))
                //            posts.PostStory = "this story is null";
                //        else
                //            posts.PostStory = result.data[i].story;
                //        if (object.ReferenceEquals(result.data[i].message, null))
                //            posts.PostMessage = "this message is null";
                //        else
                //            posts.PostMessage = result.data[i].message;

                //        posts.PostPicture = result.data[i].picture;
                //        posts.UserId = result.data[i].from.id;
                //        posts.UserName = result.data[i].from.name;

                //        postsList.Add(posts);
                //    }

                //}
                //catch (Exception)
                //{
                //    throw;
                //}

                
            }
        }
        protected void Page_Unload(object sender, EventArgs e)
        {

        }
    }
}