using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using Terraria.UI;
using StarlightRiver.GUI;
using System.IO;
using StarlightRiver.Abilities;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Linq;
using StarlightRiver.Items.CursedAccessories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using On.Terraria.GameContent.UI.Elements;
using System;
using Terraria.GameContent.UI.Elements;
using System.Reflection;
using UICharacter = Terraria.GameContent.UI.Elements.UICharacter;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace StarlightRiver
{
    public partial class StarlightRiver : Mod
    {
        public Stamina stamina;
        public Collection collection;
        public Overlay overlay;
        public Infusion infusion;
        public Cooking cooking;
        public LinkHP linkhp;
        public AbilityText abilitytext;
        public GUI.Codex codex;

        public UserInterface customResources;
        public UserInterface customResources2;
        public UserInterface customResources3;
        public UserInterface customResources4;
        public UserInterface customResources5;
        public UserInterface customResources6;
        public UserInterface customResources7;
        public UserInterface customResources8;

        public static ModHotKey Dash;
        public static ModHotKey Superdash;
        public static ModHotKey Smash;
        public static ModHotKey Wisp;
        public static ModHotKey Purify;

        public enum AbilityEnum : int {dash, wisp, purify, smash, superdash };

        public static StarlightRiver Instance { get; set; }
        public StarlightRiver()
        {
            Instance = this;
        }
        public override void UpdateMusic(ref int music, ref MusicPriority priority)
        {
            if (Main.myPlayer != -1 && !Main.gameMenu && Main.LocalPlayer.active)
            {
                if (Main.LocalPlayer.GetModPlayer<BiomeHandler>().ZoneGlass)
                {
                    music = GetSoundSlot(SoundType.Music, "Sounds/Music/GlassPassive");
                    priority = MusicPriority.BiomeMedium;
                }

                if (Main.LocalPlayer.GetModPlayer<BiomeHandler>().ZoneVoidPre)
                {
                    music = GetSoundSlot(SoundType.Music, "Sounds/Music/VoidPre");
                    priority = MusicPriority.BossLow;
                }

                if (Main.LocalPlayer.GetModPlayer<BiomeHandler>().ZoneJungleCorrupt)
                {
                    music = GetSoundSlot(SoundType.Music, "Sounds/Music/JungleCorrupt");
                    priority = MusicPriority.BiomeHigh;
                }

                if (Main.LocalPlayer.GetModPlayer<BiomeHandler>().ZoneJungleBloody)
                {
                    music = GetSoundSlot(SoundType.Music, "Sounds/Music/WhipAndNaenae");
                    priority = MusicPriority.BiomeHigh;
                }

                if (Main.LocalPlayer.GetModPlayer<BiomeHandler>().ZoneJungleHoly)
                {
                    music = GetSoundSlot(SoundType.Music, "Sounds/Music/WhipAndNaenae");
                    priority = MusicPriority.BiomeHigh;
                }

                if (Main.LocalPlayer.ZoneOverworldHeight && LegendWorld.starfall)
                {
                    music = GetSoundSlot(SoundType.Music, "Sounds/Music/Starlight");
                    priority = MusicPriority.BiomeHigh;
                }
            }
            return;           
        }

        public override void Load()
        {
            //Calls to add achievements.
            Achievements.Achievements.CallAchievements(this);

            Dash = RegisterHotKey("Dash", "LeftShift");
            Wisp = RegisterHotKey("Wisp Form", "F");
            Purify = RegisterHotKey("Purify", "N");
            Smash = RegisterHotKey("Smash", "Z");
            Superdash = RegisterHotKey("Void Dash", "Q");

            if (!Main.dedServ)
            {
                customResources = new UserInterface();
                customResources2 = new UserInterface();
                customResources3 = new UserInterface();
                customResources4 = new UserInterface();
                customResources5 = new UserInterface();
                customResources6 = new UserInterface();
                customResources7 = new UserInterface();
                customResources8 = new UserInterface();

                stamina = new Stamina();
                collection = new Collection();
                overlay = new Overlay();
                infusion = new Infusion();
                cooking = new Cooking();
                linkhp = new LinkHP();
                abilitytext = new AbilityText();
                codex = new GUI.Codex();

                customResources.SetState(stamina);
                customResources2.SetState(collection);
                customResources3.SetState(overlay);
                customResources4.SetState(infusion);
                customResources5.SetState(cooking);
                customResources6.SetState(linkhp);
                customResources7.SetState(abilitytext);
                customResources8.SetState(codex);
            }

            // Cursed Accessory Control Override
            On.Terraria.UI.ItemSlot.LeftClick_ItemArray_int_int += NoClickCurse;
            On.Terraria.UI.ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += DrawSpecial;
            On.Terraria.UI.ItemSlot.RightClick_ItemArray_int_int += NoSwapCurse;
            // Character Slot Addons
            On.Terraria.GameContent.UI.Elements.UICharacterListItem.DrawSelf += DrawSpecialCharacter;
            // Link mode healthbar
            On.Terraria.Main.DrawInterface_Resources_Life += LinkModeHealth;
            // Vitric background
            On.Terraria.Main.DrawBackgroundBlackFill += DrawVitricBackground;
            // Vitric lighting
            IL.Terraria.Lighting.PreRenderPhase += VitricLighting;
        }

        private delegate void ModLightingStateDelegate(float from, ref float to);
        private delegate void ModColorDelegate(int i, int j, ref float r, ref float g, ref float b);

        private void VitricLighting(ILContext il)
        {
            // Create our cursor at the start of the void PreRenderPhase() method.
            ILCursor c = new ILCursor(il);

            // We insert our emissions right before the ModifyLight call (line 1963, CIL 0x3428)
            // Get the TileLoader.ModifyLight method. Then, using it,
            // find where it's called and place the cursor right before that call instruction.

            MethodInfo ModifyLight = typeof(TileLoader).GetMethod("ModifyLight", BindingFlags.Public | BindingFlags.Static);
            c.GotoNext(i => i.MatchCall(ModifyLight));

            // Emit the values of I and J.
            /* To emit local variables, you have to know the indeces of where those variables are stored.
             * These are stated at the very top of the method, in a format like below:
             * .locals init ( 
             *      [0] = float32 FstName, 
             *      [1] = ScdName, 
             *      [2] = ThdName
             * )
            */

            c.Emit(OpCodes.Ldloc, 27); // [27] = n
            c.Emit(OpCodes.Ldloc, 29); // [29] = num17

            /* Emit the addresses of R, G, and B.
             * It's important to emit their *addresses*, because we're passing them—
             *   by reference, not by value. Under the hood, "ref" tokens—
             *   pass a pointer to the object (even for managed types),
             *   and that's what we need to do here.
            */
            c.Emit(OpCodes.Ldloca, 32); // [32] = num18
            c.Emit(OpCodes.Ldloca, 33); // [33] = num19
            c.Emit(OpCodes.Ldloca, 34); // [34] = num20

            // Consume the values of I,J and the addresses of R,G,B by calling EmitVitricDel.
            c.EmitDelegate<ModColorDelegate>(EmitVitricDel);

            #region DEPRECATED
            //// This following code is hacky just because I dislike writing "if"s in IL :)
            //EmitLightingState3("r2", 32); // [32] = num18 (R)
            //EmitLightingState3("g2", 33); // [33] = num19 (G)
            //EmitLightingState3("b2", 34); // [34] = num20 (B)

            //void EmitLightingState3(string fieldname, int colorIndex)
            //{
            //    // Find the field info of Lighting.LightingState's r2/g2/b2 fields.
            //    Type LightingState = typeof(Lighting).GetNestedType("LightingState", BindingFlags.NonPublic);
            //    FieldInfo field = LightingState.GetField(fieldname, BindingFlags.Public | BindingFlags.Instance);

            //    // Emit R, B, and G from before
            //    c.Emit(OpCodes.Ldloc, colorIndex);

            //    // Emit LightingState, then its r2/g2/b2 address.
            //    c.Emit(OpCodes.Ldloc, 30); // [30] = lightingState3
            //    c.Emit(OpCodes.Ldflda, field);
            //    c.EmitDelegate<ModLightingStateDelegate>(EmitLightingStateDel);
            //}
            #endregion

            // Not much more than that.
            // EmitVitricDel has the actual logic inside of it.
        }

        private static void EmitLightingStateDel(float from, ref float to)
        {
            // If the lighting at this position is less than the set R/G/B value, set it.
            if (to < from)
                to = from;
        }

        private static void EmitVitricDel(int i, int j, ref float r, ref float g, ref float b)
        {
            if (Main.tile[i, j] == null)
            {
                return;
            }
            // If the tile is in the vitric biome and doesn't block light, emit light.
            bool tileBlock = Main.tile[i, j].active() && Main.tileBlockLight[Main.tile[i, j].type];
            bool wallBlock = Main.wallLight[Main.tile[i, j].wall];
            if (LegendWorld.VitricBiome.Contains(i, j) && Main.tile[i, j] != null && !tileBlock && wallBlock)
            {
                // Change these as you please!
                r = .1f;
                g = .5f;
                b = .7f;
            }
        }

        internal static readonly List<BootlegDust> VitricBackgroundDust = new List<BootlegDust>();
        internal static readonly List<BootlegDust> VitricForegroundDust = new List<BootlegDust>();

        private void DrawVitricBackground(On.Terraria.Main.orig_DrawBackgroundBlackFill orig, Main self)
        {
            orig(self);
            Player player = null;
            if (Main.playerLoaded) { player = Main.LocalPlayer; }

            VitricBackgroundDust.ForEach(BootlegDust => BootlegDust.Update());
            VitricBackgroundDust.RemoveAll(BootlegDust => BootlegDust.time <= 0);

            VitricForegroundDust.ForEach(BootlegDust => BootlegDust.Update());
            VitricForegroundDust.RemoveAll(BootlegDust => BootlegDust.time <= 0);

            if (player != null && player.Hitbox.Intersects(LegendWorld.VitricBiome))
            {
                Vector2 basepoint = (LegendWorld.vitricTopLeft != null) ? LegendWorld.vitricTopLeft * 16 + new Vector2(-2000, 1000) : Vector2.Zero;
                for (int k = 5; k >= 0; k--)
                {
                    DrawLayer(basepoint, ModContent.GetTexture("StarlightRiver/Backgrounds/Glass" + k), k + 1);
                    if (k == 5)
                    {
                        VitricBackgroundDust.ForEach(BootlegDust => BootlegDust.Draw(Main.spriteBatch));
                    }
                    if (k == 2)
                    {
                        VitricForegroundDust.ForEach(BootlegDust => BootlegDust.Draw(Main.spriteBatch));
                    }
                }

                for (int k = (int)(player.position.X - basepoint.X) - (int)(Main.screenWidth * 1.5f); k <= (int)(player.position.X - basepoint.X) + (int)(Main.screenWidth * 1.5f); k += 30)
                {
                    if (Main.rand.Next(500) == 0)
                    {
                        BootlegDust dus = new VitricDust(ModContent.GetTexture("StarlightRiver/GUI/Light"), basepoint + new Vector2(-2000, 1000), k, 1.5f, 0.3f, 0.1f);
                        VitricBackgroundDust.Add(dus);
                    }

                    if (Main.rand.Next(400) == 0)
                    {
                        BootlegDust dus2 = new VitricDust(ModContent.GetTexture("StarlightRiver/GUI/Light"), basepoint + new Vector2(-2000, 1000), k, 2.25f, 1f, 0.4f);
                        VitricForegroundDust.Add(dus2);
                    }
                }

                for (int i = -2 + (int)(player.position.X - Main.screenWidth / 2) / 16; i <= 2 + (int)(player.position.X + Main.screenWidth / 2) / 16; i++)
                {
                    for (int j = -2 + (int)(player.position.Y - Main.screenHeight) / 16; j <= 2 + (int)(player.position.Y + Main.screenHeight) / 16; j++)
                    {
                        if (Lighting.Brightness(i,j) == 0 || ((Main.tile[i, j].active() && Main.tile[i, j].collisionType == 1) || Main.tile[i, j].wall != 0))
                        {
                            Color color = Color.Black * (1 - Lighting.Brightness(i, j) * 2);
                            Main.spriteBatch.Draw(Main.blackTileTexture, new Vector2(i * 16, j * 16) - Main.screenPosition, color);
                        }
                    }
                }
            }           
        }

        public void DrawLayer(Vector2 basepoint, Texture2D texture, int parallax)
        {
            for (int k = 0; k <= 5; k++)
            {
                Main.spriteBatch.Draw(texture,
                    new Vector2(basepoint.X + (k * 739 * 4) + GetParallaxOffset(basepoint.X, parallax * 0.1f) - (int)Main.screenPosition.X, basepoint.Y - (int)Main.screenPosition.Y),
                    new Rectangle(0, 0, 2956, 1528), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0);
            }
        }

        public int GetParallaxOffset(float startpoint, float factor)
        {
            return (int)((Main.LocalPlayer.position.X - startpoint) * factor);
        }

        private void LinkModeHealth(On.Terraria.Main.orig_DrawInterface_Resources_Life orig)
        {
            if (LinkMode.Enabled)
            {
                return;
            }
            else
            {
                orig();
            }
        }

        private void DrawSpecialCharacter(On.Terraria.GameContent.UI.Elements.UICharacterListItem.orig_DrawSelf orig, Terraria.GameContent.UI.Elements.UICharacterListItem self, SpriteBatch spriteBatch)
        {
            orig(self, spriteBatch);
            Vector2 origin = new Vector2(self.GetDimensions().X, self.GetDimensions().Y);
            Rectangle box = new Rectangle((int)(origin + new Vector2(86, 66)).X, (int)(origin + new Vector2(86, 66)).Y, 80, 25);
            int playerStamina = 0;

            //horray double reflection, fuck you vanilla
            Type typ = self.GetType();
            FieldInfo playerInfo = typ.GetField("_playerPanel", BindingFlags.NonPublic | BindingFlags.Instance);
            UICharacter character = (UICharacter)playerInfo.GetValue(self);

            Type typ2 = character.GetType();
            FieldInfo playerInfo2 = typ2.GetField("_player", BindingFlags.NonPublic | BindingFlags.Instance);
            Player player = (Player)playerInfo2.GetValue(character);
            AbilityHandler mp = player.GetModPlayer<AbilityHandler>();

            if (mp == null) { return; }

            playerStamina = mp.StatStaminaMax;


            Texture2D wind = !mp.dash.Locked ? ModContent.GetTexture("StarlightRiver/NPCs/Pickups/Wind1") : ModContent.GetTexture("StarlightRiver/NPCs/Pickups/Wind0");
            Texture2D wisp = !mp.wisp.Locked ? ModContent.GetTexture("StarlightRiver/NPCs/Pickups/Wisp1") : ModContent.GetTexture("StarlightRiver/NPCs/Pickups/Wisp0");
            Texture2D pure = !mp.pure.Locked ? ModContent.GetTexture("StarlightRiver/NPCs/Pickups/Purity1") : ModContent.GetTexture("StarlightRiver/NPCs/Pickups/Purity0");
            Texture2D smash = !mp.smash.Locked ? ModContent.GetTexture("StarlightRiver/NPCs/Pickups/Smash1") : ModContent.GetTexture("StarlightRiver/NPCs/Pickups/Smash0");
            Texture2D shadow = !mp.sdash.Locked ? ModContent.GetTexture("StarlightRiver/NPCs/Pickups/Cloak1") : ModContent.GetTexture("StarlightRiver/NPCs/Pickups/Cloak0");            

            spriteBatch.Draw(ModContent.GetTexture("StarlightRiver/GUI/box"), box, Color.White); //Stamina box

            mp.SetList();//update ability list
            if (mp.Abilities.Any(a => !a.Locked))//Draw stamina if any unlocked
            {
                spriteBatch.Draw(ModContent.GetTexture("StarlightRiver/GUI/Stamina"), origin + new Vector2(91, 68), Color.White);
                Utils.DrawBorderString(spriteBatch, playerStamina + " SP", origin + new Vector2(118, 68), Color.White);
            }
            else//Myserious if locked
            {
                spriteBatch.Draw(ModContent.GetTexture("StarlightRiver/GUI/Stamina3"), origin + new Vector2(91, 68), Color.White);
                Utils.DrawBorderString(spriteBatch,"???", origin + new Vector2(118, 68), Color.White);
            }

            //Draw ability Icons
            spriteBatch.Draw(wind, origin + new Vector2(390, 62), Color.White);
            spriteBatch.Draw(wisp, origin + new Vector2(426, 62), Color.White);
            spriteBatch.Draw(pure, origin + new Vector2(462, 62), Color.White);
            spriteBatch.Draw(smash, origin + new Vector2(498, 62), Color.White);
            spriteBatch.Draw(shadow, origin + new Vector2(534, 62), Color.White);

            if (player.statLifeMax > 400) //why vanilla dosent do this I dont know
            {
                spriteBatch.Draw(Main.heart2Texture, origin + new Vector2(80, 37), Color.White);
            }
        }

        private void NoClickCurse(On.Terraria.UI.ItemSlot.orig_LeftClick_ItemArray_int_int orig, Terraria.Item[] inv, int context, int slot)
        {
            if(inv[slot].modItem is CursedAccessory && context == 10)
            {
                return;
            }
            orig(inv, context, slot);
        }

        private void NoSwapCurse(On.Terraria.UI.ItemSlot.orig_RightClick_ItemArray_int_int orig, Terraria.Item[] inv, int context, int slot)
        {
            Player player = Main.player[Main.myPlayer];
            for (int i = 0; i < player.armor.Length; i++)
            {
                if (player.armor[i].modItem is CursedAccessory && ItemSlot.ShiftInUse && inv[slot].accessory)
                {
                    return;
                }              
            }
            orig(inv, context, slot);
        }

        private void DrawSpecial(On.Terraria.UI.ItemSlot.orig_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color orig, SpriteBatch sb, Terraria.Item[] inv, int context, int slot, Vector2 position, Color color)
        {
            if ((inv[slot].modItem is CursedAccessory || inv[slot].modItem is BlessedAccessory) && context == 10)
            {
                Texture2D back = inv[slot].modItem is CursedAccessory ? ModContent.GetTexture("StarlightRiver/GUI/CursedBack") : ModContent.GetTexture("StarlightRiver/GUI/BlessedBack");
                Color backcolor = (!Main.expertMode && slot == 8) ? Color.White * 0.25f : Color.White * 0.75f;
                sb.Draw(back, position, null, backcolor, 0f, default, Main.inventoryScale, SpriteEffects.None, 0f);

                //Zoinked from vanilla code
                Item item = inv[slot];
                Vector2 vector = back.Size() * Main.inventoryScale;
                Texture2D texture2D3 = ModContent.GetTexture("StarlightRiver/Items/CursedAccessories/"+inv[slot].modItem.Name);
                Rectangle rectangle2 = (texture2D3.Frame(1, 1, 0, 0));
                Color currentColor = color;
                float scale3 = 1f;
                ItemSlot.GetItemLight(ref currentColor, ref scale3, item, false);
                float num8 = 1f;
                if (rectangle2.Width > 32 || rectangle2.Height > 32)
                {
                    num8 = ((rectangle2.Width <= rectangle2.Height) ? (32f / (float)rectangle2.Height) : (32f / (float)rectangle2.Width));
                }
                num8 *= Main.inventoryScale;
                Vector2 position2 = position + vector / 2f - rectangle2.Size() * num8 / 2f;
                Vector2 origin = rectangle2.Size() * (scale3 / 2f - 0.5f);
                if (ItemLoader.PreDrawInInventory(item, sb, position2, rectangle2, item.GetAlpha(currentColor), item.GetColor(color), origin, num8 * scale3))
                {
                    sb.Draw(texture2D3, position2, rectangle2, Color.White, 0f, origin, num8 * scale3, SpriteEffects.None, 0f);
                }
                ItemLoader.PostDrawInInventory(item, sb, position2, rectangle2, item.GetAlpha(currentColor), item.GetColor(color), origin, num8 * scale3);
                //End zoink
            }
            else
            {
                orig(sb, inv, context, slot, position, color);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {

            int MouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (MouseTextIndex != -1)
            {
                layers.Insert(MouseTextIndex, new LegacyGameInterfaceLayer("[PH]MODNAME: Cooldown",
                delegate
                {
                    if (Stamina.visible)
                    {
                        customResources.Update(Main._drawInterfaceGameTime);
                        stamina.Draw(Main.spriteBatch);
                    }

                    return true;
                }, InterfaceScaleType.UI));

                layers.Insert(MouseTextIndex + 1, new LegacyGameInterfaceLayer("[PH]MODNAME: Collection",
                delegate
                {
                    if (Collection.visible)
                    {
                        customResources2.Update(Main._drawInterfaceGameTime);
                        collection.Draw(Main.spriteBatch);
                    }

                    return true;
                }, InterfaceScaleType.UI));

                layers.Insert(0, new LegacyGameInterfaceLayer("[PH]MODNAME: Overlay",
                delegate
                {
                    if (Overlay.visible)
                    {
                        customResources3.Update(Main._drawInterfaceGameTime);
                        overlay.Draw(Main.spriteBatch);
                    }

                    return true;
                }, InterfaceScaleType.UI));

                layers.Insert(MouseTextIndex + 2, new LegacyGameInterfaceLayer("[PH]MODNAME: Infusions",
                delegate
                {
                    if (Infusion.visible)
                    {
                        customResources4.Update(Main._drawInterfaceGameTime);
                        infusion.Draw(Main.spriteBatch);
                    }

                    return true;
                }, InterfaceScaleType.UI));

                layers.Insert(MouseTextIndex + 3, new LegacyGameInterfaceLayer("[PH]MODNAME: Cooking",
                delegate
                {
                    if (Cooking.visible)
                    {
                        customResources5.Update(Main._drawInterfaceGameTime);
                        cooking.Draw(Main.spriteBatch);
                    }

                    return true;
                }, InterfaceScaleType.UI));

                layers.Insert(MouseTextIndex + 4, new LegacyGameInterfaceLayer("[PH]MODNAME: LinkHP",
                delegate
                {
                    if (LinkHP.visible)
                    {
                        customResources6.Update(Main._drawInterfaceGameTime);
                        linkhp.Draw(Main.spriteBatch);
                    }

                    return true;
                }, InterfaceScaleType.UI));

                layers.Insert(MouseTextIndex + 5, new LegacyGameInterfaceLayer("[PH]MODNAME: Ability Text",
                delegate
                {
                    if (AbilityText.Visible)
                    {
                        customResources7.Update(Main._drawInterfaceGameTime);
                        abilitytext.Draw(Main.spriteBatch);
                    }

                    return true;
                }, InterfaceScaleType.UI));

                layers.Insert(MouseTextIndex + 6, new LegacyGameInterfaceLayer("[PH]MODNAME: Codex",
                delegate
                {
                    if (GUI.Codex.Visible)
                    {
                        customResources8.Update(Main._drawInterfaceGameTime);
                        codex.Draw(Main.spriteBatch);
                    }

                    return true;
                }, InterfaceScaleType.UI));
            }
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
           // var target = new Abilities.Ability(0);
            //var ms = new MemoryStream(Encoding.UTF8.GetBytes(reader.ReadString()));
           // var ser = new DataContractJsonSerializer(target.GetType());
            //target = ser.ReadObject(ms) as Abilities.Ability;

            //Main.player[whoAmI].GetModPlayer<AbilityHandler>().ability = target;

            LinkMode.Enabled = reader.ReadBoolean();
            LinkMode.MaxWorldHP = reader.ReadInt32();
            LinkMode.WorldHP = reader.ReadInt32();
            //Main.NewText("Packet Recieved!", 100, 100, 255);
            Console.WriteLine("Server Packet Recieved!");
        }

        public override void Unload()
        {
            if (!Main.dedServ)
            {
                customResources = null;
                customResources2 = null;
                customResources3 = null;
                customResources4 = null;
                customResources5 = null;
                customResources6 = null;
                customResources7 = null;
                customResources8 = null;

                stamina = null;
                collection = null;
                overlay = null;
                infusion = null;
                cooking = null;
                linkhp = null;
                abilitytext = null;
                codex = null;

                VitricBackgroundDust.Clear();
                VitricForegroundDust.Clear();
                CursedAccessory.Bootlegdust.Clear();
                BlessedAccessory.Bootlegdust.Clear();
                BlessedAccessory.Bootlegdust2.Clear();
                Collection.Bootlegdust.Clear();
                Overlay.Bootlegdust.Clear();

                Instance = null;
                Dash = null;
                Superdash = null;
                Wisp = null;
                Smash = null;
                Purify = null;
            }

            // Cursed Accessory Control Override
            On.Terraria.UI.ItemSlot.LeftClick_ItemArray_int_int -= NoClickCurse;
            On.Terraria.UI.ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color -= DrawSpecial;
            On.Terraria.UI.ItemSlot.RightClick_ItemArray_int_int -= NoSwapCurse;
            // Character Slot Addons
            On.Terraria.GameContent.UI.Elements.UICharacterListItem.DrawSelf -= DrawSpecialCharacter;
            // Link mode healthbar
            On.Terraria.Main.DrawInterface_Resources_Life -= LinkModeHealth;
            // Vitric background
            On.Terraria.Main.DrawBackgroundBlackFill -= DrawVitricBackground;
            // Vitric lighting
            // IL.Terraria.Lighting.PreRenderPhase -= VitricLighting;
        }
    }
}	
