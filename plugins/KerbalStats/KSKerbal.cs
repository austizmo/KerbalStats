using System;
using UnityEngine;
using KSP.IO;

namespace KerbalStats
{
	public class KSKerbal
	{
		ProtoCrewMember kerbal;

		public int baseSanity;
		public int currentSanity;

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

		private int DetermineBaseSanity() {
			return 100;
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