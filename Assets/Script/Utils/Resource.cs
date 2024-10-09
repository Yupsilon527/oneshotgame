using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

public class Resource
{
    static bool resourceDebug = false;
    bool canNegative;
    string name;
    float[] values;
    bool hasHardLimit = false;
    public UnityEvent OnValueChanged;
    public override string ToString()
    {
        return $"{name} ({values[0]},{values[1]})";
    }
    public Resource(float limit,string name,bool negative, bool limited)
    {
        this.name = name;
        hasHardLimit = limited;
        values = new float[] { limit, limit, limit };
        canNegative = negative;
        OnValueChanged = new UnityEvent();
        if (resourceDebug) Debug.Log($"[{name}] Initialized");
    }
    public float GetValue()
    {
        return values[0];
    }
    public float GetValueRounded(int d = 1)
    {
        d = (int)Mathf.Max(1,Mathf.Pow(10, d) );
        return Mathf.Round( values[0]*d)/d;
    }
    public float GetPercentage()
    {
        float value = values[0] / values[1];
        if (float.IsNaN(value))
            return 1;
        return value;
    }

    public float GetLimit(bool baseLimit = false)
    {
        return  values[baseLimit ? 2 : 1];
    }
    public void ResetLimit(LimitRule rule)
    {
        SetLimit(values[2], rule);
    }
    public float GetDifference()
    {
        return values[1] - values[0];
    }

    public void GiveValue(float value)
    {
        if (resourceDebug) Debug.Log($"[{name}]  Give " + value);
        if (value != 0)
        {
            SetValue(values[0] + value);
        }
    }

    public void SetValue(float value)
    {
        float oldlife = values[0];
        if (hasHardLimit)
            values[0] = Mathf.Min(value, values[1]);
        else 
            values[0] = value;

        if (!canNegative)
        {
            values[0] = Mathf.Max(0, values[0]);
        }
        if (resourceDebug) Debug.Log($"[{name}] Change " + oldlife + " to " + values[0]);
        OnValueChanged.Invoke();
        
    }

    public void SetPercentage(float value)
    {
        SetValue(value * GetLimit(false));
    }

    public enum LimitRule
    {
        leave_value,
        give_difference,
        percent_value,
        substract_total,
        fullheal_value,
        empty_value
    }
    public LimitRule LimitUnder = LimitRule.leave_value;
    public LimitRule LimitOver = LimitRule.give_difference;
    public void SetLimit(float value)
    {
        SetLimit(value, LimitUnder, LimitOver);
    }
    public void SetLimit(float value, LimitRule under, LimitRule over)
    {
        SetLimit(value, value < GetLimit(false) ? under : over, false);
    }

    public void SetLimit(float value, LimitRule rule = LimitRule.leave_value,bool hard = false)
    {
       // display?.SetMaximum(values[1]);
       switch (rule)
        {
            case LimitRule.leave_value:
                values[1] = value;
                SetValue(values[0]);
                break;
            case LimitRule.give_difference:
                float difference = value - values[1];
                values[1] = value;
                SetValue(values[0] + difference);
                break;
            case LimitRule.percent_value:
                float percent = GetPercentage();
                values[1] = value;
                SetPercentage(percent);
                break;
            case LimitRule.fullheal_value:
                values[1] = value;
                SetPercentage(1);
                break;
            case LimitRule.empty_value:
                values[1] = value;
                SetPercentage(0);
                break;
            case LimitRule.substract_total:
                values[0] -= values[1];
                values[1] = value;
                SetPercentage(0);
                break;
        }
        if (hard)
        {
            values[2] = value;
        }
        if (resourceDebug) Debug.Log($"[{name}]  Set Max to " + value );
    }

    public bool ChargeValue(float value)
    {
        if (resourceDebug) Debug.Log($"[{name}]  Charge " + value);
        if (value == 0)
        {
            return true;
        }
        if (GetValue() < value)
        {
            return false;
        }
        GiveValue(-value);
        return true;
    }
    public bool ChargePercentage(float value,bool total)
    {
     return ChargeValue((total ? GetLimit(false) : GetValue() )* value);
    }

    public float SubstractedValue(float value)
    {
        if (resourceDebug) Debug.Log("[" + name + "] Substract " + value);
        value = Mathf.Abs(value);
        if (!canNegative)  value = Mathf.Min(GetValue(), value);
        GiveValue(-value);
        return value;
    }
    public float RemainingValue(float value)
    {
        if (resourceDebug) Debug.Log("[" + name + "] Substract " + value);
        GiveValue(-value);
        return GetValue();
    }
}