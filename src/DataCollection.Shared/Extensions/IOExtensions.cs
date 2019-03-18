/*******************************************************************************
  * Copyright 2019 Esri
  *
  *  Licensed under the Apache License, Version 2.0 (the "License");
  *  you may not use this file except in compliance with the License.
  *  You may obtain a copy of the License at
  *
  *  http://www.apache.org/licenses/LICENSE-2.0
  *
  *   Unless required by applicable law or agreed to in writing, software
  *   distributed under the License is distributed on an "AS IS" BASIS,
  *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  *   See the License for the specific language governing permissions and
  *   limitations under the License.
******************************************************************************/

using Esri.ArcGISRuntime.Mapping;
using System.IO;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Extensions
{
    /// <summary>
    /// Class containing extension methods for handling files and directories
    /// </summary>
    internal static class IOExtensions
    {
        /// <summary>
        /// DirectoryInfo extension to clear all files and folders in a directory
        /// </summary>
        internal static void ClearDirectory(this DirectoryInfo directoryInfo)
        {
            // if directory doesn't exist, just exit
            // this happens sometimes when user hits cancel
            // the GenerateOfflineMapJob.Cancel() sometimes removes the directory, sometimes doesn't 
            if (!directoryInfo.Exists)
            {
                return;
            }

            // delete all files in the specified directory
            foreach (FileInfo file in directoryInfo.EnumerateFiles())
            {
                file.Delete();
            }

            // delete all directories and subdirectories inside the specified directory
            foreach (DirectoryInfo dir in directoryInfo.EnumerateDirectories())
            {
                dir.Delete(true);
            }
        }
    }
}
