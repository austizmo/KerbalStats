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
				if (GUILayout.Button("Close", this.buttonStyle)) SetVisible(false);
			GUILayout.EndVertical();
			GUI.DragWindow();
		}
	}
}