using System;
using System.Net;
using System.Net.Sockets;
using Gtk;
using OperatorServer;

public partial class MainWindow: Gtk.Window
{	
	private Communicator _com;
	private ValueManager _vm;
	
	public MainWindow (): base (Gtk.WindowType.Toplevel)
	{
		Build ();
		this._com = new Communicator (new TcpClient ("192.168.0.10", 8888));
		this._vm = new ValueManager (555);
	}
	
	protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	{
		Application.Quit ();
		a.RetVal = true;
	}
	
	[GLib.ConnectBefore]
	protected void OnKeyPress (object sender, KeyPressEventArgs a)
	{
		if (a.Event.Key == Gdk.Key.Return) {
			// Send Entry Value to Server
			string val = this.entry2.Text;
			this.entry2.Text = string.Empty;
			this.textview2.Buffer.Text += val + "\n";
			this.ConvertToTelegram (val);
			//Console.WriteLine (val);
			
		}
	}
	
	private void ConvertToTelegram (string val)
	{
		string[] _params = val.Split (';');
		Telegram t = Telegram.getRequest (this._vm.StringKey, _params [0]);
		for (int i=1; i<=_params.Length-1; i++) {
			t.addParameter (_params [i]);
		}
		
		this._com.SendTelegram (t);
	}
}
