using System;
using System.Collections.Generic;
using UnityEngine;

namespace KerbalStats
{
	class KerbalObserver
	{
		private StatsModel model;

		/**
		 * Listens for interesting events, and alerts the appropriate KSKerbals when they occur
		 *
		 * Takes a StatsModel instance, for access to Kerbals
		 */
		public KerbalObserver(StatsModel model) {
			this.model = model;

			GameEvents.onVesselRecovered.Add(OnVesselRecovered);
			GameEvents.onLaunch.Add(OnLaunch);
		}

		private void OnVesselRecovered(ProtoVessel vessel) {
			Debug.Log("OnVesselRecovered");
			foreach(KSKerbal kerbal in this.model.GetKerbals(vessel.vesselRef)) {
				kerbal.OnMissionComplete();
			}
		}

		private void OnLaunch(EventReport report)
		{
			Debug.Log("on launch triggered");
			Vessel vessel = FlightGlobals.ActiveVessel;
			if(vessel!=null) {
				List<KSKerbal> crew = this.model.GetKerbals(vessel);
				foreach(KSKerbal kerbal in crew) {
					kerbal.OnMissionBegin();
				}
			}
		}

		public void OnDestroy() {
			this.model = null;

			GameEvents.onVesselRecovered.Remove(OnVesselRecovered);
			GameEvents.onLaunch.Remove(OnLaunch);
		}
	}
}