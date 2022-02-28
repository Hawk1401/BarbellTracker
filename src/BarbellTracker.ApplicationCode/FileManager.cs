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
        public string FolderPath { get; set; }
        private string CurrendExtractionName;
        public string CruuentExtractionFolder => Path.Combine(FolderPath, CurrendExtractionName);

        public FileManager()
        {
            EventSystem.Subscribe(Event.StartExtractVideoInfo, NewExtractionStarted);
            FolderPath = @"C:\Users\schal\Desktop\TracktInfos";
        }

        /// <summary>
        /// The Event handler for the Event Event.StartExtractVideoInfo
        /// </summary>
        /// <param name="eventContext">The eventContext from the event system, the type of eventContext.Arg is StartExtractionInformation </param>
        /// <returns>Returns a Task that can be Awaited</returns>
        public async Task NewExtractionStarted(EventContext eventContext)
        {
            CurrendExtractionName = (eventContext.Arg as StartExtractionInformation).Id;
        }

        /// <summary>
        /// Create a path to a file based on the currend Extraction
        /// </summary>
        /// <param name="FileNameWithExtension">The name of the File with the needed Extension</param>
        /// <returns>The total Oath from the requested File</returns>
        private string getTotalPath(string FileNameWithExtension)
        {
            return Path.Combine(CruuentExtractionFolder, FileNameWithExtension);
        }


        /// <summary>
        /// Will Write the Content to the designated File.
        /// It will Create the file if it not Exist
        /// </summary>
        /// <param name="FileNameWithExtension">Only the name of the file With Extension</param>
        /// <param name="content">The content that will be written down into the file</param>
        /// <param name="Override">this parameter is set to true by default, it indicates whether the file should be overwritten if it already exists. if it is set to false, and the file already exists, nothing is done.</param>
        /// <returns>Returns whether the content could be written to the file or not.</returns>
        public bool Write(string FileNameWithExtension, string content, bool Override =true)
        {
            SetUpFolderSetUp(CruuentExtractionFolder);

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


        /// <summary>
        /// Will Write the all lines to the designated File.
        /// It will Create the file if it not Exist
        /// </summary>
        /// <param name="FileNameWithExtension">Only the name of the file With Extension</param>
        /// <param name="lines">The Lines that will be written down into the file</param>
        /// <param name="Override">this parameter is set to true by default, it indicates whether the file should be overwritten if it already exists. if it is set to false, and the file already exists, nothing is done.</param>
        /// <returns>Returns whether the content could be written to the file or not.</returns>
        public bool WriteAllLines(string FileNameWithExtension, List<string> lines, bool Override = true)
        {
            SetUpFolderSetUp(CruuentExtractionFolder);

            var totalPath = getTotalPath(FileNameWithExtension);
            try
            {

                if (File.Exists(totalPath) && !Override)
                {
                    return false;
                }

                File.WriteAllLines(totalPath, lines);
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }

        }

        /// <summary>
        /// Append A new Content To the requested File
        /// It will Create the file if it not Exist
        /// </summary>
        /// <param name="FileNameWithExtension">Only the name of the file With Extension</param>
        /// <param name="content">The content that need to be added</param>
        /// <returns></returns>
        public bool Append(string FileNameWithExtension, string content)
        {
            try
            {
                SetUpFolderSetUp(CruuentExtractionFolder);
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

        /// <summary>
        /// Append A new Line To the requested File
        /// It will Create the file if it not Exist
        /// </summary>
        /// <param name="FileNameWithExtension">Only the name of the file With Extension</param>
        /// <param name="content">The line that need to be added</param>
        /// <returns></returns>
        public bool AppendLine (string FileNameWithExtension, string content)
        {
            return Append(FileNameWithExtension, content + Environment.NewLine);
        }

        /// <summary>
        /// Read Content From a File
        /// </summary>
        /// <param name="FileNameWithExtension">Only the name of the file With Extension</param>
        /// <param name="content">The content from the File</param>
        /// <returns>Whether the file could be read</returns>
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

        private void SetUpFolderSetUp(string Path)
        {
            var p = Directory.CreateDirectory(Path);
        }
    }
}
