using System;

namespace UnityEngine.UI
{


    public static class UGUIDefaultConfig
    {
        /// <summary>
        /// 默认音量
        /// </summary>
        public static int DefaultVolume = 100;
        public static System.Func<string, AudioClip> GetAudioClicp;
        public static string DefaultAudioClipPath;
        /// <summary>
        /// 默认音频
        /// </summary>
        public static AudioClip DefaultAudioClip
        {
            get
            {
                if (GetAudioClicp != null)
                {
                    return GetAudioClicp.Invoke(DefaultAudioClipPath);
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 播放音效方法 音频，音效
        /// </summary>
        public static Action<AudioClip, int> PlayAudio;
    }
}
