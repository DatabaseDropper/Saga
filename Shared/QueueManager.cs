using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Shared
{
    public class QueueManager
    {
        public string Path { get; }

        public QueueManager(string path)
        {
            Path = path;
        }

        public List<T> LoadEntries<T>(QueueDirection direction) where T : QueueItem
        {
            try
            {
                using (var fs = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None))
                {
                    using (var streamReader = new StreamReader(fs))
                    {
                        var json = streamReader.ReadToEnd();
                        var result = JsonConvert.DeserializeObject<List<T>>(json).Where(x => x.Direction == direction);
                        return result.ToList();
                    }
                }
            }
            catch
            {
                return new List<T>();
            }
        }

        public void Save<T>(T item)
        {
            while (true)
            {
                // poor retry mechanism
                try
                {
                    var result = new List<object>();
                    using (var fs = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
                    {
                        using (var streamReader = new StreamReader(fs))
                        {
                            var json = streamReader.ReadToEnd();
                            result = JsonConvert.DeserializeObject<List<object>>(json) ?? new List<object>();
                            result.Add(item);
                        }
                    }

                    using (var fs = new FileStream(Path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
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

        public void SaveMany<T>(List<T> items)
        {
            if (items.Count == 0)
                return;

            while (true)
            {
                // retry mechanism
                try
                {
                    var result = new List<object>();
                    using (var fs = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
                    {
                        using (var streamReader = new StreamReader(fs))
                        {
                            var json = streamReader.ReadToEnd();
                            result = JsonConvert.DeserializeObject<List<object>>(json) ?? new List<object>();
                            result.AddRange(items.Cast<object>());
                        }
                    }

                    using (var fs = new FileStream(Path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
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

        public void Remove(List<Guid> IdsToRemove)
        {
            while (true)
            {
                // retry mechanism
                try
                {
                    var result = new List<QueueItem>();

                    using (var fs = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
                    {
                        using (var streamReader = new StreamReader(fs))
                        {
                            var json = streamReader.ReadToEnd();
                            result = JsonConvert.DeserializeObject<List<QueueItem>>(json) ?? new List<QueueItem>();
                        }
                    }

                    result = result.Where(x => !IdsToRemove.Contains(x.Id)).ToList();

                    using (var fs = new FileStream(Path, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
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
    }
}
