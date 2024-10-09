using UnityEngine;

public static class WeightList 
{
    [System.Serializable]
    public class WeightEntry
    {
        public float Weight = 1;

        public WeightEntry(float weight)
        {
            Weight = Mathf.Max(1,weight);
        }
    }
    [System.Serializable]
    public class WeightPrefab : WeightEntry
    {
        public GameObject prefab;
        public WeightPrefab(GameObject enemyPrefab, float weight) : base(weight)
        {
            prefab = enemyPrefab;
        }
    }
    public class WeightItem<itemt> : WeightEntry
    {
        public itemt value;
        public WeightItem(itemt value, float weight) : base(weight)
        {
            this.value = value;
        }
    }
    public static witem PickWeight<witem>(witem[] list) where witem : WeightEntry
    {
        if (list != null)
        {
            float total = Random.Range(0, GetTotal(list));

            foreach (witem item in list)
            {
                total -= Mathf.Max(.001f, item.Weight);
                if (total <= 0)
                    return item;
            }
        }
        return null;
    }
    public static witem PickWeightCombined<witem>(witem[] listA, witem[] listB) where witem : WeightEntry
    {
        if (listA != null && listB!=null)
        {
            float totalA = GetTotal(listA);
            float totalB = GetTotal(listB);

            float total = Random.Range(0, totalA+totalB);

            foreach (witem item in listA)
            {
                total -= Mathf.Max(.001f, item.Weight);
                if (total <= 0)
                    return item;
            }
            foreach (witem item in listB)
            {
                total -= Mathf.Max(.001f, item.Weight);
                if (total <= 0)
                    return item;
            }
        }
        return null;
    }
    public static float GetTotal(WeightEntry[] value) 
    {
        float total = 0;
        foreach (WeightEntry item in value)
        {
            total += Mathf.Max(.001f, item.Weight);
        }
        return total;
    }
}
