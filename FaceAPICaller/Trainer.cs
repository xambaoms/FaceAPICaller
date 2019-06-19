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
    /// Trainer object to get files in directoryss and train the FaceAPI
    /// </summary>
    public class Trainer: FaceAPIBase
    {        

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="useContainer"></param>
        public Trainer(bool useContainer):base(useContainer)
        { }

        /// <summary>
        /// Train model based on directory structure
        /// </summary>
        /// <param name="path"></param>
        public async Task TrainModel(string path)
        {
            //Clear the person group
            PersonGroups.Clear();

            //check directory
            if (!Directory.Exists(path))
                throw new ArgumentException($"Trainer Group path does not exist: {path}");

            LogMessage($"Reading: {path}");
            //find groups
            foreach (var strPersonGroup in Directory.GetDirectories(path))
            {
                //Get group name
                var personGroupName = GetPathName(strPersonGroup);

                LogMessage($"    Creating Group: {personGroupName}");

                //try fing group
                var personGroup = (await faceClient.PersonGroup.ListAsync()).FirstOrDefault(g => g.Name == personGroupName);
                var personGroupId = String.Empty;

                //grouo not exists
                if (personGroup == null)
                {
                    //create ID
                    personGroupId = GetPathID(strPersonGroup);
                    //Create person Group
                    await faceClient.PersonGroup.CreateAsync(personGroupId, personGroupName);
                }
                else
                {
                    //asign group id
                    personGroupId = personGroup.PersonGroupId;
                }


                //Get Persons
                foreach (var strPerson in Directory.GetDirectories(strPersonGroup))
                {
                    //Get persons name     
                    var personName = GetPathName(strPerson);
                    LogMessage($"        Creating Person: {personName}");

                    //try get person
                    Person person = (await faceClient.PersonGroupPerson.ListAsync(personGroupId)).FirstOrDefault(p => p.Name == personName);

                    //create if not exists
                    if (person == null)
                        person = await faceClient.PersonGroupPerson.CreateAsync(personGroupId, personName);

                    //max 0f 10 faces
                    if (person.PersistedFaceIds != null && person.PersistedFaceIds.Count > 10)
                        continue;

                    //Get Faces
                    foreach (string imagePath in Directory.GetFiles(strPerson, "*.jpg"))
                    {
                        LogMessage($"            Creating Face: {imagePath}");
                        //read face
                        using (Stream s = File.OpenRead(imagePath))
                        {
                            //add do person
                            await faceClient.PersonGroupPerson.AddFaceFromStreamAsync(personGroupId, person.PersonId, s);
                        }
                    }
                }

                LogMessage($"    Treaining Group: {personGroupId}");
                //train model
                await faceClient.PersonGroup.TrainAsync(personGroupId);
            }

            PersonGroups.Clear();
            PersonGroups.AddRange(await faceClient.PersonGroup.ListAsync());
        }

        /// <summary>
        /// Get name from directory
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string GetPathName(string path)
        {
            return new DirectoryInfo(path).Name; ;
        }

        /// <summary>
        /// Get ID from file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string GetPathID(string path)
        {
            string strFile = Path.Combine(path, ID);

            if (!File.Exists(strFile))
                File.WriteAllText(strFile, Guid.NewGuid().ToString());

            return File.ReadAllText(strFile);
        }   
    }
}
