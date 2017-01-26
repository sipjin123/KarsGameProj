using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_OSX
using UnityEditor.Callbacks;
using UnityEditor.Advertisements;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using uPromise;

using UniRx;

using Common;
using Common.Extensions;
using Common.Fsm;
using Common.Query;
using Common.Signal;

// alias
using CColor = Common.Extensions.Color;

namespace Synergy88
{

    public class SystemRoot : Scene
    {
        // utils
        private static readonly string LOG = CColor.green.LogHeader("[SYSTEMROOT]");

#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_OSX
        [PostProcessScene]
        public static void OnPostProcessScene()
        {
            AdvertisementSettings.enabled = true;
            AdvertisementSettings.initializeOnStartup = false;
        }
#endif
        
        [SerializeField]
        private EScene _CurrentScene = EScene.Invalid;
        public EScene CurrentScene
        {
            get
            {
                return _CurrentScene;
            }
            private set
            {
                _CurrentScene = value;
            }
        }

        [SerializeField]
        private EScene _PreviousScene = EScene.Invalid;
        public EScene PreviousScene
        {
            get
            {
                return _PreviousScene;
            }
            private set
            {
                _PreviousScene = value;
            }
        }

        [SerializeField]
        private Camera _SystemCamera;
        public Camera SystemCamera
        {
            get
            {
                return _SystemCamera;
            }
            private set
            {
                _SystemCamera = value;
            }
        }

        [SerializeField]
        private SystemVersion _SystemVersion;
        public SystemVersion SystemVersion
        {
            get
            {
                return _SystemVersion;
            }
            private set
            {
                _SystemVersion = value;
            }
        }
        
        private PreloaderRoot Preloader;

        private PopupCollectionRoot PopupCollection;

        #region Unity Life Cycle

        protected override void Awake()
        {
            // Setup Listeners
            this.Receive<OnLoadSceneSignal>()
                .Subscribe(sig => OnLoadScene(sig.SceneName))
                .AddTo(this);

            this.Receive<LoadSplashSignal>()
                .Subscribe(_ => OnLoadSplash())
                .AddTo(this);

            this.Receive<SplashDoneSignal>()
                .Subscribe(_ => OnSplashDone())
                .AddTo(this);

            this.Receive<ServicesReadySignal>()
                .Subscribe(_ => OnServicesReady())
                .AddTo(this);

            this.Receive<LoadHomeSignal>()
                .Subscribe(_ => OnLoadHome())
                .AddTo(this);

            this.Receive<LoadMoreGamesSignal>()
                .Subscribe(_ => OnLoadMoreGames())
                .AddTo(this);

            this.Receive<LoadShopSignal>()
                .Subscribe(_ => OnLoadShop())
                .AddTo(this);

            this.Receive<LoadSettingsSignal>()
                .Subscribe(_ => OnLoadSettings())
                .AddTo(this);

            this.Receive<LoadGameSignal>()
                .Subscribe(_ => OnLoadGame())
                .AddTo(this);
            
            this.Receive<GameEndSignal>()
                .Subscribe(_ => OnEndGame())
                .AddTo(this);

            // Setup DI Queries
            QuerySystem.RegisterResolver(QueryIds.CurrentScene, delegate (IQueryRequest request, IMutableQueryResult result)
            {
                result.Set(CurrentScene);
            });

            QuerySystem.RegisterResolver(QueryIds.PreviousScene, delegate (IQueryRequest request, IMutableQueryResult result)
            {
                result.Set(PreviousScene);
            });

            QuerySystem.RegisterResolver(QueryIds.SystemCamera, delegate (IQueryRequest request, IMutableQueryResult result)
            {
                result.Set(SystemCamera);
            });

            QuerySystem.RegisterResolver(QueryIds.SystemState, delegate (IQueryRequest request, IMutableQueryResult result)
            {
                result.Set(Fsm.GetCurrentState());
            });

            QuerySystem.RegisterResolver(QueryIds.Preloader, delegate (IQueryRequest request, IMutableQueryResult result)
            {
                result.Set(Preloader);
            });

            QuerySystem.RegisterResolver(QueryIds.PopupCollection, delegate (IQueryRequest request, IMutableQueryResult result)
            {
                result.Set(PopupCollection);
            });

            // Assert cache objects
            Assertion.AssertNotNull(SystemCamera);
            Assertion.AssertNotNull(SystemVersion);
            
            base.Awake();

            // prepare Fsm
            PrepareSceneFsm();
        }

