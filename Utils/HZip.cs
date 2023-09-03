using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using System;
using System.IO;

namespace HAtxLib.Utils {
    internal class HZip {

        public static bool UnzipTgz(string zipPath, string goalFolder) {
            Stream inStream = null;
            Stream gzipStream = null;
            TarArchive tarArchive = null;
            try {
                using (inStream = File.OpenRead(zipPath)) {
                    using (gzipStream = new GZipInputStream(inStream)) {
                        tarArchive = TarArchive.CreateInputTarArchive(gzipStream);
                        tarArchive.ExtractContents(goalFolder);
                        tarArchive.Close();
                    }
                }
                return true;
            } catch (Exception) {
                return false;
            } finally {
                tarArchive?.Close();
                gzipStream?.Close();
                inStream?.Close();
            }
        }

    }
}
