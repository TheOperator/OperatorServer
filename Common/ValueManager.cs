using System;
using System.Text;
using System.Security.Cryptography;

namespace OperatorServer
{
	public class ValueManager
	{
		public static MD5 MD5Generator = MD5.Create();
		private ulong _numbercounter;
		public ulong NumberID {
			get {
				lock (this._lock_numbercounter) {
					return this._numbercounter++;
				}
			}
			set {}
		}
		
		public string StringKey {
			get {
				lock (this._lock_numbercounter) {
					ulong tmp = this._numbercounter++;
					return tmp.ToString ();
				}
			}
		}
		private object _lock_numbercounter;
		
		public ValueManager ()
		{
			this._lock_numbercounter = new object ();
			this._numbercounter = 10000;
		}
		
		public static string GetMD5 (string val)
		{
			String lll = "";
			byte[] tmp = ValueManager.MD5Generator.ComputeHash (Encoding.Default.GetBytes (val));
			for (int i=0; i<tmp.Length; i++) {
				lll += tmp [i].ToString ("x2");
			}
			return lll;
		}
		
		public static string GetMD5(StringBuilder sb)
		{
			String lll = "";
			byte[] tmp = ValueManager.MD5Generator.ComputeHash(Encoding.Default.GetBytes(sb.ToString()));
			for(int i=0; i<tmp.Length; i++)
			{
				lll += tmp[i].ToString("x2");
			}
			return lll;
		}
	}
}

