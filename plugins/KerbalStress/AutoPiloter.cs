using System;
using UnityEngine;

namespace KerbalStress
{
	public class AutoPiloter
	{
		public enum PilotPrograms { BURN_HOME, BURN_RANDOM, STOP_RESPONDING };
		public static void BurnHome(FlightCtrlState state) {

		}

		public static void BurnRandom(FlightCtrlState state) {
			state.mainThrottle = 1;
			state.pitch = KerbalStress.rng.Next(-1,1);
			state.yaw 	= KerbalStress.rng.Next(-1,1);
		}

		public static void StopResponding(FlightCtrlState state) {

		}
	}
}