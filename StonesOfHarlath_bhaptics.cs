using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MelonLoader;
using HarmonyLib;
using UnityEngine;

using MyBhapticsTactsuit;

namespace StonesOfHarlath_bhaptics
{
    public class StonesOfHarlath_bhaptics : MelonMod
    {
        public static TactsuitVR tactsuitVr;
        public static bool dominantRightHand = true;

        public override void OnApplicationStart()
        {
            base.OnApplicationStart();
            tactsuitVr = new TactsuitVR();
            tactsuitVr.PlaybackHaptics("HeartBeat");
        }


        #region General updates

        [HarmonyPatch(typeof(PlayerCombatUnit), "UpdateHealthBar", new Type[] {  })]
        public class bhaptics_UpdateHealth
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerCombatUnit __instance)
            {
                if (__instance.IsDead) { tactsuitVr.StopHeartBeat(); return; }
                if (__instance.currentHealth <= 0.25f * __instance.maxHealth) { tactsuitVr.StartHeartBeat(); }
                else { tactsuitVr.StopHeartBeat(); }
            }
        }

        [HarmonyPatch(typeof(PlayerCombatUnit), "GainHealth", new Type[] { typeof(float) })]
        public class bhaptics_GainHealth
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerCombatUnit __instance, float healthAmount)
            {
                tactsuitVr.LOG("Gain health: " + healthAmount.ToString());
                tactsuitVr.PlaybackHaptics("Healing");
            }
        }

        [HarmonyPatch(typeof(PlayerSword), "SwapCombatHands", new Type[] { typeof(bool) })]
        public class bhaptics_SwapHandsSword
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerSword __instance, bool lefty)
            {
                dominantRightHand = !lefty;
            }
        }

        [HarmonyPatch(typeof(PlayerControls), "SwapCombatHands", new Type[] { typeof(bool) })]
        public class bhaptics_SwapHandsControls
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerControls __instance, bool lefty)
            {
                dominantRightHand = !lefty;
            }
        }

        #endregion

        #region Melee combat

        [HarmonyPatch(typeof(ShieldBlock), "DidBlock", new Type[] { })]
        public class bhaptics_ShieldBlock
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.Recoil("Block", !dominantRightHand);
            }
        }

        [HarmonyPatch(typeof(PlayerSword), "OnCollisionEnter", new Type[] { typeof(Collision) })]
        public class bhaptics_SwordCollision
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerSword __instance, Collision other)
            {
                tactsuitVr.LOG("Sword damage: " + __instance.Damage);
                tactsuitVr.Recoil("Blade", dominantRightHand);
            }
        }

        [HarmonyPatch(typeof(PlayerCombatUnit), "SwitchCombatMode", new Type[] { })]
        public class bhaptics_SwitchCombatMode
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerCombatUnit __instance)
            {
                tactsuitVr.PlaybackHaptics("Recoil_L", 0.5f);
                tactsuitVr.PlaybackHaptics("Recoil_R", 0.5f);
            }
        }


        #endregion

        #region Magic combat

        [HarmonyPatch(typeof(SpellFire), "CastSpell", new Type[] { typeof(int) })]
        public class bhaptics_CastFireSpell
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.CastSpell("Fire", dominantRightHand);
            }
        }

        [HarmonyPatch(typeof(SpellBarrier), "CastSpell", new Type[] { typeof(int) })]
        public class bhaptics_CastBarrierSpell
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.CastSpell("Barrier", dominantRightHand);
            }
        }

        [HarmonyPatch(typeof(SpellIce), "CastSpell", new Type[] { typeof(int) })]
        public class bhaptics_CastIceSpell
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.CastSpell("Ice", dominantRightHand);
            }
        }

        [HarmonyPatch(typeof(SpellLightning), "CastSpell", new Type[] { typeof(int) })]
        public class bhaptics_CastLightningSpell
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.CastSpell("Lightning", dominantRightHand);
            }
        }

        [HarmonyPatch(typeof(SpellSpaceTime), "SlowTime", new Type[] {  })]
        public class bhaptics_SlowTime
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.StartHeartBeatSlow();
            }
        }

        [HarmonyPatch(typeof(SpellSpaceTime), "ResumeSpeed", new Type[] { })]
        public class bhaptics_ResumeSpeed
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.StopHeartBeatSlow();
            }
        }

        [HarmonyPatch(typeof(SpellSpaceTime), "CastSpell", new Type[] { typeof(int) })]
        public class bhaptics_CastSpaceTimeSpell
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.CastSpell("SpaceTime", dominantRightHand);
            }
        }

        [HarmonyPatch(typeof(SpellSpaceTime), "Teleport", new Type[] { })]
        public class bhaptics_Teleport
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.PlaybackHaptics("TeleportThrough");
            }
        }

        [HarmonyPatch(typeof(SpellNature), "CastSpell", new Type[] { typeof(int) })]
        public class bhaptics_CastNatureSpell
        {
            [HarmonyPostfix]
            public static void Postfix()
            {
                tactsuitVr.CastSpell("Nature", dominantRightHand);
            }
        }

        #endregion

        #region Take damage

        [HarmonyPatch(typeof(PlayerCombatUnit), "TakeDamage", new Type[] { typeof(float) })]
        public class bhaptics_SwapHands
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerCombatUnit __instance, float initialDamage)
            {
                tactsuitVr.PlaybackHaptics("Impact");
            }
        }


        #endregion

    }
}
