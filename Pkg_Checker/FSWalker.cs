using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Pkg_Checker
{
    static class FSWalker
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">the path to reaverse</param>
        /// <param name="isRecursive">recursive into each sub directories?</param>
        /// <param name="error">indicates whether an error occurred</param>
        /// <returns>a collection of file names</returns>
        public static List<String> Walk(String path, String filter, bool isRecursive, ref bool error)
        {
            List<String> FileNames = new List<String>();
            System.IO.FileInfo[] FileInfos = null;
            System.IO.DirectoryInfo[] SubDirInfos = null;
            DirectoryInfo di;
            FileAttributes attr;

            if (String.IsNullOrWhiteSpace(path))
                return null;

            try
            {
                attr = File.GetAttributes(path);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    di = new DirectoryInfo(path);

                    FileInfos = di.GetFiles(filter);  // case sensitive?
                    if (FileInfos != null)
                    {
                        foreach (FileInfo fileInfo in FileInfos)
                            FileNames.Add(fileInfo.FullName);
                    }

                    if (isRecursive)
                    {
                        SubDirInfos = di.GetDirectories();
                        if (SubDirInfos != null)
                        {
                            foreach (DirectoryInfo subdi in SubDirInfos)
                                foreach (String fileName in Walk(subdi.FullName, filter, isRecursive, ref error))
                                    FileNames.Add(fileName);
                        }
                    }
                }

                else
                {
                    // .pdf files are archives
                    FileNames.Add(path);
                    return FileNames;
                }
            }

            catch
            {
                error = true;
            }

            return FileNames;
        }
    }
}
