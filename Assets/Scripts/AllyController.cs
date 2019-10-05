﻿using Cinemachine;
using Doozy.Engine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyController : MonoBehaviour
{
    public float maxMasterHealth;
    public float masterHealth;
    public float allyRadius = 6f;
    public Ally[] allyPrefabs;
    public List<Ally> allies = new List<Ally>();
    public CinemachineStateDrivenCamera camRig;
    public CombatMaster CM;
    public Animator camAnim;
    private float angle = Mathf.PI / 3;
    public EnemyController enemies;
    void Start()
    {
        PositionAllies();
        CM = GameObject.FindGameObjectWithTag("CombatMaster").GetComponent<CombatMaster>();
        camAnim = GetComponent<Animator>();
        camAnim.SetInteger("sweep", -1);
        camAnim.SetBool("roundEnd", false);
        enemies = GameObject.FindGameObjectWithTag("EnemyController").GetComponent<EnemyController>();
        camRig = GameObject.FindGameObjectWithTag("CamRig").GetComponent<CinemachineStateDrivenCamera>();
    }


    void Update()
    {

    }

    public void PositionAllies()
    {
        Dictionary<int, Ally.allyClass> allyAndIndex = loadAllyPosition();
        foreach (KeyValuePair<int, Ally.allyClass> pair in allyAndIndex)
        {
            int allyIndex = pair.Key;
            int allyClass = (int)pair.Value;

            float allyAngle = angle * allyIndex;
            float x = Mathf.Sin(allyAngle) * allyRadius;
            float z = Mathf.Cos(allyAngle) * allyRadius;

            Ally ally = Instantiate(allyPrefabs[allyClass], transform, this);
            UICanvas canvas = ally.GetComponentInChildren<UICanvas>();
            canvas.CanvasName = "Ally" + allyIndex * 100;
            canvas.name = canvas.CanvasName;

            ally.transform.localPosition = new Vector3(x, 0, z);
            ally.transform.localRotation = Quaternion.Euler(0, allyAngle * (180f / Mathf.PI), 0);
            ally.sectorIndex = allyIndex;
            ally.unitType = (Ally.allyClass)allyClass;
            allies.Add(ally);
        }
    }

    public Dictionary<int, Ally.allyClass> loadAllyPosition()
    {
        Dictionary<int, Ally.allyClass> allyAndIndex = new Dictionary<int, Ally.allyClass>
        {
            { 0, Ally.allyClass.Knight },
            { 1, Ally.allyClass.Knight },
            { 2, Ally.allyClass.Knight },
            { 3, Ally.allyClass.Knight },
            { 4, Ally.allyClass.Knight },
            { 5, Ally.allyClass.Knight },
        };

        return allyAndIndex;
    }
    public void Combat()
    {
        StartCoroutine(StartCombat());
    }
    IEnumerator StartCombat()
    {
        camAnim.SetBool("roundEnd", true);
        List<int> indexes = new List<int>();
        foreach (Ally a in allies)
        {
            indexes.Add(a.sectorIndex);
        }
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < 6; i++)
        {
            if (indexes.Contains(i))
            {
                camAnim.SetInteger("sweep", i);
                yield return new WaitForSeconds(1f);
                foreach (Ally a in allies)
                {
                    if (a.sectorIndex == i)
                    {
                        a.Combat();
                    }
                }
                yield return new WaitForSeconds(1.5f);
            }
        }
        camAnim.SetInteger("sweep", -1);
        yield return new WaitForSeconds(2f);
        enemies.Combat();
        yield return new WaitForSeconds(1f);
        camAnim.SetBool("roundEnd", false);
        yield return new WaitForSeconds(1f);
        CM.RoundStart();
    }
    public void RoundStart()
    {
        foreach (Ally a in allies)
        {
            a.RoundStart();
        }
        enemies.RoundStart();
    }
}
