using MasterPasswordDesktop.Infrastructure.Helpers;
using MasterPasswordDesktop.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MasterPasswordDesktop.DataAccess
{
    public class MyEncryptedFileStorage : IDataStorage
    {
        readonly string _filePath;
        string _password;
        readonly JsonSerializerSettings _settings;

        public string SetPassword(string password) => _password = password;

        public MyEncryptedFileStorage(string filePath)
        {
            _filePath = filePath;
            _settings = new JsonSerializerSettings() { Formatting = Formatting.None, PreserveReferencesHandling = PreserveReferencesHandling.Objects };

            if (!File.Exists(_filePath))
            {
                using (var file = File.Create(_filePath)) { };
            }
        }

        public void DeleteDataLine(DataLine line)
        {
            if (line != null)
            {
                var lines = ReadDataLines();
                var itemToDelete = lines.FirstOrDefault(i => i.Id == line.Id);
                if (itemToDelete != null)
                {
                    lines.Remove(itemToDelete);

                    string newJsonText = JsonConvert.SerializeObject(lines, _settings);
                    string newEncryptedText = EncryptHelper.Encrypt(newJsonText, _password);

                    File.WriteAllText(_filePath, newEncryptedText);
                }
            }
        }

        public void AddOrUpdateDataLine(DataLine newItem)
        {
            var lines = ReadDataLines();
            var oldItem = lines.FirstOrDefault(i => i.Id == newItem.Id);
            if (oldItem != null)
            {
                lines.Remove(oldItem);
            }
            lines.Add(newItem);


            string newJsonText = JsonConvert.SerializeObject(lines, _settings);
            string newEncryptedText = EncryptHelper.Encrypt(newJsonText, _password);
            File.WriteAllText(_filePath, newEncryptedText);
        }

        public List<DataLine> ReadDataLines()
        {
            string decryptedText = File.ReadAllText(_filePath);
            if (!string.IsNullOrWhiteSpace(decryptedText))
            {
                string jsonText = EncryptHelper.Decrypt(decryptedText, _password);
                var data = JsonConvert.DeserializeObject<List<DataLine>>(jsonText, _settings) ?? new List<DataLine>();
                return data.ToList();
            }
            return new List<DataLine>();
        }

    }
}
