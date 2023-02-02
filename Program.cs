using System.Net;
using System.Text;
namespace Httplistener
{
    class Program{
        static async Task  Main(string[] args){
            if(!HttpListener.IsSupported){
                Console.WriteLine ("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                throw new Exception("Not support");
            }

            HttpListener server = new HttpListener();
            server.Prefixes.Add("http://127.0.0.1:8081/");
            server.Start();
            Console.WriteLine("SERVER HTTP START...");
            do{
                var context = await server.GetContextAsync();
                Console.WriteLine("Client connected !");

                //Phản hồi khi người dùng kết nối vào server
                var respone = context.Response ;
                var outputStream = respone.OutputStream;

                respone.Headers.Add("content-type", "text/html"); //set header respone with text

                var htmlRespone = "<h1>Chao Louis team</h1>";
                var bytes = Encoding.UTF8.GetBytes(htmlRespone);
                await outputStream.WriteAsync(bytes, 0 ,bytes.Length);
                outputStream.Close();
            }while(server.IsListening);

        }
    }
}