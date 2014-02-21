using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbalStress 
{
	/**
	 * Author: Nereid
	 */
	static class Utils
	{
		public static int ConvertKerbinDaysToSeconds(int days)
		{
			return days*60*60*24;
		}

		public static String GetRootPath()
		{
			String path = KSPUtil.ApplicationRootPath;
			path = path.Replace("\\", "/");
			if (path.EndsWith("/")) path = path.Substring(0, path.Length - 1);
			//
			return path;
		}

		public static String ConvertToKerbinDuration(double ut)
		{
			double hours = ut / 60.0 / 60.0;
			double kHours = Math.Floor(hours % 24.0);
			double kMinutes = Math.Floor((ut / 60.0) % 60.0);
			double kSeconds = Math.Floor(ut % 60.0);


			double kYears = Math.Floor(hours / 2556.5402); // Kerbin year is 2556.5402 hours
			double kDays = Math.Floor(hours % 2556.5402 / 24.0);
			return ((kYears > 0) ? (kYears.ToString() + " Years ") : "")
				+ ((kDays > 0) ? (kDays.ToString() + " Days ") : "")
				+ ((kHours > 0) ? (kHours.ToString() + " Hours ") : "")
				+ ((kMinutes > 0) ? (kMinutes.ToString() + " Minutes ") : "")
				+ ((kSeconds > 0) ? (kSeconds.ToString() + " Seconds ") : "");
		}


		public static String ConvertToKerbinTime(double ut)
		{
			double hours = ut / 60.0 / 60.0;
			double kHours = Math.Floor(hours % 6.0);
			double kMinutes = Math.Floor((ut / 60.0) % 60.0);
			double kSeconds = Math.Floor(ut % 60.0);


			double kYears = Math.Floor(hours / 2556.5402) + 1; // Kerbin year is 2556.5402 hours
			double kDays = Math.Floor(hours % 2556.5402 / 6.0) + 1;

			return "Year " + kYears.ToString() + ", Day " + kDays.ToString() + " " + " " + kHours.ToString("00") + ":" + kMinutes.ToString("00") + ":" + kSeconds.ToString("00");
		}

		public static String ConvertToEarthTime(double ut)
		{
			double hours = ut / 60.0 / 60.0;
			double eHours = Math.Floor(hours % 24.0);
			double eMinutes = Math.Floor((ut / 60.0) % 60.0);
			double eSeconds = Math.Floor(ut % 60.0);


			double eYears = Math.Floor(hours / (365*24)) + 1; 
			double eDays = Math.Floor(hours % (365*24) / 24.0) + 1;

			return "Year " + eYears.ToString() + ", Day " + eDays.ToString() + " " + " " + eHours.ToString("00") + ":" + eMinutes.ToString("00") + ":" + eSeconds.ToString("00");
		}
	}
}