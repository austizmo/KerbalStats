using System;
using System.Collections.Generic;
using UnityEngine;

namespace KerbalStress
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
							OnStateChange(DisplayState.KERBAL_STATS, kerbal.name);
						}
					GUILayout.EndHorizontal();
				}
				GUILayout.EndScrollView();
				if (GUILayout.Button("Close", this.buttonStyle)) OnStateChange(DisplayState.HIDDEN, "");
			GUILayout.EndVertical();
			GUI.DragWindow();
		}

		protected override void OnStateChange(DisplayState state, String name) {
			StateChangeEventArgs args = new StateChangeEventArgs();
			args.newState 	= state;
			args.kerbalName = name;

			Changed(this, args);
		}

		protected override void OnOpen() {
			SetPosition(100,100);
		}

		protected override void OnClose() {
			this.kerbals = null;
		}
	}
}