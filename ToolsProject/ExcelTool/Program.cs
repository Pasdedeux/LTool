
using CommandLine;


namespace Litframework.ExcelTool
{
    public class Program
    {
        //C:/Personal/Unity/iFunTech/BasePackage/Assets
        private static int Main(string[] args)
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
                    if (!IsTrue(options.UseSql)) ExcelExport.Xlsx_2_CSV(IsTrue(options.UseServer), options.ExtralFileExtention);
                    else ExcelExport.XlsxToSQLite(IsTrue(options.UseServer), options.ExtralFileExtention);

                    Console.WriteLine("\n==================>\n");
                    Console.WriteLine("导出--CSV--成功!");
                    Console.WriteLine("");
                    break;
                case 2:
                    if (!IsTrue(options.UseSql)) ExcelExport.Xlsx_2_CsvCs(IsTrue(options.UseHotFix), IsTrue(options.UseServer), options.ExtralFileExtention);
                    else ExcelExport.XlsxToSQLiteCs(IsTrue(options.UseHotFix), IsTrue(options.UseServer), options.ExtralFileExtention);

                    Console.WriteLine("\n==================>\n");
                    Console.WriteLine("导出--CSV-代码--成功!");
                    Console.WriteLine("");
                    break;
                default:
                    Console.WriteLine($"参数 ExportModelType 填写错误。当前为：{options.ExportModelType}");
                    break;
            }
            return 0;

        }

        private static bool IsTrue(int boolIndex)
        {
            return boolIndex == 0 ? false : true;
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

        [Option("UseHotFix", Required = true, Default = 1, HelpText = "1-true  0-false 默认使用 1")]
        public int UseHotFix { get; set; }

        [Option("UseServer", Required = true, Default = 0, HelpText = "1-true  0-false 默认使用 0")]
        public int UseServer { get; set; }

        [Option("UseSql", Required = true, Default = 0, HelpText = "1-true  0-false 默认使用 0")]
        public int UseSql { get; set; }

        #endregion
    }
}