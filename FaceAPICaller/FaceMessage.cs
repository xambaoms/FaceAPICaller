using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Newtonsoft.Json;

namespace FaceAPICaller
{
    /// <summary>
    /// FaceAPI Detection Message
    /// </summary>
    public class FaceMessage
    {
        /// <summary>
        /// ctor
        /// </summary>
        public FaceMessage()
        {
            DetectedFaces = new List<DetectedFace>();
            IdentifyResults = new List<IdentifyResult>();
            CandidateGroups = new List<CandidateGroup>();
        }

        /// <summary>
        /// Identifier
        /// </summary>
        public Guid ID { get; set; }

        /// <summary>
        /// Origin file
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Date Time 
        /// </summary>
        public DateTime Data { get; set; }

        /// <summary>
        /// Inform that is used only for face detection, 
        /// not for identifing person
        /// </summary>
        public bool OnlyDetect { get; set; }

        /// <summary>
        /// List of detected faces
        /// </summary>
        public List<DetectedFace> DetectedFaces { get; set; }

        /// <summary>
        /// List of Identifies Candidates
        /// </summary>
        public List<IdentifyResult> IdentifyResults { get; set; }

        /// <summary>
        /// List of Candidates associated with groups
        /// </summary>
        public List<CandidateGroup> CandidateGroups { get; set; }

    }

    /// <summary>
    /// Candidate Group Association
    /// </summary>
    public class CandidateGroup
    {
        /// <summary>
        /// Person Group ID
        /// </summary>
        public string PersonGroupId { get; set; }

        /// <summary>
        /// Person Group Name
        /// </summary>
        public string PersonGroupName { get; set; }

        /// <summary>
        /// Person ID sama as Candidate ID
        /// </summary>
        public Guid PersonId { get; set; }

        /// <summary>
        /// Person Name
        /// </summary>
        public string PersonName { get; set; }

        /// <summary>
        /// Face ID
        /// </summary>
        public Guid FaceID { get; set; }
    }
}
