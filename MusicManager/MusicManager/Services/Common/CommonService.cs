using Google.Apis.Auth.OAuth2;
using MimeKit;
using MusicManager.Models;
using MusicManager.Repositories;
using Newtonsoft.Json;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;

namespace MusicManager.Services
{
    public class CommonService : ICommonService
    {
        private readonly IRepository<ApplicationUser> _repositoryUser;
        public CommonService(IRepository<ApplicationUser> repositoryUser)
        {
            _repositoryUser = repositoryUser;
        }
        public decimal ConvertDecimal(String input)
        {
            input = input.Replace(",", ".");
            decimal decimalValue = decimal.Parse(input, NumberStyles.Float, CultureInfo.InvariantCulture);
            return decimalValue;
        }
        public long GetNetSinger(object revenuePercentage, object value, string isEnterprise)
        {
            double revenue = revenuePercentage is string ? double.Parse((string)revenuePercentage) : Convert.ToDouble(revenuePercentage);
            double amount = Convert.ToDouble(value);

            double gross = (amount * 90 / 100) * revenue / 100;

            long net = isEnterprise == "True" ? (long)gross : (long)(gross * 90 / 100);

            return net;
        }
        public long GetNetEnterprise(object value)
        {
            double amount = Convert.ToDouble(value);

            long net = (long)(amount * 90 / 100);

            return net;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "JsonAuthenFCM", "mykind-19350-firebase-adminsdk-fbsvc-ba600518ef.json");
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var googleCredential = GoogleCredential.FromStream(stream)
                    .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");
                string rs = await googleCredential.UnderlyingCredential.GetAccessTokenForRequestAsync();
                return rs;
            }
        }
        public async Task SendNotificationToTopicAsync(string accessToken, string title, string body, string topic)
        {
            // URL endpoint FCM HTTP v1: thay "mykind-19350" bằng Project ID của bạn
            string url = "https://fcm.googleapis.com/v1/projects/mykind-19350/messages:send";

            // Tạo payload JSON theo cấu trúc của FCM HTTP v1
            var payload = new
            {
                message = new
                {
                    topic = topic,
                    notification = new
                    {
                        title = title,
                        body = body
                    }
                }
            };

            // Serialize payload sang JSON
            string jsonPayload = JsonConvert.SerializeObject(payload);

            using (var client = new HttpClient())
            {
                // Thiết lập header Authorization với Bearer token
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Tạo nội dung của request với kiểu "application/json"
                using (var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json"))
                {
                    // Gửi POST request đến FCM
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    string responseString = await response.Content.ReadAsStringAsync();
                }
            }
        }
        public async Task SendEmailAsync(List<string> toList, string subject, string body)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("MyKind", "contact@kindmedia.vn"));
            foreach (var email in toList)
            {
                emailMessage.To.Add(new MailboxAddress("", email));
            }
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using var client = new MailKit.Net.Smtp.SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync("contact@kindmedia.vn", "KindMusic@123");
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
        public async Task SendEmaiNoticationlAsync(string subject, string body)
        {
            List<String> dataUser = _repositoryUser.GetAll().Where(x=>x.IsAdmin == false && x.Email != null).Select(x=>x.Email).ToList();
            await SendEmailAsync(dataUser, subject, body);
        }

    }
}
