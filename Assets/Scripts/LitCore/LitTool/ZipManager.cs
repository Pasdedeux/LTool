/*======================================
* 项目名称 ：Assets.Scripts.Controller
* 项目描述 ：
* 类 名 称 ：URPCamManager
* 类 描 述 ：
* 命名空间 ：Assets.Scripts.Controller
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2021/7/6 15:08:10
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Derek Liu 2021. All rights reserved.
*******************************************************************
======================================*/

#define No_USE_SHARPZIP
using LitFramework;
using LitFramework.Base;
using System;
using System.IO;

#if USE_SHARPZIP
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Checksum;
#endif
using UnityEngine;

namespace Assets.Scripts.LitCore.LitTool
{
    public class ZipManager 
    {
        /// <summary>
        /// Use english is better...Method that compress specified files inside a folder (non-recursive) into a zip file.
        /// </summary>
        /// <param name="addedFilePaths">eg: AssetPathManager.Instance.GetStreamAssetDataPath("tutorial", false)</param>
        /// <param name="outputFilePath">eg: AssetPathManager.Instance.GetStreamAssetDataPath("test1.zip", false)</param>
        /// <param name="password"></param>
        public static void CompressFileWithPassword(string[] addedFilePaths, string outputFilePath, string password = null)
        {
#if USE_SHARPZIP
                        FileInfo fi = new FileInfo(outputFilePath);
                        if (!Directory.Exists(fi.DirectoryName)) Directory.CreateDirectory(fi.DirectoryName);

                        FileStream fs = File.Create(outputFilePath);
                        using (ZipOutputStream zipStream = new ZipOutputStream(fs))
                        {
                            if (!string.IsNullOrEmpty(password))
                                zipStream.Password = password;
                            zipStream.SetLevel(9); // 压缩级别直接顶满
                            for (int i = 0; i < addedFilePaths.Length; i++)
                                CreateZipFiles(addedFilePaths[i], zipStream);
                            zipStream.Finish();
                            zipStream.Close();
                        }
#else
            var result = lzip.compress_File_List(9, outputFilePath, addedFilePaths, password: password); // 压缩直接顶满
            if (result != 1) throw new Exception();
#endif
        }

#if USE_SHARPZIP
                /// <summary>
                /// 递归压缩文件
                /// </summary>
                /// <param name="sourceFilePath">待压缩的文件或文件夹路径</param>
                /// <param name="zipStream">打包结果的zip文件路径。全路径包括文件名和.zip扩展名</param>
                private static void CreateZipFiles(string sourceFilePath, ZipOutputStream zipStream)
                {
                    Crc32 crc = new Crc32();
                    FileStream fileStream = File.OpenRead(sourceFilePath);
                    byte[] buffer = new byte[fileStream.Length];
                    fileStream.Read(buffer, 0, buffer.Length);
                    string tempFile = sourceFilePath.Substring(sourceFilePath.LastIndexOf("\\") + 1);
                    /////剔除掉非相对根目录  /test 之上的冗余路径
                    ///// 如传入的资源路径是 /Users/lc10278/Documents/Work/ziptest/Assets/test
                    ///// 需要把test之前的路径都剔除掉  _inputResRootFolder = "test"
                    //int ind = tempFile.LastIndexOf("/");
                    string realPath = tempFile;//.Substring(ind, tempFile.Length - ind);
                    ///realPath 就是  /test
                    ZipEntry entry = new ZipEntry(realPath);
                    entry.DateTime = DateTime.Now;
                    entry.Size = fileStream.Length;
                    fileStream.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    zipStream.PutNextEntry(entry);
                    zipStream.Write(buffer, 0, buffer.Length);
                }



