using System;
using System.Collections.Generic;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Linq;
using Microsoft.Xna.Framework.Input;

namespace StashItemsToChest
{
    public class StashItemsToChest : Mod
    {
		//private KeyboardState lastKeyboardState;

		public static string keystr(Keys[] keys)
		{
			return String.Join(",", keys.Select(key => key.ToString()));
		}

        public static StashItemsToChestConfig ModConfig { get; protected set; }

        public override void Entry(IModHelper helper)
        {
            ModConfig = helper.ReadConfig<StashItemsToChestConfig>();

			//ControlEvents.KeyReleased += ControlEvents_KeyReleased;
			ControlEvents.KeyPressed += ControlEvents_KeyReleased;

			ControlEvents.KeyboardChanged += (sender, e) =>
			{
				Monitor.Log($"KeyboardChanged. Before: '{keystr(e.PriorState.GetPressedKeys())}', After: '{keystr(e.NewState.GetPressedKeys())}'");
			};
            //StardewModdingAPI.Events.GameEvents.UpdateTick += UpdateTickEvent;

            //lastKeyboardState = Keyboard.GetState();
        }

		private void ControlEvents_KeyReleased(object sender, EventArgsKeyPressed e)
		{
			if (!Context.IsWorldReady) return;

			if (e.KeyPressed == ModConfig.stashKey)
			{
				StashUp();
			}

		}

        //PhthaloBlue: these blocks of codes below are from Chest Pooling mod by mralbobo
        //repo link here: https://github.com/mralbobo/stardew-chest-pooling, they are useful so I use them
        static Chest getOpenChest()
        {
            if (Game1.activeClickableMenu == null) return null;

            ItemGrabMenu itemGrabMenu = Game1.activeClickableMenu as ItemGrabMenu;
            if (itemGrabMenu != null)
            {
                if (itemGrabMenu.behaviorOnItemGrab != null && itemGrabMenu.behaviorOnItemGrab.Target is Chest)
                {
                    return itemGrabMenu.behaviorOnItemGrab.Target as Chest;
                }
            }
            //else
            //{
            //    if (Game1.activeClickableMenu.GetType().Name == "ACAMenu")
            //    {
            //        dynamic thing = (dynamic)Game1.activeClickableMenu;
            //        if (thing != null && thing.chestItems != null)
            //        {
            //            Chest aChest = new Chest(true);
            //            aChest.items = thing.chestItems;
            //            return aChest;
            //        }
            //    }
            //}
            return null;
        }

        static void StashUp()
        {
			if (!Game1.game1.IsActive || Game1.currentLocation == null) return;
			
            Chest OpenChest = getOpenChest();
            
			if (OpenChest == null || OpenChest.isEmpty()) return;

            foreach (Item chestItem in OpenChest.items)
            {
                foreach (Item playerItem in Game1.player.items)
                {
                    if (playerItem == null)
                        continue;

                    if (playerItem.canStackWith(chestItem))
                    {
                        OpenChest.grabItemFromInventory(playerItem, Game1.player);
                        break;
                    }
                }
            }
        }
    }

    public class StashItemsToChestConfig
    {
        public Keys stashKey = Keys.Tab;
    }
}
