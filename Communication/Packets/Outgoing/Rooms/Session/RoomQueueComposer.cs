namespace StarBlue.Communication.Packets.Outgoing.Rooms.Session
{
    public class RoomQueueComposer : MessageComposer
    {
        private int UsersQueued { get; }

        public RoomQueueComposer(int UsersQueued)
            : base(Composers.RoomQueueComposer)
        {
            this.UsersQueued = UsersQueued;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(2); // 1: Solo Cola; 2: Cola con Opción Espectador
            {
                packet.WriteString("visitors");
                packet.WriteInteger(2); // 1: Cola para Modo Espectador; 2: Cola normal

                packet.WriteInteger(1);
                {
                    packet.WriteString("visitors");
                    packet.WriteInteger(UsersQueued); // tu puesto en la cola
                }

                packet.WriteString("spectators");
                packet.WriteInteger(1); // Bool para boton espectador

                packet.WriteInteger(1);
                {
                    packet.WriteString("spectators");
                    packet.WriteInteger(0); // specs?
                }
            }
        }
    }
}