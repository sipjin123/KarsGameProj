using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using uPromise;

using UniRx;

using Common;
using Common.Extensions;
using Common.Query;
using Common.Signal;

// alias
using CColor = Common.Extensions.Color;
using UScene = UnityEngine.SceneManagement.Scene;

namespace Synergy88
{

    public delegate void ProcessDelegate();

    /// <summary>
    /// This is the base MVP Presenter class to be extended by each scene root.
    /// </summary>
    public partial class Scene : MonoBehaviour
    {
        private static readonly string ERROR = CColor.red.LogHeader("[ERROR]");
        private static readonly string WARNING = CColor.yellow.LogHeader("[WARNING]");
        
        private static Dictionary<EScene, GameObject> CachedScenes = new Dictionary<EScene, GameObject>();
        
        /// <summary>
        /// Do not edit! cached values for Editor
        /// </summary>
        [SerializeField, HideInInspector]
        private string _SceneTypeString = string.Empty;
        public string SceneTypeString
        {
            get
            {
                return _SceneTypeString;
            }
            set
            {
                Debug.LogWarningFormat(WARNING + " Scene::SceneTypeString Only the SceneEditor.cs is allowed to call this method!\n");
                _SceneTypeString = value;
            }
        }

        /// <summary>
        /// Do not edit! cahced values for Editor
        /// </summary>
        [SerializeField, HideInInspector]
        private string _SceneDepthString = string.Empty;
        public string SceneDepthString
        {
            get
            {
                return _SceneDepthString;
            }
            set
            {
                Debug.LogWarningFormat(WARNING + " Scene::SceneDepthString Only the SceneEditor.cs is allowed to call this method!\n");
                _SceneDepthString = value;
            }
        }

        /// <summary>
        /// The type/ID of the scene this root is for.
        /// This should match the scene's name.
        /// </summary>
        private EScene _SceneType;
        public EScene SceneType
        {
            get
            {
                return _SceneType;
            }
            protected set
            {
                _SceneType = value;
            }
        }
        
        /// <summary>
        /// The scene's layer when shown with other scenes.
        /// </summary>
        private ESceneDepth _SceneDepth;
        public ESceneDepth SceneDepth
        {
            get
            {
                return _SceneDepth;
            }
            protected set
            {
                _SceneDepth = value;
            }
        }

        /// <summary>
        /// Data container passed upon loading this scene.
        /// Note: 
        ///     When there is no data passed, this value is set to null.
        ///     Access this only after Awake and OnEnable.
        /// </summary>
        private ISceneData _SceneData = null;
        public ISceneData SceneData
        {
            get
            {
                return _SceneData;
            }
            private set
            {
                _SceneData = value;
            }
        }

        /// <summary>
        /// Data container passed upon loading this scene.
        /// Note: 
        ///     When there is no data passed, this value is set to null.
        ///     Access this only after Awake and OnEnable.
        /// </summary>
        public T GetSceneData<T>() where T : ISceneData
        {
            return (T)SceneData;
        }

        /// <summary>
        /// Returns the name of the GameObject where the presenter is attached.
        /// </summary>
        public string Name
        {
            get
            {
                return gameObject.name;
            }
        }

        /// <summary>
        /// Persistent scenes indicates that they are exempted from UnloadScenes.
        /// Developers must manually unload the scene
        /// </summary>
        [SerializeField]
        private bool _IsPersistent = false;
        public bool IsPersistent
        {
            get
            {
                return _IsPersistent;
            }
            private set
            {
                _IsPersistent = value;
            }
        }

        /// <summary>
        /// The list of canvases in this scene.
        /// </summary>
        [SerializeField]
        protected List<Canvas> CanvasList;

        /// <summary>
        /// Mapping of button types and click handlers.
        /// </summary>
        protected Dictionary<EButtonType, Action<ButtonClickedSignal>> ButtonMap;

        /// <summary>
        /// Holder for subscriptions to be disposed when this Scene is disabled.
        /// </summary>
        protected CompositeDisposable OnDisableDisposables = new CompositeDisposable();

        #region Unity Life Cycle

