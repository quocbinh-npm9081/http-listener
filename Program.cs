using System.Net;
using System.Text;
using Newtonsoft.Json;
namespace Httplistener
{

    class Program{

        class MyHttpListener {
            private HttpListener listener;
            public MyHttpListener(string[] prefixes){
                if(!HttpListener.IsSupported){
                    Console.WriteLine ("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                    throw new Exception("Not support");
                }

                listener = new HttpListener();
                foreach(string prefix in prefixes) listener.Prefixes.Add(prefix);
                Console.WriteLine ("Server start ...");

            }
            public async Task Start(){
                this.listener.Start();
                do
                {
                    HttpListenerContext context = await listener.GetContextAsync();
                    Console.WriteLine($"[ {DateTime.Now.ToLongDateString()} ] Client connected ");
                    await ProcessRequest(context);
                } while (listener.IsListening);
            }

            public async Task ProcessRequest(HttpListenerContext context){
                HttpListenerResponse response = context.Response;
                HttpListenerRequest request = context.Request;                   
                var outputStream = response.OutputStream;
                //send message to teminal for test domain
                Console.BackgroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"{request.HttpMethod} - {request.RawUrl} - {request.Url.AbsolutePath} - {response.StatusCode.ToString()}");
                Console.ResetColor();
                //set headder response 
                var headers = context.Response.Headers;
                //setup routing
                switch(request.Url.AbsolutePath){
                    case "/" : 
                    {                       
                        //set headder response 
                        headers.Add("Content-Type", "text/html");
                        //send body 
                        string html = "<h1>Home louis team</h1>";
                        byte[] htmlBuffer = Encoding.UTF8.GetBytes(html); 
                        response.ContentLength64 = htmlBuffer.Length;
                        await outputStream.WriteAsync(htmlBuffer, 0 , htmlBuffer.Length);
                    }
                    break;
                    case "/members" : {
                        //set headder response 
                        headers.Add("Content-Type", "text/html");
                        string html = "<h1>Member louis team</h1>";
                        byte[] htmlBuffer = Encoding.UTF8.GetBytes(html); 
                        response.ContentLength64 = htmlBuffer.Length;
                        await outputStream.WriteAsync(htmlBuffer, 0 , htmlBuffer.Length);
                    }
                    break;
                    case "/json" : {
                        //set headder response 
                        headers.Add("Content-Type", "application/json");
                        var product = new {
                            Name= "samsung",
                            Price = 2000
                        };
                        var jsonResponse = JsonConvert.SerializeObject(product);
                        byte[] jsonResponseBuffer = Encoding.UTF8.GetBytes(jsonResponse);
                        response.ContentLength64 = jsonResponseBuffer.Length;
                        await outputStream.WriteAsync(jsonResponseBuffer, 0 , jsonResponseBuffer.Length);
                    }
                    break;
                    case "/image_natra" : {
                        //set headder response 
                        headers.Add("Content-Type", "image/jpg");
                        byte[] imageBuffer = await File.ReadAllBytesAsync("image_natra.jpg");
                        response.ContentLength64 = imageBuffer.Length;
                        await outputStream.WriteAsync(imageBuffer, 0 , imageBuffer.Length);
                    }
                    break;
                    default :
                    {
                        //send status code
                        response.StatusCode = (int) HttpStatusCode.NotFound;
                        Console.WriteLine(response.StatusCode.ToString());
                        string html = "<h1>Pages not found</h1>";
                        byte[] htmlBuffer = Encoding.UTF8.GetBytes(html); 
                        response.ContentLength64 = htmlBuffer.Length;
                        await outputStream.WriteAsync(htmlBuffer, 0 , htmlBuffer.Length);
                  
                    }
                    break;
                }             
                outputStream.Close();
            }


        }

        static  async Task Main(string[] args){
            MyHttpListener server = new MyHttpListener(new string[] {"http://127.0.0.1:8081/"} );
            await server.Start();

        }
    }
}