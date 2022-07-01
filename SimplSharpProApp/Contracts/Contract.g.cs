using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace Response
{
    /// <summary>
    /// Common Interface for Root Contracts.
    /// </summary>
    public interface IContract
    {
        object UserObject { get; set; }
        void AddDevice(BasicTriListWithSmartObject device);
        void RemoveDevice(BasicTriListWithSmartObject device);
    }

    /// <summary>
    /// Whole system
    /// </summary>
    public class Contract : IContract, IDisposable
    {
        #region Components

        private ComponentMediator ComponentMediator { get; set; }

        public Response.IRoom Room { get { return (Response.IRoom)InternalRoom; } }
        private Response.Room InternalRoom { get; set; }

        public Response.TestContract.IButtons Buttons { get { return (Response.TestContract.IButtons)InternalButtons; } }
        private Response.TestContract.Buttons InternalButtons { get; set; }

        #endregion

        #region Construction and Initialization

        public Contract()
            : this(new List<BasicTriListWithSmartObject>().ToArray())
        {
        }

        public Contract(BasicTriListWithSmartObject device)
            : this(new [] { device })
        {
        }

        public Contract(BasicTriListWithSmartObject[] devices)
        {
            if (devices == null)
                throw new ArgumentNullException("Devices is null");

            ComponentMediator = new ComponentMediator();

            InternalRoom = new Response.Room(ComponentMediator, 1);
            InternalButtons = new Response.TestContract.Buttons(ComponentMediator, 2);

            for (int index = 0; index < devices.Length; index++)
            {
                AddDevice(devices[index]);
            }
        }

        #endregion

        #region Standard Contract Members

        public object UserObject { get; set; }

        public void AddDevice(BasicTriListWithSmartObject device)
        {
            InternalRoom.AddDevice(device);
            InternalButtons.AddDevice(device);
        }

        public void RemoveDevice(BasicTriListWithSmartObject device)
        {
            InternalRoom.RemoveDevice(device);
            InternalButtons.RemoveDevice(device);
        }

        #endregion

        #region IDisposable

        public bool IsDisposed { get; set; }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            IsDisposed = true;

            InternalRoom.Dispose();
            InternalButtons.Dispose();
            ComponentMediator.Dispose(); 
        }

        #endregion

    }
}
