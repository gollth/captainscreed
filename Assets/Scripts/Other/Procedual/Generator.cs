using UnityEngine;
using System;
using System.Linq;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer)), ExecuteInEditMode]
public class Generator : MonoBehaviour {

	#region Tweakables
	[SerializeField, Range(2, 4096)] int resolution = 256;
	[SerializeField] Gradient coloring;
	[SerializeField] bool loadOnStart = true;
	[HideInInspector] public GameObject display;
	#endregion

	#region Properties
	public bool Live { get; set; }
	public float Progress { get; private set; }
	public Filter CurrentFilter { get; private set; }
	public int Resolution { get { return resolution; }}
	public Texture2D Tex {get; private set; }
	public float[,] Samples { get; private set; }
	public bool Working { get; private set; }
	#endregion

	#region Events
	public event Action OnGenerationComplete;
	public event Action OnTransformChanged;
	#endregion

	#region References
	IEnumerable<Filter> filters;
	float[,] DebugSamples;
	SpriteRenderer render;
	Vector3 p00, p01, p10, p11;
	Thread worker;
	float step;
	#endregion

	void OnEnable() {
		render = GetComponent<SpriteRenderer> ();
		Clear ();
		if (Application.isPlaying && loadOnStart) Generate();
	}

	void OnDisable() {
		Stop();
	}

	public void Clear(Texture2D newtexture = null) {
		if (newtexture == null) Tex = new Texture2D (resolution, resolution, TextureFormat.RGBA32, mipmap: false);
		else Tex = newtexture;
		Tex.name = "Procedual Texture";
		Tex.wrapMode = TextureWrapMode.Clamp;
		Tex.filterMode = FilterMode.Trilinear;
		Tex.anisoLevel = 9;
		Tex.SetPixels(Enumerable.Repeat(Color.clear, resolution * resolution).ToArray());
		Tex.Apply();
		CalculateBorders ();
		render.sprite = Sprite.Create(
			Tex,	new Rect(0, 0, Tex.width, Tex.height),
			pivot:         new Vector2(0.5f,0.5f),
			pixelsPerUnit: resolution
		);
		render.sprite.name = "Procedual";
	}

	void CalculateBorders() {
		p00 = transform.TransformPoint (new Vector3 (-.5f, -.5f));
		p01 = transform.TransformPoint (new Vector3 (-.5f, +.5f));
		p10 = transform.TransformPoint (new Vector3 (+.5f, -.5f));
		p11 = transform.TransformPoint (new Vector3 (+.5f, +.5f));
	}

	void Update() {
		if (!transform.hasChanged) return;

		transform.hasChanged = false;
		CalculateBorders ();
		if (OnTransformChanged != null) OnTransformChanged();
	}

	public Vector3 ImageToWorld (int x, int y)
	{
		var step = 1f / resolution;
		var a = Vector3.Lerp (p00, p01, (y + .5f) * step);
		var b = Vector3.Lerp (p10, p11, (y + .5f) * step);
		var point = Vector3.Lerp (a, b, (x + .5f) * step);
		return point;
	}


	void ShowSamples ()
	{
		for (int y = 0; y < resolution; y++) {
			for (int x = 0; x < resolution; x++) {
				Tex.SetPixel (x, y, coloring.Evaluate (DebugSamples [x, y]));
			}
		}
		Tex.Apply();
	}


	void Work(IEnumerable<Filter> trunk, IEnumerable<IGrouping<string, Filter>> branches, string displayName = "") {
		Working = true;
		// Execute own pipeline
		foreach (var filter in trunk) {
			CurrentFilter = filter;
			for(int y = 0; y < resolution; y++) {
				for (int x = 0; x < resolution; x++) {
					filter.ProcessPixel(x,y);
					if (!Working) return;
				}
				Progress += step; 
			}
			Samples = filter.Finish();
		}
		DebugSamples = new float[resolution,resolution];
		if (string.IsNullOrEmpty(displayName)) Array.Copy(Samples, DebugSamples, resolution * resolution);

		// Execute the other branch pipelines
		var original = new float[resolution, resolution];
		Array.Copy(Samples, original, resolution * resolution);
		foreach (var pipeline in branches) {
			foreach (var filter in pipeline) {
				CurrentFilter = filter;
				for(int y = 0; y < resolution; y++) {
					for (int x = 0; x < resolution; x++) {
						filter.ProcessPixel(x,y);
						if (!Working) return;
					}
					Progress += step;
				}
				Samples = filter.Finish();
			}
			if (pipeline.Key == displayName) {
				Array.Copy(Samples, DebugSamples, resolution * resolution);
			}
			Array.Copy(original, Samples, resolution * resolution);
		}
		Working = false;	// all work done =)
	}

	public void Generate() {
		if (!Live && !Application.isPlaying) return;

		if (Tex.width != resolution) Tex.Resize (resolution, resolution);
		if (Tex.height != resolution) Tex.Resize (resolution, resolution);

		Samples = new float[resolution,resolution];
		filters = GetComponentsInChildren<Filter>().Where(Util.Enabled<Filter>());
		foreach (var filter in filters) filter.Initialize();

		Progress = 0;
		step = 1f / (resolution * filters.Count());


		var trunk    = filters.Where(Util.BelongsTo<Filter> (this.gameObject)).ToList();
		var branches = filters.Where(Util.Not(Util.BelongsTo<Filter> (this.gameObject)))
							  .GroupBy(f => f.name).ToList();

		Stop();
		var displayName = display == null ? "" : display.name;
		worker = new Thread(() => Work (trunk, branches, displayName));
		worker.Start();

		StopAllCoroutines();
		StartCoroutine(WaitUntilWorkerFinished());
	}

	public void Stop() {
		if (worker == null) return;
		Working = false;	// notify the thread to stop!
		worker.Join();		// wait for the thread to finish...
	}

	IEnumerator WaitUntilWorkerFinished() {
		yield return new WaitUntil(() => !Working);

		foreach(var filter in filters) filter.FinishMainThread();
		ShowSamples();
		if (OnGenerationComplete != null) OnGenerationComplete();
	}

}
