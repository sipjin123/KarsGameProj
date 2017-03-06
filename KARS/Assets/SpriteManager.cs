using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour {

    private static SpriteManager _instance;
    public static SpriteManager Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
    }

    public GameObject[] SkillIcons;
}
