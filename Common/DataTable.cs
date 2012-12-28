using System;
using System.Collections;
using System.Collections.Generic;

namespace OperatorServer
{
	public class DataTable : CollectionBase
	{
		private string _name;
		public String Name
		{
			get {
				return this._name;
			}
			set {
				this._name = value;
			}
		}
		
		public ArrayList this[int index]
		{
			get
			{
				return (ArrayList)this.rows[index];
			}
			set
			{
				List[index] = value;
			}
		}
		
		private ArrayList columns;
		private ArrayList rows;
		private bool strictMode;
		/*
		private TableConnector tc;
		public TableConnector connector
		{
			get {
				return this.tc;
			}
			set {
				this.tc = value;
				this.tc.tableID = this._name;
				this.tc.columns = this.columns;
			}
		}
		*/
		//private List<TableConnector> connectors;
		
		private object lock_rows;
		
		public DataTable (string name, bool strict)
		{
			// TODO der strict=false mode muss ausführlich getestet werden
			this._name = name;
			this.columns = new ArrayList();
			this.rows = new ArrayList();
			this.strictMode = strict;
			this.lock_rows = new object();
			//this.tc = null;
			//this.connectors = new List<TableConnector>();
		}
		/*
		public void addConnector(TableConnector tc)
		{
			tc.tableID = this._name;
			tc.columns = this.columns;
			this.connectors.Add(tc);
		}
		*/
		public void disableStrictMode()
		{
			this.strictMode = false;
		}
		
		public int countRows()
		{
			return this.rows.Count;
		}
		
		public void addColumn(string name, Type t)
		{
			Hashtable h = new Hashtable();
			h.Add("identifier",name);
			h.Add("type",t);
			this.columns.Add(h);
		}
		
		public ArrayList selectRow(string col, string val)
		{
			int num_col = this.searchColumn(col);
			for(int i=0; i<=this.rows.Count-1; i++)
			{
				if(((string)((ArrayList)this.rows[i])[num_col]).Equals(val))
				{
					return (ArrayList)this.rows[i];
				}
			}
			return new ArrayList();
		}
		
		public void addRow(params object[] list)
		{
			if(this.strictMode && this.columns.Count == 0)
				throw new Exception("Strict-Mode: Define header first!");
						
			// Anzahl werte müssen der anzahl spalten übereinstimmen
			if(list.Length == this.columns.Count)
			{
				ArrayList al = new ArrayList();
				for(int i=0;i<=list.Length-1;i++)
				{
					if(this.strictMode)
					{
						Object o = list[i];
						Type t = o.GetType();
						if(t.Equals(((Type)((Hashtable)this.columns[i])["type"])))
						{
							al.Add(list[i]);
						} else
							throw new Exception("Strict-Mode: Invalid Type: " + i.ToString());
					} else {
						al.Add(list[i]);
					}
				}
				
				this.addRow(al);
				/*
				if(this.tc != null)
				{
					//this.tc.sendRow(al);
					for(int i=0; i<= this.connectors.Count-1; i++)
					{
						this.connectors[i].sendRow(al);
					}
				}
				*/
			} else
			{
				throw new Exception("Wrong Count params: list-length: "+list.Length+" columns-length: "+this.columns.Count);
			}
		}
		
		private int addRow(ArrayList al)
		{
			lock(this.lock_rows)
			{
				this.rows.Add(al);
				return this.rows.Count-1;
			}
		}
		
		public int addRow(string row)
		{
			ArrayList al = new ArrayList();
			string[] elems = row.Split(';');
			for(int i=0; i<=elems.Length-1; i++)
			{
				al.Add(elems[i]);
			}
			lock(this.lock_rows)
			{
				this.rows.Add(al);
				return this.rows.Count-1;
			}
		}
		
		public string toString()
		{
			string tmp = "";
			for(int i=0; i<= this.rows.Count-1; i++)
			{
				ArrayList al = (ArrayList)this.rows[i];
				for(int j=0; j<=al.Count-1; j++)
				{
					tmp += (string)al[j] + " ";
				}
				tmp += "\n";
			}
			return tmp;
		}
		
		// TODO Beim erzeugen der columns soll eine weitere tabelle angelegt werden: spaltenname -> index
		// dadurch wird searchColumn überflüssig
		private int searchColumn(string col)
		{
			int num_col = 0;
			for(int i=0; i<= this.columns.Count-1; i++)
			{
				if(((string)((Hashtable)this.columns[i])["identifier"]).Equals(col))
					num_col = i;
			}
			return num_col;
		}
		
		
		// TODO deleteRow müssen beide noch getestet werden
		public void deleteRow(string col, string val)
		{
			// TODO Beim definieren der spalten muss eine weitere liste angelegt werden ala key = spaltennummer damit searchColumn
			// rausfliegen kann
			
			int num_col = this.searchColumn(col);
			for(int i=0; i<=this.rows.Count-1; i++)
			{
				if(((string)((ArrayList)this.rows[i])[num_col]).Equals(val))
					this.deleteRow(i);
			}
		}
		
		public void modifyRow(string where_col, string where_val, string update_col, object new_val)
		{
			// TODO modifyRow muss den strictMode beachten
			// update table set update_col=new_val where where_col=where_val
			int num_where_col = this.searchColumn(where_col);
			int num_update_col = 0;
			for(int i=0; i<=this.rows.Count-1; i++)
			{
				ArrayList al = (ArrayList)this.rows[i];
				string t = (string)al[num_where_col];
				
				//if(((string)((ArrayList)this.rows[i])[num_col]).Equals(where_val))
				
				if(t.Equals(where_val))
				{
					num_update_col = this.searchColumn(update_col);
					this.modifyRow (i, num_update_col, new_val);
				}
			}
			
		}
		
		public void modifyRow(int i, int update_col, object new_val)
		{
			lock(this.lock_rows)
			{
				((ArrayList)this.rows[i])[update_col] = new_val;
			}
		}
		
		public void deleteRow(int i)
		{
			lock(this.lock_rows)
			{
				this.rows.RemoveAt(i);
			}
		}
		
		public void addColumns()
		{
		}
	}
}
