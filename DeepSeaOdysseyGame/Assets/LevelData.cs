using UnityEngine;

[CreateAssetMenu(fileName = "Level_01", menuName = "DeepSea/Level Data")]
public class LevelData : ScriptableObject
{
    public int levelNumber = 1;
    public string levelName = "Shallow Reef";
    public int patrollerCount = 1;
    public int shooterCount = 1;
    public int chargerCount = 0;
    public float levelTimeLimit = 60f;
    public int targetScore = 100;
}