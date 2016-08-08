using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Maglev.Monorail.Pak
{
    public class PakFileMaker
    {
        public void MakePakFile(string inputDirectoryPath, string outputFilePath)
        {
            var index = new Dictionary<String, PakFileRecord>();

            var rootDir = inputDirectoryPath;

            Console.WriteLine("Input path: " + inputDirectoryPath);
            Console.WriteLine("Output File: " + outputFilePath);


            // Write files in directory
            using (var outputStream = File.Create(outputFilePath))
            {
                WriteDirectory(rootDir, inputDirectoryPath, outputStream, ref index);
            }

            // Write the header
            using (var outputstream = File.Create(outputFilePath + ".index"))
            {
                using (var bw = new BinaryWriter(outputstream))
                {
                    bw.Write(index.Count);

                    foreach (var kvp in index)
                    {
                        bw.Write(kvp.Key);

                        var value = kvp.Value;
                        bw.Write(value.Extension);
                        bw.Write(value.Offset);
                        bw.Write(value.Size);
                    }
                }
            }
        }

        private static void WriteDirectory(string rootDir, string inputDirectoryPath, FileStream destFile, ref Dictionary<string, PakFileRecord> index)
        {
            var inputFilePaths = Directory.GetFiles(inputDirectoryPath, "*.*");
            
            foreach (var inputFilePath in inputFilePaths)
            {
                using (var inputStream = File.OpenRead(inputFilePath))
                {
                    var record = new PakFileRecord
                        {
                            Name = inputFilePath.Substring(rootDir.Length+1, inputFilePath.Length-rootDir.Length-1).ToLower(),
                            Extension = Path.GetExtension(inputFilePath),
                            Offset = destFile.Position,
                            Size = inputStream.Length
                        };

                    index.Add(record.Name, record);
                    inputStream.CopyTo(destFile);
                }
            }

            var directories = Directory.GetDirectories(inputDirectoryPath);
            foreach (var directory in directories)
            {
                WriteDirectory(rootDir, directory, destFile, ref index);
            }
        }
    }
}
