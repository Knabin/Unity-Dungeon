using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensionMethods
{
	public static void Shuffle<T>(this IList<T> list)
	{
		int i = list.Count;
		while (i > 1)
		{
			i--;
			int index =Random.Range(0, i);
			T value = list[index];
			list[index] = list[i];
			list[i] = value;
		}
	}
}
