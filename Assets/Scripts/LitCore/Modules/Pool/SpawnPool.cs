/// <Licensing>
/// � 2011 (Copyright) Path-o-logical Games, LLC
/// Licensed under the Unity Asset Package Product License (the "License");
/// You may not use this file except in compliance with the License.
/// You may obtain a copy of the License at: http://licensing.path-o-logical.com
/// </Licensing>
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitFramework;
using UnityEditor;
using Sirenix.OdinInspector;

namespace PathologicalGames
{
    [System.Serializable]
    public class SortSpawnPool
    {
        [Header( "对象池分类名称" )]
        public string SortSpawnName;
        [Header( "对象池集合" )]
        public List<PrefabPool> Pools;
    }

    /// <description>
    /// Online Docs: 
    ///     http://docs.poolmanager2.path-o-logical.com/code-reference/spawnpool
    ///     
    ///	A special List class that manages object pools and keeps the scene 
    ///	organized.
    ///	
    ///  * Only active/spawned instances are iterable. Inactive/despawned
    ///    instances in the pool are kept internally only.
    /// 
    ///	 * Instanciated objects can optionally be made a child of this GameObject
    ///	   (reffered to as a 'group') to keep the scene hierachy organized.
    ///		 
    ///	 * Instances will get a number appended to the end of their name. E.g. 
    ///	   an "Enemy" prefab will be called "Enemy(Clone)001", "Enemy(Clone)002", 
    ///	   "Enemy(Clone)003", etc. Unity names all clones the same which can be
    ///	   confusing to work with.
    ///		   
    ///	 * Objects aren't destroyed by the Despawn() method. Instead, they are
    ///	   deactivated and stored to an internal queue to be used again. This
    ///	   avoids the time it takes unity to destroy gameobjects and helps  
    ///	   performance by reusing GameObjects. 
    ///		   
    ///  * Two events are implimented to enable objects to handle their own reset needs. 
    ///    Both are optional.
    ///      1) When objects are Despawned BroadcastMessage("OnDespawned()") is sent.
    ///		 2) When reactivated, a BroadcastMessage("OnRespawned()") is sent. 
    ///		    This 
    /// </description>
    [AddComponentMenu( "Path-o-logical/PoolManager/SpawnPool" )]
    public abstract class SpawnPool : MonoBehaviour, IList<Transform>
    {
        #region Inspector Parameters
        /// <summary>
        /// Returns the name of this pool used by PoolManager. This will always be the
        /// same as the name in Unity, unless the name contains the work "Pool", which
        /// PoolManager will strip out. This is done so you can name a prefab or
        /// GameObject in a way that is development friendly. For example, "EnemiesPool" 
        /// is easier to understand than just "Enemies" when looking through a project.
        /// </summary>
        public string poolName = "";

        /// <summary>
        /// Matches new instances to the SpawnPool GameObject's scale.
        /// </summary>
        public bool matchPoolScale = false;

        /// <summary>
        /// Matches new instances to the SpawnPool GameObject's layer.
        /// </summary>
        public bool matchPoolLayer = false;

        /// <summary>
        /// If True, do not reparent instances under the SpawnPool's Transform.
        /// </summary>
        public bool dontReparent = false;

        /// <summary>
        /// If true, the Pool's group, GameObject, will be set to Unity's 
        /// Object.DontDestroyOnLoad()
        /// </summary>
        public bool dontDestroyOnLoad
        {
            get
            {
                return this._dontDestroyOnLoad;
            }

            set
            {
                this._dontDestroyOnLoad = value;

                if ( this.group != null )
                    Object.DontDestroyOnLoad( this.group.gameObject );
            }
        }
        public bool _dontDestroyOnLoad = false;  // Property backer and used by GUI.

        /// <summary>
        /// Print information to the Unity Console
        /// </summary>
        public bool logMessages = false;

        /// <summary>
        /// A list of PreloadDef options objects used by the inspector for user input
        /// </summary>
        //public List<PrefabPool> perPrefabPoolOptions = new List<PrefabPool>();

        public List<SortSpawnPool> perPrefabPoolOptions = new List<SortSpawnPool>();

        /// <summary>
        /// Used by the inspector to store this instances foldout states.
        /// </summary>
        public Dictionary<object, bool> prefabsFoldOutStates = new Dictionary<object, bool>();
        #endregion Inspector Parameters



        #region Public Code-only Parameters
        /// <summary>
        /// The time in seconds to stop waiting for particles to die.
        /// A warning will be logged if this is triggered.
        /// </summary>
        public float maxParticleDespawnTime = 300;