        protected override void OnDestroy()
        {
            QuerySystem.RemoveResolver(QueryIds.CurrentScene);
            QuerySystem.RemoveResolver(QueryIds.PreviousScene);
            QuerySystem.RemoveResolver(QueryIds.SystemCamera);
            QuerySystem.RemoveResolver(QueryIds.SystemState);
            QuerySystem.RemoveResolver(QueryIds.Preloader);
            QuerySystem.RemoveResolver(QueryIds.PopupCollection);
        }

        #endregion

        #region Scene Fsm

        // initial state
        private const string IDLE = "Idle";

        // events
        private const string START_SPLASH = "StartSplash";
        private const string START_PRELOAD = "StartPreload";
        private const string START_LOGIN = "StartLogin";
        private const string LOAD_HOME = "LoadHome";
        private const string LOAD_MORE_GAMES = "LOAD_MORE_GAMES";
        private const string LOAD_SHOP = "LOAD_SHOP";
        private const string LOAD_SETTINGS = "LOAD_SETTINGS";
        private const string LOAD_GAME = "LOAD_GAME";
        private const string END_GAME = "END_GAME";
        private const string FINISHED = "Finished";

        private Fsm Fsm;

        private void PrepareSceneFsm()
        {
            Fsm = new Fsm("SceneFsm");

            // states
            FsmState idle = Fsm.AddState(IDLE);
            FsmState splash = Fsm.AddState("splash");
            FsmState preload = Fsm.AddState("preload");
            FsmState login = Fsm.AddState("login");
            FsmState home = Fsm.AddState("home");
            FsmState moreGames = Fsm.AddState("moreGames");
            FsmState shop = Fsm.AddState("shop");
            FsmState settings = Fsm.AddState("settings");
            FsmState game = Fsm.AddState("game");
            FsmState done = Fsm.AddState("done");
            FsmState result = Fsm.AddState("result");

            // actions
            idle.AddAction(new FsmDelegateAction(idle, delegate (FsmState owner)
            {
                Debug.LogFormat(LOG + " SystemRoot::SceneFsm State:{0}\n", owner.GetName());

                // Start with splash
                this.Publish(new LoadSplashSignal());
            }));

            splash.AddAction(new FsmDelegateAction(splash, delegate (FsmState owner)
            {
                Debug.LogFormat(LOG + " SystemRoot::SceneFsm State:{0}\n", owner.GetName());
                
                Promise.AllSequentially(EndFramePromise)                                            
                    .Then(_ => this.LoadScenePromise<PreloaderRoot>(EScene.Preloader))
                    .Then(_ => this.LoadScenePromise<PopupCollectionRoot>(EScene.PopupCollection))
                    .Then(_ => PopupCollection = Scene.GetScene<PopupCollectionRoot>(EScene.PopupCollection))
                    .Then(_ => Preloader = Scene.GetScene<PreloaderRoot>(EScene.Preloader))
                    .Then(_ => Preloader.LoadLoadingScreenPromise())
                    .Then(_ => Preloader.FadeInLoadingScreenPromise())
                    .Then(_ => this.LoadScenePromise<CleanerRoot>(EScene.Cleaner))
                    .Then(_ => this.LoadScenePromise<BackgroundRoot>(EScene.Background))
                    .Then(_ => this.LoadSceneAdditivePromise<AudioRoot>(EScene.Audio))
                    .Then(_ => this.LoadSceneAdditivePromise<SplashRoot>(EScene.Splash, new SceneData() { PrevSceneName = Name }))
                    .Then(_ => Preloader.FadeOutLoadingScreenPromise())
                    .Then(_ => EndFramePromise())  
                    .Finally(_ => Debug.LogFormat(LOG + " SystemRoot::SceneFsm State:{0} Splash loaded!\n", owner.GetName()));
            }));

            preload.AddAction(new FsmDelegateAction(preload, delegate (FsmState owner)
            {
                Debug.LogFormat(LOG + " SystemRoot::SceneFsm State:{0}\n", owner.GetName());

                // load preloader here
                this.LoadScenePromise<ServicesRoot>(EScene.Services);
            }));

            login.AddAction(new FsmDelegateAction(login, delegate (FsmState owner)
            {
                Debug.LogFormat(LOG + " SystemRoot::SceneFsm State:{0}\n", owner.GetName());
                //this.LoadPromise<LoginRoot>(EScene.Login);

                Promise.AllSequentially(EndFramePromise)
                    .Then(_ => this.LoadScenePromise<LoginRoot>(EScene.Login))
                    //.Then(_ => PopupCollection.Show(Popup.Popup001))
                    .Finally(_ => {});
            }));

            home.AddAction(new FsmDelegateAction(home, delegate (FsmState owner)
            {
                Debug.LogFormat(LOG + " SystemRoot::SceneFsm State:{0}\n", owner.GetName());
                
                Promise.AllSequentially(Preloader.LoadLoadingScreenPromise)
                    .Then(_ => Preloader.FadeInLoadingScreenPromise())
                    .Then(_ => this.LoadScenePromise<CleanerRoot>(EScene.Cleaner))
                    .Then(_ => this.LoadScenePromise<HomeRoot>(EScene.Home))
                    .Then(_ => this.LoadSceneAdditivePromise<CurrencyRoot>(EScene.Currency))
                    .Then(_ => Preloader.FadeOutLoadingScreenPromise())
                    .Then(_ => EndFramePromise())
                    .Finally(_ => Debug.LogFormat(LOG + " SystemRoot::SceneFsm State:{0} Home loaded!\n", owner.GetName()));
            }));

            moreGames.AddAction(new FsmDelegateAction(moreGames, delegate (FsmState owner)
            {
                Debug.LogFormat(LOG + " SystemRoot::SceneFsm State:{0}\n", owner.GetName());
                
                Promise.AllSequentially(Preloader.LoadLoadingScreenPromise)
                    .Then(_ => Preloader.FadeInLoadingScreenPromise())
                    .Then(_ => this.LoadScenePromise<CleanerRoot>(EScene.Cleaner))
                    .Then(_ => this.LoadScenePromise<MoreGamesRoot>(EScene.MoreGames))
                    .Then(_ => this.LoadSceneAdditivePromise<BackRoot>(EScene.Back))
                    .Then(_ => Preloader.FadeOutLoadingScreenPromise())
                    .Then(_ => EndFramePromise())
                    .Finally(_ => Debug.LogFormat(LOG + " SystemRoot::SceneFsm State:{0} More Games loaded!\n", owner.GetName()));
            }));

            shop.AddAction(new FsmDelegateAction(shop, delegate (FsmState owner)
            {
                Debug.LogFormat(LOG + " SystemRoot::SceneFsm State:{0}\n", owner.GetName());
                
                Promise.AllSequentially(Preloader.LoadLoadingScreenPromise)
                    .Then(_ => Preloader.FadeInLoadingScreenPromise())
                    .Then(_ => this.LoadScenePromise<CleanerRoot>(EScene.Cleaner))
                    .Then(_ => this.LoadScenePromise<ShopRoot>(EScene.Shop))
                    .Then(_ => this.LoadSceneAdditivePromise<CurrencyRoot>(EScene.Currency))
                    .Then(_ => this.LoadSceneAdditivePromise<BackRoot>(EScene.Back))
                    .Then(_ => Preloader.FadeOutLoadingScreenPromise())
                    .Then(_ => EndFramePromise())
                    .Finally(_ => Debug.LogFormat(LOG + " SystemRoot::SceneFsm State:{0} Shop loaded!\n", owner.GetName()));
            }));

            settings.AddAction(new FsmDelegateAction(settings, delegate (FsmState owner)
            {
                Debug.LogFormat(LOG + " SystemRoot::SceneFsm State:{0}\n", owner.GetName());
                
                Promise.AllSequentially(Preloader.LoadLoadingScreenPromise)
                    .Then(_ => Preloader.FadeInLoadingScreenPromise())
                    .Then(_ => this.LoadScenePromise<CleanerRoot>(EScene.Cleaner))
                    .Then(_ => this.LoadScenePromise<SettingsRoot>(EScene.Settings))
                    .Then(_ => this.LoadSceneAdditivePromise<BackRoot>(EScene.Back))
                    .Then(_ => Preloader.FadeOutLoadingScreenPromise())
                    .Then(_ => EndFramePromise())
                    .Finally(_ => Debug.LogFormat(LOG + " SystemRoot::SceneFsm State:{0} Settings loaded!\n", owner.GetName()));
            }));

            game.AddAction(new FsmDelegateAction(game, delegate (FsmState owner)
            {
                Debug.LogFormat(LOG + " SystemRoot::SceneFsm State:{0}\n", owner.GetName());
                
                Promise.AllSequentially(Preloader.LoadLoadingScreenPromise)
                    .Then(_ => Preloader.FadeInLoadingScreenPromise())
                    .Then(_ => this.LoadScenePromise<CleanerRoot>(EScene.Cleaner))
                    .Then(_ => this.LoadScenePromise<GameRoot>(EScene.Game))
                    //.Then(_ => this.LoadSceneAdditivePromise<CurrencyRoot>(EScene.Currency))
                    .Then(_ => Preloader.FadeOutLoadingScreenPromise())
                    .Then(_ => EndFramePromise())
                    .Finally(_ => Debug.LogFormat(LOG + " SystemRoot::SceneFsm State:{0} Settings loaded!\n", owner.GetName()));
            }));

            result.AddAction(new FsmDelegateAction(result, delegate (FsmState owner)
            {
                Debug.LogFormat(LOG + " SystemRoot::SceneFsm State:{0}\n", owner.GetName());

                Promise.AllSequentially(Preloader.LoadLoadingScreenPromise)
                    .Then(_ => Preloader.FadeInLoadingScreenPromise())
                    .Then(_ => this.LoadScenePromise<CleanerRoot>(EScene.Cleaner))
                    .Then(_ => this.LoadScenePromise<ResultsRoot>(EScene.Results))
                    .Then(_ => Preloader.FadeOutLoadingScreenPromise())
                    .Then(_ => EndFramePromise())
                    .Finally(_ => Debug.LogFormat(LOG + " SystemRoot::SceneFsm State:{0} Result loaded!\n", owner.GetName()));

            }));

            done.AddAction(new FsmDelegateAction(done, delegate (FsmState owner)
            {
                Debug.LogFormat(LOG + " SystemRoot::SceneFsm State:{0}\n", owner.GetName());
            }));
            
            // transitions
            idle.AddTransition(START_SPLASH, splash);

            splash.AddTransition(START_PRELOAD, preload);

            preload.AddTransition(START_LOGIN, login);

            login.AddTransition(LOAD_HOME, home);

            home.AddTransition(LOAD_MORE_GAMES, moreGames);
            home.AddTransition(LOAD_SHOP, shop);
            home.AddTransition(LOAD_SETTINGS, settings);
            home.AddTransition(LOAD_GAME, game);

            game.AddTransition(END_GAME, result);

            result.AddTransition(LOAD_GAME, game);
            result.AddTransition(LOAD_HOME, home);

            moreGames.AddTransition(LOAD_HOME, home);
            shop.AddTransition(LOAD_HOME, home);
            settings.AddTransition(LOAD_HOME, home);
            game.AddTransition(LOAD_HOME, home);

            // auto start fsm
            Fsm.Start(IDLE);
        }

