using UnityEngine;
using System.Collections;
using System;

public class GameMgr : MonoBehaviour
{
    public static GameMgr instance;

    public UInt32 id = 0;
    public UInt32 roomId = 0;

	// Use this for initialization
	void Awake()
	{
        instance = this;
	}

	// Update is called once per frame
	void Update()
	{
			
	}
}
