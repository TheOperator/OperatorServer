using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

namespace monoServer
{
	public class Telegram : CollectionBase
	{
		public const string REQUEST = "req";
		public const string RESPONSE = "res";
		public const string ACKNOWLEDGE = "ack";
		public const string CONNECTOR = "con";
		
		private String _key; // Wird evtl. in Zukunft noch benötigt
		public string key
		{
			get {
				return this._key;
			}
		}

		private String _type;
		public string type
		{
			get {
				return this._type;
			}
		}
		
		private String _keyword;
		public String keyWord
		{
			get {
				return this._keyword;
			}
		}
		
		private string _requestKey;
		public string requestKey
		{
			get {
				return this._requestKey;
			}
			set {
				this._requestKey = value;
			}
		}
		
		private string _acknowledgeKey;
		public string acknowledgeKey
		{
			get {
				return this._acknowledgeKey;
			}
			set {
				this._acknowledgeKey = value;
			}
		}
		
		private string _connectorkey;
		public string connectorKey
		{
			get {
				return this._connectorkey;
			}
			set {
				this._connectorkey = value;
			}
		}
		
		
		private string _connectorcommand;
		public string connectorCommand
		{
			get {
				return this._connectorcommand;
			}
			set {
				this._connectorcommand = value;
			}
		}
		
		//private ArrayList _parameters;
		private List<string> _parameters;
		
		public String this[int index]
		{
			get
			{
				return this._parameters[index];
			}
		}
		
		public Telegram()
		{
			this._parameters = new List<string>();
		}
		
		/*
		public Telegram (String keyword, String type, string key)
		{
			// Standard-Konstruktor
			this._key = key;
			this._keyword = keyword;
			this._type = type;
			this._parameters = new List<string>();
		}
		*/
		
		public Telegram (Telegram t, string key)
		{
			// Dieser Konstruktor erzeugt anhand von t ein Empfangs-Bestätigungs-Telegram
			this._key = key;
			//this._keyword = "ACKNOWLEDGE";
			this._type = Telegram.ACKNOWLEDGE;
			//this._parameters = new List<string>();
			//this._parameters.Add(t.key);
			this.acknowledgeKey = key;
		}
		
		public int countParameters()
		{
			return this._parameters.Count;
		}
		
		
		public void addParameter(String param)
		{
			this._parameters.Add(param);
		}
		
		public String getParamsAsString()
		{
			String tmp = "";
			for(int i=0;i<=this._parameters.Count-1;i++)
			{
				tmp += this._parameters[i] + ";";
			}
			return tmp;
		}
		
		/*
		public String toString()
		{
			long time = System.DateTime.Now.Ticks;
			String t = this._type + ";" + this._key + ";" + time.ToString() + ";" + this._keyword + ";";
			t += this.getParamsAsString();
			t += DataManager.getMD5(t);
			
			int num_chars = t.Length;
			t = num_chars.ToString() + ";" + t;
			
			return t;
		}
		*/
		
		public string serialize()
		{
			StringBuilder sb = new StringBuilder();
			long time = DateTime.Now.Ticks;
			sb.Append(this._type);
			sb.Append(";");
			sb.Append(this._key);
			sb.Append(";");
			sb.Append(time.ToString());
			sb.Append(";");
			
			switch(this._type)
			{
			case Telegram.REQUEST:
				sb.Append(this._keyword);
				sb.Append(";");
				sb.Append(this.getParamsAsString());
				break;
			case Telegram.RESPONSE:
				sb.Append(this._requestKey);
				sb.Append(";");
				sb.Append(this.getParamsAsString());
				break;
			case Telegram.ACKNOWLEDGE:
				sb.Append(this._acknowledgeKey);
				sb.Append(";");
				break;
			case Telegram.CONNECTOR:
				sb.Append(this._connectorkey);
				sb.Append(";");
				sb.Append(this._connectorcommand);
				sb.Append(";");
				sb.Append(this.getParamsAsString());
				break;
			}
			
			sb.Append(DataManager.getMD5(sb));
			sb.Insert(0, ";");
			sb.Insert(0, sb.Length-1);
			
			return sb.ToString();
		}
		
		public static Telegram getConnector(string key, ulong conid, string cmd, params string[] parameters)
		{
			Telegram t = new Telegram();
			t._type = Telegram.CONNECTOR;
			t._keyword = "CONNECTOR";
			t._key = key;
			t._requestKey = null;
			t._acknowledgeKey = null;
			t._connectorkey = conid.ToString();
			t._connectorcommand = cmd;
			
			for (int i=0; i<=parameters.Length-1; i++)
			{
				t.addParameter(parameters[i]);
			}
			
			return t;
		}
		
		public static Telegram getConnector(string key, ulong conid, string cmd)
		{
			Telegram t = new Telegram();
			t._type = Telegram.CONNECTOR;
			t._keyword = "CONNECTOR";
			t._key = key;
			t._requestKey = null;
			t._acknowledgeKey = null;
			t._connectorkey = conid.ToString();
			t._connectorcommand = cmd;
			
			return t;
		}
		
