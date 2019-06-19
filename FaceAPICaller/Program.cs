using System;

namespace FaceAPICaller
{
    class Program
    {
        //directorys to monitor
        private static string directoriesToMonitor;

        //interval to capture
        private static double interval = 0;

        //use local container 
        private static bool useContainer = true;

        //delete local images
        private static bool deleteLocalImages = true;

        /// <summary>
        /// Main routine
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //show inicial instructions
            Instructions();

            //Create the manager 
            //remember to alter the Subscriptio key and FaceAPI endpoint, 
            //inside the manager
            using (Manager manager = new Manager(interval, useContainer, deleteLocalImages))
            {
                //Init the manager
                manager.Init(directoriesToMonitor);

                //inform to waith until exit
                Console.WriteLine(@"");
                Console.WriteLine("Press Enter to exit...");
                //wait until enter
                Console.ReadLine();

                Console.WriteLine(@"");
                Console.WriteLine(@"********************************************************************");
                Console.WriteLine(@" Sttoping ");
                Console.WriteLine(@"********************************************************************");
                //end the manager
                manager.End();
            }
        }

        /// <summary>
        /// Introduction screen and seetings
        /// </summary>
        private static void Instructions()
        {
            //welcome 
            Console.WriteLine(@"                      AZURE - COGNITIVE SERVICES                               ");
            Console.WriteLine(@"                          FaceAPI - Container                                  ");
            Console.WriteLine(@"                               ......                                          ");
            Console.WriteLine(@"                            .:||||||||:.                                       ");
            Console.WriteLine(@"                           /            \                                      ");
            Console.WriteLine(@"                          (   o      o   )                                     ");
            Console.WriteLine(@"------------------@@@@----------:  :----------@@@@-----------------------------");
            Console.WriteLine(@"|                                  GUID                                       |");
            Console.WriteLine(@"|  What to inform:                                                            |");
            Console.WriteLine(@"|      1. Inform a directory list as a TXT file to start monitor              |");
            Console.WriteLine(@"|      2. Inform RTSP capturer intervall                                      |");
            Console.WriteLine(@"|                                                                             |");
            Console.WriteLine(@"|  Obs: Each monitored directory, could have a rtsp file 'rtsp.txt'           |");
            Console.WriteLine(@"|       with the RTSP URL to be captured                                      |");
            Console.WriteLine(@"|                                                                             |");
            Console.WriteLine(@"|  How it works:                                                              |");
            Console.WriteLine(@"|      1. Train the Groups on Groups directory                                |");
            Console.WriteLine(@"|      2. Monitor all files on the informed directory's list                  |");
            Console.WriteLine(@"|      3. if exists 'rtsp.txt', grabb a frame from RTSP streamming (interval) |");
            Console.WriteLine(@"|      4. Try to recognize faces on the image                                 |");
            Console.WriteLine(@"|      5. Try to find faces on trained groups                                 |");
            Console.WriteLine(@"|      6.  Generate JSON with result                                          |");
            Console.WriteLine(@"|      7. Send JSON to Event-Hub                                              |");
            Console.WriteLine(@"|      9. Delete the image                                                    |");
            Console.WriteLine(@"|                                                                             |");
            Console.WriteLine(@"|  Goals:                                                                     |");
            Console.WriteLine(@"|      1. Running FaceAPI on container                                        |");
            Console.WriteLine(@"|      2. Send only JSON to Cloud                                             |");
            Console.WriteLine(@"|                                                                             |");
            Console.WriteLine(@"|  Config:                                                                    |");
            Console.WriteLine(@"|      1. Edit file: config.json setting your Azure data                      |");
            Console.WriteLine(@"|         - FaceApiSubscriptionKey: Azure Cognitive Services Face API KEY     |");
            Console.WriteLine(@"|         - FaceApiEndpointAzure: Face API endpoint created on Azure          |");
            Console.WriteLine(@"|         - FaceApiEndpointLocal: Face API endpoint on Docker container       |");
            Console.WriteLine(@"|         - EventHubConnectionString: Azure Event Hub ConnectionString        |");
            Console.WriteLine(@"|         - EventHubName: Azure Event Hub Name                                |");
            Console.WriteLine(@"|         - GroupPersonPath: Path to Group/Person directory                   |");
            Console.WriteLine(@"|         - FfmpegExe: ffmpeg.exe path                                        |");
            Console.WriteLine(@"|         - FfmpegArgs: ffmpeg ARGS used to grabb a frame from RTSP           |");
            Console.WriteLine(@"|                                                                             |");
            Console.WriteLine(@"|                                                                             |");
            Console.WriteLine(@"|  Important: Using ffmpeg (https://ffmpeg.org/) to capture RTSP              |");
            Console.WriteLine(@"|-----------------------------------------------------------------------------|");

            Console.WriteLine(@"");
            Console.WriteLine(@"Please inform:");

            // Get the file with the list of directories to be processed.
            //Each directory must be on one line
            Console.Write("    Enter the path to a file with directory's list to be monitored: ");
            directoriesToMonitor = Console.ReadLine();

            //Get the intervall to capture
            while (interval <= 0)
            {
                Console.Write("    Enter the interval (milisseconds) to grabb a frame from RTSP (default = 1000): ");
                var strInterval = Console.ReadLine();

                if (String.IsNullOrWhiteSpace(strInterval))
                    strInterval = "1000";

                if (!Double.TryParse(strInterval, out interval))
                {
                    Console.WriteLine(@"    Invalid interval");
                    Console.WriteLine(@"");
                }
            }

            Console.Write("    Use Local FaceAPI Container [Y/n]: ");
            var strContainer = Console.ReadLine();
            if (!String.IsNullOrWhiteSpace(strContainer) && strContainer.ToLowerInvariant() == "n")
                useContainer = false;

            Console.Write("    Delete local images [Y/n]: ");
            var strDeleteLocal = Console.ReadLine();
            if (!String.IsNullOrWhiteSpace(strDeleteLocal) && strDeleteLocal.ToLowerInvariant() == "n")
                deleteLocalImages = false;
        }
    }
}

