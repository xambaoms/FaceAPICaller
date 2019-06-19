using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Timers;

namespace FaceAPICaller
{
    /// <summary>
    /// Capture frames from RTSP streamings 
    /// and saves into specific directory
    /// </summary>
    public class Capturer : IDisposable
    {
        /// <summary>
        /// RTSP file under monitored directory with RTSP URL
        /// </summary>
        private const string RTSP_FILE = @"rtsp.txt";

        //count  running process 
        private static int _runningProcess = 0;

        //timer toexecute the capturer
        private readonly Timer _timer;

        //LCameras RTSP
        public Dictionary<string, Uri> Cameras { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="interval">milisseconds</param>
        public Capturer(double interval)
        {
            //crete camera list
            Cameras = new Dictionary<string, Uri>();

            //create timer
            _timer = new Timer(interval);

            //set interval
            _timer.Elapsed += _timer_Elapsed;
        }

        /// <summary>
        /// Return if there is running process
        /// </summary>
        /// <returns></returns>
        public bool HasRunningProcess()
        {
            return _runningProcess > 0;
        }

        /// <summary>
        /// Start timer
        /// </summary>
        public void Start()
        {
            //only start if there is cameras
            if (Cameras.Count == 0)
                return;

            //start
            _timer.Start();
        }

        /// <summary>
        /// Stop timer
        /// </summary>
        public void Stop()
        {
            //stop
            _timer.Stop();
        }


        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            //if exists dispose
            if (_timer != null)
                _timer.Dispose();
        }

        /// <summary>
        /// Add camera to list, look for a file "rtsp.txt"
        /// on directory with an RTSP URL
        /// use this information to monitor
        /// </summary>
        /// <param name="directory"></param>
        public void AddCamera(string directory)
        {
            //there is a rtsp.txt file on directory
            var rtspArq = Path.Combine(directory, RTSP_FILE);

            //Continue if not exists RTSP file
            if (!File.Exists(rtspArq))
                return;

            //read content 
            var rtspContent = File.ReadAllLines(rtspArq);

            //Exists only one line/camera
            if (rtspContent == null || rtspContent.Length != 1)
                return;

            //is URL
            if (!Uri.TryCreate(rtspContent[0], UriKind.Absolute, out Uri uriResult))
                return;

            //add to cameras
            Cameras.Add(directory, uriResult);
        }

        /// <summary>
        /// Event on timer interval
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //read all cameras
            foreach (var camera in Cameras)
            {
                try
                {
                    //increment number of open process
                    System.Threading.Interlocked.Increment(ref _runningProcess);

                    //create a new name
                    var file = Path.Combine(camera.Key, $"{Guid.NewGuid()}.jpg");

                    //process the streamming
                    ProcessRTSP(file, camera.Value.AbsoluteUri);
                }
                catch (Exception ex)
                {
                    //show error
                    Logger.Instance.Log(ex.Message);
                }
                finally
                {
                    //decrement number of open process
                    System.Threading.Interlocked.Decrement(ref _runningProcess);
                }
            }
        }

        /// <summary>
        /// Process RTSP treamming to get the jpg iage
        /// </summary>
        /// <param name="file">destination file name</param>
        /// <param name="rtsp">streaaming to be captured</param>
        private void ProcessRTSP(string file, string rtsp)
        {   
            Logger.Instance.Log($"Frame captured: {rtsp}");

            //create process to call ffmpeg
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                FileName = Configuration.Instance.FfmpegExe,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = String.Format(Configuration.Instance.FfmpegArgs, rtsp, file)
            };


            // Start the process with the info we specified.
            // Call WaitForExit and then the using statement will close.
            using (Process exeProcess = Process.Start(startInfo))
            {
                exeProcess.WaitForExit();
            }

        }
    }

}
