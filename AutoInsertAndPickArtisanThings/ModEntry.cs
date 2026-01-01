using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace AutoInsertAndPickArtisanThings
{
    public sealed class ModConfig
    {
        public bool autoInsert { get; set; } = true;
        public bool autoPick { get; set; } = true;
        public int radius { get; set; } = 2;
        public SButton insertKeybind { get; set; } = SButton.NumPad9;
        public SButton pickupKeybind { get; set; } = SButton.NumPad9;
    }

    internal sealed class ModEntry : Mod
    {
        private ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            I18n.Init(helper.Translation);
            Config = Helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: I18n.Config_AutoInsert,
                tooltip: I18n.Config_AutoInsert_Desc,
                getValue: () => Config.autoInsert,
                setValue: value => Config.autoInsert = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: I18n.Config_AutoPick,
                tooltip: I18n.Config_AutoPick_Desc,
                getValue: () => Config.autoPick,
                setValue: value => Config.autoPick = value
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: I18n.Config_DetectionRange,
                tooltip: I18n.Config_DetectionRange_Desc,
                getValue: () => Config.radius,
                setValue: value => Config.radius = value,
                min: 1,
                max: 20,
                interval: 1
            );

            configMenu.AddKeybind(
                mod: ModManifest,
                name: I18n.Config_KeybindInsert,
                tooltip: I18n.Config_KeybindInsert_Desc,
                getValue: () => Config.insertKeybind,
                setValue: value => Config.insertKeybind = value
            );

            configMenu.AddKeybind(
                mod: ModManifest,
                name: I18n.Config_KeybindPick,
                tooltip: I18n.Config_KeybindPick,
                getValue: () => Config.pickupKeybind,
                setValue: value => Config.pickupKeybind = value
            );
        }

        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if (!Context.IsWorldReady) return;
            //this.Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
            if (e.Button == Config.pickupKeybind) DoPickup();
            if (e.Button == Config.insertKeybind) DoInsertions();
        }

        private static bool IsValidArtisanInput(
            int inputParentSheetIndex,
            int? inputCategory = null
        )
        {
            if (inputCategory.HasValue && ModContants.AllValidArtisanInputs.Contains(inputCategory.Value))
            {
                return true;
            }

            return ModContants.AllValidArtisanInputs.Contains(inputParentSheetIndex);
        }

        private bool IsHoldingIngredient()
        {
            Item selectedItem = Game1.player.CurrentItem;

            if (selectedItem == null || !(Game1.player.CurrentItem is StardewValley.Object obj)) return false;

            // cask wines
            if (
                selectedItem.HasContextTag("wine_item") || // variants
                selectedItem.HasContextTag("item_wine") // default wine
            )
            {
                return true;
            }

            bool valid = IsValidArtisanInput(
                selectedItem.ParentSheetIndex,
                selectedItem.Category
            );

            return valid;
        }

        private void DoInsertions()
        {
            if (!IsHoldingIngredient()) return;

            var artisanMachines = GetNearbyArtisanEquipment();
            //Monitor.Log($"Total machines: {artisanMachines.Count}", LogLevel.Debug);

            if (artisanMachines.Count == 0) return;

            foreach (var machine in artisanMachines) {
                if (machine.MinutesUntilReady > 0) continue;

                var item = Game1.player.CurrentItem;
                var success = machine.performObjectDropInAction(item, false, Game1.player);
                //Monitor.Log($"{(success==true ? "✅" : '❌')} {item.BaseName} -> {machine.BaseName}", LogLevel.Debug);
                //machine.PlaceInMachine(machine.GetMachineData(), item, false, Game1.player);
            }
        }

        private void DoPickup()
        {
            var artisanMachines = GetNearbyArtisanEquipment();
            if (artisanMachines.Count == 0) return;

            foreach (var machine in artisanMachines) {
                if (machine.MinutesUntilReady > 0 || machine.heldObject.Value == null) continue;

                var item = machine.heldObject.Value;
                var success = machine.checkForAction(Game1.player);
                //Monitor.Log($"{(success == true ? "✅" : '❌')} {machine.BaseName} -> [⭐{item.Quality}]{item.BaseName}", LogLevel.Debug);
            }
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            if (Config.autoPick) DoPickup();

            if (Config.autoInsert) DoInsertions();
        }

        private static bool IsAllowedArtisanEquipment(StardewValley.Object obj)
        {
            if (obj == null) return false;

            if (!obj.bigCraftable.Value) return false;

            return ModContants.AllowedArtisanIds.Contains(obj.ParentSheetIndex);
        }

        private List<StardewValley.Object> GetNearbyArtisanEquipment()
        {
            return ScanObjectsInRadius().ToList()
                .Where(IsAllowedArtisanEquipment)
                .ToList();
        }

        private bool IsWithinRadius(Vector2 center, Vector2 target)
        {
            float radiusSq = Config.radius * Config.radius;
            float distSq = Vector2.DistanceSquared(center, target);
            return distSq <= radiusSq;
        }

        private IEnumerable<StardewValley.Object> ScanObjectsInRadius()
        {
            GameLocation location = Game1.player.currentLocation;
            Vector2 playerTile = Game1.player.Tile;

            foreach (var pair in location.Objects.Pairs)
            {
                if (IsWithinRadius(playerTile, pair.Key))
                    yield return pair.Value;
            }
        }

        private Vector2 GetPlayerLocation()
        {
            Vector2 playerLocation = Game1.player.Tile;
            return playerLocation;
        }
    }
}
