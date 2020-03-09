/*======================================
* 项目名称 ：LitFramework.LitPool
* 项目描述 ：
* 类 名 称 ：PrefabPool
* 类 描 述 ：
* 命名空间 ：LitFramework.LitPool
* 机器名称 ：DEREK-SURFACEPR 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Derek Liu
* 创建时间 ：2019/9/5 8:14:44
* 更新时间 ：2019/9/5 8:14:44
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Inplayable 2019. All rights reserved.
*******************************************************************

-------------------------------------------------------------------
*Fix Note:
*修改时间：2019/9/5 8:14:44
*修改人： Derek Liu
*版本号： V1.0.0.0
*描述：
*
======================================*/

using PathologicalGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// This class is used to display a more complex user entry interface in the 
/// Unity Editor so we can collect more options related to each Prefab.
/// 
/// See this class documentation for Editor Options.
/// 
/// This class is also the primary pool functionality for SpawnPool. SpawnPool
/// manages the Pool using these settings and methods. See the SpawnPool 
/// documentation for user documentation and usage.
/// </summary>

namespace LitFramework
{

    [System.Serializable]
    public class PrefabPool : MonoBehaviour
    {

        #region Public Properties Available in the Editor
        /// <summary>
        /// The prefab to preload
        /// </summary>
        public Transform prefab;

        /// <summary>
        /// A reference of the prefab's GameObject stored for performance reasons
        /// </summary>
        internal GameObject prefabGO;  // Hidden in inspector, but not Debug tab

        /// <summary>
        /// The number of instances to preload
        /// </summary>
        public int preloadAmount = 1;

        /// <summary>
        /// Displays the 'preload over time' options
        /// </summary>
        public bool preloadTime = false;

        /// <summary>
        /// The number of frames it will take to preload all requested instances
        /// </summary>
        public int preloadFrames = 2;

        /// <summary>
        /// The number of seconds to wait before preloading any instances
        /// </summary>
        public float preloadDelay = 0;

        /// <summary>
        /// Limits the number of instances allowed in the game. Turning this ON
        ///	means when 'Limit Amount' is hit, no more instances will be created.
        /// CALLS TO SpawnPool.Spawn() WILL BE IGNORED, and return null!
        ///
        /// This can be good for non-critical objects like bullets or explosion
        ///	Flares. You would never want to use this for enemies unless it makes
        ///	sense to begin ignoring enemy spawns in the context of your game.
        /// </summary>
        public bool limitInstances = false;

        /// <summary>
        /// This is the max number of instances allowed if 'limitInstances' is ON.
        /// </summary>
        public int limitAmount = 100;

        /// <summary>
        /// FIFO stands for "first-in-first-out". Normally, limiting instances will
        /// stop spawning and return null. If this is turned on (set to true) the
        /// first spawned instance will be despawned and reused instead, keeping the
        /// total spawned instances limited but still spawning new instances.
        /// </summary>
        public bool limitFIFO = false;  // Keep after limitAmount for auto-inspector

        /// <summary>
        /// Turn this ON to activate the culling feature for this Pool. 
        /// Use this feature to remove despawned (inactive) instances from the pool
        /// if the size of the pool grows too large. 
        ///	
        /// DO NOT USE THIS UNLESS YOU NEED TO MANAGE MEMORY ISSUES!
        /// This should only be used in extreme cases for memory management. 
        /// For most pools (or games for that matter), it is better to leave this 
        /// off as memory is more plentiful than performance. If you do need this
        /// you can fine tune how often this is triggered to target extreme events.
        /// 
        /// A good example of when to use this would be if you you are Pooling 
        /// projectiles and usually never need more than 10 at a time, but then
        /// there is a big one-off fire-fight where 50 projectiles are needed. 
        /// Rather than keep the extra 40 around in memory from then on, set the 
        /// 'Cull Above' property to 15 (well above the expected max) and the Pool 
        /// will Destroy() the extra instances from the game to free up the memory. 
        /// 
        /// This won't be done immediately, because you wouldn't want this culling 
        /// feature to be fighting the Pool and causing extra Instantiate() and 
        /// Destroy() calls while the fire-fight is still going on. See 
        /// "Cull Delay" for more information about how to fine tune this.
        /// </summary>
        public bool cullDespawned = false;

