using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NpcStatusComponent : MonoComponent<StatusSystem>, IDamageable
{
    [SerializeField] private int npcId;
    [SerializeField] private string npcName;
    [SerializeField] private TextMeshPro NameText;
    [SerializeField] private TextMeshPro HpText;

	public void RequestTakeDamage(int attackerPlayerId, int damage)
	{
		if (System.TryNpcChangeHealth(attackerPlayerId, -damage) == false)
			return;

		return;
	}

    public int GetNpcId()
    {
        return npcId;
    }

    private void OnEnable() 
    {
        NameText.text = npcName;
    }

    private void Update() 
    {
        HpText.text = System.GetNpcHealth(npcId).ToString();
    }

}
