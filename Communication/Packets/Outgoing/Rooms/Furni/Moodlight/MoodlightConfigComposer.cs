
using StarBlue.HabboHotel.Items.Data.Moodlight;

namespace StarBlue.Communication.Packets.Outgoing.Rooms.Furni.Moodlight
{
    internal class MoodlightConfigComposer : MessageComposer
    {
        public MoodlightData Data { get; }

        public MoodlightConfigComposer(MoodlightData MoodlightData)
            : base(Composers.MoodlightConfigMessageComposer)
        {
            this.Data = MoodlightData;
        }

        public override void Compose(Composer packet)
        {
            packet.WriteInteger(Data.Presets.Count);
            packet.WriteInteger(Data.CurrentPreset);

            int i = 1;
            foreach (MoodlightPreset Preset in Data.Presets)
            {
                packet.WriteInteger(i);
                packet.WriteInteger(Preset.BackgroundOnly ? 2 : 1);
                packet.WriteString(Preset.ColorCode);
                packet.WriteInteger(Preset.ColorIntensity);
                i++;
            }
        }
    }
}