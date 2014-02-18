using System;
using System.Collections.Generic;
using UnityEngine;

namespace KerbalStats
{
	class KerbalSelector : AbstractWindow 
	{
		private List<KSKerbal> kerbals;

		private Vector2 scrollPos = Vector2.zero;

		public override event StateChangeHandler Changed;

		public KerbalSelector(List<KSKerbal> kerbals) : base(1185, "Kerbal Stats: Kerbal Selector") {
			this.kerbals = kerbals;
		}

		protected override void OnWindow(int id) {
			GUILayout.BeginVertical(this.windowStyle);
				GUILayout.Label("Kerbal Selector Window!");
				this.scrollPos = GUILayout.BeginScrollView(this.scrollPos, this.scrollStyle);
				foreach(KSKerbal kerbal in kerbals) {
					GUILayout.BeginHorizontal();
						GUILayout.Label(kerbal.name);
						if(GUILayout.Button("View Stats")) {
							Debug.Log("Trying to fire event");
							StateChangeEventArgs args = new StateChangeEventArgs();
							args.newState = DisplayState.KERBAL_STATS;
							args.kerbalName = kerbal.name;
							Changed(this, args);
						}
					GUILayout.EndHorizontal();
				}
				GUILayout.EndScrollView();
				if (GUILayout.Button("Close", this.buttonStyle)) SetVisible(false);
			GUILayout.EndVertical();
			GUI.DragWindow();
		}

		protected override void OnOpen() {
			SetPosition(100,100);
		}

		protected override void OnClose() {

		}
	}
}