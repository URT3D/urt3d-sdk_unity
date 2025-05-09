using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Urt3d.Traits;
using Urt3d.Utilities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d.Configs.Persona
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Data container for custom input config.
    /// </summary>
    public class InputConfig : Config
    {
        /// <summary>
        /// Singleton accessor for the global InputConfig instance.
        /// Provides centralized access to input configuration and mapping.
        /// </summary>
        public static InputConfig Instance
        {
            get
            {
                if (s_self == null)
                {
                    s_self = new InputConfig();
                }
                return s_self;
            }
        }
        private static InputConfig s_self = null;

        /// <summary>
        /// Defines the standard input action types supported by the URT3D system.
        /// These represent the high-level input actions that can be mapped to various
        /// input devices and control schemes.
        /// </summary>
        public enum InputType
        {
            Attack,
            Crouch,
            Grab,
            Interact,
            Jump,
            Laser
        }

        /// <summary>
        /// Event triggered when an input action is performed.
        /// Subscribers can respond to various input types through this centralized event.
        /// </summary>
        public Action<InputType> OnInput;

        #region Private Types

        /// <summary>
        /// Defines the input controls available for AR/VR/XR devices.
        /// Maps to standard XR controller inputs like buttons, triggers, and grips.
        /// </summary>
        private enum ArVrXrType
        {
            None,
            A, B, X, Y, A_X, B_Y,
            Grip, Left_Grip, Right_Grip,
            Trigger, Left_Trigger, Right_Trigger
        }

        /// <summary>
        /// Defines the input controls available for gamepad devices.
        /// Maps to standard gamepad inputs like buttons, triggers, bumpers, and D-pad.
        /// </summary>
        private enum GamepadType
        {
            None,
            Button_North, Button_South, Button_East, Button_West,
            DPad_Up, DPad_Down, DPad_Left, DPad_Right,
            Left_Shoulder, Right_Shoulder,
            Left_Trigger, Right_Trigger
        }

        /// <summary>
        /// Defines the input controls available for keyboard devices.
        /// Maps to standard keyboard keys including letters, arrows, modifiers, and special keys.
        /// </summary>
        private enum KeyboardType
        {
            None,
            A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
            Alt, Backspace, Control, Delete, Enter, Shift, Space,
            Up_Arrow, Down_Arrow, Left_Arrow, Right_Arrow,
        }

        #endregion

        #region Private Control Paths

        /// <summary>
        /// Converts an enum value representing an input control to its corresponding Input System path.
        /// This is a generic method that routes to the appropriate type-specific conversion method.
        /// </summary>
        /// <param name="type">The enum value representing an input control</param>
        /// <returns>The Input System control path string for the specified input</returns>
        private static string GetControlPath(Enum type)
        {
            if (type is ArVrXrType arVrXrType) return GetControlPath(arVrXrType);
            if (type is GamepadType gamepadType) return GetControlPath(gamepadType);
            if (type is KeyboardType keyboardType) return GetControlPath(keyboardType);
            return default;
        }

        /// <summary>
        /// Converts an AR/VR/XR input type to its corresponding Input System control path.
        /// Maps the enum values to the appropriate XR controller binding paths.
        /// </summary>
        /// <param name="type">The AR/VR/XR input type</param>
        /// <returns>The Input System control path for the specified XR input</returns>
        private static string GetControlPath(ArVrXrType type)
        {
            var path = type switch
            {
                ArVrXrType.A             => "{RightHand}/{PrimaryAction}",
                ArVrXrType.B             => "{RightHand}/{SecondaryAction}",
                ArVrXrType.X             => "{LeftHand}/{PrimaryAction}",
                ArVrXrType.Y             => "{LeftHand}/{SecondaryAction}",
                ArVrXrType.A_X           => "{PrimaryAction}",
                ArVrXrType.B_Y           => "{SecondaryAction}",
                ArVrXrType.Grip          => "{SecondaryTrigger}",
                ArVrXrType.Left_Grip     => "{LeftHand}/{SecondaryTrigger}",
                ArVrXrType.Right_Grip    => "{RightHand}/{SecondaryTrigger}",
                ArVrXrType.Trigger       => "{PrimaryTrigger}",
                ArVrXrType.Left_Trigger  => "{LeftHand}/{PrimaryTrigger}",
                ArVrXrType.Right_Trigger => "{RightHand}/{PrimaryTrigger}",
                _ => ""
            };
            return GetControlPath($"<XRController>/{path}");
        }

        /// <summary>
        /// Converts a gamepad input type to its corresponding Input System control path.
        /// Maps the enum values to the appropriate gamepad binding paths.
        /// </summary>
        /// <param name="type">The gamepad input type</param>
        /// <returns>The Input System control path for the specified gamepad input</returns>
        private static string GetControlPath(GamepadType type)
        {
            var path = $"{type}";
            path = path.Replace("DPad_", "dpad/");
            return GetControlPath($"<Gamepad>/{path}");
        }

        /// <summary>
        /// Converts a keyboard input type to its corresponding Input System control path.
        /// Maps the enum values to the appropriate keyboard binding paths.
        /// </summary>
        /// <param name="type">The keyboard input type</param>
        /// <returns>The Input System control path for the specified keyboard input</returns>
        private static string GetControlPath(KeyboardType type)
        {
            return GetControlPath($"<Keyboard>/{type}");
        }

        /// <summary>
        /// Formats and normalizes an Input System control path string.
        /// Removes underscores to match Unity's Input System path format requirements.
        /// </summary>
        /// <param name="path">The raw input path to format</param>
        /// <returns>The formatted control path string</returns>
        private static string GetControlPath(string path)
        {
            return path.Replace("_", "");
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Data structure that groups input traits for different device types.
        /// Stores traits for AR/VR/XR, gamepad, and keyboard input mappings for a single action type.
        /// </summary>
        private struct InputTraits
        {
            public Trait<ArVrXrType> ArVrXr;
            public Trait<GamepadType> Gamepad;
            public Trait<KeyboardType> Keyboard;
        }

        /// <summary>
        /// Initializes the InputConfig with default mappings for all supported input types.
        /// Sets up the traits for different input devices (AR/VR, gamepad, keyboard) and
        /// their respective control mappings for actions like attack, crouch, grab, etc.
        /// 
        /// This method enforces that only one InputConfig instance exists in the system,
        /// making this class effectively a singleton. It also adds all the input traits
        /// to the asset, making them accessible through the standard Urt3d trait system.
        /// </summary>
        protected override void Initialize()
        {
            var data = new Dictionary<InputType, InputTraits>
            {
                [InputType.Attack] = new()
                {
                    ArVrXr = new TraitInput<ArVrXrType>(this, InputType.Attack, ArVrXrType.None),
                    Gamepad = new TraitInput<GamepadType>(this, InputType.Attack, GamepadType.Button_North),
                    Keyboard = new TraitInput<KeyboardType>(this, InputType.Attack, KeyboardType.Q)
                },
                [InputType.Crouch] = new()
                {
                    ArVrXr = new TraitInput<ArVrXrType>(this, InputType.Crouch, ArVrXrType.X),
                    Gamepad = new TraitInput<GamepadType>(this, InputType.Crouch, GamepadType.Button_West),
                    Keyboard = new TraitInput<KeyboardType>(this, InputType.Crouch, KeyboardType.Alt)
                },
                [InputType.Grab] = new()
                {
                    ArVrXr = new TraitInput<ArVrXrType>(this, InputType.Grab, ArVrXrType.Grip),
                    Gamepad = new TraitInput<GamepadType>(this, InputType.Grab, GamepadType.Left_Trigger),
                    Keyboard = new TraitInput<KeyboardType>(this, InputType.Grab, KeyboardType.R)
                },
                [InputType.Interact] = new()
                {
                    ArVrXr = new TraitInput<ArVrXrType>(this, InputType.Interact, ArVrXrType.Trigger),
                    Gamepad = new TraitInput<GamepadType>(this, InputType.Interact, GamepadType.Button_South),
                    Keyboard = new TraitInput<KeyboardType>(this, InputType.Interact, KeyboardType.E)
                },
                [InputType.Jump] = new()
                {
                    ArVrXr = new TraitInput<ArVrXrType>(this, InputType.Jump, ArVrXrType.A),
                    Gamepad = new TraitInput<GamepadType>(this, InputType.Jump, GamepadType.Button_East),
                    Keyboard = new TraitInput<KeyboardType>(this, InputType.Jump, KeyboardType.Space)
                },
                [InputType.Laser] = new()
                {
                    ArVrXr = new TraitInput<ArVrXrType>(this, InputType.Laser, ArVrXrType.B_Y),
                    Gamepad = new TraitInput<GamepadType>(this, InputType.Laser, GamepadType.Right_Trigger),
                    Keyboard = new TraitInput<KeyboardType>(this, InputType.Laser, KeyboardType.L)
                }
            };

            base.Initialize();

            EnforceMutualExclusivity<InputConfig>();

            AddTrait(data[InputType.Attack].Keyboard);
            AddTrait(data[InputType.Crouch].Keyboard);
            AddTrait(data[InputType.Grab].Keyboard);
            AddTrait(data[InputType.Interact].Keyboard);
            AddTrait(data[InputType.Jump].Keyboard);
            AddTrait(data[InputType.Laser].Keyboard);

            AddTrait(data[InputType.Attack].Gamepad);
            AddTrait(data[InputType.Crouch].Gamepad);
            AddTrait(data[InputType.Grab].Gamepad);
            AddTrait(data[InputType.Interact].Gamepad);
            AddTrait(data[InputType.Jump].Gamepad);
            AddTrait(data[InputType.Laser].Gamepad);

            AddTrait(data[InputType.Attack].ArVrXr);
            AddTrait(data[InputType.Crouch].ArVrXr);
            AddTrait(data[InputType.Grab].ArVrXr);
            AddTrait(data[InputType.Interact].ArVrXr);
            AddTrait(data[InputType.Jump].ArVrXr);
            AddTrait(data[InputType.Laser].ArVrXr);
        }

        /// <summary>
        /// Cleans up resources associated with the InputConfig when it is destroyed.
        /// This method clears the static reference to the singleton instance,
        /// allowing a new InputConfig to be created if needed after this one is destroyed.
        /// 
        /// Input actions created by the TraitInput instances are automatically disposed
        /// when their owning traits are destroyed by the base Asset class.
        /// </summary>
        protected override void Deinitialize()
        {
            s_self = null;
        }

        #endregion

        /// <summary>
        /// Generic trait implementation for input control mappings.
        /// Maps an input type (like attack, jump, etc.) to a specific control on a device.
        /// Creates and manages the actual InputAction that responds to user input.
        /// </summary>
        /// <typeparam name="T">The enum type representing the device-specific control</typeparam>
        private class TraitInput<T> : TraitAnonymous<T> where T : Enum
        {
            private readonly InputAction _inputAction;

            public TraitInput(Asset asset, InputType type, T value) : base(asset, GetName(type, value), value)
            {
                if (asset is not InputConfig input)
                {
                    Log.Error($"Urt3d {asset.Metadata.NameInstance} is not of type {typeof(InputConfig)}");
                    return;
                }

                _inputAction = new InputAction(type: InputActionType.Button);
                _inputAction.AddBinding(GetControlPath(value));
                _inputAction.performed += _ => input.OnInput?.Invoke(type);
                _inputAction.Enable();
            }

            protected override void OnSet(T value)
            {
                base.OnSet(value);

                _inputAction?.ChangeBinding(0).WithPath(GetControlPath(value));
            }

            private static string GetName(InputType input, T value)
            {
                var strType = input.ToString();
                var strValue = typeof(T).ToString();
                strValue = strValue.Replace("Type", "");
                return $"{strType} ({strValue})";
            }
        }
    }
}
