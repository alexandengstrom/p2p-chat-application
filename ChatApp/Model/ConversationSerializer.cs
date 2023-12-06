using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ChatApp.Model
{
    /// <summary>
    /// Used to save and load conversation objects into JSON, in order to save local data.
    /// </summary>
    public class ConversationSerializer
    {
        private string? hostEndpoint = null;
        private string? hostName = null;

        private readonly object _lock = new object();

        // saved in Documents/SavedChats as base
        private string baseDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SavedChats");

        public ConversationSerializer() { }

        /// <summary>
        /// Saves a single conversation.
        /// </summary>
        /// <param name="conversationModel">The conversation model to be saved.</param>
        public void Save(ConversationModel conversationModel)
        {
            InitializeHost();
            // creates a folder in the base directory, using the hosts name and the hosts endpoint as the folder name for this users conversations
            string directoryPath = Path.Combine(baseDirectory, $"{hostName}_{hostEndpoint}");
            Directory.CreateDirectory(directoryPath);

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            string filePath = Path.Combine(directoryPath, $"{conversationModel.User.Name}_{conversationModel.User.Ip}-{conversationModel.User.Port}.json");

            string json = JsonConvert.SerializeObject(conversationModel, settings);

            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Loads a single JSON file into the appropriate object.
        /// </summary>
        /// <param name="filePath">The path of the JSON file.</param>
        /// <returns>The conversation model stored in the JSON file.</returns>
        public ConversationModel Load(string filePath)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            string json = File.ReadAllText(filePath);

            ConversationModel conversation = JsonConvert.DeserializeObject<ConversationModel>(json, settings);

            return conversation;
        }

        /// <summary>
        /// Loads all historic conversations from the correct directory.
        /// </summary>
        /// <returns>A list containing all historic conversations.</returns>
        public List<ConversationModel> LoadAll() 
        {
            try
            {
                InitializeHost();
                List<ConversationModel> conversationModels = new List<ConversationModel>();

                string directoryPath = Path.Combine(baseDirectory, $"{hostName}_{hostEndpoint}");

                if (Directory.Exists(directoryPath))
                {
                    string[] files = Directory.GetFiles(directoryPath);

                    try
                    {
                        foreach (string file in files)
                        {
                            ConversationModel conversation = Load(file);
                            conversationModels.Add(conversation);
                        }
                    } catch (Exception ex) { 
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                }

                return conversationModels;

            } catch (Exception ex)
            {
                return new List<ConversationModel>();
            }
        }

        /// <summary>
        /// Helper method to ensure that the host strings are set. Builds the host strings from the NetworkManager only once.
        /// </summary>
        private void InitializeHost()
        {
            if (hostEndpoint == null || hostName == null)
            {
                hostEndpoint = NetworkManager.Instance.Host.Ip + "-" + NetworkManager.Instance.Host.Port;
                hostName = NetworkManager.Instance.Host.Name;
            }
        }
    }
}
