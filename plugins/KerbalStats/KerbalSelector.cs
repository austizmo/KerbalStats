using System;
using System.Collections.Generic;
using UnityEngine;

namespace KerbalStats
{
	class KerbalSelector : AbstractWindow 
	{
		private List<KSKerbal> kerbals;

		public KerbalSelector(List<KSKerbal> kerbals) : base(1185, "Kerbal Stats: Kerbal Selector") {
			this.kerbals = kerbals;
		}

		protected override void OnWindow(int id) {
			switch (HighLogic.LoadedScene) {
				case(GameScenes.EDITOR):
				case(GameScenes.SPH):
					break;
						
				case(GameScenes.FLIGHT):
					break;
						
				case(GameScenes.SPACECENTER):
					break;
						
				default:
					Debug.Log ("KStats: Got toolbar click in unsupported gamescene");
					break;
			}
			GUILayout.BeginHorizontal(this.windowStyle);
				GUILayout.Label("Kerbal Selector Window!");
				if (GUILayout.Button("Close", this.buttonStyle)) SetVisible(false);
			GUILayout.EndHorizontal();
			GUI.DragWindow();
		}

		protected override void OnOpen() {
			SetPosition(100,100);
		}
	}
}