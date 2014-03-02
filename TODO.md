TODO List
v1
UI Design
	-hook into game notification messages on important events (high stress, past breaking point, failed test)
	-display detail stats (numbers for bars, etc) in drop down box
Balance Tuning
Stress Triggers
	-crew death
	-collision
*	-bedlam
	-nearby explosion
Situation Modifiers
*	-time alone
*	-time with crew
	-flight path
*	-g level
*	-livable space to crew ratio
	-remaining resources
Panic Actions
*	-eva
*	-random burn
*	-increase crew stress
	-dump resources
	-undock craft
	-toggle action groups

v2
Astronaut Complex Integration
Kerbal Portait UI Integration
CLS Integration
	-VM: CLSSpace integration
TAC Integration
	-VM: life support level
	-ST: life support depleted
	-MBA: dump food
	-MBA: contaminate food
RemoteTech Integration
	-ST: loss of contact with mission control
Parts
	-sleeping quarters
		-allow resting
	-cryo chamber
		-allow long term sleep
	-gravity ring
		-removes g stress
Stress Mods:
	-docking (speed/distance to taget)?
Individual stress tolerances for Kerbals
	-socialite
	-pilot
	-scientist
Lessen modifier with repeated exposure
	-this kerbal been to the mun before? less stressful
	-'any' kerbal been to the mun before? slightly less stressful for all kerbals with each successful Mun mission


BUGS
bug when hiring new kerbals in game
bug on kerbal death
bug when loading kerbals

API NOTES

Vessel.cs
/// <summary>
/// You can add your own function to this callback to register a function that can provide flight control input
/// to the vessel. Once you've registered this callback, it will be called once per FixedUpdate. Provide flight
/// control input by modifying the FlightCtrlState passed to your function. This FlightCtrlState will already
/// contain the player's input, which you can modify or override as desired.
/// </summary>
/// <example>
/// <code>
/// void MyAutopilotFunction(FlightCtrlState s) {
///   s.yaw = 1;
/// }
/// ...
/// vessel.OnFlyByWire += MyAutopilotFunction
/// </code>
/// This will create an autopilot that always yaws hard to the right. You can probably devise something more useful, though...
/// </example>
public FlightInputCallback OnFlyByWire;