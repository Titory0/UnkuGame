using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class EnemyStatePoolEntry
{
    public EnemyState state;
    [Min(0f)] public float weight = 1f;
}

[System.Serializable]
public class EnemyStatePool
{
    public List<EnemyStatePoolEntry> entries = new List<EnemyStatePoolEntry>();

    public EnemyState GetAnyState()
    {
        if (entries == null || entries.Count == 0) { return null; }

        float totalWeight = 0f;
        foreach (var entry in entries)
        {
            if (entry.state != null && entry.weight > 0f)
            {
                totalWeight += entry.weight;
            }
        }

        if (totalWeight <= 0f) { return null; }

        float randomValue = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var entry in entries)
        {
            if (entry.state == null || entry.weight <= 0f) { continue; }

            cumulative += entry.weight;
            if (randomValue <= cumulative)
            {
                return entry.state;
            }
        }

        return null;
    }

    public static implicit operator EnemyState(EnemyStatePool pool)
    {
        return pool == null ? null : pool.GetAnyState();
    }
}

#if UNITY_EDITOR

// Affiche la liste "entries" directement sous le champ EnemyStatePool, sans niveau intermédiaire.
[CustomPropertyDrawer(typeof(EnemyStatePool))]
public class EnemyStatePoolDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var entries = property.FindPropertyRelative("entries");
        return EditorGUI.GetPropertyHeight(entries, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var entries = property.FindPropertyRelative("entries");
        EditorGUI.PropertyField(position, entries, label, true);
    }
}
#endif