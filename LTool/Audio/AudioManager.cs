/*======================================
* 项目名称 ：LitFramework
* 项目描述 ：
* 类 名 称 ：AudioManager
* 类 描 述 ：
* 命名空间 ：LitFramework
* 机器名称 ：SKY-20170413SEJ 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2018/5/7 15:23:37
* 更新时间 ：2018/5/7 15:23:37
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Liu Hanwen 2018. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2018/5/8 15:23:37
*修改人： LHW
*版本号： V1.0.0.0
*描述：
*
* 精简音频管理器功能
======================================*/


namespace LitFramework
{
    using UnityEngine;
    using System.Collections.Generic;
    using System;
    using LitFramework.Base;

    /// <summary>
    /// 音频组建更新
    /// 
    /// 创建时需要外传加载方法
    /// </summary>
    public class AudioManager : SingletonMono<AudioManager>,IManager
    {
        public bool IsEnabled { get; set; }
        public Func<string , AudioClip> LoadResourceFunc;
        private float _sevol = 1;
        private float _bgmvol = 1;

        /// <summary>
        /// 音效音量
        /// </summary>
        public float VolumeSE
        {
            set
            {
                _sevol = value;
                PlayerPrefs.SetFloat( "Setting_SE", value );
                if ( _soloAudioSource != null ) _soloAudioSource.volume = _sevol;
            }
            get { return _sevol; }
        }

        /// <summary>
        /// 音乐音量
        /// </summary>
        public float VolumeBGM
        {
            set
            {
                _bgmvol = value;
                PlayerPrefs.SetFloat( "Setting_BGM", value );
                if ( _audioBGM != null ) _audioBGM.volume = _bgmvol;
            }
            get { return _bgmvol; }
        }

        private List<AudioSource> _tempList;
        private AudioSource _audioBGM;
        private AudioSource _soloAudioSource;
        private Stack<AudioSource> _soundAvalibleList;
        private Dictionary<string , AudioClip> _audios;
        private Dictionary<string , AudioSource> _soundLoopPlayingDict;

        /// <summary>
        /// 启动音频模块
        /// </summary>
        /// <param name="loadFunction">提供音频加载方法</param>
        public void Install()
        {
            IsEnabled = true;

            _tempList = new List<AudioSource>();
            _audios = new Dictionary<string , AudioClip>();
            _soundAvalibleList = new Stack<AudioSource>();
            _soundLoopPlayingDict = new Dictionary<string , AudioSource>();

            _soloAudioSource = gameObject.AddComponent<AudioSource>();
            _audioBGM = gameObject.AddComponent<AudioSource>();
            _audioBGM.playOnAwake = false;

            VolumeBGM = PlayerPrefs.GetFloat( "Setting_BGM" , 1 );
            VolumeSE = PlayerPrefs.GetFloat( "Setting_SE" , 1 );
        }


        /// <summary>
        /// 卸载模块
        /// </summary>
        public void Uninstall()
        {
            if( gameObject != null )
            {
                LoadResourceFunc = null;
            }

            StopAllSE();
            _soundLoopPlayingDict = null;


            while( _soundAvalibleList.Count > 0 )
            {
                var ad = _soundAvalibleList.Pop();
                Destroy( ad );
                ad = null;
            }
            _soundAvalibleList = null;

            Destroy( _audioBGM );
            _audioBGM = null;

            Destroy( _soloAudioSource );
            _soloAudioSource = null;

            _tempList.Clear();
            _tempList = null;

            _audios.Clear();
            _audios = null;
        }



        private void LoadAudio( string name )
        {
            if( _audios.ContainsKey( name ) )
                return;

            //获取音频加载方法
            AudioClip clip = LoadResourceFunc( name );
            if( clip == null )
                throw new Exception( "未指定音频文件加载方法或音频文件路径指定错误 ==>" + name );
            _audios.Add( name , clip );
        }


        /// <summary>
        /// 获取音乐
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public AudioClip GetBGM( string name )
        {
            if( !_audios.ContainsKey( name ) )
            {
                LoadAudio( name );
            }
            else if( _audios[ name ] == null )
            {
                _audios.Remove( name );
                LoadAudio( name );
            }
            return _audios[ name ];
        }


        /// <summary>
        /// 获取音效
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public AudioClip GetSE( string name )
        {
            if( !_audios.ContainsKey( name ) )
            {
                LoadAudio( name );
            }
            else if( _audios[ name ] == null )
            {
                _audios.Remove( name );
                LoadAudio( name );
            }
            return _audios[ name ];
        }


