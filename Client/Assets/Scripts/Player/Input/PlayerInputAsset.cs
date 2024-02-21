using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInputAsset : MonoBehaviour
{
	public Vector2 moveDir { get; private set; }
	public Vector2 lookDir { get; private set; }

	public bool isSprint { get; private set; }	
	public bool isInteract { get; private set; }
	public bool isAim { get; private set; }

	[Header("Movement Settings")]
	public bool analogMovement;

	[Header("Mouse Cursor Settings")]
	public bool cursorLocked = true;
	public bool cursorInputForLook = true;

	public event Action onJump = null;
	public event Action onSprint = null;
	public event Action onUseItem = null;
	public event Action<int> onClickNumber = null;
	public event Action onAim = null;

	public void OnMove(InputValue value)
	{
		moveDir = value.Get<Vector2>();
	}

	public void OnLook(InputValue value)
	{
		if (cursorInputForLook)
		{
			lookDir = value.Get<Vector2>();
		}
	}

	public void OnJump(InputValue value)
	{
		onJump?.Invoke();
		Debug.Log("점프 키 눌림");
	}

	public void OnSprint(InputValue value)
	{
		isSprint = value.isPressed;
		if (isSprint)
		{
			Debug.Log("스프린트 시작");
		}
		else
		{
			Debug.Log("스프린트 종료");
		}
	}

	public void OnUseItem(InputValue value)
	{
		onUseItem?.Invoke();
		Debug.Log("공격 키 눌림");
	}

	public void OnInteract(InputValue value)
	{
		isInteract = value.isPressed;
	}

	public void OnNumber1(InputValue value)
	{
		onClickNumber?.Invoke(1);
		Debug.Log($"숫자 키 1 눌림");
	}

	public void OnNumber2(InputValue value)
	{
		onClickNumber?.Invoke(2);
		Debug.Log($"숫자 키 2 눌림");
	}

	public void OnNumber3(InputValue value)
	{
		onClickNumber?.Invoke(3);
		Debug.Log($"숫자 키 3 눌림");
	}

	public void OnNumber4(InputValue value)
	{
		onClickNumber?.Invoke(4);
		Debug.Log($"숫자 키 4 눌림");
	}

	public void OnNumber5(InputValue value)
	{
		onClickNumber?.Invoke(5);
		Debug.Log($"숫자 키 5 눌림");
	}

	public void OnAim(InputValue value)
	{
		isAim = value.isPressed;
		if (isAim)
		{
			Debug.Log("조준 시작");
		}
		else
		{
			Debug.Log("조준 종료");
		}
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		SetCursorState(cursorLocked);
	}

	private void SetCursorState(bool newState)
	{
		Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
	}

	public void ChangeCursorStateAndLook(bool newCursorLocked)
	{
		cursorInputForLook = newCursorLocked;
		cursorLocked = newCursorLocked;
		SetCursorState(cursorLocked);
	}

}