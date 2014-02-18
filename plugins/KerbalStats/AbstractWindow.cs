using System;
using UnityEngine;
using KSP.IO;

namespace KerbalStats
{
	public abstract class AbstractWindow
	{
		private readonly int 	id;
		private readonly string title;

		private 	bool 		visible    	= false;
		protected 	Rect 		bounds   	= new Rect(0,0,600,800);
		protected 	GUIStyle 	windowStyle = new GUIStyle(HighLogic.Skin.window);
		protected	GUIStyle	buttonStyle	= new GUIStyle(HighLogic.Skin.button);
		protected 	GUIStyle 	scrollStyle = new GUIStyle(HighLogic.Skin.scrollView);

		public abstract event StateChangeHandler Changed;
		
		public AbstractWindow(int id, string title) {
			this.id     = id;
			this.title  = title;

			try {
			RenderingManager.AddToPostDrawQueue(0, OnDraw);
			} 
			catch {}
		}

		protected virtual void OnOpen() {}

		protected virtual void OnClose() {}

		protected virtual void OnStateChange(Event e) {}

		private void OnDraw() {
			if (this.visible) {
				bounds = GUILayout.Window(id, bounds, OnWindowInternal, title, this.windowStyle);
			}
		}

		private void OnWindowInternal(int id) {
			OnWindow(id);
		}

		protected abstract void OnWindow(int id);

		public void SetVisible(bool visible) {
			if (!this.visible && visible) OnOpen();
			if (this.visible && !visible) OnClose();
			this.visible = visible;
		}

		public void SetPosition(int x, int y) {
			bounds.Set(x, y, bounds.width, bounds.height);
		}

		public bool IsVisible() {
			return this.visible;
		}
	}
}

