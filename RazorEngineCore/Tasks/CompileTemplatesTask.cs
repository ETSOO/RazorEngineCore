using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.IO;
using System.Linq;

namespace RazorEngineCore.Tasks
{
    /// <summary>
    /// Compile Razor templates in the specified project directory before the build process
    /// Another idea is inline task: https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-inline-tasks?view=visualstudio
    /// 在构建过程之前编译指定项目目录中的 Razor 模板
    /// </summary>
    public class CompileTemplatesTask : Task
    {
        /// <summary>
        /// Template directory
        /// 模板目录
        /// </summary>
        public required string TemplateDir { get; set; }

        private bool ShouldCompile(string inputFile, string outputFile)
        {
            if (!File.Exists(outputFile))
            {
                return true;
            }

            var inputTime = File.GetLastWriteTimeUtc(inputFile);
            var outputTime = File.GetLastWriteTimeUtc(outputFile);

            return inputTime > outputTime;
        }

        private void CompileTemplate(string file)
        {
            Log.LogMessage(MessageImportance.High, $"RazorEngineCore compiling template {file} ...");

            var outputFile = Path.ChangeExtension(file, ".bin");

            if (!ShouldCompile(file, outputFile))
            {
                Log.LogMessage(MessageImportance.High, $"RazorEngineCore compilation skipped for {file}");
                return;
            }

            var content = File.ReadAllText(file);

            var razorEngine = new RazorEngine();

            var meta = razorEngine.CompileMeta<object>(content);

            using var memoryStream = new MemoryStream();
            meta.WriteAsync(memoryStream).GetAwaiter().GetResult();
            memoryStream.Position = 0;

            using var fileStream = File.Create(outputFile);
            memoryStream.CopyTo(fileStream);

            Log.LogMessage(MessageImportance.High, $"RazorEngineCore compiled template {file} to {outputFile}");
        }

        public override bool Execute()
        {
            try
            {
                // Show a message to indicate that the task is running
                Log.LogMessage(MessageImportance.High, $"RazorEngineCore BeforeBuild Task: compiling templates in {TemplateDir} ...");

                // Compile all templates in the TemplateDir directory
                var files = Directory
                    .GetFiles(TemplateDir, "*.cshtml", SearchOption.AllDirectories)
                    .ToList();

                System.Threading.Tasks.Parallel.ForEach(files, file =>
                {
                    try
                    {
                        CompileTemplate(file);
                    }
                    catch (Exception ex)
                    {
                        Log.LogErrorFromException(new Exception($"RazorEngineCore compiling template {file} failed", ex));
                    }
                });

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(new Exception("RazorEngineCore Compiling Execution Exception", ex));
                return false;
            }
        }
    }
}
