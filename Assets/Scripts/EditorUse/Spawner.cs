using UnityEngine;

public class Spawner : MonoBehaviour
{
    // 要实例化的游戏对象。
    public GameObject entityToSpawn;

    //上面定义的 ScriptableObject 的一个实例。
    public SpawnManagerScriptableObject spawnManagerValues;

    //这将附加到创建的实体的名称，并在创建每个实体时递增。
    int instanceNumber = 1;

    void Start()
    {
        SpawnEntities();
    }

    void SpawnEntities()
    {
        int currentSpawnPointIndex = 0;

        for ( int i = 0; i < spawnManagerValues.numberOfPrefabsToCreate; i++ )
        {
            //在当前生成点处创建预制件的实例。
            GameObject currentEntity = Instantiate( entityToSpawn, spawnManagerValues.spawnPoints[ currentSpawnPointIndex ], Quaternion.identity );

            //将实例化实体的名称设置为 ScriptableObject 中定义的字符串，然后为其附加一个唯一编号。
            currentEntity.name = spawnManagerValues.prefabName + instanceNumber;

            // 移动到下一个生成点索引。如果超出范围，则回到起始点。
            currentSpawnPointIndex = ( currentSpawnPointIndex + 1 ) % spawnManagerValues.spawnPoints.Length;

            instanceNumber++;
        }
    }
}