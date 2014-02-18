using System;
using UnityEngine;
using KSP.IO;

namespace KerbalStats
{
	public class KSKerbal
	{
		public ProtoCrewMember kerbal;

		public float baseSanity;
		public float currentSanity;

		public String name {
			get {
				return this.kerbal.name;
			}
		}

		public KSKerbal(ProtoCrewMember kerbal) {
			Debug.Log("Creating new kerbal");
			this.kerbal 		= kerbal;
			this.baseSanity 	= DetermineBaseSanity();
			this.currentSanity 	= this.baseSanity;
		}

		private float DetermineBaseSanity() {
			System.Random random = new System.Random();
			int s = random.Next(70,100);
			return s/100;
		}

		public String PrintStats() {
			String stats = "Sanity: \t" + this.currentSanity.ToString() + "/" + this.baseSanity.ToString() + "\n";
			return stats;
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