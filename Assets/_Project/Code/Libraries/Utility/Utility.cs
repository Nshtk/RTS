using System;
using UnityEngine;

namespace Libraries
{
	public static class Utility
	{
		public static readonly System.Random Random =new System.Random();

		public static T getRandomEnum<T>()
		{
			Array enum_values=Enum.GetValues(typeof(T));
			return (T)enum_values.GetValue(Random.Next(enum_values.Length));
		}
		public static Vector3 getRandomPointInCollider(BoxCollider collider)
		{
			Vector3 extents = collider.size / 2f;
			Vector3 point = new Vector3(
				UnityEngine.Random.Range(-extents.x, extents.x),
				UnityEngine.Random.Range(-extents.y, extents.y),
				UnityEngine.Random.Range(-extents.z, extents.z)
			)  + collider.center;
			return collider.transform.TransformPoint(point);
		}
	}
}