using UnityEngine;
using System.Collections.Generic;

public class AIAttacker : MonoBehaviour
{
    [Header("Доступні типи ворогів")]
    public EnemyData goblinData;
    public EnemyData orcData;
    public EnemyData ghostData;

    /// <summary>
    /// </summary>
    public List<EnemyData> GenerateWave(int budget, int round)
    {
        List<EnemyData> wave = new List<EnemyData>();
        int remaining = budget;
        int maxEnemies = GameManager.Instance.config.maxEnemiesPerWave;

        float orcChance = Mathf.Clamp01(0.1f + round * 0.05f);
        float ghostChance = Mathf.Clamp01(0.05f + round * 0.04f);

        while (remaining > 0 && wave.Count < maxEnemies)
        {
            EnemyData chosen = ChooseEnemy(remaining, orcChance, ghostChance);
            if (chosen == null) break;

            wave.Add(chosen);
            remaining -= chosen.cost;
        }

        if (round >= 5)
        {
            wave.Sort((a, b) => b.cost.CompareTo(a.cost));
            for (int i = Mathf.Min(3, wave.Count); i < wave.Count; i++)
            {
                int j = Random.Range(i, wave.Count);
                var temp = wave[i];
                wave[i] = wave[j];
                wave[j] = temp;
            }
        }
        else
        {
            for (int i = 0; i < wave.Count; i++)
            {
                int j = Random.Range(i, wave.Count);
                var temp = wave[i];
                wave[i] = wave[j];
                wave[j] = temp;
            }
        }

        return wave;
    }

    EnemyData ChooseEnemy(int remaining, float orcChance, float ghostChance)
    {
        List<(EnemyData data, float weight)> options = new List<(EnemyData, float)>();

        if (goblinData != null && goblinData.cost <= remaining)
            options.Add((goblinData, 1f - orcChance - ghostChance));

        if (orcData != null && orcData.cost <= remaining)
            options.Add((orcData, orcChance));

        if (ghostData != null && ghostData.cost <= remaining)
            options.Add((ghostData, ghostChance));

        if (options.Count == 0) return null;

        float totalWeight = 0f;
        foreach (var opt in options) totalWeight += opt.weight;

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;
        foreach (var opt in options)
        {
            cumulative += opt.weight;
            if (roll <= cumulative) return opt.data;
        }

        return options[0].data;
    }
}