                /// <summary>
                /// Method that compress all the files inside a folder (non-recursive) into a zip file.
                /// </summary>
                /// <param name="directoryPath">eg: AssetPathManager.Instance.GetStreamAssetDataPath("tutorial", false)</param>
                /// <param name="outputFilePath">eg: AssetPathManager.Instance.GetStreamAssetDataPath("test1.zip", false)</param>
                /// <param name="compressionLevel"></param>
                public static void CompressDirectoryWithPassword(string directoryPath, string outputFilePath, string password = null, int compressionLevel = 9)
                {
                    try
                    {
                        // Depending on the directory this could be very large and would require more attention
                        // in a commercial package.
                        string[] filenames = Directory.GetFiles(directoryPath);

                        // 'using' statements guarantee the stream is closed properly which is a big source
                        // of problems otherwise.  Its exception safe as well which is great.
                        using (ZipOutputStream OutputStream = new ZipOutputStream(File.Create(outputFilePath)))
                        {
                            // Define a password for the file (if providen)
                            // set its value to null or don't declare it to leave the file
                            // without password protection
                            if (!string.IsNullOrEmpty(password))
                                OutputStream.Password = password;

                            // Define the compression level
                            // 0 - store only to 9 - means best compression
                            OutputStream.SetLevel(compressionLevel);

                            byte[] buffer = new byte[4096];

                            foreach (string file in filenames)
                            {

                                // Using GetFileName makes the result compatible with XP
                                // as the resulting path is not absolute.
                                ZipEntry entry = new ZipEntry(Path.GetFileName(file));

                                // Setup the entry data as required.

                                // Crc and size are handled by the library for seakable streams
                                // so no need to do them here.

                                // Could also use the last write time or similar for the file.
                                entry.DateTime = DateTime.Now;
                                OutputStream.PutNextEntry(entry);

                                using (FileStream fs = File.OpenRead(file))
                                {

                                    // Using a fixed size buffer here makes no noticeable difference for output
                                    // but keeps a lid on memory usage.
                                    int sourceBytes;

                                    do
                                    {
                                        sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                        OutputStream.Write(buffer, 0, sourceBytes);
                                    } while (sourceBytes > 0);
                                }
                            }

                            // Finish/Close arent needed strictly as the using statement does this automatically

                            // Finish is important to ensure trailing information for a Zip file is appended.  Without this
                            // the created file would be invalid.
                            OutputStream.Finish();

                            // Close is important to wrap things up and unlock the file.
                            OutputStream.Close();

                            Console.WriteLine("Files successfully compressed");
                        }
                    }
                    catch (Exception ex)
                    {
                        // No need to rethrow the exception as for our purposes its handled.
                        Console.WriteLine("Exception during processing {0}", ex);
                    }
                }

                private static string GetPath()
                {
                    string path = "";
                    using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                    {
                        using (AndroidJavaObject currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity"))
                        {
                            path = currentActivity.Call<AndroidJavaObject>("getFilesDir").Call<string>("getCanonicalPath");
                        }
                    }
                    return path;
                }
#endif

        /// <summary>
        /// Extracts the content from a .zip file inside an specific folder.
        /// </summary>
        /// <param name="fileZipPath">eg: AssetPathManager.Instance.GetStreamAssetDataPath("test.zip", false)</param>
        /// <param name="outputFolder">AssetPathManager.Instance.GetStreamAssetDataPath("", false)</param>
        /// <param name="password"></param>
        public static void ExtractZipContent(string fileZipPath, string outputFolder, string password = null)
        {
#if USE_SHARPZIP

                        ZipFile file = null;
                        try
                        {
                            FileStream fs = File.OpenRead(fileZipPath);
                            file = new ZipFile(fs);

                            if (!string.IsNullOrEmpty(password))
                            {
                                // AES encrypted entries are handled automatically
                                file.Password = password;
                            }

                            foreach (ZipEntry zipEntry in file)
                            {
                                if (!zipEntry.IsFile)
                                {
                                    // Ignore directories
                                    continue;
                                }

                                string entryFileName = zipEntry.Name;
                                // to remove the folder from the entry:- entryFileName = Path.GetFileName(entryFileName);
                                // Optionally match entrynames against a selection list here to skip as desired.
                                // The unpacked length is available in the zipEntry.Size property.

                                // 4K is optimum
                                byte[] buffer = new byte[4096];
                                Stream zipStream = file.GetInputStream(zipEntry);

                                // Manipulate the output filename here as desired.
                                string fullZipToPath = Path.Combine(outputFolder, entryFileName);
                                string directoryName = Path.GetDirectoryName(fullZipToPath);

                                if (directoryName.Length > 0)
                                {
                                    Directory.CreateDirectory(directoryName);
                                }

                                // Unzip file in buffered chunks. This is just as fast as unpacking to a buffer the full size
                                // of the file, but does not waste memory.
                                // The "using" will close the stream even if an exception occurs.
                                using (FileStream streamWriter = File.Create(fullZipToPath))
                                {
                                    StreamUtils.Copy(zipStream, streamWriter, buffer);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            LDebug.LogError(" Zip Error >> " + e);
                        }
                        finally
                        {
                            if (file != null)
                            {
                                file.IsStreamOwner = true; // Makes close also shut the underlying stream
                                file.Close(); // Ensure we release resources
                            }
                        }
#else
            var result = lzip.decompress_File(fileZipPath, outputFolder, password: password);
            if (result != 1) throw new Exception();
#endif
        }

    }
}
