using System.Collections.Generic;

public class CircularBuffer<T> : IEnumerable<T>, System.Collections.IEnumerable {
	Queue<T> buffer;
	public readonly int Count;
	
	public CircularBuffer(int length) {
		buffer = new Queue<T>(length);
		this.Count = length;
	}

	public void Add(T o) {
		if (buffer.Count >= Count) buffer.Dequeue();
		buffer.Enqueue (o);
	}

	public T PopLatest () {
		if (buffer.Count == 0) return default(T);
		return buffer.Dequeue ();
	}

	public void Clear() {
		buffer.Clear();
	}
	
	public IEnumerator<T> GetEnumerator() {
		return (buffer as IEnumerable<T>).GetEnumerator();
    }
	
	System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
		return this.GetEnumerator();
	}
	
	
}