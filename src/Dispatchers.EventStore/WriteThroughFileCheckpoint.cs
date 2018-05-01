// <copyright file="WriteThroughFileCheckpoint.cs" company="Cognisant">
// Copyright (c) Cognisant. All rights reserved.
// </copyright>

namespace CR.MessageDispatch.Dispatchers.EventStore
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Microsoft.Win32.SafeHandles;

    internal class WriteThroughFileCheckpoint
    {
        private readonly string _filename;
        private readonly string _name;
        private readonly bool _cached;
        private readonly BinaryWriter _writer;
        private readonly BinaryReader _reader;
        private readonly MemoryStream _memStream;
        private readonly byte[] _buffer;
        private long _last;
        private long _lastFlushed;
        private FileStream _stream;

        public WriteThroughFileCheckpoint(string filename)
            : this(filename, Guid.NewGuid().ToString(), false)
        {
        }

        public WriteThroughFileCheckpoint(string filename, string name)
            : this(filename, name, false)
        {
        }

        public WriteThroughFileCheckpoint(string filename, string name, bool cached, long initValue = 0)
        {
            _filename = filename;
            _name = name;
            _cached = cached;
            _buffer = new byte[4096];
            _memStream = new MemoryStream(_buffer);

            var handle = Filenative.CreateFile(
                _filename,
                (uint)FileAccess.ReadWrite,
                (uint)FileShare.ReadWrite,
                IntPtr.Zero,
                (uint)FileMode.OpenOrCreate,
                Filenative.FILE_FLAG_NO_BUFFERING | (int)FileOptions.WriteThrough,
                IntPtr.Zero);

            _stream = new FileStream(handle, FileAccess.ReadWrite, 4096);
            var exists = _stream.Length == 4096;
            _stream.SetLength(4096);
            _reader = new BinaryReader(_stream);
            _writer = new BinaryWriter(_memStream);
            if (!exists)
            {
                Write(initValue);
                Flush();
            }

            _last = _lastFlushed = ReadCurrent();
        }

        public string Name
        {
            get { return _name; }
        }

        public void Close()
        {
            Flush();
            _stream.Close();
            _stream.Dispose();
        }

        public void Write(long checkpoint)
        {
            Interlocked.Exchange(ref _last, checkpoint);
        }

        public void Flush()
        {
            _memStream.Seek(0, SeekOrigin.Begin);
            _stream.Seek(0, SeekOrigin.Begin);
            var last = Interlocked.Read(ref _last);
            _writer.Write(last);
            _stream.Write(_buffer, 0, _buffer.Length);

            Interlocked.Exchange(ref _lastFlushed, last);

            // FlushFileBuffers(_file.SafeMemoryMappedFileHandle.DangerousGetHandle());
        }

        public long Read()
        {
            return _cached ? Interlocked.Read(ref _lastFlushed) : ReadCurrent();
        }

        public long ReadNonFlushed()
        {
            return Interlocked.Read(ref _last);
        }

        public void Dispose()
        {
            Close();
        }

        [DllImport("kernel32.dll")]
        private static extern bool FlushFileBuffers(IntPtr hFile);

        private long ReadCurrent()
        {
            _stream.Seek(0, SeekOrigin.Begin);
            return _reader.ReadInt64();
        }

        private static class Filenative
        {
#pragma warning disable SA1310 // Field names should not contain underscore - Gets pulled from the eventstore DLLs
            public const int FILE_FLAG_NO_BUFFERING = 0x20000000;
#pragma warning restore SA1310 // Field names should not contain underscore

            [DllImport("kernel32", SetLastError = true)]
            internal static extern SafeFileHandle CreateFile(
                string fileName,
                uint desiredAccess,
                uint shareMode,
                IntPtr securityAttributes,
                uint creationDisposition,
                int flagsAndAttributes,
                IntPtr hTemplate);
        }
    }
}
