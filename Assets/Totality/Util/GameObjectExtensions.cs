using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class GameObjectExtensions
{
	public static GameObject InstantiateGameObject(this GameObject i_gameObject)
	{
		return (GameObject)GameObject.Instantiate(i_gameObject);
	}

	public static GameObject InstantiateGameObject(this GameObject i_gameObject, Transform i_parent)
	{
		return (GameObject)GameObject.Instantiate(i_gameObject, i_parent);
	}

	public static GameObject InstantiateGameObject(this GameObject i_gameObject, Transform i_parent, bool i_worldPositionStays)
	{
		return (GameObject)GameObject.Instantiate(i_gameObject, i_parent, i_worldPositionStays);
	}

	public static GameObject InstantiateGameObject(this GameObject i_gameObject, Vector3 i_positionWS, Quaternion i_rotationWS)
	{
		return (GameObject)GameObject.Instantiate(i_gameObject, i_positionWS, i_rotationWS);
	}

	public static GameObject InstantiateGameObject(this GameObject i_gameObject, Vector3 i_positionWS, Quaternion i_rotationWS, Transform i_parent)
	{
		return (GameObject)GameObject.Instantiate(i_gameObject, i_positionWS, i_rotationWS, i_parent);
	}

	public static IEnumerable<T> GetComponentsInDescendents<T>(this GameObject i_gameObject)
	{
		return i_gameObject.transform.GetComponentsInDescendents<T>();
	}

	public static IEnumerable<T> GetComponentsInSelfOrDescendents<T>(this GameObject i_gameObject)
	{
		return i_gameObject.transform.GetComponentsInSelfOrDescendents<T>();
	}

	public static IEnumerable<T> GetComponentsInAncestors<T>(this GameObject i_gameObject)
	{
		return i_gameObject.transform.GetComponentsInAncestors<T>();
	}

	public static T GetComponentInAncestors<T>(this GameObject i_gameObject)
	{
		return i_gameObject.transform.GetComponentInAncestors<T>();
	}

	public static IEnumerable<GameObject> GetChildren(this GameObject i_gameObject)
	{
		for (int i = 0; i < i_gameObject.transform.childCount; ++i)
		{
			yield return i_gameObject.transform.GetChild(i).gameObject;
		}
	}

	public static IEnumerable<GameObject> GetDescendents(this GameObject i_gameObject)
	{
		return i_gameObject.transform.GetDescendents().Select(t => t.gameObject);
	}

	public static IEnumerable<GameObject> FindDescendentsWithTag(this GameObject i_gameObject, string i_tag)
	{
		return i_gameObject.GetDescendents().Where(go => (go.tag == i_tag));
	}

	public static GameObject FindDescendentWithTag(this GameObject i_gameObject, string i_tag)
	{
		return i_gameObject.FindDescendentsWithTag(i_tag).FirstOrDefault();
	}

	public static void ReparentChildren(this GameObject i_gameObject, Transform i_newParent)
	{
		i_gameObject.transform.ReparentChildren(i_newParent);
	}

	public static void DestroyChildren(this GameObject i_gameObject)
	{
		i_gameObject.GetChildren().ToList().ForEach(child => GameObject.Destroy(child));
	}

	public static void DestroyChildrenImmediate(this GameObject i_gameObject)
	{
		i_gameObject.GetChildren().ToList().ForEach(child => GameObject.DestroyImmediate(child));
	}

	public static void ReparentAndDestroyChildren(this GameObject i_gameObject, Transform i_endOfLifeParent)
	{
		i_gameObject.GetChildren().ToList().ForEach
		(
			child =>
			{
				child.transform.SetParent(i_endOfLifeParent);
				GameObject.Destroy(child);
			}
		);
	}

	public static T EnsureComponent<T>(this GameObject i_gameObject) where T : Component
	{
		T t = i_gameObject.GetComponent<T>();
		if (!t)
		{
			t = i_gameObject.AddComponent<T>();
		}
		return t;
	}

	public static void GetComponent<T>(this GameObject i_gameObject, out T o_component) where T : Component
	{
		o_component = i_gameObject.GetComponent<T>();
	}
}
