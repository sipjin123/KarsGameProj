using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using uPromise;

using UniRx;

using Common;
using Common.Extensions;
using Common.Query;
using Common.Signal;
using Common.Utils;

// alias
using UColor = UnityEngine.Color;
using URandom = UnityEngine.Random;
using UScene = UnityEngine.SceneManagement.Scene;
using CColor = Common.Extensions.Color;
using CSScene = Synergy88.Scene;

namespace Synergy88
{
    /// <summary>
    /// Enum of loading screen IDs.
    /// </summary>
    [Flags]
    public enum LoadingImages
    {
        Invalid = -1,

        Loading001,
        Loading002,
        Loading003,
        Loading004,

        Max,
    };

    /// <summary>
    /// This is an interface to be implemented by preloaders that can fade in and fade out.
    /// </summary>
    public interface IPreloader
    {
        /// <summary>
        /// Loads/initializes the preloader.
        /// </summary>
        /// <returns>Promise</returns>
        Promise LoadPromise();
        /// <summary>
        /// Fades in the preloader.
        /// </summary>
        /// <returns></returns>
        Promise FadeInPromise();
        /// <summary>
        /// Fades out the preloader.
        /// </summary>
        /// <returns></returns>
        Promise FadeOutPromise();
    };

    /// <summary>
    /// Scene loading transision helper class.
    /// Touch input blocker.
    /// To use, sequence the following promises:
    /// 1) LoadLoadingScreen()
    /// 2) FadeInLoadingScreen()
    /// 3) ... load additive scenes to be loaded
    /// 4) FadeOutLoadingScreen().
    /// </summary>
    public class PreloaderRoot : Scene
    {
        public const float IN_DURATION = 0.5f;
        public const float OUT_DURATION = 0.5f;
        public const float FIXED_DELTA = 0.01656668f;

        public static readonly Vector2 TARGET_RESOLUTION = new Vector2(1536.0f, 2048.0f);

        /// <summary>
        /// True while the loading screen is being loaded or active.
        /// </summary>
        [SerializeField]
        private bool _IsLoading;
        public bool IsLoading
        {
            get
            {
                return _IsLoading;
            }
            private set
            {
                _IsLoading = value;
            }
        }
        
        /// <summary>
        /// This swallows all the touch/mouse input when enabled
        /// </summary>
        [SerializeField]
        private Image _Blocker;
        public Image Blocker
        {
            get
            {
                return _Blocker;
            }
        }

        /// <summary>
        /// This parents loaded loading screens.
        /// </summary>
        [SerializeField]
        private Transform _Container;
        public Transform Container
        {
            get
            {
                return _Container;
            }
        }

        /// <summary>
        /// The currently loaded loading screen.
        /// </summary>
        [SerializeField]
        private Image LoadedImage;

        #region Unity Life Cycle

        protected override void Awake()
        {
            // force set scene type and depth
            SceneType = EScene.Preloader;
            SceneDepth = ESceneDepth.Overlay;

            Assertion.Assert(Blocker, string.Format(CColor.red.LogHeader("[ERROR]") + " PreloaderRoot::Awake Blocker:{0} is null!\n", Blocker));
            Assertion.Assert(Container, string.Format(CColor.red.LogHeader("[ERROR]") + " PreloaderRoot::Awake Container:{0} is null!\n", Container));

            base.Awake();
        }

        #endregion

        /// <summary>
        /// Sets the image's alpha.
        /// </summary>
        private UColor SetImageAlpha
        {
            set
            {
                UColor color = LoadedImage.color;
                color.a = value.a;
                LoadedImage.color = color;
            }
        }

        #region Preloader Promise

        /// <summary>
        /// Loads the preloader by enabling the blocker and loading the loading screen to be displayed.
        /// </summary>
        /// <returns></returns>
        public Promise LoadLoadingScreenPromise()
        {
            IsLoading = true;
            Blocker.gameObject.SetActive(true);

            Deferred deferred = new Deferred();
            StartCoroutine(LoadLoadingScreen(deferred));
            return deferred.Promise;
        }

