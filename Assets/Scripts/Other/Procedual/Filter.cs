using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[ExecuteInEditMode]
public abstract class Filter : MonoBehaviour {

	public Generator Generator { get; private set; }

	public string Key { get { return "game.loading." + this.GetType().Name.ToLower();}}

	protected float[,] buffer;

	protected virtual void OnEnable () {}

	public virtual void Initialize() {	// runs on main thread
		Generator = GetComponentInParent <Generator>();
		if (Generator == null) Debug.LogErrorFormat("{0} [{1}] cannot find Generator script", name, GetType().Name);
		buffer = new float[Generator.Resolution,Generator.Resolution];

	}
	public virtual void ProcessPixel (int x, int y) {
		buffer[x,y] = Process(Generator.Samples[x,y], x, y);
	}
	protected virtual float Process (float value, int x, int y) { return value; }	// runs on worker thread
	public virtual float[,] Finish (){	return buffer;	}	// runs on worker thread

	public virtual void FinishMainThread (){}	// runs on Main Thread again
}
