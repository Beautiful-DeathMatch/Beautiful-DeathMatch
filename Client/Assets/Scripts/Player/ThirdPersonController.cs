using System;
using UnityEngine;

namespace StarterAssets
{
	public class ThirdPersonController : MonoBehaviour
    {
		private PlayerInputAsset inputAsset;

        private const float threshold = 0.01f;

        public event Action onHoldInteract = null;
        public event Action onCancelInteract = null;

        public event Action<int> onClickNumber = null;
        public event Action onClickUse = null;
        public event Action onStartAim = null;
        public event Action onEndAim = null;

        public event Action<float, float> onRotate = null;
        public event Action onJump = null;
        public event Action<bool, bool, Vector2> onMove = null;
        public event Action<bool, bool, Vector2> onSwim = null;

        public event Func<bool> IsInWater;

        public event Func<bool> IsNotYetJump;
        public event Func<bool> IsGrounded;
        public event Func<bool> IsUIOpened;

		private bool isInteracting = false;
		private bool isAimming = false;

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
				inputAsset.onUseItem -= OnUseItem;
				inputAsset.onJump -= OnJump;
				inputAsset.onClickNumber -= OnInputNumber;
			}
		}

		private void SetInputActions()
		{
			ClearInputActions();

			if (inputAsset != null)
			{
				inputAsset.onUseItem += OnUseItem;
				inputAsset.onJump += OnJump;
				inputAsset.onClickNumber += OnInputNumber;
			}			
		}

		private void Update()
        {
            if (inputAsset)
            {
				CheckMove();
				CheckInteract();
				CheckSwim();
				CheckAim();
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

        private void CheckRotation()
        {
            float yaw = 0.0f;
            float pitch = 0.0f;

            if (inputAsset.lookDir.sqrMagnitude >= threshold)
            {
				yaw += inputAsset.lookDir.x; // * Time.deltaTime;
				pitch += inputAsset.lookDir.y; // * Time.deltaTime;
			}

			onRotate?.Invoke(yaw, pitch);
		}

        private void CheckMove()
        {
			if (IsUIOpened())
				return;
			
			if (IsInWater())
				return;

            onMove?.Invoke(inputAsset.isSprint, inputAsset.analogMovement, inputAsset.moveDir);
        }


        private void CheckSwim()
        {
			if (IsUIOpened())
				return;
			
			if (!IsInWater())
				return;

            onSwim?.Invoke(inputAsset.isSprint, inputAsset.analogMovement, inputAsset.moveDir);
		}

		private void CheckInteract()
		{
			if (inputAsset.isInteract && !IsUIOpened())
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

		// InputSystem에서 Jump 버튼이 입력되면 함수가 실행된다.
		private void OnJump()
		{
			if (IsUIOpened())
				return;

			if (IsGrounded())
			{
				if (IsNotYetJump())
				{
					onJump?.Invoke();
				}
			}
		}

		private void OnInputNumber(int number)
        {
			if (IsUIOpened())
				return;

			onClickNumber?.Invoke(number);
		}

		private void CheckAim()
		{
			if (inputAsset.isAim && !IsUIOpened())
			{
				if (isAimming)
					return;

				onStartAim?.Invoke();
				isAimming = true;
			}
			else if (isAimming)
			{
				onEndAim?.Invoke();
				isAimming = false;
			}
				
		}

		/// <summary>
		/// Left Click은 기획상 공격이 아니라 '아이템 사용' 입니다.
		/// Gun, Knife를 장착하고 있을 때 해당 아이템을 사용한다면 총알이 나가는 형태로 로직을 구성합니다.
		/// </summary>
		private void OnUseItem()
        {
			if (IsUIOpened())
				return;

			if (IsInWater())
				return;

			onClickUse?.Invoke();
		}
	}
} 