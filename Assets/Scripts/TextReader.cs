using System;
using System.Collections.Generic;
using System.IO;

namespace Tools.FileReading.Minimal
{
    public class SimpleFileReader : IDisposable
    {
        private const int CacheSize = 50;
        private readonly Dictionary<int, string> _cache;
        private readonly List<int> _cachingOrder;
        private readonly StreamReader _reader;
        private int _readerPosition;

        public SimpleFileReader(string filePath)
        {
            _cache = new Dictionary<int, string>();
            _cachingOrder = new List<int>();

            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            _reader = new StreamReader(fileStream);
        }

        ~SimpleFileReader()
        {
            Dispose();
        }

        public void Dispose()
        {
            _cache?.Clear();
            _reader?.Dispose();
        }

        public string ReadLine(int lineNumber)
        {
            if (lineNumber < 1) throw new ArgumentException("Line number must be greater than zero.", nameof(lineNumber));

            if (_cache.ContainsKey(lineNumber))
            {
                return _cache[lineNumber];
            }

            if (lineNumber > _readerPosition)
            {
                while (_readerPosition < lineNumber)
                {
                    string line = _reader.ReadLine() ?? throw new ArgumentOutOfRangeException(nameof(lineNumber), "The file does not contain the specified line number.");
                    _readerPosition++;

                    if ((lineNumber - CacheSize) < _readerPosition)
                    {
                        HadleCache(_readerPosition, line);
                    }

                    if (lineNumber == _readerPosition)
                    {
                        return line;
                    }
                }

                throw new ArgumentOutOfRangeException(nameof(lineNumber), "The file does not contain the specified line number.");
            }
            else
            {
                _reader.BaseStream.Position = 0;
                _reader.DiscardBufferedData();
                _readerPosition = 0;

                while (_readerPosition < lineNumber)
                {
                    string line = _reader.ReadLine() ?? throw new ArgumentOutOfRangeException(nameof(lineNumber), "The file does not contain the specified line number.");
                    _readerPosition++;

                    if ((lineNumber - CacheSize) < _readerPosition)
                    {
                        HadleCache(_readerPosition, line);
                    }

                    if (lineNumber == _readerPosition)
                    {
                        return line;
                    }
                }

                throw new ArgumentOutOfRangeException(nameof(lineNumber), "The file does not contain the specified line number.");
            }
        }

        private void HadleCache(int lineNumber, string line)
        {
            if (!_cache.ContainsKey(lineNumber))
            {
                if (_cache.Count >= CacheSize)
                {
                    //This caching stores last CacheSize amount of readed lines
                    int lastAdded = _cachingOrder[0];
                    _cachingOrder.RemoveAt(0);
                    _cache.Remove(lastAdded);
                }

                _cache.Add(lineNumber, line);
                _cachingOrder.Add(lineNumber);
            }
        }
    }
}