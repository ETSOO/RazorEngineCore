using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.IO;
using System.Linq;

namespace RazorEngineCore.Tasks
{
    /// <summary>
    /// Clean compiled Razor templates in the specified project directory
    /// 清理指定项目目录中的已编译 Razor 模板
    /// </summary>
    public class CleanTemplatesBuildTask : Task
    {
        /// <summary>
        /// Template directory
        /// 模板目录
        /// </summary>
        public required string TemplateDir { get; set; }

        public override bool Execute()
        {
            try
            {
                // Show a message to indicate that the task is running
                Log.LogMessage(MessageImportance.High, $"RazorEngineCore After Clean Task: cleaning templates in {TemplateDir} ...");

                // Compile all templates in the TemplateDir directory
                var files = Directory
                    .GetFiles(TemplateDir, "*.cshtml", SearchOption.AllDirectories)
                    .ToList();

                System.Threading.Tasks.Parallel.ForEach(files, file =>
                {
                    try
                    {
                        var outputFile = Path.ChangeExtension(file, ".bin");
                        File.Delete(outputFile);
                    }
                    catch (Exception ex)
                    {
                        Log.LogErrorFromException(new Exception($"RazorEngineCore cleaning template {file} failed", ex));
                    }
                });

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(new Exception("RazorEngineCore Cleaning Execution Exception", ex));
                return false;
            }
        }
    }
}
