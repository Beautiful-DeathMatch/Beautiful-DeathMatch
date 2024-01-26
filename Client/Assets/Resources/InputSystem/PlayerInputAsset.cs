using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInputAsset : MonoBehaviour
{
	public Vector2 moveDir { get; private set; }
	public Vector2 lookDir { get; private set; }

	public bool isSprint { get; private set; }	

	[Header("Movement Settings")]
	public bool analogMovement;

	[Header("Mouse Cursor Settings")]
	public bool cursorLocked = true;
	public bool cursorInputForLook = true;

	public event Action onJump = null;
	public event Action onSprint = null;
	public event Action onAttack = null;
	public event Action onInteract = null;
	public event Action<int> onClickNumber = null;

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
	}

	public void OnSprint(InputValue value)
	{
		isSprint = value.isPressed;
	}

	public void OnAttack(InputValue value)
	{
		onAttack?.Invoke();
	}

	public void OnInteract(InputValue value)
	{
		onInteract?.Invoke();
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		SetCursorState(cursorLocked);
	}

	private void SetCursorState(bool newState)
	{
		Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
	}
}