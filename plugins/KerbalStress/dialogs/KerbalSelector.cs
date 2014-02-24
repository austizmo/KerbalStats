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
		private GUIStyle NameStyle;
		private GUIStyle StressStyle;

		//textures
		private Texture2D more_info;
		private Texture2D stress_meter;
		private Texture2D stress_indicator;
		private Texture2D total_stress_bar;

		private String selectedKerbal = "";

		public KerbalSelector(List<KSKerbal> kerbals) : base(1185, "Kerbal Stress: Kerbal Selector") {
			this.kerbals = kerbals;

			this.ListEntryArea = new GUIStyle(this.buttonStyle);
			this.ListEntryArea.fixedHeight = 65;
			this.ListEntryArea.hover = this.ListEntryArea.normal;

			this.MoreInfoButton = new GUIStyle(this.buttonStyle);
			this.MoreInfoButton.fixedHeight = 65;
			this.MoreInfoButton.fixedWidth = 24;

			this.NameStyle = new GUIStyle(HighLogic.Skin.box);

			this.StressStyle = new GUIStyle(HighLogic.Skin.box);
			this.StressStyle.imagePosition = ImagePosition.ImageOnly;

			this.more_info 			= AbstractWindow.GetTexture("KerbalStress/Resource/more_info");
			this.stress_meter 		= AbstractWindow.GetTexture("KerbalStress/Resource/stress_meter");
			this.total_stress_bar 	= AbstractWindow.GetTexture("KerbalStress/Resource/total_stress_bar");
			this.stress_indicator	= AbstractWindow.GetTexture("KerbalStress/Resource/stress_indicator");
		}

		protected override void OnWindow(int id) {
			this.scrollPos = GUILayout.BeginScrollView(this.scrollPos, this.scrollStyle, GUILayout.Height(315), GUILayout.Width(250));
			foreach(KSKerbal kerbal in kerbals) {
				GUILayout.BeginHorizontal();
					GUILayout.BeginVertical(this.ListEntryArea);
						GUILayout.Button(kerbal.firstName, this.NameStyle);
						BuildStressMeter(kerbal);
					GUILayout.EndVertical();
					if(GUILayout.Button(new GUIContent(more_info, "More Info"), this.MoreInfoButton)) {
						this.selectedKerbal = kerbal.name;
					}
				GUILayout.EndHorizontal();
				if(kerbal.name == this.selectedKerbal) {
					BuildStressorList(kerbal);
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

		private void BuildStressMeter(KSKerbal kerbal) {
			GUILayout.Label(this.stress_meter, this.StressStyle, GUILayout.Height(24), GUILayout.Width(86));
			Rect meterRect = GUILayoutUtility.GetLastRect();

			float indicatorPosition = meterRect.x + 1 + (meterRect.width*(float)kerbal.currentStress)/2;
			Rect indicatorRect = new Rect(indicatorPosition, meterRect.y, meterRect.width, meterRect.height);
			GUI.Label(indicatorRect, this.stress_indicator);
		}

		private void BuildStressBar(KSKerbal kerbal) {

		}

		private void BuildStressorList(KSKerbal kerbal) {
			GUILayout.BeginVertical();
			if(kerbal.onDuty) {
				GUILayout.BeginHorizontal();
					GUILayout.Label("Social:");
					GUILayout.Label(kerbal.socialMod.ToString());
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
					GUILayout.Label("Flight Path:");
					GUILayout.Label(kerbal.flightPathMod.ToString());
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
					GUILayout.Label("G-Level:");
					GUILayout.Label(kerbal.gLevelMod.ToString());
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
					GUILayout.Label("Vessel:");
					GUILayout.Label(kerbal.vesselMod.ToString());
				GUILayout.EndHorizontal();
			} else {
				GUILayout.BeginHorizontal();
					GUILayout.Label("Resting:");
					GUILayout.Label(KSKerbal.BASE_REST_STRESS.ToString());
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
		}
	}
}