using NhemDangFugBixs.Attributes;
using TinyMonsterArena.Application.Services.Interfaces;
using TinyMonsterArena.Shared.Audio;
using UnityEngine;
using AudioType = TinyMonsterArena.Shared.Audio.AudioType;

namespace TinyMonsterArena.Infrastructure.Audio {
    public class BroAudioService : IAudioService {
        public void PlaySFX(AudioKey key, Vector3? position = null) {
        }
        public void PlayBGM(AudioKey key, float fadeDuration = 1) {
        }
        public void StopAll(AudioType type, float fadeDuration = 1) {
        }
        public void SetVolume(AudioType type, float volume) {
        }
    }
}