        #endregion

        #region Signals

        /// <summary>
        /// Records the currently and previously loaded scenes.
        /// Does not track Invalid and Cleaner scenes.
        /// </summary>
        private void OnLoadScene(EScene sceneName)
        {
            Debug.LogFormat("{0} OnLoadScene: {1}", Time.time, sceneName);
            
            if (CurrentScene == EScene.Invalid)
            {
                CurrentScene = sceneName;
            }
            else if (sceneName != EScene.Invalid && sceneName != EScene.Cleaner)
            {
                PreviousScene = CurrentScene;
                CurrentScene = sceneName;
            }
        }

        private void OnLoadSplash()
        {
            Debug.LogFormat("{0} OnLoadSplash", Time.time);
            Fsm.SendEvent(START_SPLASH);
        }

        private void OnSplashDone()
        {
            Debug.LogFormat("{0} OnSplashDone", Time.time);
            Fsm.SendEvent(START_PRELOAD);
        }

        private void OnServicesReady()
        {
            Debug.LogFormat("{0} OnServicesReady", Time.time);
            Fsm.SendEvent(START_LOGIN);
        }

        private void OnLoadHome()
        {
            Debug.LogFormat("{0} OnLoadHome", Time.time);
            Fsm.SendEvent(LOAD_HOME);
        }

        private void OnLoadMoreGames()
        {
            Debug.LogFormat("{0} OnLoadMoreGames", Time.time);
            Fsm.SendEvent(LOAD_MORE_GAMES);
        }

        private void OnLoadShop()
        {
            Debug.LogFormat("{0} OnLoadShop", Time.time);
            Fsm.SendEvent(LOAD_SHOP);
        }

        private void OnLoadSettings()
        {
            Debug.LogFormat("{0} OnLoadSettings", Time.time);
            Fsm.SendEvent(LOAD_SETTINGS);
        }

        private void OnLoadGame()
        {
            Debug.LogFormat("{0} OnLoadGame", Time.time);
            Fsm.SendEvent(LOAD_GAME);
        }
        
        public void OnEndGame()
        {
            Debug.LogFormat("{0} OnEndGame", Time.time);
            Fsm.SendEvent(END_GAME);
        }
        #endregion
    }

}