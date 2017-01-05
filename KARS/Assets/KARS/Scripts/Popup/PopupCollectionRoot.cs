using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using uPromise;

using Common;
using Common.Extensions;
using Common.Fsm;
using Common.Query;
using Common.Signal;
using Common.Utils;

// alias
using UScene = UnityEngine.SceneManagement.Scene;

namespace Synergy88
{
    public class PopupCollectionRoot : Scene
    {

        [SerializeField]
        private Popup Popup;

        [SerializeField]
        private GameObject Blocker;

        [SerializeField]
        private List<PopupWindow> Popups;
        
        #region Unity Life Cycle

        protected override void Awake()
        {
            base.Awake();

            Assertion.AssertNotNull(Blocker);
            Assertion.AssertNotNull(Popups);

            AddButtonHandler(EButtonType.Popup001, signal =>
            {
                //QuerySystem.Query<PopupCollectionRoot>(QueryIds.PopupCollection).Show(Popup.Popup001);
                Show(Popup.Popup001);
            });

            AddButtonHandler(EButtonType.Popup002, signal =>
            {
                //QuerySystem.Query<PopupCollectionRoot>(QueryIds.PopupCollection).Show(Popup.Popup002);
                Show(Popup.Popup002);
            });

            AddButtonHandler(EButtonType.Popup003, signal =>
            {
                //QuerySystem.Query<PopupCollectionRoot>(QueryIds.PopupCollection).Show(Popup.Popup003);
                Show(Popup.Popup003);
            });

            AddButtonHandler(EButtonType.Close, signal =>
            {
                CloseActivePopup();
            });
        }

        #endregion

        private void CloseActivePopup()
        {
            if (CanvasList.Count <= 1)
            {
                return;
            }

            Canvas canvas = CanvasList[1];
            Popups.Remove(canvas.GetComponent<PopupWindow>());
            CanvasList.Remove(canvas);

            GameObject.DestroyObject(canvas.gameObject);

            if (CanvasList.Count <= 1)
            {
                Blocker.SetActive(false);
            }

            // Sort
            SortPopupCanvas();
        }

        private void SortPopupCanvas()
        {
            // Fix canvas order
            Canvas canvas = null;
            Camera camera = QuerySystem.Query<Camera>(QueryIds.SystemCamera);
            float depth = (float)SceneDepth.ToInt();
            int count = CanvasList.Count;

            for (int i = 1; i < count; i++)
            {
                if (i == 1)
                {
                    canvas = CanvasList[0];
                    canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    canvas.sortingOrder = SceneDepth.ToInt() - 1;
                    canvas.worldCamera = camera;
                    canvas.transform.SetSiblingIndex(0);
                }

                canvas = CanvasList[i];
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.sortingOrder = SceneDepth.ToInt();
                canvas.worldCamera = camera;
                canvas.transform.SetSiblingIndex(i);
            }

            for (int i = 2; i < count; i++)
            {
                canvas = CanvasList[i];
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.sortingOrder = SceneDepth.ToInt() - 2;
                canvas.worldCamera = camera;
                canvas.transform.SetSiblingIndex(i);
            }
        }
        
        private IEnumerator Load(Popup popUp, Deferred deferred = null)
        {
            string popupScene = popUp.ToString();

            AsyncOperation operation = SceneManager.LoadSceneAsync(popupScene, LoadSceneMode.Additive);
            yield return operation;

            Transform root = transform;
            UScene loadedScene = SceneManager.GetSceneByName(popupScene);
            List<GameObject> rawObjects = new List<GameObject>(loadedScene.GetRootGameObjects());
            List<PopupWindow> objects = rawObjects.ToArray<PopupWindow>();
            
            // make sure the scenes only has 1 root object
            Assertion.Assert(objects.Count == 1);

            // fix object parenting setup
            GameObject obj = objects[0].gameObject;
            obj.transform.SetParent(root);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            obj.transform.SetAsLastSibling();
            obj.SetActive(true);

            // fix canvas settings
            Popups.Add(obj.GetComponent<PopupWindow>());
            
            if (CanvasList.Count <= 1)
            {
                CanvasList.Add(obj.GetComponent<Canvas>());
            }
            else
            {
                CanvasList.Insert(1, obj.GetComponent<Canvas>());
            }
            
            // Fix canvas sorting
            SortPopupCanvas();
            
            yield return SceneManager.UnloadSceneAsync(popupScene);

            if (deferred != null)
            {
                deferred.Resolve();
            }
        }

        public bool IsLoaded(Popup popup)
        {
            return Popup == popup;
        }
        
        public Promise Show(Popup popup)
        {
            // cache pop up
            Popup = popup;
            Blocker.SetActive(true);
            Deferred deferred = new Deferred();
            StartCoroutine(Load(popup, deferred));
            return deferred.Promise;
        }
        
        public void Hide()
        {
            CloseActivePopup();
        }

        /// <summary>
        /// Returns the Top/Currently showing Popup
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetPopup<T>()
        {
            return Popups[0].GetComponent<T>();
        }

        public bool HasPopUp(Popup popup)
        {
            return Popups.Exists(p => p.PopUp == popup);
        }
    }
}