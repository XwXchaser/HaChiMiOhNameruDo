using UnityEngine;

namespace HaChiMiOhNameruDo.Managers
{
    /// <summary>
    /// 音频管理器 - 单例模式
    /// 负责管理游戏音效和背景音乐
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("音效剪辑")]
        [SerializeField] private AudioClip cutSound;         // 碎纸声
        [SerializeField] private AudioClip pawSound;         // 拍击声
        [SerializeField] private AudioClip swipeSound;       // 划动声
        [SerializeField] private AudioClip gameStartSound;   // 游戏开始音效
        [SerializeField] private AudioClip gameEndSound;     // 游戏结束音效

        [Header("背景音乐")]
        [SerializeField] private AudioClip backgroundMusic;  // 背景音乐
        [SerializeField] private float musicVolume = 0.5f;   // 音乐音量
        [SerializeField] private float sfxVolume = 0.8f;     // 音效音量

        [Header("AudioSource 组件")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        private void Awake()
        {
            // 单例模式
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // 自动创建 AudioSource 组件
            if (musicSource == null)
                musicSource = gameObject.AddComponent<AudioSource>();
            if (sfxSource == null)
                sfxSource = gameObject.AddComponent<AudioSource>();

            // 配置 AudioSource
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.volume = musicVolume;

            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            sfxSource.volume = sfxVolume;
        }

        private void Start()
        {
            // 可选：自动播放背景音乐
            // PlayMusic();
        }

        #region 音效播放

        /// <summary>
        /// 播放碎纸音效
        /// </summary>
        public void PlayCutSound()
        {
            PlaySFX(cutSound);
        }

        /// <summary>
        /// 播放拍击音效
        /// </summary>
        public void PlayPawSound()
        {
            PlaySFX(pawSound);
        }

        /// <summary>
        /// 播放划动音效
        /// </summary>
        public void PlaySwipeSound()
        {
            PlaySFX(swipeSound);
        }

        /// <summary>
        /// 播放游戏开始音效
        /// </summary>
        public void PlayGameStartSound()
        {
            PlaySFX(gameStartSound);
        }

        /// <summary>
        /// 播放游戏结束音效
        /// </summary>
        public void PlayGameEndSound()
        {
            PlaySFX(gameEndSound);
        }

        /// <summary>
        /// 播放指定音效
        /// </summary>
        public void PlaySFX(AudioClip clip)
        {
            if (clip != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }

        #endregion

        #region 背景音乐控制

        /// <summary>
        /// 播放背景音乐
        /// </summary>
        public void PlayMusic()
        {
            if (backgroundMusic != null && musicSource != null)
            {
                musicSource.clip = backgroundMusic;
                musicSource.Play();
            }
        }

        /// <summary>
        /// 停止背景音乐
        /// </summary>
        public void StopMusic()
        {
            if (musicSource != null)
            {
                musicSource.Stop();
            }
        }

        /// <summary>
        /// 暂停背景音乐
        /// </summary>
        public void PauseMusic()
        {
            if (musicSource != null)
            {
                musicSource.Pause();
            }
        }

        /// <summary>
        /// 恢复背景音乐
        /// </summary>
        public void ResumeMusic()
        {
            if (musicSource != null)
            {
                musicSource.UnPause();
            }
        }

        /// <summary>
        /// 设置音乐音量
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            if (musicSource != null)
                musicSource.volume = musicVolume;
        }

        /// <summary>
        /// 设置音效音量
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            if (sfxSource != null)
                sfxSource.volume = sfxVolume;
        }

        #endregion
    }
}
