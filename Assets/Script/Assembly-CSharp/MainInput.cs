// GENERATED AUTOMATICALLY FROM 'Assets/Main.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace SCPCB.Remaster
{
    public class @MainInput : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @MainInput()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""Main"",
    ""maps"": [
        {
            ""name"": ""Game"",
            ""id"": ""f9f9854a-7cd3-4037-9860-ae1a78f0ee09"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""8bc0b0a8-6d6a-4fc5-9a61-34904fa6d52a"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""e969077c-365c-4496-a792-d7c7cfcb1b34"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""08f6af44-9f38-4628-b618-7e8fb5dd8066"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Blink"",
                    ""type"": ""Button"",
                    ""id"": ""586fb2e8-c1a6-409e-8f64-9b7101fc7c09"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Sprint"",
                    ""type"": ""Button"",
                    ""id"": ""4c02c063-be27-4b4e-8634-2bfad38c0692"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""a21ae1c4-c31d-45d5-b52f-603b4d42c392"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Inventory"",
                    ""type"": ""Button"",
                    ""id"": ""00933347-f45d-4e51-b4b0-fe25a735211d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""QuickSave"",
                    ""type"": ""Button"",
                    ""id"": ""320a15a1-54e2-4109-9b93-198dd8799806"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Console"",
                    ""type"": ""Button"",
                    ""id"": ""e4785e56-580f-4f12-88c9-6d9795941c81"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""56b0d800-7770-4df2-899e-e601159cc86b"",
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
                    ""id"": ""fd9bd39b-c92b-48f7-a3a1-f78cb43c8407"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse + Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""202f0a45-b8c4-4f45-a720-b95f70c254cc"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse + Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""e5158b07-918a-48b1-89df-0171e43f0084"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse + Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""9a6fecae-1083-4127-9c4c-63d536ee3969"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse + Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""daa71e07-e4fa-49ce-98ce-2f3f9db4fd93"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c6a7f882-f9d1-471d-ad75-393134b658e3"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""df5db9f0-1fee-40c9-954c-4ba71a3653a5"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse + Keyboard"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""608b7519-2b6e-4e87-939f-4da2fc47acd9"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse + Keyboard"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cd2d2549-9981-49f4-8a74-8d278ff8f0fc"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d72a6261-02c8-4f71-bb31-32349a2ec64c"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse + Keyboard"",
                    ""action"": ""Blink"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f612f056-1c83-41ac-9cd1-e1d729257eee"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Blink"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""370bb3cf-5d58-4972-8b92-3e8a62bed83e"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse + Keyboard"",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""614942c1-b979-4a6d-be77-00ffa21359d5"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3d85bd22-4514-44c6-bda4-57a8e68165dc"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse + Keyboard"",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d77ff0ca-d48f-4355-93d4-7a1d2f520828"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3aca6d50-f396-4e7e-8a4f-eea7085615c4"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse + Keyboard"",
                    ""action"": ""Inventory"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9c2a38d2-b617-4949-bc9f-608695f8395e"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Inventory"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""26b1d5f1-a215-4abb-88d9-da083cacb82f"",
                    ""path"": ""<Keyboard>/f5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse + Keyboard"",
                    ""action"": ""QuickSave"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f631aee1-be77-4e71-9468-00df4229d0e7"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""QuickSave"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""13d29f7c-ddc3-4f84-afae-d4baef175101"",
                    ""path"": ""<Keyboard>/backquote"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse + Keyboard"",
                    ""action"": ""Console"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Button With Two Modifiers"",
                    ""id"": ""ef5f6540-201b-4ae5-a85e-89a78f313e77"",
                    ""path"": ""ButtonWithTwoModifiers"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Console"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier1"",
                    ""id"": ""0ef5a4e5-b41d-4ee8-a007-f1ffef34a270"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Console"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""modifier2"",
                    ""id"": ""1b31b45f-d728-40be-9605-0f7cfa89558a"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Console"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""39959432-31cf-4dae-a1c9-75ddc8c6152d"",
                    ""path"": ""<Gamepad>/leftStickPress"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Console"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Mouse + Keyboard"",
            ""bindingGroup"": ""Mouse + Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // Game
            m_Game = asset.FindActionMap("Game", throwIfNotFound: true);
            m_Game_Move = m_Game.FindAction("Move", throwIfNotFound: true);
            m_Game_Look = m_Game.FindAction("Look", throwIfNotFound: true);
            m_Game_Interact = m_Game.FindAction("Interact", throwIfNotFound: true);
            m_Game_Blink = m_Game.FindAction("Blink", throwIfNotFound: true);
            m_Game_Sprint = m_Game.FindAction("Sprint", throwIfNotFound: true);
            m_Game_Crouch = m_Game.FindAction("Crouch", throwIfNotFound: true);
            m_Game_Inventory = m_Game.FindAction("Inventory", throwIfNotFound: true);
            m_Game_QuickSave = m_Game.FindAction("QuickSave", throwIfNotFound: true);
            m_Game_Console = m_Game.FindAction("Console", throwIfNotFound: true);
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

        // Game
        private readonly InputActionMap m_Game;
        private IGameActions m_GameActionsCallbackInterface;
        private readonly InputAction m_Game_Move;
        private readonly InputAction m_Game_Look;
        private readonly InputAction m_Game_Interact;
        private readonly InputAction m_Game_Blink;
        private readonly InputAction m_Game_Sprint;
        private readonly InputAction m_Game_Crouch;
        private readonly InputAction m_Game_Inventory;
        private readonly InputAction m_Game_QuickSave;
        private readonly InputAction m_Game_Console;
        public struct GameActions
        {
            private @MainInput m_Wrapper;
            public GameActions(@MainInput wrapper) { m_Wrapper = wrapper; }
            public InputAction @Move => m_Wrapper.m_Game_Move;
            public InputAction @Look => m_Wrapper.m_Game_Look;
            public InputAction @Interact => m_Wrapper.m_Game_Interact;
            public InputAction @Blink => m_Wrapper.m_Game_Blink;
            public InputAction @Sprint => m_Wrapper.m_Game_Sprint;
            public InputAction @Crouch => m_Wrapper.m_Game_Crouch;
            public InputAction @Inventory => m_Wrapper.m_Game_Inventory;
            public InputAction @QuickSave => m_Wrapper.m_Game_QuickSave;
            public InputAction @Console => m_Wrapper.m_Game_Console;
            public InputActionMap Get() { return m_Wrapper.m_Game; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(GameActions set) { return set.Get(); }
            public void SetCallbacks(IGameActions instance)
            {
                if (m_Wrapper.m_GameActionsCallbackInterface != null)
                {
                    @Move.started -= m_Wrapper.m_GameActionsCallbackInterface.OnMove;
                    @Move.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnMove;
                    @Move.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnMove;
                    @Look.started -= m_Wrapper.m_GameActionsCallbackInterface.OnLook;
                    @Look.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnLook;
                    @Look.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnLook;
                    @Interact.started -= m_Wrapper.m_GameActionsCallbackInterface.OnInteract;
                    @Interact.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnInteract;
                    @Interact.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnInteract;
                    @Blink.started -= m_Wrapper.m_GameActionsCallbackInterface.OnBlink;
                    @Blink.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnBlink;
                    @Blink.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnBlink;
                    @Sprint.started -= m_Wrapper.m_GameActionsCallbackInterface.OnSprint;
                    @Sprint.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnSprint;
                    @Sprint.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnSprint;
                    @Crouch.started -= m_Wrapper.m_GameActionsCallbackInterface.OnCrouch;
                    @Crouch.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnCrouch;
                    @Crouch.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnCrouch;
                    @Inventory.started -= m_Wrapper.m_GameActionsCallbackInterface.OnInventory;
                    @Inventory.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnInventory;
                    @Inventory.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnInventory;
                    @QuickSave.started -= m_Wrapper.m_GameActionsCallbackInterface.OnQuickSave;
                    @QuickSave.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnQuickSave;
                    @QuickSave.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnQuickSave;
                    @Console.started -= m_Wrapper.m_GameActionsCallbackInterface.OnConsole;
                    @Console.performed -= m_Wrapper.m_GameActionsCallbackInterface.OnConsole;
                    @Console.canceled -= m_Wrapper.m_GameActionsCallbackInterface.OnConsole;
                }
                m_Wrapper.m_GameActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Move.started += instance.OnMove;
                    @Move.performed += instance.OnMove;
                    @Move.canceled += instance.OnMove;
                    @Look.started += instance.OnLook;
                    @Look.performed += instance.OnLook;
                    @Look.canceled += instance.OnLook;
                    @Interact.started += instance.OnInteract;
                    @Interact.performed += instance.OnInteract;
                    @Interact.canceled += instance.OnInteract;
                    @Blink.started += instance.OnBlink;
                    @Blink.performed += instance.OnBlink;
                    @Blink.canceled += instance.OnBlink;
                    @Sprint.started += instance.OnSprint;
                    @Sprint.performed += instance.OnSprint;
                    @Sprint.canceled += instance.OnSprint;
                    @Crouch.started += instance.OnCrouch;
                    @Crouch.performed += instance.OnCrouch;
                    @Crouch.canceled += instance.OnCrouch;
                    @Inventory.started += instance.OnInventory;
                    @Inventory.performed += instance.OnInventory;
                    @Inventory.canceled += instance.OnInventory;
                    @QuickSave.started += instance.OnQuickSave;
                    @QuickSave.performed += instance.OnQuickSave;
                    @QuickSave.canceled += instance.OnQuickSave;
                    @Console.started += instance.OnConsole;
                    @Console.performed += instance.OnConsole;
                    @Console.canceled += instance.OnConsole;
                }
            }
        }
        public GameActions @Game => new GameActions(this);
        private int m_MouseKeyboardSchemeIndex = -1;
        public InputControlScheme MouseKeyboardScheme
        {
            get
            {
                if (m_MouseKeyboardSchemeIndex == -1) m_MouseKeyboardSchemeIndex = asset.FindControlSchemeIndex("Mouse + Keyboard");
                return asset.controlSchemes[m_MouseKeyboardSchemeIndex];
            }
        }
        private int m_GamepadSchemeIndex = -1;
        public InputControlScheme GamepadScheme
        {
            get
            {
                if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
                return asset.controlSchemes[m_GamepadSchemeIndex];
            }
        }
        public interface IGameActions
        {
            void OnMove(InputAction.CallbackContext context);
            void OnLook(InputAction.CallbackContext context);
            void OnInteract(InputAction.CallbackContext context);
            void OnBlink(InputAction.CallbackContext context);
            void OnSprint(InputAction.CallbackContext context);
            void OnCrouch(InputAction.CallbackContext context);
            void OnInventory(InputAction.CallbackContext context);
            void OnQuickSave(InputAction.CallbackContext context);
            void OnConsole(InputAction.CallbackContext context);
        }
    }
}
