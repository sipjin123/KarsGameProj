using UniRx;

namespace Synergy88
{
    /// <summary>
    /// Services that need to be loaded before the home screen should implement this.
    /// The service Component should also be attached to a GameObject under the ServicesRoot prefab.
    /// </summary>
    public interface IService
    {
        /// <summary>
        /// The name of the service for logging purposes.
        /// </summary>
        string ServiceName { get; }
        
        /// <summary>
        /// True if the service has to be loaded to proceed.
        /// </summary>
        bool IsServiceRequired { get; }

        /// <summary>
        /// The service's initialization state.
        /// ServiceRoot subscribes to this property's change to check if all services are ready.
        /// </summary>
        ReactiveProperty<ServiceState> CurrentServiceState { get; }
        
        /// <summary>
        /// Called to initialized the service when ServicesRoot is enabled.
        /// </summary>
        void InitializeService();

        /// <summary>
        /// Called to terminate the service when ServicesRoot is disabled or destroyed.
        /// </summary>
        void TerminateService();
    }
}
