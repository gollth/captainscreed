using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(Image))]
public class Gif : MonoBehaviour {

	[SerializeField] string folder;
	[SerializeField, Range(1, 60)] float fps = 25;


	Sprite[] frames;
	Image image;
	float timer;

	// Use this for initialization
	void OnEnable () {
		image = GetComponent<Image> ();
		frames = Resources.LoadAll<Sprite> (folder);
		timer = 0;
	}

	// Update is called once per frame
	void Update () {
		timer += fps * Time.deltaTime;	// 25 Hz Animation rate
		if (timer >= frames.Length) timer -= frames.Length;
		image.sprite = frames [(int)timer];
	}
}
