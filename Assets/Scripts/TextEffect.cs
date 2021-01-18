using UnityEngine;
using TMPro;
[RequireComponent(typeof(TextMeshProUGUI))]
public class TextEffect : MonoBehaviour
{
    //private TextMeshProUGUI ugui;
    //private TextEffectPool pool;
    //private float timer_ = 0f;

    //private void Awake()
    //{
    //    ugui = GetComponent<TextMeshProUGUI>();
    //    pool = GetComponentInParent<TextEffectPool>();
    //}

    //private void Update()
    //{
    //    float step = Settings.s.textEffectFliesUpSpeed * Time.deltaTime; // calculate distance to move
    //    transform.position = Vector3.MoveTowards(transform.position, Vector3.up*100f, step).With(z:0);
    //    timer_ += Time.deltaTime;
    //    if(timer_>= Settings.s.textEffectTimeOnScreen)
    //    {
    //        timer_ = 0;
    //        pool.ReturnToPool(this);
    //    }
    //}

    //public void SetText(string text)
    //{
    //    ugui.text = text;
    //}

    //public void SetColor(Color color)
    //{
    //    ugui.color = color;
    //}

}
