/*******************************************************************************
  * Copyright 2018 Esri
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

using System;
using System.Collections.Generic;

namespace Esri.ArcGISRuntime.ExampleApps.DataCollection.Shared.Utilities
{
    public static class FileExtensionHelper
    {
        /// <summary>
        /// Method to retrieve mime type from extension
        /// </summary>
        public static string GetTypeFromExtension(string extension)
        {
            if (extension == null)
            {
                throw new ArgumentNullException("extension");
            }

            if (!extension.StartsWith("."))
            {
                extension = "." + extension;
            }

            string mime;

            return AllowedExtensions.TryGetValue(extension, out mime) ? mime : "application/octet-stream";
        }

        /// <summary>
        /// Dictionary with extensions and mime type mappings
        /// </summary>
        public static IDictionary<string, string> AllowedExtensions = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            // TODO: Move these into a file
            {".bmp", "image/bmp"},
            {".ecw", "application/x-ImageWebServer-ecw"},
            {".emf", "application/emf"},
            {".eps", "application/postscript"},
            {".ps", "application/postscript"},
            {".gif", "image/gif"},
            {".img", "image/img"},
            {".jpc", "image/jp2"},
            {".j2k", "image/jp2"},
            {".jpf", "image/jpf"},
            {".jp2", "image/jp2"},
            {".jpg", "image/jpeg"},
            {".jpeg", "image/jpeg"},
            {".jpe", "image/jpeg"},
            {".png", "image/png"},
            {".psd", "application/octet-stream"},
            {".raw", "image/x-dcraw"},
            {".sid", "audio/x-psid"}, // not sure about this one
            {".tif", "image/tiff"},
            {".tiff", "image/tiff"},
            {".wmf", "windows/metafile"},
            {".wps", "application/vnd.ms-works"},
            {".avi", "video/avi"},
            {".mpg", "video/mpeg"}, // this could also be audio/mpeg, test it out
            {".mpe", "video/mpeg"},
            {".mpeg", "video/mpeg"},
            {".mov", "video/quicktime"},
            {".wmv", "video/x-ms-wmv"},
            {".aif", "audio/aiff"},
            {".mid", "audio/midi"},
            {".rmi", "audio/mid"},
            {".mp2", "video/mpeg"}, // this could also be audio/mpeg, test it out
            {".mp3", "video/mpeg"}, // this could also be audio/mpeg, test it out
            {".mp4", "video/mp4"},
            {".pma", "application/x-perfmon"},
            {".mp2v", "video/mpeg2"},
            {".qt", "video/quicktime"},
            {".ra", "audio/x-pn-realaudio"},
            {".ram", "audio/x-pn-realaudio"},
            {".wav", "audio/x-wav"},
            {".wma", "audio/x-ms-wma"},
            {".doc", "application/msword"},
            {".docx", "application/msword"},
            {".dot", "application/msword"},
            {".xls", "application/excel"},
            {".xlsx", "application/excel"},
            {".xlt", "application/excel"},
            {".pdf", "application/pdf"},
            {".ppt", "application/mspowerpoint"},
            {".pptx", "application/mspowerpoint"},
            {".txt", "text/plain"},
            {".zip", "application/x-zip-compressed"},
            {".7z", "application/x-7z-compressed"},
            {".gz", "application/x-gzip"},
            {".gtar", "application/x-gtar"},
            {".tar", "application/x-tar"},
            {".tgz", "application/gnutar"},
            {".vrml", "application/x-vrml"},
            {".gml", "application/gml+xml"},
            {".json", "application/json"},
            {".xml", "application/xml"},
            {".mdb", "application/msaccess"},
            {".geodatabase", "???"},
        };
    }
}