        /// <summary>
        /// The number of TOTAL (spawned + despawned) instances to keep. 
        /// </summary>
        public int cullAbove = 50;

        /// <summary>
        /// The amount of time, in seconds, to wait before culling. This is timed 
        /// from the moment when the Queue's TOTAL count (spawned + despawned) 
        /// becomes greater than 'Cull Above'. Once triggered, the timer is repeated 
        /// until the count falls below 'Cull Above'.
        /// </summary>
        public int cullDelay = 60;

        /// <summary>
        /// The maximum number of instances to destroy per this.cullDelay
        /// </summary>
        public int cullMaxPerPass = 5;

        /// <summary>
        /// Prints information during run-time to make debugging easier. This will 
        /// be set to true if the owner SpawnPool is true, otherwise the user's setting
        /// here will be used
        /// </summary>
        public bool _logMessages = false;  // Used by the inspector
        public bool logMessages            // Read-only
        {
            get
            {
                if ( forceLoggingSilent ) return false;

                if ( this.spawnPool.logMessages )
                    return this.spawnPool.logMessages;
                else
                    return this._logMessages;
            }
        }

        // Forces logging to be silent regardless of user settings.
        private bool forceLoggingSilent = false;


        /// <summary>
        /// Used internally to reference back to the owner spawnPool for things like
        /// anchoring co-routines.
        /// </summary>
        public SpawnPool spawnPool;
        #endregion Public Properties Available in the Editor


        #region Constructor and Self-Destruction
        /// <description>
        ///	Constructor to require a prefab Transform
        /// </description>
        public PrefabPool( Transform prefab )
        {
            this.prefab = prefab;
            this.prefabGO = prefab.gameObject;
        }

        /// <description>
        ///	Constructor for Serializable inspector use only
        /// </description>
        public PrefabPool() { }

        /// <description>
        ///	A pseudo constructor to init stuff not init by the serialized inspector-created
        ///	instance of this class.
        /// </description>
        internal void inspectorInstanceConstructor()
        {
            this.prefabGO = this.prefab.gameObject;
            this._spawned = new List<Transform>();
            this._despawned = new List<Transform>();
        }


        /// <summary>
        /// Run by a SpawnPool when it is destroyed
        /// </summary>
        internal void SelfDestruct()
        {
            // Probably overkill but no harm done
            this.prefab = null;
            this.prefabGO = null;
            this.spawnPool = null;

            // Go through both lists and destroy everything
            foreach ( Transform inst in this._despawned )
                if ( inst != null )
                    UnityEngine.Object.Destroy( inst.gameObject );

            foreach ( Transform inst in _spawned )
                if ( inst != null )
                    UnityEngine.Object.Destroy( inst.gameObject );

            this._spawned.Clear();
            this._despawned.Clear();
        }
        #endregion Constructor and Self-Destruction


        #region Pool Functionality
        /// <summary>
        /// Is set to true when the culling coroutine is started so another one
        /// won't be
        /// </summary>
        private bool cullingActive = false;


        /// <summary>
        /// The active instances associated with this prefab. This is the pool of
        /// instances created by this prefab.
        /// 
        /// Managed by a SpawnPool
        /// </summary>
        internal List<Transform> _spawned = new List<Transform>();
        public List<Transform> spawned { get { return new List<Transform>( this._spawned ); } }

        /// <summary>
        /// The deactive instances associated with this prefab. This is the pool of
        /// instances created by this prefab.
        /// 
        /// Managed by a SpawnPool
        /// </summary>
        internal List<Transform> _despawned = new List<Transform>();
        public List<Transform> despawned { get { return new List<Transform>( this._despawned ); } }


        /// <summary>
        /// Returns the total count of instances in the PrefabPool
        /// </summary>
        public int totalCount
        {
            get
            {
                // Add all the items in the pool to get the total count
                int count = 0;
                count += this._spawned.Count;
                count += this._despawned.Count;
                return count;
            }
        }