        protected virtual void Awake()
        {
            // Update Scene Type & Depth from Editor
            SceneType = SceneTypeString.ToEnum<EScene>();
            SceneDepth = SceneDepthString.ToEnum<ESceneDepth>();

            // Update canvas settings
            SetupSceneCanvas();

            // Initialize button map
            ButtonMap = new Dictionary<EButtonType, Action<ButtonClickedSignal>>();

            // Cache the Root scene object
            CachedScenes[SceneType] = this.gameObject;
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnEnable()
        {
            this.Receive<ButtonClickedSignal>()
                .Subscribe(sig => OnClickedButton(sig))
                .AddTo(OnDisableDisposables);
        }

        protected virtual void OnDisable()
        {
            // dispose all subscriptions and clear list
            OnDisableDisposables.Clear();
        }

        protected virtual void OnDestroy()
        {
            if (ButtonMap != null)
            {
                ButtonMap.Clear();
                ButtonMap = null;
            }

            CachedScenes[SceneType] = null;
            CachedScenes.Remove(SceneType);
        }

        #endregion

        /// <summary>
        /// Intializes the scene's canvases to use the common UI camera.
        /// </summary>
        protected virtual void SetupSceneCanvas()
        {
            Camera camera = QuerySystem.Query<Camera>(QueryIds.SystemCamera);

            CanvasList.ForEach(canvas => {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                //canvas.planeDistance = SceneDepth.ToInt();
                canvas.sortingOrder = SceneDepth.ToInt();
                canvas.worldCamera = camera;
            });
        }
        
        /// <summary>
        /// Sets the scene's handler for the given button type.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="action"></param>
        protected void AddButtonHandler(EButtonType button, Action<ButtonClickedSignal> action)
        {
            ButtonMap[button] = action;
        }

        /// <summary>
        /// Returns true if this scene has data.
        /// </summary>
        /// <returns></returns>
        protected bool HasSceneData()
        {
            return SceneData != null;
        }

        #region Load Scene Promise

        private void PassDataToScene<T>(string scene, ISceneData data) where T : Scene
        {
            UScene loadedScene = SceneManager.GetSceneByName(scene);
            GameObject[] objects = loadedScene.GetRootGameObjects();

            Assertion.Assert(objects.Length > 0);
            Assertion.Assert(objects[0] is GameObject);
            Assertion.Assert(objects[0].GetComponent<T>() != null);

            // pass the data 
            objects[0].GetComponent<T>().SceneData = data;
        }

        public IEnumerator LoadSceneAsync<T>(Deferred deffered, string scene) where T : Scene
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
            yield return operation;
            deffered.Resolve();
        }

        public IEnumerator LoadSceneAsync<T>(Deferred deffered, string scene, ISceneData data) where T : Scene
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
            yield return operation;

            PassDataToScene<T>(scene, data);
            deffered.Resolve();
        }

        /// <summary>
        /// Unloads everything except the SystemRoot then loads the target scene.
        /// </summary>
        public IEnumerator LoadSceneAsync<T>(Deferred deffered, EScene scene) where T : Scene
        {
            Assertion.Assert(scene != EScene.System);

            // Unload all other scenes except flagged as persistent
            UnloadScenes();
            
            //AsyncOperation operation = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Single);
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
            yield return operation;
            deffered.Resolve();

            this.Publish(new OnLoadSceneSignal() { SceneName = scene });
        }

        public IEnumerator LoadSceneAsync<T>(Deferred deffered, EScene scene, ISceneData data) where T : Scene
        {
            Assertion.Assert(scene != EScene.System);

            // Unload all other scenes except flagged as persistent
            UnloadScenes();

            //AsyncOperation operation = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Single);
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
            yield return operation;

            PassDataToScene<T>(scene.ToString(), data);
            deffered.Resolve();

            this.Publish(new OnLoadSceneSignal() { SceneName = scene });
        }

