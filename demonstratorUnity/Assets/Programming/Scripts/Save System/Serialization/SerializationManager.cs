using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerializationManager : MonoBehaviour
{
	public static bool Save(string saveName, object saveData)
	{
		BinaryFormatter formatter = new BinaryFormatter();

		if (!Directory.Exists(Application.persistentDataPath + "/saves"))
		{
			Directory.CreateDirectory(Application.persistentDataPath + "/saves");
		}

		string path = Application.persistentDataPath + "/saves/" + saveName + ".save";

		FileStream file = File.Create(path);

		formatter.Serialize(file, saveData);

		file.Close();

		return true;
	}

	public static object Load(string path)
	{
		if (!File.Exists(path))
		{
			return null;
		}

		BinaryFormatter formatter = GetBinaryFormatter();

		FileStream file = File.Open(path, FileMode.Open);

		try
		{
			object save = formatter.Deserialize(file);
			file.Close();
			return save;
		}
		catch
		{
			Debug.LogErrorFormat("Failed to load file at {0}", path);
			file.Close();
			return null;
		}
	}

	public static BinaryFormatter GetBinaryFormatter()
	{
		BinaryFormatter formatter = new BinaryFormatter();

		SurrogateSelector selector = new SurrogateSelector();

		Vector3SerializationSurrogate vector3Surrogate = new Vector3SerializationSurrogate();
		QuaternionSerializationSurrogate QuaternionSurrogate = new QuaternionSerializationSurrogate();

		selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3Surrogate);
		selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), QuaternionSurrogate);

		formatter.SurrogateSelector = selector;

		return formatter;
	}


}

//      _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
// + Python