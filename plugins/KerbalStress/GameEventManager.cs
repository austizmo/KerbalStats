using System;
using System.Collections.Generic;
using UnityEngine;

namespace KerbalStress
{
	/** EventArgs continer for bedlam event */
	public class BedlamEventArgs : EventArgs {
		public Vessel vessel;
	}

	/** Event handler for bedlam event */
	public delegate void BedlamHandler(KSKerbal sender, BedlamEventArgs e);

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

			foreach(KSKerbal kerbal in this.model.GetKerbals()) {
				kerbal.OnInciteBedlam += new BedlamHandler(OnInciteBedlam);
			}

			GameEvents.onVesselRecovered.Add(OnVesselRecovered);
			GameEvents.onLaunch.Add(OnLaunch);
			GameEvents.onGameStateSaved.Add(OnGameSaved);
			GameEvents.onCrewKilled.Add(OnCrewKilled);
			GameEvents.onCollision.Add(OnCollision);
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
			foreach(KSKerbal kerbal in this.model.GetKerbals()) {
				kerbal.OnCrewDeath();
			}
		}

		private void OnCollision(EventReport report) {
			Debug.Log("on collision");
			Vessel vessel = report.origin.vessel;
			if(vessel!=null) {
				foreach(KSKerbal kerbal in this.model.GetKerbals(vessel)) {
					kerbal.OnCollision();
				}
			}
		}

		private void OnInciteBedlam(KSKerbal sender, BedlamEventArgs report) {
			Debug.Log("caught incite bedlam event, notifying crewemates");
			foreach(KSKerbal kerbal in this.model.GetKerbals(report.vessel)) {
				if(kerbal.name == sender.name) continue; //don't add to our own stress level by inciting panic
				kerbal.OnBedlam();
			}
		}

		public void OnDestroy() {
			this.model = null;

			GameEvents.onVesselRecovered.Remove(OnVesselRecovered);
			GameEvents.onLaunch.Remove(OnLaunch);
			GameEvents.onGameStateSaved.Remove(OnGameSaved);
			GameEvents.onCrewKilled.Remove(OnCrewKilled);
			GameEvents.onCollision.Remove(OnCollision);
		}
	}
}