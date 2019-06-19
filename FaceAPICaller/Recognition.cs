using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Newtonsoft.Json;

namespace FaceAPICaller
{
    /// <summary>
    /// Class to recognize faces and persons inside an image
    /// </summary>
    public class Recognition : FaceAPIBase
    {
        //FAce attributes singleton
        private static List<FaceAttributeType> faceAttributeType;

        /// <summary>
        /// Face atributes
        /// </summary>
        public static IList<FaceAttributeType> FaceAttributes
        {
            get
            {
                if (faceAttributeType == null)
                {
                    faceAttributeType = new List<FaceAttributeType>();
                    faceAttributeType.Add(FaceAttributeType.Age);
                    faceAttributeType.Add(FaceAttributeType.Gender);
                    //faceAttributeType.Add(FaceAttributeType.HeadPose);
                    //faceAttributeType.Add(FaceAttributeType.Smile);
                    //faceAttributeType.Add(FaceAttributeType.FacialHair);
                    //faceAttributeType.Add(FaceAttributeType.Glasses);
                    faceAttributeType.Add(FaceAttributeType.Emotion);
                    //faceAttributeType.Add(FaceAttributeType.Hair);
                    //faceAttributeType.Add(FaceAttributeType.Makeup);
                    //faceAttributeType.Add(FaceAttributeType.Occlusion);
                    //faceAttributeType.Add(FaceAttributeType.Accessories);
                    //faceAttributeType.Add(FaceAttributeType.Blur);
                    //faceAttributeType.Add(FaceAttributeType.Exposure);
                    //faceAttributeType.Add(FaceAttributeType.Noise);
                }
                return faceAttributeType;
            }
        }

        /// <summary>
        /// ctor
        /// </summary>
        public Recognition(bool useContainer) : base(useContainer)
        {

        }

        /// <summary>
        /// Identify faces and persons inside a picture
        /// </summary>
        /// <param name="testImageFile"></param>
        /// <param name="onlyDetect"></param>
        /// <returns></returns>
        public async Task<FaceMessage> Identify(string testImageFile, bool onlyDetect = true)
        {
            //create return message
            FaceMessage message = new FaceMessage
            {
                ID = Guid.NewGuid(),
                Data = DateTime.Now,
                File = testImageFile,
                OnlyDetect = onlyDetect
            };

            LogMessage($"Detecting Face: {testImageFile}");
            //try read image
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    //read image
                    using (Stream s = File.OpenRead(testImageFile))
                    {
                        //detec faces
                        message.DetectedFaces.AddRange(
                            await faceClient.Face.DetectWithStreamAsync(
                                s, true, false, FaceAttributes));
                    }
                }
                catch (Exception)
                {
                    //probably lock problem with the image, so wait some time
                    Thread.Sleep(100);
                }
                finally
                {
                    //quit the loop
                    i = 10;
                }

            }

            //Only detect faces
            if (onlyDetect || message.DetectedFaces.Count == 0)
                return message;

            //get face id
            var faceIds = message.DetectedFaces.Select(face => (Guid)(face.FaceId ?? Guid.Empty));

            //Get interest groups
            if (PersonGroups.Count == 0)
                PersonGroups.AddRange(await faceClient.PersonGroup.ListAsync());

            //get id as list
            var ids = faceIds.ToList();

            //look at groups
            foreach (var personGroup in PersonGroups)
            {
                try
                {

                    LogMessage($"Group: {personGroup.Name}");

                    //look person groups
                    message.IdentifyResults.AddRange(await faceClient.Face.IdentifyAsync(ids, personGroup.PersonGroupId));

                    //Lok identify results 
                    foreach (var identifyResult in message.IdentifyResults)
                    {
                        LogMessage($"    Face: {identifyResult.FaceId}");

                        //look candidates
                        foreach (var candidate in identifyResult.Candidates)
                        {
                            LogMessage($"        Candidate({candidate.Confidence}): {candidate.PersonId}");

                            //try find person
                            var person = await faceClient.PersonGroupPerson.GetAsync(personGroup.PersonGroupId, candidate.PersonId);

                            //dont exist 
                            if (person == null)
                                continue;

                            //create person inside group return
                            message.CandidateGroups.Add(new CandidateGroup
                            {
                                FaceID = identifyResult.FaceId,
                                PersonGroupId = personGroup.PersonGroupId,
                                PersonGroupName = personGroup.Name,
                                PersonId = person.PersonId,
                                PersonName = person.Name
                            });
                            LogMessage($"            Person: {person.Name} ({personGroup.Name})");
                        }
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }

            }
            return message;
        }
    }
}
