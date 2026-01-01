public static class ModContants
{
    public enum ArtisanMachineType
    {
        Cask,
        CheesePress,
        Keg,
        Loom,
        MayoMachine,
        OilMaker,
        PreservesJar,
        Dehydrator,
    }

    public static readonly HashSet<int> AllowedArtisanIds = new()
    {
        163, // Cask
        16,  // Cheese Press
        12,  // Keg
        17,  // Loom
        24,  // Mayo Machine
        19,  // Oil Maker
        15,  // Preserves Jar
    };

    public static readonly Dictionary<ArtisanMachineType, HashSet<int>> MachineValidInputs = new()
    {
        {
            ArtisanMachineType.Cask,
            new HashSet<int>
            {
                348, // Wine
                303, // Pale Ale
                346, // Beer
                459, // Mead
                424, // Cheese
                426  // Goat Cheese
            }
        },
        {
            ArtisanMachineType.CheesePress,
            new HashSet<int>
            {
                184, // Milk
                186, // Large Milk
                436, // Goat Milk
                438, // Large Goat Milk
            }
        },
        {
            ArtisanMachineType.Keg,
            new HashSet<int>
            {
                StardewValley.Object.FruitsCategory,
                StardewValley.Object.VegetableCategory,
                262, // Wheat
                423, // Rice
                433, // Coffee Bean
                614, // Tea Leaves
                340, // Honey
                304, // Hops
            }
        },
        {
            ArtisanMachineType.Loom,
            new HashSet<int>
            {
                440 // Wool
            }
        },
        {
            ArtisanMachineType.MayoMachine,
            new HashSet<int>
            {
                176, 180, // Egg
                174, 182, // Large Egg
                289,      // Ostrich Egg
                928,      // Golden Egg
                442,      // Duck Egg
                305,      // Void Egg
                107,      // Dinosaur Egg
            }
        },
        {
            ArtisanMachineType.OilMaker,
            new HashSet<int>
            {
                430, // Truffle
                270, // Corn
                431, // Sunflower Seeds
                421  // Sunflower
            }
        },
        {
            ArtisanMachineType.PreservesJar,
            new HashSet<int>
            {
                StardewValley.Object.FruitsCategory,
                StardewValley.Object.VegetableCategory,
                281, // Chanterelle
                404, // Common Mushroom
                851, // Magma Cap
                257, // Morel
                422, // Purple Mushroom
                812, // Roe +(Sturgeon Roe?)
                // --- positive energy forage?
            }
        },
        {
            ArtisanMachineType.Dehydrator,
            new HashSet<int>
            {
                StardewValley.Object.FruitsCategory,
                StardewValley.Object.GreensCategory
            }
        }
    };

    public static readonly HashSet<int> AllValidArtisanInputs =
        MachineValidInputs.Values
            .SelectMany(v => v)
            .ToHashSet();
}
