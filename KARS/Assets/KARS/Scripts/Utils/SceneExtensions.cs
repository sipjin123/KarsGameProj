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
using Common.Query;
using Common.Signal;

namespace Synergy88
{

    public static class SceneExtensions
    {
        /// <summary>
        /// Loads the given scene.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="eScene">The type/ID of the scene to be loaded</param>
        /// <returns></returns>
        public static Promise LoadScenePromise<T>(this Scene scene, EScene eScene) where T : Scene
        {
            Debug.LogFormat("[SYNERGY88] SceneExtensions::LoadPromise SceneType:{0} Scene:{1}\n", typeof(T), eScene);

            Deferred deferred = new Deferred();
            scene.StartCoroutine(scene.LoadSceneAsync<T>(deferred, eScene));
            return deferred.Promise;
        }

        /// <summary>
        /// Loads the given scene with data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="eScene">The type/ID of the scene to be loaded</param>
        /// <param name="data">Data to be passed to the scene</param>
        /// <returns></returns>
        public static Promise LoadScenePromise<T>(this Scene scene, EScene eScene, ISceneData data) where T : Scene
        {
            Debug.LogFormat("[SYNERGY88] SceneExtensions::LoadPromise SceneType:{0} Scene:{1}\n", typeof(T), eScene);

            Deferred deferred = new Deferred();
            scene.StartCoroutine(scene.LoadSceneAsync<T>(deferred, eScene, data));
            return deferred.Promise;
        }

        /// <summary>
        /// Loads the given scene.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="sScenee">The name of the scene to be loaded</param>
        /// <returns></returns>
        public static Promise LoadScenePromise<T>(this Scene scene, string sScenee) where T : Scene
        {
            Debug.LogFormat("[SYNERGY88] SceneExtensions::LoadPromise SceneType:{0} Scene:{1}\n", typeof(T), sScenee);

            Deferred deferred = new Deferred();
            scene.StartCoroutine(scene.LoadSceneAsync<T>(deferred, sScenee));
            return deferred.Promise;
        }

        /// <summary>
        /// Loads the given scene with data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="sScenee">The name of the scene to be loaded</param>
        /// <param name="data">Data to be passed to the scene</param>
        /// <returns></returns>
        public static Promise LoadScenePromise<T>(this Scene scene, string sScenee, ISceneData data) where T : Scene
        {
            Debug.LogFormat("[SYNERGY88] SceneExtensions::LoadPromise SceneType:{0} Scene:{1}\n", typeof(T), sScenee);

            Deferred deferred = new Deferred();
            scene.StartCoroutine(scene.LoadSceneAsync<T>(deferred, sScenee, data));
            return deferred.Promise;
        }

        /// <summary>
        /// Loads the given scene additively.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="eScene"></param>
        /// <returns></returns>
        public static Promise LoadSceneAdditivePromise<T>(this Scene scene, EScene eScene) where T : Scene
        {
            Debug.LogFormat("[SYNERGY88] SceneExtensions::LoadAdditivePromise SceneType:{0} Scene:{1}\n", typeof(T), eScene);

            Deferred deferred = new Deferred();
            scene.StartCoroutine(scene.LoadAdditiveSceneAsync<T>(deferred, eScene));
            return deferred.Promise;
        }

        /// <summary>
        /// Loads the given scene additively with data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scene"></param>
        /// <param name="eScene"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Promise LoadSceneAdditivePromise<T>(this Scene scene, EScene eScene, ISceneData data) where T : Scene
        {
            Debug.LogFormat("[SYNERGY88] SceneExtensions::LoadAdditivePromise SceneType:{0} Scene:{1}\n", typeof(T), eScene);

            Deferred deferred = new Deferred();
            scene.StartCoroutine(scene.LoadAdditiveSceneAsync<T>(deferred, eScene, data));
            return deferred.Promise;
        }

        /// <summary>
        /// Loads the given scene additively.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="sScenee"></param>
        /// <returns></returns>
        public static Promise LoadSceneAdditivePromise(this Scene scene, string sScenee)
        {
            Debug.LogFormat("[SYNERGY88] SceneExtensions::LoadAdditivePromise Scene:{0}\n", sScenee);

            Deferred deferred = new Deferred();
            scene.StartCoroutine(scene.LoadAdditiveSceneAsync(deferred, sScenee));
            return deferred.Promise;
        }

        //public static void Publish<T>(this Scene scene, T message) where T : IMessage
        public static void Publish<T>(this Scene scene, T message)
        {
            MessageBroker.Default.Publish<T>(message);
        }

        //public static IObservable<T> Receive<T>() where T : IMessage
        public static IObservable<T> Receive<T>(this Scene scene)
        {
            return MessageBroker.Default.Receive<T>();
        }

    }

}

