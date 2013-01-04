using System;
using System.Net;
namespace OperatorServer
{
	public class Session
	{
		private string _key;
		private IPAddress _ip;
		private Communicator _com;
		
		public Session (Communicator c, string key)
		{
			this._key = key;
			this._com = c;
			this._ip = this._com.IP;
		}
		
		public void SendTelegram (Telegram t)
		{
			this._com.SendTelegram (t);
		}
	}
}

