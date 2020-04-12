using System;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Math;
using Microsoft.Xna.Framework;

namespace Soccer.Screens
{
    public partial class GameScreen
    {
        private bool _isInKickSelectionMode;

        void CustomInitialize()
        {
            Camera.Main.AttachTo(PlayerInstance);
            Camera.Main.RelativeZ = 100;
        }

        void CustomActivity(bool firstTimeCalled)
        {
            if (InputManager.Xbox360GamePads[0].ButtonDown(Xbox360GamePad.Button.RightTrigger))
            {
                if (PlayerInstance.BallCatchArea.CollideAgainst(Ball1.Collision))
                {
                    if (!_isInKickSelectionMode)
                    {
                        EnableKickDirectionSelection();
                    }
                }
                else
                {
                    if (_isInKickSelectionMode)
                    {
                        DisableKickDirectionSelection();
                    }
                }
            }
            else
            {
                if (_isInKickSelectionMode)
                {
                    DisableKickDirectionSelection();
                }
            }

            if (_isInKickSelectionMode)
            {
                // Control ball arrow via analog stick
                Ball1.KickIndicatorInstance.RelativeRotationZ = (float) InputManager.Xbox360GamePads[0].LeftStick.Angle;
            }
        }

        void CustomDestroy()
        {
        }

        static void CustomLoadStaticContent(string contentManagerName)
        {
        }

        private void EnableKickDirectionSelection()
        {
            TimeManager.TimeFactor = 1 / 10f;
            PlayerInstance. InputEnabled = false;
            _isInKickSelectionMode = true;
            Ball1.KickIndicatorInstance.SpriteInstanceVisible = true;
        }

        private void DisableKickDirectionSelection()
        {
            TimeManager.TimeFactor = 1;
            PlayerInstance.InputEnabled = true;
            _isInKickSelectionMode = false;
            Ball1.KickIndicatorInstance.SpriteInstanceVisible = false;
            
            // If we are still colliding with the ball, then we are stopping kick selection
            // due to releasing the button, so we want to propel the ball in the specified direction
            if (PlayerInstance.BallCatchArea.CollideAgainst(Ball1.Collision))
            {
                var angle = InputManager.Xbox360GamePads[0].LeftStick.Angle;
                var ballVelocity = new Vector3(
                    (float)Math.Cos(angle) * PlayerInstance.KickVelocity,
                    (float)Math.Sin(angle) * PlayerInstance.KickVelocity,
                    0);
                
                var kickBackVelocity = new Vector3(
                    (float)Math.Cos(angle) * PlayerInstance.KickBackVelocity,
                    (float)Math.Sin(angle) * PlayerInstance.KickBackVelocity,
                    0); 
                
                Ball1.Velocity = ballVelocity;
                PlayerInstance.Velocity += -kickBackVelocity;
            }
        }
    }
}