        public IEnumerator LoadAdditiveSceneAsync(Deferred deffered, string scene)
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
            yield return operation;
            deffered.Resolve();
        }
        
        public IEnumerator LoadAdditiveSceneAsync<T>(Deferred deffered, EScene scene) where T : Scene
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
            yield return operation;
            deffered.Resolve();

            this.Publish(new OnLoadSceneSignal() { SceneName = scene });
        }

        public IEnumerator LoadAdditiveSceneAsync<T>(Deferred deffered, EScene scene, ISceneData data) where T : Scene
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
            yield return operation;

            PassDataToScene<T>(scene.ToString(), data);
            deffered.Resolve();

            this.Publish(new OnLoadSceneSignal() { SceneName = scene });
        }

        #endregion

        #region Signals

        private void OnClickedButton(ButtonClickedSignal signal)
        {
            EButtonType button = signal.ButtonType;

            if (ButtonMap.ContainsKey(button) && gameObject.activeSelf)
            {
                Debug.LogFormat("Scene::OnClickedButton Button:{0}\n", button);
                ButtonMap[button](signal);
            }
        }

        #endregion

        #region Helpers
        
        public static bool ShowScene<T>(EScene scene) where T : Scene
        {
            if (CachedScenes.ContainsKey(scene))
            {
                CachedScenes[scene].gameObject.SetActive(true);
                return true;
            }

            return false;
        }

        public static bool HasScene<T>(EScene scene) where T : Scene
        {
            return CachedScenes.ContainsKey(scene);
        }

        public static T GetScene<T>(EScene scene) where T : Scene
        {
            if (!HasScene<T>(scene))
            {
                return default(T);
            }

            return CachedScenes[scene].GetComponent<T>();
        }

        public static void UnloadScene(EScene scene)
        {
            Debug.LogFormat("[SYNERGY88] Scene::UnloadScene Scene:{0} Cached:{1} Loaded:{2}\n", scene, CachedScenes.ContainsKey(scene), SceneManager.GetSceneByName(scene.ToString()));

            // unload if cached
            if (CachedScenes.ContainsKey(scene))
            {
                GameObject.Destroy(CachedScenes[scene].gameObject);
                CachedScenes.Remove(scene);
            }
            
            // unload if loaded
            if (SceneManager.GetSceneByName(scene.ToString()) != null)
            {
                SceneManager.UnloadScene(scene.ToString());
            }
        }

        /// <summary>
        /// Unloads every scene except for the scenes that marked as Persistent.
        /// </summary> 
        public static void UnloadScenes()
        {
            int sceneCount = SceneManager.sceneCount;
            UScene[] scenes = new UScene[sceneCount];

            for (int i = 0; i < sceneCount; i++)
            {
                scenes[i] = SceneManager.GetSceneAt(i);
            }

            foreach (UScene scene in scenes)
            {
                EScene loadScene = EScene.Invalid;

                try
                {
                    loadScene = scene.name.ToEnum<EScene>();
                }
                // Catch the loaded non Synergy Scenes
                catch (ArgumentException)
                {
                    loadScene = EScene.Invalid;
                    AsyncOperation async = SceneManager.UnloadSceneAsync(scene);
                }
                finally
                {
                    if (loadScene != EScene.Invalid && !GetScene<Scene>(loadScene).IsPersistent)
                    {
                        UnloadScene(loadScene);
                    }
                }
            }

            scenes = null;
        }

        public static bool IsLoaded(EScene scene)
        {
            //return CachedScenes.ContainsKey(scene);
            return SceneManager.GetSceneByName(scene.ToString()) != null;
        }

        #endregion

    }

    #region Partial Scene for Coroutines

    public partial class Scene : MonoBehaviour
    {
        public Promise EndFramePromise()
        {
            Deferred deferred = new Deferred();
            this.StartCoroutine(this.EndFrame(deferred));
            return deferred.Promise;
        }

        public Promise WaitPromise(float seconds = 1.0f)
        {
            Deferred deferred = new Deferred();
            this.StartCoroutine(this.Wait(deferred, seconds));
            return deferred.Promise;
        }

        protected IEnumerator EndFrame(Deferred deferred)
        {
            yield return null;
            deferred.Resolve();
        }

        protected IEnumerator Wait(Deferred deferred, float seconds = 1.0f)
        {
            yield return null;
            yield return new WaitForSeconds(seconds);
            deferred.Resolve();
        }
    }

    #endregion
    
}

