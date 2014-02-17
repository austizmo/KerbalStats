using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using UnityEngine;
using Toolbar;

namespace KerbalStats
{
	[KSPAddonFixed(KSPAddon.Startup.EveryScene, false, typeof(KerbalStats))]
	public class KerbalStats : MonoBehaviour
	{
		private IButton button;
		private String 	button_on  = "KerbalStats/Resource/button_on";
        private String 	button_off = "KerbalStats/Resource/button_off";

		private KerbalSelector 	selector;
		private static KerbalObserver observer = new KerbalObserver();

		public void Awake() {}

		public void Start() {
			Debug.Log("KerbalStats Start");
			AddToolbarButton();
		}

		public void Update() {}

		/**
		 * Adds the KS button to the toolbar
		 */
		private void AddToolbarButton() {
            button = ToolbarManager.Instance.add("KerbalsStats", "button");
            if (button != null)
            {
				button.TexturePath 	= this.button_off;
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
		 * Toggles the button icon texture
		 */
		private void SetToolbarTexture(bool visible) {
			if(visible) 	this.button.TexturePath = this.button_on;
			if(!visible)	this.button.TexturePath = this.button_off;
		}

		/**
		 * Spawn the Kerbal Selector, if needed, and toggle its visibility
		 */
		private void OnToolbarClick() {
			if(this.selector == null) {
				//load our kerbals
				List<KSKerbal> kerbals = SaveManager.LoadKerbals();
				if(kerbals == null) {
					Debug.Log("Creating kerbals");
					kerbals = CreateKerbals();
				}
				this.selector = new KerbalSelector(kerbals);
			}
			this.selector.SetVisible(!this.selector.IsVisible());
			SetToolbarTexture(this.selector.IsVisible());
		}

		/**
		 * Create a KSKerbal for all the kerbals in our current game, then save them
		 */
		private List<KSKerbal> CreateKerbals() {
			List<KSKerbal> kerbals = new List<KSKerbal>();
			Debug.Log("Looping through crew");
			foreach (ProtoCrewMember kerbal in HighLogic.CurrentGame.CrewRoster) {
				kerbals.Add(new KSKerbal(kerbal));
			}
			SaveManager.SaveKerbals(kerbals);
			return kerbals;
		}

		internal void OnDestroy() {
			Debug.Log("KerbalStats OnDestroy");
			button.Destroy();
		}
	}
}