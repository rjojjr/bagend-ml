using System;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using bagend_ml.ML.MLModels;
using static System.Net.Mime.MediaTypeNames;

namespace bagend_ml.ML
{
	public class ModelMetaFileManager
	{

        private readonly ConcurrentDictionary<string, object> _fileLocks;
        private readonly ILogger<ModelMetaFileManager> _logger;

        private const string DefaultsFilePath = "/data/bagend-ml/models/meta/collective/defaults/";

        public ModelMetaFileManager(ILogger<ModelMetaFileManager> logger)
		{
            _logger = logger;
            _fileLocks = new ConcurrentDictionary<string, object>();
            makeDirs();
		}

        public IList<DefaultTickerModelEntry> GetDefaults()
        {
            var json = ReadFile(DefaultsFilePath + "defaults.json.meta");
            return json != null && json != "" ? JsonSerializer.Deserialize<DefaultModels>(json).Defaults : new List<DefaultTickerModelEntry>();
        }

        public void SaveDefaults(IList<DefaultTickerModelEntry> defaults)
        {
            var contents = new DefaultModels(defaults);
            var json = JsonSerializer.Serialize(contents);

            WriteFile(DefaultsFilePath + "defaults.json.meta", json);
        }

        public IList<T> GetAllModelMeta<T>()
        {
            var isCollective = typeof(T) == typeof(CollectiveMLModelMeta);
            var fileDir = !isCollective
                ? "/data/bagend-ml/models/meta/"
                : "/data/bagend-ml/models/meta/collective/";
            var models = new List<T>();
            var filePaths = Directory.EnumerateFiles(fileDir, "*.json.meta");

            foreach(string filePath in filePaths)
            {
                var urlParts = filePath.Split("/");
                var modelName = urlParts[urlParts.Length - 1].Split(".json.meta")[0];

                models.Add(GetMeta<T>(modelName));
            }

            return models;
        }

        public T? GetMeta<T>(string modelName)
        {
            var isCollective = typeof(T) == typeof(CollectiveMLModelMeta);
            var json = ReadFile(ConstructMetaFilePath(modelName, isCollective));
            return json != null
                ? (isCollective
                    ? (T)Convert.ChangeType(CollectiveMLModelMeta.FromJson(json), typeof(T))
                    : (T)Convert.ChangeType(ForcastingModelMeta.FromJson(json), typeof(T)))
                : default;
        }

        public CollectiveMLModelMeta? GetCollectiveMeta(string modelName)
        {
            var json = ReadFile(ConstructMetaFilePath(modelName, true));
            return json != null ? CollectiveMLModelMeta.FromJson(json) : null;
        }

        public void WriteMeta(IMLMeta meta)
        {
            var isCollective = meta is CollectiveMLModelMeta;
            WriteFile(ConstructMetaFilePath(meta.getName(), isCollective),
                meta.toJson());
        }

        public void WriteMeta(CollectiveMLModelMeta forcastingModelMeta)
        {
            _logger.LogInformation("writing meta for model {}", forcastingModelMeta.CollectiveModelName);
            WriteFile(ConstructMetaFilePath(forcastingModelMeta.CollectiveModelName, true),
                forcastingModelMeta.toJson());
        }



        private void makeDirs()
        {
            var metaDir = "/data/bagend-ml/models/meta/";
            var collectiveMetaDir = "/data/bagend-ml/models/meta/collective";
            var trainedDir = "/data/bagend-ml/models/trained/";

            if (!File.Exists(metaDir))
            {
                _logger.LogInformation("creating dir {}", metaDir);
                Directory.CreateDirectory(metaDir);
            }

            if (!File.Exists(collectiveMetaDir))
            {
                _logger.LogInformation("creating dir {}", collectiveMetaDir);
                Directory.CreateDirectory(collectiveMetaDir);
            }

            if (!File.Exists(trainedDir))
            {
                _logger.LogInformation("creating dir {}", trainedDir);
                Directory.CreateDirectory(trainedDir);
            }

            if (!File.Exists(DefaultsFilePath))
            {
                _logger.LogInformation("creating dir {}", DefaultsFilePath);
                Directory.CreateDirectory(DefaultsFilePath);
            }
        }

        private static string ConstructMetaFilePath(string modelName, bool isCollective = false)
        {
            return isCollective
                ? "/data/bagend-ml/models/meta/collective/" + modelName + ".json.meta"
                : "/data/bagend-ml/models/meta/" + modelName + ".json.meta";
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

		private string? ReadFile(string filePath)
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
                var path = System.IO.Path.GetFullPath(filePath);
                var file = System.IO.File.Create(path);
                var fileWriter = new System.IO.StreamWriter(file);
                fileWriter.WriteLine(contents);
                fileWriter.Dispose();
            }
        }
	}
}

