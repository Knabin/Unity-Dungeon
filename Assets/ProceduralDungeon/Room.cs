using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Room : MonoBehaviour
{
	public enum Theme
	{
		Crypt = 1,
		Cave = 2,
	}

	private static List<RoomConnection> tempConnections = new List<RoomConnection>();
	public bool _enabled = true;
	public Vector3Int size = new Vector3Int(8, 4, 8);
	public Theme theme = Theme.Crypt;
	public bool entrance;
	public bool endCap;
	public int endCapPrio;

	public int minPlaceOrder;

	[NonSerialized]
	public int _placeOrder;

	[NonSerialized]
	public int _seed;

	private RoomConnection[] _roomConnections;

	private void OnDrawGizmos()
	{
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
		Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, new Vector3(1f, 1f, 1f));
		Gizmos.DrawWireCube(Vector3.zero, new Vector3(size.x, size.y, size.z));
		Gizmos.matrix = Matrix4x4.identity;
	}

	public RoomConnection[] GetConnections()
	{
		if (_roomConnections == null)
		{
			_roomConnections = GetComponentsInChildren<RoomConnection>(false);
		}
		return _roomConnections;
	}

	public RoomConnection GetConnection(RoomConnection other)
	{
		RoomConnection[] connections = GetConnections();
		tempConnections.Clear();
		RoomConnection[] array = connections;
		foreach (RoomConnection roomConnection in array)
		{
			if (roomConnection._type == other._type)
			{
				tempConnections.Add(roomConnection);
			}
		}
		if (tempConnections.Count == 0)
		{
			return null;
		}
		return tempConnections[UnityEngine.Random.Range(0, tempConnections.Count)];
	}

	public RoomConnection GetEntrance()
	{
		RoomConnection[] connections = GetConnections();
		Debug.Log("Connections " + connections.Length);
		RoomConnection[] array = connections;
		foreach (RoomConnection roomConnection in array)
		{
			if (roomConnection._entrance)
			{
				return roomConnection;
			}
		}
		return null;
	}

	public bool HaveConnection(RoomConnection other)
	{
		RoomConnection[] connections = GetConnections();
		for (int i = 0; i < connections.Length; i++)
		{
			if (connections[i]._type == other._type)
			{
				return true;
			}
		}
		return false;
	}
}
