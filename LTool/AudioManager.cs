﻿/*======================================
* 项目名称 ：LitFramework
* 项目描述 ：
* 类 名 称 ：AudioManager
* 类 描 述 ：
* 命名空间 ：LitFramework
* 机器名称 ：SKY-20170413SEJ 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：LHW
* 创建时间 ：2018/5/7 15:23:37
* 更新时间 ：2018/5/7 15:23:37
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ ShengYanTech 2018. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2018/5/7 15:23:37
*修改人： LHW
*版本号： V1.0.0.0
*描述：
*
======================================*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LitFramework
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Collections;
    using System;

    /// <summary>
    /// 音频组建更新
    /// </summary>
    class AudioManager : SingletonMono<AudioManager>
    {
        AudioSource _audioBGM;
        AudioSource _soloAudioSource;
        Stack<AudioSource> _soundAvalibleList;
        Dictionary<string , AudioSource> _soundLoopPlayingDict;
        Dictionary<string , AudioClip> _audios = new Dictionary<string , AudioClip>();
        List<AudioSource> _tempList = new List<AudioSource>();
        Func<string , AudioClip> LoadFunction;

        float _sevol = 1;
        float _bgmvol = 1;

        public float VolumeSE
        {
            set { _sevol = value; }
            get { return _sevol; }
        }

        public float VolumeBGM
        {
            set
            {
                _bgmvol = value;
                if( _audioBGM != null )
                    _audioBGM.volume = _bgmvol;
            }
            get { return _bgmvol; }
        }

        public void SetVolume( string config )
        {
            VolumeBGM = PlayerPrefs.GetFloat( config + "_BGM" , 1 );
            VolumeSE = PlayerPrefs.GetFloat( config + "_SE" , 1 );
        }

        public void Awake()
        {
            _audioBGM = gameObject.AddComponent<AudioSource>();
            _audioBGM.playOnAwake = false;

            _soloAudioSource = gameObject.AddComponent<AudioSource>();

            VolumeBGM = PlayerPrefs.GetFloat( "Setting_BGM" , 1 );
            VolumeSE = PlayerPrefs.GetFloat( "Setting_SE" , 1 );

            _soundAvalibleList = new Stack<AudioSource>();
            _soundLoopPlayingDict = new Dictionary<string , AudioSource>();
        }


        public void Install( Func<string, AudioClip> loadFunction )
        {
            LoadFunction = loadFunction;
        }
        
        
        
        public void LoadAudio( string name )
        {
            if( _audios.ContainsKey( name ) )
                return;

            //获取音频加载方法
            AudioClip clip = LoadFunction( name );
            _audios.Add( name , clip );
        }

        public IEnumerator LoadAudioAsync( string[] names )
        {
            for( int i = 0; i < names.Length; i++ )
            {
                if( !_audios.ContainsKey( names[ i ] ) )
                {
                    AudioClip clip = LoadFunction( name );
                    _audios.Add( names[ i ] , clip );
                }
                yield return 0;
            }


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
            if( string.IsNullOrEmpty( name ) )
                return;
            if( !_soundLoopPlayingDict.ContainsKey( name ) )
            {
                AudioSource currentAS = null;

                if( !isParallel )
                {
                    if( _soundAvalibleList.Count > 0 )
                        currentAS = _soundAvalibleList.Pop();
                    else
                    {
                        currentAS = gameObject.AddComponent<AudioSource>();
                        _soundAvalibleList.Push( currentAS );
                        PlaySE( name , loop );
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
                    currentAS.volume = _sevol * volumeRate;
                    currentAS.loop = true;

                    _soundLoopPlayingDict.Add( name , currentAS );
                }
                else
                {
                    currentAS.clip = null;
                    currentAS.volume = _sevol * volumeRate;
                    currentAS.loop = false;

                    currentAS.PlayOneShot( GetSE( name ) , _sevol * volumeRate );
                }
            }
        }



        /// <summary>
        /// 关闭所有类型音效
        /// </summary>
        public void StopAllKindsSE()
        {
            foreach( var item in _soundAvalibleList )
            {
                if( item != null )
                {
                    item.Stop();
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
            if( _soundLoopPlayingDict.ContainsKey( name ) )
            {
                AudioSource currentSE = _soundLoopPlayingDict[ name ];
                currentSE.Stop();
                _soundAvalibleList.Push( currentSE );
                _soundLoopPlayingDict.Remove( name );
            }
        }


        public void StopAllSE()
        {
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
        }


        public string currentMusicName = "";
        /// <summary>
        /// 播放音乐
        /// </summary>
        /// <param name="name"></param>
        /// <param name="loop"></param>
        public void PlayBGM( string name , bool loop = true )
        {
            if( string.IsNullOrEmpty( name ) || currentMusicName == name )
                return;

            currentMusicName = name;
            _audioBGM.clip = GetBGM( name );
            _audioBGM.Play();
            _audioBGM.volume = _bgmvol;
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


        public void ResumeBGM()
        {
            if( _audioBGM.clip != null )
                _audioBGM.Play();
        }

        public void OnBGMValumeChange( float vol )
        {
            VolumeBGM = vol;
        }
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
            if( _soloAudioSource == null || _soloAudioSource.isPlaying )
                return;
            _soloAudioSource.PlayOneShot( GetSE( name ) , _sevol );
        }

    }
}
