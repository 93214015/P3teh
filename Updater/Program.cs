using System;
using System.IO;
using System.Collections.Generic;

namespace Updater
{
    class Program
    {
        const string _TempRelativeDir = "Temp";
        const string _BackupRelativeDir = "Backup";

        class CorrespondFile
        {
            public string Path = "";
            public string NewFile = "";
        }

        static void Main(string[] args)
        {
            Console.WriteLine(args[0]);

            if (string.IsNullOrEmpty(args[0]))
            {
                Console.WriteLine("No Arguments\n");
                Environment.Exit(0);
            }

            List<CorrespondFile> _Files = new List<CorrespondFile>();

            string _CurrentDir = Directory.GetCurrentDirectory();
            string _TempDir = Path.Combine(Directory.GetCurrentDirectory(), _TempRelativeDir);
            string _BackupDir = Path.Combine(Directory.GetCurrentDirectory(), _BackupRelativeDir);

            if (Directory.Exists(_BackupDir))
            {
                Directory.Delete(_BackupDir, true);
            }
            Directory.CreateDirectory(_BackupDir);

            foreach (var _File in Directory.GetFiles(_TempDir))
            {
                string _FileName = Path.GetFileName(_File);
                string _OldFile = Path.Combine(_CurrentDir, _FileName);
                _Files.Add(new CorrespondFile { NewFile = _File, Path = _OldFile });

                FileInfo _FileInfo = new FileInfo(_OldFile);
                if (_FileInfo.Exists)
                {
                    File.Copy(_OldFile, Path.Combine(_BackupDir, _FileName), true);
                }
            }

            foreach (var _Dir in Directory.GetDirectories(_TempDir))
            {
                foreach (var _File in Directory.GetFiles(_Dir))
                {
                    string _RelativePath = Path.GetRelativePath(_TempDir, _File);
                    string _OldFile = Path.Combine(_CurrentDir, _RelativePath);
                    _Files.Add(new CorrespondFile { NewFile = _File, Path = _OldFile });

                    FileInfo _FileInfo = new FileInfo(_OldFile);
                    if (_FileInfo.Exists)
                    {
                        FileInfo _BackupFileInfo = new FileInfo(Path.Combine(_BackupDir, _RelativePath));

                        if (!_BackupFileInfo.Directory.Exists)
                        {
                            Directory.CreateDirectory(_BackupFileInfo.DirectoryName);
                        }
                        File.Copy(_OldFile, Path.Combine(_BackupDir, _RelativePath), true);
                    }
                }
            }

            foreach (var _File in _Files)
            {
                var _Directory = Path.GetDirectoryName(_File.Path);

                if (_Directory != null)
                {
                    Directory.CreateDirectory((string)_Directory);
                }

                File.Copy(_File.NewFile, _File.Path, true);
            }

            System.Diagnostics.Process.Start(args[0]);

        }
    }
}
