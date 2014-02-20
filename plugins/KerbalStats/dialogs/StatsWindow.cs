using System;
using System.Collections.Generic;
using UnityEngine;

namespace KerbalStats
{
	class StatsWindow : AbstractWindow 
	{
		private KSKerbal kerbal;

		public override event StateChangeHandler Changed;

		public StatsWindow(KSKerbal kerbal) : base(1186, kerbal.name) {
			this.kerbal = kerbal;
		}

		protected override void OnWindow(int id) {
			GUILayout.BeginVertical(this.windowStyle);
				GUILayout.Label("Stats");
				GUILayout.Label(this.kerbal.PrintStats());
				if (GUILayout.Button("Close", this.buttonStyle)) OnStateChange(DisplayState.SELECTOR_ALL, "");
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
			this.kerbal = null;
		}
	}
}