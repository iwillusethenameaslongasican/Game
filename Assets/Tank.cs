using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using code.unity.TankGame.Assets.proto.protoId;
using code.unity.TankGame.Assets.proto.fight;
using Net;

public class Tank : MonoBehaviour
{
	//炮塔炮管轮子履带
	public Transform turret;
	public Transform gun;
	private Transform wheels;
	private Transform tracks;
	//炮塔旋转速度 
	private float turretRotSpeed = 0.5f;
	//炮塔炮管目标角度
	private float turretRotTarget = 0;
	private float turretRollTarget = 0;
	//炮管的旋转范围
	private float maxRoll = 10f;
	private float minRoll = -4f;


	//轮轴
	public List<AxleInfo> axleInfos;
	//马力/最大马力
	private float motor = 0;
	public float maxMotorTorque;
	//制动/最大制动
	private float brakeTorque = 0;
	public float maxBrakeTorque = 100;
	//转向角/最大转向角
	private float steering = 0;
	public float maxSteeringAngle;


	//炮弹预设
	public GameObject bullet;
	//上一次开炮的时间
	public float lastShootTime = 0;
	//开炮的时间间隔
	private float shootInterval = 0.5f;

	// 坦克的最大生命值
	private float maxHp = 100;
	// 当前生命值
	public float hp = 100;

	// 焚烧特效
	public GameObject destoryEffect;

	// 中心准心
	public Texture2D centerSight;
	// 坦克准心
	public Texture2D tankSight;

    // 生命条素材
    public Texture2D hpBarBg;
    public Texture2D hpBar;


    // 击杀提示图标
    public Texture2D killUI;
    // 击杀图标开始显示的时间
    public float killUIStartTime = float.MinValue;

    // 人工智能
    private AI ai;

	//  操控类型
	public enum CtrlType
	{
		none,
		player,
		computer,
        net,
	}
	// 默认操控类型为玩家
	public CtrlType ctrlType = CtrlType.player;

    //网络同步
    private float lastSendInfoTime = float.MinValue;

    //用于预测位置的变量
    //
    //上次更新位置
    Vector3 lPos;
    Vector3 lRot;
    //forecast 预测的位置信息
    Vector3 fPos;
    Vector3 fRot;
    //时间间隔
    float delta = 1;
    //上次接收的时间
    float lastRecvInfoTime = 0;


	//玩家控制
	public void PlayerCtrl()
	{
		// 只有玩家操控的坦克才会生效
		if (ctrlType != CtrlType.player)
		{
			return;
		}
        //Debug.Log("PlayerCtrl!!!");
		//马力和转向角
		motor = maxMotorTorque * Input.GetAxis("Vertical");
		steering = maxSteeringAngle * Input.GetAxis("Horizontal");
		//制动
		brakeTorque = 0;
		foreach (AxleInfo axleInfo in axleInfos)
		{
			if (axleInfo.leftWheel.rpm > 5 && motor < 0)  //前进时，按下“下”键
				brakeTorque = maxBrakeTorque;
			else if (axleInfo.leftWheel.rpm < -5 && motor > 0)  //后退时，按下“上”键
				brakeTorque = maxBrakeTorque;
			continue;
		}

		// 炮塔炮管角度
		TargetSignPos();

		//发射炮弹
		if (Input.GetMouseButton(0))
			Shoot();
        //网络同步
        if (Time.time - lastSendInfoTime > 0.2f){
            SendUnitInfo();
            lastSendInfoTime = Time.time;
        }
	}

    // 电脑控制
    public void ComputerCtrl() {
        if(ctrlType != CtrlType.computer) {
            return;
        }

        // 炮塔目标角度
        Vector3 rot = ai.GetTurretTarget();
        turretRotTarget = rot.y;
        turretRollTarget = rot.x;

        // 发射炮弹
        if(ai.IsShoot()) {
            Shoot();
        }

        // 移动
        steering = ai.GetSteering();
        motor = ai.GetMotor();
        brakeTorque = ai.GetBrakeTorque();
    }

    // 无人控制
    public void NoneCtrl() {
        if(ctrlType != CtrlType.none) {
            return;
        }

        motor = 0;
        steering = 0;
        brakeTorque = maxBrakeTorque / 2;
    }


	//开始时执行
	void Start()
	{
		//获取炮塔
		turret = transform.Find("turret");
		//获取炮管
		gun = turret.Find("gun");
		//获取轮子
		wheels = transform.Find("wheels");
		//获取履带
		tracks = transform.Find("tracks");

        // 人工智能
        if(ctrlType == CtrlType.computer) {
            ai = gameObject.AddComponent<AI>();
            ai.tank = this;
        }

	}

