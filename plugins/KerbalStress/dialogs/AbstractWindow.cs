using System;
using UnityEngine;
using KSP.IO;

namespace KerbalStress
{
	public abstract class AbstractWindow
	{
		private readonly int 	id;
		private readonly string title;

		private static readonly int AUTO_HEIGHT = -1;
		private static readonly int DEFAULT_WIDTH = 400;

		private 	bool 		visible    	= false;
		protected 	Rect 		bounds   	= new Rect();
		protected 	GUIStyle 	windowStyle = new GUIStyle(HighLogic.Skin.window);
		protected	GUIStyle	buttonStyle	= new GUIStyle(HighLogic.Skin.button);
		protected 	GUIStyle 	scrollStyle = new GUIStyle(HighLogic.Skin.scrollView);
		protected	GUIStyle	labelStyle	= new GUIStyle(HighLogic.Skin.label);

		public abstract event StateChangeHandler Changed;
		
		public AbstractWindow(int id, string title) {
			this.id     = id;
			this.title  = title;

			try {
				RenderingManager.AddToPostDrawQueue(0, OnDraw);
			} 
			catch {}
		}

		protected virtual int GetWidth() {
			return DEFAULT_WIDTH;
		}

		protected virtual int GetHeight() {
			return AUTO_HEIGHT;
		}

		protected virtual void OnOpen() {}

		protected virtual void OnClose() {}

		protected abstract void OnStateChange (DisplayState state, String name);

		private void OnDraw() {
			if (this.visible) {
				if(GetHeight()==AUTO_HEIGHT) {
					bounds = GUILayout.Window(id, bounds, OnWindowInternal, title, this.windowStyle);
				} else {
					bounds = GUILayout.Window(id, bounds, OnWindowInternal, title, this.windowStyle, GUILayout.Width(GetWidth()), GUILayout.Height(GetHeight()));
				}
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

		public static Texture2D GetTexture(String path) {
			Texture2D texture = GameDatabase.Instance.GetTexture(path, false);
			return texture;
		}

		protected void OnDestroy() {
			RenderingManager.RemoveFromPostDrawQueue(0, OnDraw);
		}
	}
}

