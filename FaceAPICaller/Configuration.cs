using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FaceAPICaller
{
    /// <summary>
    /// Get the configuration parameters
    /// </summary>
    public sealed class Configuration
    {
        //Configuration Root
        private readonly IConfigurationRoot config;

        //singleton
        private static Configuration _insrance;

        /// <summary>
        /// Instance
        /// </summary>
        public static Configuration Instance
        {
            get
            {
                if (_insrance == null)
                    _insrance = new Configuration();
                return _insrance;
            }
        }

        /// <summary>
        /// ctor
        /// </summary>
        private Configuration()
        {
            config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json",
                             optional: true,
                             reloadOnChange: true)
                .Build();
        }

        /// <summary>
        /// Subscrption Key do FaceAPI Cognitive Services, only necessary when using on AZURE
        /// </summary>
        public string FaceApiSubscriptionKey
        {
            get
            {
                return config["FaceApiSubscriptionKey"];
            }
        }

        /// <summary>
        /// Face API Endpoint, could be internal (running on container) or on Azure
        /// </summary>  
        public string FaceApiEndpointAzure
        {
            get
            {
                return config["FaceApiEndpointAzure"];
            }
        }

        /// <summary>
        /// Face API Endpoint, could be internal (running on container) or on Azure
        /// </summary>  
        public string FaceApiEndpointLocal
        {
            get
            {
                return config["FaceApiEndpointLocal"];
            }
        }

        /// <summary>
        /// Connection string to Event HUB
        /// </summary>
        public string EventHubConnectionString
        {
            get
            {
                return config["EventHubConnectionString"];
            }
        }

        /// <summary>
        /// Name do Event HUB
        /// </summary>
        public string EventHubName
        {
            get
            {
                return config["EventHubName"];
            }
        }


        /// <summary>
        /// Path to Groups and Persons to train the model
        /// directories structure:
        /// Group 
        ///   - Group1
        ///     - Person1
        ///     - Person2
        ///   -Group2
        ///     -Person3
        ///     -Person4
        /// </summary>
        public string GroupPersonPath
        {
            get
            {
                return config["GroupPersonPath"];
            }
        }

        /// <summary>
        /// Path to FFMPEG file
        /// </summary>
        public string FfmpegExe
        {
            get
            {
                return config["FfmpegExe"];
            }
        }

        /// <summary>
        /// Args used into FFMEG
        //   -hide_banner - ocults inicial ffmpeg banner
        ///  -loglevel quiet hide all output from ffmpeg
        ///  -i {0} Input RTSP streamming
        ///  -r numbers of frames to be captured
        ///  -t time for the capture
        ///  -s scale to 1920x1080
        ///  {1} output file
        /// </summary>
        public string FfmpegArgs
        {
            get
            {
                return config["FfmpegArgs"];
            }
        }

    }
}
