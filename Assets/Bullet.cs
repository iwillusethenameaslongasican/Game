using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Bullet : MonoBehaviour {
    public float speed = 100f;
    // 爆炸效果
    public GameObject explode;
    // 最大存活时间
    public float maxLiftTime = 2f;
    // 初始化时间
    public float instantiateTime = 0f;

    // 攻击方
    public GameObject attckTank;

	// Use this for initialization
	void Start () {
        instantiateTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        // 前进
        transform.position += transform.forward * speed * Time.deltaTime;
        // 超过存活时间 摧毁
        if(Time.time - instantiateTime > maxLiftTime) {
            Destroy(gameObject);
        }
	}

    // 碰撞
    private void OnCollisionEnter(Collision collision)
    {
        // 打到自身
        if(collision.gameObject == attckTank) {
            return;
        }
        // 爆炸效果
        Instantiate(explode, transform.position, transform.rotation);
        // 销毁自身
        Destroy(gameObject);
        // 取得被击中的坦克对象
        Tank tank = collision.gameObject.GetComponent<Tank>();
        if(tank != null && attckTank.name == Convert.ToString(GameMgr.instance.id)) {
            float att = GetAtt();
            tank.SendHit(UInt32.Parse(tank.name), att);
        }
    }

    // 计算攻击力
    private float GetAtt() {
        float att = 100 - (Time.time - instantiateTime) * 40;
        if(att < 1) {
            att = 1;
        }
        return att;
    }
}
