using System;
namespace OperatorServer
{
	public class ValueManager
	{
		private ulong _numbercounter;
		public ulong NumberID {
			get {
				lock (this._lock_numbercounter) {
					return this._numbercounter++;
				}
			}
			set {}
		}
		
		private object _lock_numbercounter;
		
		public ValueManager ()
		{
			this._numbercounter = 10000;
		}
	}
}

