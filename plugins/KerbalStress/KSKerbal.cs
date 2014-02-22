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
		public double courage;
		public double stupidity;
		public bool isBadass;
		public int breakpoint;

		//stress info
		public const int BASE_STRESS 		= 0;
		public const int MAX_CURRENT_STRESS = 1;
		public const int MIN_CURRENT_STRESS	= -1;

		public const int MAX_STRESS_BREAKPOINT 	= 1000;

		//stressor levels
		public const double BASE_MISSION_STRESS = 0.1;
		public const double BASE_REST_STRESS 	= -0.1;
		public const double LOW_G_STRESS 		= 0.1;
		public const double MED_G_STRESS		= 0.2;
		public const double HIGH_G_STRESS		= 0.5;

		public const double BASE_VESSEL_STRESS 	= 0;
		public const double FULL_VESSEL_STRESS	= .1;


		//stress level breakpoints
		public const double LOW = .4;
		public const double HIGH = .8;

		//stress stats
		public double currentStress; 
		public double cumulativeStress;

		//timer values
		public double lastLaunchTime;
		public double lastReturnTime;

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
			this.courage		= kerbal.courage;
			this.stupidity		= kerbal.stupidity;
			this.isBadass		= kerbal.isBadass;

			this.breakpoint 	= CalculateBreakpoint();

			this.lastCheckup 	= Planetarium.GetUniversalTime();
			this.lastStressTest = Planetarium.GetUniversalTime();
		}

		/**
		 * Used by the XMLDeserializer to create KSKerbal objects from serialized file
		 */
		public KSKerbal() {}

		public void Checkup(bool activeCheck = false) {
			Vessel vessel = (activeCheck) ? FlightGlobals.ActiveVessel : null;

			this.currentStress = CalculateStress(vessel);
			
			double elapsed = Planetarium.GetUniversalTime() - this.lastCheckup;

			this.cumulativeStress += this.currentStress * elapsed;

			if(this.cumulativeStress < BASE_STRESS) this.cumulativeStress = BASE_STRESS;
			if(this.cumulativeStress >= this.breakpoint && this.currentStress > LOW) {
				OnStressTest();
			}

			this.lastCheckup = Planetarium.GetUniversalTime();
		}

		/**
		 * Invoked when a kerbal begins a mission, updates timers
		 */
		public void OnMissionBegin() {
			//Debug.Log("on mission begin invoked");
			this.onDuty = true;
		}

		/**
		 * Invoked upon successfull completion of a mission, updates timers and counters.
		 */
		public void OnMissionComplete() {
			//Debug.Log("on mission complete invoked for "+this.name);
			//remove from duty
			this.onDuty = false;
			//reset stress mod 
			this.currentStress = BASE_REST_STRESS;
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

		/**
		 * Determines cumulative stress level before Kerbals need to take stress tests
		 */
		private int CalculateBreakpoint() {
			if(this.isBadass) return MAX_STRESS_BREAKPOINT;
			return (int)(this.courage * MAX_STRESS_BREAKPOINT);
		}

		private double CalculateStress(Vessel vessel) {
			//off duty
			if(!this.onDuty) return BASE_REST_STRESS;

			//on duty with active vessel
			if(vessel != null) {
				double stress = GetVesselMod(vessel);
				stress += GetGLevelMod(vessel);

				return stress;
			} 
			//on duty, but not in active vessel
			else {
				return this.currentStress;
			}
		}

		private void OnStressTest() {
			double elapsed = Planetarium.GetUniversalTime() - this.lastStressTest;
			if(elapsed < 60) return;

			int test = KerbalStress.rng.Next(0,10);
			if(this.currentStress > LOW && this.currentStress < HIGH) { 
				if(test >= 5) { //50% chance of failure
					OnFailStressTest();
				}
			}
			if(this.currentStress >= HIGH) { 
				if(test >= 2) { //80% chance of failure
					OnFailStressTest();
				}
			}

			this.lastStressTest = Planetarium.GetUniversalTime();
		}

		private void OnFailStressTest() {
			//do a thing!
			Debug.Log(this.name+" failed a stress test!");
		}

		private double GetDockingMod() { return 0; }
		private double GetDistanceMod() { return 0; }
		private double GetSolitudeMod() { return 0; }
		private double GetTimeWithCrewMod() { return 0; }
		private double GetFlightPathMod() { return 0; } //unstable orbit, impact time, etc

		private double GetGLevelMod(Vessel vessel) {
			double gForce 	= vessel.geeForce;
			if(gForce < .65) { //exposure to low g is a major cause of physical and mental stress in the real world
				return BASE_MISSION_STRESS + LOW_G_STRESS;
			} else if(gForce >= .65 && gForce <= 1.55) { //if we're around normal g, it's not that stressful
				return BASE_MISSION_STRESS;
			} else if(gForce > 1.55 && gForce <= 6) { //medium g, trained individuals in g suits should be able to hand this with little trouble, but not for sustained periods
				return BASE_MISSION_STRESS + MED_G_STRESS;	
			} else if(gForce > 6 && gForce <= 9) { //even trained astronauts in g suits have difficulty with this level of g
				return BASE_MISSION_STRESS + HIGH_G_STRESS;
			} else { //G > 9 at this level, you're probably unconscious, regardless of who you are
				return MAX_CURRENT_STRESS;
			}
		}

		private double GetVesselMod(Vessel vessel) {
			int maxCrew 	= vessel.GetCrewCapacity();
			int totalCrew 	= vessel.GetCrewCount();

			double vesselStress = BASE_VESSEL_STRESS;
			//no extra space
			if(totalCrew == maxCrew) {
				vesselStress += FULL_VESSEL_STRESS;
			} 
			return vesselStress;
		}

		private double GetDeltaVMod() { return 0; }
		private double GetChargeMod() { return 0; }
	}
}

