using UnityEngine;
using System.Collections;

using UniRx;

namespace Synergy88 {
		
	public class BuildRoot : SignalComponent {

		private void Start() {
            Publish(new LoadSplashSignal());
		}

	}

}