        /// <summary>
        /// Used to make PreloadInstances() a one-time event. Read-only.
        /// </summary>
        private bool _preloaded = false;
        internal bool preloaded
        {
            get { return this._preloaded; }
            private set { this._preloaded = value; }
        }


        /// <summary>
        /// Move an instance from despawned to spawned, set the position and 
        /// rotation, activate it and all children and return the transform
        /// </summary>
        /// <returns>
        /// True if successfull, false if xform isn't in the spawned list
        /// </returns>
        internal bool DespawnInstance( Transform xform )
        {
            return DespawnInstance( xform, true );
        }

        internal bool DespawnInstance( Transform xform, bool sendEventMessage )
        {
            if ( this.logMessages )
                Debug.Log( string.Format( "SpawnPool {0} ({1}): Despawning '{2}'",
                                       this.spawnPool.poolName,
                                       this.prefab.name,
                                       xform.name ) );

            // Switch to the despawned list
            this._spawned.Remove( xform );
            this._despawned.Add( xform );

            // Notify instance of event OnDespawned for custom code additions.
            //   This is done before handling the deactivate and enqueue incase 
            //   there the user introduces an unforseen issue.
            if ( sendEventMessage )
                xform.gameObject.BroadcastMessage(
                    "OnDespawned",
                    this.spawnPool,
                    SendMessageOptions.DontRequireReceiver
                );

            // Deactivate the instance and all children
            PoolManagerUtils.SetActive( xform.gameObject, false );

            // Trigger culling if the feature is ON and the size  of the 
            //   overall pool is over the Cull Above threashold.
            //   This is triggered here because Despawn has to occur before
            //   it is worth culling anyway, and it is run fairly often.
            if ( !this.cullingActive &&   // Cheap & Singleton. Only trigger once!
                this.cullDespawned &&    // Is the feature even on? Cheap too.
                this.totalCount > this.cullAbove )   // Criteria met?
            {
                this.cullingActive = true;
                this.spawnPool.StartCoroutine( CullDespawned() );
            }
            return true;
        }



        /// <summary>
        /// Waits for 'cullDelay' in seconds and culls the 'despawned' list if 
        /// above 'cullingAbove' amount. 
        /// 
        /// Triggered by DespawnInstance()
        /// </summary>
        internal IEnumerator CullDespawned()
        {
            if ( this.logMessages )
                Debug.Log( string.Format( "SpawnPool {0} ({1}): CULLING TRIGGERED! " +
                                          "Waiting {2}sec to begin checking for despawns...",
                                        this.spawnPool.poolName,
                                        this.prefab.name,
                                        this.cullDelay ) );

            // First time always pause, then check to see if the condition is
            //   still true before attempting to cull.
            yield return new WaitForSeconds( this.cullDelay );

            while ( this.totalCount > this.cullAbove )
            {
                // Attempt to delete an amount == this.cullMaxPerPass
                for ( int i = 0; i < this.cullMaxPerPass; i++ )
                {
                    // Break if this.cullMaxPerPass would go past this.cullAbove
                    if ( this.totalCount <= this.cullAbove )
                        break;  // The while loop will stop as well independently

                    // Destroy the last item in the list
                    if ( this._despawned.Count > 0 )
                    {
                        Transform inst = this._despawned[ 0 ];
                        this._despawned.RemoveAt( 0 );
                        MonoBehaviour.Destroy( inst.gameObject );

                        if ( this.logMessages )
                            Debug.Log( string.Format( "SpawnPool {0} ({1}): " +
                                                    "CULLING to {2} instances. Now at {3}.",
                                                this.spawnPool.poolName,
                                                this.prefab.name,
                                                this.cullAbove,
                                                this.totalCount ) );
                    }
                    else if ( this.logMessages )
                    {
                        Debug.Log( string.Format( "SpawnPool {0} ({1}): " +
                                                    "CULLING waiting for despawn. " +
                                                    "Checking again in {2}sec",
                                                this.spawnPool.poolName,
                                                this.prefab.name,
                                                this.cullDelay ) );

                        break;
                    }
                }

                // Check again later
                yield return new WaitForSeconds( this.cullDelay );
            }

            if ( this.logMessages )
                Debug.Log( string.Format( "SpawnPool {0} ({1}): CULLING FINISHED! Stopping",
                                        this.spawnPool.poolName,
                                        this.prefab.name ) );

            // Reset the singleton so the feature can be used again if needed.
            this.cullingActive = false;
            yield return null;
        }



