namespace StarBlue.Communication.Packets.Outgoing.Catalog
{
    internal class GetClubOffersMessageComposer : MessageComposer
    {
        public GetClubOffersMessageComposer()
            : base(Composers.GetClubOffersMessageComposer)
        {
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(0); // Dont know
            packet.WriteString("asd"); // Dont know
            packet.WriteBoolean(true); // Dont know
            packet.WriteInteger(75); // Resultado créditos  credits
            packet.WriteInteger(5); // Resultado extra    diamonds
            packet.WriteInteger(-1); // Dont know
            packet.WriteBoolean(true); // Alargar o Prolongar
            packet.WriteInteger(1); // Precio multiplicado
            packet.WriteInteger(1); // Dont know
            packet.WriteBoolean(true); // Activar moneda extra

            packet.WriteInteger(0); // Dont know
            packet.WriteInteger(0); // Dont know
            packet.WriteInteger(0); // Dont know
            packet.WriteInteger(0); // Dont know
            packet.WriteInteger(80); // Créditos
            packet.WriteInteger(5); // Extra
            packet.WriteInteger(105); // Tipo de Moneda
            packet.WriteInteger(1); // Dias disponible

        }
    }
}
