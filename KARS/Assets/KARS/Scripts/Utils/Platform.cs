using UnityEngine;
using System.Collections;

namespace Synergy88 {
	
	public abstract class Platform {

		public static bool IsEditor() {
			if ((Application.platform == RuntimePlatform.OSXDashboardPlayer)
			||	(Application.platform == RuntimePlatform.OSXEditor)
			||	(Application.platform == RuntimePlatform.OSXPlayer)
			||	(Application.platform == RuntimePlatform.OSXWebPlayer)
			||	(Application.platform == RuntimePlatform.WindowsEditor)
			||	(Application.platform == RuntimePlatform.WindowsPlayer)
			||	(Application.platform == RuntimePlatform.WindowsWebPlayer)
			) {
				//Debug.Log("#### Editor:" + Application.platform);
				return true;
			}

			return false;
		}

		public static bool IsMobileAndroid() {
			if ((Application.platform == RuntimePlatform.Android)) {
				//Debug.Log("#### Android Mobile:" + Application.platform);
				return true;
			}

			return false;
		}

		public static bool IsMobileIOS() {
			if ((Application.platform == RuntimePlatform.IPhonePlayer)) {
				//Debug.Log("#### IOS Mobile:" + Application.platform);
				return true;
			}

			return false;
		}

	}

}