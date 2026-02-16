using System.Diagnostics;
using System.Runtime.InteropServices;

namespace YtDlpGui.Services {
    public static class ProcessHelper {
        private enum CtrlTypes : uint {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT,
            CTRL_CLOSE_EVENT,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT
        }

        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(uint dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(ConsoleCtrlDelegate? HandlerRoutine, bool Add);
        private delegate bool ConsoleCtrlDelegate(CtrlTypes CtrlType);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GenerateConsoleCtrlEvent(CtrlTypes dwCtrlEvent, uint dwProcessGroupId);

        public static void SendCtrlC(Process process) {
            if (AttachConsole((uint)process.Id)) {
                SetConsoleCtrlHandler(null, true);
                GenerateConsoleCtrlEvent(CtrlTypes.CTRL_C_EVENT, (uint)0);
                FreeConsole();
                process.WaitForExit();
                SetConsoleCtrlHandler(null, false);
            }
        }
    }
}
