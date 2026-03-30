using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 关卡模板 - 预设固定布局，适合新手教程关卡
/// </summary>
[System.Serializable]
public class LevelTemplate
{
    public int levelIndex;
    public string levelName;
    public string description;
    public List<TemplateEnemy> enemies;
    public List<TemplateBuilding> buildings;
    public int difficulty;
}

[System.Serializable]
public class TemplateEnemy
{
    public int x;
    public int y;
    public string enemyType;
    public int extraHealth;
}

[System.Serializable]
public class TemplateBuilding
{
    public int x;
    public int y;
}

/// <summary>
/// 关卡模板集合 - 可以创建多个教程关卡
/// </summary>
[CreateAssetMenu(fileName = "LevelTemplates", menuName = "IntoTheBreach/Level Templates")]
public class LevelTemplateCollection : ScriptableObject
{
    public List<LevelTemplate> templates = new List<LevelTemplate>();
    
    // 预设教程关卡
    public static LevelTemplate GetTutorialLevel1()
    {
        LevelTemplate template = new LevelTemplate
        {
            levelIndex = 0,
            levelName = "初次接触",
            description = "击退两个敌人，保护中间的建筑",
            difficulty = 1,
            enemies = new List<TemplateEnemy>
            {
                new TemplateEnemy { x = 2, y = 6, enemyType = "Beetle" },
                new TemplateEnemy { x = 5, y = 5, enemyType = "Spitter" }
            },
            buildings = new List<TemplateBuilding>
            {
                new TemplateBuilding { x = 4, y = 3 }
            }
        };
        return template;
    }
}
