using System;
using System.IO;
using System.Collections.Generic;


namespace FaceAPICaller
{
    /// <summary>
    /// Manage the process of detection
    /// </summary>
    public class Manager : IDisposable
    {
        //File Watcher list where images are dropped to process
        private readonly List<Watcher> _watchers;

        //capture RTSP
        private readonly Capturer _capturer;

        //use container
        private readonly bool _useContainer;

        private readonly bool _deleteLocalImages;
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="interval">in milisseconds</param>
        public Manager(double interval, bool useContainer, bool deleteLocalImages)
        {
            //Create List of diretory watchers
            _watchers = new List<Watcher>();

            //create capturer
            _capturer = new Capturer(interval);

            _useContainer = useContainer;

            _deleteLocalImages = deleteLocalImages;
        }

        /// <summary>
        /// Start the manager
        /// </summary>
        /// <param name="directoriesToMonitor"></param>
        public void Init(string directoriesToMonitor)
        {
            //check the file
            if (File.Exists(directoriesToMonitor))
            {
                try
                {
                    //run the routine to train the API 
                    Train(Configuration.Instance.GroupPersonPath);

                    //start monitoring of the directories
                    Run(directoriesToMonitor);
                }
                catch (Exception e)
                {
                    //show exception
                    Logger.Instance.Log($"ex: {e.Message}");
                }
            }
            else
            {
                //alert of wrong file
                Logger.Instance.Log($"Ex: Invalid file path.{directoriesToMonitor}");
            }
        }

        /// <summary>
        /// Train the Model to recognize Groups and persons
        /// </summary>
        /// <param name="pathToTrain"></param>
        private void Train(string pathToTrain)
        {
            //Create trainer object
            using (var trainer = new Trainer(_useContainer))
            {
                //Train the model, and wait
                trainer.TrainModel(pathToTrain).Wait();
            }
        }

        /// <summary>
        /// Monior directories to detect faces and recognize groups and persons
        /// </summary>
        /// <param name="directoriesToMonitor"></param>
        private void Run(string directoriesToMonitor)
        {
            //force to stop existing watchers
            End();

            //Get all directoriesnon file, one per line
            foreach (var directory in File.ReadAllLines(directoriesToMonitor))
            {
                //only monitor if exits
                if (!Directory.Exists(directory))
                    continue;

                //add a new watcher to list, inform to start immediattely
                _watchers.Add(new Watcher(directory, _useContainer, _deleteLocalImages, true));

                //print information
                Logger.Instance.Log($"Watching folder: {directory}");

                //Add camera do Capturer
                _capturer.AddCamera(directory);
            }

            //start capture only after watcher has been created
            _capturer.Start();
        }

        /// <summary>
        /// Stop the watchers
        /// </summary>
        public void End()
        {
            //stop the capturer
            _capturer.Stop();

            //wait until all process finishing
            for (int i = 0; i < 10; i++)
            {
                if (_capturer.HasRunningProcess())
                    System.Threading.Thread.Sleep(250);
                else
                    i = 10;
            }

            //clear the cameras
            _capturer.Cameras.Clear();

            //stop the watcher
            _watchers.ForEach(w => w.Stop());

            //dispose the objects
            _watchers.ForEach(w => w.Dispose());

            //clear the list
            _watchers.Clear();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (_capturer != null)
                _capturer.Dispose();
        }
    }
}
