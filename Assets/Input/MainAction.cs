//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.5.1
//     from Assets/Input/MainAction.inputactions
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

public partial class @MainAction: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @MainAction()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""MainAction"",
    ""maps"": [
        {
            ""name"": ""KeyboardControl"",
            ""id"": ""20587219-0232-4330-8b67-8317765ee981"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""e3cd9535-3a97-41c8-8279-070cd87b60d8"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Rotation"",
                    ""type"": ""Value"",
                    ""id"": ""5bb0914d-f8ec-416f-b608-340d496c1f7f"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Zoom"",
                    ""type"": ""Value"",
                    ""id"": ""4e6da400-c994-41b8-b841-775a85c6ee07"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Skew"",
                    ""type"": ""Value"",
                    ""id"": ""5ade4ca7-6db2-490d-a811-14dd81cf4936"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Map"",
                    ""type"": ""Button"",
                    ""id"": ""d917b44a-efda-44f6-8bfb-1678457b0103"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MainInteract"",
                    ""type"": ""Button"",
                    ""id"": ""bab45d5e-a03f-4171-b547-b58a6469117f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Keyboard"",
                    ""id"": ""1ddce7b7-2e55-4c5f-bd95-4e4aa08ba73e"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""2556c0e7-5c63-41cd-a798-670611b3491a"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""7e8b253f-6626-4d31-a86e-cb1950496a57"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""9ac3d1a9-1691-4b8d-8a40-8189ab4c47de"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""b11cdd49-14e5-46e6-bb5f-b9ef09e74886"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Keyboard"",
                    ""id"": ""158ad21c-14fa-462c-82f2-c1fbd96033a4"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotation"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Left"",
                    ""id"": ""339542c8-af6a-49e0-850a-ab743dedba01"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Right"",
                    ""id"": ""ae6cb25b-4f1d-4394-b3a8-ba59a7e780ed"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Keyboard"",
                    ""id"": ""60a69b53-eaa8-4bc4-a5a7-3daa1e915f8d"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Skew"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""1fe68ae8-98ab-4d05-95be-e3c87e1ad386"",
                    ""path"": ""<Keyboard>/c"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Skew"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""b1feba86-134b-45ba-ab1c-6a108b545b1e"",
                    ""path"": ""<Keyboard>/v"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Skew"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Keyboard"",
                    ""id"": ""ca0e6785-7b8d-4dba-b367-e96fc215d837"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Zoom"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""71763616-08a9-4d01-b844-783afcd75f66"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""6973bb9b-b611-44e1-8b2c-3dc6933e7baa"",
                    ""path"": ""<Keyboard>/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""fdcd4410-8283-4881-bf76-db930b7f356b"",
                    ""path"": ""<Keyboard>/m"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Map"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""97e7530f-4488-4e9e-984e-de1d5051964a"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MainInteract"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""MouseControl"",
            ""id"": ""fdaa4c54-5b65-4367-983d-77bdacfb2860"",
            ""actions"": [
                {
                    ""name"": ""Left"",
                    ""type"": ""Button"",
                    ""id"": ""b3cc98ab-a940-4f1b-b483-88987fc9775c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Right"",
                    ""type"": ""Button"",
                    ""id"": ""f5f1e215-80da-42d3-96bf-8dfb885865c9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RightDouble"",
                    ""type"": ""Button"",
                    ""id"": ""236085ac-2e17-4103-9912-7e8084eb2af0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""MultiTap"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Middle"",
                    ""type"": ""Button"",
                    ""id"": ""bb8f42de-4de0-4b4e-9c27-11cf8a924dd9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Position"",
                    ""type"": ""Value"",
                    ""id"": ""cd4c4852-4e8a-40ae-a63f-75ce0ce8123a"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Scroll"",
                    ""type"": ""Value"",
                    ""id"": ""37d459b6-2550-48b8-88f8-5583a1a0cb02"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""RightHold"",
                    ""type"": ""Button"",
                    ""id"": ""d2d98a75-8d7b-4edc-8433-bf3fca153efe"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MiddleHold"",
                    ""type"": ""Button"",
                    ""id"": ""5b671bc0-8eb1-4af0-8669-52f3d920ea6f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""dc900831-520f-40fb-8677-c38a4151c3ca"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Left"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a42d5fc3-fd31-491f-9caa-ebdc7e79962f"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Right"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7f032eb3-7dc2-408a-bb4a-48c1488f449d"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Middle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""232b1acf-b2b4-40eb-9588-cacc9123a3ba"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Position"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d67d4071-2ce2-466b-a175-956d7192d420"",
                    ""path"": ""<Mouse>/scroll"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Scroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a5f8f0d2-06e6-4f59-b11c-8593ef3988b6"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightDouble"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4e011ca7-6126-4d26-87b2-e4dd1ff2edfb"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": ""Hold"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MiddleHold"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""97d8ed4f-fcff-4915-87e8-9b43c6c1ae80"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": ""Hold"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightHold"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": []
        }
    ]
}");
        // KeyboardControl
        m_KeyboardControl = asset.FindActionMap("KeyboardControl", throwIfNotFound: true);
        m_KeyboardControl_Move = m_KeyboardControl.FindAction("Move", throwIfNotFound: true);
        m_KeyboardControl_Rotation = m_KeyboardControl.FindAction("Rotation", throwIfNotFound: true);
        m_KeyboardControl_Zoom = m_KeyboardControl.FindAction("Zoom", throwIfNotFound: true);
        m_KeyboardControl_Skew = m_KeyboardControl.FindAction("Skew", throwIfNotFound: true);
        m_KeyboardControl_Map = m_KeyboardControl.FindAction("Map", throwIfNotFound: true);
        m_KeyboardControl_MainInteract = m_KeyboardControl.FindAction("MainInteract", throwIfNotFound: true);
        // MouseControl
        m_MouseControl = asset.FindActionMap("MouseControl", throwIfNotFound: true);
        m_MouseControl_Left = m_MouseControl.FindAction("Left", throwIfNotFound: true);
        m_MouseControl_Right = m_MouseControl.FindAction("Right", throwIfNotFound: true);
        m_MouseControl_RightDouble = m_MouseControl.FindAction("RightDouble", throwIfNotFound: true);
        m_MouseControl_Middle = m_MouseControl.FindAction("Middle", throwIfNotFound: true);
        m_MouseControl_Position = m_MouseControl.FindAction("Position", throwIfNotFound: true);
        m_MouseControl_Scroll = m_MouseControl.FindAction("Scroll", throwIfNotFound: true);
        m_MouseControl_RightHold = m_MouseControl.FindAction("RightHold", throwIfNotFound: true);
        m_MouseControl_MiddleHold = m_MouseControl.FindAction("MiddleHold", throwIfNotFound: true);
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

    // KeyboardControl
    private readonly InputActionMap m_KeyboardControl;
    private List<IKeyboardControlActions> m_KeyboardControlActionsCallbackInterfaces = new List<IKeyboardControlActions>();
    private readonly InputAction m_KeyboardControl_Move;
    private readonly InputAction m_KeyboardControl_Rotation;
    private readonly InputAction m_KeyboardControl_Zoom;
    private readonly InputAction m_KeyboardControl_Skew;
    private readonly InputAction m_KeyboardControl_Map;
    private readonly InputAction m_KeyboardControl_MainInteract;
    public struct KeyboardControlActions
    {
        private @MainAction m_Wrapper;
        public KeyboardControlActions(@MainAction wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_KeyboardControl_Move;
        public InputAction @Rotation => m_Wrapper.m_KeyboardControl_Rotation;
        public InputAction @Zoom => m_Wrapper.m_KeyboardControl_Zoom;
        public InputAction @Skew => m_Wrapper.m_KeyboardControl_Skew;
        public InputAction @Map => m_Wrapper.m_KeyboardControl_Map;
        public InputAction @MainInteract => m_Wrapper.m_KeyboardControl_MainInteract;
        public InputActionMap Get() { return m_Wrapper.m_KeyboardControl; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(KeyboardControlActions set) { return set.Get(); }
        public void AddCallbacks(IKeyboardControlActions instance)
        {
            if (instance == null || m_Wrapper.m_KeyboardControlActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_KeyboardControlActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @Rotation.started += instance.OnRotation;
            @Rotation.performed += instance.OnRotation;
            @Rotation.canceled += instance.OnRotation;
            @Zoom.started += instance.OnZoom;
            @Zoom.performed += instance.OnZoom;
            @Zoom.canceled += instance.OnZoom;
            @Skew.started += instance.OnSkew;
            @Skew.performed += instance.OnSkew;
            @Skew.canceled += instance.OnSkew;
            @Map.started += instance.OnMap;
            @Map.performed += instance.OnMap;
            @Map.canceled += instance.OnMap;
            @MainInteract.started += instance.OnMainInteract;
            @MainInteract.performed += instance.OnMainInteract;
            @MainInteract.canceled += instance.OnMainInteract;
        }

        private void UnregisterCallbacks(IKeyboardControlActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @Rotation.started -= instance.OnRotation;
            @Rotation.performed -= instance.OnRotation;
            @Rotation.canceled -= instance.OnRotation;
            @Zoom.started -= instance.OnZoom;
            @Zoom.performed -= instance.OnZoom;
            @Zoom.canceled -= instance.OnZoom;
            @Skew.started -= instance.OnSkew;
            @Skew.performed -= instance.OnSkew;
            @Skew.canceled -= instance.OnSkew;
            @Map.started -= instance.OnMap;
            @Map.performed -= instance.OnMap;
            @Map.canceled -= instance.OnMap;
            @MainInteract.started -= instance.OnMainInteract;
            @MainInteract.performed -= instance.OnMainInteract;
            @MainInteract.canceled -= instance.OnMainInteract;
        }

        public void RemoveCallbacks(IKeyboardControlActions instance)
        {
            if (m_Wrapper.m_KeyboardControlActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IKeyboardControlActions instance)
        {
            foreach (var item in m_Wrapper.m_KeyboardControlActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_KeyboardControlActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public KeyboardControlActions @KeyboardControl => new KeyboardControlActions(this);

    // MouseControl
    private readonly InputActionMap m_MouseControl;
    private List<IMouseControlActions> m_MouseControlActionsCallbackInterfaces = new List<IMouseControlActions>();
    private readonly InputAction m_MouseControl_Left;
    private readonly InputAction m_MouseControl_Right;
    private readonly InputAction m_MouseControl_RightDouble;
    private readonly InputAction m_MouseControl_Middle;
    private readonly InputAction m_MouseControl_Position;
    private readonly InputAction m_MouseControl_Scroll;
    private readonly InputAction m_MouseControl_RightHold;
    private readonly InputAction m_MouseControl_MiddleHold;
    public struct MouseControlActions
    {
        private @MainAction m_Wrapper;
        public MouseControlActions(@MainAction wrapper) { m_Wrapper = wrapper; }
        public InputAction @Left => m_Wrapper.m_MouseControl_Left;
        public InputAction @Right => m_Wrapper.m_MouseControl_Right;
        public InputAction @RightDouble => m_Wrapper.m_MouseControl_RightDouble;
        public InputAction @Middle => m_Wrapper.m_MouseControl_Middle;
        public InputAction @Position => m_Wrapper.m_MouseControl_Position;
        public InputAction @Scroll => m_Wrapper.m_MouseControl_Scroll;
        public InputAction @RightHold => m_Wrapper.m_MouseControl_RightHold;
        public InputAction @MiddleHold => m_Wrapper.m_MouseControl_MiddleHold;
        public InputActionMap Get() { return m_Wrapper.m_MouseControl; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MouseControlActions set) { return set.Get(); }
        public void AddCallbacks(IMouseControlActions instance)
        {
            if (instance == null || m_Wrapper.m_MouseControlActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_MouseControlActionsCallbackInterfaces.Add(instance);
            @Left.started += instance.OnLeft;
            @Left.performed += instance.OnLeft;
            @Left.canceled += instance.OnLeft;
            @Right.started += instance.OnRight;
            @Right.performed += instance.OnRight;
            @Right.canceled += instance.OnRight;
            @RightDouble.started += instance.OnRightDouble;
            @RightDouble.performed += instance.OnRightDouble;
            @RightDouble.canceled += instance.OnRightDouble;
            @Middle.started += instance.OnMiddle;
            @Middle.performed += instance.OnMiddle;
            @Middle.canceled += instance.OnMiddle;
            @Position.started += instance.OnPosition;
            @Position.performed += instance.OnPosition;
            @Position.canceled += instance.OnPosition;
            @Scroll.started += instance.OnScroll;
            @Scroll.performed += instance.OnScroll;
            @Scroll.canceled += instance.OnScroll;
            @RightHold.started += instance.OnRightHold;
            @RightHold.performed += instance.OnRightHold;
            @RightHold.canceled += instance.OnRightHold;
            @MiddleHold.started += instance.OnMiddleHold;
            @MiddleHold.performed += instance.OnMiddleHold;
            @MiddleHold.canceled += instance.OnMiddleHold;
        }

        private void UnregisterCallbacks(IMouseControlActions instance)
        {
            @Left.started -= instance.OnLeft;
            @Left.performed -= instance.OnLeft;
            @Left.canceled -= instance.OnLeft;
            @Right.started -= instance.OnRight;
            @Right.performed -= instance.OnRight;
            @Right.canceled -= instance.OnRight;
            @RightDouble.started -= instance.OnRightDouble;
            @RightDouble.performed -= instance.OnRightDouble;
            @RightDouble.canceled -= instance.OnRightDouble;
            @Middle.started -= instance.OnMiddle;
            @Middle.performed -= instance.OnMiddle;
            @Middle.canceled -= instance.OnMiddle;
            @Position.started -= instance.OnPosition;
            @Position.performed -= instance.OnPosition;
            @Position.canceled -= instance.OnPosition;
            @Scroll.started -= instance.OnScroll;
            @Scroll.performed -= instance.OnScroll;
            @Scroll.canceled -= instance.OnScroll;
            @RightHold.started -= instance.OnRightHold;
            @RightHold.performed -= instance.OnRightHold;
            @RightHold.canceled -= instance.OnRightHold;
            @MiddleHold.started -= instance.OnMiddleHold;
            @MiddleHold.performed -= instance.OnMiddleHold;
            @MiddleHold.canceled -= instance.OnMiddleHold;
        }

        public void RemoveCallbacks(IMouseControlActions instance)
        {
            if (m_Wrapper.m_MouseControlActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IMouseControlActions instance)
        {
            foreach (var item in m_Wrapper.m_MouseControlActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_MouseControlActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public MouseControlActions @MouseControl => new MouseControlActions(this);
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get
        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    public interface IKeyboardControlActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnRotation(InputAction.CallbackContext context);
        void OnZoom(InputAction.CallbackContext context);
        void OnSkew(InputAction.CallbackContext context);
        void OnMap(InputAction.CallbackContext context);
        void OnMainInteract(InputAction.CallbackContext context);
    }
    public interface IMouseControlActions
    {
        void OnLeft(InputAction.CallbackContext context);
        void OnRight(InputAction.CallbackContext context);
        void OnRightDouble(InputAction.CallbackContext context);
        void OnMiddle(InputAction.CallbackContext context);
        void OnPosition(InputAction.CallbackContext context);
        void OnScroll(InputAction.CallbackContext context);
        void OnRightHold(InputAction.CallbackContext context);
        void OnMiddleHold(InputAction.CallbackContext context);
    }
}