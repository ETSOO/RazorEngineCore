using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.IO;
using System.Linq;

namespace RazorEngineCore.Tasks
{
    public class ClearTemplatesBuildTask : Task
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
                Log.LogMessage(MessageImportance.High, $"RazorEngineCore AfterClear Task: clearing templates in {TemplateDir} ...");

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
                        Log.LogErrorFromException(new Exception($"RazorEngineCore clearing template {file} failed", ex));
                    }
                });

                return true;
            }
            catch (Exception ex)
            {
                Log.LogErrorFromException(new Exception("RazorEngineCore Clearing Execution Exception", ex));
                return false;
            }
        }
    }
}
