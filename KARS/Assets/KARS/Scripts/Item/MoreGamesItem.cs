using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

using Common.Signal;

// alias
using Const = Synergy88.Const;

using UniRx;

namespace Synergy88
{
	public class MoreGamesItem : ItemView
    {
		[SerializeField]
		private Image body;

		[SerializeField]
		private RawImage avatar;

		[SerializeField]
		private Text title;

		[SerializeField]
		private Text description;

		private int index;

		private void Awake()
        {
			Assertion.AssertNotNull(this.body);
			Assertion.AssertNotNull(this.avatar);
			Assertion.AssertNotNull(this.title);
			Assertion.AssertNotNull(this.description);

            this.Receive<ImageDownloadedSignal>()
                .Subscribe(sig => OnImageDownloaded(sig))
                .AddTo(this);
		}

        /// <summary>
        /// Setup item view based on given data.
        /// </summary>
        /// <param name="index">Item index</param>
        /// <param name="data">More games item data</param>
		public void SetupView(int index, MoreGamesItemData data)
        {
			this.index = index;
            ItemId = data.ItemId;

            if (this.index % 2 == 0)
            {
                this.body.color = Color.gray;
            }
            else
            {
                this.body.color = Color.gray * 1.5f;
            }

            // update labels
            this.title.text = data.Name;
            this.description.text = data.Description;

            // download image
            Publish(new DownloadImageSignal()
            {
                ImageId = data.ItemId,
                Url = data.Avatar
            });
        }

		#region Signals

		private void OnImageDownloaded(ImageDownloadedSignal signal)
        {
			string imageId = signal.ImageId;
			Texture2D imageTexture = signal.Texture;
			if (ItemId.Equals(imageId))
            {
				this.avatar.texture = imageTexture;
			}
		}

		#endregion
	}
}