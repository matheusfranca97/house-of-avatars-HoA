using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Diagnostics;
using UnityEditor;

public class FixWebGLMouseBug : IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPostprocessBuild(BuildReport report)
    {
        if (report.summary.platform == BuildTarget.WebGL)
        {
            // string outputPath = Path.Combine(report.summary.outputPath, "Build/Web/");
            string outputPath = report.summary.outputPath;
            ModifyFilesInDirectory(outputPath);
        }
    }

    void ModifyFilesInDirectory(string directoryPath)
    {
        // Search for gzipped JavaScript files first
        var gzippedFiles = Directory.GetFiles(directoryPath, "*.js.gz", SearchOption.AllDirectories);
        if (gzippedFiles.Length > 0)
        {
            foreach (var filePath in gzippedFiles)
            {
                ModifyAndCompressFile(filePath, true);
            }
        }
        else
        {
            // If no gzipped files, search for plain JavaScript files
            var jsFiles = Directory.GetFiles(directoryPath, "*.js", SearchOption.AllDirectories);
            foreach (var filePath in jsFiles)
            {
                // Avoid modifying .js files that are already covered by .js.gz processing
                if (!filePath.EndsWith(".js.gz"))
                {
                    ModifyAndCompressFile(filePath, false);
                }
            }
        }
    }

    void ModifyAndCompressFile(string filePath, bool isGzipped)
    {
        string tempFilePath = filePath;
        if (isGzipped)
        {
            tempFilePath = Path.ChangeExtension(filePath, ".tmp.js");
            // Decompress the file
            ProcessStartInfo decompress = new ProcessStartInfo("gzip", $"-d -k -c \"{filePath}\" > \"{tempFilePath}\"");
            decompress.UseShellExecute = false;
            Process.Start(decompress).WaitForExit();
        }

        // Modify the content
        string content = File.ReadAllText(tempFilePath);
        content = content.Replace("requestPointerLock()", "requestPointerLock({unadjustedMovement: true}).catch(function(error) {console.log(error);})");
        File.WriteAllText(tempFilePath, content);

        if (isGzipped)
        {
            // Compress the file again and cleanup
            File.Delete(filePath); // Delete the original gz file as gzip won't overwrite
            ProcessStartInfo compress = new ProcessStartInfo("gzip", $"-c \"{tempFilePath}\" > \"{filePath}\"");
            compress.UseShellExecute = false;
            Process.Start(compress).WaitForExit();

            File.Delete(tempFilePath); // Delete the temporary decompressed file
        }
    }
}