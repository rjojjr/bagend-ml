using System;
using System.Collections.Concurrent;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace bagend_ml.ML
{
	public class ModelMetaFileManager
	{

        private readonly ConcurrentDictionary<string, object> _fileLocks;

        public ModelMetaFileManager()
		{
            _fileLocks = new ConcurrentDictionary<string, object>();
		}

        public IList<ForcastingModelMeta> GetAllModelMeta()
        {
            var models = new List<ForcastingModelMeta>();
            var filePaths = Directory.EnumerateFiles("/data/bagend-ml/models/meta/", "*.json.meta");

            foreach(string filePath in filePaths)
            {
                var urlParts = filePath.Split("/");
                var modelName = urlParts[urlParts.Length - 1].Split(".json.meta")[0];

                models.Add(GetMeta(modelName));
            }

            return models;
        }

        public ForcastingModelMeta GetMeta(string modelName)
        {
            var json = ReadFile(ConstructMetaFilePath(modelName));
            return json != null ? ForcastingModelMeta.FromJson(json) : null;
        }

        public void WriteMeta(ForcastingModelMeta forcastingModelMeta)
        {
            WriteFile(ConstructMetaFilePath(forcastingModelMeta.ModelName),
                forcastingModelMeta.toJson());
        }

        private static string ConstructMetaFilePath(string modelName)
        {
            return "/data/bagend-ml/models/meta/" + modelName + ".json.meta";
        }

        private object GetOrCreateFileLock(string filePath)
        {
            return GetOrCreateDictionaryValue(filePath, _fileLocks);
            
        }

        private static object GetOrCreateDictionaryValue(string key, ConcurrentDictionary<string, object> dictionary)
        {
            object value = null;
            if (!dictionary.TryGetValue(key, out value))
            {
                value = dictionary.GetOrAdd(key, new object());
            }
            return value;
        }

		private string ReadFile(string filePath)
		{
            Func<string> readFile = () =>
            {
                // Reads file line by line
                StreamReader Textfile = new StreamReader(filePath);
                string line;
                StringBuilder contents = new StringBuilder();

                while ((line = Textfile.ReadLine()) != null)
                {
                    contents.AppendLine(line);
                }

                Textfile.Close();

                return contents.ToString();
            };

            if (File.Exists(filePath))
            {
                lock (GetOrCreateFileLock(filePath)) {
                    return readFile.Invoke();
                }
            }
            return null;
        }

        public void WriteFile(string filePath, string contents)
        {
            lock (GetOrCreateFileLock(filePath))
            {
                File.WriteAllText(filePath, contents);
            }
        }
	}
}

