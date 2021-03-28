namespace Assets.Scripts
{
    /// <summary>
    /// Represents the logical outcome of pressing a key, allowing
    /// multiple physical keys to map to the same logical behavior.
    /// Authors: Scott Clarke and Daniel Darnell.
    /// </summary>
    public enum VirtualKey
    {
        RotateRight,
        RotateLeft,
        Accelerate,
        Brake,
        Up,
        Down,
        Left,
        Right,
        Primary,
        Secondary,
        Face,
        Teleport,
        Accept,
        Previous,
        Pause,
        Unpause,
        OpenUpgrade,
        CloseUpgrade,
    }
}