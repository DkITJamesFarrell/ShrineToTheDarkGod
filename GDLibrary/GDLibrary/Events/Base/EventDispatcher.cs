﻿/*
Function: 		Represent a message broker for events received and routed through the game engine. 
                Allows the receiver to receive event messages with no reference to the publisher - decouples the sender and receiver.
Author: 		NMCG
Version:		1.0
Bugs:			None
Fixes:			None
Comments:       Should consider making this class a Singleton because of the static message Stack - See https://msdn.microsoft.com/en-us/library/ff650316.aspx
*/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GDLibrary
{
    public class EventDispatcher : GameComponent
    {
        #region Static
        private static Queue<EventData> queue; //Stores events in arrival sequence
        private static HashSet<EventData> uniqueSet; //Prevents the same event from existing in the stack for a single update cycle (e.g. when playing a sound based on keyboard press)
        #endregion

        #region Delegates
        public delegate void GameEventHandler(EventData eventData);
        public delegate void CameraEventHandler(EventData eventData);
        public delegate void ScreenEventHandler(EventData eventData);
        public delegate void MenuChangedEventHandler(EventData eventData);
        public delegate void Sound2DEventHandler(EventData eventData);
        public delegate void Sound3DEventHandler(EventData eventData);
        public delegate void GlobalSoundEventHandler(EventData eventData);
        public delegate void OpacityEventHandler(EventData eventData);
        public delegate void AddActorEventHandler(EventData eventData);
        public delegate void RemoveActorEventHandler(EventData eventData);
        public delegate void DebugEventHandler(EventData eventData);
        public delegate void CombatEventHandler(EventData eventData);
        public delegate void DoorEventHandler(EventData eventData);
        public delegate void InventoryEventHandler(EventData eventData);
        public delegate void PlayerEventHandler(EventData eventData);
        public delegate void EnemyEventHandler(EventData eventData);
        public delegate void UIChangedEventHandler(EventData eventData);
        public delegate void ObjectPickingEventHandler(EventData eventData);
        public delegate void TextboxChangedEventHandler(EventData eventData);
        public delegate void EnemyDeathEventHandler(EventData eventData);
        public delegate void KeybindEventHandler(EventData eventData);
        #endregion

        #region Events
        public event GameEventHandler GameChanged;
        public event CameraEventHandler CameraChanged;
        public event ScreenEventHandler ScreenChanged;
        public event MenuChangedEventHandler MenuChanged;
        public event Sound2DEventHandler Sound2DChanged;
        public event Sound3DEventHandler Sound3DChanged;
        public event GlobalSoundEventHandler GlobalSoundChanged;
        public event OpacityEventHandler OpacityChanged;
        public event AddActorEventHandler AddActorChanged;
        public event RemoveActorEventHandler RemoveActorChanged;
        public event DebugEventHandler DebugChanged;
        public event CombatEventHandler CombatEvent;
        public event DoorEventHandler DoorEvent;
        public event InventoryEventHandler InventoryEvent;
        public event PlayerEventHandler PlayerChanged;
        public event EnemyEventHandler EnemyChanged;
        public event UIChangedEventHandler UIChanged;
        public event ObjectPickingEventHandler ObjectPickChanged;
        public event TextboxChangedEventHandler TextboxChanged;
        public event EnemyDeathEventHandler EnemyDeathEvent;
        public event KeybindEventHandler KeybindEvent;
        #endregion

        #region Constuctors
        public EventDispatcher(
            Game game, 
            int initialSize
        ) : base(game) {
            queue = new Queue<EventData>(initialSize);
            uniqueSet = new HashSet<EventData>(new EventDataEqualityComparer());
        }
        #endregion

        #region Class-Specific Methods
        EventData eventData;

        public static void Publish(EventData eventData)
        {
            //This prevents the same event being added multiple times within a single update e.g. 10x bell ring sounds
            if (!uniqueSet.Contains(eventData))
            {
                queue.Enqueue(eventData);
                uniqueSet.Add(eventData);
            }
        }

        public override void Update(GameTime gameTime)
        { 
            while(queue.Count > 0)
            {
                eventData = queue.Dequeue();
                Process(eventData);
                uniqueSet.Remove(eventData);
            }

            base.Update(gameTime);
        }

        private void Process(EventData eventData)
        {
            //Switch - See https://msdn.microsoft.com/en-us/library/06tc147t.aspx
            //One case for each category type
            switch (eventData.EventCategoryType)
            {
                case EventCategoryType.Game:
                    OnGameChanged(eventData);
                    break;

                case EventCategoryType.Camera:
                    OnCameraChanged(eventData);
                    break;
                    
                case EventCategoryType.Screen:
                    OnScreen(eventData);
                    break;

                case EventCategoryType.Menu:
                    OnMenuChanged(eventData);
                    break;

                case EventCategoryType.Sound2D:
                    OnSound2D(eventData);
                    break;

                case EventCategoryType.Sound3D:
                    OnSound3D(eventData);
                    break;

                case EventCategoryType.GlobalSound:
                    OnGlobalSound(eventData);
                    break;

                case EventCategoryType.Opacity:
                    OnOpacity(eventData);
                    break;

                case EventCategoryType.SystemAdd:
                    OnAddActor(eventData);
                    break;

                case EventCategoryType.SystemRemove:
                    OnRemoveActor(eventData);
                    break;

                case EventCategoryType.Debug:
                    OnDebug(eventData);
                    break;
                    
                case EventCategoryType.Combat:
                    OnCombat(eventData);
                    break;

                case EventCategoryType.Door:
                    OnDoorOpen(eventData);
                    break;

                case EventCategoryType.Inventory:
                    OnInventoryChanged(eventData);
                    break;

                case EventCategoryType.Player:
                    OnPlayer(eventData);
                    break;

                case EventCategoryType.Enemy:
                    OnEnemy(eventData);
                    break;

                case EventCategoryType.ObjectPicking:
                    OnObjectPicking(eventData);
                    break;

                case EventCategoryType.Textbox:
                    OnTextboxChanged(eventData);
                    break;

                case EventCategoryType.UIMenu:
                    OnUIChanged(eventData);
                    break;

                case EventCategoryType.EnemyDeath:
                    OnEnemyDeath(eventData);
                    break;

                default:
                    break;
            }
        }

        #endregion

        #region Event Methods
        private void OnPlayer(EventData eventData)
        {
            PlayerChanged?.Invoke(eventData);
        }

        private void OnEnemy(EventData eventData)
        {
            EnemyChanged?.Invoke(eventData);
        }

        //Called when the game state has changed (next level, reset level etc.)
        protected virtual void OnGameChanged(EventData eventData)
        {
            GameChanged?.Invoke(eventData);
        }

        //Called when a screen event needs to be generated (e.g. change screen layout)
        protected virtual void OnScreen(EventData eventData)
        {
            ScreenChanged?.Invoke(eventData);
        }

        //Called when a camera event needs to be generated
        protected virtual void OnCameraChanged(EventData eventData)
        {
            CameraChanged?.Invoke(eventData);
        }

        protected virtual void OnTextboxChanged(EventData eventData)
        {
            TextboxChanged?.Invoke(eventData);
        }

        //Called when a menu change is requested
        protected virtual void OnMenuChanged(EventData eventData)
        {
            MenuChanged?.Invoke(eventData);
        }

        protected virtual void OnUIChanged(EventData eventData)
        {
            UIChanged?.Invoke(eventData);
        }

        //Called when a 2D sound event is sent e.g. play "menu music"
        protected virtual void OnSound2D(EventData eventData)
        {
            Sound2DChanged?.Invoke(eventData);
        }

        //Called when a 3D sound event is sent e.g. play "boom"
        protected virtual void OnSound3D(EventData eventData)
        {
            Sound3DChanged?.Invoke(eventData);
        }

        //Called when a global sound event is sent to set volume by category or mute all sounds
        protected virtual void OnGlobalSound(EventData eventData)
        {
            GlobalSoundChanged?.Invoke(eventData);
        }

        //Called when a drawn objects opacity changes
        protected virtual void OnOpacity(EventData eventData)
        {
            OpacityChanged?.Invoke(eventData);
        }

        //Called when a drawn objects needs to be added - see PickingManager::DoFireNewObject()
        protected virtual void OnAddActor(EventData eventData)
        {
            AddActorChanged?.Invoke(eventData);
        }

        //Called when a drawn objects needs to be removed - see UIMouseObject::HandlePickedObject()
        protected virtual void OnRemoveActor(EventData eventData)
        {
            RemoveActorChanged?.Invoke(eventData);
        }

        protected virtual void OnEnemyDeath(EventData eventData)
        {
            EnemyDeathEvent?.Invoke(eventData);
        }

        //Called when a debug related event occurs (e.g. show/hide debug info)
        protected virtual void OnDebug(EventData eventData)
        {
            DebugChanged?.Invoke(eventData);
        }

        protected virtual void OnCombat(EventData eventData)
        {
            CombatEvent?.Invoke(eventData);
        }

        //Called when the PickingManager picks an object
        protected virtual void OnObjectPicking(EventData eventData)
        {
            ObjectPickChanged?.Invoke(eventData);
        }

        protected virtual void OnDoorOpen(EventData eventData)
        {
            DoorEvent?.Invoke(eventData);
        }
        
        protected virtual void OnInventoryChanged(EventData eventData)
        {
            InventoryEvent?.Invoke(eventData);
        }

        protected virtual void OnKeybind(EventData eventData)
        {
            KeybindEvent?.Invoke(eventData);
        }
        #endregion
    }
}