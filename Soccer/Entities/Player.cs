using System;
using System.Collections.Generic;
using System.Text;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;
using FlatRedBall.Math.Geometry;
using Microsoft.Xna.Framework;

namespace Soccer.Entities
{
    public partial class Player
    {
        private bool _wasMoving;
        private bool _wasOnGround;
        
        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
        private void CustomInitialize()
        {
            JumpAction = () => SpriteInstance.CurrentChainName = "JumpUp";
        }

        private void CustomActivity()
        {
            var isMoving = Velocity != Vector3.Zero;
            if (!IsOnGround)
            {
                if (Velocity.Y > 0)
                {
                    SpriteInstance.CurrentChainName = "JumpUp";
                }
                else
                {
                    SpriteInstance.CurrentChainName = "DropDown";
                }
            }
            else
            {
                if (!_wasOnGround || _wasMoving != isMoving)
                {
                    // We were in the air but now on the ground, or we were on the ground but changed
                    // if we were moving.  Therefore we need to change up the animation
                    SpriteInstance.CurrentChainName = isMoving ? "Run" : "Idle";
                }
            }

            _wasMoving = isMoving;
            _wasOnGround = IsOnGround;
            SpriteInstance.FlipHorizontal = DirectionFacing == HorizontalDirection.Left;
        }

        private void CustomDestroy()
        {
        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {
        }
    }
}
