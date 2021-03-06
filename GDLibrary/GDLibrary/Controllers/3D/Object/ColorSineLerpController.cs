﻿/*
Function: 		Lerps target actor's color between two user specified colors. Can be used to decorate static object and give it a visually interesting behaviour.
Author: 		NMCG
Version:		1.0
Date Updated:	24/10/17
Bugs:			None
Fixes:			None
*/

using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class ColorSineLerpController : SineLerpController
    {
        #region Fields
        private int totalElapsedTime;
        private Color startColor;
        private Color endColor;
        #endregion

        #region Properties
        public Color StartColor
        {
            get
            {
                return this.startColor;
            }
            set
            {
                this.startColor = value;
            }
        }

        public Color EndColor
        {
            get
            {
                return this.endColor;
            }
            set
            {
                this.endColor = value;
            }
        }
        #endregion

        public ColorSineLerpController(
            string id, 
            ControllerType controllerType, 
            Color startColor, 
            Color endColor, 
            TrigonometricParameters trigonometricParameters
        ) : base(id, controllerType, trigonometricParameters) {
            this.startColor = startColor;
            this.endColor = endColor;
        }

        public override void Update(GameTime gameTime, IActor actor)
        {
            if (actor is DrawnActor3D parentActor)
            {
                //Accumulate elapsed time - note we are not formally resetting this time if the controller becomes inactive - we should mirror the approach used for the UI sine controllers.
                this.totalElapsedTime += gameTime.ElapsedGameTime.Milliseconds;

                //Aine wave in the range 0 -> max amplitude
                float lerpFactor = MathUtility.SineLerpByElapsedTime(this.TrigonometricParameters, this.totalElapsedTime);
                parentActor.EffectParameters.DiffuseColor = MathUtility.Lerp(this.startColor, this.endColor, lerpFactor);
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ColorSineLerpController other))
                return false;
            else if (this == other)
                return true;

            return this.startColor.Equals(other.StartColor)
                    && this.endColor.Equals(other.EndColor)
                        && base.Equals(obj);
        }

        public override int GetHashCode()
        {
            int hash = 1;
            hash = hash * 31 + this.startColor.GetHashCode();
            hash = hash * 17 + this.endColor.GetHashCode();
            hash = hash * 11 + base.GetHashCode();
            return hash;
        }

        public override object Clone()
        {
            return new ColorSineLerpController(
                "Clone - " + this.ID,                                           //Deep
                this.ControllerType,                                            //Deep
                this.startColor,                                                //Deep
                this.endColor,                                                  //Deep
                (TrigonometricParameters)this.TrigonometricParameters.Clone()); //Deep
        }
    }
}