    // 绘图
    void OnGUI()
    {
        // 只有玩家控制的坦克会显示准心
        if(ctrlType != CtrlType.player) {
            return;
        }
        // 绘制准心
        DrawSight();
        // 绘制生命条
        DrawHp();
        // 绘制击杀提示
        DrawKillUI();
    }

    //每帧执行一次
    void Update()
	{
        //网络同步
        if(ctrlType == CtrlType.net){
            NetUpdate();
            return;
        }
		//玩家控制操控
		PlayerCtrl();
        // 电脑控制
        ComputerCtrl();
        // 无人控制
        NoneCtrl();
		// 计算爆炸位置
		CalExplodePoint();
		//遍历车轴
		foreach (AxleInfo axleInfo in axleInfos)
		{
			//转向
			if (axleInfo.steering)
			{
				axleInfo.leftWheel.steerAngle = steering;
				axleInfo.rightWheel.steerAngle = steering;
			}
			//马力
			if (axleInfo.motor)
			{
				axleInfo.leftWheel.motorTorque = motor;
				axleInfo.rightWheel.motorTorque = motor;
			}
			//制动
			if (true)
			{
				axleInfo.leftWheel.brakeTorque = brakeTorque;
				axleInfo.rightWheel.brakeTorque = brakeTorque;
			}
			//转动轮子履带
			if (axleInfos[1] != null && axleInfo == axleInfos[1])
			{
				WheelsRotation(axleInfos[1].leftWheel);
				TrackMove();
			}
		}

		//炮塔炮管旋转
		TurretRotation();
		TurretRoll();

	}

	//炮塔旋转
	public void TurretRotation()
	{
		if (Camera.main == null)
			return;
		if (turret == null)
			return;

		//归一化角度
		float angle = turret.eulerAngles.y - turretRotTarget;
		if (angle < 0) angle += 360;

		if (angle > turretRotSpeed && angle < 180)
			turret.Rotate(0f, -turretRotSpeed, 0f);
		else if (angle > 180 && angle < 360 - turretRotSpeed)
			turret.Rotate(0f, turretRotSpeed, 0f);
	}

	//炮管旋转
	public void TurretRoll()
	{
		if (Camera.main == null)
			return;
		if (turret == null)
			return;
		//获取角度
		Vector3 worldEuler = gun.eulerAngles;
		Vector3 localEuler = gun.localEulerAngles;
		//世界坐标系角度计算
		worldEuler.x = turretRollTarget;
		gun.eulerAngles = worldEuler;
		//本地坐标系角度限制
		Vector3 euler = gun.localEulerAngles;
		if (euler.x > 180)
			euler.x -= 360;

		if (euler.x > maxRoll)
			euler.x = maxRoll;
		if (euler.x < minRoll)
			euler.x = minRoll;
		gun.localEulerAngles = new Vector3(euler.x, localEuler.y, localEuler.z);
	}

	//轮子旋转
	public void WheelsRotation(WheelCollider collider)
	{
		if (wheels == null)
			return;
		//获取旋转信息
		Vector3 position;
		Quaternion rotation;
		collider.GetWorldPose(out position, out rotation);
		//旋转每个轮子
		foreach (Transform wheel in wheels)
		{
			wheel.rotation = rotation;
		}
	}


	//履带滚动
	public void TrackMove()
	{
		if (tracks == null)
			return;

		float offset = 0;
		if (wheels.GetChild(0) != null)
			offset = wheels.GetChild(0).localEulerAngles.x / 90f;

		foreach (Transform track in tracks)
		{
			MeshRenderer mr = track.gameObject.GetComponent<MeshRenderer>();
			if (mr == null) continue;
			Material mtl = mr.material;
			mtl.mainTextureOffset = new Vector2(0, offset);
		}
	}


	public void Shoot()
	{
		//发射间隔
		if (Time.time - lastShootTime < shootInterval)
			return;
		//子弹
		if (bullet == null)
			return;
		//发射
		Vector3 pos = gun.position + gun.forward * 5;
        GameObject bulletObj = (GameObject)Instantiate(bullet, pos, gun.rotation);
        Bullet bulletCmp = bulletObj.GetComponent<Bullet>();
        // 使炮弹附带上发射方的信息
        if(bulletCmp != null) {
            bulletCmp.attckTank = this.gameObject;
        }
		lastShootTime = Time.time;

        //发送炮弹同步信息
        if(ctrlType == CtrlType.player){
            SendShootInfo(bulletObj.transform);
        }
	}


