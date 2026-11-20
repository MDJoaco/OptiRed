using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;

namespace OptiRed
{
    internal class GeneradorRutas
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AttachConsole(int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        static extern bool DeleteMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        const uint SC_CLOSE = 0xF060;
        const uint MF_BYCOMMAND = 0x00000000;

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        private string _contenidos;
        private string _ejecutable;
        private Process? _javaProcess;

        public GeneradorRutas(string contenidos, string ejecutable)
        {
            _contenidos = contenidos;
            _ejecutable = ejecutable;
        }

        public void Start()
        {
            try
            {
                string jarPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _contenidos, _ejecutable);
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "java",
                    Arguments = $"-jar \"{jarPath}\"",
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                _javaProcess = Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to launch Java: {ex.Message}\nEnsure Java (JRE) is installed on this machine.");
            }
        }

        public bool Running()
        {
            return _javaProcess != null && !_javaProcess.HasExited;
        }

        public int CargarMapa(string camino)
        {
            if (!Running())
            {
                return 1;
            }

            try
            {
                _javaProcess.StandardInput.WriteLine("cargar mapa " + camino);
                _javaProcess.StandardInput.Flush();
                string output = _javaProcess.StandardOutput.ReadLine();

                if (output == "Datos cargados correctamente!")
                    return 0;

                MessageBox.Show($"Error loading map: {output}");

                return 3;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error communicating with Java process: {ex.Message}");
                return 2;
            }
        }

        public int CargarViajes(string camino)
        {
            if (!Running())
            {
                return 1;
            }

            try
            {
                _javaProcess.StandardInput.WriteLine("cargar viajes " + camino);
                _javaProcess.StandardInput.Flush();
                string output = _javaProcess.StandardOutput.ReadLine();

                if (output == "Datos cargados correctamente!")
                    return 0;

                MessageBox.Show($"Error loading map: {output}");

                return 3;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error communicating with Java process: {ex.Message}");
                return 2;
            }
        }

        public int ShowConsole()
        {
            if (!Running())
                return 1;

            if (!AttachConsole(_javaProcess.Id))
            {
                MessageBox.Show("Failed to attach to console.");
                return 2;
            }

            var writer = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
            Console.SetOut(writer);
            Console.SetError(writer);

            IntPtr consoleHandle = GetConsoleWindow();
            ShowWindow(consoleHandle, SW_SHOW);

            IntPtr systemMenu = GetSystemMenu(consoleHandle, false);
            DeleteMenu(systemMenu, SC_CLOSE, MF_BYCOMMAND);

            return 0;
        }

        public int HideConsole()
        {
            if (!Running())
                return 1;

            IntPtr consoleHandle = GetConsoleWindow();

            if (consoleHandle != IntPtr.Zero)
            {
                ShowWindow(consoleHandle, SW_HIDE);
            }

            FreeConsole();

            return 0;
        }

        public void Stop()
        {
            if (Running())
            {
                _javaProcess.StandardInput.WriteLine("salir");
                _javaProcess.StandardInput.Flush();
                _javaProcess.WaitForExit();
            }
        }
    }
}
