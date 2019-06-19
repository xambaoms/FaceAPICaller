using System;
using System.Text;
using Microsoft.Azure.EventHubs;
using System.Threading.Tasks;

namespace FaceAPICaller
{
    /// <summary>
    /// Class to send mesasges to event hub
    /// </summary>
    public class EventSender : IDisposable
    {
        //Event hub client
        private readonly EventHubClient eventHubClient;

        /// <summary>
        /// ctor
        /// </summary>
        public EventSender()
        {
            // Creates an EventHubsConnectionStringBuilder object from the connection string, and sets the EntityPath.
            // Typically, the connection string should have the entity path in it, but this simple scenario
            // uses the connection string from the namespace.
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(Configuration.Instance.EventHubConnectionString)
            {
                EntityPath = Configuration.Instance.EventHubName
            };

            //create eventhub client
            eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

        }

        /// <summary>
        /// Send message to EventHub
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public async Task Send(string json)
        {

            try
            {
                //Send message
                await eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(json)));
            }
            catch (Exception exception)
            {
                //inform exception
                Logger.Instance.Log($"Ex: {exception.Message}");
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public async void Dispose()
        {
            //if exists, dispose
            if (eventHubClient != null)
                await eventHubClient.CloseAsync();
        }
    }
}
