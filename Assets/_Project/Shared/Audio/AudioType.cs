namespace TinyMonsterArena.Shared.Audio {
    public enum AudioType
    {
        BGM,
        SFX,
        UI,
        Ambient,
        Voice
    }

    /// <summary>
    /// Abstract identifier for audio assets.
    /// This keeps our logic decoupled from library-specific SoundIDs.
    /// </summary>
    public enum AudioKey
    {
        None = 0,
        
        // UI
        UI_Click,
        UI_Hover,
        
        // Gameplay
        SFX_Ragdoll_Hit,
        SFX_Ice_Slip,
        SFX_Revive,
        SFX_Struggle,
        
        // BGM
        BGM_Main_Menu,
        BGM_Ice_Palace,
        BGM_Time_Sanctuary
    }
}