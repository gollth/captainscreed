using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ResourceLoader : MonoBehaviour {

	static List<GameObject> loaded = new List<GameObject>();
	public static event Action<GameObject> OnLoaded;
	public static bool IsLoaded(GameObject go) { 
		RemoveDestroyed ();
		return loaded.Contains(go); 
	}
	public static void WaitFor(string tag, Action<GameObject> callback) {
		WaitFor (go => go.CompareTag (tag), callback);
	}
	public static void WaitFor<T>(string tag, Action<T> callback) where T : MonoBehaviour {
		WaitFor<T> (go => go.CompareTag (tag), callback);
	}
	public static void WaitFor<T>(Func<GameObject, bool> filter, Action<T> callback) where T : MonoBehaviour {
		WaitFor (filter, callback: go => go.GetComponent<T> ());
	}
	public static void WaitFor(Func<GameObject, bool> filter, Action<GameObject> callback) {
		RemoveDestroyed ();
		var g = loaded.FirstOrDefault (filter);
		if (g != null) callback (g);
		else OnLoaded += go => { if (filter (go)) callback (go); };
	}

	static void RemoveDestroyed() {
		loaded = loaded.Where (item => item != null).ToList();
	}

	[SerializeField] string asset;
	[SerializeField] bool alignWithMe = true;

	GameObject go;

	void OnEnable() {
		SceneManager.sceneLoaded += OnLevelLoad;
		if (asset == null || asset == "") return;
		Load (Resources.Load (asset) as GameObject);
		
	}
	void OnDisable() { 
		SceneManager.sceneLoaded -= OnLevelLoad;
		Destroy (go); 
	}

	void OnLevelLoad (Scene scene, LoadSceneMode mode) {
		loaded.Clear();
	}

	public GameObject Load(GameObject prefab) {
		if (prefab == null) {
			Debug.LogErrorFormat ("[{0}@{1}]: cannot find resource {2}", GetType ().Name, name, asset);
			return null;
		}

		go = Instantiate(prefab) as GameObject;
		go.transform.SetParent (this.transform, !alignWithMe);
		loaded.Add (go);
		if (OnLoaded != null) OnLoaded.Invoke (go);
		return go;
	}

}
