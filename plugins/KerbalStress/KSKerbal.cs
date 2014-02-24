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
		public String firstName;
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
		public const double BASE_REST_STRESS 	= 0;//-0.1;
		public const double LOW_G_STRESS 		= 0.1;
		public const double MED_G_STRESS		= 0.2;
		public const double HIGH_G_STRESS		= 0.5;
		public const double BASE_SOCIAL_STRESS	= .1;
		public const double BASE_VESSEL_STRESS 	= 0;
		public const double FULL_VESSEL_STRESS	= 0.1;
		public const double SUB_ORBITAL_STRESS	= 0.1;
		public const double ORBITAL_STRESS 		= -0.1;


		//stress level breakpoints
		public const double LOW = .4;
		public const double HIGH = .8;

		//stress stats
		public double currentStress; 
		public double cumulativeStress;

		//timer values
		public double timeAlone;
		public double timeWithCrew;

		//True when kerbal is on a mission
		public bool onDuty = false;

		public double lastCheckup;
		public double lastStressTest;
		public double lastSocialCheck;

		public double socialMod;
		public double flightPathMod;
		public double vesselMod;
		public double gLevelMod;

		private static readonly Char[] NAME_DELIMITER = new Char[] { ' ' };

		/**
		 * Given a ProtoCrewMember, create a new KSKerbal with default stats
		 */
		public KSKerbal(ProtoCrewMember kerbal) {
			//Debug.Log("Creating new kerbal");
			this.name 			= kerbal.name;
			this.firstName 		= kerbal.name.Split(NAME_DELIMITER)[0];
			this.courage		= kerbal.courage;
			this.stupidity		= kerbal.stupidity;
			this.isBadass		= kerbal.isBadass;

			this.breakpoint 	= CalculateBreakpoint();

			this.lastCheckup 	= Planetarium.GetUniversalTime();
			this.lastStressTest = Planetarium.GetUniversalTime();
			this.lastSocialCheck= Planetarium.GetUniversalTime();
		}

		/**
		 * Used by the XMLDeserializer to create KSKerbal objects from serialized file
		 */
		public KSKerbal() {}

		/**
		 * Invoked repeatedly (once per second for active vessel kerbals, twice per minute for all others)
		 * Determines the current stress levels for kerbals and checks if they should make a stress test
		 * 
		 * @type {bool} is this check for the active vessel only?
		 */
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

		/**
		 * Calculates and stores the Kerbal's current stress level. 
		 */
		private double CalculateStress(Vessel vessel) {
			//off duty
			if(!this.onDuty) return BASE_REST_STRESS;

			//on duty with active vessel
			if(vessel != null) {
				double stress = GetVesselMod(vessel);
				stress += GetGLevelMod(vessel);
				stress += GetSocialMod(vessel);
				stress += GetFlightPathMod(vessel);

				return stress;
			} 
			//on duty, but not in active vessel
			else {
				return this.currentStress;
			}
		}

		/**
		 * Called when a kerbal must make a stress test. Determines if the Kerbal fails the test.
		 */
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

		/**
		 * Called when a kerbal fails a stress test. Determines which of our mental breack actions to take.
		 */
		private void OnFailStressTest() {
			//do a thing!
			Debug.Log(this.name+" failed a stress test!");
		}

		/***************
		* Stress Triggers, single instance events which add directly to cumulative stress
		***************/
		public void OnCrewDeath() {}
		public void OnCollision() {}
		public void OnBedlam() {}
		public void OnExplosion() {}

		/***************
		* Stress Modification Functions, return modifier for current stress based on current situations
		***************/

		/**
		 * Returns the stress modifier indicated by the kerbals socialization timers
		 * Being alone or with crew for more than a day increases stress levels
		 *
		 * @type {Vessel} the active vessel
		 */
		private double GetSocialMod(Vessel vessel) { 
			//TODO: add check for living space changes, if you have enough spaces so that one space can always host a single kerbal,
			//assume kerbals move within the craft to deal with their social needs (implement seat moving for this purpose?)
			double elapsed = Planetarium.GetUniversalTime() - lastSocialCheck;
			double time;
			double stress;

			if(vessel.GetCrewCount() == 1) {
				this.timeWithCrew = 0;
				this.timeAlone += elapsed;
				time = this.timeAlone;
			} else {
				this.timeAlone = 0;
				this.timeWithCrew += elapsed;
				time = this.timeWithCrew;
			}

			if (time > Utils.SECONDS_IN_A_KDAY) {
				stress = (time % Utils.SECONDS_IN_A_KDAY) * BASE_SOCIAL_STRESS; //TODO: tune these numbers so the stress mod scales for long missions
			} else {
				stress = 0;
			}

			this.socialMod = stress;
			return stress;
		}

		/**
		 * Returns the stress mod indicated by the current flight path
		 *
		 * @type {Vessel} the active vessel
		 */
		private double GetFlightPathMod(Vessel vessel) { 
			Vessel.Situations situation = vessel.situation;
			double stress;
			switch(situation) {
			case Vessel.Situations.SUB_ORBITAL:
					//TODO: increase with decreasing impact time
					//TODO: increase if vertical velocity is decreasing
					stress = SUB_ORBITAL_STRESS;
					break;
			case Vessel.Situations.ORBITING:
					stress = ORBITAL_STRESS;
					break;
				default:
					stress = 0;
					break;
			}
			this.flightPathMod = stress;
			return stress;
		} 

		/**
		 * Returns the stress mod indicated by the current G level
		 *
		 * @type {Vessel} the active vessel
		 */
		private double GetGLevelMod(Vessel vessel) {
			double gForce 	= vessel.geeForce;
			double stress 	= 0;
			if(gForce < .65) { //exposure to low g is a major cause of physical and mental stress in the real world
				stress = BASE_MISSION_STRESS + LOW_G_STRESS;
			} else if(gForce >= .65 && gForce <= 1.55) { //if we're around normal g, it's not that stressful
				stress = BASE_MISSION_STRESS;
			} else if(gForce > 1.55 && gForce <= 6) { //medium g, trained individuals in g suits should be able to hand this with little trouble, but not for sustained periods
				stress = BASE_MISSION_STRESS + MED_G_STRESS;	
			} else if(gForce > 6 && gForce <= 9) { //even trained astronauts in g suits have difficulty with this level of g
				stress = BASE_MISSION_STRESS + HIGH_G_STRESS;
			} else { //G > 9 at this level, you're probably unconscious, regardless of who you are
				stress = MAX_CURRENT_STRESS;
			}

			this.gLevelMod = stress;
			return stress;
		}

		/**
		 * Returns the stress mod indicated by the current vessel configuration
		 * Tests crew capacity vs current crew, eventually tests for part modules
		 *
		 * @type {Vessel} the active vessel
		 */
		private double GetVesselMod(Vessel vessel) {
			int maxCrew 	= vessel.GetCrewCapacity();
			int totalCrew 	= vessel.GetCrewCount();

			double vesselStress = BASE_VESSEL_STRESS;
			//no extra space
			if(totalCrew == maxCrew) {
				vesselStress += FULL_VESSEL_STRESS;
			} 

			this.vesselMod = vesselStress;
			return vesselStress;
		}

		private double GetDistanceMod() { return 0; }
		private double GetDeltaVMod() { return 0; }
		private double GetChargeMod() { return 0; }

		/***************
		* Mental Break Actions, things to do when a stress test is failed
		***************/
		private void GoEVA() {}
		private void StopResponding() {}
		private void InitiateBurn() {}
		private void InciteBedlam() {}
		private void DumpResources() {}
		private void UndockCraft() {}
		private void FlipSwitches() {} //toggle action groups at random
		private void StageCraft() {}
	}
}