        /// <summary>
        /// Move an instance from despawned to spawned, set the position and 
        /// rotation, activate it and all children and return the transform.
        /// 
        /// If there isn't an instance available, a new one is made.
        /// </summary>
        /// <returns>
        /// The new instance's Transform. 
        /// 
        /// If the Limit option was used for the PrefabPool associated with the
        /// passed prefab, then this method will return null if the limit is
        /// reached.
        /// </returns>    
        internal Transform SpawnInstance( Vector3 pos, Quaternion rot )
        {
            // Handle FIFO limiting if the limit was used and reached.
            //   If first-in-first-out, despawn item zero and continue on to respawn it
            if ( this.limitInstances && this.limitFIFO &&
                this._spawned.Count >= this.limitAmount )
            {
                Transform firstIn = this._spawned[ 0 ];

                if ( this.logMessages )
                {
                    Debug.Log( string.Format
                    (
                        "SpawnPool {0} ({1}): " +
                            "LIMIT REACHED! FIFO=True. Calling despawning for {2}...",
                        this.spawnPool.poolName,
                        this.prefab.name,
                        firstIn
                    ) );
                }

                this.DespawnInstance( firstIn );

                // Because this is an internal despawn, we need to re-sync the SpawnPool's
                //  internal list to reflect this
                this.spawnPool._spawned.Remove( firstIn );
            }

            Transform inst;

            // If nothing is available, create a new instance
            if ( this._despawned.Count == 0 )
            {
                // This will also handle limiting the number of NEW instances
                inst = this.SpawnNew( pos, rot );
            }
            else
            {
                // Switch the instance we are using to the spawned list
                // Use the first item in the list for ease
                inst = this._despawned[ 0 ];
                this._despawned.RemoveAt( 0 );
                this._spawned.Add( inst );

                // This came up for a user so this was added to throw a user-friendly error
                if ( inst == null )
                {
                    var msg = "Make sure you didn't delete a despawned instance directly.";
                    throw new MissingReferenceException( msg );
                }

                if ( this.logMessages )
                    Debug.Log( string.Format( "SpawnPool {0} ({1}): respawning '{2}'.",
                                            this.spawnPool.poolName,
                                            this.prefab.name,
                                            inst.name ) );

                // Get an instance and set position, rotation and then 
                //   Reactivate the instance and all children
                inst.position = pos;
                inst.rotation = rot;
                PoolManagerUtils.SetActive( inst.gameObject, true );

            }

            //
            // NOTE: OnSpawned message broadcast was moved to main Spawn() to ensure it runs last
            //

            return inst;
        }



        /// <summary>
        /// Spawns a NEW instance of this prefab and adds it to the spawned list.
        /// The new instance is placed at the passed position and rotation
        /// </summary>
        /// <param name="pos">Vector3</param>
        /// <param name="rot">Quaternion</param>
        /// <returns>
        /// The new instance's Transform. 
        /// 
        /// If the Limit option was used for the PrefabPool associated with the
        /// passed prefab, then this method will return null if the limit is
        /// reached.
        /// </returns>
        public Transform SpawnNew() { return this.SpawnNew( Vector3.zero, Quaternion.identity ); }
        public Transform SpawnNew( Vector3 pos, Quaternion rot )
        {
            // Handle limiting if the limit was used and reached.
            if ( this.limitInstances && this.totalCount >= this.limitAmount )
            {
                if ( this.logMessages )
                {
                    Debug.Log( string.Format
                    (
                        "SpawnPool {0} ({1}): " +
                                "LIMIT REACHED! Not creating new instances! (Returning null)",
                            this.spawnPool.poolName,
                            this.prefab.name
                    ) );
                }

                return null;
            }

            // Use the SpawnPool group as the default position and rotation
            if ( pos == Vector3.zero ) pos = this.spawnPool.group.position;
            if ( rot == Quaternion.identity ) rot = this.spawnPool.group.rotation;

            var inst = ( Transform )UnityEngine.Object.Instantiate( this.prefab, pos, rot );
            this.nameInstance( inst );  // Adds the number to the end

            if ( !this.spawnPool.dontReparent )
                inst.SetParent( this.spawnPool.group );  // The group is the parent by default

            if ( this.spawnPool.matchPoolScale )
                inst.localScale = Vector3.one;

            if ( this.spawnPool.matchPoolLayer )
                this.SetRecursively( inst, this.spawnPool.gameObject.layer );

            // Start tracking the new instance
            this._spawned.Add( inst );

            if ( this.logMessages )
                Debug.Log( string.Format( "SpawnPool {0} ({1}): Spawned new instance '{2}'.",
                                        this.spawnPool.poolName,
                                        this.prefab.name,
                                        inst.name ) );

            return inst;
        }


