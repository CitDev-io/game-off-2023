using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface ICharacterAbility
{
    
}


/*
    What does a method need to know to resolve an ability?
    - DARK/LIGHT ability type
    - Who is the attacker?
    - Who is the defender? 
    - What are the current positions of the attacker and defender?
    - What is the current status of the LIGHT/DARK meter?
    - What are the current upgrades owned by the attacker?
    - What are the current upgrades owned by the defender?


    What can we ask a resolver about the ability to-be-cast?
    - Can the ability be cast?


    Player will:
    - pick an ability to cast
    - pick a target


    Things that might factor in:
    - upgrades
    - light vs light and dark vs dark
    - ability affects multiple targets
    - ability affects self
    - light/dark meter position
    - ability type (light/dark)
    - ability cost
    - ability cooldown



    SHIELD BASH
    - affects multiple enemies
    - applies STUNNED to target

    DEBUFF: STUNNED (1 turn) cannot act
    DEBUFF: VULNERABLE (3 turns) damage received increased by 40%
    DEBUFF: Ailment (3 turns) take damage each turn


    BACKSTAB
    - deal 180% damage and apply VULERNABLE to target

    HEAL
    - heal target ally for 5 health

    WITHERING BOLT
    - do basic damage and apply AILMENT to target
    
    
    
    BUFF: SHIELD (3 turns) damage received reduced by 40%
    BUFF: FOCUS (3 turns) damage dealt increased by 40%


*/


