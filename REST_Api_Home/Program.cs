using System;
using System.Net;
using System.Net.Http.Json;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;


//UPDATE(by id)
namespace REST_Api_Home
{
    public class Post
    {
        public int Id { get; set; }
        [JsonProperty("post_id")]
        public int PostId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Body { get; set; }
    }

    class Program
    {
        public static string CommentsUri = "public/v2/comments";

        public static RestRequest ReadById(int id)
        {
            RestRequest request = new RestRequest(CommentsUri + $"/{id}");
            return request;
        }

        public static RestRequest ReadByPage(int page)
        {
            RestRequest request = new RestRequest(CommentsUri + $"?page={page}");
            return request;
        }

        public static RestRequest Create(Post post)
        {
            RestRequest request = new RestRequest(CommentsUri);
            request.AddBody(post);
            request.Method = Method.Post;

            return request;
        }

        public static RestRequest Delete(int id)
        {
            RestRequest request = new RestRequest(CommentsUri + $"/{id}");
            request.Method = Method.Delete;

            return request;
        }

        //need to fix (added null parameters)
        public static RestRequest Update(Post post, int id)
        {
            RestRequest request = new RestRequest(CommentsUri + $"/{id}");
            request.AddBody(post);
            request.Method = Method.Put;

            return request;
        }

        static async Task Main(string[] args)
        {
            using var client = new RestClient("https://gorest.co.in/");
            client.Authenticator = new JwtAuthenticator("4b45f57ccf6dfaacc9721fd3515054a1aa42f82d6967dfacbde8b90ddb1b7e28");
            int input;
            do
            {
                Console.WriteLine("1 - Create");
                Console.WriteLine("2 - Read by id");
                Console.WriteLine("3 - Read by page");
                Console.WriteLine("4 - Delete by id");
                Console.WriteLine("5 - Update by id");
                Console.WriteLine("0 - Exit");
                input = Convert.ToInt32(Console.ReadLine());
                RestRequest request = null;
                switch (input)
                {
                    case 1:
                        request = Create(
                            new Post
                            {
                                Body = GetRandom(),
                                Email = GetRandomMail(),
                                Name = GetRandom(),
                                PostId = 1000
                            });
                        break;
                    case 2:
                        Console.Write("ID:");
                        int id = Convert.ToInt32(Console.ReadLine());
                        request = ReadById(id);
                        break;
                    case 3:
                        Console.Write("Page:");
                        int page = Convert.ToInt32(Console.ReadLine());
                        request = ReadByPage(page);
                        break;
                    case 4:
                        Console.Write("ID:");
                        id = Convert.ToInt32(Console.ReadLine());
                        request = Delete(id);
                        break;
                    case 5:
                        Console.Write("ID:");
                        id = Convert.ToInt32(Console.ReadLine());
                        var response1 = await client.ExecuteAsync(ReadById(id));
                        var body = JsonConvert.DeserializeObject<Post>(response1.Content);
                        RequestBodyChages(body);
                        request = Update(body, id);
                        break;
                    default:
                        return;
                }

                var response = await client.ExecuteAsync(request);
                Console.WriteLine(response.Content);
            } while (input != 0);

            // var responseBody = JsonSerializer.Deserialize<Post>(response.Content);

        }

        static Random random = new Random();
        static string GetRandom(int min = 5, int max = 25)
        {
            var length = random.Next(min, max);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append((char)random.Next('a', 'z'));
            }

            return sb.ToString();
        }

        static string GetRandomMail()
        {
            return GetRandom(5, 10) + "@gmail.com";
        }

        public static void RequestBodyChages(Post postContent)
        {
            Console.WriteLine("1 - change Name");
            Console.WriteLine("2 - change Email");
            Console.WriteLine("3 - change Body");
            var input = Convert.ToInt32(Console.ReadLine());
            switch (input)
            {
                case 1:
                    Console.WriteLine("Enter new Name");
                    postContent.Name = Console.ReadLine();
                    break;
                case 2:
                    Console.WriteLine("Enter new Email");
                    postContent.Email = Console.ReadLine();
                    break;
                case 3:
                    Console.WriteLine("Enter new Body");
                    postContent.Body = Console.ReadLine();
                    break;
                default:
                    return;
            }
        }
    }
}