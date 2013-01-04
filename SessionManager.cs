using System;
using System.Collections.Generic;

namespace OperatorServer
{
	public class SessionManager
	{
		private List<Session> _sessions;
		
		public SessionManager ()
		{
			this._sessions = new List<Session> ();
		}
		
		public void AddSession (Session s)
		{
			this._sessions.Add (s);
		}
		
		public void Broadcast (Telegram t)
		{
			foreach (Session s in this._sessions) {
				s.SendTelegram (t);
			}
		}
		
		public void EndAllSessions ()
		{
			foreach (Session s in this._sessions) {
				s.EndSession();
			}
		}
	
	}
}

