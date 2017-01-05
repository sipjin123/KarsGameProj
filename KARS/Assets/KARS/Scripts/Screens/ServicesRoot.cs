using UniRx;
using UnityEngine;

using System;

namespace Synergy88
{
    /// <summary>
    /// This handles initialization and termination of services.
    /// IService references are retrieved from this GameObject's children.
    /// Services are initialized when this MonoBehaviour is enabled.
    /// Services are terminated when this MonoBehaviour is disabled or destroyed.
    /// </summary>
    public class ServicesRoot : Scene
    {
        private IService[] Services;

        protected override void OnEnable()
        {
            base.OnEnable();

            InitializeServices();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            TerminateServices();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            TerminateServices();
        }

        /// <summary>
        /// Finds and initializes services.
        /// </summary>
        private void InitializeServices()
        {
            // get all services from child GameObjects
            Services = GetComponentsInChildren<IService>();

            if (Services.Length == 0)
            {
                // let system know services are ready
                this.Publish(new ServicesReadySignal());
                return;
            }

            // initialize each service
            foreach (IService service in Services)
            {
                Debug.LogWarningFormat("{0} Initializing {1}...", Time.time, service.ServiceName);

                // subscribe to service being ready
                service.CurrentServiceState
                    .Where(state => state > ServiceState.Uninitialized)
                    .Take(1)
                    .Subscribe(state =>
                    {
                        Debug.LogWarningFormat("{0} {1} state: {2}", Time.time, service.ServiceName, state);
                        CheckAllServicesReady();
                    })
                    .AddTo(this);

                // initialize service
                service.InitializeService();
            }
        }

        /// <summary>
        /// Checks if all services are ready and publishes ServicesReadySignal when they are.
        /// </summary>
        private void CheckAllServicesReady()
        {
            bool allServicesReady = Array.Find(Services, s => s.CurrentServiceState.Value == ServiceState.Uninitialized) == null && // check if no service is still at None state
                Array.Find(Services, s => s.CurrentServiceState.Value == ServiceState.Error && s.IsServiceRequired) == null; // check if no required service has an error
            Debug.LogWarningFormat("{0} CheckAllServicesReady: {1}", Time.time, allServicesReady);
            if (allServicesReady)
            {
                // let system know services are ready
                this.Publish(new ServicesReadySignal());
            }
        }

        /// <summary>
        /// Terminates all cached services and resets service list.
        /// </summary>
        private void TerminateServices()
        {
            // no need to terminate if there aren't cached services
            if (Services == null) return;

            // terminate services
            foreach (IService service in Services)
            {
                if (service == null)
                {
                    UnityEngine.Debug.LogWarningFormat("ServicesRoot: Tried to terminate a null service reference");
                    continue;
                }

                service.TerminateService();
            }

            // reset service list
            Services = null;
        }
    }
}
