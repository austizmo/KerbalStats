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
		public const int MAX_CURRENT_STRESS = 10;
		public const int MIN_CURRENT_STRESS	= -10;

		public const int MAX_STRESS_BREAKPOINT 	= 1000000;

		//stressor levels
		public const double BASE_REST_STRESS 	= -3.75;
		public const double BASE_MISSION_STRESS = 0.1;

		public const double BASE_SOCIAL_STRESS 	= 0.18;

		public const double FULL_VESSEL_STRESS	= 1;

		public const double FALLING_STRESS 		= .5;
		public const double SUB_ORBITAL_STRESS 	= .5;

		//stress trigger levels
		public const double CREW_DEATH_STRESS 	= 100000;
		public const double BEDLAM_STRESS		= 25000;
		public const double EXPLOSION_STRESS	= 10000;
		public const double COLLISION_STRESS	= 5000;

		//stress level references
		public const double LOW 	= .33;
		public const double MED 	= 3.75;
		public const double HIGH 	= 10;

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

		public event BedlamHandler OnInciteBedlam;

		private Vessel vessel;

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
			this.vessel = (activeCheck) ? FlightGlobals.ActiveVessel : null;

			this.currentStress = CalculateStress();
			
			double elapsed = Planetarium.GetUniversalTime() - this.lastCheckup;

			this.cumulativeStress += this.currentStress * elapsed;

			if(this.cumulativeStress < BASE_STRESS) this.cumulativeStress = BASE_STRESS;
			if(this.cumulativeStress >= this.breakpoint && this.currentStress >= MED) {
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
			//reset stress mod and timers
			this.currentStress = BASE_REST_STRESS;
			this.timeAlone = 0;
			this.timeWithCrew = 0;
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
			double percentMod = (this.courage < .50) ? .50 : this.courage;
			return (int)(percentMod * MAX_STRESS_BREAKPOINT);
		}

		/**
		 * Calculates and stores the Kerbal's current stress level. 
		 * Limits current stress to max current stress
		 */
		private double CalculateStress() {
			//off duty
			if(!this.onDuty) return BASE_REST_STRESS;

			//on duty with active vessel
			if(this.vessel != null) {
				double stress = GetVesselMod();
				stress += GetGLevelMod();
				stress += GetSocialMod();
				stress += GetFlightPathMod();

				if(stress > MAX_CURRENT_STRESS) stress = MAX_CURRENT_STRESS; //limit stress to make balance simpler. nice to have a lower limit on space flight durations

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
			//TODO: badass flag/low stupidity to influence failure chance
			//TODO: balance failure chance
			double elapsed = Planetarium.GetUniversalTime() - this.lastStressTest;
			if(elapsed < 60) return;

			int test = KerbalStress.rng.Next(0,10);//GetFailureChance();
			if(this.currentStress >= MED && this.currentStress < HIGH) { 
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
		 * Called when a kerbal fails a stress test. Determines which of our panic actions to take.
		 */
		private void OnFailStressTest() {
			//TODO: choose panic action based on current situation, attempt to avoid unrecoverable actions, like going eva on reentry
			//TODO: stupidity influences severity of failure
			int choice = KerbalStress.rng.Next(0,1);
			switch(choice) {
				case 0: 
					this.GoEVA();
					break;
				case 1:
					this.InciteBedlam();
					break;
			}
		}

		public void OnDeath() {
			Debug.Log(this.name + " has died. should I do something?");
		}

		private ProtoCrewMember GetProtoCrewMember() {
			if(this.vessel == null) return null;

			foreach(ProtoCrewMember crew in this.vessel.GetVesselCrew()) {
				if(this.name == crew.name) {
					return crew;
				}
			}

			return null;
		}

		/***************
		* Stress Triggers, single instance events which add directly to cumulative stress
		***************/
		
		/**
		 * called when another kerbal dies
		 * 
		 * @type {int} number of crew members lost
		 */
		public void OnCrewDeath(int count) {
			//TODO: allow for increasing stress when multiple crew die
			this.cumulativeStress += CREW_DEATH_STRESS;
		}

		public void OnCollision() {
			this.cumulativeStress += COLLISION_STRESS;
		}

		/** 
		 * called when another kerbal on your vessel panics and incites bedlam
		 */
		public void OnBedlam() {
			//TODO: track number of kerbals inciting bedlam and increase multiplier
			this.cumulativeStress += BEDLAM_STRESS;
		}
		public void OnExplosion() {
			//TODO: adjust for awesomeness of explosion
			this.cumulativeStress += EXPLOSION_STRESS;
		}

		/***************
		* Stress Modification Functions, return modifier for current stress based on current situations
		***************/

		/**
		 * Returns the stress modifier indicated by the kerbals socialization timers
		 * Being alone or with crew for more than a day increases stress levels
		 *
		 * @type {Vessel} the active vessel
		 */
		private double GetSocialMod() { 
			//TODO: add check for living space changes, if you have enough spaces so that one space can always host a single kerbal,
			//assume kerbals move within the craft to deal with their social needs (implement seat moving for this purpose?) CLS intergration dependant
			double elapsed = Planetarium.GetUniversalTime() - lastSocialCheck;
			double time;
			double stress;

			if(this.vessel.GetCrewCount() == 1) {
				this.timeWithCrew = 0;
				this.timeAlone += elapsed;
				time = this.timeAlone;
			} else {
				this.timeAlone = 0;
				this.timeWithCrew += elapsed;
				time = this.timeWithCrew;
			}

			if (time > Utils.SECONDS_IN_A_KDAY) {
				stress = (time/Utils.SECONDS_IN_A_KDAY) * BASE_SOCIAL_STRESS;
			} else {
				stress = 0;
			}

			this.socialMod = stress;

			this.lastSocialCheck = Planetarium.GetUniversalTime();
			return stress;
		}

		/**
		 * Returns the stress mod indicated by the current flight path
		 *
		 * @type {Vessel} the active vessel
		 */
		private double GetFlightPathMod() { 
			Vessel.Situations situation = this.vessel.situation;
			double stress = 0;
			switch(situation) {
				case Vessel.Situations.FLYING:
					//TODO: add altitude and impact time stresses
					if(this.vessel.verticalSpeed < -20) { //falling at ~45mph
						stress = FALLING_STRESS;
					}
					break;
				case Vessel.Situations.SUB_ORBITAL:
					if(this.vessel.verticalSpeed < 0) { 
						stress = SUB_ORBITAL_STRESS;
					}
					break;
				case Vessel.Situations.ORBITING:
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
		private double GetGLevelMod() {
			double gForce 	= this.vessel.geeForce;
			double stress 	= 0;
			
			//low g stress is a major concern over long duration flights
			if(gForce < .65) { 
				stress = .65 - gForce;
			}
			//if we're around normal g, it's not stressful 
			else if(gForce >= .65 && gForce <= 1.50) { 
				stress = 0;
			} 
			//under 6G, we should scale stress along a curve
			else if(gForce > 1.5 && gForce <= 6) {
				stress = (0.138889*Math.Pow(gForce, 3)) - (1.23611*Math.Pow(gForce, 2)) + (4.04167*gForce) - 3.75; //https://www.wolframalpha.com/input/?i=polynomial+fit&a=*C.polynomial+fit-_*Calculator.dflt-&f2=%7B%7B1.5%2C0%7D%2C%7B2%2C.5%7D%2C%7B3%2C1%7D%2C%7B6%2C6%7D%7D&f=InterpolatingPolynomialCalculator.data2_%7B%7B1.5%2C0%7D%2C%7B2%2C.5%7D%2C%7B3%2C1%7D%2C%7B6%2C6%7D%7D&a=*FVarOpt.1-_***InterpolatingPolynomialCalculator.data2--.***InterpolatingPolynomialCalculator.data---.*--
			}
			//otherwise, stress = gForce
			else {
				stress = gForce;
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
		private double GetVesselMod() {
			//TODO: implement part modules and check all parts in the vessel for modifiers
			int maxCrew 	= this.vessel.GetCrewCapacity();
			int totalCrew 	= this.vessel.GetCrewCount();
			int extraSpace 	= totalCrew - maxCrew;
			
			double vesselStress = 0;
			//no extra space
			if(extraSpace <= 0) {
				vesselStress = FULL_VESSEL_STRESS;
			} 
			//we have extra space, give a low resting bonus
			else { 
				if(extraSpace > 5) extraSpace = 5;
				vesselStress = -Math.Log(2, extraSpace)/2;
			}

			this.vesselMod = vesselStress;
			return vesselStress;
		}

		private double GetResourceMod() { return 0; }

		/***************
		* Panic Actions, things to do when a stress test is failed
		***************/
		
		private void GoEVA() {
			//TODO: implement eva burn, towards nearest body?
			FlightEVA eva = FlightEVA.fetch;
			ProtoCrewMember crew = this.GetProtoCrewMember();
			eva.spawnEVA(crew, crew.KerbalRef.InPart, crew.KerbalRef.InPart.airlock);
		}

		private void StopResponding() {}
		private void InitiateBurn() {}

		/**
		* Adds a static amount of stress to all kerbals in the same vessel
		*/
		public void InciteBedlam() {
			//Debug.Log("fire incite bedlam event");
			BedlamEventArgs report = new BedlamEventArgs();
			report.vessel = this.vessel;
			this.OnInciteBedlam(this, report);
		}

		private void DumpResources() {}
		private void UndockCraft() {}
		private void FlipSwitches() {} //toggle action groups at random
		private void StageCraft() {}
	}
}

