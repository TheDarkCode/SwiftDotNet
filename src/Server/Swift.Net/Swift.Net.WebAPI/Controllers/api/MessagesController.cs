using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;


namespace Swift.Net.WebAPI.Controllers.api
{
    [BotAuthentication]
    [RoutePrefix("api/Messages")]
    public class MessagesController : ApiController
    {
        ///<summary>
        ///POST: api/Messages
        ///Receive a message from a user and reply to it
        ///</summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            if (activity.Type == ActivityTypes.Message)
            {
                // calculate something for us to return
                int length = (activity.Text ?? string.Empty).Length;

                // return our reply to the user
                Activity reply = activity.CreateReply($"You sent {activity.Text} which was {length} characters");
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {

            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
    }
}