        /// <summary>
        /// Sets the layer of the passed transform and all of its children
        /// </summary>
        /// <param name="xform">The transform to process</param>
        /// <param name="layer">The new layer</param>
        private void SetRecursively( Transform xform, int layer )
        {
            xform.gameObject.layer = layer;
            foreach ( Transform child in xform )
                SetRecursively( child, layer );
        }


        /// <summary>
        /// Used by a SpawnPool to add an existing instance to this PrefabPool.
        /// This is used during game start to pool objects which are not 
        /// instantiated at runtime
        /// </summary>
        /// <param name="inst">The instance to add</param>
        /// <param name="despawn">True to despawn on add</param>
        internal void AddUnpooled( Transform inst, bool despawn )
        {
            this.nameInstance( inst );   // Adds the number to the end

            if ( despawn )
            {
                // Deactivate the instance and all children
                PoolManagerUtils.SetActive( inst.gameObject, false );

                // Start Tracking as despawned
                this._despawned.Add( inst );
            }
            else
                this._spawned.Add( inst );
        }


        /// <summary>
        /// Preload PrefabPool.preloadAmount instances if they don't already exist. In 
        /// otherwords, if there are 7 and 10 should be preloaded, this only creates 3.
        /// This is to allow asynchronous Spawn() usage in Awake() at game start
        /// </summary>
        /// <returns></returns>
        internal void PreloadInstances()
        {
            // If this has already been run for this PrefabPool, there is something
            //   wrong!
            if ( this.preloaded )
            {
                Debug.Log( string.Format( "SpawnPool {0} ({1}): " +
                                          "Already preloaded! You cannot preload twice. " +
                                          "If you are running this through code, make sure " +
                                          "it isn't also defined in the Inspector.",
                                        this.spawnPool.poolName,
                                        this.prefab.name ) );

                return;
            }

            if ( this.prefab == null )
            {
                Debug.LogError( string.Format( "SpawnPool {0} ({1}): Prefab cannot be null.",
                                             this.spawnPool.poolName,
                                             this.prefab.name ) );

                return;
            }

            // Protect against preloading more than the limit amount setting
            //   This prevents an infinite loop on load if FIFO is used.
            if ( this.limitInstances && this.preloadAmount > this.limitAmount )
            {
                Debug.LogWarning
                (
                    string.Format
                    (
                        "SpawnPool {0} ({1}): " +
                            "You turned ON 'Limit Instances' and entered a " +
                            "'Limit Amount' greater than the 'Preload Amount'! " +
                            "Setting preload amount to limit amount.",
                         this.spawnPool.poolName,
                         this.prefab.name
                    )
                );

                this.preloadAmount = this.limitAmount;
            }

            // Notify the user if they made a mistake using Culling
            //   (First check is cheap)
            if ( this.cullDespawned && this.preloadAmount > this.cullAbove )
            {
                Debug.LogWarning( string.Format( "SpawnPool {0} ({1}): " +
                    "You turned ON Culling and entered a 'Cull Above' threshold " +
                    "greater than the 'Preload Amount'! This will cause the " +
                    "culling feature to trigger immediatly, which is wrong " +
                    "conceptually. Only use culling for extreme situations. " +
                    "See the docs.",
                    this.spawnPool.poolName,
                    this.prefab.name
                ) );
            }

            if ( this.preloadTime )
            {
                if ( this.preloadFrames > this.preloadAmount )
                {
                    Debug.LogWarning( string.Format( "SpawnPool {0} ({1}): " +
                        "Preloading over-time is on but the frame duration is greater " +
                        "than the number of instances to preload. The minimum spawned " +
                        "per frame is 1, so the maximum time is the same as the number " +
                        "of instances. Changing the preloadFrames value...",
                        this.spawnPool.poolName,
                        this.prefab.name
                    ) );

                    this.preloadFrames = this.preloadAmount;
                }

                this.spawnPool.StartCoroutine( this.PreloadOverTime() );
            }
            else
            {
                // Reduce debug spam: Turn off this.logMessages then set it back when done.
                this.forceLoggingSilent = true;

                Transform inst;
                while ( this.totalCount < this.preloadAmount ) // Total count will update
                {
                    // Preload...
                    // This will parent, position and orient the instance
                    //   under the SpawnPool.group
                    inst = this.SpawnNew();
                    this.DespawnInstance( inst, false );
                }

                // Restore the previous setting
                this.forceLoggingSilent = false;
            }
        }

