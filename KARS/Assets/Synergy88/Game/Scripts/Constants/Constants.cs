using UnityEngine;
using System.Collections;

namespace Synergy88 {
		
	public abstract class Const {
	
		#region MoreGames Constants

		// id address
		//public const string PUBLIC_IP = "54.69.107.250";
		public const string MORE_GAMES_PUBLIC_IP = "54.69.107.250";

		// id
		public const string MORE_GAMES_GAME_ID = "17615";

		// format
		public const string MORE_GAMES_URL_FORMAT = "http://{0}/api/v1/game/more?game_id={1}&tag={2}";

		// platforms
		public const string MORE_GAMES_IOS = "ios";
		public const string MORE_GAMES_ANDROID = "android";

		// headers
		public const string MORE_GAMES_AUTHORIZATION_KEY = "X-Authorization";
		public const string MORE_GAMES_AUTHORIZATION_VALUE = "c29858aa3c61b7d93717431049cdc332a708ed89";
		public const string MORE_GAMES_CONTENT_TYPE_KEY = "Content-Type";
		public const string MORE_GAMES_CONTENT_TYPE_VALUE = "application/json";

		// item keys
		public const string MORE_GAMES_DATA = "data"; // List<MoreGamesItemData>
		public const string MORE_GAMES_ITEM_ID = "id";
		public const string MORE_GAMES_ITEM_NAME = "name";
		public const string MORE_GAMES_ITEM_DESCRIPTION = "description";
		public const string MORE_GAMES_ITEM_AVATAR = "avatar";
		public const string MORE_GAMES_ITEM_LINK = "link";

		#endregion

		#region Google Mobile Ads (Banner)

		// game ids
		//public const string GA_BANNER_GAME_ID = "1017925";
		//public const string GA_BANNER_GAME_ID = "1017925";
		public const string GA_BANNER_GAME_ID = "1017925";

		// banner ids
		public const string GA_BANNER_AD_UNIT_ID_ANDROID = "ca-app-pub-1490909482181755/4368314423";
		public const string GA_BANNER_AD_UNIT_ID_IOS = "ca-app-pub-1490909482181755/2650588828";

		#endregion

		#region Google Mobile Ads (Interstitials)

		// game ids
		public const string GA_INTERSTITIAL_GAME_ID = "";

		// interstitial ids
		public const string GA_INTERSTITIALS_AD_UNIT_ID_ANDROID = "";
		public const string GA_INTERSTITIALS_AD_UNIT_ID_IOS = "";

		#endregion

		#region Unity Ads
		
		//public const string UNITY_GAME_ID = "1021194";
		//public const string UNITY_GAME_ID = "1017925";
		public const string UNITY_GAME_ID_ANDROID = "1035902";
		public const string UNITY_GAME_ID_IOS = "1035902";

		#endregion

		#region Unity Analytics

		// custom events
		public const string UNI_ANA_SCREEN_SPLASH = "ScreenEvent_Splash";
		public const string UNI_ANA_SCREEN_LOGIN = "ScreenEvent_Login";
		public const string UNI_ANA_SCREEN_HOME = "ScreenEvent_Home";
		public const string UNI_ANA_SCREEN_MOREGAMES = "ScreenEvent_MoreGames";
		public const string UNI_ANA_SCREEN_COINS = "ScreenEvent_Coins";
		public const string UNI_ANA_SCREEN_HELP = "ScreenEvent_Help";
		public const string UNI_ANA_SCREEN_LEADERBOARDS = "ScreenEvent_Leaderboards";
		public const string UNI_ANA_SCREEN_SHOP = "ScreenEvent_Shop";
		public const string UNI_ANA_SCREEN_SETTINGS = "ScreenEvent_Settings";
		public const string UNI_ANA_SCREEN_TUTORIAL = "ScreenEvent_Tutorial";

