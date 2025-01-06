using System.Windows.Forms;
using Xioa.Admin.Core.Services.CommandLine;

namespace Xioa.Admin.Core;

public partial class App {
    private CommandLineParser _commandLine;
    private void EnableDebugMode()
    {
        // 调试模式逻辑
       
    }
    private void StartupCommandLine(string[]? args) {
        if (args is null || args.Length < 1)
            return;
        // --debug  --width 1024 --height 768 --maximize true
        _commandLine = new CommandLineParser(args);
        if (_commandLine.HasParameter("debug"))
        {
            EnableDebugMode();
        }
        
        // 获取窗口大小
        int width = _commandLine.GetIntValue("width", 1600);
        int height = _commandLine.GetIntValue("height", 900);

        // 是否最大化窗口
        bool maximize = _commandLine.GetBoolValue("maximize");

        
    }
}