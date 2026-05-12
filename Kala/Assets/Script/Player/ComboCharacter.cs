using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboCharacter : MonoBehaviour
{

    private StateMachine meleeStateMachine;

    [SerializeField] public Collider2D hitbox;
    //[SerializeField] public GameObject Hiteffect;

    // Start is called before the first frame update
    void Start()
    {
        meleeStateMachine = GetComponent<StateMachine>();
    }

    // Update is called once per frame
    void Update()
    {/*
        if (Input.GetMouseButton(0) && meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState))
        {
            meleeStateMachine.SetNextState(new GroundEntryState());
        }
*/
        if (Input.GetMouseButton(0))
    {
        Debug.Log("Mouse button pressed, current state: " + meleeStateMachine.CurrentState?.GetType());
        if (meleeStateMachine.CurrentState.GetType() == typeof(IdleCombatState))
        {
            Debug.Log("Triggering GroundEntryState");
            meleeStateMachine.SetNextState(new GroundEntryState());
        }
    }
    }
}