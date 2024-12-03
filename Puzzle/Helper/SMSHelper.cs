using RestSharp;

namespace Helper
{
    public class SMSHelper
    {
        public static void Send(string Message, string Mobile)
        {
            var client = new RestClient("http://ippanel.com/api/select");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", "{\"op\" : \"send\"" +
                ",\"uname\" : \"Omranieh\"" +
                ",\"pass\":  \"MahdiGhasemi\"" +
                ",\"message\" : \"" + Message + "\"" +
                ",\"from\": \"5000125475\"" +
                ",\"to\" : [\"" + Mobile + "\"]}"
                , ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }

        public static void SendByPattern(string Mobile, string Name, string Code, string Url = null)
        {
            var client = new RestClient("http://ippanel.com/api/select");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Content-Type", "application/json");

            string X = "{\"op\" : \"pattern\"" +
                ",\"user\" : \"omranieh\"" +
                ",\"pass\":  \"MahdiGhasemi\"" +
                ",\"fromNum\" : \"+985000125475\"" +
                ",\"toNum\": \"" + Mobile + "\"" +
                ",\"patternCode\": \"" + Code + "\"";

            if (Url == null)
            {
                X += ",\"inputData\" : [{\"name\": \"" + Name + "\"}]}";
            }
            else
            {
                X += ",\"inputData\" : [{\"name\": \"" + Name + "\", \"vote-url\": \"" + Url + "\"}]}";
            }

            request.AddParameter("undefined", X, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }

        public static void SendByPattern(string Mobile, int verifyCode, string Code)
        {
            var client = new RestClient("http://ippanel.com/api/select");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Content-Type", "application/json");

            string X = "{\"op\" : \"pattern\"" +
                ",\"user\" : \"omranieh\"" +
                ",\"pass\":  \"MahdiGhasemi\"" +
                ",\"fromNum\" : \"+985000125475\"" +
                ",\"toNum\": \"" + Mobile + "\"" +
                ",\"patternCode\": \"" + Code + "\"";

            X += ",\"inputData\" : [{\"code\": \"" + verifyCode + "\"}]}";


            request.AddParameter("undefined", X, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }

        public static void GetCredit()
        {
            var client = new RestClient("http://ippanel.com/api/select");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", "{\"op\" : \"credit\",\"uname\" : \"Omranieh\",\"pass\":  \"MahdiGhasemi\"}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }

        public static void GetDelivery(int Id)
        {
            var client = new RestClient("http://ippanel.com/api/select");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", "{\"op\" : \"delivery\",\"uname\" : \"Omranieh\",\"pass\":  \"MahdiGhasemi\",\"uniqid\":  \"" + Id + "\"}", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
        }
    }
}
