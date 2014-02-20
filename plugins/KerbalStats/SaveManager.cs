using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;

namespace KerbalStats
{
	/**
	 * Handles saving and loading of Kerbal data to disk
	 */
	static class SaveManager
	{
		private static readonly String ROOT_PATH = Utils.GetRootPath();
		private static readonly String SAVE_BASE_FOLDER = ROOT_PATH + "/saves/";
		private static readonly String SAVE_FILE = SAVE_BASE_FOLDER + HighLogic.SaveFolder + "/kstats.sav";

		/**
		 * Reads serialized xml KSKerbals from save file specified in static members
		 * Though serialized to XML, the kerbals are not stored in an xml file due to linmitations with the serializer, 
		 * instead each kerbal has it's own xml file within a larger save file,
		 * this function must parse this larger file into sections approriate to be deserialized
		 */
		public static List<KSKerbal> LoadKerbals() {
			//Debug.Log("Loading Kerbals from disk");
			List<KSKerbal> kerbals = new List<KSKerbal>();
			XmlSerializer xmlSerializer = new XmlSerializer(Type.GetType("KerbalStats.KSKerbal"));
			StreamReader file = null;
			try {
				if(!File.Exists(SAVE_FILE)) {
					Debug.Log("Unable to find " + SAVE_FILE);
					return kerbals;
				}

				file = File.OpenText(SAVE_FILE);

				String line; //line holds out current line
				String section = ""; //section holds the set of lines which makes up the xml segment we have parsed so far
				while( (line=file.ReadLine()) != null ) { //loop through our file
					if (line.Trim().Length > 0) {
						if(line == "</KSKerbal>") { //if this line indicates that we're at the end of a KSKerbal definition, add it to the section, then deserialize the section
							section += line;
							StringReader stream = new StringReader(section);
							KSKerbal kerbal = (KSKerbal) xmlSerializer.Deserialize(stream);
							//Debug.Log("Adding "+kerbal.name);
							kerbals.Add(kerbal);
							stream.Close();
							section = "";
						} else { //otherwise, just add the line to the current section
							section += line;
						}		
					} else {
						Debug.Log("Unable to parse line " + line);
					}
				} 
			}
			catch(Exception ex) {
				Debug.LogException(ex);
				return kerbals;
			}
			finally {
				if(file!=null) file.Close();
			}
			return kerbals;
		}

		/**
		 * Takes a list of KSKerbals, serializes it to a format readible by LoadKerbals(), and saves it to disk
		 */
		public static void SaveKerbals(List<KSKerbal> kerbals) {
			StreamWriter file = File.CreateText(SAVE_FILE);
			foreach (KSKerbal kerbal in kerbals)
			{
				//Debug.Log("Saving Kerbal "+kerbal.name+" with "+kerbal.baseSanity+" sanity");
				file.WriteLine(kerbal.Serialize());
			}
			file.Close();
		}
	}
}