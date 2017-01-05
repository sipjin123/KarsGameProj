using UnityEngine;
using UniRx;

namespace Synergy88
{
    /// <summary>
    /// This is a MonoBehaviour/Component from which classes can inherit methods for publishing and receiving signals.
    /// </summary>
    public class SignalComponent : MonoBehaviour
    {
        /// <summary>
        /// Publish a given signal (an object of any Type).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        public void Publish<T>(T message)
        {
            MessageBroker.Default.Publish(message);
        }

        /// <summary>
        /// Get the observable for the given Type of signal.
        /// From this observable, normal Rx operations such as filtering (Where()) and subscription (Subscribe()) may be done.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IObservable<T> Receive<T>()
        {
            return MessageBroker.Default.Receive<T>();
        }
    }
}
