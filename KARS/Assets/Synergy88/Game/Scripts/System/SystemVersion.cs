using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

using Common;
using Common.Signal;

using Common.Query;

namespace Synergy88 {

    public class SystemVersion : MonoBehaviour {

        [SerializeField]
        private string buildVersion;

        [SerializeField]
        private string releaseVersion;

        [SerializeField]
        private Text labelVersion;

        private void Awake() {
            Debug.LogWarningFormat("[Framework] Local Build:{0} Release:{1}\n", this.buildVersion, this.releaseVersion);

            Assertion.AssertNotNull(this.labelVersion);

            QuerySystem.RegisterResolver(QueryIds.DevelopmentVersion, (IQueryRequest request, IMutableQueryResult result) => {
                result.Set(this.buildVersion);
            });

            QuerySystem.RegisterResolver(QueryIds.ReleaseVersion, (IQueryRequest request, IMutableQueryResult result) => {
                result.Set(this.releaseVersion);
            });

            this.UpdateLabel();
        }

        private void Start() {
            this.UpdateLabel();
        }

        private void OnDestroy() {
            QuerySystem.RemoveResolver(QueryIds.DevelopmentVersion);
            QuerySystem.RemoveResolver(QueryIds.ReleaseVersion);
        }

        public void Hide() {
            //#if DEVELOPMENT_BUILD || UNITY_EDITOR
            //            // Disable Build Version
            //            this.labelVersion.gameObject.SetActive(true);
            //            this.labelVersion.text = this.buildVersion;
            //#else
            this.labelVersion.gameObject.SetActive(false);
            //#endif
        }

        public void UpdateLabel() {
#if DEVELOPMENT_BUILD
            this.labelVersion.gameObject.SetActive(true);
            this.labelVersion.text = this.buildVersion;
#endif

#if UNITY_EDITOR
            Canvas.ForceUpdateCanvases();
#endif
        }
    }

}