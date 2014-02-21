using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using KSP.IO;

namespace KerbalStress
{
	/**
	 * KSKerbal represents a unique kerbal, and contains properties pertaining to its stats, methods for serialization and data access, and methods for updating stats on events. 
	 */
	public class KSKerbal
	{
		//used as a unique identifier for this kerbal
		public String name;

		//stress info
		public const int BASE_STRESS 		= 0;
		public const int MAX_CURRENT_STRESS = 1;
		public const int MIN_CURRENT_STRESS	= -1;
		public const int STRESS_BREAKPOINT 	= 100;

		//stress level breakpoints
		public const double LOW = .4;
		public const double HIGH = .8;

		//stress stats
		public double currentStress; 
		public double cumulativeStress;

		//stress modifiers
		public double vesselModifier;

		//timer values
		public double lastLaunchTime;
		public double lastReturnTime;
		public double currentMissionTime	= 0;

		//True when kerbal is on a mission
		public bool onDuty = false;

		public double lastCheckup;
		public double lastStressTest;

		/**
		 * Given a ProtoCrewMember, create a new KSKerbal with default stats
		 */
		public KSKerbal(ProtoCrewMember kerbal) {
			//Debug.Log("Creating new kerbal");
			this.name 			= kerbal.name;

			this.lastLaunchTime = Planetarium.GetUniversalTime();
			this.lastReturnTime = Planetarium.GetUniversalTime();
			this.lastCheckup 	= Planetarium.GetUniversalTime();
			this.lastStressTest = Planetarium.GetUniversalTime();
		}

		/**
		 * Used by the XMLDeserializer to create KSKerbal objects from serialized file
		 */
		public KSKerbal() {}

		/**
		 * Returns the elapsed time for the Kerbal's current mission
		 */
		public double CurrentMissionTime {
			get {
				if(!this.onDuty) return 0;
				return Planetarium.GetUniversalTime() - this.lastLaunchTime;
			}
		}

		/**
		 * represents the amount of stress added to the cumulative stress stat at each checkup
		 */
		public double CurrentStress {
			get {
				double stress;
				if(this.onDuty) {
					stress = MAX_CURRENT_STRESS * .01;
				} else {
					stress = MAX_CURRENT_STRESS * -.01;
				}
				this.currentStress = stress + vesselModifier;
				return this.currentStress;
			}
		}

		public void Checkup(bool activeCheck = false) {
			if(activeCheck) this.vesselModifier = CalculateVesselStressMod();

			double elapsed = Planetarium.GetUniversalTime() - this.lastCheckup;

			this.cumulativeStress += this.CurrentStress * elapsed;

			if(this.cumulativeStress < BASE_STRESS) this.cumulativeStress = BASE_STRESS;
			if(this.cumulativeStress >= STRESS_BREAKPOINT) {
				OnStressTest();
			}

			this.lastCheckup = Planetarium.GetUniversalTime();
		}

		/**
		 * TODO: replace with a better stats screen
		 * Returns a string of the kerbal's stats, formatted for use in the stats window
		 */
		public String PrintStats() {
			String stats = "Stress Level: \t" + this.CurrentStress + "\n";
			stats += "Cumulative Stress: \t" + this.cumulativeStress + "\n";
			if(this.onDuty) {
				stats += "Time on Current Mission: \t" + this.CurrentMissionTime + "\n";
			} else {
				stats += "Time Rested: \t" + (Planetarium.GetUniversalTime() - this.lastReturnTime) + "\n";
			}
			return stats;
		}

		/**
		 * Invoked when a kerbal begins a mission, updates timers
		 */
		public void OnMissionBegin() {
			Debug.Log("on mission begin invoked");
			this.onDuty = true;
			this.lastLaunchTime = Planetarium.GetUniversalTime();
			this.vesselModifier = CalculateVesselStressMod();
		}

		/**
		 * Invoked upon successfull completion of a mission, updates timers and counters.
		 */
		public void OnMissionComplete() {
			Debug.Log("on mission complete invoked for "+this.name);
			if(!this.onDuty) {
				//prevent counting mission stats when recovering unlaunched vehicles from the launch pad
				Debug.Log("Not on duty, not recording mission stats");
				return;	
			}

			//set the last return time
			this.lastReturnTime = Planetarium.GetUniversalTime();
			//reset current mission time
			this.currentMissionTime = 0;
			//remove from duty
			this.onDuty = false; 
			//reset stress mod
			this.vesselModifier = 0;
		}

		/**
		 * Returns a string representing the kerbal's relevant stats
		 */
		public String Serialize() {
			XmlSerializer xmlSerializer = new XmlSerializer(this.GetType());
			StringWriter textWriter = new StringWriter();

			xmlSerializer.Serialize(textWriter, this);
			textWriter.Close();
			return textWriter.ToString();
		}

		private double CalculateVesselStressMod() {
			Vessel vessel 	= FlightGlobals.ActiveVessel;
			double gForce 	= vessel.geeForce;
			int maxCrew 	= vessel.GetCrewCapacity();
			int totalCrew 	= vessel.GetCrewCount();
			return 0.01;
		}

		private void OnStressTest() {
			if(this.CurrentStress <= LOW) return;

			double elapsed = Planetarium.GetUniversalTime() - this.lastStressTest;
			if(elapsed < 60) return;

			int test = KerbalStress.rng.Next(0,10);
			if(this.CurrentStress > LOW && this.CurrentStress < HIGH) { 
				if(test >= 5) Debug.Log(this.name+" failed a stress test!"); //50% chance of failure
			}
			if(this.CurrentStress >= HIGH) { 
				if(test >= 2) Debug.Log(this.name+" failed a stress test!"); //80% chance of failure
			}

			this.lastStressTest = Planetarium.GetUniversalTime();
		}
	}
}

/**
 * STAT IDEAS
 * 
 * distance traveled
 * bodies visited
 * 	- orbits completed
 * 	- times landed
 * 	- steps taken
 * dockings completed
 * flights landed
 * collisions survived
 * explosions survived
 * health
 * happiness
 * 
 */