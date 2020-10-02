using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CreateRPGCharacterMessage : MessageBase
{
    public Race race;
    public string name;
    public Color skinColor;
    public Class characterClass;
}

public enum Race
{
    None,
    Elvish,
    Dwarvish,
    Human
}

public enum Class
{
    Barbarian,
    Bard,
    Cleric,
    Druid,
    Fighter,
    Monk,
    Paladin,
    Ranger,
    Rogue,
    Sorcerer,
    Warlock,
    Wizard
}