		public static Telegram getRequest(string key, string keyword)
		{
			Telegram t = new Telegram();
			t._type = Telegram.REQUEST;
			t._key = key;
			t._keyword = keyword;
			t._requestKey = null;
			t._acknowledgeKey = null;
			t._connectorkey = null;
			t._connectorcommand = null;
			
			return t;
		}
		
		public static Telegram getRequest(string key, string keyword, params string[] parameters)
		{
			Telegram t = new Telegram();
			t._type = Telegram.REQUEST;
			t._key = key;
			t._keyword = keyword;
			t._requestKey = null;
			t._acknowledgeKey = null;
			t._connectorkey = null;
			t._connectorcommand = null;
			
			// TODO geht das auch iwie effizienter?
			for(int i=0; i<=parameters.Length-1; i++)
			{
				//t._parameters.Add(parameters[i]);
				t.addParameter(parameters[i]);
			}
			return t;
		}
		
		public static Telegram getResponse(string key, string req_id)
		{
			Telegram t = new Telegram();
			t._type = Telegram.RESPONSE;
			t._key = key;
			t._requestKey = req_id;
			t._acknowledgeKey = null;
			t._connectorkey = null;
			t._connectorcommand = null;
			
			return t;
		}
		
		public static Telegram getResponse(string key, string req_id, params string[] parameters)
		{
			Telegram t = new Telegram();
			t._type = Telegram.RESPONSE;
			t._key = key;
			t._requestKey = req_id;
			t._acknowledgeKey = null;
			t._connectorkey = null;
			t._connectorcommand = null;
			
			for(int i=0; i<=parameters.Length-1; i++)
			{
				t.addParameter(parameters[i]);
			}
			
			return t;
		}
		
		public static Telegram getAcknowledge(string key, string tid)
		{
			Telegram t = new Telegram();
			t._type = Telegram.ACKNOWLEDGE;
			t._key = key;
			t._acknowledgeKey = tid;
			t._connectorcommand = null;
			t._connectorkey = null;
			t._requestKey = null;
			
			return t;
		}
		
		public static Telegram Parse(string t)
		{
			/*******************************************************************************************
			 * 
			 * Aufbau eines Telegamms:
			 * num_chars;type;key;timestamp;keyword;param1;param2....;checksum
			 * |         |     |   |         |       |                 |
			 * |         |     |   |         |       |                 -- ein MD5-Hash des Telegrams (ohne das erste Segment, also num_chars+; der letzte ; der Parameter wird
			 * |         |     |   |         |       |                    mit verwendet
			 * |         |     |   |         |       |
			 * |         |     |   |         |       -- Mit ; Separierte Liste der Parameter. Erlaubte Anzahl: 0 - n
			 * |         |     |   |         |
			 * |         |     |   |         -- Schlüsselwort um welches Telegram es sich handelt. Z.B.: LOGIN oder LOGOUT oder PING
			 * |         |     |   |
			 * |         |     |   -- Zeitstempel wann die toString()-Methode aufgerufen wurde. Verwendet wird DateTime.Ticks; Anzahl der Ticks seit 01.01.0001
			 * |         |     |
			 * |         |     |-- Ein Systemweit eindeutiger Schlüssel mit dem das Telegram Indentifiziert werden kann. Wird erzeugt duch DataManager.currentTelegramKey
			 * |         |
			 * |         |-- Gibt den type: 0 (Request), 1 (Response) oder 2 (Acknowledge)
			 * |
			 * -- Anzahl der Bytes des Telegrams ohne die Anzahl selbst
			 * 
			 *********************************************************************************************/
			
			// TODO checksum muss überprüft werden !!!
			Telegram tt = null;
			string[] arr = t.Split(';');
			string key = arr[2];
			int params_start = 0;
		
			switch(arr[1])
			{
			case Telegram.REQUEST:
				tt = Telegram.getRequest(key, arr[4]);
				params_start = 5;
				break;
			case Telegram.RESPONSE:
				tt = Telegram.getResponse(key, arr[4]);
				params_start = 5;
				break;
			case Telegram.ACKNOWLEDGE:
				tt = Telegram.getAcknowledge(key, arr[4]);
				params_start = 0;
				break;
			case Telegram.CONNECTOR:
				tt = Telegram.getConnector(key, ulong.Parse(arr[4]),arr[5]);
				params_start = 6;
				break;
			}
			
			if(params_start > 0)
			{
				for(int i=params_start; i<=arr.Length-2; i++)
				{
					tt.addParameter(arr[i]);
				}
			}
			
			return tt;
			/*
			Telegram tt = new Telegram((String)arr[4], (string)arr[1], (string)arr[2]);
			for(int i=5;i<=arr.Length-2;i++)
			{
				tt.addParameter(arr[i]);
			}
			
			return tt;
			*/
			
		}
	}
}
