using System;
using System.Collections.Generic;
using UnityEngine;

namespace KerbalStats
{
	class KerbalObserver
	{
		private StatsModel model;

		public KerbalObserver(StatsModel model) {
			this.model = model;

			GameEvents.onVesselRecovered.Add(OnVesselRecovered);
			GameEvents.onLaunch.Add(OnLaunch);
		}

		private void OnVesselRecovered(ProtoVessel vessel) {
			List<KSKerbal> crew = this.model.GetKerbals(vessel.vesselRef);
			foreach(KSKerbal kerbal in crew) {
				kerbal.OnMissionComplete();
			}
		}

		private void OnLaunch(EventReport report)
		{
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
		}
	}
}