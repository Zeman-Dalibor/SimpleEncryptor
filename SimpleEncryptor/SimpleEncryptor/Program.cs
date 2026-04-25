using SimpleEncryptor.Core;

namespace SimpleEncryptor
{
    internal static class Program
    {
        [STAThread]
        static int Main(string[] args)
        {
            if (args.Length > 0)
            {
                return CliRunner.Run(args);
            }

            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
            return 0;
        }
    }
}