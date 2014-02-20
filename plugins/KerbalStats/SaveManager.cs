using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;

namespace KerbalStats
{
	public static class SaveManager
	{
		private static readonly String ROOT_PATH = Utils.GetRootPath();
		private static readonly String SAVE_BASE_FOLDER = ROOT_PATH + "/saves/";
		private static readonly String SAVE_FILE = SAVE_BASE_FOLDER + HighLogic.SaveFolder + "/kstats.sav";

		public static List<KSKerbal> LoadKerbals() {
			Debug.Log("Loading Kerbals from disk");
			List<KSKerbal> kerbals = new List<KSKerbal>();
			XmlSerializer xmlSerializer = new XmlSerializer(Type.GetType("KerbalStats.KSKerbal"));
			StreamReader file = null;
			try {
				if(!File.Exists(SAVE_FILE)) {
					Debug.Log("Unable to find " + SAVE_FILE);
					return kerbals;
				}

				file = File.OpenText(SAVE_FILE);
				String line;
				String section = "";
				while( (line=file.ReadLine()) != null ) {
					if (line.Trim().Length > 0) {
						if(line == "</KSKerbal>") {
							section += line;
							StringReader stream = new StringReader(section);
							KSKerbal kerbal = (KSKerbal) xmlSerializer.Deserialize(stream);
							Debug.Log("Adding "+kerbal.name);
							kerbals.Add(kerbal);
							stream.Close();
							section = "";
						} else {
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

		public static void SaveKerbals(List<KSKerbal> kerbals) {
			StreamWriter file = File.CreateText(SAVE_FILE);
			foreach (KSKerbal kerbal in kerbals)
			{
				Debug.Log("Saving Kerbal "+kerbal.name);
				file.WriteLine(kerbal.Serialize());
			}
			file.Close();
		}
	}
}