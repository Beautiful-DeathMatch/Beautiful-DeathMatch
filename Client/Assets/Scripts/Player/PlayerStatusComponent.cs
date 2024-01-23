using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{

}

/// <summary>
/// 데미지 및 회복을 관리합니다.
/// </summary>
public class PlayerStatusComponent : MonoComponent<StatusSystem>, IDamageable
{

}
