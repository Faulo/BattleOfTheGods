//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.2.0
//     from Assets/Input.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace Runtime
{
    public partial class @Input : IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }
        public @Input()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""Input"",
    ""maps"": [
        {
            ""name"": ""GameplayActionMap"",
            ""id"": ""6a83ac4a-3246-4ae1-975c-71fbe177deac"",
            ""actions"": [
                {
                    ""name"": ""Click"",
                    ""type"": ""Button"",
                    ""id"": ""5688887f-65dc-46cf-9ab2-aa5a933c6316"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""c27ab054-f121-43f6-9c13-4f4b37f89a5a"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Click"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // GameplayActionMap
            m_GameplayActionMap = asset.FindActionMap("GameplayActionMap", throwIfNotFound: true);
            m_GameplayActionMap_Click = m_GameplayActionMap.FindAction("Click", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }
        public IEnumerable<InputBinding> bindings => asset.bindings;

        public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
        {
            return asset.FindAction(actionNameOrId, throwIfNotFound);
        }
        public int FindBinding(InputBinding bindingMask, out InputAction action)
        {
            return asset.FindBinding(bindingMask, out action);
        }

        // GameplayActionMap
        private readonly InputActionMap m_GameplayActionMap;
        private IGameplayActionMapActions m_GameplayActionMapActionsCallbackInterface;
        private readonly InputAction m_GameplayActionMap_Click;
        public struct GameplayActionMapActions
        {
            private @Input m_Wrapper;
            public GameplayActionMapActions(@Input wrapper) { m_Wrapper = wrapper; }
            public InputAction @Click => m_Wrapper.m_GameplayActionMap_Click;
            public InputActionMap Get() { return m_Wrapper.m_GameplayActionMap; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(GameplayActionMapActions set) { return set.Get(); }
            public void SetCallbacks(IGameplayActionMapActions instance)
            {
                if (m_Wrapper.m_GameplayActionMapActionsCallbackInterface != null)
                {
                    @Click.started -= m_Wrapper.m_GameplayActionMapActionsCallbackInterface.OnClick;
                    @Click.performed -= m_Wrapper.m_GameplayActionMapActionsCallbackInterface.OnClick;
                    @Click.canceled -= m_Wrapper.m_GameplayActionMapActionsCallbackInterface.OnClick;
                }
                m_Wrapper.m_GameplayActionMapActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Click.started += instance.OnClick;
                    @Click.performed += instance.OnClick;
                    @Click.canceled += instance.OnClick;
                }
            }
        }
        public GameplayActionMapActions @GameplayActionMap => new GameplayActionMapActions(this);
        public interface IGameplayActionMapActions
        {
            void OnClick(InputAction.CallbackContext context);
        }
    }
}
