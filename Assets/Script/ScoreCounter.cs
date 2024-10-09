using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour
{
    public static ScoreCounter main;
    int bullets = 0;
    public int nBullets
    {
        get
        {
            return bullets;
        }
        set
        {
            bullets = value;
        //    bulletsText.text = "Bullets: " + bullets;
        }
    }
    int enemies = 0;
    public int nBadies
    {
        get
        {
            return enemies;
        }
        set
        {
            enemies = value;
        //    baddiesText.text = "Baddies: " + enemies;
        }
    }
    Text bulletsText;
    Text baddiesText;
    private void Awake()
    {
        main = this;
       // bulletsText = transform.Find("Bullets").GetComponent<Text>();
        //baddiesText = transform.Find("Enemies").GetComponent<Text>();
    }
}
