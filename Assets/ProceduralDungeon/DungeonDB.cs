using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonDB : MonoBehaviour
{
    public class RoomData
    {
        public Room room;
        //[NonSerialized]
        //public List<RandomSpawn> _randomSpawns = new List<RandomSpawn>();

        public override string ToString()
        {
            return room.ToString();
        }
    }

	public List<string> roomScenes = new List<string>();

	private List<RoomData> _rooms = new List<RoomData>();
	private Dictionary<int, RoomData> _roomByHash = new Dictionary<int, RoomData>();

	private static DungeonDB _instance;
	public static DungeonDB instance
	{
		get
		{
			return _instance;
		}
	}

	private void Awake()
	{
		_instance = this;
		foreach (string roomScene in roomScenes)
		{
			SceneManager.LoadScene(roomScene, LoadSceneMode.Additive);
		}
	}

	private void Start()
	{
		Debug.Log("DungeonDB Start " + Time.frameCount);
		_rooms = SetupRooms();
		GenerateHashList();
	}

	public static List<RoomData> GetRooms()
	{
		Debug.Log(_instance._rooms.Count);
		return _instance._rooms;
	}

	private static List<RoomData> SetupRooms()
	{
		// Room[] array = GameObject.FindObjectsOfType<Room>(false);
		Room[] array = Resources.FindObjectsOfTypeAll<Room>();
		List<RoomData> list = new List<RoomData>();

		foreach (Room room in array)
		{
			if (room == null || (_instance && room.gameObject.activeSelf))
			{
				Debug.Log("room missing or its enabled");
			}

            RoomData roomData = new RoomData();
            roomData.room = room;
            list.Add(roomData);
        }
		Debug.Log("Setup Rooms Done!" + list.Count);
		return list;
	}

	public RoomData GetRoom(int hash)
	{
		RoomData value;
		if (_roomByHash.TryGetValue(hash, out value))
		{
			return value;
		}
		return null;
	}

	private void GenerateHashList()
	{
		_roomByHash.Clear();
		foreach (RoomData room in _rooms)
		{
			int stableHashCode = room.room.gameObject.name.GetStableHashCode();
			if (_roomByHash.ContainsKey(stableHashCode))
			{
				
			}
			else
			{
				_roomByHash.Add(stableHashCode, room);
			}
		}
	}
}
