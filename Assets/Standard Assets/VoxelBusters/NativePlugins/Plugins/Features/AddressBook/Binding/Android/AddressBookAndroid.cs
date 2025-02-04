﻿using UnityEngine;
using System.Collections;
using VoxelBusters.DebugPRO;
using VoxelBusters.Utility;

#if UNITY_ANDROID
namespace VoxelBusters.NativePlugins
{
	using Internal;

	public partial class AddressBookAndroid : AddressBook 
	{
		
		#region Constructors
		
		AddressBookAndroid()
		{
			Plugin = AndroidPluginUtility.GetSingletonInstance(Native.Class.NAME);
		}
		
		#endregion
		
		#region Overriden API's

		public override eABAuthorizationStatus GetAuthorizationStatus ()
		{
			bool _accessGranted = Plugin.Call<bool>(Native.Methods.IS_AUTHORIZED);

			if(_accessGranted)
			{
				return eABAuthorizationStatus.AUTHORIZED;
			}
			else
			{
				return eABAuthorizationStatus.DENIED;
			}
		}
		
		protected override void ReadContacts (eABAuthorizationStatus _status, ReadContactsCompletion _onCompletion)
		{
			base.ReadContacts(_status, _onCompletion);

			if (_status != eABAuthorizationStatus.AUTHORIZED)
				return;

			// Native method is called
			Plugin.Call(Native.Methods.READ_CONTACTS);
		}
		
		#endregion
	}
}
#endif