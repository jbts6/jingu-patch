using System.Collections.Generic;
using DBLoad;

public enum BehaviorType { }

public class SaveData
{
    public void CostAction(BehaviorType type) { }
    public void AddItems(List<ItemInfo> items, bool showTips) { }
    public void AddEffectValue(EffectId id, float value, bool needTips) { }
}

public class GlobalData
{
    public void AddFishHash(int id) { }
}

public class SaveManager : MonoSingleton<SaveManager>
{
    public SaveData SaveData
    {
        get { return this.m_saveData; }
        set { this.m_saveData = value; }
    }

    public SaveData m_saveData;
    public GlobalData GlobalData;
}
