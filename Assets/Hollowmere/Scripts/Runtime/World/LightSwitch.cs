namespace Hollowmere
{
    /// <summary>A room's light switch. The light only shines while the house has power.</summary>
    public class LightSwitch : Interactable
    {
        public Room Room;

        public override string Prompt(PlayerAgent player)
        {
            if (Room.House != null && !Room.House.Powered) return "The switch does nothing. No power.";
            return Room.SwitchOn ? "Turn the light off" : "Turn the light on";
        }

        public override bool CanInteract(PlayerAgent player) => Room.House == null || Room.House.Powered;

        public override void Interact(PlayerAgent player, InteractKind kind)
        {
            Room.SwitchOn = !Room.SwitchOn;
            Room.House.RefreshLights();
        }
    }
}
