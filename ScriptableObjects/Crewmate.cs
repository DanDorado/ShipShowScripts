using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "Data", menuName = "Crewmate", order = 53)]
public class Crewmate : ScriptableObject
{
    public Head head;
    public Body body;
}