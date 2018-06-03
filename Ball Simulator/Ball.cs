using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Ball_Simulator
{
    public class Ball
    {
        #region Fields

        public double mass;
        public double radius;

        // coordinates
        public double x;
        public double y;

        // velocity
        public double velocity_x;
        public double velocity_y;
        
        // bounciness
        public double elasticity;
        public double coefficientOfRestitution;

        // gravity
        public double gravity;

        // states
        public bool isAtRest;
        public bool isRolling;

        // friction
        public double coefficientOfRollingFriction;

        #endregion

        #region Constructors

        public Ball(
            double mass,
            double radius,
            double initial_x,
            double initial_y,
            double initialVelocity_x,
            double initialVelocity_y,
            double elasticity,
            double restitution,
            double gravity,
            double rollingFriction)
        {
            this.mass = mass;
            this.radius = radius;
            this.x = initial_x;
            this.y = initial_y;
            this.velocity_x = initialVelocity_x;
            this.velocity_y = initialVelocity_y;
            this.elasticity = elasticity;
            this.coefficientOfRestitution = restitution;
            this.gravity = gravity;
            this.coefficientOfRollingFriction = rollingFriction;
        }

        #endregion

        #region Methods

        public void Move(double timeIncrement, double boundWidth, double boundHeight)
        {
            velocity_x += 0;
            velocity_y += gravity * timeIncrement;


            if (WillBeOutsideXBound(timeIncrement, boundWidth) &&
                WillBeOutsideYBound(timeIncrement, boundHeight))
            {
                BounceX(timeIncrement, boundWidth);
                BounceY(timeIncrement, boundHeight);
            }
            else if (WillBeOutsideXBound(timeIncrement, boundWidth) &&
                    !WillBeOutsideYBound(timeIncrement, boundHeight))
            {
                y += velocity_y * timeIncrement;
                BounceX(timeIncrement, boundWidth);
            }
            else if (WillBeOutsideYBound(timeIncrement, boundHeight) &&
                    !WillBeOutsideXBound(timeIncrement, boundWidth))
            {
                x += velocity_x * timeIncrement;
                BounceY(timeIncrement, boundHeight);
            }
            else
            {
                x += velocity_x * timeIncrement;
                y += velocity_y * timeIncrement;
            }

            if (IsBallRolling())
            {
                AdjustForFriction(timeIncrement);
            }
        }

        public void BounceX(double timeIncrement, double boundWidth)
        {
            double energyLostToElasticCompression = 1;
            double postBounceVelocity_x = 0;

            #region Hits left wall
            if (velocity_x < 0)
            {
                postBounceVelocity_x =
                    Math.Sqrt(
                        Math.Pow(coefficientOfRestitution, 2) * Math.Pow(velocity_x, 2));
                
                velocity_x = Math.Max(postBounceVelocity_x - energyLostToElasticCompression, 0);
            }
            #endregion

            #region Hits right wall
            else if (velocity_x > 0)
            {
                postBounceVelocity_x =
                    -Math.Sqrt(
                        Math.Pow(coefficientOfRestitution, 2) * Math.Pow(velocity_x, 2));

                velocity_x = Math.Min(postBounceVelocity_x + energyLostToElasticCompression, 0);
            }
            #endregion
        }

        public void BounceY(double timeIncrement, double boundHeight)
        {
            double energyLostToElasticCompression = 1;
            
            double postBounceVelocity_y =
                Math.Sqrt(
                    Math.Pow(coefficientOfRestitution, 2) * Math.Pow(velocity_y, 2));
            
            velocity_y = Math.Max(postBounceVelocity_y - energyLostToElasticCompression, 0);
        }
        
        public bool WillBeOutsideXBound(double timeIncrement, double boundWidth)
        {
            double d_x = velocity_x * timeIncrement;

            double newXCoord = x + d_x;

            double boundLeft = 0;
            double boundRight = boundWidth;

            if (newXCoord + radius > boundRight || newXCoord + 0 < boundLeft)
                return true;

            return false;
        }

        public bool WillBeOutsideYBound(double timeIncrement, double boundHeight)
        {
            double d_y = velocity_y * timeIncrement;
            
            double newYCoord = y + d_y;
            
            double boundFloor = 0;
            
            if (newYCoord - radius < boundFloor)
                return true;

            return false;
        }

        private bool IsBallRolling()
        {
            // if y acceleration = 0
            // and if y = 0
            // and if x velocity != 0
            // return true

            if (velocity_x != 0 && 
                y < radius + (radius * 0.2))
            {
                return true;
            }

            return false;
        }

        private void AdjustForFriction(double timeIncrement)
        {
            if (velocity_x > 0)
                velocity_x = Math.Max(0, -(coefficientOfRollingFriction * gravity * -timeIncrement) + velocity_x);
            if (velocity_x < 0)
                velocity_x = Math.Min(0, (coefficientOfRollingFriction * gravity * -timeIncrement) + velocity_x);
        }

        #endregion
    }
}
