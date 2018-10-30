using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestProject.Gameplay.Views {

    public class BombView : View {

        // These parameters must be provided from SO object, but to simplify the setup - leave them here
        public float throwHeight = 10f;      // in meters
        public float throwSpeed = 1f;        // 1 meter per second
        public float damage = 10f;
        public float damageRange = 2f;
        
    }

}