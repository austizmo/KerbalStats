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
		}

		private void OnVesselRecovered(ProtoVessel vessel) {
			List<KSKerbal> crew = this.model.GetKerbals(vessel.vesselRef);
			foreach(KSKerbal kerbal in crew) {
				kerbal.OnMissionComplete();
			}
		}

		public void OnDestroy() {
			this.model = null;
		}
	}
}