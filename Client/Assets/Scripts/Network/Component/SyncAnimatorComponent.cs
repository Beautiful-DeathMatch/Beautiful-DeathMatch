using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncAnimatorComponent : SyncComponent
{
    [SerializeField] private Animator animator = null;
    
    private static readonly int Grounded = Animator.StringToHash("Grounded");
    private static readonly int Jump = Animator.StringToHash("Jump");
    private static readonly int MotionSpeed = Animator.StringToHash("MotionSpeed");
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int FreeFall = Animator.StringToHash("FreeFall");

	protected override bool IsSendCondition()
	{
        return animator.runtimeAnimatorController != null;
	}

	protected override void TrySend()
    {
        var packet = new REQ_ANIMATOR();
        
        packet.Grounded = animator.GetBool(Grounded) ? 1 : 0;
        packet.FreeFall = animator.GetBool(FreeFall) ? 1 : 0;
        packet.Jump = animator.GetBool(Jump) ? 1 : 0;
        packet.MotionSpeed = animator.GetFloat(MotionSpeed);
        packet.speed = animator.GetFloat(Speed);
        
        SessionManager.Instance.Send(packet);
    }
    
    public override void OnReceive(IPacket packet)
    {
        if (packet is RES_ANIMATOR animatorPacket == false)
            return;

        if (animatorPacket.playerId != playerId)
            return;
        
        animator.SetFloat(Speed, animatorPacket.speed);
        animator.SetFloat(MotionSpeed, animatorPacket.MotionSpeed);

        animator.SetBool(Jump, animatorPacket.Jump > 0);
        animator.SetBool(Grounded, animatorPacket.Grounded > 0);
        animator.SetBool(FreeFall, animatorPacket.FreeFall > 0);
    }
    
}
