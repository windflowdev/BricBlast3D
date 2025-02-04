﻿using UnityEngine;
using System.Collections;

#if USES_NETWORK_CONNECTIVITY && UNITY_ANDROID
using System.Collections.Generic;
using VoxelBusters.Utility;
using VoxelBusters.DebugPRO;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	public partial class NetworkConnectivityAndroid : NetworkConnectivity 
	{	
		#region Constructors
		
		NetworkConnectivityAndroid()
		{
			Plugin = AndroidPluginUtility.GetSingletonInstance(Native.Class.NAME);
		}
		
		#endregion

		#region API

		public override void Initialise ()
		{
			base.Initialise ();

			NetworkConnectivitySettings _settings = NPSettings.NetworkConnectivity;

			Plugin.Call(Native.Methods.INITIALIZE,_settings.IPAddress, 
			            							_settings.Android.Port, 
													_settings.Android.TimeGapBetweenPolling, 
													_settings.Android.TimeOutPeriod,
													_settings.Android.MaxRetryCount);
			
		}	

		#endregion
	}
}
#endif