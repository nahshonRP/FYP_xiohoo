using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Lesson12.Models;

namespace Lesson12.Controllers
{
    [Produces("application/json")]
    [Route("api/Azure")]
    public class AzureApiController : Controller
    {
        private const string SK_SPEECH_SERVICES = "0c75f41242ae4ab69d6c8d3b5a2c9d7f";
        private const string TOKEN_ENDPOINT = "https://southeastasia.api.cognitive.microsoft.com/sts/v1.0/issuetoken";
        private const string SERVICE_SPEECHTOTEXT_ENDPOINT = "https://southeastasia.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1?language=en-US";
        private static string _token = "";
        private static DateTime _tokenReceivedAt = DateTime.Now;

        public async Task<String> GetAccessToken()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", SK_SPEECH_SERVICES);
                UriBuilder uriBuilder = new UriBuilder(TOKEN_ENDPOINT);

                var result = await client.PostAsync(uriBuilder.Uri.AbsoluteUri, null);
                Console.WriteLine("Token Uri: {0}", uriBuilder.Uri.AbsoluteUri);
                return await result.Content.ReadAsStringAsync();
            }
        }

        // TODO Lesson12 Task 3 Examine code from part 1 to 5, write down what each part is doing in worksheet
        [HttpPost("SpeechToText")]
        public IActionResult Transcribe(IFormFile sound)
        {
            #region Part5
            if (sound != null)
            {
                var filePath = Path.Combine(Environment.CurrentDirectory, $"wwwroot\\SoundFiles\\{Path.GetFileName(sound.FileName)}");

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    sound.CopyTo(stream);
                }
                return SpeechToText(filePath);
            }
            else
                return BadRequest();
            #endregion 
        }

        private IActionResult SpeechToText(string filePath)
        {
            #region Part1
            // get access token if needed
            if (_token == "" || (DateTime.Now - _tokenReceivedAt).TotalMinutes > 9)
            {
                _token = GetAccessToken().Result;
                _tokenReceivedAt = DateTime.Now;
            }
            #endregion

            #region Part2
            UriBuilder uriBuilder = new UriBuilder(SERVICE_SPEECHTOTEXT_ENDPOINT);

            HttpWebRequest request = null;
            request = (HttpWebRequest)HttpWebRequest.Create(uriBuilder.Uri.AbsoluteUri);
            request.SendChunked = true;
            request.Accept = @"application/json;text/xml";
            request.Method = "POST";
            request.ProtocolVersion = HttpVersion.Version11;
            request.ContentType = @"audio/wav; codec=audio/pcm; samplerate=16000";
            request.Headers["Ocp-Apim-Subscription-Key"] = SK_SPEECH_SERVICES;
            request.Headers["Authorization"] = $"Bearer {GetAccessToken().Result}";
            #endregion

            #region part3
            // Send an audio file by 1024 byte chunks
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {

                /*
                * Open a request stream and write 1024 byte chunks in the stream one at a time.
                */
                byte[] buffer = null;
                int bytesRead = 0;
                using (Stream requestStream = request.GetRequestStream())
                {
                    /*
                    * Read 1024 raw bytes from the input audio file.
                    */
                    buffer = new Byte[checked((uint)Math.Min(1024, (int)fs.Length))];
                    while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        requestStream.Write(buffer, 0, bytesRead);
                    }

                    // Flush
                    requestStream.Flush();
                }
            }
            #endregion

            #region part4
            /*
             * Get the response from the service.
            */
            Console.WriteLine("Response:");
            string responseString = "";
            using (WebResponse response = request.GetResponse())
            {
                Console.WriteLine(((HttpWebResponse)response).StatusCode);

                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    responseString = sr.ReadToEnd();
                }
            }
            dynamic data = JsonConvert.DeserializeObject(responseString);

            return Ok(data.DisplayText);
            #endregion part4

        }

        [HttpGet("SpeechToText/Demo")]
        public IActionResult SpeechToTextDemo()
        {
            string fn = Path.Combine(Environment.CurrentDirectory, "wwwroot\\Demo\\sample.wav");
            return SpeechToText(fn);
        }

    }
}