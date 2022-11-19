using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class DungeonGenerator : MonoBehaviour
{

	[HideInInspector]
	public int generatedSeed;
	public int generatedHash;

	private static List<Room> _placedRooms = new List<Room>();
	private static List<RoomConnection> _openConnections = new List<RoomConnection>();
	private static List<RoomConnection> _doorConnections = new List<RoomConnection>();
	private static List<DungeonDB.RoomData> _availableRooms = new List<DungeonDB.RoomData>();
	private static List<DungeonDB.RoomData> _tempRooms = new List<DungeonDB.RoomData>();

	public int maxRooms = 3;
	public int minRooms = 20;
	public int minRequiredRooms;

	public Room.Theme themes = Room.Theme.Crypt;

	private BoxCollider _colliderA;
	private BoxCollider _colliderB;
	// Start is called before the first frame update
	void Start()
	{
		// Generate();
	}

	// Update is called once per frame
	void Update()
	{

	}

    private void OnEnable()
    {
		// 임시 코드
		Generate();
		for (int i = 0; i < base.transform.childCount; i++)
		{
			base.transform.GetChild(i).gameObject.SetActive(true);
		}
	}

    private void OnDisable()
    {
		Clear();
    }






    public void Generate()
	{
		int seed = GetSeed();
		Generate(seed);
	}

	public int GetSeed()
	{
		return 500;
		//return 300;   // test
	}

	public void Generate(int seed)
	{
		generatedSeed = seed;
		Clear();

		SetupColliders();

		SetupAvailableRooms();

		_placedRooms.Clear();
		_openConnections.Clear();
		_doorConnections.Clear();
		
		// UnityEngine.Random.State state = UnityEngine.Random.state;
		// UnityEngine.Random.InitState(seed);
		
		GenerateRooms();

		Debug.Log("Placed " + _placedRooms.Count + " rooms");
		string text = "";
		foreach (Room placedRoom in _placedRooms)
		{
			text += placedRoom.name;
		}

		generatedHash = text.GetHashCode();
		Debug.Log(string.Format("Dungeon generated with seed {0} and hash {1}", seed, generatedHash));

		// UnityEngine.Random.state = state;


		_placedRooms.Clear();
		_openConnections.Clear();
		_doorConnections.Clear();

		DestroyImmediate(_colliderA);
		DestroyImmediate(_colliderB);
	}
	private void SetupColliders()
	{
		if (!(_colliderA != null))
		{
			BoxCollider[] componentsInChildren = base.gameObject.GetComponentsInChildren<BoxCollider>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				DestroyImmediate(componentsInChildren[i]);
			}
			_colliderA = base.gameObject.AddComponent<BoxCollider>();
			_colliderB = base.gameObject.AddComponent<BoxCollider>();
		}
	}

	private void GenerateRooms()
	{
		PlaceStartRoom();
		PlaceRooms();
		PlaceEndCaps();
		//PlaceDoors();
	}

	private void PlaceStartRoom()
	{
		DungeonDB.RoomData roomData = FindStartRoom();
		RoomConnection entrance = roomData.room.GetEntrance();
		Quaternion rotation = base.transform.rotation;
		Vector3 pos;
		Quaternion rot;
		CalculateRoomPosRot(entrance, base.transform.position, rotation, out pos, out rot);
		PlaceRoom(roomData, pos, rot, entrance);
	}

	private DungeonDB.RoomData FindStartRoom()
	{
		_tempRooms.Clear();
		foreach (DungeonDB.RoomData availableRoom in _availableRooms)
		{
			if (availableRoom.room.entrance)
			{
				Debug.Log("ADD!!");
				_tempRooms.Add(availableRoom);
			} else Debug.Log("ELSE!!");
		}
		Debug.Log(_tempRooms.Count);
		return _tempRooms[UnityEngine.Random.Range(0, _tempRooms.Count)];
	}

	private void CalculateRoomPosRot(RoomConnection roomCon, Vector3 exitPos, Quaternion exitRot, out Vector3 pos, out Quaternion rot)
	{
		Debug.Log(roomCon);
		Quaternion quaternion = Quaternion.Inverse(roomCon.transform.localRotation);
		rot = exitRot * quaternion;
		Vector3 localPosition = roomCon.transform.localPosition;
		pos = exitPos - rot * localPosition;
	}

	private void PlaceRoom(DungeonDB.RoomData room, Vector3 pos, Quaternion rot, RoomConnection fromConnection)
	{
		Vector3 vector = pos;
		int seed = (int)vector.x * 4271 + (int)vector.y * 9187 + (int)vector.z * 2134;
		UnityEngine.Random.State state = UnityEngine.Random.state;
		UnityEngine.Random.InitState(seed);

		//foreach (RandomSpawn randomSpawn2 in room.m_randomSpawns)
		//{
		//	randomSpawn2.Randomize();
		//}
		
		Room component2 = Instantiate(room.room.gameObject, pos, rot, base.transform).GetComponent<Room>();
		component2.gameObject.name = room.room.gameObject.name;
		component2._placeOrder = (fromConnection ? (fromConnection._placeOrder + 1) : 0);
		component2._seed = seed;
		_placedRooms.Add(component2);
		AddOpenConnections(component2, fromConnection);
	
		UnityEngine.Random.state = state;
	}

	private void AddOpenConnections(Room newRoom, RoomConnection skipConnection)
	{
		RoomConnection[] connections = newRoom.GetConnections();
		if (skipConnection != null)
		{
			RoomConnection[] array = connections;
			foreach (RoomConnection roomConnection in array)
			{
				if (!roomConnection._entrance && !(Vector3.Distance(roomConnection.transform.position, skipConnection.transform.position) < 0.1f))
				{
					roomConnection._placeOrder = newRoom._placeOrder;
					_openConnections.Add(roomConnection);
				}
			}
		}
		else
		{
			RoomConnection[] array = connections;
			for (int i = 0; i < array.Length; i++)
			{
				array[i]._placeOrder = newRoom._placeOrder;
			}
			_openConnections.AddRange(connections);
		}
	}

	private void PlaceRooms()
	{
		for (int i = 0; i < maxRooms; i++)
		{
			PlaceOneRoom();
			//if (CheckRequiredRooms() && m_placedRooms.Count > m_minRooms)
			//{
			//	break;
			//}
		}
	}
	private bool PlaceOneRoom()
	{
		RoomConnection openConnection = GetOpenConnection();
		if (openConnection == null)
		{
			return false;
		}
		for (int i = 0; i < 10; i++)
		{
			DungeonDB.RoomData roomData = GetRandomRoom(openConnection);
			if (roomData == null)
			{
				break;
			}
			if (PlaceRoom(openConnection, roomData))
			{
				return true;
			}
		}
		return false;
	}

	private bool PlaceRoom(RoomConnection connection, DungeonDB.RoomData roomData)
	{
		Room room = roomData.room;
		Quaternion rotation = connection.transform.rotation;
		rotation *= Quaternion.Euler(0f, 180f, 0f);
		RoomConnection connection2 = room.GetConnection(connection);

		Vector3 pos;
		Quaternion rot;
		CalculateRoomPosRot(connection2, connection.transform.position, rotation, out pos, out rot);
		if (room.size.x != 0 && room.size.z != 0 && TestCollision(room, pos, rot))
		{
			return false;
		}
		PlaceRoom(roomData, pos, rot, connection);
		if (!room.endCap)
		{
			if (connection._allowDoor)
			{
				_doorConnections.Add(connection);
			}
			_openConnections.Remove(connection);
		}
		return true;
	}

	private void PlaceEndCaps()
	{
		for (int i = 0; i < DungeonGenerator._openConnections.Count; i++)
		{
			RoomConnection roomConnection = DungeonGenerator._openConnections[i];
			this.FindEndCaps(roomConnection, DungeonGenerator._tempRooms);


			bool flag = false;
			IOrderedEnumerable<DungeonDB.RoomData> orderedEnumerable = from item in DungeonGenerator._tempRooms
																	   orderby item.room.endCapPrio descending
																	   select item;
			foreach (DungeonDB.RoomData roomData in orderedEnumerable)
			{
				if (this.PlaceRoom(roomConnection, roomData))
				{
					flag = true;
					break;
				}
			}
				
			if (!flag)
			{
				Debug.Log("Failed to place end cap " + roomConnection.name + " " + roomConnection.transform.parent.gameObject.name);
			}
			
		}
	}

