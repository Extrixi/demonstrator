using System.Collections;
using System.Runtime.Serialization;
using UnityEngine;

public class QuaternionSerializationSurrogate : ISerializationSurrogate
{
	public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
	{
		Quaternion quanternion = (Quaternion)obj;
		info.AddValue("x", quanternion.x);
		info.AddValue("y", quanternion.y);
		info.AddValue("z", quanternion.z);
		info.AddValue("w", quanternion.w);
	}

	public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
	{
		Quaternion quaternion = (Quaternion)obj;
		quaternion.x = (float)info.GetValue("x", typeof(float));
		quaternion.y = (float)info.GetValue("y", typeof(float));
		quaternion.z = (float)info.GetValue("z", typeof(float));
		quaternion.w = (float)info.GetValue("w", typeof(float));
		obj = quaternion;
		return obj;
	}
}

//      _                 _ _                     
//     | |               (_) |                    
//   __| | ___  _ __ ___  _| |__  _ __ ___  _ __  
//  / _` |/ _ \| '_ ` _ \| | '_ \| '__/ _ \| '_ \ 
// | (_| | (_) | | | | | | | |_) | | | (_) | | | |
//  \__,_|\___/|_| |_| |_|_|_.__/|_|  \___/|_| |_|
// + Python