        /// <summary>
        /// The group is an empty game object which will be the parent of all
        /// instances in the pool. This helps keep the scene easy to work with.
        /// </summary>
        public Transform group { get; private set; }

        /// <summary>
        /// Returns the prefab of the given name (dictionary key)
        /// </summary>
        public PrefabsDict prefabs = new PrefabsDict();

        // Keeps the state of each individual foldout item during the editor session
        public Dictionary<object, bool> _editorListItemStates = new Dictionary<object, bool>();

        private Dictionary<string, PrefabPool> _prefabPoolDics;

        /// <summary>
        /// Readonly access to prefab pools via a dictionary<string, PrefabPool>.
        /// </summary>
        public Dictionary<string, PrefabPool> prefabPools
        {
            get
            {
                if ( _prefabPoolDics != null ) { return _prefabPoolDics; }
                _prefabPoolDics = new Dictionary<string, PrefabPool>();

                for ( int i = 0; i < _prefabPools.Count; i++ )
                    _prefabPoolDics.Add( _prefabPools[ i ].prefabGO.name, _prefabPools[ i ] );

                return _prefabPoolDics;
            }
        }
        #endregion Public Code-only Parameters



        #region Private Properties
        private List<PrefabPool> _prefabPools = new List<PrefabPool>();
        internal List<Transform> _spawned = new List<Transform>();
        #endregion Private Properties

        [Header("是否使用动态加载")]
        public bool useSpawnConfig = false;

        [EnableIf("useSpawnConfig")]
        [Header("动态加载方式")]
        public ResLoadType loadType = ResLoadType.Resource;

        #region Constructor and Init
        private void Awake()
        {
            // Make this GameObject immortal if the user requests it.
            if ( this._dontDestroyOnLoad ) Object.DontDestroyOnLoad( this.gameObject );

            this.group = this.transform;

            // Default name behavior will use the GameObject's name without "Pool" (if found)
            if ( this.poolName == "" )
            {
                // Automatically Remove "Pool" from names to allow users to name prefabs in a 
                //   more development-friendly way. E.g. "EnemiesPool" becomes just "Enemies".
                //   Notes: This will return the original string if "Pool" isn't found.
                //          Do this once here, rather than a getter, to avoide string work
                this.poolName = this.group.name.Replace( "Pool", "" );
                this.poolName = this.poolName.Replace( "(Clone)", "" );
            }

            if ( this.logMessages )
                Debug.Log( string.Format( "SpawnPool {0}: Initializing..", this.poolName ) );

            // Only used on items defined in the Inspector
            for ( int i = 0; i < this.perPrefabPoolOptions.Count; i++ )
            {
                //if (this.perPrefabPoolOptions[i].prefab == null)
                //{
                //    Debug.LogWarning(string.Format("Initialization Warning: Pool '{0}' " +
                //              "contains a PrefabPool with no prefab reference. Skipping.",
                //               this.poolName));
                //    continue;
                //}

                //// Init the PrefabPool's GameObject cache because it can't do it.
                ////   This is only needed when created by the inspector because the constructor
                ////   won't be run.
                //this.perPrefabPoolOptions[i].inspectorInstanceConstructor();
                //this.CreatePrefabPool(this.perPrefabPoolOptions[i]);
                if ( this.perPrefabPoolOptions[ i ].Pools == null )
                {
                    Debug.LogWarning( string.Format( "Initialization Warning: Pool '{0}' " +
                              "contains a PrefabPool with no prefab reference. Skipping.",
                               this.poolName ) );
                    continue;
                }
                List<PrefabPool> sorts = this.perPrefabPoolOptions[ i ].Pools;
                for ( int j = 0; j < sorts.Count; j++ )
                {
                    try
                    {
                        sorts[ j ].inspectorInstanceConstructor();
                        this.CreatePrefabPool( sorts[ j ] );
                    }
                    catch ( System.Exception )
                    {
                        LDebug.LogError( string.Format( "==>对象池预制件丢失!  池类型：{0}  Index: {1}", this.perPrefabPoolOptions[ i ].SortSpawnName, i ) );
                        throw;
                    }

                }

            }

            //perPrefabPoolOptions.Clear();
            //perPrefabPoolOptions = null;
            // Add this SpawnPool to PoolManager for use. This is done last to lower the 
            //   possibility of adding a badly init pool.
            PoolManager.Pools.Add( this );
        }

        public virtual void LoadSpawnConfig(  bool isHotFix = false ) { }

