using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;

using Common;
using Common.Extensions;
using Common.Signal;

// alias
using CColor = Common.Extensions.Color;
using UScene = UnityEngine.SceneManagement.Scene;

namespace Synergy88
{
    /// <summary>
    /// This component should be attached to a button.
    /// This publishes signals for when the button is clicked, hovered, unhovered, pressed, and released.
    /// This can be extended by buttons that need to pass data when clicked (ex. ItemButton).
    /// </summary>
	public class Button : SignalComponent
    {
        private static readonly string ERROR = CColor.red.LogHeader("[ERROR]");
        private static readonly string WARNING = CColor.yellow.LogHeader("[WARNING]");

        /// <summary>
        /// Do not edit! cached values for Editor.
        /// Stores the string value of EButton enum of this button.
        /// </summary>
        [SerializeField, HideInInspector]
        private string _ButtonType = string.Empty;
        public string ButtonType
        {
            get
            {
                return _ButtonType;
            }
            set
            {
                Debug.LogWarningFormat(WARNING + " Button::ButtonType Only the ButtonEditor.cs is allowed to call this method!\n");
                _ButtonType = value;
            }
        }

        /// <summary>
        /// The type of button this is.
        /// </summary>
		protected EButtonType _Button;

        private void Awake() {
            // Update Button Type Editor
            _Button = ButtonType.ToEnum<EButtonType>();

            Assertion.Assert(_Button != EButtonType.Invalid);
		}
        
        /// <summary>
        /// This should be called when the button is clicked.
        /// This publishes ButtonClickedSignal.
        /// </summary>
		public void OnClickedButton() {
            Publish(new ButtonClickedSignal()
            {
                ButtonType = _Button
            });
		}
        
        /// <summary>
        /// This should be called when the button is hovered.
        /// This publishes ButtonHoveredSignal.
        /// </summary>
        public void OnHoveredButton()
        {
            Publish(new ButtonHoveredSignal()
            {
                ButtonType = _Button
            });
        }

        /// <summary>
        /// This should be called when the button is unhovered.
        /// This publishes ButtonUnhoveredSignal.
        /// </summary>
        public void OnUnhoveredButton()
        {
            Publish(new ButtonUnhoveredSignal()
            {
                ButtonType = _Button
            });
        }

        /// <summary>
        /// This should be called when the button is pressed.
        /// This publishes ButtonPressedSignal.
        /// </summary>
        public void OnPressedButton()
        {
            Publish(new ButtonPressedSignal()
            {
                ButtonType = _Button
            });
        }

        /// <summary>
        /// This should be called when the pointer is released on top of the button.
        /// This publishes ButtonReleasedSignal.
        /// </summary>
        public void OnReleasedButton()
        {
            Publish(new ButtonReleasedSignal()
            {
                ButtonType = _Button
            });
        }
    }

}