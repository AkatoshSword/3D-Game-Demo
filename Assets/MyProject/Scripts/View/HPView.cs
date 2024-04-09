using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPView : MonoBehaviour
{
    public GameObject hpItemPrefab;
    public DamageAble damageAble;

    private Toggle[] hps;

    private void Start()
    {
        hps = new Toggle[damageAble.maxHp];
        for (int i = 0; i < damageAble.maxHp; i++)
        {
            GameObject hpItem = GameObject.Instantiate(hpItemPrefab, transform.Find("HPs"));
            hps[i] = hpItem.GetComponent<Toggle>();
        }
    }

    public void UpdateHPView()
    {
        for (int i = 0; i < hps.Length; i++)
        {
            hps[i].isOn = i < damageAble.CurrentHp;
        }
    }
}
