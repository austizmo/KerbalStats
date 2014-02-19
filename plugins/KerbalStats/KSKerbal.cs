using System;
using System.Collections.Generic;
using UnityEngine;
using KSP.IO;

namespace KerbalStats
{
	public class KSKerbal
	{
		public ProtoCrewMember kerbal;

		public String name;

		public double baseSanity;
		public double currentSanity;

		public int totalMissionTime		= 0;
		public int currentMissionTime	= 0;
		public int lastLaunchTime		= 0;
		public int lastReturnTime		= 0;

		private static readonly Char[] FIELD_SEPARATOR = new Char[] { ',' };
		private static readonly Char[] PAIR_SEPARATOR = new Char[] { '=' };

		public KSKerbal(ProtoCrewMember kerbal) {
			Debug.Log("Creating new kerbal");
			this.name 			= kerbal.name;
			this.baseSanity 	= DetermineBaseSanity();
			this.currentSanity 	= this.baseSanity;
		}

		public KSKerbal(String kerbal) {
			Debug.Log("Creating kerbal from string");
			Dictionary<String, String> properties = Deserialize(kerbal);
			this.name 				= properties["name"];
			this.baseSanity 		= double.Parse(properties["baseSanity"]);
			this.currentSanity		= double.Parse(properties["currentSanity"]);
			this.totalMissionTime	= int.Parse(properties["totalMissionTime"]);
			this.currentMissionTime = int.Parse(properties["currentMissionTime"]);
			this.lastLaunchTime		= int.Parse(properties["lastLaunchTime"]);
			this.lastReturnTime		= int.Parse(properties["lastReturnTime"]);
		}

		private double DetermineBaseSanity() {
			System.Random random = new System.Random();
			int s = random.Next(70,100);
			return (double)s/100;
		}

		public String PrintStats() {
			String stats = "Sanity: \t" + this.currentSanity.ToString() + "/" + this.baseSanity.ToString() + "\n";
			return stats;
		}

		public void OnMissionComplete() {
			//add to total mission time

			//set last return time

			//reset current mission time
		}

		public String Serialize() {
			String serialized = 
				"name="					+ this.name 				+
				",baseSanity="			+ this.baseSanity			+
				",currentSanity="		+ this.currentSanity		+
				",totalMissionTime="	+ this.totalMissionTime		+	
				",currentMissionTime="	+ this.currentMissionTime	+
				",lastLaunchTime="		+ this.lastLaunchTime		+
				",lastReturnTime="		+ this.lastReturnTime;

			return serialized;
		}

		private Dictionary<String, String> Deserialize(String kerbalString) {
			String[] fields = kerbalString.Split(FIELD_SEPARATOR);
			Dictionary<String, String> dict = new Dictionary<String, String>();
           	foreach(String field in fields) {
           		String[] values = field.Split(PAIR_SEPARATOR, 2);
           		dict.Add(values[0], values[1]);
           	}
           	return dict;
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