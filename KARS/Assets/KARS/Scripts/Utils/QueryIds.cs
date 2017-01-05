using UnityEngine;
using System.Collections;

namespace Synergy88 { 
	
	public abstract class QueryIds {

        // system
        public const string SystemCamera = "SystemCamera";
        public const string SystemState = "SystemState";
        public const string DevelopmentVersion = "DevelopmentVersion";
        public const string ReleaseVersion = "ReleaseVersion";

        // scene
        public const string CurrentScene = "CurrentScene";
		public const string PreviousScene = "PreviousScene";
        public const string Preloader = "Preloader";
        public const string PopupCollection = "PopupCollection";

        // Facebook Login
        public const string HasLoggedInUser = "HasLoggedInUser";
		public const string UserEmail = "UserEmail";
		public const string UserFacebookId = "UserFacebookId";
        public const string UserFirstName = "UserFirstName";
        public const string UserFullName = "UserFullName";
		public const string UserProfilePhoto = "UserProfilePhoto";
        public const string UserGender = "UserGender";
        public const string UserBirthday = "UserBirthday";

        // IAP
        public const string StoreIsReady = "StoreIsReady";
		public const string StoreItems = "StoreItems";
		public const string StoreItemsWithType = "StoreItemsWithType";
		public const string StoreItemType = "StoreItemType";
		public const string StoreItemId = "StoreItemId";
		public const string PurchaseInProgress = "PurchaseInProgress";

        // More Games
        public const string MoreGamesItems = "MoreGamesItems";
        public const string MoreGamesItemId = "MoreGamesItemId";
    }

}