		// button events
		public const string UNI_ANA_BUTTON_LOGIN_GUEST = "ButtonEvent_LoginGuest";
		public const string UNI_ANA_BUTTON_LOGIN_FB = "ButtonEvent_LoginFacebook";
		public const string UNI_ANA_BUTTON_COINS = "ButtonEvent_Coins";
		public const string UNI_ANA_BUTTON_MOREGAMES = "ButtonEvent_MoreGames";
		public const string UNI_ANA_BUTTON_HELP = "ButtonEvent_Help";
		public const string UNI_ANA_BUTTON_LEADERBOARDS = "ButtonEvent_Leaderboards";
		public const string UNI_ANA_BUTTON_HOME = "ButtonEvent_Home";
		public const string UNI_ANA_BUTTON_ADS = "ButtonEvent_Ads";
		public const string UNI_ANA_BUTTON_SHOP = "ButtonEvent_Shop";
		public const string UNI_ANA_BUTTON_PLAY = "ButtonEvent_Play";
		public const string UNI_ANA_BUTTON_SETTINGS = "ButtonEvent_Settings";
		public const string UNI_ANA_BUTTON_CREDITS = "ButtonEvent_Credits";
		public const string UNI_ANA_BUTTON_RESTORE_PURCHASE = "ButtonEvent_RestorePurchase";
		public const string UNI_ANA_BUTTON_TOGGLE_SFX = "ButtonEvent_ToggleSFX";
		public const string UNI_ANA_BUTTON_TOGGLE_BGM = "ButtonEvent_ToggleBGM";

		// purchase
		public const string UNI_ANA_PURCHASE_COINS = "PurchaseEvent_Coins";
		public const string UNI_ANA_PURCHASE_SHOP = "PurchaseEvent_SHOP";

		// purchase event data key
		public const string UNI_ANA_PURCHASE_COINS_ID = "PurchaseEvent_Coins_Id";
		public const string UNI_ANA_PURCHASE_COINS_QUANTITY = "PurchaseEvent_Coins_Quantity";
		public const string UNI_ANA_PURCHASE_SHOP_ID = "PurchaseEvent_Shop_Id";
		public const string UNI_ANA_PURCHASE_SHOP_ID_QUANTITY = "PurchaseEvent_Shop_Quantity";

		// button event moregames data key
		public const string UNI_ANA_BUTTON_MOREGAMES_ID = "ButtonEvent_MoreGames_Id";

		// button event toggle data key
		public const string UNI_ANA_BUTTON_TOGGLE_SFX_STATUS = "ButtonEvent_ToggleSFX_Status";
		public const string UNI_ANA_BUTTON_TOGGLE_BGM_STATUS = "ButtonEvent_ToggleBGM_Status";

		#endregion

		#region Helpers

		public static string GetMoreGamesUrl() {
			// mobile device
			if (Platform.IsMobileAndroid()) {
				return string.Format(MORE_GAMES_URL_FORMAT, MORE_GAMES_PUBLIC_IP, MORE_GAMES_GAME_ID, MORE_GAMES_ANDROID);
			}
			else if (Platform.IsMobileIOS()) {
				return string.Format(MORE_GAMES_URL_FORMAT, MORE_GAMES_PUBLIC_IP, MORE_GAMES_GAME_ID, MORE_GAMES_IOS);
			}

			// editor
			#if UNITY_ANDROID
			return string.Format(MORE_GAMES_URL_FORMAT, MORE_GAMES_PUBLIC_IP, MORE_GAMES_GAME_ID, MORE_GAMES_ANDROID);
			#elif UNITY_IOS
			return string.Format(MORE_GAMES_URL_FORMAT, MORE_GAMES_PUBLIC_IP, MORE_GAMES_GAME_ID, MORE_GAMES_IOS);
			#else
			Assertion.Assert(false, "Unsupported Platform");
			return string.Empty;
			#endif
		}

		public static string GetMoreGamesAvatarUrl(string avatar) {
			return string.Format("http://{0}/{1}", MORE_GAMES_PUBLIC_IP, avatar);
		}

		#endregion
		
	}

}