        /// <summary>
        /// Runs when this group GameObject is destroyed and executes clean-up
        /// </summary>
        private void OnDestroy()
        {
            if ( this.logMessages )
                Debug.Log( string.Format( "SpawnPool {0}: Destroying...", this.poolName ) );

            PoolManager.Pools.Remove( this );

            this.StopAllCoroutines();

            // We don't need the references to spawns which are about to be destroyed
            this._spawned.Clear();

            // Clean-up
            foreach ( PrefabPool pool in this._prefabPools ) pool.SelfDestruct();

            // Probably overkill, and may not do anything at all, but...
            this._prefabPools.Clear();
            this.prefabs._Clear();
        }


        /// <summary>
        /// Creates a new PrefabPool in this Pool and instances the requested 
        /// number of instances (set by PrefabPool.preloadAmount). If preload 
        /// amount is 0, nothing will be spawned and the return list will be empty.
        /// 
        /// It is rare this function is needed during regular usage.
        /// This function should only be used if you need to set the preferences
        /// of a new PrefabPool, such as culling or pre-loading, before use. Otherwise, 
        /// just use Spawn() and if the prefab is used for the first time a PrefabPool 
        /// will automatically be created with defaults anyway.
        /// 
        /// Note: Instances with ParticleEmitters can be preloaded too because 
        ///       it won't trigger the emmiter or the coroutine which waits for 
        ///       particles to die, which Spawn() does.
        ///       
        /// Usage Example:
        ///     // Creates a prefab pool and sets culling options but doesn't
        ///     //   need to spawn any instances (this is fine)
        ///     PrefabPool prefabPool = new PrefabPool()
        ///     prefabPool.prefab = myPrefabReference;
        ///     prefabPool.preloadAmount = 0;
        ///     prefabPool.cullDespawned = True;
        ///     prefabPool.cullAbove = 50;
        ///     prefabPool.cullDelay = 30;
        ///     
        ///     // Enemies is just an example. Any pool is fine.
        ///     PoolManager.Pools["Enemies"].CreatePrefabPool(prefabPool);
        ///     
        ///     // Then, just use as normal...
        ///     PoolManager.Pools["Enemies"].Spawn(myPrefabReference);
        /// </summary>
        /// <param name="prefabPool">A PrefabPool object</param>
        /// <returns>A List of instances spawned or an empty List</returns>
        public void CreatePrefabPool( PrefabPool prefabPool )
        {
            // Only add a PrefabPool once. Uses a GameObject comparison on the prefabs
            //   This will rarely be needed and will almost Always run at game start, 
            //   even if user-executed. This really only fails If a user tries to create 
            //   a PrefabPool using a prefab which already has a PrefabPool in the same
            //   SpawnPool. Either user created twice or PoolManager went first or even 
            //   second in cases where a user-script beats out PoolManager's init during 
            //   Awake();
            bool isAlreadyPool = this.GetPrefabPool( prefabPool.prefab ) == null ? false : true;
            // Used internally to reference back to this spawnPool for things 
            //   like anchoring co-routines.
            prefabPool.spawnPool = this;
            if ( !isAlreadyPool )
            {
                this._prefabPools.Add( prefabPool );

                // Add to the prefabs dict for convenience
                try
                {
                    this.prefabs._Add( prefabPool.prefab.name, prefabPool.prefab );
                }
                catch ( System.Exception )
                {
                    throw new System.Exception( string.Format( "对象池中已存在同名文件: {0} 。请检查配置表和对象池是否存在同名对象", prefabPool.prefab.name ) );
                }
            }

            // Preloading (uses a singleton bool to be sure this is only done once)
            if ( prefabPool.preloaded != true )
            {
                if ( this.logMessages )
                    Debug.Log( string.Format( "SpawnPool {0}: Preloading {1} {2}",
                                               this.poolName,
                                               prefabPool.preloadAmount,
                                               prefabPool.prefab.name ) );

                prefabPool.PreloadInstances();
            }
        }