        /// <summary>
        /// 播放音效
        /// 修改：
        /// 用栈管理多个音效组件，处理同时存在的多个持续存在的声音效果
        /// </summary>
        /// <param name="name">名称</param>
        public void PlaySE( string name , bool loop = false , bool isParallel = true , float volumeRate = 1f )
        {
            if ( string.IsNullOrEmpty( name ) || !IsEnabled ) 
                return;
            if( !_soundLoopPlayingDict.ContainsKey( name ) )
            {
                AudioSource currentAS = null;

                if( !isParallel )
                {
                    if( _soundAvalibleList.Count > 0 )
                    {
                        currentAS = _soundAvalibleList.Peek();
                        currentAS.Stop();
                    }
                    else
                    {
                        currentAS = gameObject.AddComponent<AudioSource>();
                        _soundAvalibleList.Push( currentAS );
                        PlaySE( name , loop , isParallel , volumeRate );
                        return;
                    }
                }
                //如果是平行播放音效，出栈查找
                else
                {
                    if( _soundAvalibleList.Count < 2 )
                    {
                        currentAS = gameObject.AddComponent<AudioSource>();
                        _soundAvalibleList.Push( currentAS );
                    }
                    else
                    {
                        bool hasOne = false;
                        int adNum = _soundAvalibleList.Count;

                        for( int i = 0; i < adNum; i++ )
                        {
                            var tp = _soundAvalibleList.Pop();
                            _tempList.Add( tp );
                            if( !tp.isPlaying )
                            {
                                hasOne = true;
                                currentAS = tp;
                                break;
                            }
                        }

                        for( int i = 0; i < _tempList.Count; i++ )
                        {
                            _soundAvalibleList.Push( _tempList[ i ] );
                        }

                        if( !hasOne )
                        {
                            currentAS = gameObject.AddComponent<AudioSource>();
                            _soundAvalibleList.Push( currentAS );
                        }

                        _tempList.Clear();
                    }
                }

                if( currentAS == null )
                    return;
                currentAS.playOnAwake = false;

                if( loop )
                {
                    currentAS.clip = GetSE( name );
                    currentAS.Play();
                    currentAS.volume = VolumeSE * volumeRate;
                    currentAS.loop = true;

                    _soundLoopPlayingDict.Add( name , currentAS );
                }
                else
                {
                    currentAS.clip = GetSE( name );
                    currentAS.volume = VolumeSE * volumeRate;
                    currentAS.loop = false;

                    currentAS.Play();
                }
            }
        }


        /// <summary>
        /// 停止持续播放的音效
        /// 只针对loop声效有效
        /// </summary>
        /// <param name="name"></param>
        public void StopSE( string name )
        {
            name = GetSourceName( name );
            
            foreach ( var item in _soundAvalibleList )
            {
                if ( item.clip.name == name )
                {
                    item.Stop();
                    break;
                }
            }

            if( _soundLoopPlayingDict.ContainsKey( name ) )
            {
                AudioSource currentSE = _soundLoopPlayingDict[ name ];
                currentSE.Stop();
                _soundAvalibleList.Push( currentSE );
                _soundLoopPlayingDict.Remove( name );
            }
        }

        /// <summary>
        /// 关闭全部音效
        /// </summary>
        public void StopAllSE()
        {
            //关闭循环播放音效
            List<string> nameList = new List<string>();
            foreach( var name in _soundLoopPlayingDict.Keys )
            {
                nameList.Add( name );
            }

            for( int i = 0; i < nameList.Count; i++ )
            {
                StopSE( nameList[ i ] );
            }

            nameList.Clear();
            nameList = null;

            //单次正在播放的音效关闭
            foreach( var item in _soundAvalibleList )
            {
                if( item != null )
                {
                    item.Stop();
                }
            }
        }


        public string currentMusicName = "";
        /// <summary>
        /// 播放音乐
        /// </summary>
        /// <param name="name"></param>
        /// <param name="loop"></param>
        public void PlayBGM( string name , bool loop = true )
        {
            if( string.IsNullOrEmpty( name ) || currentMusicName == name || !IsEnabled )
                return;

            currentMusicName = name;
            _audioBGM.clip = GetBGM( name );
            _audioBGM.Play();
            _audioBGM.volume = VolumeBGM;
            _audioBGM.loop = loop;

        }


        /// <summary>
        /// 暂停音乐
        /// </summary>
        public void PauseBGM()
        {
            if( _audioBGM.clip != null )
                _audioBGM.Pause();
        }


        /// <summary>
        /// 停止音乐
        /// </summary>
        public void StopBGM()
        {
            if( _audioBGM.clip != null )
                _audioBGM.Stop();
        }


        /// <summary>
        /// 继续播放音乐
        /// </summary>
        public void ResumeBGM()
        {
            if ( !IsEnabled ) return;

            if( _audioBGM.clip != null && !_audioBGM.isPlaying )
                _audioBGM.Play();
        }


        /// <summary>
        /// 侦听音量改变
        /// </summary>
        /// <param name="vol"></param>
        public void OnBGMValumeChange( float vol )
        {
            VolumeBGM = vol;
        }


        /// <summary>
        /// 侦听音乐改变
        /// </summary>
        /// <param name="vol"></param>
        public void OnSEValumeChange( float vol )
        {
            VolumeSE = vol;
        }


        /// <summary>
        /// 播放独立音效，单次且不可覆盖播放
        /// </summary>
        /// <param name="name"></param>
        public void PlaySoloSE( string name )
        {
            if( _soloAudioSource == null || _soloAudioSource.isPlaying || !IsEnabled )
                return;
            _soloAudioSource.PlayOneShot( GetSE( name ) , VolumeSE );
        }

        public void StopSoloSE( string name )
        {
            if ( _soloAudioSource == null || !_soloAudioSource.isPlaying || !IsEnabled )
                return;
            _soloAudioSource.Stop();
        }


        private string GetSourceName( string name )
        {
            if ( name.Contains( "/" ) )
            {
                var strings = name.Split( '/' );
                name = strings[ strings.Length - 1 ];
            }
            return name;
        }

    }
}
