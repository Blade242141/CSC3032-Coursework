using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Command", menuName = "New Command")]
public class Command : ScriptableObject {
    public new string name; // Name of command, this name is used to call the function that shares the same name
    public string[] keyword; // This is the keyword that the user must say to activate the command
    public string desc; // Brief description of the comand

    public enum CommandTypes { movement, data, music } // The scipts available to choose from

    public CommandTypes commandType; // The chosen script
}
