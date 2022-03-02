
using CommandLine;


namespace Litframework.ExcelTool
{
    public class Program
    {
        //C:/Personal/Unity/iFunTech/BasePackage/Assets
        private static int Main(string[] args )
        {

            // 命令行参数
            Options options = null;
            Parser.Default.ParseArguments<Options>(args)
                    .WithNotParsed(error => throw new Exception($"命令行格式错误!"))
                    .WithParsed(o => { options = o; });

            ExcelExport.ProjectPath = options.ProjectPath;
            switch (options.ExportModelType)
            {
                case 1:
                    ExcelExport.Xlsx_2_CSV( options.ExtralFileExtention);
                    Console.WriteLine("\n\n");
                    Console.WriteLine("导出--CSV--成功!");
                    Console.WriteLine("");
                    break;
                case 2:
                    ExcelExport.Xlsx_2_CsvCs(options.UseHotFix, options.ExtralFileExtention);
                    Console.WriteLine("\n\n");
                    Console.WriteLine("导出--CSV-代码--成功!");
                    Console.WriteLine("");
                    break;
                default:
                    Console.WriteLine($"参数 ExportModelType 填写错误。当前为：{options.ExportModelType}");
                    break;
            }
            return 0;

        }
    }

    public enum AppType
    {
        Server,
        Robot,
        Watcher, // 每台物理机一个守护进程，用来启动该物理机上的所有进程
        GameTool,
        ExcelExporter,
        Proto2CS
    }

    public class Options
    {
        public static Options Instance { get; set; }

        [Option("AppType", Required = false, Default = AppType.Server, HelpText = "serverType enum")]
        public AppType AppType { get; set; }

        [Option("Process", Required = false, Default = 1)]
        public int Process { get; set; } = 1;

        [Option("Develop", Required = false, Default = 0, HelpText = "develop mode, 0正式 1开发 2压测")]
        public int Develop { get; set; } = 0;

        [Option("LogLevel", Required = false, Default = 2)]
        public int LogLevel { get; set; } = 2;

        [Option("Console", Required = false, Default = 0)]
        public int Console { get; set; } = 0;

        // 进程启动是否创建该进程的scenes
        [Option("CreateScenes", Required = false, Default = 1)]
        public int CreateScenes { get; set; } = 1;

        #region ExcelExportSetting

        [Option("ProjectPath", Required = true, Default = "", HelpText = "eg: C:/Personal/Unity/iFunTech/BasePackage/Assets")]
        public string ProjectPath { get; set; }

        [Option("ExportModelType", Required = true, Default = 1, HelpText = "excel export mode, 1导csv 2导csv+代码")]
        public int ExportModelType { get; set; }

        [Option("ExtralFileExtention", Required = true, Default = "json|dat|assetbundle", HelpText = "excel export mode, 1导csv 2导csv+代码")]
        public string ExtralFileExtention { get; set; }

        [Option("UseHotFix", Required = true, Default = true, HelpText = "默认使用 true")]
        public bool UseHotFix { get; set; }

        #endregion
    }
}