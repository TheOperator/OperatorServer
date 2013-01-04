using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Gtk;

namespace OperatorServer
{
	class AdminKonsole
	{
		private  Window _win;
		private Communicator _com = new Communicator (new TcpClient ("192.168.0.10",8889));
		private bool _loopstarted = false;
		private Thread _loopthread;
		
		public static void Main (string[] args)
		{
			AdminKonsole a = new AdminKonsole ();
			a._loopthread.Start ();
			a.Show ();
			Application.Run ();
		}
		
		public AdminKonsole ()
		{
			Application.Init ();
			this._win = new Window ("Admin-Konsole");
			this._win.DeleteEvent += this.OnDeleteEvent;
			this._loopthread = new Thread (this.MainLoop);
		}
		
		public void Show ()
		{
			this._win.Show ();
		}
		
		protected void OnDeleteEvent (object sender, DeleteEventArgs a)
		{
			Application.Quit ();
			//a.RetVal = true;
		}
		
		public void MainLoop ()
		{
			this._loopstarted = true;
			while (this._loopstarted) {
				Telegram t = this._com.LastTelegram;
				if(t != null)
				{
					
				}
			}
		}
	}
}
