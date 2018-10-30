using UnityEngine;
using System.Collections.Generic;
using views = TestProject.Gameplay.Views;

namespace TestProject.Gameplay {

    public class InputModule : ModuleBase {

        public InputModule(Battle battle) : base(battle) { }

        public bool DoUnitSpawn(int viewIndex, Vector3 position) {

            return this.battle.gameplay.SpawnUnit(this.battle.unitsConfig.GetViewByIndex(viewIndex), position);

        }

    }

}