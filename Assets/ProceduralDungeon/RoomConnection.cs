using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoomConnection : MonoBehaviour
{
	public string _type = "";

	public bool _entrance;

	public bool _allowDoor = true;

	[NonSerialized]
	public int _placeOrder;

	private void OnDrawGizmos()
	{
		if (_entrance)
		{
			Gizmos.color = Color.white;
		}
		else
		{
			Gizmos.color = new Color(1f, 1f, 0f, 1f);
		}
		Gizmos.matrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, new Vector3(1f, 1f, 1f));
		Gizmos.DrawCube(Vector3.zero, new Vector3(2f, 0.02f, 0.2f));
		Gizmos.DrawCube(new Vector3(0f, 0f, 0.35f), new Vector3(0.2f, 0.02f, 0.5f));
		Gizmos.matrix = Matrix4x4.identity;
	}

	public bool TestContact(RoomConnection other)
	{
		return Vector3.Distance(base.transform.position, other.transform.position) < 0.1f;
	}
}
