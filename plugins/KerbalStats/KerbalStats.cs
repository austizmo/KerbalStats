using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using UnityEngine;
using Toolbar;

namespace KerbalStats 
{
	public class StateChangeEventArgs : EventArgs {
		public DisplayState newState;
		public String kerbalName;
	}
	public enum DisplayState { HIDDEN, SELECTOR_ALL, SELECTOR_VESSEL, KERBAL_STATS }
	public delegate void StateChangeHandler(object sender, StateChangeEventArgs e);

	class KerbalStats 
	{	
		private IButton button;
		private String 	button_on;
		private	String 	button_off;

		private AbstractWindow window;

		private DisplayState state = DisplayState.HIDDEN;

		private KerbalObserver 	observer;
		private StatsModel 		model;	

		public KerbalStats() {
			this.model 		= new StatsModel();
			this.observer 	= new KerbalObserver(this.model);

			if(HighLogic.CurrentGame != null) {
				CreateWindow();
				AddToolbarButton();
			}
		}

		private DisplayState State {
			get {
				if(this.state == DisplayState.HIDDEN) {
					if(HighLogic.LoadedScene == GameScenes.FLIGHT) {
						if(FlightGlobals.ActiveVessel.isEVA) {
							this.state = DisplayState.KERBAL_STATS;
						} else {
							this.state = DisplayState.SELECTOR_VESSEL;
						}
					} else {
						this.state = DisplayState.SELECTOR_ALL;
					}
				}
				return this.state;
			}
		}

		private void CreateWindow() {
			Debug.Log("creating window");
			switch(this.State) {
				case DisplayState.SELECTOR_ALL:
					this.window = new KerbalSelector(model.GetKerbals());
					break;
				case DisplayState.SELECTOR_VESSEL:
					this.window = new KerbalSelector(model.GetKerbals(FlightGlobals.ActiveVessel));
					break;
				case DisplayState.KERBAL_STATS:
					List<ProtoCrewMember> crew = FlightGlobals.ActiveVessel.GetVesselCrew();
					if(crew.Count == 1) {
						this.window = new StatsWindow(model.GetKerbal(crew[0].name));
					}
					break;
			}
			this.window.Changed += new StateChangeHandler(OnStateChange);
		}

		private void OnStateChange(object sender, StateChangeEventArgs e) {
			Debug.Log("Got state change event. new state: " + e.newState.ToString());
			this.window.SetVisible(false);
			this.state = e.newState;
			switch(e.newState) {
				case DisplayState.SELECTOR_VESSEL:
					Debug.Log("state change to vessel");
					this.window = new KerbalSelector(model.GetKerbals(FlightGlobals.ActiveVessel));
					break;
				case DisplayState.KERBAL_STATS:
					Debug.Log("state change to stats");
					this.window = new StatsWindow(model.GetKerbal(e.kerbalName));
				break;
				case DisplayState.HIDDEN: //fallthrough to all selector
				case DisplayState.SELECTOR_ALL:
				default:
					Debug.Log("state change to selector");
					this.window = new KerbalSelector(model.GetKerbals());
					break;
			}
			if(e.newState != DisplayState.HIDDEN) this.window.SetVisible(true);
			this.window.Changed += new StateChangeHandler(OnStateChange);
		}

		private void AddToolbarButton() {
			Debug.Log("adding toolbar button");
			button = ToolbarManager.Instance.add("KerbalsStats", "button");
            if (button != null)
            {
            	button_on  = "KerbalStats/Resource/button_on";
				button_off = "KerbalStats/Resource/button_off";

				button.TexturePath 	= button_off;
				button.ToolTip 		= "View KerbalStats";
				button.OnClick += (e) => {	OnToolbarClick();	};

				button.Visibility = new GameScenesVisibility(GameScenes.EDITOR, GameScenes.SPH, GameScenes.FLIGHT, GameScenes.SPACECENTER);
            }
            else
            {
				Debug.Log("toolbar button was null");
            }
		}

		/**
		 * toggle current window visibility
		 */
		private void OnToolbarClick() {
			if(this.window.IsVisible()) this.button.TexturePath = button_off;
			if(!this.window.IsVisible()) this.button.TexturePath = button_on;
			this.window.SetVisible(!this.window.IsVisible());
		}

		public void OnDestroy() {
			Debug.Log("KerbalStats.cs OnDestroy()");
			this.observer.OnDestroy();
			this.model.OnDestroy();
			this.observer = null;
			this.model = null;
		}
	}
}