﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SkinnedModel;
using System;
using System.Collections.Generic;

namespace GDLibrary
{
    //used internally as a unique key (take + model name) to access a specific animation (useful when lots of FBX files have same default take name i.e. Take001)
    class AnimationDictionaryKey
    {
        public string takeName;
        public string fileNameNoSuffix;

        public AnimationDictionaryKey(string takeName, string fileNameNoSuffix)
        {
            this.takeName = takeName;
            this.fileNameNoSuffix = fileNameNoSuffix;
        }

        //Why do we override equals and gethashcode? Clue: this.modelDictionary.ContainsKey()
        public override bool Equals(object obj)
        {
            AnimationDictionaryKey other = obj as AnimationDictionaryKey;
            return this.takeName.Equals(other.takeName) && this.fileNameNoSuffix.Equals(other.fileNameNoSuffix);
        }

        public override int GetHashCode()
        {
            int hash = 1;
            hash = hash * 31 + this.takeName.GetHashCode();
            hash = hash * 17 + this.fileNameNoSuffix.GetHashCode();
            return hash;
        }

    }

    public class AnimatedCharacterObject : CharacterObject
    {
        #region Variables
        private AnimationStateType animationState;
        private AnimationPlayer animationPlayer;
        private SkinningData skinningData;

        //stores all the data related to a character with multiple individual FBX animation files (e.g. walk.fbx, idle,fbx, run.fbx)
        private Dictionary<AnimationDictionaryKey, Model> modelDictionary;
        private Dictionary<AnimationDictionaryKey, AnimationPlayer> animationPlayerDictionary;
        private Dictionary<AnimationDictionaryKey, SkinningData> skinningDataDictionary;
        private AnimationDictionaryKey oldKey;
        #endregion

        #region Properties
        public AnimationStateType AnimationState
        {
            get
            {
                return this.animationState;
            }
            set
            {
                this.animationState = value;
            }
        }

        public AnimationPlayer AnimationPlayer
        {
            get
            {
                return this.animationPlayer;
            }
        }
        #endregion

        public AnimatedCharacterObject(
            string id,
            ActorType actorType,
            Transform3D transform,
            EffectParameters effectParameters,
            Model model,
            float accelerationRate,
            float decelerationRate,
            Vector3 movementVector,
            Vector3 rotationVector,
            float moveSpeed,
            float rotateSpeed,
            float health,
            float attack,
            float defence,
            ManagerParameters managerParameters
        ) : base(
            id,
            actorType,
            transform,
            effectParameters,
            model,
            movementVector,
            rotationVector,
            moveSpeed,
            rotateSpeed,
            health,
            attack,
            defence,
            managerParameters
        ) {
            //initialize dictionaries
            this.modelDictionary = new Dictionary<AnimationDictionaryKey, Model>();
            this.animationPlayerDictionary = new Dictionary<AnimationDictionaryKey, AnimationPlayer>();
            this.skinningDataDictionary = new Dictionary<AnimationDictionaryKey, SkinningData>();
        }

        public void AddAnimation(string takeName, string fileNameNoSuffix, Model model)
        {
            AnimationDictionaryKey key = new AnimationDictionaryKey(takeName, fileNameNoSuffix);

            //If not already added
            if (!this.modelDictionary.ContainsKey(key))
            {
                this.modelDictionary.Add(key, model);
                
                //Read the skinning data (i.e. the set of transforms applied to each model bone for each frame of the animation)
                skinningData = model.Tag as SkinningData;

                if (skinningData == null)
                    throw new InvalidOperationException("The model [" + fileNameNoSuffix + "] does not contain a SkinningData tag.");

                //Make an animation player for the model
                this.animationPlayerDictionary.Add(key, new AnimationPlayer(skinningData));

                //Store the skinning data for the model 
                this.skinningDataDictionary.Add(key, skinningData);
            }
        }

        public override void Update(GameTime gameTime)
        {
            //Update character to return bone transforms for the appropriate frame in the animation
            animationPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
            base.Update(gameTime);
        }

        //Sets the first frame for the take and file (e.g. "Take 001", "dude")
        public void SetAnimation(string takeName, string fileNameNoSuffix)
        {
            AnimationDictionaryKey key = new AnimationDictionaryKey(takeName, fileNameNoSuffix);

            //Have we requested a different animation and is it in the dictionary?
            //First time or different animation request
            if (this.oldKey == null || (!this.oldKey.Equals(key) && this.modelDictionary.ContainsKey(key)))
            {
                //Set the model based on the animation being played
                this.Model = modelDictionary[key];

                //Retrieve the animation player
                animationPlayer = animationPlayerDictionary[key];

                //Retrieve the skinning data
                skinningData = skinningDataDictionary[key];

                //Set the skinning data in the animation player and set the player to start at the first frame for the take
                animationPlayer.StartClip(skinningData.AnimationClips[key.takeName]);
            }

            //Store current key for comparison in next update to prevent re-setting the same animation in successive calls to SetAnimation()
            this.oldKey = key;
        }

        //sets the take based on what the user presses/clicks
        protected virtual void SetAnimationByInput()
        {
        }
    }
}