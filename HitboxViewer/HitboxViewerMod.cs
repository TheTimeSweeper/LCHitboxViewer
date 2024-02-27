using BepInEx;
using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
using UnityEngine;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace HitboxViewer
{

    [BepInPlugin("TheTimeSweeper.HitboxViewerMod", "HitboxViewerMod", "0.0.69")]
    public class HitboxViewerMod : BaseUnityPlugin
    {
        private static List<HitboxRevealer> _hurtboxRevealers = new List<HitboxRevealer>();

        void Awake()
        {
            Log.Init(Logger);

            Configs.Init(Config);

            On.Shovel.HitShovel += Shovel_HitShovel;
            On.EnemyAI.Start += EnemyAI_Start;
            //On.EnemyAICollisionDetect.IHittable_Hit += EnemyAICollisionDetect_IHittable_Hit;
        }

        //private bool EnemyAICollisionDetect_IHittable_Hit(On.EnemyAICollisionDetect.orig_IHittable_Hit orig, EnemyAICollisionDetect self, int force, Vector3 hitDirection, GameNetcodeStuff.PlayerControllerB playerWhoHit, bool playHitSFX) {
        //    Log.Message("actually fuckin hit :3");
        //    return orig(self, force, hitDirection, playerWhoHit, playHitSFX);
        //}

        //idk what constitutes a hitbox. just gonna put them on every collider on the enemy
        private void EnemyAI_Start(On.EnemyAI.orig_Start orig, EnemyAI self)
        {
            orig(self);
            foreach (Collider col in self.GetComponentsInChildren<Collider>(true))
            {
                if (col is CapsuleCollider)
                {
                    //plese go to prefabs and load a better material
                    _hurtboxRevealers.Add(Instantiate(Prefabs.hitboxCapsule).InitHurtbox(col.transform, col as CapsuleCollider));
                }
                if (col is SphereCollider)
                {
                    _hurtboxRevealers.Add(Instantiate(Prefabs.hitboxSphere).InitHurtbox(col.transform, col as SphereCollider));
                }
                if (col is BoxCollider)
                {
                    _hurtboxRevealers.Add(Instantiate(Prefabs.hitboxCube).InitHurtbox(col.transform, col as BoxCollider));
                }
            }
        }

        private void Shovel_HitShovel(On.Shovel.orig_HitShovel orig, Shovel self, bool cancel)
        {
            orig(self, cancel);

            //if (!cancel) 
                HitboxRevealer.InitSpheresAlongSpherecast(
                    self.previousPlayerHeldBy.gameplayCamera.transform.position + self.previousPlayerHeldBy.gameplayCamera.transform.right * -0.35f,
                    self.previousPlayerHeldBy.gameplayCamera.transform.forward,
                    1.5f,
                    0.8f);
            //}
        }

        public static void BindShowAllHurtboxes()
        {
            Log.Warning("update");
            bool shouldShow = HitboxRevealer.showingAnyBoxes && HitboxRevealer.showingHurtBoxes;

            for (int i = _hurtboxRevealers.Count - 1; i >= 0; i--)
            {
                if (_hurtboxRevealers[i] == null)
                {
                    _hurtboxRevealers.RemoveAt(i);
                    continue;
                }
                _hurtboxRevealers[i].ShowHurtbox(shouldShow);
            }
        }
    }
}
