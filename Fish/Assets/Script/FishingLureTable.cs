using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FishingLureTable", menuName = "Fishing/Lure Table")]
public class FishingLureTable : ScriptableObject
{
    public string lureID = "LureA";

    [Tooltip("このルアーで釣れる魚のPrefabリスト")]
    public List<GameObject> fishPrefabs = new List<GameObject>();

    /// <summary>
    /// ランダムで1匹選択して生成する
    /// </summary>
    public GameObject SpawnRandomFish(Vector3 position, Quaternion rotation)
    {
        if (fishPrefabs == null || fishPrefabs.Count == 0) return null;

        int randomIndex = Random.Range(0, fishPrefabs.Count);
        GameObject selectedPrefab = fishPrefabs[randomIndex];

        return Instantiate(selectedPrefab, position, rotation);
    }
}