        private IEnumerator PreloadOverTime()
        {
            yield return new WaitForSeconds( this.preloadDelay );

            Transform inst;

            // subtract anything spawned by other scripts, just in case
            int amount = this.preloadAmount - this.totalCount;
            if ( amount <= 0 )
                yield break;

            // Doesn't work for Windows8...
            //  This does the division and sets the remainder as an out value.
            //int numPerFrame = System.Math.DivRem(amount, this.preloadFrames, out remainder);
            int remainder = amount % this.preloadFrames;
            int numPerFrame = amount / this.preloadFrames;

            // Reduce debug spam: Turn off this.logMessages then set it back when done.
            this.forceLoggingSilent = true;

            int numThisFrame;
            for ( int i = 0; i < this.preloadFrames; i++ )
            {
                // Add the remainder to the *last* frame
                numThisFrame = numPerFrame;
                if ( i == this.preloadFrames - 1 )
                {
                    numThisFrame += remainder;
                }

                for ( int n = 0; n < numThisFrame; n++ )
                {
                    // Preload...
                    // This will parent, position and orient the instance
                    //   under the SpawnPool.group
                    inst = this.SpawnNew();
                    if ( inst != null )
                        this.DespawnInstance( inst, false );

                    yield return null;
                }

                // Safety check in case something else is making instances. 
                //   Quit early if done early
                if ( this.totalCount > this.preloadAmount )
                    break;
            }

            // Restore the previous setting
            this.forceLoggingSilent = false;
        }

        #endregion Pool Functionality


        #region Utilities
        /// <summary>
        /// If this PrefabPool spawned or despawned lists contain the given 
        /// transform, true is returned. Othrewise, false is returned
        /// </summary>
        /// <param name="transform">A transform to test.</param>
        /// <returns>bool</returns>
        public bool Contains( Transform transform )
        {
            if ( this.prefabGO == null )
                Debug.LogError( string.Format( "SpawnPool {0}: PrefabPool.prefabGO is null",
                                             this.spawnPool.poolName ) );

            bool contains;

            contains = this.spawned.Contains( transform );
            if ( contains )
                return true;

            contains = this.despawned.Contains( transform );
            if ( contains )
                return true;

            return false;
        }

        /// <summary>
        /// Appends a number to the end of the passed transform. The number
        /// will be one more than the total objects in this PrefabPool, so 
        /// name the object BEFORE adding it to the spawn or depsawn lists.
        /// </summary>
        /// <param name="instance"></param>
        private void nameInstance( Transform instance )
        {
            // Rename by appending a number to make debugging easier
            //   ToString() used to pad the number to 3 digits. Hopefully
            //   no one has 1,000+ objects.
            instance.name += ( this.totalCount + 1 ).ToString( "#000" );
        }
        #endregion Utilities

    }

}

