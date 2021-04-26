using ScriptableObjects;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private SettingsParams settingsParams;
    [SerializeField] private AgentSpawner agentSpawner;
    
    
    // private void fetchAgent(AgentType agentType, int difficulty){}
    
    // processes settings params and supplies it the the game Manager
    // Takes requests from the GameManager  
    // If one of the players is an agent, require additional AgentType Argument
    // agent spawner has links to all types of agents in all their difficulty settings
    // settings manager fetches the relevant agent from the agent spawner
    // and supplies it back to the gameManager that ultimately spawns the players
    
    // GameManager Params should also get merged to this
    // needs getters setters to enable the values to be edited through the UI 
    // Settings page
    
    // Hallalujah! Thats the plan here's to hoping that it comes to fruition 
    // withing today
    
}