        /// <summary>
        /// Fades in the loading screen.
        /// </summary>
        /// <returns></returns>
        public Promise FadeInLoadingScreenPromise()
        {
            Deferred deferred = new Deferred();
            StartCoroutine(FadeInLoadingScreen(deferred));
            return deferred.Promise;
        }
        
        /// <summary>
        /// Fades out the loading screen.
        /// </summary>
        /// <returns></returns>
        public Promise FadeOutLoadingScreenPromise()
        {
            Deferred deferred = new Deferred();
            StartCoroutine(FadeOutLoadingScreen(deferred));
            return deferred.Promise;
        }
        
        #endregion

        #region Coroutines

        /// <summary>
        /// Loads a random loading screen.
        /// </summary>
        /// <param name="deffered"></param>
        /// <returns></returns>
        private IEnumerator LoadLoadingScreen(Deferred deffered)
        {
            yield return null;

            // pick random loading screen
            string imageScene = URandom.Range(LoadingImages.Invalid.ToInt() + 1, LoadingImages.Max.ToInt()).ToEnum<LoadingImages>().ToString();

            // load scene with loading screen
            AsyncOperation operation = SceneManager.LoadSceneAsync(imageScene, LoadSceneMode.Additive);
            yield return operation;

            // get objects in the scene
            UScene loadedScene = SceneManager.GetSceneByName(imageScene);
            GameObject[] objects = loadedScene.GetRootGameObjects();

            // make sure the scenes only has 1 root object
            Assertion.Assert(objects.Length == 1);

            // get the first and only object (which should be the loading screen image)
            GameObject obj = objects[0];

            // fix object parenting setup
            Transform root = Container;
            obj.transform.SetParent(root);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;

            // update loading screen size
            RectTransform parent = obj.transform.parent.GetComponent<RectTransform>();
            obj.transform.GetComponent<Image>().SetNativeSize();
            obj.transform.GetComponent<RectTransform>().sizeDelta = TARGET_RESOLUTION;

            obj.SetActive(true);

            // set loaded image
            LoadedImage = obj.GetComponent<Image>();
            LoadedImage.gameObject.SetActive(true);
            
            SceneManager.UnloadScene(imageScene);

            deffered.Resolve();
        }
        
        /// <summary>
        /// Fades in the loading screen.
        /// </summary>
        /// <param name="deferred"></param>
        /// <returns></returns>
        private IEnumerator FadeInLoadingScreen(Deferred deferred)
        {
            yield return null;
            
            float timer = 0.0f;
            UColor color = UColor.white;
            Deferred def = deferred;

            while (timer <= IN_DURATION)
            {
                float scale = Mathf.Clamp((timer += FIXED_DELTA), 0.0f, IN_DURATION) / IN_DURATION;
                color.a = scale;
                SetImageAlpha = color;
                yield return null;
            }

            color.a = 1.0f;
            SetImageAlpha = color;
            yield return null;
            
            def.Resolve();
        }

        /// <summary>
        /// Fades out the loading screen and disables the blocker.
        /// </summary>
        /// <param name="deferred"></param>
        /// <returns></returns>
        private IEnumerator FadeOutLoadingScreen(Deferred deferred)
        {
            yield return null;
            
            float timer = 0.0f;
            UColor color = UColor.white;
            Deferred def = deferred;

            while (timer <= OUT_DURATION)
            {
                float scale = 1.0f - Mathf.Clamp((timer += FIXED_DELTA), 0.0f, OUT_DURATION) / OUT_DURATION;
                color.a = scale;
                SetImageAlpha = color;
                yield return null;
            }

            color.a = 0.0f;
            SetImageAlpha = color;
            yield return null;

            IsLoading = false;
            Blocker.gameObject.SetActive(false);
            Destroy(LoadedImage.gameObject);

            def.Resolve();
        }
        
        #endregion
    }

}