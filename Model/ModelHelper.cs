using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using Main_Base.Model;

namespace Main_Base.Model
{
   public class ModelHelper
    {
        private readonly string folderPath =AppDomain.CurrentDomain.BaseDirectory;
        private readonly string fileName = "ModelManager.json";
        private string FilePath =>
        Path.Combine(folderPath, fileName);
        public void Save(string modelName, ParameterModel model)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                Dictionary<string, ParameterModel> data;

                // Nếu chưa có file -> tạo file mới
                if (!File.Exists(FilePath))
                {
                    File.WriteAllText(FilePath, "{}");
                }

                // Đọc dữ liệu cũ
                string json = File.ReadAllText(FilePath);

                data = JsonConvert.DeserializeObject
            <Dictionary<string, ParameterModel>>(json)
            ?? new Dictionary<string, ParameterModel>();

                data[modelName] = model;

                string output =
                    JsonConvert.SerializeObject(data, Formatting.Indented);

                File.WriteAllText(FilePath, output);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public ParameterModel Load(string modelName)
        {
            if (!File.Exists(FilePath))
                return null;

            string json = File.ReadAllText(FilePath);

            var data =
                JsonConvert.DeserializeObject
                <Dictionary<string, ParameterModel>>(json);

            if (data != null &&
                data.ContainsKey(modelName))
            {
                return data[modelName];
            }

            return null;
        }
    }
}
