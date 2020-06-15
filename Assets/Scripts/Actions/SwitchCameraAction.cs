using System;
using Events.Runtime;

namespace Actions
{
    public class SwitchCameraAction : IEvent
    {
        public Type[] DispatchAs { get; }

        public SwitchCameraAction()
        {
            DispatchAs = new[] {typeof(SwitchCameraAction)};
        }
    }
}