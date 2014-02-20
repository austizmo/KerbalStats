using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using KSP.IO;

namespace KerbalStats
{
	/**
	 * KSKerbal represents a unique kerbal, and contains properties pertaining to its stats, methods for serialization and data access, and methods for updating stats on events. 
	 */
	public class KSKerbal
	{
		//used as a unique identifier for this kerbal
		public String name;

		//stat values
		public double baseSanity;
		public double currentSanity;
		public double hiredTime;
		public double lastLaunchTime;
		public double lastReturnTime;
		public double totalMissionTime		= 0;
		public double currentMissionTime	= 0;

		//True when kerbal is on a mission
		private bool onDuty = false;

		/**
		 * Given a ProtoCrewMember, create a new KSKerbal with default stats
		 */
		public KSKerbal(ProtoCrewMember kerbal) {
			Debug.Log("Creating new kerbal");
			this.name 			= kerbal.name;
			this.baseSanity 	= DetermineBaseSanity();
			this.currentSanity 	= this.baseSanity;
			this.hiredTime 		= Planetarium.GetUniversalTime();
			this.lastLaunchTime = Planetarium.GetUniversalTime();
			this.lastReturnTime = Planetarium.GetUniversalTime();
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
		 * Determines the kerbals max sanity stat
		 * called once when creating a kerbal for the first time
		 */
		private double DetermineBaseSanity() {
			System.Random random = new System.Random();
			int s = random.Next(70,100);
			return (double)s/100;
		}

		/**
		 * TODO: replace with a better stats screen
		 * Returns a string of the kerbal's stats, formatted for use in the stats window
		 */
		public String PrintStats() {
			String stats = "Sanity: \t" + this.currentSanity.ToString() + "/" + this.baseSanity.ToString() + "\n";
			stats += "Total Mission Time: \t" + this.totalMissionTime + "\n";
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
		}

		/**
		 * Invoked upon successfull completion of a mission, updates timers and counters.
		 */
		public void OnMissionComplete() {
			Debug.Log("on mission complete invoked");
			//set the last return time
			this.lastReturnTime = Planetarium.GetUniversalTime();

			//add to the total mission time
			this.totalMissionTime += this.lastReturnTime - this.lastLaunchTime;

			//reset current mission time
			this.currentMissionTime = 0;

			//remove from duty
			this.onDuty = false;
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