        /// <summary>
        /// Add an existing instance to this pool. This is used during game start 
        /// to pool objects which are not instanciated at runtime
        /// </summary>
        /// <param name="instance">The instance to add</param>
        /// <param name="prefabName">
        /// The name of the prefab used to create this instance
        /// </param>
        /// <param name="despawn">True to depawn on start</param>
        /// <param name="parent">True to make a child of the pool's group</param>
        public void Add( Transform instance, string prefabName, bool despawn, bool parent )
        {
            for ( int i = 0; i < this._prefabPools.Count; i++ )
            {
                if ( this._prefabPools[ i ].prefabGO == null )
                {
                    Debug.LogError( "Unexpected Error: PrefabPool.prefabGO is null" );
                    return;
                }

                if ( this._prefabPools[ i ].prefabGO.name == prefabName )
                {
                    this._prefabPools[ i ].AddUnpooled( instance, despawn );

                    if ( this.logMessages )
                        Debug.Log( string.Format(
                                "SpawnPool {0}: Adding previously unpooled instance {1}",
                                                this.poolName,
                                                instance.name ) );

                    if ( parent ) instance.SetParent( this.group );

                    // New instances are active and must be added to the internal list 
                    if ( !despawn ) this._spawned.Add( instance );

                    return;
                }
            }

            // Log an error if a PrefabPool with the given name was not found
            Debug.LogError( string.Format( "SpawnPool {0}: PrefabPool {1} not found.",
                                         this.poolName,
                                         prefabName ) );

        }
        #endregion Constructor and Init



        #region List Overrides
        /// <summary>
        /// Not Implimented. Use Spawn() to properly add items to the pool.
        /// This is required because the prefab needs to be stored in the internal
        /// data structure in order for the pool to function properly. Items can
        /// only be added by instencing them using SpawnInstance().
        /// </summary>
        /// <param name="item"></param>
        public void Add( Transform item )
        {
            string msg = "Use SpawnPool.Spawn() to properly add items to the pool.";
            throw new System.NotImplementedException( msg );
        }


        /// <summary>
        /// Not Implimented. Use Despawn() to properly manage items that should remain 
        /// in the Queue but be deactivated. There is currently no way to safetly
        /// remove items from the pool permentantly. Destroying Objects would
        /// defeat the purpose of the pool.
        /// </summary>
        /// <param name="item"></param>
        public void Remove( Transform item )
        {
            string msg = "Use Despawn() to properly manage items that should " +
                         "remain in the pool but be deactivated.";
            throw new System.NotImplementedException( msg );
        }

        #endregion List Overrides



        #region Pool Functionality
        /// <description>
        ///	Spawns an instance or creates a new instance if none are available.
        ///	Either way, an instance will be set to the passed position and 
        ///	rotation.
        /// 
        /// Detailed Information:
        /// Checks the Data structure for an instance that was already created
        /// using the prefab. If the prefab has been used before, such as by
        /// setting it in the Unity Editor to preload instances, or just used
        /// before via this function, one of its instances will be used if one
        /// is available, or a new one will be created.
        /// 
        /// If the prefab has never been used a new PrefabPool will be started 
        /// with default options. 
        /// 
        /// To alter the options on a prefab pool, use the Unity Editor or see
        /// the documentation for the PrefabPool class and 
        /// SpawnPool.SpawnPrefabPool()
        ///		
        /// Broadcasts "OnSpawned" to the instance. Use this to manage states.
        ///		
        /// An overload of this function has the same initial signature as Unity's 
        /// Instantiate() that takes position and rotation. The return Type is different 
        /// though. Unity uses and returns a GameObject reference. PoolManager 
        /// uses and returns a Transform reference (or other supported type, such 
        /// as AudioSource and ParticleSystem)
        /// </description>
        /// <param name="prefab">
        /// The prefab used to spawn an instance. Only used for reference if an 
        /// instance is already in the pool and available for respawn. 
        /// NOTE: Type = Transform
        /// </param>
        /// <param name="pos">The position to set the instance to</param>
        /// <param name="rot">The rotation to set the instance to</param>
        /// <param name="parent">An optional parent for the instance</param>
        /// <returns>
        /// The instance's Transform. 
        /// 
        /// If the Limit option was used for the PrefabPool associated with the
        /// passed prefab, then this method will return null if the limit is
        /// reached. You DO NOT need to test for null return values unless you 
        /// used the limit option.
        /// </returns>
        public Transform Spawn( Transform prefab, Vector3 pos, Quaternion rot, Transform parent )
        {
            Transform inst;

            #region Use from Pool
            for ( int i = 0; i < this._prefabPools.Count; i++ )
            {
                // Determine if the prefab was ever used as explained in the docs
                //   I believe a comparison of two references is processor-cheap.
                if ( this._prefabPools[ i ].prefabGO == prefab.gameObject )
                {
                    // Now we know the prefabPool for this prefab exists. 
                    // Ask the prefab pool to setup and activate an instance.
                    // If there is no instance to spawn, a new one is instanced
                    inst = this._prefabPools[ i ].SpawnInstance( pos, rot );

                    // This only happens if the limit option was used for this
                    //   Prefab Pool.
                    if ( inst == null ) return null;

                    if ( parent != null )  // User explicitly provided a parent
                    {
                        inst.SetParent( parent );
                    }
                    else if ( !this.dontReparent && inst.parent != this.group )  // Auto organize?
                    {
                        // If a new instance was created, it won't be grouped
                        inst.SetParent( this.group );
                    }

                    // Add to internal list - holds only active instances in the pool
                    // 	 This isn't needed for Pool functionality. It is just done 
                    //	 as a user-friendly feature which has been needed before.
                    this._spawned.Add( inst );

                    // Notify instance it was spawned so it can manage it's state
                    inst.gameObject.BroadcastMessage(
                        "OnSpawned",
                        this,
                        SendMessageOptions.DontRequireReceiver
                    );

                    // Done!
                    return inst;
                }
            }
            #endregion Use from Pool


            #region New PrefabPool
            // The prefab wasn't found in any PrefabPools above. Make a new one
            PrefabPool newPrefabPool = new PrefabPool( prefab );
            this.CreatePrefabPool( newPrefabPool );

            // Spawn the new instance (Note: prefab already set in PrefabPool)
            inst = newPrefabPool.SpawnInstance( pos, rot );

            if ( parent != null )  // User explicitly provided a parent
            {
                inst.SetParent( parent );
            }
            else  // Auto organize
            {
                inst.SetParent( this.group );
            }


            // New instances are active and must be added to the internal list 
            this._spawned.Add( inst );
            #endregion New PrefabPool

            // Notify instance it was spawned so it can manage it's state
            inst.gameObject.BroadcastMessage(
                "OnSpawned",
                this,
                SendMessageOptions.DontRequireReceiver
            );

            // Done!
            return inst;
        }


