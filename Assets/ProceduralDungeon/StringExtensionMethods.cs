using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringExtensionMethods
{
	// https://referencesource.microsoft.com/#system.servicemodel/System/ServiceModel/StringUtil.cs,9e8fa99987f83da8,references

	public static int GetStableHashCode(this string str)
	{
		int num = 5381;
		int num2 = num;
		for (int i = 0; i < str.Length && str[i] != 0; i += 2)
		{
			num = ((num << 5) + num) ^ str[i];
			if (i == str.Length - 1 || str[i + 1] == '\0')
			{
				break;
			}
			num2 = ((num2 << 5) + num2) ^ str[i + 1];
		}
		return num + num2 * 1566083941;
	}
}
