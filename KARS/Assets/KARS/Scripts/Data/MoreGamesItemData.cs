using System;
using System.Collections;
using System.Collections.Generic;

// alias
using Const = Synergy88.Const;

namespace Synergy88 {

	[Serializable]
	public class MoreGamesItemData : IItemData {
		
		public string ItemId;
		public string Name;
		public string Description;
		public string Avatar;
		public string Link;

		public void Parse(Dictionary<string, object> rawData) {
			this.ItemId = (string)rawData[Const.MORE_GAMES_ITEM_ID];
			this.Name = (string)rawData[Const.MORE_GAMES_ITEM_NAME];
			this.Description = (string)rawData[Const.MORE_GAMES_ITEM_DESCRIPTION];
			this.Avatar = (string)rawData[Const.MORE_GAMES_ITEM_AVATAR];
			this.Avatar = Const.GetMoreGamesAvatarUrl(this.Avatar);
			this.Link = (string)rawData[Const.MORE_GAMES_ITEM_LINK];
		}
	}

}