        /// <summary>
        /// See primary Spawn method for documentation.
        /// </summary>
        public Transform Spawn( Transform prefab, Vector3 pos, Quaternion rot )
        {
            Transform inst = this.Spawn( prefab, pos, rot, null );

            // Can happen if limit was used
            if ( inst == null ) return null;

            return inst;
        }


        /// <summary>
        /// See primary Spawn method for documentation.
        /// 
        /// Overload to take only a prefab and instance using an 'empty' 
        /// position and rotation.
        /// </summary>
        public Transform Spawn( Transform prefab )
        {
            return this.Spawn( prefab, Vector3.zero, Quaternion.identity );
        }


        /// <summary>
        /// See primary Spawn method for documentation.
        /// 
        /// Convienince overload to take only a prefab  and parent the new 
        /// instance under the given parent
        /// </summary>
        public Transform Spawn( Transform prefab, Transform parent )
        {
            return this.Spawn( prefab, Vector3.zero, Quaternion.identity, parent );
        }


        /// <summary>
        /// See primary Spawn method for documentation.
        /// 
        /// Overload to take only a prefab name. The cached reference is pulled  
        /// from the SpawnPool.prefabs dictionary.
        /// </summary>
        public Transform Spawn( string prefabName )
        {
            Transform prefab = this.prefabs[ prefabName ];
            return this.Spawn( prefab );
        }


        /// <summary>
        /// See primary Spawn method for documentation.
        /// 
        /// Convienince overload to take only a prefab name and parent the new 
        /// instance under the given parent
        /// </summary>
        public Transform Spawn( string prefabName, Transform parent )
        {
            Transform prefab = this.prefabs[ prefabName ];
            return this.Spawn( prefab, parent );
        }


        /// <summary>
        /// See primary Spawn method for documentation.
        /// 
        /// Overload to take only a prefab name. The cached reference is pulled from 
        /// the SpawnPool.prefabs dictionary. An instance will be set to the passed 
        /// position and rotation.
        /// </summary>
        public Transform Spawn( string prefabName, Vector3 pos, Quaternion rot )
        {
            Transform prefab = this.prefabs[ prefabName ];
            return this.Spawn( prefab, pos, rot );
        }


        /// <summary>
        /// See primary Spawn method for documentation.
        /// 
        /// Convienince overload to take only a prefab name and parent the new 
        /// instance under the given parent. An instance will be set to the passed 
        /// position and rotation.
        /// </summary>
        public Transform Spawn( string prefabName, Vector3 pos, Quaternion rot,
                               Transform parent )
        {
            Transform prefab = this.prefabs[ prefabName ];
            return this.Spawn( prefab, pos, rot, parent );
        }


        public AudioSource Spawn( AudioSource prefab,
                            Vector3 pos, Quaternion rot )
        {
            return this.Spawn( prefab, pos, rot, null );  // parent = null
        }


