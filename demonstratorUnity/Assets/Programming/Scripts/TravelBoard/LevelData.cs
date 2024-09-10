// using System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class LevelData
{
	public enum LevelType
	{
		Hub = 0,
		Level = 1
	}

	/// <summary>
	/// Used for Identification and optimisation on finding a level in the level list.
	/// </summary>
	[System.Serializable]
	public struct IDKey
	{
		public int FirstID;

		[Tooltip("-1 is for hub")]
		public int LastID;

		/// <summary>
		/// Creates a new ID key to be used in the level list.
		/// </summary>
		/// <param name="firstID">The unique id to the level or hub.</param>
		/// <param name="lastID">Used for levels for level ids, -1 if for hub or leave blank.</param>
		public IDKey(int firstID, int lastID = -1)
		{
			FirstID = firstID;
			LastID = lastID;
		}

		public override string ToString()
		{
			return $"[{FirstID}, {LastID}]";
		}

		public override int GetHashCode()
		{
			return ShiftAndWrap(FirstID.GetHashCode(), 2) ^ LastID.GetHashCode();
		}

		public override bool Equals(object obj)
		{

			if (obj == null || !(obj is IDKey)) return false;



			return (this.FirstID == ((IDKey)obj).FirstID && this.LastID == ((IDKey)obj).LastID);

		}

		private int ShiftAndWrap(int value, int key)
		{
			key = key & 0x1F;

			// Save the existing bit pattern, but interpret it as an unsigned integer.
			uint number = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
			// Preserve the bits to be discarded.
			uint wrapped = number >> (32 - key);
			// Shift and wrap the discarded bits.
			return BitConverter.ToInt32(BitConverter.GetBytes((number << key) | wrapped), 0);
		}

		public static bool operator ==(IDKey lhs, IDKey rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(IDKey lhs, IDKey rhs)
		{
			return !(lhs == rhs);
		}
	}

	/// <summary>
	/// The data for levels, so we can get the scene's name and load it.
	/// </summary>
	[System.Serializable]
	public struct LevelInfo
	{
		public LevelType levelType;

		public IDKey Key;

		public string Name;

		public string First;
		public string Last;

		public LevelInfo(LevelType levelType, int firstID, int lastID, string name, string first, string last)
		{
			this.levelType = levelType;

			Key.FirstID = firstID;

			Key.LastID = lastID;

			Name = name;

			First = first;

			Last = last;
		}

		public LevelInfo(LevelType levelType, IDKey key, string name, string first, string last)
		{
			this.levelType = levelType;

			Key = key;

			Name = name;

			First = first;

			Last = last;
		}
	}


	/// <summary>
	/// Special list that has levels used in game.
	/// DO NOT MODIFY AT RUNTIME.
	/// </summary>
	public static Dictionary<IDKey, LevelInfo> AllLevels = new Dictionary<IDKey, LevelInfo>()
	{
		{
			new IDKey(-1),
			new LevelInfo (LevelType.Hub, -1, -1, "Domi", "Domi", "Domi")
		},
		{
			new IDKey(0),
			new LevelInfo (LevelType.Hub, 0, -1, "Hub_0_Westmonton", "Hub", "Westmonton")
		},
		{
			new IDKey(0,0),
			new LevelInfo (LevelType.Level, 0, 0, "Level_0.0_Tutorial", "Level", "Tutorial")
		},
		{
			new IDKey(0,1),
			new LevelInfo(LevelType.Level, 0, 1, "Level_0.1_AbandonedWarehouse", "Level", "AbandonedWarehouse")
		},
		{
			new IDKey(0,2),
			new LevelInfo(LevelType.Level, 0, 2, "Level_0.2_Underpass", "Level", "Underpass")
		},
		{
			new IDKey(0, 3),
			new LevelInfo(LevelType.Level, 0, 3, "Level_0.3_Tracks", "Level", "Tracks")
		},
		{
			new IDKey(1),
			new LevelInfo(LevelType.Level, 1, -1, "Hub_1_Dunfall", "Hub", "Dunfall")
		}
	};



	public static LevelInfo? GetlevelByName(string name)
	{
		foreach (LevelInfo levelInfo in AllLevels.Values)
		{
			if (levelInfo.Name == name) return levelInfo;
		}

		return null;
	}

	public static LevelInfo? GetLevelByKey(IDKey key)
	{
		if (AllLevels.ContainsKey(key)) return AllLevels[key];
		else return null;
	}

	public static LevelInfo? GetLevelByKey(int firstID, int lastID = -1)
	{
		IDKey key = new IDKey(firstID, lastID);

		if (AllLevels.ContainsKey(key)) return AllLevels[key];
		else return null;
	}

#nullable enable
	public static string? GetLevelNameByKey(IDKey key)
	{
		if (AllLevels.ContainsKey(key)) return AllLevels[key].Name;
		else return null;
	}
#nullable restore

	public static bool CheckIfKeyExsists(IDKey key)
	{
		return AllLevels.ContainsKey(key);
	}


}
