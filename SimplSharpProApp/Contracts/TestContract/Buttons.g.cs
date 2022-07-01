using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace Response.TestContract
{
    public interface IButtons
    {
        object UserObject { get; set; }

        event EventHandler<UIEventArgs> Red_Pressed;
        event EventHandler<UIEventArgs> Green_Pressed;
        event EventHandler<UIEventArgs> Yellow_Pressed;

        void Red_Button_fb(ButtonsBoolInputSigDelegate callback);
        void Green_Button_fb(ButtonsBoolInputSigDelegate callback);
        void Yellow_Button_fb(ButtonsBoolInputSigDelegate callback);

    }

    public delegate void ButtonsBoolInputSigDelegate(BoolInputSig boolInputSig, IButtons buttons);

    /// <summary>
    /// Set of Buttons and their feedback
    /// </summary>
    internal class Buttons : IButtons, IDisposable
    {
        #region Standard CH5 Component members

        private ComponentMediator ComponentMediator { get; set; }

        public object UserObject { get; set; }

        public uint ControlJoinId { get; private set; }

        private IList<BasicTriListWithSmartObject> _devices;
        public IList<BasicTriListWithSmartObject> Devices { get { return _devices; } }

        #endregion

        #region Joins

        private static class Joins
        {
            internal static class Booleans
            {
                public const uint Red_Pressed = 1;
                public const uint Green_Pressed = 2;
                public const uint Yellow_Pressed = 3;

                public const uint Red_Button_fb = 1;
                public const uint Green_Button_fb = 2;
                public const uint Yellow_Button_fb = 3;
            }
        }

        #endregion

        #region Construction and Initialization

        internal Buttons(ComponentMediator componentMediator, uint controlJoinId)
        {
            ComponentMediator = componentMediator;
            Initialize(controlJoinId);
        }

        private void Initialize(uint controlJoinId)
        {
            ControlJoinId = controlJoinId; 
 
            _devices = new List<BasicTriListWithSmartObject>(); 
 
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Red_Pressed, onRed_Pressed);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Green_Pressed, onGreen_Pressed);
            ComponentMediator.ConfigureBooleanEvent(controlJoinId, Joins.Booleans.Yellow_Pressed, onYellow_Pressed);

        }

        public void AddDevice(BasicTriListWithSmartObject device)
        {
            Devices.Add(device);
            ComponentMediator.HookSmartObjectEvents(device.SmartObjects[ControlJoinId]);
        }

        public void RemoveDevice(BasicTriListWithSmartObject device)
        {
            Devices.Remove(device);
            ComponentMediator.UnHookSmartObjectEvents(device.SmartObjects[ControlJoinId]);
        }

        #endregion

        #region CH5 Contract

        public event EventHandler<UIEventArgs> Red_Pressed;
        private void onRed_Pressed(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = Red_Pressed;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> Green_Pressed;
        private void onGreen_Pressed(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = Green_Pressed;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }

        public event EventHandler<UIEventArgs> Yellow_Pressed;
        private void onYellow_Pressed(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = Yellow_Pressed;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }


        public void Red_Button_fb(ButtonsBoolInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.Red_Button_fb], this);
            }
        }

        public void Green_Button_fb(ButtonsBoolInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.Green_Button_fb], this);
            }
        }

        public void Yellow_Button_fb(ButtonsBoolInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].BooleanInput[Joins.Booleans.Yellow_Button_fb], this);
            }
        }

        #endregion

        #region Overrides

        public override int GetHashCode()
        {
            return (int)ControlJoinId;
        }

        public override string ToString()
        {
            return string.Format("Contract: {0} Component: {1} HashCode: {2} {3}", "Buttons", GetType().Name, GetHashCode(), UserObject != null ? "UserObject: " + UserObject : null);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            Red_Pressed = null;
            Green_Pressed = null;
            Yellow_Pressed = null;
        }

        #endregion

    }
}