        public AudioSource Spawn( AudioSource prefab )
        {
            return this.Spawn
            (
                prefab,
                Vector3.zero, Quaternion.identity,
                null  // parent = null
            );
        }


        public AudioSource Spawn( AudioSource prefab, Transform parent )
        {
            return this.Spawn
            (
                prefab,
                Vector3.zero,
                Quaternion.identity,
                parent
            );
        }


        public AudioSource Spawn( AudioSource prefab,
                                 Vector3 pos, Quaternion rot,
                                 Transform parent )
        {
            // Instance using the standard method before doing particle stuff
            Transform inst = Spawn( prefab.transform, pos, rot, parent );

            // Can happen if limit was used
            if ( inst == null ) return null;

            // Get the emitter and start it
            var src = inst.GetComponent<AudioSource>();
            src.Play();

            this.StartCoroutine( this.ListForAudioStop( src ) );

            return src;
        }


        /// <summary>
        ///	See docs for SpawnInstance(Transform prefab, Vector3 pos, Quaternion rot)
        ///	for basic functionalty information.
        ///		
        /// Pass a ParticleSystem component of a prefab to instantiate, trigger 
        /// emit, then listen for when all particles have died to "auto-destruct", 
        /// but instead of destroying the game object it will be deactivated and 
        /// added to the pool to be reused.
        /// 
        /// IMPORTANT: 
        ///     * You must pass a ParticleSystem next time as well, or the emitter
        ///       will be treated as a regular prefab and simply activate, but emit
        ///       will not be triggered!
        ///     * The listner that waits for the death of all particles will 
        ///       time-out after a set number of seconds and log a warning. 
        ///       This is done to keep the developer aware of any unexpected 
        ///       usage cases. Change the public property "maxParticleDespawnTime"
        ///       to adjust this length of time.
        /// 
        /// Broadcasts "OnSpawned" to the instance. Use this instead of Awake()
        ///		
        /// This function has the same initial signature as Unity's Instantiate() 
        /// that takes position and rotation. The return Type is different though.
        /// </summary>
        public ParticleSystem Spawn( ParticleSystem prefab,
                                    Vector3 pos, Quaternion rot )
        {
            return Spawn( prefab, pos, rot, null );  // parent = null

        }

        /// <summary>
        /// See primary Spawn ParticleSystem method for documentation.
        /// 
        /// Convienince overload to take only a prefab name and parent the new 
        /// instance under the given parent. An instance will be set to the passed 
        /// position and rotation.
        /// </summary>
        public ParticleSystem Spawn( ParticleSystem prefab,
                                    Vector3 pos, Quaternion rot,
                                    Transform parent )
        {
            // Instance using the standard method before doing particle stuff
            Transform inst = this.Spawn( prefab.transform, pos, rot, parent );

            // Can happen if limit was used
            if ( inst == null ) return null;

            // Get the emitter and start it
            var emitter = inst.GetComponent<ParticleSystem>();
            //emitter.Play(true);  // Seems to auto-play on activation so this may not be needed

            this.StartCoroutine( this.ListenForEmitDespawn( emitter ) );

            return emitter;
        }




        /// <summary>
        ///	If the passed object is managed by the SpawnPool, it will be 
        ///	deactivated and made available to be spawned again.
        ///		
        /// Despawned instances are removed from the primary list.
        /// </summary>
        /// <param name="item">The transform of the gameobject to process</param>
        public void Despawn( Transform instance )
        {
            // Find the item and despawn it
            bool despawned = false;
            for ( int i = 0; i < this._prefabPools.Count; i++ )
            {
                if ( this._prefabPools[ i ]._spawned.Contains( instance ) )
                {
                    despawned = this._prefabPools[ i ].DespawnInstance( instance );
                    break;
                }  // Protection - Already despawned?
                else if ( this._prefabPools[ i ]._despawned.Contains( instance ) )
                {
                    //Debug.LogError(
                    //    string.Format("SpawnPool {0}: {1} has already been despawned. " +
                    //                   "You cannot despawn something more than once!",
                    //                    this.poolName,
                    //                    instance.name));
                    return;
                }
            }

            // If still false, then the instance wasn't found anywhere in the pool
            if ( !despawned )
            {
                Debug.LogError( string.Format( "SpawnPool {0}: {1} not found in SpawnPool",
                               this.poolName,
                               instance.name ) );
                return;
            }

            // Remove from the internal list. Only active instances are kept. 
            // 	 This isn't needed for Pool functionality. It is just done 
            //	 as a user-friendly feature which has been needed before.
            this._spawned.Remove( instance );
        }


