﻿using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace StarlightRiver.NPCs.Hostile
{
    class OvergrowSkeletonLarge : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Swordsman Skeleton");
            Main.npcFrameCount[npc.type] = 6;
        }
        public override void SetDefaults()
        {
            npc.width = 36;
            npc.height = 56;
            npc.damage = 1;
            npc.defense = 10;
            npc.lifeMax = 500;
            npc.HitSound = SoundID.NPCHit42;
            npc.DeathSound = SoundID.NPCDeath1;
            npc.value = 10f;
            npc.knockBackResist = 0.6f;
            npc.noGravity = false;
            npc.aiStyle = -1;

            npc.direction = 1;
        }

        /*
        ai[0] : state | main state
        ai[0] : state | counts blocks for jump height
        ai[2] : timer | 1 acts as timer for losing intrest, timer for looking for a new target
        ai[3] : timer | 2 acts as dash warmup, counts jump attempts until turn around when no target
        */

        //Main stuck points: zipping up blocks/slabs, and falling through platforms
        //zombies check a bit closer for blocks

        public override void OnHitByItem(Player player, Item item, int damage, float knockback, bool crit)
        {
            if (npc.HasValidTarget)
            {
                if (damage >= changeAgroDamage)
                {
                    npc.target = player.whoAmI;
                }
            }
            else
            {
                npc.target = player.whoAmI;
                npc.ai[2] = 0;
                npc.ai[3] = 0;
            }
        }
        public override void OnHitByProjectile(Projectile projectile, int damage, float knockback, bool crit)
        {
            if (npc.HasValidTarget)
            {
                if (damage >= changeAgroDamage)
                {
                    npc.target = projectile.owner;
                }
            }
            else
            {
                npc.target = projectile.owner;
                npc.ai[2] = 0;
                npc.ai[3] = 0;
            }
        }


        const int intrestTime = 800; //time before gained intrest
        const int loseIntrestTime = 300; //time before lost intrest

        const int changeAgroDamage = 5; //how much damage minimum from another player changes the agro
        const int countdownSpeed = 2; //how fast the charge timer counts down then target is not in range
        const int maxRangeTimer = 250; //how long the charge timer lasts
        const float walkSpeedMax = 1.5f;
        const float dashSpeedMax = 4f;
        const int distancePastPlayer = 75; //if npc is past the player, how long should it go before it stops and turns around
        const int jumpheight = 6; //not max jumpheight in blocks, max of blocks checked, this increases jumpheight in blocks exponentially. real jump height is 3 more than this (3 is the minimum for 1 block)
        Vector2 detectRange = new Vector2(400, 100); //detect range for dash

        public override void AI()
        {
            Player target = new Player();
            if (npc.HasValidTarget)
            {
                target = Main.player[npc.target];
            }

            /*
                int playerSide = Math.Min(Math.Max((int)(target.position.X - npc.position.X), -1), 1);
                npc.direction = playerSide; old version. saved in case npc direction and player side need to be seperate
            */

            /*if (true == false) //easy disabling
            {
                Main.NewText("DEBUG:"); //guess what this is for
                Main.NewText("state 1: " + npc.ai[0]); //debug
                Main.NewText("state 2: " + npc.ai[1]); //debug
                Main.NewText("timer 1: " + npc.ai[2]); //debug
                Main.NewText("timer 2: " + npc.ai[3]); //debug
                Main.NewText(npc.velocity.X + " " + npc.velocity.Y); //debug
            }*/ //debug

            switch (npc.ai[0])//main switch
            {
                case 0: //walking (handles wandering and tracking)
                    npc.spriteDirection = npc.direction;

                    if (npc.HasValidTarget)
                    {
                        npc.direction = Math.Min(Math.Max((int)(target.Center.X - npc.Center.X), -1), 1);
                        npc.spriteDirection = npc.direction;

                        if (npc.position.X == npc.oldPosition.X)//note: may have to change this to if new pos is within range of old pos
                        {
                            npc.ai[2]++; //losing intrest because npc is stuck
                        }
                        else if (npc.ai[2] > 0)
                        {
                            npc.ai[2]--; //npc unstuck
                        }

                        if (npc.ai[2] >= loseIntrestTime) //intrest timer
                        {
                            npc.target = 255;
                            npc.ai[2] = 0;
                            npc.ai[3] = 0;
                        }

                        if (Math.Abs(target.Center.Y - npc.Center.Y) < detectRange.Y && Math.Abs(target.Center.X - npc.Center.X) < detectRange.X)
                        { //if player is within range, start the dash countdown
                            npc.ai[3]++;
                            //Main.NewText("within range"); //debug
                        }
                        else if (npc.ai[3] >= countdownSpeed)
                        { //else decrease timer
                            npc.ai[3] -= countdownSpeed;
                        }

                        if (npc.ai[3] >= maxRangeTimer)
                        {//timer max, reset ai[]s and move to next step

                            for (int y = 0; y < 30; y++)//placeholder dash dust
                            {
                                Dust.NewDustPerfect(new Vector2(npc.Center.X - ((npc.width / 2) * npc.direction) + Main.rand.Next(-5, 5), Main.rand.Next((int)npc.position.Y + 5, (int)npc.position.Y + npc.height) - 5), 31, new Vector2((Main.rand.Next(-20, 30) * 0.03f) * npc.direction, Main.rand.Next(-20, 20) * 0.02f), 0, default, 2);
                            }



                            npc.ai[2] = 0;
                            npc.ai[3] = 0;
                            npc.ai[0] = 1;//start dash
                        }
                    }
                    else //if no target
                    {

                        npc.ai[2]++;

                        if (npc.ai[3] >= 2 && npc.velocity.Y == 0)
                        {
                            npc.direction = -npc.direction;
                            npc.ai[3] = 0;
                        }

                        if (npc.ai[2] >= intrestTime)
                        {
                            npc.TargetClosest();
                            npc.ai[2] = 0;
                            npc.ai[3] = 0;
                        }
                    }

                    if (npc.velocity.Y == 0)//jumping. note: (the could be moved to just before it sets the velocity high in MoveVertical())
                    {
                        MoveVertical(true);
                    }

                    Move(walkSpeedMax);

                    break;

                case 1: //start dash
                    if (npc.velocity.Y == 0)//jumping. note: (the could be moved to just before it sets the velocity high in MoveVertical())
                    {
                        MoveVertical(false);
                        if (Main.rand.Next(4) == 0)//placeholder dash
                        {
                            Dust.NewDustPerfect(new Vector2(npc.Center.X, npc.position.Y + npc.height), 16, new Vector2((Main.rand.Next(-20, 20) * 0.02f), Main.rand.Next(-20, 20) * 0.02f), 0, default, 1.2f);
                        }
                    }

                    Move(dashSpeedMax);

                    if (npc.collideX && npc.position.X == npc.oldPosition.X && npc.velocity.X == 0)//note: npc.velocity.X == 0 seemed to fix catching on half blocks
                    {
                        Collide(); //thunk

                        npc.ai[3] = 0;
                        npc.ai[2] = 0;
                        npc.ai[0] = 2;//bonk cooldown, then back to case 0
                        break;
                    }

                    if (npc.direction != Math.Min(Math.Max((int)(target.Center.X - npc.Center.X), -1), 1) || !npc.HasValidTarget)
                    {
                        npc.ai[3]++;
                    }

                    if (npc.ai[3] >= distancePastPlayer)
                    {
                        npc.direction = -npc.direction;
                        npc.spriteDirection = npc.direction;
                        npc.ai[3] = 0;//slide to a halt and then back to case 0
                        npc.ai[2] = 1;//tells case 2 to spawn particles 
                        npc.ai[0] = 2;//turns out this case is the exact same for both
                        break;
                    }
                    break;

                case 2:
                    npc.ai[3]++;
                    npc.velocity.X *= 0.95f;
                    if (npc.ai[2] == 1)//this checks if this is for hitting a wall or slowing down
                    {

                    }

                    if (npc.ai[3] >= 50)
                    {
                        npc.ai[3] = 0;
                        npc.ai[2] = 0;
                        npc.ai[0] = 0;
                    }
                    break;
            }

        }

        void Move(float speed) //note: seperated for simplicity
        {
            if ((npc.velocity.X * npc.direction) <= speed)//getting up to max speed
            {
                npc.velocity.X += 0.1f * npc.direction;
            }
            else if ((npc.velocity.X * npc.direction) >= speed + 0.1f)//slowdown if too fast
            {
                npc.velocity.X -= 0.2f * npc.direction;
            }
        }

        void MoveVertical(bool jump) //idea: could be seperated farther
        {
            npc.ai[1] = 0;//reset jump counter
            for (int y = 0; y < jumpheight; y++)//idea: this should have diminishing results for output jump height
            {
                Tile tileType = Framing.GetTileSafely((int)(npc.position.X / 16) + (npc.direction * 2) + 1, (int)((npc.position.Y + npc.height + 8) / 16) - y - 1);
                if ((Main.tileSolid[tileType.type] || Main.tileSolidTop[tileType.type]) && tileType.active()) //how tall the wall is
                {
                    npc.ai[1] = (y + 1);
                }
                if (y >= npc.ai[1] + (npc.height / 16)) //stops counting if there is room for the npc to walk under //((int)((npc.position.Y - target.position.Y) / 16) + 1)
                {
                    if (npc.HasValidTarget && jump)
                    {
                        Player target = Main.player[npc.target];
                        if (npc.ai[1] >= ((int)((npc.position.Y - target.position.Y) / 16) + 1) - ((int)(npc.height / 16) - 1))
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (npc.ai[1] > 0)//jump and step up
            {
                Tile tileType = Framing.GetTileSafely((int)(npc.position.X / 16) + (npc.direction * 2) + 1, (int)((npc.position.Y + npc.height + 8) / 16) - 1);
                if (npc.ai[1] == 1 && npc.collideX)
                {
                    if (tileType.halfBrick() || (Main.tileSolid[tileType.type] && (npc.position.Y % 16) == 0))
                    {
                        npc.position.Y -= 8;//note: these just zip the npc up the block and it looks bad, need to figure out how vanilla glides them up
                        npc.velocity.X = npc.oldVelocity.X;
                    }
                    else if (Main.tileSolid[tileType.type])
                    {
                        npc.position.Y -= 16;
                        npc.velocity.X = npc.oldVelocity.X;
                    }
                }
                else if (npc.ai[1] == 2 && (npc.position.Y % 16) == 0 && Framing.GetTileSafely((int)(npc.position.X / 16) + (npc.direction * 2) + 1, (int)((npc.position.Y + npc.height) / 16) - 1).halfBrick())
                {//note: I dislike this extra check, but couldn't find a way to avoid it
                    if (npc.collideX)
                    {
                        npc.position.Y -= 16;
                        npc.velocity.X = npc.oldVelocity.X;
                    }
                }
                else if (npc.ai[1] > 1 && jump == true)
                {

                    /*if (npc.position.Y > target.position.Y)
                    {
                        npc.velocity.Y = -(3 + jumpheight); //note: this raises the jump height to max if player is above npc, similar to zombies
                    }
                    else
                    {*/

                    npc.velocity.Y = -(3 + npc.ai[1]);
                    if (!npc.HasValidTarget && npc.velocity.X == 0)
                    {
                        npc.ai[3]++;
                    }
                }
            }
        }

        void Collide() //bonk
        {
            //note: if this is effected by player's dash, move dusts to where this is called, or add a check
            for (int y = 0; y < 12; y++)
            {
                Dust.NewDustPerfect(new Vector2(npc.Center.X - ((npc.width / 2) * -npc.direction), Main.rand.Next((int)npc.position.Y, (int)npc.position.Y + npc.height)), 53, new Vector2((Main.rand.Next(0, 20) * 0.08f) * -npc.direction, Main.rand.Next(-10, 10) * 0.04f), 0, default, 1.2f);
            }
            Main.PlaySound(SoundID.NPCHit42, npc.Center);
            npc.velocity.X -= (4 * npc.direction);
            npc.velocity.Y -= 4;
        }

        public override void FindFrame(int frameHeight)//note: this controls everything to do with the npc frame
        {
            npc.frameCounter += Math.Abs(npc.velocity.X);//note: slightly jank, but best I could come up with
            if ((int)(npc.frameCounter / 10) > Main.npcFrameCount[npc.type] - 1)
            {
                npc.frameCounter = 0;
            }
            npc.frame.Y = (int)(npc.frameCounter / 10) * frameHeight;
            //Main.NewText(npc.frame.Y / frameHeight); //debug
        }


        /*public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            return (spawnInfo.player.ZoneRockLayerHeight && spawnInfo.player.GetModPlayer<BiomeHandler>().ZoneGlass) ? 1f : 0f;
        }*/

        public override void NPCLoot()
        {

        }
    }
}
