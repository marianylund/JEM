using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIBees : MonoBehaviour
{
    void Start()
    {
        Transform[] bees = GetComponentsInChildren<Transform>().Where(go => go.gameObject != this.gameObject).ToArray(); ;
        int numberOfBeesToDisable = bees.Length - Settings.s.NUMBER_OF_BEES;
        Debug.Assert(numberOfBeesToDisable >= 0, "There are less bees in the menu than needed. We have: " + bees.Length + " number of bees: " + Settings.s.NUMBER_OF_BEES);

        foreach (Transform bee in bees)
        {
            if(numberOfBeesToDisable > 0)
            {
                bee.gameObject.SetActive(false);
                numberOfBeesToDisable -= 1;
            }
            //else
            //{
            //    float scale = Random.Range(35, 50);
            //    bee.localScale = new Vector3(scale, scale, scale);
            //}
        }
    }
}
