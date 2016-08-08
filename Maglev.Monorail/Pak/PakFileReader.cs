using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Maglev.Monorail.Resource;

namespace Maglev.Monorail.Pak
{
    public class PakFileReader
    {
        private class FileHandle : IDisposable
        {
            public readonly FileStream FileStream;

            public readonly BinaryReader BinaryReader;

            public readonly StreamReader StreamReader;

            public FileHandle(String name)
            {
                FileStream = new FileStream(name, FileMode.Open, FileAccess.Read);
                StreamReader = new StreamReader(FileStream);
                BinaryReader = new BinaryReader(FileStream);
            }

            public void Dispose()
            {
                FileStream.Dispose();
                BinaryReader.Dispose();
                StreamReader.Dispose();
                FileStream.Close();
            }
        }

        public Dictionary<String, PakFileRecord> Index = new Dictionary<string, PakFileRecord>();

        private bool m_IsOpen;

        public String Name { get; private set; }

        private readonly Queue<FileHandle> m_FileHandles = new Queue<FileHandle>(4); 

        ~PakFileReader()
        {
            if (m_IsOpen)
            {
                while (m_FileHandles.Count > 0)
                {
                    var handle = m_FileHandles.Dequeue();
                    handle.Dispose();
                }

                m_IsOpen = false;
            }
        }

        public void ExtractAll(string directory)
        {
            throw new NotImplementedException();
        }

        public byte[] ReadBytes(string name)
        {
            if (m_IsOpen == false)
                throw new Exception("Pakfile must be opened first");

            name = name.ToLower();
            if (!Index.ContainsKey(name))
                throw new Exception(String.Format("{0} Not found in packfile {1}", name, Name));

            var record = Index[name];
            var buffer = new byte[record.Size];

            FileHandle fh = null;
            lock (m_FileHandles)
            {
                if (m_FileHandles.Count == 0)
                {
                    fh = new FileHandle(Name);
                    m_FileHandles.Enqueue(fh);
                }
                fh = m_FileHandles.Dequeue();
            }

            fh.FileStream.Seek(record.Offset, SeekOrigin.Begin);  
            fh.BinaryReader.Read(buffer, 0, (int)record.Size);

            lock (m_FileHandles)
            {
                m_FileHandles.Enqueue(fh);
            }
            
            return buffer;
        }

        public String ReadTextFile(string name)
        {
            if (m_IsOpen == false)
                throw new Exception("Pakfile must be opened first");

            name = name.ToLower();
            if (!Index.ContainsKey(name))
                throw new Exception(String.Format("{0} Not found in packfile {1}",name, Name));

            var record = Index[name];
            var buffer = new char[record.Size];
            FileHandle fh = null;
            lock (m_FileHandles)
            {
                if (m_FileHandles.Count == 0)
                {
                    fh = new FileHandle(Name);
                    m_FileHandles.Enqueue(fh);
                }
                fh = m_FileHandles.Dequeue();
            }

            fh.FileStream.Seek(record.Offset, SeekOrigin.Begin);
            fh.StreamReader.Read(buffer, 0, (int)record.Size);

            lock (m_FileHandles)
            {
                m_FileHandles.Enqueue(fh);
            }

            return new string(buffer);
        }

        public void OpenPackFile(string filename)
        {
            Index = ReadHeader(filename + ".index");
            Name = filename;
            m_IsOpen = true;

            for (var i = 0; i < Environment.ProcessorCount; i++)
            {
                var fh = new FileHandle(Name);
                m_FileHandles.Enqueue(fh);
            }
        }

        public Dictionary<String, PakFileRecord> ReadHeader(string filename)
        {
            var rv = new Dictionary<String, PakFileRecord>();
            using (var outputstream = File.OpenRead(filename))
            {
                using (var br = new BinaryReader(outputstream))
                {
                    var count = br.ReadInt32();
                    for (var i = 0; i < count; i++)
                    {
                        var key = br.ReadString();
                        var ext = br.ReadString();
                        var offset = br.ReadInt64();
                        var size = br.ReadInt64();

                        rv.Add(key,new PakFileRecord() { Name = key, Extension = ext,Offset = offset,Size = size });
                    }
                }
            }

            return rv;
        }
    }
}
