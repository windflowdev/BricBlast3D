﻿using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using VoxelBusters.Utility;
using VoxelBusters.AssetStoreProductUtility;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	[CustomEditor(typeof(NPSettings))]
	public class NPSettingsInspector : AssetStoreProductInspector
	{
		private enum eTabView
		{
			NONE,
			APPLICATION_SETTINGS,
			BILLING_SETTINGS,
			CLOUD_SERVICES_SETTINGS,
			GAME_SERVICES_SETTINGS,
			MEDIA_LIBRARY_SETTINGS,
			NETWORK_CONNECTVITY_SETTINGS,
			NOTIFICATION_SERVICE_SETTINGS,
			SOCIAL_NETWORK_SETTINGS,
			UTILITY_SETTINGS,
			ADDON_SERVICES_SETTINGS
		}

		#region Constants

		private		const 	string					kActiveView				= "np-active-view";

		// URL
		private		const 	string					kFullVersionProductURL	= "http://bit.ly/1Fnpb5j";
		private		const 	string					kLiteVersionProductURL	= "http://bit.ly/1KFEzdi";
		private		const 	string					kTutorialURL			= "http://bit.ly/1ZFadk4";
		private		const	string					kDocumentationURL		= "http://bit.ly/1cBFHDd";
		private		const	string					kForumURL				= "http://bit.ly/1AjQRYp";
		
		// Keys
		private		const	string					kUndoGroupApplicationSettings	= "application-settings";

		#endregion

		#region Properties

		// Toolbar tabs
		private 			eTabView				m_activeView;
		private				Dictionary<eTabView, SerializedProperty>	m_settingsCollection	= new Dictionary<eTabView, SerializedProperty>();
		private				Vector2					m_scrollPosition		= Vector2.zero;

		// GUI contents
#pragma warning disable
		private 			GUIContent				m_documentationText		= new GUIContent("Documentation", 	"One click access to online documentation.");
		private 			GUIContent				m_saveChangesText		= new GUIContent("Save", 			"Save all your changes.");
		private 			GUIContent				m_forumText				= new GUIContent("Forum - Quick Response", 	"Houston, we have a problem!");
		private 			GUIContent				m_contactText			= new GUIContent("Contact Us", 		"Contact us");
		private 			GUIContent				m_tutotialsText			= new GUIContent("Tutorials", 		"Check our blog posts about product features and usage.");
		private 			GUIContent				m_writeReviewText		= new GUIContent("Write a review", 	"Write a review to share your experience with others.");
		private 			GUIContent				m_upgradeText			= new GUIContent("Upgrade", 		"Click to find out more about full version product.");
#pragma warning restore

		#endregion

		#region Methods

		private void OnInspectorUpdate () 
		{
			// Call Repaint on OnInspectorUpdate as it repaints the windows
			// less times as if it was OnGUI/Update
			Repaint();
		}

		protected override void OnEnable ()
		{
			base.OnEnable();

			// Initialise 
			m_settingsCollection.Add(eTabView.APPLICATION_SETTINGS,			serializedObject.FindProperty("m_applicationSettings"));
			m_settingsCollection.Add(eTabView.BILLING_SETTINGS,				serializedObject.FindProperty("m_billingSettings"));
			m_settingsCollection.Add(eTabView.CLOUD_SERVICES_SETTINGS,		serializedObject.FindProperty("m_cloudServicesSettings"));
			m_settingsCollection.Add(eTabView.MEDIA_LIBRARY_SETTINGS,		serializedObject.FindProperty("m_mediaLibrarySettings"));
			m_settingsCollection.Add(eTabView.GAME_SERVICES_SETTINGS,		serializedObject.FindProperty("m_gameServicesSettings"));
			m_settingsCollection.Add(eTabView.NETWORK_CONNECTVITY_SETTINGS,	serializedObject.FindProperty("m_networkConnectivitySettings"));
			m_settingsCollection.Add(eTabView.NOTIFICATION_SERVICE_SETTINGS,serializedObject.FindProperty("m_notificationSettings"));
			m_settingsCollection.Add(eTabView.SOCIAL_NETWORK_SETTINGS,		serializedObject.FindProperty("m_socialNetworkSettings"));
			m_settingsCollection.Add(eTabView.UTILITY_SETTINGS,				serializedObject.FindProperty("m_utilitySettings"));
			m_settingsCollection.Add(eTabView.ADDON_SERVICES_SETTINGS,		serializedObject.FindProperty("m_addonServicesSettings"));

			// Restoring last selection
			m_activeView	= (eTabView)EditorPrefs.GetInt(kActiveView, 0);
		}

		protected override void OnDisable ()
		{
			base.OnDisable();

			// Save changes to settings
			EditorPrefs.SetInt(kActiveView, (int)m_activeView);	
		}

		protected override void OnGUIWindow ()
		{
			// Disable GUI when its compiling
			GUI.enabled			= !EditorApplication.isCompiling;

			// Drawing tabs
			GUILayout.BeginVertical(UnityEditorUtility.kOuterContainerStyle);
			{	
				base.OnGUIWindow();

				UnityEditorUtility.DrawSplitter(new Color(0.35f, 0.35f, 0.35f), 1, 10);
				DrawTopBarButtons();

				GUILayout.Space(10f);
				GUILayout.BeginVertical(UnityEditorUtility.kOuterContainerStyle);
				{
					GUILayout.Space(2f);
					m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition);
					{
						DrawTabViews();
					}
					GUILayout.EndScrollView();
					GUILayout.Space(2f);
				}
				GUILayout.EndVertical();

				GUILayout.Space(10f);
				GUILayout.BeginHorizontal();
				{
					GUILayout.FlexibleSpace();

					// Change button color, as a feedback to user activity
					Color _GUIColorOld 	= GUI.color;
					GUI.color			= EditorPrefs.GetBool(NPSettings.kPrefsKeyPropertyModified) ? Color.red : Color.green;

					if (GUILayout.Button(m_saveChangesText, GUILayout.MinWidth(120)))
						OnPressingSave();

					// Reset back to old state
					GUI.color 			= _GUIColorOld;

					GUILayout.FlexibleSpace();
				}
				GUILayout.EndHorizontal();
				GUILayout.FlexibleSpace();
			}
			GUILayout.EndVertical();

			// Reset GUI state
			GUI.enabled			= true;
		}

		#endregion

		#region Misc. Methods

		private void DrawTopBarButtons ()
		{
			GUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();

				if (GUILayout.Button(m_documentationText, Constants.kButtonMidStyle))
					Application.OpenURL(kDocumentationURL);
				
				if (GUILayout.Button(m_tutotialsText, Constants.kButtonMidStyle))
					Application.OpenURL(kTutorialURL);

				if (GUILayout.Button(m_forumText, Constants.kButtonMidStyle))
					Application.OpenURL(kForumURL);
				
				if (GUILayout.Button(m_contactText, Constants.kButtonMidStyle))
					OnPressingContact();
				
#if NATIVE_PLUGINS_LITE_VERSION
				if (GUILayout.Button(m_upgradeText, Constants.kButtonMidStyle))
					Application.OpenURL(kFullVersionProductURL);
#endif
				
				if (GUILayout.Button(m_writeReviewText, Constants.kButtonRightStyle))
					OnPressingWriteReview();
				
				GUILayout.Space(10f);
			}
			GUILayout.EndHorizontal();
		}

		private void DrawTabViews ()
		{
			// Draw settings tab view
			Dictionary<eTabView, SerializedProperty>.Enumerator _enumerator	= m_settingsCollection.GetEnumerator();
			
			while (_enumerator.MoveNext())
			{
				eTabView			_curTabView		= _enumerator.Current.Key;
				SerializedProperty	_curProperty	= _enumerator.Current.Value;
				
				if (DrawSerializedProperty(_curProperty))
				{
					// Minimize old selection
					if (m_activeView != eTabView.NONE)
					{
						SerializedProperty _curActiveProperty	= m_settingsCollection[m_activeView];

						if (_curActiveProperty != null)
							_curActiveProperty.isExpanded		= false;
					}
					
					// Update current active view
					if (_curProperty.isExpanded)
						m_activeView	= _curTabView;
					else
						m_activeView	= eTabView.NONE;
				}
			}
		}

		private bool DrawSerializedProperty (SerializedProperty _property)
		{
			if (_property == null || !_property.hasVisibleChildren)
				return false;

			// Draw header
			bool	_isSelected		= UnityEditorUtility.DrawPropertyHeader(_property);

			// Draw childrens
			if (_property.hasVisibleChildren && _property.isExpanded)
			{
				GUILayout.Space(-4f);
				GUILayout.BeginHorizontal("HelpBox");
				{
					GUILayout.Space(8f);
					GUILayout.BeginVertical();
					{
						SerializedProperty	_propertyCopy	= _property.Copy();
						SerializedProperty 	_endProperty	= _property.GetEndProperty();

						// Move pointer to first child and start drawing
						_propertyCopy.NextVisible(true);
					
						do
						{
							if (SerializedProperty.EqualContents(_propertyCopy, _endProperty))
								break;

							_propertyCopy.isExpanded	= true;
							
							EditorGUILayout.PropertyField(_propertyCopy, true);
						} while (_propertyCopy.NextVisible(false));
					}
					GUILayout.EndVertical();
				}
				GUILayout.EndHorizontal();
			}

			return _isSelected;
		}

		private void OnPressingSave ()
		{
			// Save changes
			(target as NPSettings).SaveConfigurationChanges();
		}

		private void OnPressingContact ()
		{
			string	_mailToAddress	= WWW.EscapeURL("support+cross_platform_native_plugins@voxelbusters.com").Replace("+","%20");
			
			string	_mailToString	= string.Format("mailto:{0}", _mailToAddress);
			
			// Opens mail client
			Application.OpenURL(_mailToString);
		}

		private void OnPressingWriteReview ()
		{
			string	_assetPageURL	= null;

#if NATIVE_PLUGINS_LITE_VERSION
			_assetPageURL			= kLiteVersionProductURL;
#else
			_assetPageURL			= kFullVersionProductURL;
#endif

			Application.OpenURL(_assetPageURL);
		}
			   
		#endregion
	}
}
#endif