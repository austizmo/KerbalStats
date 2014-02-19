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
	    //This will kick us into the save called default and set the first vessel active
    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class Debug_AutoLoadPersistentSaveOnStartup : MonoBehaviour
    {
        //use this variable for first run to avoid the issue with when this is true and multiple addons use it
        public static bool first = true;
        public void Start()
        {
            //only do it on the first entry to the menu
            if (first)
            {
                first = false;
                HighLogic.SaveFolder = "default";
                var game = GamePersistence.LoadGame("persistent", HighLogic.SaveFolder, true, false);
                if (game != null && game.flightState != null && game.compatible)
                {
                    FlightDriver.StartAndFocusVessel(game, 0);
                }
                //CheatOptions.InfiniteFuel = true;
            }
        }
    }
}