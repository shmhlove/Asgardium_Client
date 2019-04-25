using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class SHCustomJsonSerializer : JWT.IJsonSerializer
{
        /// <summary>
        /// Serialize an object to JSON string
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>JSON string</returns>
        public string Serialize(object obj)
        {
            return SHUtils.GetJsonStringFromObject<Dictionary<string, object>>(obj as Dictionary<string, object>);
        }

        /// <summary>
        /// Deserialize a JSON string to typed object.
        /// </summary>
        /// <typeparam name="T">type of object</typeparam>
        /// <param name="json">JSON string</param>
        /// <returns>typed object</returns>
        public T Deserialize<T>(string json)
        {
            return SHUtils.GetObjectFromJsonString<T>(json);
        }
}