private void FindEndCaps(RoomConnection connection, List<DungeonDB.RoomData> rooms)
{
	rooms.Clear();
	foreach (DungeonDB.RoomData roomData in DungeonGenerator._availableRooms)
	{
		if (roomData.room.endCap && roomData.room.HaveConnection(connection))
		{
			rooms.Add(roomData);
		}
	}
	rooms.Shuffle();
}

private RoomConnection GetOpenConnection()
	{
		if (_openConnections.Count == 0)
		{
			return null;
		}
		return _openConnections[UnityEngine.Random.Range(0, _openConnections.Count)];
	}

	private void SetupAvailableRooms()
	{
		_availableRooms.Clear();
		foreach (DungeonDB.RoomData room in DungeonDB.GetRooms())
		{
			if ((room.room.theme & themes) != 0 && room.room._enabled)
			{
				Debug.Log("Add!");
				_availableRooms.Add(room);
			}
			else Debug.Log("Else!");
		}
	}

	private DungeonDB.RoomData GetRandomRoom(RoomConnection connection)
	{
		_tempRooms.Clear();
		foreach (DungeonDB.RoomData availableRoom in _availableRooms)
		{
			if (!availableRoom.room.entrance && !availableRoom.room.endCap && (!connection || (availableRoom.room.HaveConnection(connection) && connection._placeOrder >= availableRoom.room.minPlaceOrder)))
			{
				_tempRooms.Add(availableRoom);
			}
		}
		if (_tempRooms.Count == 0)
		{
			return null;
		}
		return _tempRooms[UnityEngine.Random.Range(0, _tempRooms.Count)];
	}
	public void Clear()
	{
		while (base.transform.childCount > 0)
		{
			UnityEngine.Object.DestroyImmediate(base.transform.GetChild(0).gameObject);
		}
	}

	private bool TestCollision(Room room, Vector3 pos, Quaternion rot)
	{
		//if (!IsInsideDungeon(room, pos, rot))
		//{
		//	return true;
		//}
		_colliderA.size = new Vector3((float)room.size.x - 0.1f, (float)room.size.y - 0.1f, (float)room.size.z - 0.1f);
		foreach (Room placedRoom in _placedRooms)
		{
			_colliderB.size = placedRoom.size;
			Vector3 direction;
			float distance;
			if (Physics.ComputePenetration(_colliderA, pos, rot, _colliderB, placedRoom.transform.position, placedRoom.transform.rotation, out direction, out distance))
			{
				return true;
			}
		}
		return false;
	}
}
