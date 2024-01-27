using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    public class ThirdPersonController : MonoBehaviour
    {
		// animation IDs
        int _animIDGrounded = Animator.StringToHash("Grounded");
        int _animIDJump = Animator.StringToHash("Jump");
        int _animIDFreeFall = Animator.StringToHash("FreeFall");

        int _animIDItemOffset = Animator.StringToHash("ItemOffset");
        int _animIDAttack = Animator.StringToHash("Attack");
        int _animIDSwim = Animator.StringToHash("Swim");

		private PlayerInputAsset inputAsset;
		private Animator _animator;

        private const float _threshold = 0.01f;

        public event Action onHoldInteract = null;
        public event Action onCancelInteract = null;

        public event Action<int> onClickNumber = null;
        public event Action onClickUse = null;

        public event Action<float, float> onRotate = null;
        public event Action onJump = null;
        public event Action<bool, bool, Vector2> onMove = null;

        public event Func<bool> IsInWater;

        public event Func<bool> IsNotYetJump;
        public event Func<bool> IsFallTimeout;
        public event Func<bool> IsGrounded;

		private bool isInteracting = false;

        public void SetAnimatorController(RuntimeAnimatorController controller, Avatar avatar)
		{
			_animator = GetComponent<Animator>();
			_animator.runtimeAnimatorController = controller;
            _animator.avatar = avatar;

			CharacterControllerState.Initialize(_animator);
		}

		public void SetInput(PlayerInputAsset inputAsset)
        {
            this.inputAsset = inputAsset;

			SetInputActions();
		}

		private void OnEnable()
		{
			SetInputActions();
		}

		private void OnDisable()
		{
			ClearInputActions();
		}

		private void ClearInputActions()
		{
			if (inputAsset != null)
			{
				inputAsset.onAttack -= OnInputAttack;
				inputAsset.onJump -= OnJump;
				inputAsset.onClickNumber -= OnInputNumber;
			}
		}

		private void SetInputActions()
		{
			ClearInputActions();

			if (inputAsset != null)
			{
				inputAsset.onAttack += OnInputAttack;
				inputAsset.onJump += OnJump;
				inputAsset.onClickNumber += OnInputNumber;
			}			
		}

		private void Update()
        {
            if (inputAsset)
            {
				// 상시 적용이라 얘넨 스테이트로 안 가도 됨
				CheckGrounded();
				CheckFall();

                // 얘네 가도 됨
				CheckMove();
				CheckInteract();
				CheckSwim();
			}
        }

        private void LateUpdate()
        {
            if (inputAsset)
			{
                // 얘도 가도 됨
				CheckRotation();
			}
		}

        private void CheckGrounded()
        {
            bool isGrounded = IsGrounded();
            if (isGrounded)
            {
				_animator.SetBool(_animIDFreeFall, false);
			}

			_animator.SetBool(_animIDGrounded, isGrounded);
			_animator.SetBool(_animIDJump, false);
		}

        private void CheckRotation()
        {
            float yaw = 0.0f;
            float pitch = 0.0f;

            if (inputAsset.lookDir.sqrMagnitude >= _threshold)
            {
				yaw += inputAsset.lookDir.x; // * Time.deltaTime;
				pitch += inputAsset.lookDir.y; // * Time.deltaTime;
			}

			onRotate?.Invoke(yaw, pitch);
		}

        private void CheckMove()
        {
            onMove?.Invoke(inputAsset.isSprint, inputAsset.analogMovement, inputAsset.moveDir);
        }

        private void CheckFall()
        {
			if (IsGrounded() == false)
			{
				if (IsFallTimeout())
				{
					_animator.SetBool(_animIDFreeFall, true);
				}
            }
        }

        private void CheckSwim()
        {
            if (IsInWater() == false)
                return;

			_animator.SetBool(_animIDSwim, true);
		}

		private void CheckInteract()
		{
			if (inputAsset.isInteract)
			{
                onHoldInteract?.Invoke();
                isInteracting = true;
			}
			else if (isInteracting)
			{
				onCancelInteract?.Invoke();
				isInteracting = false;
			}
		}

		private void OnJump()
		{
			if (IsGrounded())
			{
				if (IsNotYetJump())
				{
					onJump?.Invoke();
					_animator.SetBool(_animIDJump, true);
				}
			}
		}


		private void OnInputNumber(int number)
        {
			_animator.SetInteger(_animIDItemOffset, number);
		}

		/// <summary>
		/// Left Click은 기획상 공격이 아니라 '아이템 사용' 입니다.
		/// Gun, Knife를 장착하고 있을 때 해당 아이템을 사용한다면 총알이 나가는 형태로 로직을 구성합니다.
		/// </summary>
		private void OnInputAttack()
        {
			onClickUse?.Invoke();
			// _animator.SetBool(_animIDAttack, true);
		}
	}
}