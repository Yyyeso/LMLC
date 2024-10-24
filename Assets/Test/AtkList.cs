using UnityEngine;
using System.Collections.Generic;

public class AtkList : MonoBehaviour
{
    [SerializeField] List<TestRange> atkList;
    public List<TestRange> AttackList => atkList;
}