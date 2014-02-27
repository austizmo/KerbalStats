using System;
using System.Collections.Generic;
using UnityEngine;

namespace KerbalStress
{
	class GameEventManager
	{
		private StatsModel model;

		/**
		 * Listens for interesting events, and alerts the appropriate KSKerbals when they occur
		 *
		 * Takes a StatsModel instance, for access to Kerbals
		 */
		public GameEventManager(ref StatsModel model) {
			this.model = model;

			GameEvents.onVesselRecovered.Add(OnVesselRecovered);
			GameEvents.onLaunch.Add(OnLaunch);
			GameEvents.onGameStateSaved.Add(OnGameSaved);
			GameEvents.onCrewKilled.Add(OnCrewKilled);
		}

		private void OnVesselRecovered(ProtoVessel vessel) {
			Debug.Log("OnVesselRecovered");
			if(vessel != null && vessel.vesselRef != null) {
				foreach(KSKerbal kerbal in this.model.GetKerbals(vessel.vesselRef)) {
					kerbal.OnMissionComplete();
				}
			}
		}

		private void OnLaunch(EventReport report) {
			Debug.Log("on launch triggered");
			Vessel vessel = FlightGlobals.ActiveVessel;
			if(vessel!=null) {
				List<KSKerbal> crew = this.model.GetKerbals(vessel);
				foreach(KSKerbal kerbal in crew) {
					kerbal.OnMissionBegin();
				}
			}
		}

		private void OnGameSaved(Game game) {
			Debug.Log("on game saved");
			SaveManager.SaveKerbals(this.model.GetKerbals());
		}

		private void OnCrewKilled(EventReport report) {
			Debug.Log("on crew killed");
			Vessel vessel = report.origin.vessel;
			if(vessel!=null) {
				foreach(KSKerbal kerbal in this.model.GetKerbals(vessel)) {
					kerbal.OnDeath();
				}
			}
			foreach(KSKerbal kerbal in this.model.GetKerbals()) {
				kerbal.OnCrewDeath(this.model.GetKerbals(vessel).Count);
			}
		}

		public void OnDestroy() {
			this.model = null;

			GameEvents.onVesselRecovered.Remove(OnVesselRecovered);
			GameEvents.onLaunch.Remove(OnLaunch);
			GameEvents.onGameStateSaved.Remove(OnGameSaved);
			GameEvents.onCrewKilled.Remove(OnCrewKilled);
		}
	}
}