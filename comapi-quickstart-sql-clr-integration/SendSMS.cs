using Newtonsoft.Json;
using RestSharp;
using System;
using System.Data.SqlTypes;

public class ComapiOneApi
{
    // **** Enter you API Space and security token details here ****
    private const string APISPACE = "{ENTER YOUR API SPACE ID HERE}";
    private const string TOKEN = "{ENTER YOUR ACCESS TOKEN FOR SMS SENDING HERE}";

    //////////////////
    // SQL CLR Methods
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static void Send(SqlString phoneNumber, SqlString from, SqlString message)
    {
        // Set the channel options; optional step, comment out to use a local number to send from automatically
        var myChannelOptions = new SMSSendRequest.channelOptionsStruct();
        myChannelOptions.sms = new SMSSendRequest.smsChannelOptions() { from = from.Value, allowUnicode = true };

        // Send the messages
        var myRequest = new SMSSendRequest();
        myRequest.to = new SMSSendRequest.toStruct(phoneNumber.Value);
        myRequest.body = message.Value;
        myRequest.channelOptions = myChannelOptions;

        // Send it.
        SendSMS(myRequest);
    }

    //////////////////
    // Private methods
    private static void SendSMS(SMSSendRequest smsRequest)
    {
        // Setup a REST client object using the message send URL with our API Space incorporated
        var client = new RestClient(string.Format("https://api.comapi.com/apispaces/{0}/messages", APISPACE));
        var request = new RestRequest(Method.POST);
        request.AddHeader("cache-control", "no-cache");
        request.AddHeader("content-type", "application/json");
        request.AddHeader("accept", "application/json");
        request.AddHeader("authorization", "Bearer " + TOKEN); // Add the security token

        // Serialise our SMS request object to JSON for submission
        string requestJson = JsonConvert.SerializeObject(smsRequest, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        request.AddParameter("application/json", requestJson, ParameterType.RequestBody);

        // Make the web service call
        IRestResponse response = client.Execute(request);

        if (response.StatusCode != System.Net.HttpStatusCode.Created | response.ErrorException != null)
        {
            // Something went wrong.
            if (response.ErrorException != null)
            {
                throw new InvalidOperationException(string.Format("Call to Comapi failed with error: {0}", response.ErrorException.ToString()));
            }
            else
            {
                throw new InvalidOperationException(string.Format("Call to Comapi failed with status code ({0}), and body: {1}", response.StatusCode, response.Content));
            }
        }
    }

    /// <summary>
    /// This object represents an SMS send for the Comapi "One" API
    /// </summary>
    public class SMSSendRequest
    {
        public SMSSendRequest()
        {
            // Default the Comapi channel rules to SMS
            this.rules = new string[] { "sms" };
        }

        #region "Structs"
        public struct toStruct
        {
            public toStruct(string mobileNumber)
            {
                this.phoneNumber = mobileNumber;
            }

            /// <summary>
            /// The phone number you want to send to in international format e.g. 447123123123
            /// </summary>
            public string phoneNumber;
        }

        public struct smsChannelOptions
        {
            /// <summary>
            /// The originator the SMS is from, this could be a phone number, shortcode or alpha
            /// </summary>
            public string from;

            /// <summary>
            /// Flag to indicate whether unicode messages are allowed to be sent
            /// </summary>
            public bool? allowUnicode;
        }

        public struct channelOptionsStruct
        {
            /// <summary>
            /// The SMS channels options
            /// </summary>
            public smsChannelOptions sms;
        }
        #endregion

        /// <summary>
        /// The SMS message body
        /// </summary>
        public string body { get; set; }

        /// <summary>
        /// The addressing information
        /// </summary>
        public toStruct to { get; set; }

        /// <summary>
        /// The channel options for the request
        /// </summary>
        public channelOptionsStruct? channelOptions { get; set; }

        /// <summary>
        /// The Comapi API channel rules
        /// </summary>
        public string[] rules { get; set; }
    }

    /// <summary>
    /// Formats JSON to make it more readable.
    /// </summary>
    /// <param name="json"></param>
    /// <returns>Formatted JSON string</returns>
    private static string FormatJson(string json)
    {
        dynamic parsedJson = JsonConvert.DeserializeObject(json);
        return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
    }
}
