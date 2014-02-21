using System;
using System.Collections.Generic;
using UnityEngine;

namespace KerbalStats
{
	class StatsModel 
	{
		private List<KSKerbal> kerbals;

		/**
		 * Holds onto our list of KSKerbals and provides methods for accessing them
		 */
		public StatsModel() {
			//load our kerbals
			if(HighLogic.CurrentGame != null) {
				//Debug.Log("Loading Kerbals");
				this.kerbals = SaveManager.LoadKerbals();
				if(kerbals == null || kerbals.Count == 0) {	
						Debug.Log("Unable to load Kerbals "+this.kerbals.Count);
						CreateKerbals();
				} 
			} else {
				this.kerbals = new List<KSKerbal>();
			}
		}

		/**
		 * Create a KSKerbal for all the kerbals in our current game, then save them
		 */
		private void CreateKerbals() {
			//Debug.Log("Creating brand new kerbals");
			this.kerbals = new List<KSKerbal>();
			foreach (ProtoCrewMember kerbal in HighLogic.CurrentGame.CrewRoster) {
				this.kerbals.Add(new KSKerbal(kerbal));
			}
			SaveManager.SaveKerbals(this.kerbals);
		}

		public List<KSKerbal> GetKerbals() {
			//Debug.Log("getting kerbals all");
			return this.kerbals;
		}

		public List<KSKerbal> GetKerbals(Vessel vessel) {
			//Debug.Log("getting kerbals vessel");
			List<KSKerbal> crew = new List<KSKerbal>();
			foreach(ProtoCrewMember crewMember in vessel.GetVesselCrew()) {
				KSKerbal kerbal = GetKerbal(crewMember.name);
				crew.Add(kerbal);
			}
			return crew;
		}

		public KSKerbal GetKerbal(String name) {
			//Debug.Log("getting kerbal");
			foreach(KSKerbal kerbal in this.kerbals) {
				if(kerbal.name == name) {
					return kerbal;
				}
			}
			return null;
		}

		public void OnDestroy() {
			this.kerbals = null;
		}
	}
}