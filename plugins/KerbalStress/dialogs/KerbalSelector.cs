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
		private GUIStyle NameStyle;
		private GUIStyle StressStyle;
		private GUIStyle IndicatorStyle;
		private GUIStyle TotalIndicatorStyle;

		//textures
		private Texture2D more_info;
		private Texture2D stress_meter;
		private Texture2D stress_indicator;		
		private Texture2D stress_indicator_90;
		private Texture2D total_stress_bar;

		private String selectedKerbal = "";

		public KerbalSelector(List<KSKerbal> kerbals) : base(1185, "Kerbal Stress") {
			this.kerbals = kerbals;

			this.ListEntryArea = new GUIStyle(this.buttonStyle);
			this.ListEntryArea.fixedHeight = 100;
			this.ListEntryArea.hover = this.ListEntryArea.normal;

			this.NameStyle = new GUIStyle(HighLogic.Skin.button);
			this.NameStyle.fixedWidth = 85;
			this.NameStyle.hover = this.NameStyle.normal;

			this.StressStyle = new GUIStyle(HighLogic.Skin.box);
			this.StressStyle.imagePosition = ImagePosition.ImageOnly;

			this.IndicatorStyle = new GUIStyle(this.StressStyle);
			this.TotalIndicatorStyle = new GUIStyle(this.StressStyle);

			this.more_info 			= AbstractWindow.GetTexture("KerbalStress/Resource/more_info");
			this.stress_meter 		= AbstractWindow.GetTexture("KerbalStress/Resource/stress_meter");
			this.total_stress_bar 	= AbstractWindow.GetTexture("KerbalStress/Resource/total_stress_bar");
			this.stress_indicator	= AbstractWindow.GetTexture("KerbalStress/Resource/stress_indicator");
			this.stress_indicator_90= AbstractWindow.GetTexture("KerbalStress/Resource/stress_indicator_horizontal");
		}

		protected override void OnWindow(int id) {
			this.scrollPos = GUILayout.BeginScrollView(this.scrollPos, this.scrollStyle, GUILayout.Height(315), GUILayout.Width(160));
			foreach(KSKerbal kerbal in kerbals) {
				GUILayout.BeginHorizontal(this.ListEntryArea);
					GUILayout.BeginVertical();
						if(GUILayout.Button(kerbal.firstName, this.NameStyle)) {
							this.selectedKerbal = kerbal.name;
						}
						BuildStressMeter(kerbal);
					GUILayout.EndVertical();
					BuildStressBar(kerbal);
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
			this.StressStyle.padding.left = 0;
			GUILayout.Label(this.stress_meter, this.StressStyle, GUILayout.Height(23), GUILayout.Width(85));
			Rect meterRect = GUILayoutUtility.GetLastRect();

			double maxMeterValue = KSKerbal.MAX_CURRENT_STRESS; 
			double percentStress = kerbal.currentStress/maxMeterValue;
			percentStress = (percentStress >=1) ? 1 : percentStress;
			percentStress = (percentStress <=-1) ? -1 : percentStress;

			Vector2 currentOffset = new Vector2();
			currentOffset.x = (int)(meterRect.width*percentStress)/2;
			currentOffset.y = 3;

			this.IndicatorStyle.contentOffset = currentOffset;
			GUI.Label(meterRect, this.stress_indicator, this.IndicatorStyle);
		}

		private void BuildStressBar(KSKerbal kerbal) {
			this.StressStyle.padding.left = 1;
			GUILayout.Label(this.total_stress_bar, this.StressStyle, GUILayout.Height(87), GUILayout.Width(24));
			Rect meterRect = GUILayoutUtility.GetLastRect();

			double percentStress = kerbal.cumulativeStress/kerbal.breakpoint;
			percentStress = (percentStress >=1) ? 1 : percentStress;

			Vector2 currentOffset = new Vector2();
			currentOffset.y = (int)-(meterRect.height*percentStress) - 2;
			currentOffset.x = 1;

			this.TotalIndicatorStyle.contentOffset = currentOffset;
			GUI.Label(meterRect, this.stress_indicator_90, this.TotalIndicatorStyle);
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
				if(GUILayout.Button("Panic!", this.buttonStyle)) {
					kerbal.OnFailStressTest();
				}
			} else {
				GUILayout.BeginHorizontal();
					GUILayout.Label("Resting:");
					GUILayout.Label(KSKerbal.BASE_REST_STRESS.ToString());
				GUILayout.EndHorizontal();
			}
			GUILayout.BeginHorizontal();
					GUILayout.Label("Total:");
					GUILayout.Label(kerbal.cumulativeStress.ToString());
				GUILayout.EndHorizontal();

			GUILayout.EndVertical();
		}
	}
}