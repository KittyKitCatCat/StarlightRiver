﻿using Terraria.ModLoader;

namespace StarlightRiver.Codex.Entries
{
    class LoreEntry : CodexEntry
    {
        public LoreEntry()
        {
            Category = (int)Categories.Abilities;
            Title = "Starlight Codex";
            Body = "A mysterious compendium containing lost knowledge, it seems to write itself as you travel. Click the codex icon in your inventory to view the codex.";
            Image = ModContent.GetTexture("StarlightRiver/GUI/Book1Closed");
            Icon = ModContent.GetTexture("StarlightRiver/GUI/Book1Closed");
        }
    }
    class WindsEntry : CodexEntry
    {
        public WindsEntry()
        {
            Category = (int)Categories.Abilities;
            Title = "Forbidden Winds";
            Body = "Wow this is such an amzing test entry\nYes dont you thing wow wow\nhahaha\nDont you just love multiline strings?";
            Image = ModContent.GetTexture("StarlightRiver/Achievements/WindsAchievementOn");
            Icon = ModContent.GetTexture("StarlightRiver/NPCs/Pickups/Wind1");
        }
    }
    class FaeEntry : CodexEntry
    {
        public FaeEntry()
        {
            Category = (int)Categories.Abilities;
            Title = "Faeflame";
            Body = "Shoop de poop loob";
            Image = ModContent.GetTexture("StarlightRiver/Achievements/WispAchievementOn");
            Icon = ModContent.GetTexture("StarlightRiver/NPCs/Pickups/Wisp1");
        }
    }
    class PureEntry : CodexEntry
    {
        public PureEntry()
        {
            Category = (int)Categories.Abilities;
            Title = "Corona of Purity";
            Body = "T\nT\nT\nT\nT\nT";
            Image = ModContent.GetTexture("StarlightRiver/Achievements/WindsAchievementOff");
            Icon = ModContent.GetTexture("StarlightRiver/NPCs/Pickups/Purity1");
        }
    }
    class SmashEntry : CodexEntry
    {
        public SmashEntry()
        {
            Category = (int)Categories.Abilities;
            Title = "Gaia's Fist";
            Body = "T\nT\nT\nT\nT\nT";
            Image = ModContent.GetTexture("StarlightRiver/Achievements/WindsAchievementOff");
            Icon = ModContent.GetTexture("StarlightRiver/NPCs/Pickups/Smash1");
        }
    }
    class CloakEntry : CodexEntry
    {
        public CloakEntry()
        {
            Category = (int)Categories.Abilities;
            Title = "Zzelera's Cloak";
            Body = "T\nT\nT\nT\nT\nT";
            Image = ModContent.GetTexture("StarlightRiver/Achievements/WindsAchievementOff");
            Icon = ModContent.GetTexture("StarlightRiver/NPCs/Pickups/Cloak1");
        }
    }
}
