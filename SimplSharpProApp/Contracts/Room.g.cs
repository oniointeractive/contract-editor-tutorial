using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace Response
{
    public interface IRoom
    {
        object UserObject { get; set; }

        event EventHandler<UIEventArgs> Opacity_Value;
        event EventHandler<UIEventArgs> Text;

        void Opacity_fb(RoomUShortInputSigDelegate callback);
        void Text_fb(RoomStringInputSigDelegate callback);

    }

    public delegate void RoomUShortInputSigDelegate(UShortInputSig uShortInputSig, IRoom room);
    public delegate void RoomStringInputSigDelegate(StringInputSig stringInputSig, IRoom room);

    internal class Room : IRoom, IDisposable
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
            internal static class Numerics
            {
                public const uint Opacity_Value = 1;

                public const uint Opacity_fb = 1;
            }
            internal static class Strings
            {
                public const uint Text = 1;

                public const uint Text_fb = 1;
            }
        }

        #endregion

        #region Construction and Initialization

        internal Room(ComponentMediator componentMediator, uint controlJoinId)
        {
            ComponentMediator = componentMediator;
            Initialize(controlJoinId);
        }

        private void Initialize(uint controlJoinId)
        {
            ControlJoinId = controlJoinId; 
 
            _devices = new List<BasicTriListWithSmartObject>(); 
 
            ComponentMediator.ConfigureNumericEvent(controlJoinId, Joins.Numerics.Opacity_Value, onOpacity_Value);
            ComponentMediator.ConfigureStringEvent(controlJoinId, Joins.Strings.Text, onText);

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

        public event EventHandler<UIEventArgs> Opacity_Value;
        private void onOpacity_Value(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = Opacity_Value;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }


        public void Opacity_fb(RoomUShortInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].UShortInput[Joins.Numerics.Opacity_fb], this);
            }
        }

        public event EventHandler<UIEventArgs> Text;
        private void onText(SmartObjectEventArgs eventArgs)
        {
            EventHandler<UIEventArgs> handler = Text;
            if (handler != null)
                handler(this, UIEventArgs.CreateEventArgs(eventArgs));
        }


        public void Text_fb(RoomStringInputSigDelegate callback)
        {
            for (int index = 0; index < Devices.Count; index++)
            {
                callback(Devices[index].SmartObjects[ControlJoinId].StringInput[Joins.Strings.Text_fb], this);
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
            return string.Format("Contract: {0} Component: {1} HashCode: {2} {3}", "Room", GetType().Name, GetHashCode(), UserObject != null ? "UserObject: " + UserObject : null);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            Opacity_Value = null;
            Text = null;
        }

        #endregion

    }
}
