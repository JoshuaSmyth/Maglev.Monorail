using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Maglev.Monorail.Async;
using Maglev.Monorail.Pak;
using NUnit.Framework;

namespace Maglev.Monorail.Tests
{
    [TestFixture]
    public class PakFileTests
    {

        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public static string LocateResource(string path)
        {
            return Path.Combine(AssemblyDirectory, path);
        }

        [Test]
        public void CreatePakFile()
        {
            var pakFileMaker = new PakFileMaker();
            var path = LocateResource(@"Resources\RawFiles\Pak001");
            pakFileMaker.MakePakFile(path, LocateResource("pak001.pak"));


            var pakFileReader = new PakFileReader();
            pakFileReader.OpenPackFile(LocateResource("pak001.pak"));

            var header = pakFileReader.Index;
        }


        [Test]
        public void ReadPakFileAsync()
        {
            var manager = new AsyncOps();
            var pakFileReader = new PakFileReader();
            
                pakFileReader.OpenPackFile(LocateResource("pak001.pak"));

                manager.Queue(() =>
                    {
                        var bytes = pakFileReader.ReadBytes(@"Images\testimage001.png");

                        File.WriteAllBytes(@"c:\temp\test.png", bytes);
                        var count = bytes.Length;
                    });

                manager.Queue(() =>
                    {
                        var bytes = pakFileReader.ReadBytes(@"Images\testimage002.png");
                    });


            while(manager.HasActions())
                manager.Update(16);
            
        }
    }
}
