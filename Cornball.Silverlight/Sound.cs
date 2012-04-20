using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Resources;
using Microsoft.Xna.Framework.Audio;

namespace Cornball
{
    public static class Sound
    {
        private static readonly Dictionary<Sounds, SoundEffectInstance> Sounds =
            new Dictionary<Sounds, SoundEffectInstance>();

        internal static void Play(Sounds sound)
        {
            if (!Sounds.ContainsKey(sound))
                Sounds.Add(sound, CreateInstance(sound.ToString()));

            if (Sounds[sound] != null && Settings.SoundEnabled)
                Sounds[sound].Play();
        }

        private static SoundEffectInstance CreateInstance(string resource)
        {
            var uri = new Uri(String.Format("Resources/Sounds/{0}.wav", resource), UriKind.RelativeOrAbsolute);
            StreamResourceInfo resourceStream = Application.GetResourceStream(uri);
            if (resourceStream != null)
            {
                SoundEffect soundEffect = SoundEffect.FromStream(resourceStream.Stream);
                if (soundEffect != null)
                    return soundEffect.CreateInstance();
            }
            return null;
        }
    }

    internal enum Sounds
    {
        Click,
        Moved,
        Correct,
        Error,
        Hint,
        RoundEnd,
        GameWon,
        GameLost,
        Shuffle
    }
}