        /// <summary>
        ///	See docs for Despawn(Transform instance) for basic functionalty information.
        ///		
        /// Convienince overload to provide the option to re-parent for the instance 
        /// just before despawn.
        /// </summary>
        public void Despawn( Transform instance, Transform parent )
        {
            instance.SetParent( parent );
            this.Despawn( instance );
        }


        /// <description>
        /// See docs for Despawn(Transform instance). This expands that functionality.
        ///   If the passed object is managed by this SpawnPool, it will be 
        ///   deactivated and made available to be spawned again.
        /// </description>
        /// <param name="item">The transform of the instance to process</param>
        /// <param name="seconds">The time in seconds to wait before despawning</param>
        public void Despawn( Transform instance, float seconds )
        {
            this.StartCoroutine( this.DoDespawnAfterSeconds( instance, seconds, false, null ) );
        }


        /// <summary>
        ///	See docs for Despawn(Transform instance) for basic functionalty information.
        ///		
        /// Convienince overload to provide the option to re-parent for the instance 
        /// just before despawn.
        /// </summary>
        public void Despawn( Transform instance, float seconds, Transform parent )
        {
            this.StartCoroutine( this.DoDespawnAfterSeconds( instance, seconds, true, parent ) );
        }


        /// <summary>
        /// Waits X seconds before despawning. See the docs for DespawnAfterSeconds()
        /// the argument useParent is used because a null parent is valid in Unity. It will 
        /// make the scene root the parent
        /// </summary>
        private IEnumerator DoDespawnAfterSeconds( Transform instance, float seconds, bool useParent, Transform parent )
        {
            GameObject go = instance.gameObject;
            while ( seconds > 0 )
            {
                yield return null;

                // If the instance was deactivated while waiting here, just quit
                if ( !go.activeInHierarchy )
                    yield break;

                seconds -= Time.deltaTime;
            }

            if ( useParent )
                this.Despawn( instance, parent );
            else
                this.Despawn( instance );
        }


        /// <description>
        /// Despawns all active instances in this SpawnPool
        /// </description>
        public void DespawnAll()
        {
            var spawned = new List<Transform>( this._spawned );
            for ( int i = 0; i < spawned.Count; i++ )
                this.Despawn( spawned[ i ] );
        }


        /// <description>
        ///	Returns true if the passed transform is currently spawned.
        /// </description>
        /// <param name="item">The transform of the gameobject to test</param>
        public bool IsSpawned( Transform instance )
        {
            return this._spawned.Contains( instance );
        }

        #endregion Pool Functionality



        #region Utility Functions
        /// <summary>
        /// Returns the prefab pool for a given prefab.
        /// </summary>
        /// <param name="prefab">The Transform of an instance</param>
        /// <returns>PrefabPool</returns>
        public PrefabPool GetPrefabPool( Transform prefab )
        {
            for ( int i = 0; i < this._prefabPools.Count; i++ )
            {
                if ( this._prefabPools[ i ].prefabGO == null )
                    Debug.LogError( string.Format( "SpawnPool {0}: PrefabPool.prefabGO is null",
                                                 this.poolName ) );

                if ( this._prefabPools[ i ].prefabGO == prefab.gameObject )
                    return this._prefabPools[ i ];
            }

            // Nothing found
            return null;
        }


        /// <summary>
        /// Returns the prefab pool for a given prefab.
        /// </summary>
        /// <param name="prefab">The GameObject of an instance</param>
        /// <returns>PrefabPool</returns>
        public PrefabPool GetPrefabPool( GameObject prefab )
        {
            for ( int i = 0; i < this._prefabPools.Count; i++ )
            {
                if ( this._prefabPools[ i ].prefabGO == null )
                    Debug.LogError( string.Format( "SpawnPool {0}: PrefabPool.prefabGO is null",
                                                 this.poolName ) );

                if ( this._prefabPools[ i ].prefabGO == prefab )
                    return this._prefabPools[ i ];
            }

            // Nothing found
            return null;
        }


        /// <summary>
        /// Returns the prefab used to create the passed instance. 
        /// This is provided for convienince as Unity doesn't offer this feature.
        /// </summary>
        /// <param name="instance">The Transform of an instance</param>
        /// <returns>Transform</returns>
        public Transform GetPrefab( Transform instance )
        {
            for ( int i = 0; i < this._prefabPools.Count; i++ )
                if ( this._prefabPools[ i ].Contains( instance ) )
                    return this._prefabPools[ i ].prefab;

            // Nothing found
            return null;
        }


