using System;

namespace Libraries
{
	public static class Utility
	{
		public static readonly Random Random=new Random();

		public static T getRandomEnum<T>()
		{
			Array enum_values=Enum.GetValues(typeof(T));
			return (T)enum_values.GetValue(Random.Next(enum_values.Length));
		}
	}
}