// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Player/Input/InputMaster.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputMaster : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputMaster()
    {
        this.asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputMaster"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""a052ea54-9d40-4d66-a294-f509bd48b9c2"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""PassThrough"",
                    ""id"": ""3ba67c98-3b29-4c0d-a19d-b1cb629e3907"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""b4929b82-6898-469c-974c-73a2454924b0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""PassThrough"",
                    ""id"": ""a16746e7-e947-468e-ad81-b5a88ea06e5c"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Restart"",
                    ""type"": ""Button"",
                    ""id"": ""bd3fc753-783a-4f33-9932-c94d121aa02a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Ground Pound"",
                    ""type"": ""Button"",
                    ""id"": ""8dafbd59-a234-4194-98ab-acd6ac84e920"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""Burst"",
                    ""type"": ""Button"",
                    ""id"": ""eb8a9c01-9b08-4665-9dcf-cd0993eabc85"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Upward Dash"",
                    ""type"": ""Button"",
                    ""id"": ""b1bc063b-999f-44b6-a2c1-91c3bb09649d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""Full Stop"",
                    ""type"": ""Button"",
                    ""id"": ""ead930ef-e477-4b0f-8081-58521b34d665"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Grapple Pole"",
                    ""type"": ""Button"",
                    ""id"": ""ded072bd-f6d3-4ec4-985d-12fe0fe19b03"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WSAD"",
                    ""id"": ""55d7f66b-a134-4754-bd5f-8b3103137eb3"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""56d6979e-5213-4196-9193-70d8a4511c69"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""b9317c59-0392-433a-86c4-ef776619c233"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""68d741b8-0b96-47be-a22e-0500de09acc1"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""61886821-b17c-4f64-8770-8fba6a29c110"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""f77d6814-1fdb-4cef-be4f-7e6c0ca84165"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""de6cc492-285a-4283-af8a-94fdd17ce4fe"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""aaf0dbcd-e3f3-47a4-a570-2453da31fe45"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Restart"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d7081889-1f4e-4768-8f78-3ee95bbb188b"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ground Pound"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1e93637b-176f-472d-ae71-78c7ec8a5a4c"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Burst"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fc96117b-391c-44b2-808b-da68dc8961d7"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Upward Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a8e4321c-b2e1-4269-a640-dd0c566a7f7d"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Full Stop"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""adb02bd0-09d5-409d-b2ed-1ab5dc789b4d"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Grapple Pole"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = this.asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Movement = m_Player.FindAction("Movement", throwIfNotFound: true);
        m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
        m_Player_Look = m_Player.FindAction("Look", throwIfNotFound: true);
        m_Player_Restart = m_Player.FindAction("Restart", throwIfNotFound: true);
        m_Player_GroundPound = m_Player.FindAction("Ground Pound", throwIfNotFound: true);
        m_Player_Burst = m_Player.FindAction("Burst", throwIfNotFound: true);
        m_Player_UpwardDash = m_Player.FindAction("Upward Dash", throwIfNotFound: true);
        m_Player_FullStop = m_Player.FindAction("Full Stop", throwIfNotFound: true);
        m_Player_GrapplePole = m_Player.FindAction("Grapple Pole", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(this.asset);
    }

    public InputBinding? bindingMask
    {
        get => this.asset.bindingMask;
        set => this.asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => this.asset.devices;
        set => this.asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => this.asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return this.asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return this.asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        this.asset.Enable();
    }

    public void Disable()
    {
        this.asset.Disable();
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Movement;
    private readonly InputAction m_Player_Jump;
    private readonly InputAction m_Player_Look;
    private readonly InputAction m_Player_Restart;
    private readonly InputAction m_Player_GroundPound;
    private readonly InputAction m_Player_Burst;
    private readonly InputAction m_Player_UpwardDash;
    private readonly InputAction m_Player_FullStop;
    private readonly InputAction m_Player_GrapplePole;
    public struct PlayerActions
    {
        private @InputMaster m_Wrapper;
        public PlayerActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Player_Movement;
        public InputAction @Jump => m_Wrapper.m_Player_Jump;
        public InputAction @Look => m_Wrapper.m_Player_Look;
        public InputAction @Restart => m_Wrapper.m_Player_Restart;
        public InputAction @GroundPound => m_Wrapper.m_Player_GroundPound;
        public InputAction @Burst => m_Wrapper.m_Player_Burst;
        public InputAction @UpwardDash => m_Wrapper.m_Player_UpwardDash;
        public InputAction @FullStop => m_Wrapper.m_Player_FullStop;
        public InputAction @GrapplePole => m_Wrapper.m_Player_GrapplePole;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                this.@Movement.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                this.@Movement.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                this.@Movement.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                this.@Jump.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                this.@Jump.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                this.@Jump.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                this.@Look.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                this.@Look.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                this.@Look.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnLook;
                this.@Restart.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRestart;
                this.@Restart.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRestart;
                this.@Restart.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnRestart;
                this.@GroundPound.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGroundPound;
                this.@GroundPound.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGroundPound;
                this.@GroundPound.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGroundPound;
                this.@Burst.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBurst;
                this.@Burst.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBurst;
                this.@Burst.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBurst;
                this.@UpwardDash.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUpwardDash;
                this.@UpwardDash.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUpwardDash;
                this.@UpwardDash.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnUpwardDash;
                this.@FullStop.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFullStop;
                this.@FullStop.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFullStop;
                this.@FullStop.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnFullStop;
                this.@GrapplePole.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGrapplePole;
                this.@GrapplePole.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGrapplePole;
                this.@GrapplePole.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnGrapplePole;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                this.@Movement.started += instance.OnMovement;
                this.@Movement.performed += instance.OnMovement;
                this.@Movement.canceled += instance.OnMovement;
                this.@Jump.started += instance.OnJump;
                this.@Jump.performed += instance.OnJump;
                this.@Jump.canceled += instance.OnJump;
                this.@Look.started += instance.OnLook;
                this.@Look.performed += instance.OnLook;
                this.@Look.canceled += instance.OnLook;
                this.@Restart.started += instance.OnRestart;
                this.@Restart.performed += instance.OnRestart;
                this.@Restart.canceled += instance.OnRestart;
                this.@GroundPound.started += instance.OnGroundPound;
                this.@GroundPound.performed += instance.OnGroundPound;
                this.@GroundPound.canceled += instance.OnGroundPound;
                this.@Burst.started += instance.OnBurst;
                this.@Burst.performed += instance.OnBurst;
                this.@Burst.canceled += instance.OnBurst;
                this.@UpwardDash.started += instance.OnUpwardDash;
                this.@UpwardDash.performed += instance.OnUpwardDash;
                this.@UpwardDash.canceled += instance.OnUpwardDash;
                this.@FullStop.started += instance.OnFullStop;
                this.@FullStop.performed += instance.OnFullStop;
                this.@FullStop.canceled += instance.OnFullStop;
                this.@GrapplePole.started += instance.OnGrapplePole;
                this.@GrapplePole.performed += instance.OnGrapplePole;
                this.@GrapplePole.canceled += instance.OnGrapplePole;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    public interface IPlayerActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnRestart(InputAction.CallbackContext context);
        void OnGroundPound(InputAction.CallbackContext context);
        void OnBurst(InputAction.CallbackContext context);
        void OnUpwardDash(InputAction.CallbackContext context);
        void OnFullStop(InputAction.CallbackContext context);
        void OnGrapplePole(InputAction.CallbackContext context);
    }
}
