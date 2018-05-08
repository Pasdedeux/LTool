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
    /// 
    /// 创建时需要外传加载方法
    /// </summary>
    class AudioManager : SingletonMono<AudioManager>
    {
        AudioSource _audioBGM;
        AudioSource _soloAudioSource;
        Stack<AudioSource> _soundAvalibleList;
        Func<string , AudioClip> LoadFunction;
        List<AudioSource> _tempList = new List<AudioSource>();
        Dictionary<string , AudioSource> _soundLoopPlayingDict;
        Dictionary<string , AudioClip> _audios = new Dictionary<string , AudioClip>();


        public float VolumeSE { get; set; }

        public float VolumeBGM { get; set; }


        public void Awake()
        {
            _soundAvalibleList = new Stack<AudioSource>();
            _soundLoopPlayingDict = new Dictionary<string , AudioSource>();

            _audioBGM = gameObject.AddComponent<AudioSource>();
            _audioBGM.playOnAwake = false;

            _soloAudioSource = gameObject.AddComponent<AudioSource>();

            VolumeBGM = PlayerPrefs.GetFloat( "Setting_BGM" , 1 );
            VolumeSE = PlayerPrefs.GetFloat( "Setting_SE" , 1 );
        }


        /// <summary>
        /// 启动音频模块
        /// </summary>
        /// <param name="loadFunction">提供音频加载方法</param>
        public void Install( Func<string, AudioClip> loadFunction )
        {
            LoadFunction = loadFunction;
        }


        /// <summary>
        /// 卸载模块
        /// </summary>
        public void Uninstall()
        {
            if( gameObject != null )
            {
                LoadFunction = null;
            }

            while( _soundAvalibleList.Count>0 )
            {
                var ad = _soundAvalibleList.Pop();
                Destroy( ad );
                ad = null;
            }
            _soundAvalibleList = null;

            StopAllSE();
            _soundLoopPlayingDict = null;

            Destroy( _audioBGM );
            _audioBGM = null;

            Destroy( _soloAudioSource );
            _soloAudioSource = null;
        }
        
        
        
        private void LoadAudio( string name )
        {
            if( _audios.ContainsKey( name ) )
                return;

            //获取音频加载方法
            AudioClip clip = LoadFunction( name );
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
                    currentAS.volume = VolumeSE * volumeRate;
                    currentAS.loop = true;

                    _soundLoopPlayingDict.Add( name , currentAS );
                }
                else
                {
                    currentAS.clip = null;
                    currentAS.volume = VolumeSE * volumeRate;
                    currentAS.loop = false;

                    currentAS.PlayOneShot( GetSE( name ) , VolumeSE * volumeRate );
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
            if( string.IsNullOrEmpty( name ) || currentMusicName == name )
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
            if( _audioBGM.clip != null )
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
            if( _soloAudioSource == null || _soloAudioSource.isPlaying )
                return;
            _soloAudioSource.PlayOneShot( GetSE( name ) , VolumeSE );
        }
    }
}
