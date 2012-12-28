using System;
using System.Collections;
using System.Threading;

namespace OperatorServer
{
	public class ThreadManager
	{
		private Hashtable _threads;
		private int _maxthreads;
		private Queue _threadqueue; 
		
		public ThreadManager (int mx)
		{
			this._threadqueue = new Queue();
			this._maxthreads = mx;
			this._threads = new Hashtable();
		}
		
		public ulong GetNewThread(ThreadStart method, ulong id)
		{
			//ulong tid = this.dm.curentThreadKey;
			this._threads.Add(id, new Thread(method));
			return id;
		}
		
		public void StartThread(ulong id)
		{
			if(this._threads.Count < this._maxthreads)
			{
				((Thread)this._threads[id]).Start();
			} else {
				this._threadqueue.Enqueue(id);
			}
		}
		
		public void StopThread(ulong id)
		{
			Thread t = (Thread)this._threads[id];
			t.Abort();
			if(this._threadqueue.Count > 0)
			{
				ulong key2 = (ulong)this._threadqueue.Dequeue();
				((Thread)this._threads[key2]).Start();
			}
		}
	}
}
