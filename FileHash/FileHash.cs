using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace FileHash
{
    public interface IFileHash
    {
        string getFilenameAsHash(string fileAbsPath);
        void setFileHash(string fileAbsPath);
        void removeFileHash(string fileAbsPath);
        bool isFileHashed(string fileAbsPath);
    }

    public class FileHash : IFileHash
    {
        protected string hashSavedFilePath;
        protected HashSet<string> fileHash;
        protected static readonly int KEY = 0;
        protected static readonly int VALUE = 1;

        public FileHash()
        {
            this.hashSavedFilePath = @"C:\tmp\hash.txt";
            if (!File.Exists(this.hashSavedFilePath))
            {
                File.Create(this.hashSavedFilePath).Close();
            }
            this.refreshInternalHash();
        }

        public string getFilenameAsHash(string fileAbsPath)
        {
            this.refreshInternalHash();
            string gotHash = this.convertFilenameToHash(fileAbsPath);
            if (string.IsNullOrEmpty(gotHash))
            {
                return string.Empty;
            }
            else
            {
                return gotHash;
            }
        }

        public void setFileHash(string fileAbsPath)
        {
            this.refreshInternalHash();
            string newHashOfFileName = this.convertFilenameToHash(fileAbsPath);
            if (this.fileHash.Contains(newHashOfFileName))
            {
                return;
            }
            this.fileHash.Add(newHashOfFileName);
            File.AppendAllText(this.hashSavedFilePath, newHashOfFileName);
        }

        public void removeFileHash(string fileAbsPath)
        {
            this.refreshInternalHash();
            string hash = this.convertFilenameToHash(fileAbsPath);
            if (string.IsNullOrEmpty(fileAbsPath))
            {
                return;
            }
            this.fileHash.Remove(hash);

            // tmeporary file is
            var temporaryFile = Path.GetTempFileName();
            var linesWithoutDeletedHash = File.ReadLines(this.hashSavedFilePath).Where(line => !line.Equals(hash));

            // "C:\\Users\\%USERNAME%\\AppData\\Local\\Temp\\xxxx.tmp"
            File.WriteAllLines(temporaryFile, linesWithoutDeletedHash);
            File.Delete(this.hashSavedFilePath);
            File.Move(temporaryFile, this.hashSavedFilePath);
        }

        public bool isFileHashed(string fileAbsPath)
        {
            this.refreshInternalHash();
            string gotHash = this.convertFilenameToHash(fileAbsPath);
            if (string.IsNullOrEmpty(gotHash))
            {
                return false;
            }
            return true;
        }

        protected string convertFilenameToHash(string fileAbsPath)
        {
            string command = "certutil";
            var process = new Process();
            var processStartInfo = new ProcessStartInfo(command);
            string arguments = string.Format(@"-hashfile {0} SHA256", fileAbsPath);
            processStartInfo.Arguments = arguments;
            processStartInfo.CreateNoWindow = false;
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            try
            {
                process = Process.Start(processStartInfo);
                process.WaitForExit();

                int exitCode = process.ExitCode;

                string standartOut = string.Empty;
                string standarddError = string.Empty;
                if (exitCode == 0)
                {
                    // result appears belows. so get index of 1 (line of 2)
                    //
                    // SHA256 ハッシュ (対象 C:\\tmp\\wi_icon_cursor_eraser.png):
                    // {hash code}
                    // CertUtil: -hashfile コマンドは正常に完了しました。
                    string result = process.StandardOutput.ReadToEnd();
                    string hashCode = result.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[1];
                    hashCode = this.convertFormalHashCode(hashCode);
                    return hashCode;
                }
                else
                {
                    standarddError = process.StandardError.ReadToEnd();
                    return string.Empty;
                }

            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        private string convertFormalHashCode(string inputHashCode)
        {
            // windows os 8 series makes hashcode with blank space.
            // convert it "without" blank space.

            return inputHashCode.Replace(" ", "");
        }

        protected void refreshInternalHash()
        {
            string[] lines = File.ReadAllLines(this.hashSavedFilePath);
            this.fileHash = new HashSet<string>(lines);
        }
    }
}
