using BarbellTracker.DomainCode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarbellTracker.ApplicationCode
{
    public class FileManager
    {
        public static FileManager  Instance = new FileManager();

        public string FolderPath { get; set; }
        private string CurrendExtractionName;
        public string CruuentExtractionFolder => Path.Combine(FolderPath, CurrendExtractionName);


        public FileManager()
        {
            EventSystem.Subscribe(Event.StartExtractVideoInfo, NewExtractionStarted);
        }
        public async Task NewExtractionStarted(EventContext eventContext)
        {
            CurrendExtractionName = (eventContext.Arg as StartExtractionInformation).Id;
        }
        private string getTotalPath(string FileNameWithExtension)
        {
            return Path.Combine(CruuentExtractionFolder, FileNameWithExtension);
        }



        public bool Write(string FileNameWithExtension, string content, bool Override =true)
        {
            var totalPath = getTotalPath(FileNameWithExtension);
            try
            {

                if (File.Exists(totalPath) && !Override)
                {
                    return false;
                }

                File.WriteAllText(totalPath, content);
                return true;

            }catch (Exception ex)
            {
                return false;
            }

        }
        public bool Append(string FileNameWithExtension, string content)
        {
            try
            {

                var totalPath = getTotalPath(FileNameWithExtension);

                if (!File.Exists(totalPath))
                {
                    File.Create(totalPath);
                }

                File.AppendAllText(totalPath, content);
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }
        public bool AppendLine (string FileNameWithExtension, string content)
        {
            return Append(FileNameWithExtension, content + Environment.NewLine);
        }
        public bool ReadContent(string FileNameWithExtension, out string[] content)
        {
            content = new string[0];
            var totalPath = getTotalPath(FileNameWithExtension);

            if (!File.Exists(totalPath))
            {
                return false;
            }

            content = File.ReadAllLines(totalPath);
            return true;
        }

    }
}