	// 计算目标实际角度 （炮台炮管的目标角度）
	public void TargetSignPos()
	{
		// 碰撞信息和碰撞点
		Vector3 hitPos = Vector3.zero;
		RaycastHit raycastHit;
		// 屏幕中心位置
		Vector3 centerVec = new Vector3(Screen.width / 2, Screen.height / 2, 0);
		Ray ray = Camera.main.ScreenPointToRay(centerVec);
		// 射线检测
		if (Physics.Raycast(ray, out raycastHit, 400.0f))
		{
			hitPos = raycastHit.point;
		}
		else
		{
			hitPos = ray.GetPoint(400.0f);
		}
		// 计算目标角度
		Vector3 dir = hitPos - turret.position;
		Quaternion angle = Quaternion.LookRotation(dir);
		turretRotTarget = angle.eulerAngles.y;
		turretRollTarget = angle.eulerAngles.x;

	}

	// 计算爆炸位置
	public Vector3 CalExplodePoint()
	{
		// 碰撞信息和碰撞点
		Vector3 hitPos = Vector3.zero;
		RaycastHit raycastHit;
		// 沿着炮管方向的射线
		Vector3 pos = gun.position + gun.forward * 5;
		Ray ray = new Ray(pos, gun.forward); //（起始点，方向）
											 // 射线检测
		if (Physics.Raycast(ray, out raycastHit, 400.0f))
		{
			hitPos = raycastHit.point;
		}
		else
		{
			hitPos = ray.GetPoint(400.0f);
		}

		//// 结束调试
		return hitPos;
	}

    // 绘制准心
    public void DrawSight() {
        // 计算实际射击位置
        Vector3 explodePoint = CalExplodePoint();
        //  获取“坦克准心”的屏幕坐标
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(explodePoint);

        // 绘制坦克准心
        Rect tankRect = new Rect(screenPoint.x - tankSight.width / 2, Screen.height - screenPoint.y - tankSight.height / 2,
                                 tankSight.width, tankSight.height);
        GUI.DrawTexture(tankRect, tankSight);

        // 绘制中心准心
        Rect centerRect = new Rect(Screen.width / 2 - centerSight.width / 2, Screen.height / 2 - centerSight.height / 2,
                                   centerSight.width, centerSight.height);
        GUI.DrawTexture(centerRect, centerSight);
    }

    // 绘制生命条
    public void DrawHp() {
        // 底框
        Rect bgRect = new Rect(30, Screen.height - hpBarBg.height - 15,
                               hpBarBg.width, hpBarBg.height);
        GUI.DrawTexture(bgRect, hpBarBg);

        // 指示条
        float width = hp * 102 / maxHp;
        Rect hpRect = new Rect(bgRect.x + 29, bgRect.y + 9, width, hpBar.height);
        GUI.DrawTexture(hpRect, hpBar);

        // 文字
        string text = Mathf.Ceil(hp).ToString() + "/" + Mathf.Ceil(maxHp).ToString();
        Rect textRect = new Rect(bgRect.x + 80, bgRect.y - 10, 50, 50);
        GUI.Label(textRect, text);
    }

    // 显示击杀图标
    public void StartDrawKill() {
        killUIStartTime = Time.time;
    }

    // 绘制击杀图标
    private void DrawKillUI() {
        if (Time.time - killUIStartTime < 1f) {
            Rect rect = new Rect(Screen.width / 2 - killUI.width / 2, 30,
                                 killUI.width, killUI.height);
            GUI.DrawTexture(rect, killUI);
        }
    }

    //发送同步信息
    public void SendUnitInfo(){
        c2s_update_unit_request request = new c2s_update_unit_request();
        //位置旋转
        Vector3 pos = transform.position;
        Vector3 rot = transform.eulerAngles;
        request.pos_x = (Int32)(pos.x * 1000);
        request.pos_y = (Int32)(pos.y * 1000);
        request.pos_z = (Int32)(pos.z * 1000);
        request.rot_x = (Int32)(rot.x * 1000);
        request.rot_y = (Int32)(rot.y * 1000);
        request.rot_z = (Int32)(rot.z * 1000);
        //炮塔
        Int32 angleY = (Int32)(turretRotTarget * 1000);
        request.gun_rot = angleY;
        //炮管
        Int32 angleX = (Int32)(turretRollTarget);
        request.gun_roll = angleX;
        byte[] data = NetMgr.servConn.CreateData((int)ProtoId.c2s_update_unit_request, request);
        NetMgr.servConn.SendMessage(data);
    }

    //初始化位置预测数据
    public void InitNetCtrl(){
        lPos = transform.position;
        lRot = transform.eulerAngles;
        fPos = transform.position;
        fRot = transform.eulerAngles;
        //冻结物理系统，使物理系统不对坦克产生影响
        Rigidbody r = GetComponent<Rigidbody>();
        r.constraints = RigidbodyConstraints.FreezeAll;
    }

