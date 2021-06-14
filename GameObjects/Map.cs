using LimitedBudgetMonojam.GameObjects.PhysicsObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using static LimitedBudgetMonojam.Editor.LevelEditor;

namespace LimitedBudgetMonojam.GameObjects
{
    class Map : GameObject
    {
        
        public Map(Level lvl) : base(new Position(0, 0), default)
        {
            foreach (var s in lvl.Obstacles)
                Colliders.Add(s);
        }
    }
}
