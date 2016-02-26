﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Contracts;
using KSP;
using KSPAchievements;

//One

namespace MyAsteroidScanner.Contracts.Parameters
{
	public class AsteroidRedirectAltitudeParameters : ContractParameter
	{
		string AsteroidVesselID = null;

		public AsteroidRedirectAltitudeParameters()
		{
			AsteroidVesselID = "";
		}

		public AsteroidRedirectAltitudeParameters(String Asteroid)
		{
			SetAsteroidName(Asteroid);
		}

		public void SetAsteroidName(String Asteroid)
		{
			AsteroidVesselID = Asteroid;
		}

		protected override string GetHashString ()
		{
			return AsteroidVesselID;
		}
		protected override string GetTitle ()
		{
			return "Raise the periapsis above the atmosphere of kerbin";
		}

		protected override void OnRegister ()
		{
			
			//Add function to check if Achivement is there.
			//GameEvents.onVesselOrbitClosed.Add(...);
			//GameEvents.onVesselOrbitEscaped.Add(...);
			//GameEvents.onPartAttach.Add(..);
			GameEvents.onVesselOrbitClosed.Add(OnOrbitChange);
			GameEvents.onVesselOrbitEscaped.Add (OnOrbitChange);
			GameEvents.onVesselChange.Add (OnOrbitChange);

		}
		protected override void OnUnregister ()
		{
			GameEvents.onVesselOrbitClosed.Remove(OnOrbitChange);
			GameEvents.onVesselOrbitEscaped.Remove(OnOrbitChange);
			GameEvents.onVesselChange.Remove (OnOrbitChange);
		}

		protected override void OnLoad (ConfigNode node)
		{
			AsteroidVesselID = node.GetValue ("AsteroidVesselID");
		}
		protected override void OnSave (ConfigNode node)
		{
			node.AddValue ("AsteroidVesselID", AsteroidVesselID);
		}

		private void OnOrbitChange(Vessel ActiveVessel)
		{
			Orbit o = ActiveVessel.orbit;
			while(o.activePatch)
			{
			//@@@TODO: Make Altitude variable. 70k for now
				if (o.referenceBody.name == FlightGlobals.Bodies [1].bodyName)  // Around Kerbin
				{
					if(o.PeA >= 70000 )
					{
						base.SetComplete();
						return;
					}
					if(o.PeA < 70000 )
					{
						base.SetIncomplete();
						return;
					}

				}



				o = o.nextPatch;
		
			}


		}
	}
}

//Two

namespace MyAsteroidScanner.Contracts.Parameters
{
	
	/// <summary>
	/// Asteroid redirect docking parameters.
	/// 
	/// Check if the docked vessel is the rght asteroid
	/// </summary>
	public class AsteroidRedirectDockingParameters : ContractParameter
	{
		string AsteroidName = null;
		string AsteroidPartID = null;
		string AsteroidVesselID = null;



		public AsteroidRedirectDockingParameters()
		{

		}


		public AsteroidRedirectDockingParameters(string Asteroid, string AsteroidVessel)
		{
			SetAsteroidValues(Asteroid,AsteroidVessel);
		}

		public AsteroidRedirectDockingParameters(string Asteroid,string AsteroidVessel ,string AsteroidPart)
		{
			SetAsteroidValues(Asteroid,AsteroidVessel, AsteroidPart);
		}

		public void SetAsteroidValues(string Asteroid, string AsteroidVessel,string AsteroidPart =null )
		{
			AsteroidName = Asteroid;
			AsteroidVesselID = AsteroidVessel;
			AsteroidPartID = AsteroidPart;
		}

		protected override string GetHashString ()
		{
			return AsteroidName;
		}
		protected override string GetTitle ()
		{
			return "Grab Asteroid " + AsteroidName;
		}

		protected override void OnRegister ()
		{

			//Add function to check if Achivement is there.
			GameEvents.onPartCouple.Add (OnDock);
			GameEvents.onVesselChange.Add(OnVesselLoad);

		}
		protected override void OnUnregister ()
		{
			GameEvents.onPartCouple.Remove (OnDock);
		}

		protected override void OnLoad (ConfigNode node)
		{
			AsteroidName = node.GetValue ("AsteroidName");
			AsteroidVesselID = node.GetValue ("AsteroidVesselID");
			AsteroidPartID = node.GetValue ("AsteroidPartID");
		}
		protected override void OnSave (ConfigNode node)
		{
			node.AddValue ("AsteroidName", AsteroidName);
			node.AddValue ("AsteroidVesselID", AsteroidVesselID);
			node.AddValue ("AsteroidPartID", AsteroidPartID);
		}


		private void OnVesselLoad(Vessel v)
		{
			if (AsteroidPartID != null)
			{
				foreach (Part p in  v.parts.FindAll(s => s.partName == "PotatoRoid"))
				{
					if (p.flightID.ToString () == AsteroidPartID)
					{
						base.SetComplete ();
						return;
					}
				}
			} else // 
			{
				if(v.id.ToString() == AsteroidVesselID )
				{
					
					base.SetComplete ();
					return;
				}
					
				
			}
			base.SetIncomplete ();
		}

		private void OnDock(GameEvents.FromToAction<Part, Part> action)
		{
			if (AsteroidPartID != null)
			{
				if (action.from.flightID.ToString () == AsteroidPartID || action.to.flightID.ToString () == AsteroidPartID)
				{
					base.SetComplete ();
			
				}
			}
			if(action.from.vessel.id.ToString () == AsteroidVesselID)
			{
				List <Part> PartList = action.from.vessel.parts.FindAll(s => s.name == "PotatoRoid");
				if(PartList.Count > 0)
				{
					AsteroidPartID =	PartList[0].flightID.ToString ();
				}

				base.SetComplete ();
				return;
			}
			if(action.to.vessel.id.ToString () == AsteroidVesselID)
			{
				List <Part> PartList = action.to.vessel.parts.FindAll(s => s.name == "PotatoRoid");
				if(PartList.Count > 0)
				{
					AsteroidPartID =	PartList[0].flightID.ToString ();
				}

				base.SetComplete ();
				return;
			}
	
		}
	}



}



namespace MyAsteroidScanner.Contracts.Parameters
{

	/// <summary>
	/// Asteroid redirect docking parameters.
	/// 
	/// Check if the docked vessel is the rght asteroid
	/// </summary>
	public class AsteroidDectectedParameters : ContractParameter
	{
		string AsteroidVesselID = null;


		public AsteroidDectectedParameters()
		{
			AsteroidVesselID = null;
			base.SetIncomplete();
		}


		public AsteroidDectectedParameters(String Asteroid)
		{
			SetAsteroidName(Asteroid);
		}

		public void SetAsteroidName(String Asteroid)
		{
			AsteroidVesselID = Asteroid;
			if(AsteroidVesselID != null && AsteroidVesselID.Length > 0)
				base.SetComplete ();
			else
				base.SetIncomplete();
		}

		protected override string GetHashString ()
		{
			return AsteroidVesselID;
		}
		protected override string GetTitle ()
		{
			if (AsteroidVesselID == null)
				return "Detect new asteroid";
			else
				return "Detected Asteroid " + AsteroidVesselID;
		}

		protected override void OnRegister ()
		{
			//Nothing

		}
		protected override void OnLoad (ConfigNode node)
		{
			SetAsteroidName(node.GetValue ("AsteroidVesselID"));

		}
		protected override void OnSave (ConfigNode node)
		{
			node.AddValue ("AsteroidVesselID", AsteroidVesselID);
		}

	}
}




