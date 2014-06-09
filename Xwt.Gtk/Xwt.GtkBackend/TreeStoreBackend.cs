// 
// TreeStoreBackend.cs
//  
// Author:
//       Lluis Sanchez <lluis@xamarin.com>
// 
// Copyright (c) 2011 Xamarin Inc
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using Xwt.Backends;
using Xwt.Drawing;


namespace Xwt.GtkBackend
{
	class IterPos: TreePosition
	{
		public IterPos ()
		{
		}
		
		public IterPos (int treeVersion, Gtk.TreeIter iter)
		{
			this.Iter = iter;
			this.Version = treeVersion;
		}
		
		public Gtk.TreeIter Iter;
		public Gtk.TreeIter LastChildIter;
		public int LastChildIndex = -1;
		public int ChildrenCount = -1;
		public int Version;
	}
	
	public class TreeStoreBackend: TableStoreBackend, ITreeStoreBackend
	{
		Type[] columnTypes;
		int version;
		
		public Gtk.TreeStore Tree {
			get { return (Gtk.TreeStore) Store; }
		}
		
		public override Gtk.TreeModel InitializeModel (Type[] columnTypes)
		{
			this.columnTypes = columnTypes;
			return new Gtk.TreeStore (columnTypes);
		}
		
		public event EventHandler<TreeNodeEventArgs> NodeInserted;
		public event EventHandler<TreeNodeChildEventArgs> NodeDeleted;
		public event EventHandler<TreeNodeEventArgs> NodeChanged;
		public event EventHandler<TreeNodeOrderEventArgs> NodesReordered;
		
		IterPos GetIterPos (TreePosition pos)
		{
			IterPos tpos = (IterPos) pos;
			if (tpos != null && tpos.Version != version) {
				tpos.LastChildIndex = -1;
				tpos.ChildrenCount = -1;
			}
			return tpos;
		}
		
		public void Clear ()
		{
			version++;
			Tree.Clear ();
		}

		public TreePosition GetChild (TreePosition pos, int index)
		{
			IterPos tpos = GetIterPos (pos);
			if (tpos != null && tpos.LastChildIndex == index)
				return new IterPos (version, tpos.LastChildIter);
			if (index == 0) {
				if (tpos != null) {
					Gtk.TreeIter it;
					if (Tree.IterChildren (out it, tpos.Iter)) {
						tpos.LastChildIter = it;
						tpos.LastChildIndex = 0;
						return new IterPos (version, it);
					}
				} else {
					Gtk.TreeIter it;
					if (Tree.GetIterFirst (out it))
						return new IterPos (version, it);
				}
				return null;
			}
			
			if (tpos == null) {
				Gtk.TreeIter it;
				if (Tree.IterNthChild (out it, index))
					return new IterPos (version, it);
				else
					return null;
			}
			
			if (tpos.LastChildIndex == -1 || tpos.LastChildIndex > index) {
				Gtk.TreeIter it;
				if (Tree.IterNthChild (out it, tpos.Iter, index)) {
					tpos.LastChildIter = it;
					tpos.LastChildIndex = index;
					return new IterPos (version, it);
				} else
					return null;
			}
			
			// tpos.LastChildIndex < index
			
			Gtk.TreeIter iter = tpos.LastChildIter;
			for (int n = tpos.LastChildIndex; n < index; n++) {
				if (!Tree.IterNext (ref iter))
					return null;
			}
			tpos.LastChildIter = iter;
			tpos.LastChildIndex = index;
			return new IterPos (version, iter);
		}
		
		public int GetChildrenCount (TreePosition pos)
		{
			if (pos == null)
				return Tree.IterNChildren ();
			
			IterPos tpos = GetIterPos (pos);
			
			if (tpos.ChildrenCount != -1)
				return tpos.ChildrenCount;
			
			return tpos.ChildrenCount = Tree.IterNChildren (tpos.Iter);
		}

		public void SetValue (TreePosition pos, int column, object value)
		{
			IterPos tpos = GetIterPos (pos);
			SetValue (tpos.Iter, column, value);
		}

		public void SetValue (object pos, int column, object value)
		{
			SetValue ((TreePosition)pos, column, value);
		}

		public object GetValue (TreePosition pos, int column)
		{
			IterPos tpos = GetIterPos (pos);
			return GetValue (tpos.Iter, column);
		}

		public object GetValue (object pos, int column)
		{
			return GetValue ((TreePosition)pos, column);
		}

		public TreePosition InsertBefore (TreePosition pos)
		{
			version++;
			IterPos tpos = GetIterPos (pos);
			var p = Tree.InsertNodeBefore (tpos.Iter);
			return new IterPos (version, p);
		}

		public TreePosition InsertAfter (TreePosition pos)
		{
			version++;
			IterPos tpos = GetIterPos (pos);
			var p = Tree.InsertNodeAfter (tpos.Iter);
			return new IterPos (version, p);
		}

		public TreePosition AddChild (TreePosition pos)
		{
			version++;
			IterPos tpos = GetIterPos (pos);
			Gtk.TreeIter it;
			if (pos == null)
				it = Tree.AppendNode ();
			else
				it = Tree.AppendNode (tpos.Iter);
			return new IterPos (version, it);
		}
		
		public void Remove (TreePosition pos)
		{
			version++;
			IterPos tpos = GetIterPos (pos);
			Gtk.TreeIter it = tpos.Iter;
			Tree.Remove (ref it);
		}

		public TreePosition GetNext (TreePosition pos)
		{
			IterPos tpos = GetIterPos (pos);
			Gtk.TreeIter it = tpos.Iter;
			if (!Tree.IterNext (ref it))
				return null;
			return new IterPos (version, it);
		}

		public TreePosition GetPrevious (TreePosition pos)
		{
			throw new NotImplementedException ();
		}

		public TreePosition GetParent (TreePosition pos)
		{
			IterPos tpos = GetIterPos (pos);
			Gtk.TreeIter it;
			if (!Tree.IterParent (out it, tpos.Iter))
				return null;
			return new IterPos (version, it);
		}
		
		public Type[] ColumnTypes {
			get {
				return columnTypes;
			}
		}
		
		public void EnableEvent (object eventId)
		{
		}
		
		public void DisableEvent (object eventId)
		{
		}
	}
}

