using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BuffetAuxiliaryLib.DTOs
{
    public abstract class SerDes<T> where T : class
    {
        [JsonIgnore]
        private string _jsonString;

        public string Serialize()
        {
            string jsonString = JsonConvert.SerializeObject(this);
            this._jsonString = jsonString;
            return jsonString;
        }

        public T Deserialize()
        {
            if (_jsonString != null)
            {
                T obj = JsonConvert.DeserializeObject<T>(_jsonString);
                _jsonString = null;
                return obj;
            }
            else return null;
        }
    }
}
