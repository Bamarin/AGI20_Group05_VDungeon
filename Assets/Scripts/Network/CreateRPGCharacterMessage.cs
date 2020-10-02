using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CreateRPGCharacterMessage : MessageBase
{
    public Race race;
    public string name;
    public Color skinColor;
    public RPGClass characterClass;
}

public enum Race
{
    None,
    Elvish,
    Dwarvish,
    Human
}

public enum RPGClass
{
    Fighter = 0,
    Wizard = 1
}
