using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunController : MonoBehaviour
{

	PlayerController player;
	Rigidbody2D playerRb;
	// Use this for initialization
	void Start()
	{
		//test stuff
		rocketTime = _rocketTime;
		reloadTime = _reloadTime;
		//
		player = GetComponentInParent<PlayerController>();
		playerRb = GetComponentInParent<Rigidbody2D>();
		rocketTimer = rocketTime;

		curGrav = player.Gravity; // изменить на приватную gravity
	}

	// Update is called once per frame
	void Update()
	{
		//test stuff
		rocketTime = _rocketTime;
		//
		
	}

	void FixedUpdate()
	{
		RocketJump(rocketSpeed);
	}

	//test stuff
	public float rocketSpeed;
	float curGrav;
	public float rocketGravityCoff;
	public float _rocketTime;
	private float rocketTime;
	private float rocketTimer;
	private float rocketJumpCount;

	public float _reloadTime;
	private float reloadTime;
	private float reloadTimer;

	private bool isRocketJump;
	private bool canRocketJump = true;
	private bool isFirstAdd = false;
	public void RocketJump(float speed)
	{
		Vector2 cursorPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		//Vector2 moveOffCursor = (cursorPos - (Vector2)transform.position);
		Vector2 moveOffCursor = -transform.up;
		Vector2 rocket = Quaternion.Euler(0, 0, player.Movement) * moveOffCursor.normalized;
		transform.rotation = Quaternion.Euler(0, 0, Mathf.Lerp(transform.rotation.z ,player.Movement * -90, 0.1f));
		Debug.DrawRay(transform.position, rocket);

		Vector2 rocketNow = Vector2.zero;

		if (Input.GetMouseButtonDown(1))
		{
			if (canRocketJump) // если можно делать Rocket Jump
			{
				isRocketJump = true; // указываем, что совершается Rocket Jump
				player.smoothWalk = true; // указываем, что нужно применять плавный способ ходьбы героя
				playerRb.velocity = Vector2.zero; //обнуляем velocity героя
				if (!isFirstAdd)
				{
					isFirstAdd = true;
					rocketJumpCount++; // добавляем + 1 к числу совершенных Rocket Jump
				}
				reloadTimer = reloadTime; // устанавливаем время перезарядки
				rocketTimer = rocketTime; // задаем время действия импульса
				rocketNow = -rocket; // задаем вектор импулься

				if (player.Gravity == curGrav) // если гравитация не снижена
				{
					player.Gravity = curGrav / rocketGravityCoff; // уменьшаем гравитацию
				}
				
				if (rocketJumpCount == 2)// если Rocket Jump совершается 2й раз
				{
					canRocketJump = false; // и выключаем возможность совершать Rocket Jump
				}
				
			}
		}
		else if(Input.GetMouseButtonUp(1))
		{
			isFirstAdd = false;
		}

		if (isRocketJump) // если совершается Rocket Jump
		{
			if (rocketTimer > 0) // если время действия импульса не вышло
			{
				rocketTimer -= Time.deltaTime; // уменьшаем таймер
				playerRb.AddForce(rocketNow * speed , ForceMode2D.Impulse); // добавляем импульс к RigidBody героя
			}
			else if (rocketTimer <= 0) // иначе
			{
				isRocketJump = false; // говорим, что Rocket Jump завершен
			}
		}
		else // иначе
		{
			
		}

		if (reloadTimer > 0) // если время перезарядки не вышло
		{
			reloadTimer -= Time.deltaTime; // уменьшаем таймер
		}
		else // иначе
		{
			if (rocketJumpCount > 0) // если число совершенных Rocket Jump больше 0
			{
				player.Gravity = curGrav; // возвращаем гравитацию в исходное состояние
				rocketJumpCount = 0; // обнуляем счетчик Rocket Jump
				canRocketJump = true; // возвращаем возможность делать Rocket Jump
			}
		}

		if (player.IsGrounded && rocketTimer < rocketTime - 0.1f) // если герой заземлен и прошло немного времени 
																  //с момента начала Rocket Jump
		{
			player.smoothWalk = false; // указываем, что нужно применять жесткий способ ходьбы героя
			player.Gravity = curGrav; // возвращаем гравитацию в исходное состояние
		}
	}


}
