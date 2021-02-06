using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Shared
{
    public class FileDb
    {
        public string Path { get; }

        public FileDb(string path)
        {
            Path = path;
        }

        public void Create(object o)
        {
            while (true)
            {
                // poor retry mechanism
                try
                {
                    var result = new List<object>();

                    using (var fs = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None))
                    {
                        using (var streamReader = new StreamReader(fs))
                        {
                            var json = streamReader.ReadToEnd();
                            result = JsonConvert.DeserializeObject<List<object>>(json) ?? new List<object>();
                            result.Add(o);
                        }
                    }

                    using (var fs = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                    {
                        using (var streamWriter = new StreamWriter(fs))
                        {
                            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
                            streamWriter.Write(json);
                        }
                    }

                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public List<T> GetAll<T>()
        {
            try
            {
                using (var fs = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None))
                {
                    using (var streamReader = new StreamReader(fs))
                    {
                        var json = streamReader.ReadToEnd();
                        var result = JsonConvert.DeserializeObject<List<T>>(json).ToList();
                        return result;
                    }
                }
            }
            catch
            {
                return new List<T>();
            }
        }

        public void Overwrite<T>(List<T> items)
        {
            if (items.Count == 0)
                return;

            while (true)
            {
                // poor retry mechanism
                try
                {
                    using (var fs = new FileStream(Path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                    {
                        using (var streamWriter = new StreamWriter(fs))
                        {
                            var json = JsonConvert.SerializeObject(items, Formatting.Indented);
                            streamWriter.Write(json);
                        }
                    }

                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
