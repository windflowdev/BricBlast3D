using UnityEngine;
using System.Collections;

public class GGoogleAnalyticsSetting : MonoBehaviour
{
	public Object GAv4;

	public void Awake()
	{
		GGoogleAnalyticsManager.Inst.GAv4 = GAv4;
	}
}

