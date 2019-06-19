using System;
using System.Collections.Generic;
using System.Text;

namespace FaceAPICaller
{
    /// <summary>
    /// Class to log 
    /// </summary>
    public sealed class Logger
    {
        //singleton
        private static Logger _instance;

        /// <summary>
        /// Instance
        /// </summary>
        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Logger();
                return _instance;
            }
        }

        /// <summary>
        /// ctor
        /// </summary>
        private Logger(){}

        /// <summary>
        /// Lotg Message
        /// </summary>
        /// <param name="message"></param>
        public void Log(String message)
        {            
            Console.WriteLine(message);
        }
    }
}
