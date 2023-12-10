using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/MapList")]
public class MapList : ScriptableObject
{
    [Serializable]
    public struct Map {
        public string name;
        public string sceneName;
    }

    public Map[] maps;

    public string GetSceneName(string mapName)
    {
        return maps.First(x => x.name == mapName).sceneName;
    }
}
