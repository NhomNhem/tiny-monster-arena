using TinyMonsterArena.Shared.Audio;
using UnityEngine;
using AudioType = TinyMonsterArena.Shared.Audio.AudioType;

namespace TinyMonsterArena.Application.Services.Interfaces {
    public interface IAudioService {
        /// <summary>
        /// Plays a sound effect globally (2D) or at a specific position (3D).
        /// </summary>
        void PlaySFX(AudioKey key, Vector3? position = null);

        /// <summary>
        /// Plays background music with an optional fade-in/crossfade.
        /// </summary>
        void PlayBGM(AudioKey key, float fadeDuration = 1.0f);

        /// <summary>
        /// Stops all audio of a specific type.
        /// </summary>
        void StopAll(AudioType type, float fadeDuration = 1.0f);

        /// <summary>
        /// Sets the volume for a specific audio group (0.0 to 1.0).
        /// </summary>
        void SetVolume(AudioType type, float volume);
    }
}