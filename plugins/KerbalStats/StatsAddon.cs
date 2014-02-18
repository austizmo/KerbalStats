using System;
using UnityEngine;

namespace KerbalStats
{
	[KSPAddonFixed(KSPAddon.Startup.EveryScene, false, typeof(StatsAddon))]
	public class StatsAddon : MonoBehaviour
	{
		private KerbalStats stats = null;

		public void Awake() {}

		public void Start() {
			Debug.Log("KerbalStats Start");
			if(this.stats == null) {
				Debug.Log("stats null, creating now");
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