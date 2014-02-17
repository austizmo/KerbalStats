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
		protected 	Rect 		bounds   	= new Rect();
		protected 	GUIStyle 	windowStyle = new GUIStyle(HighLogic.Skin.window);
		protected	GUIStyle	buttonStyle	= new GUIStyle(HighLogic.Skin.button);

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