    //位置预测
    public void NetForecastInfo(Vector3 nPos, Vector3 nRot) //当前的同步位置
    {
        //预测的位置
        fPos = lPos + (nPos - lPos) * 2;
        fRot = lRot + (nRot - lRot) * 2;
        //特殊情况的处理，若出现异常的网络延迟，则不能使用上述公式计算
        if (Time.time - lastRecvInfoTime > 0.3f){
            fPos = nPos;
            fRot = nRot;
        }
        //Debug.Log("NetForecastInfo: " + fPos.x);
        //时间
        delta = Time.time - lastRecvInfoTime;
        //更新
        lPos = nPos;
        lRot = nRot;
        lastRecvInfoTime = Time.time;
    }

    //网络更新
    public void NetUpdate(){
        //当前位置
        Vector3 pos = transform.position;
        Vector3 rot = transform.eulerAngles;
        //更新位置
        if (delta > 0)
        {
            transform.position = Vector3.Lerp(pos, fPos, delta);
            transform.rotation = Quaternion.Lerp(Quaternion.Euler(rot),
                                                 Quaternion.Euler(fRot), delta);
        }

        //炮塔旋转
        TurretRotation();
        TurretRoll();
        //轮子履带马达音效
        NetWheelsRotation();
    }

    //同步炮塔炮管
    public void NetTurretTarget(float y, float x){
        turretRotTarget = y;
        turretRollTarget = x;
    }

    //处理轮子和履带
    public void NetWheelsRotation(){
        float z = transform.InverseTransformPoint(fPos).z;
        //判断坦克是否正在移动
        if (Mathf.Abs(z) < 0.1f || delta <= 0.05f){
            return;
        }
        //轮子
        foreach(Transform wheel in wheels){
            wheel.localEulerAngles += new Vector3(360 * z / delta, 0, 0);
        }
        //履带
        float offset = -wheels.GetChild(0).localEulerAngles.x / 90f;
        foreach(Transform track in tracks){
            MeshRenderer mr = track.gameObject.GetComponent<MeshRenderer>();
            if(mr == null){
                continue;
            }
            Material mtl = mr.material;
            mtl.mainTextureOffset = new Vector2(0, offset);
        }
    }

    //发送射击信息
    public void SendShootInfo(Transform bulletTrans){
        c2s_shooting_request request = new c2s_shooting_request();
        //位置旋转
        Vector3 pos = bulletTrans.position;
        Vector3 rot = bulletTrans.eulerAngles;
        request.pos_x = (Int32)(pos.x * 1000);
        request.pos_y = (Int32)(pos.y * 1000);
        request.pos_z = (Int32)(pos.z * 1000);
        request.rot_x = (Int32)(rot.x * 1000);
        request.rot_y = (Int32)(rot.y * 1000);
        request.rot_z = (Int32)(rot.z * 1000);
        byte[] data = NetMgr.servConn.CreateData((int)ProtoId.c2s_shooting_request, request);
		Debug.Log("发送协议 c2s_shooting_request ");
		NetMgr.servConn.SendMessage(data);
    }

    //产生炮弹
    public void NetShoot(Vector3 pos, Vector3 rot){
        //产生炮弹
        GameObject bulletObj = (GameObject)Instantiate(bullet, pos, Quaternion.Euler(rot));
        Debug.Log("NetShoot");
        Bullet bulletCmp = bulletObj.GetComponent<Bullet>();
        if(bulletCmp != null){
            bulletCmp.attckTank = gameObject;
        }
    }

    //发送伤害信息
    public void SendHit(UInt32 id, float damage){
        c2s_hit_request request = new c2s_hit_request();
        Debug.Log("SendHit id: " + id);
        request.enemy_id = id;
        request.damage = (Int32)(damage * 1000);
        byte[] data = NetMgr.servConn.CreateData((int)ProtoId.c2s_hit_request, request);
		Debug.Log("发送协议 c2s_hit_request ");
        NetMgr.servConn.SendMessage(data);
    }

    //被攻击
    public void NetBeAttacked(float att, GameObject attackTank) //#2攻击者
    {
        //扣除生命值
        if(hp <= 0){
            return;
        }
        if(hp > 0){
            hp -= att;
        }
        //坦克被摧毁
        if(hp <= 0){
            //改变操作模式
            ctrlType = CtrlType.none;
            //播放着火特效
            GameObject destroyObj = (GameObject)Instantiate(destoryEffect);
            destroyObj.transform.SetParent(transform, false);
            destroyObj.transform.localPosition = Vector3.zero;

            //播放击杀提示
            if(attackTank != null){
                Tank tankCmp = attackTank.GetComponent<Tank>();
                if(tankCmp != null && tankCmp.ctrlType == CtrlType.player){
                    tankCmp.StartDrawKill();
                }
            }
        }
    }
}