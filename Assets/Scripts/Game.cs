using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public TestProject.MapEditor.Map map;
    public TestProject.Database.UnitsConfig unitsConfig;
    public TestProject.Database.BombsConfig bombsConfig;
    
    private TestProject.Gameplay.Battle battle;
    
    public void OnGUI() {

        if (this.battle == null) {

            if (GUILayout.Button("Initialize") == true) {

                this.battle = new TestProject.Gameplay.Battle(this.map.mapInfo, this.unitsConfig, this.bombsConfig);

            }

        } else {

            GUILayout.Label("Battle state: " + this.battle.state);
            if (this.battle.state == TestProject.Gameplay.Battle.State.Playing) {

                if (GUILayout.Button("Spawn Unit") == true) {

                    if (this.battle.input.DoUnitSpawn(Random.Range(0, this.unitsConfig.GetCount()), this.battle.map.GetRandomPointOnLandAndEmpty()) == true) { 
                    
                        Debug.Log("Spawned");
                    
                    }

                }

                GUILayout.Label("--------------");

                if (GUILayout.Button("Pause") == true) {

                    this.battle.Pause();
                
                }

            } else if (this.battle.state == TestProject.Gameplay.Battle.State.Paused) {

                if (GUILayout.Button("Resume") == true) {

                    this.battle.Resume();

                }
            
            } else if (this.battle.state == TestProject.Gameplay.Battle.State.Initialized) {

                if (GUILayout.Button("Start") == true) {

                    this.battle.Start();

                }
                
            }

            if (GUILayout.Button("Stop") == true) {

                this.battle.Stop();
                this.battle = null;
                UnityEngine.SceneManagement.SceneManager.LoadScene("Main");

            }
            
        }
        
    }

    public void Update() { 
    
        if (this.battle != null) this.battle.Update(Time.deltaTime);
    
    }

}
