using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Newtonsoft.Json;

namespace FaceAPICaller
{
    /// <summary>
    /// Abstract class to deal with FACE API Cognitive Services
    /// </summary>
    public abstract class FaceAPIBase : IDisposable
    {
        //FaceA API Cleient
        protected readonly FaceClient faceClient;

        //ID file name
        protected const string ID = "id.txt";

        //Person Groups singleton
        private static List<PersonGroup> personGroups;

        /// <summary>
        /// PErson Group
        /// </summary>
        public static List<PersonGroup> PersonGroups
        {
            get
            {
                //create if not exists
                if (personGroups == null)
                    personGroups = new List<PersonGroup>();

                //return
                return personGroups;
            }

        }

        /// <summary>
        /// ctor
        /// </summary>
        public FaceAPIBase(bool useContainer = true)
        {
            //Create FAceAPI client object
            faceClient = new FaceClient(
                new ApiKeyServiceClientCredentials(Configuration.Instance.FaceApiSubscriptionKey),
                new HttpClient(), true)
            {
                Endpoint = useContainer ? Configuration.Instance.FaceApiEndpointLocal : Configuration.Instance.FaceApiEndpointAzure
            };

        }
         
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            //if exist dispose
            if (faceClient != null)
                faceClient.Dispose();
        }

        /// <summary>
        /// Log Message
        /// </summary>
        /// <param name="message"></param>
        protected void LogMessage(string message)
        {
            Logger.Instance.Log(message);
        }
    }
}
