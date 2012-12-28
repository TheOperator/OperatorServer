using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Collections;
using System.Net.Sockets;

namespace OperatorServer
{
	public class Communicator
	{
		private TcpClient _client;
		private NetworkStream _stream;
		private Thread _thread;
		private volatile bool _threadstarted;
		private Queue _telegramqueue;
		public Telegram LastTelegram {
			get {
				if (this._telegramqueue.Count > 0)
					return (Telegram)this._telegramqueue.Dequeue ();
				else
					return null;
			}
			set {}
		}
		private DataTable _telegramtable;
		private ValueManager _vm;
		
		public Communicator (TcpClient c)
		{
			this._client = c;
			this._stream = c.GetStream ();
			this._telegramtable = new DataTable ("communicator-telegrams", true);
			this._telegramtable.addColumn ("id", typeof(string));
			this._telegramtable.addColumn ("timestamp", typeof(DateTime));
			this._telegramtable.addColumn ("session-key", typeof(String));
			this._telegramtable.addColumn ("telegram", typeof(String));
			this._telegramqueue = new Queue ();
			this._thread = new Thread (this.Start);
			this._thread.Start ();
		}
		
		public void SetValueManager (ValueManager vm)
		{
			this._vm = vm;
		}
		
		public void CloseConnection ()
		{
			this._threadstarted = false;
			this._stream.Close ();
			this._client.Close ();
		}
		
		public void SendTelegram(Telegram t)
		{
			// TODO Gesendete Telegramme müssen in einer Tabelle zwischengespeichert werden
			// damit bei Bedarf das Telegram erneut gesendet wird. Erst wenn das Telegram
			// vom Server bestätigt worden ist wird das Telegramm aus der Tabelle entfernt.
			// Das Telegramm wird solange erneut gesendet bis die Bestätigung beim Client
			// verarbeitet wurde
			// Sollte ein Telegram just in dem Augenblick erneut zum Server gesendet werden
			// wenn die Bestätigung ankommt muss der Server entsprechend reagieren (Telegramm Ignorieren oder so ähnlich)
			string tmp = t.ToString();
			if(!t.keyWord.Equals("ACKNOWLEDGE"))
				this._telegramtable.addRow(t.key, DateTime.Now, "skey", tmp);
			
			this.SendPreparedTelegram(tmp, t.key);
		}
		
		public void SendPreparedTelegram(String t, string key)
		{
			//this.l.writeTelegramToFile(t, false);
			byte[] writeBuffer = Encoding.ASCII.GetBytes(t);
			this._stream.Write(writeBuffer, 0, writeBuffer.Length);
			this._stream.Flush();
		}
		
		public void Start ()
		{
			String commandLine = null;
			String numBytes = null;
			byte[] tmpbyte = new Byte[1];
			byte b = byte.Parse ("59");
			this._threadstarted = true;
			
			while (this._threadstarted) {
				if (this._stream.DataAvailable) {
					commandLine = null;
					
					tmpbyte = new Byte[1];
					while (!tmpbyte[0].Equals(b)) {
						// TODO Vorkehrungen treffen dass im Falle eines Falschen Telegrams (falsche oder fehlende byteanzahl keine endlos-schleife entsteht)
						this._stream.Read (tmpbyte, 0, tmpbyte.Length);
						numBytes += Encoding.ASCII.GetString (tmpbyte);
					}
					
					int i = Int16.Parse (numBytes.Substring (0, numBytes.Length - 1));
					
					for (int j=0; j<=i-1; j++) {
						this._stream.Read (tmpbyte, 0, tmpbyte.Length);
						commandLine += Encoding.ASCII.GetString (tmpbyte);
					}
					
					//this.l.writeTelegramToFile (numBytes + commandLine, true);
					Telegram t = Telegram.Parse (numBytes + commandLine);
					if (!t.keyWord.Equals ("ACKNOWLEDGE")) {
						// Bestätigung des Empfangs des Telegrams versenden (aber nur wenn das empfange Telegramm kein ACKNOWLEDGE ist
						Telegram tt = new Telegram (t, this._vm.StringKey);
						this.SendTelegram (tt);
						this._telegramqueue.Enqueue (t);
					} else {
						this._telegramtable.deleteRow ("id", t [0]);
					}
					numBytes = null;				
				}
			}
		}
	}
}

