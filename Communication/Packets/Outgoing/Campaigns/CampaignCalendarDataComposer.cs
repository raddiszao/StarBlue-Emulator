namespace StarBlue.Communication.Packets.Outgoing.Campaigns
{
    internal class CampaignCalendarDataComposer : MessageComposer
    {
        private bool[] OpenedBoxes { get; }

        public CampaignCalendarDataComposer(bool[] OpenedBoxes)
            : base(Composers.CampaignCalendarDataMessageComposer)
        {
            this.OpenedBoxes = OpenedBoxes;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteString(StarBlueServer.GetGame().GetCalendarManager().GetCampaignName()); // NOMBRE DE LA CAMPAÑA.
            packet.WriteString("asd"); // NO TIENE FUNCIÓN EN LA SWF.
            packet.WriteInteger(StarBlueServer.GetGame().GetCalendarManager().GetUnlockDays()); // DÍAS ACTUAL (DESBLOQUEADOS).
            packet.WriteInteger(StarBlueServer.GetGame().GetCalendarManager().GetTotalDays()); // DÍAS TOTALES.
            int OpenedCount = 0;
            int LateCount = 0;

            for (int i = 0; i < OpenedBoxes.Length; i++)
            {
                if (OpenedBoxes[i])
                {
                    OpenedCount++;
                }
                else
                {
                    // DÍA ACTUAL (EVITAMOS)
                    if (StarBlueServer.GetGame().GetCalendarManager().GetUnlockDays() == i)
                    {
                        continue;
                    }

                    LateCount++;
                }
            }
            // CAJAS ABIERTAS HASTA EL MOMENTO.
            packet.WriteInteger(OpenedCount);
            for (int i = 0; i < OpenedBoxes.Length; i++)
            {
                if (OpenedBoxes[i])
                {
                    packet.WriteInteger(i);
                }
            }

            // CAJAS QUE SE HAN PASADO DE FECHA.
            packet.WriteInteger(LateCount);
            for (int i = 0; i < OpenedBoxes.Length; i++)
            {
                // DÍA ACTUAL (EVITAMOS)
                if (StarBlueServer.GetGame().GetCalendarManager().GetUnlockDays() == i)
                {
                    continue;
                }

                if (!OpenedBoxes[i])
                {
                    packet.WriteInteger(i);
                }
            }
        }
    }
}