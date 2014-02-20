using System;
using UnityEngine;

namespace KerbalStats
{
	[KSPAddonFixed(KSPAddon.Startup.SpaceCentre, false, typeof(StatsAddon))]
	public class StatsAddon : MonoBehaviour
	{
		private KerbalStats stats = null;

		public void Awake() {}

		public void Start() {
			Debug.Log("KerbalStats Start in scene: " + HighLogic.LoadedScene);
			if(this.stats == null) {
				this.stats = new KerbalStats();
			}
		}

		public void Update() {}

		internal void OnDestroy() {
			Debug.Log("KerbalStats OnDestroy");
			this.stats.OnDestroy();
		}
	}

	[KSPAddonFixed(KSPAddon.Startup.Flight, false, typeof(StatsAddonFlight))]
	public class StatsAddonFlight : MonoBehaviour
	{
		private KerbalStats stats = null;
		
		public void Awake() {}
		
		public void Start() {
			Debug.Log("KerbalStats Start in scene: " + HighLogic.LoadedScene);
			if(this.stats == null) {
				this.stats = new KerbalStats();
			}
		}
		
		public void Update() {}
		
		internal void OnDestroy() {
			Debug.Log("KerbalStats OnDestroy");
			this.stats.OnDestroy();
		}
	}

	[KSPAddonFixed(KSPAddon.Startup.EditorAny, false, typeof(StatsAddonEditor))]
	public class StatsAddonEditor : MonoBehaviour
	{
		private KerbalStats stats = null;
		
		public void Awake() {}
		
		public void Start() {
			Debug.Log("KerbalStats Start in scene: " + HighLogic.LoadedScene);
			if(this.stats == null) {
				this.stats = new KerbalStats();
			}
		}
		
		public void Update() {}
		
		internal void OnDestroy() {
			Debug.Log("KerbalStats OnDestroy");
			this.stats.OnDestroy();
		}
	}
}