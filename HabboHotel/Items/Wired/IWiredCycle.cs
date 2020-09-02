namespace StarBlue.HabboHotel.Items.Wired
{
    internal interface IWiredCycle
    {
        int Delay { get; set; }
        int TickCount { get; set; }
        bool OnCycle();
    }
}
