using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChatApp.Model
{
    /// <summary>
    /// This wrapper allows protocol information to be added to any datamodel.
    /// </summary>
    internal class ProtocolWrapper
    {
        public DataModel dataModel { get; set; }
        public string protocol { get; private set; } = "P2P_TDDD49";
        public string version { get; private set; } = "1.0";

        /// <summary>
        /// Wraps the datamodel in a wrapper.
        /// </summary>
        /// <param name="dataModel">The datamodel to wrap.</param>
        public ProtocolWrapper(DataModel dataModel) 
        { 
            this.dataModel = dataModel;
        }

        public DataModel DataModel { get { return dataModel; } }
    }

    /// <summary>
    /// This class allows the NetworkManager to encode messages into bytesarrays, and decodes bytearrays into the correct object.
    /// </summary>
    public class Protocol
    {
        private string protocol = "P2P_TDDD49";
        private string version = "1.0";

        public Protocol() { }

        /// <summary>
        /// Used to encode any DataModel into a bytearray.
        /// </summary>
        /// <typeparam name="T">To enable polymorphic behaviour, we template the input of the method.</typeparam>
        /// <param name="dataModel">The datamodel to encode.</param>
        /// <returns>A bytearray representing the encoded datamodel.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the bytearray is too large to be sent through the pre-defined buffer size of the network manager.</exception>
        public byte[] Encode<T>(T dataModel) where T : DataModel
        {
            ProtocolWrapper wrapper = new ProtocolWrapper(dataModel);

            // enables easy decoding of the json object
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };

            // converts the object to JSON, then the JSON into a bytearray
            string json = JsonConvert.SerializeObject(wrapper, settings);
            byte[] byteArray = Encoding.UTF8.GetBytes(json);

            if (byteArray.Length > 4096)
            {
                throw new ArgumentOutOfRangeException();
            }
            return byteArray;
        }

        /// <summary>
        /// Used to decode a bytearray into the correct object.
        /// </summary>
        /// <param name="byteArray">The bytearray to decode.</param>
        /// <returns>The decoded DataModel.</returns>
        /// <exception cref="ArgumentException">Thrown when the protocol wrapper does not contain the correct protocol name or version.</exception>
        /// <exception cref="JsonSerializationException">Thrown when there is an issue decoding the bytearray into an object.</exception>
        public DataModel Decode(byte[] byteArray)
        {
            ProtocolWrapper? wrapper = null;

            // enables proper serialization into a proper object
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            string json = Encoding.UTF8.GetString(byteArray);

            wrapper = JsonConvert.DeserializeObject<ProtocolWrapper>(json, settings);

            if (wrapper.protocol != protocol || wrapper.version != version)
            {
                throw new ArgumentException();
            }

            DataModel dataModel = wrapper.DataModel;

            return dataModel;
        }
    }
}
