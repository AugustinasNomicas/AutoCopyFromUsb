using System;
using System.IO;
using System.Windows.Forms;

namespace AutoCopyFromUsb
{
    public partial class MainForm : Form
    {
        private const string Filter = "*.txt";
        private readonly string _targetDirectory;

        private readonly UsbDetection _usbDetection;
        public MainForm()
        {
            InitializeComponent();
            _usbDetection = new UsbDetection();
            _targetDirectory = Path.Combine(Directory.GetCurrentDirectory(), "CopiedFiles");
            Console.WriteLine(@"Filter: {0} Target: {1}", Filter, _targetDirectory);
        }

        protected override void WndProc(ref Message m)
        {
            var letter = _usbDetection.GetNewDeviceLetter(m);
            base.WndProc(ref m);

            if (!string.IsNullOrEmpty(letter))
                CopyAllFilesFromLetter(letter);
        }

        private void CopyAllFilesFromLetter(string letter)
        {
            var sourceDir = letter + @":\";
            Copy(sourceDir, Path.Combine(_targetDirectory, letter));
        }


        void Copy(string sourceDir, string targetDir)
        {
            Console.WriteLine(@"Copying from {0} to {1}", sourceDir, targetDir);
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir, Filter))
                File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)), true);

            foreach (var directory in Directory.GetDirectories(sourceDir))
                Copy(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
        }
    }
}
