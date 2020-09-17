using StarBlue.Communication.WebSocket;

namespace StarBlue.Communication.Packets.Outgoing.WebSocket
{
    internal class BuilderToolComposer : WebComposer
    {
        public BuilderToolComposer(bool stack, string stackValue, bool state, int stateValue, bool rotation, int rotationValue) : base(7)
        {
            base.WriteBoolean(stack);
            base.WriteString(stackValue);
            base.WriteBoolean(state);
            base.WriteInteger(stateValue);
            base.WriteBoolean(rotation);
            base.WriteInteger(rotationValue);
        }
    }
}