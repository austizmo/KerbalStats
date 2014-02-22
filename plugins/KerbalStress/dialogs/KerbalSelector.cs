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

		private GUIStyle ListEntryArea;
		private GUIStyle StatsDisplay;
		private GUIStyle MoreInfoButton;

		//textures
		private Texture2D more_info;

		private String selectedKerbal = "";

		public KerbalSelector(List<KSKerbal> kerbals) : base(1185, "Kerbal Stress: Kerbal Selector") {
			this.kerbals = kerbals;

			this.ListEntryArea = new GUIStyle(this.buttonStyle);
			this.ListEntryArea.fixedHeight = 65;
			this.ListEntryArea.onHover = this.ListEntryArea.onNormal;

			this.StatsDisplay = new GUIStyle(HighLogic.Skin.box);
			//this.StatsDisplay.fixedHeight = 35;
			this.StatsDisplay.fixedWidth = 210;

			this.MoreInfoButton = new GUIStyle(this.buttonStyle);
			this.MoreInfoButton.fixedHeight = 65;
			this.MoreInfoButton.fixedWidth = 24;
		}

		protected override void OnWindow(int id) {
			this.scrollPos = GUILayout.BeginScrollView(this.scrollPos, this.scrollStyle, GUILayout.Height(444));
			this.more_info = AbstractWindow.GetTexture("KerbalStress/Resource/more_info");
			foreach(KSKerbal kerbal in kerbals) {
				GUILayout.BeginHorizontal();
				GUILayout.BeginHorizontal(this.ListEntryArea);
					GUILayout.Label(kerbal.name, this.labelStyle);
					GUILayout.BeginHorizontal(this.StatsDisplay);
						GUILayout.BeginVertical();
							GUILayout.Label("Current Stress:");
							GUILayout.Label("Total Stress:");
						GUILayout.EndVertical();
						GUILayout.BeginVertical();
							GUILayout.Label(kerbal.currentStress.ToString());
							GUILayout.Label(kerbal.cumulativeStress.ToString());
						GUILayout.EndVertical();
					GUILayout.EndHorizontal();
				GUILayout.EndHorizontal();
				if(GUILayout.Button(new GUIContent(more_info, "More Info"), this.MoreInfoButton)) {
					this.selectedKerbal = kerbal.name;
				}
				GUILayout.EndHorizontal();
				if(kerbal.name == this.selectedKerbal) {
					GUILayout.BeginHorizontal(this.ListEntryArea);
						GUILayout.Label("More Info About Stressors");
					GUILayout.EndHorizontal();
				}
			}
			GUILayout.EndScrollView();
			if (GUILayout.Button("Close", this.buttonStyle)) OnStateChange(DisplayState.HIDDEN, "");
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
		}
	}
}