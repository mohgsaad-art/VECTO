using System;
using System.Collections.Generic;
using System.IO;
using Castle.Core.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace VECTO3GUI.Helper
{
	public static class SerializeHelper
	{
		public const string DATE_FORMAT = "yyyy-MM-dd'T'HH:mm:ss'Z'";
		
		public static void SerializeToFile<T>(string filePath, T file, IContractResolver resolver = null)
		{
			if (filePath.IsNullOrEmpty() || file == null)
				return;

			var settings = new JsonSerializerSettings {
				Formatting = Formatting.Indented,
				DateFormatString = DATE_FORMAT,
				NullValueHandling = NullValueHandling.Ignore
			};
			
			if (resolver != null)
				settings.ContractResolver = resolver;
			
			var serializedObject = JsonConvert.SerializeObject(file, settings);
			File.WriteAllText(filePath, serializedObject);
		}

		public static T DeserializeToObject<T>(string filePath, IContractResolver resolver = null)
		{
			if (filePath.IsNullOrEmpty())
				return default(T);

			using (var file = File.OpenText(filePath)) {
				var serializer = new JsonSerializer();
				serializer.DateFormatString = DATE_FORMAT; 

				if (resolver != null)
					serializer.ContractResolver = resolver;
				
				var result = serializer.Deserialize(file,typeof(T));
				if (result == null)
					return default(T);

				return (T)result;
			}
		}

	}
}
