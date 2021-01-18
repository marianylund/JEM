
using UnityEngine;

public class TextEffectPool : GenericObjectPool<TextEffect>
{
    //protected override void ChildAwake()
    //{
    //    SelfDrivingBeeBehaviour.onGettingEnergy += ShowAddedEnergy;
    //}

    //private void OnDestroy()
    //{
    //    SelfDrivingBeeBehaviour.onGettingEnergy -= ShowAddedEnergy;
    //}

    //private void ShowAddedEnergy(int amount, Vector3 position)
    //{
    //    TextEffect textObj = Get();
    //    if (textObj != null)
    //    {
    //        textObj.gameObject.SetActive(true);
    //        textObj.SetText("+" + amount.ToString());
    //        textObj.SetColor(Settings.s.energyTextColor);
    //        textObj.transform.position = position;
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Wanted to show added energy, but now enough Text obj in the pool");
    //    }
    //}
}
