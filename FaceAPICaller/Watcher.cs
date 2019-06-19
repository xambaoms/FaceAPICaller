using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FaceAPICaller
{
    /// <summary>
    /// Monitors directoies to inspect file changes 
    /// and send them to FACEAPI, and EventHub
    /// </summary>
    public class Watcher : FileSystemWatcher
    {
        //count  running process 
        private int _runningProcess = 0;

        //Files to be monitored 
        private const string FILE_FILTER = "*.jpg";

        //Recognition object to communicate with FaceAPI
        private readonly Recognition recognition;

        //Sender object to send information to  EventHub
        private readonly EventSender eventSender;

        private readonly bool _deleteLocalImages;
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="path">Path to monitor</param>
        /// <param name="start">start immediattely</param>
        public Watcher(string path, bool useContainer, bool deleteLocalImages, bool start = false)
        {
            //check if directory exists
            if (!Directory.Exists(path))
                throw new ArgumentException($"Directory no exists: {path}");

            _deleteLocalImages = deleteLocalImages;

            //create recognition
            recognition = new Recognition(useContainer);

            //Create sender
            eventSender = new EventSender();

            //set the path to monitor
            this.Path = path;

            // Watch for changes in  LastWrite times, and
            // the renaming of files or directories.
            this.NotifyFilter = NotifyFilters.LastWrite
                                 | NotifyFilters.FileName
                                 | NotifyFilters.DirectoryName;

            // Only watch jpg files.
            this.Filter = FILE_FILTER;

            // Add event handlers.               
            this.Created += OnCreated; ;
            this.Deleted += OnDeleted;

            //check if need to start immediattely
            if (start)
                Run();
        }

        /// <summary>
        /// start the monitoring
        /// </summary>
        public void Run()
        {
            // Begin watching.
            this.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Stop the monitoring
        /// </summary>
        public void Stop()
        {
            //wait until end all the running process
            while (_runningProcess > 0)
                System.Threading.Thread.Sleep(100);

            // stop watching.
            this.EnableRaisingEvents = false;
        }

        /// <summary>
        /// Crreate event handler
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private async void OnCreated(object source, FileSystemEventArgs e)
        {
            //register the operation.
            Logger.Instance.Log($"File {e.ChangeType}: {e.FullPath}");

            //if other than create exit
            if (e.ChangeType != WatcherChangeTypes.Created)
                return;

            try
            {                

                //increment number of open process
                System.Threading.Interlocked.Increment(ref _runningProcess);


                //Recognize image
                var message = await recognition.Identify(e.FullPath, false);

                //if had detected faces send to event hub
                if (message.DetectedFaces != null && message.DetectedFaces.Count > 0)
                {                    

                    //convert to json
                    var json = JsonConvert.SerializeObject(message, Formatting.Indented);

                    await eventSender.Send(json);

                    //inform json
                    Logger.Instance.Log($"json: {e.FullPath}.json");

                    //output json to directory
                    File.WriteAllText($"{e.FullPath}.json", json);
                }
            }
            catch (Exception exception)
            {
                //inform exception
                Logger.Instance.Log($"Ex OnCreated: {exception.Message}");
            }
            finally
            {
                //delete the origina file
                if (_deleteLocalImages && File.Exists(e.FullPath))
                    File.Delete(e.FullPath);

                //increment number of open process
                System.Threading.Interlocked.Decrement(ref _runningProcess);
            }
        }

        /// <summary>
        /// Delete handler
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnDeleted(object source, FileSystemEventArgs e)
        {
            // Specify what is done when a file is changed, created, or deleted.
            Logger.Instance.Log($"File {e.ChangeType}: {e.FullPath}");
        }

        /// <summary>
        /// Dispose 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            //dispose sender
            if (eventSender != null)
                eventSender.Dispose();

            //dispose recognition
            if (recognition != null)
                recognition.Dispose();

            //dispose base
            base.Dispose(disposing);
        }
    }
}