        /// <summary>
        /// Returns the prefab used to create the passed instance. 
        /// This is provided for convienince as Unity doesn't offer this feature.
        /// </summary>
        /// <param name="instance">The GameObject of an instance</param>
        /// <returns>GameObject</returns>
        public GameObject GetPrefab( GameObject instance )
        {
            for ( int i = 0; i < this._prefabPools.Count; i++ )
                if ( this._prefabPools[ i ].Contains( instance.transform ) )
                    return this._prefabPools[ i ].prefabGO;

            // Nothing found
            return null;
        }


        private IEnumerator ListForAudioStop( AudioSource src )
        {
            // Safer to wait a frame before testing if playing.
            yield return null;

            while ( src.isPlaying )
                yield return null;

            this.Despawn( src.transform );
        }



        // ParticleSystem (Shuriken) Version...
        private IEnumerator ListenForEmitDespawn( ParticleSystem emitter )
        {
            // Wait for the delay time to complete
            // Waiting the extra frame seems to be more stable and means at least one 
            //  frame will always pass
            yield return new WaitForSeconds( emitter.startDelay + 0.25f );

            // Do nothing until all particles die or the safecount hits a max value
            float safetimer = 0;   // Just in case! See Spawn() for more info
            while ( emitter.IsAlive( true ) )
            {
                if ( !PoolManagerUtils.activeInHierarchy( emitter.gameObject ) )
                {
                    emitter.Clear( true );
                    yield break;  // Do nothing, already despawned. Quit.
                }

                safetimer += Time.deltaTime;
                if ( safetimer > this.maxParticleDespawnTime )
                    Debug.LogWarning
                    (
                        string.Format
                        (
                            "SpawnPool {0}: " +
                                "Timed out while listening for all particles to die. " +
                                "Waited for {1}sec.",
                            this.poolName,
                            this.maxParticleDespawnTime
                        )
                    );

                yield return null;
            }

            // Turn off emit before despawning
            //emitter.Clear(true);
            this.Despawn( emitter.transform );
        }

        #endregion Utility Functions



        /// <summary>
        /// Returns a formatted string showing all the spawned member names
        /// </summary>
        public override string ToString()
        {
            // Get a string[] array of the keys for formatting with join()
            var name_list = new List<string>();
            foreach ( Transform item in this._spawned )
                name_list.Add( item.name );

            // Return a comma-sperated list inside square brackets (Pythonesque)
            return System.String.Join( ", ", name_list.ToArray() );
        }


        /// <summary>
        /// Read-only index access. You can still modify the instance at the given index.
        /// Read-only reffers to setting an index to a new instance reference, which would
        /// change the list. Setting via index is never needed to work with index access.
        /// </summary>
        /// <param name="index">int address of the item to get</param>
        /// <returns></returns>
        public Transform this[ int index ]
        {
            get { return this._spawned[ index ]; }
            set { throw new System.NotImplementedException( "Read-only." ); }
        }

        /// <summary>
        /// The name "Contains" is misleading so IsSpawned was implimented instead.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains( Transform item )
        {
            string message = "Use IsSpawned(Transform instance) instead.";
            throw new System.NotImplementedException( message );
        }


        /// <summary>
        /// Used by OTHERList.AddRange()
        /// This adds this list to the passed list
        /// </summary>
        /// <param name="array">The list AddRange is being called on</param>
        /// <param name="arrayIndex">
        /// The starting index for the copy operation. AddRange seems to pass the last index.
        /// </param>
        public void CopyTo( Transform[] array, int arrayIndex )
        {
            this._spawned.CopyTo( array, arrayIndex );
        }


        /// <summary>
        /// Returns the number of items in this (the collection). Readonly.
        /// </summary>
        public int Count
        {
            get { return this._spawned.Count; }
        }


        /// <summary>
        /// Impliments the ability to use this list in a foreach loop
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Transform> GetEnumerator()
        {
            for ( int i = 0; i < this._spawned.Count; i++ )
                yield return this._spawned[ i ];
        }

        /// <summary>
        /// Impliments the ability to use this list in a foreach loop
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            for ( int i = 0; i < this._spawned.Count; i++ )
                yield return this._spawned[ i ];
        }

        // Not implemented
        public int IndexOf( Transform item ) { throw new System.NotImplementedException(); }
        public void Insert( int index, Transform item ) { throw new System.NotImplementedException(); }
        public void RemoveAt( int index ) { throw new System.NotImplementedException(); }
        public void Clear() { throw new System.NotImplementedException(); }
        public bool IsReadOnly { get { throw new System.NotImplementedException(); } }
        bool ICollection<Transform>.Remove( Transform item ) { throw new System.NotImplementedException(); }

    }



}
