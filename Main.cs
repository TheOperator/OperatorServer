using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;

namespace OperatorServer
{
	class Server
	{
		public static void Main (string[] args)
		{
			try {
				Server s = new Server ();
				s.Start ();
			} catch (Exception ex) {
				Console.WriteLine (ex.Message);
			}
		}
		
		private bool _started;
		public bool Started {
			get {
				return this._started;
			}
		}
		
		private TcpListener _clientlistener;
		private TcpListener _adminlistener;
		private ThreadManager _tm;
		private ulong _mainthreadid;
		private ulong _adminthreadid;
		private ValueManager _vm;
		private SessionManager _sm;
		
		private Queue _connectionqueue;
		
		public Server ()
		{
			this._sm = new SessionManager ();
			this._connectionqueue = new Queue ();
			this._vm = new ValueManager (10000);
			this._mainthreadid = this._vm.NumberID;
			this._adminthreadid = this._vm.NumberID;
			this._started = false;
			this._clientlistener = new TcpListener (IPAddress.Parse ("192.168.0.10"), 8888);
			this._adminlistener = new TcpListener (IPAddress.Parse ("192.168.0.10"), 8889);
			this._tm = new ThreadManager (10);
			this._tm.GetNewThread (this.MainLoop, this._mainthreadid);
			this._tm.GetNewThread (this.AdminLoop, this._adminthreadid);
		}
		
		public void AdminLoop ()
		{
			this._adminlistener.Start ();
			while (true) {
				Communicator c = new Communicator (this._adminlistener.AcceptTcpClient ());
				this.AdminThread (c);
			}
		}
		
		public void AdminThread (Communicator c)
		{
			c.SetValueManager (this._vm);
			bool threadstarted = true;
			while (threadstarted) {
				Telegram t = c.LastTelegram;
				if (t != null) {
					//Console.WriteLine ("Admin: " + t.ToString ());
					switch (t.keyWord) {
					case "SHUTDOWN":
						this.ShutDown ();
					}
				}
			}
		}
		
		public void ShutDown(int timer)
		{
			
		}
		
		public void Start ()
		{
			this._started = true;
			this._tm.StartThread (this._mainthreadid);
			this._tm.StartThread (this._adminthreadid);
		}
		
		public void Stop ()
		{
			this._started = false;
		}
		
		public void MainLoop ()
		{
			this._clientlistener.Start ();
			while (this._started) {
				this._connectionqueue.Enqueue (new Communicator (this._clientlistener.AcceptTcpClient ()));
				ulong id = this._tm.GetNewThread (this.ClientThread, this._vm.NumberID);
				this._tm.StartThread (id);
			}
		}
		
		public void ClientThread ()
		{
			Communicator c = (Communicator)this._connectionqueue.Dequeue ();
			c.SetValueManager (this._vm);
			Session s = new Session (c, this._vm.StringKey);
			this._sm.AddSession (s);
			
			bool threadStarted = true;
			Telegram t = null;
			
			while (threadStarted) {
				t = c.LastTelegram;
				if (t != null) { // Telegram in queue
					Console.WriteLine (t.ToString ());
				}
			}
		}
	}
}
