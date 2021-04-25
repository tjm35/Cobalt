using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class TransformExtensions
{
	public static IEnumerable<T> GetComponentsInDescendents<T>(this Transform i_transform)
	{
		for (int i = 0; i < i_transform.childCount; ++i)
		{
			Transform c = i_transform.GetChild(i);
			foreach (T t in c.GetComponentsInSelfOrDescendents<T>())
			{
				yield return t;
			}
		}
	}

	public static IEnumerable<T> GetComponentsInSelfOrDescendents<T>(this Transform i_transform)
	{
		foreach (T t in i_transform.GetComponents<T>())
		{
			yield return t;
		}
		for (int i = 0; i < i_transform.childCount; ++i)
		{
			Transform c = i_transform.GetChild(i);
			foreach (T t in c.GetComponentsInSelfOrDescendents<T>())
			{
				yield return t;
			}
		}
	}

	public static IEnumerable<T> GetComponentsInSelfOrAncestors<T>(this Transform i_transform)
	{
		Transform parent = i_transform;
		while (parent)
		{
			foreach (T t in parent.GetComponents<T>())
			{
				yield return t;
			}
			parent = parent.parent;
		}
	}

	public static IEnumerable<T> GetComponentsInAncestors<T>(this Transform i_transform)
	{
		return i_transform.parent.GetComponentsInSelfOrAncestors<T>();
	}

	public static T GetComponentInAncestors<T>(this Transform i_transform)
	{
		return i_transform.GetComponentsInAncestors<T>().FirstOrDefault();
	}

	public static T GetComponentInSelfOrAncestors<T>(this Transform i_transform)
	{
		return i_transform.GetComponentsInSelfOrAncestors<T>().FirstOrDefault();
	}

	public static IEnumerable<Transform> GetChildren(this Transform i_transform)
	{
		for (int i = 0; i < i_transform.childCount; ++i)
		{
			yield return i_transform.GetChild(i);
		}
	}

	public static IEnumerable<Transform> GetDescendents(this Transform i_transform)
	{
		for (int i = 0; i < i_transform.childCount; ++i)
		{
			Transform c = i_transform.GetChild(i);
			yield return c;
			foreach (Transform t in c.GetDescendents())
			{
				yield return t;
			}
		}
	}

	public static bool IsDescendentOf(this Transform i_transform, Transform i_potentialAncestor)
	{
		Transform parent = i_transform.parent;
		while (parent)
		{
			if (parent == i_potentialAncestor)
			{
				return true;
			}
			parent = parent.parent;
		}
		return false;
	}

	public static void ReparentChildren(this Transform i_transform, Transform i_newParent)
	{
		i_transform.GetChildren().ToList().ForEach(child => child.SetParent(i_newParent));
	}
}
