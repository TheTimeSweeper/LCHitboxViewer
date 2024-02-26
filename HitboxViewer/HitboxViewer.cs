using BepInEx;
using System;
using System.Collections;
using System.Security;
using System.Security.Permissions;
using UnityEngine;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace HitboxViewer {

    public class Revealinator : MonoBehaviour {
        public static bool showing = true;

        public enum boxType {
            HIT,
            BLAST,
            BULLET,
            BULLET_THIN,
            BULLET_POINT,
            HURT,
        }

        //void Update() {
        //    if (Input.GetKeyDown(KeyCode.Semicolon)) {
        //        GetComponent<Renderer>().enabled = !GetComponent<Renderer>().enabled;
        //    }
        //}

        public Revealinator init(boxType box, Transform boxParentTransform, bool isMerc = false) {

            //_overlapAttackTransform = boxParentTransform;
            transform.parent = boxParentTransform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            //if (VRCompat.enabled && box == boxType.HIT) transform.parent = null;

            //float setAlpha = getBoxAlpha(box);
            //float setLum = 0.6f;
            //float setHue = 0.00f; //random min
            //float setHueHue = 1.00f; //random max

            //if (isMerc && cfg_MercSoften) {
            //    //low alpha, and colors in cool blue-ish range
            //    setAlpha = 0.12f;
            //    setLum = 0.4f;
            //    setHue = 0.35f;
            //    setHueHue = 0.90f;
            //}

            //_matColor = Random.ColorHSV(setHue, setHueHue, 0.5f, 0.5f, setLum, setLum);

            //_matColor.a = setAlpha;
            ////Utils.LogReadout($"init box. alpha: {setAlpha}");

            //_matProperties.SetColor("_Color", _matColor);

            //rend.SetPropertyBlock(_matProperties);

            return this;
        }

        public static void InitSpheres(Vector3 origin, Vector3 direction, float maxDistance, float radius) {
            int spheres = 3;
            for (int i = 0; i <= spheres; i++) {

                Vector3 spawnPosition = origin + direction * maxDistance/spheres * i;

                Instantiate(HitboxViewer.spheer).InitSphere(spawnPosition, radius);
            }
        }

        public Revealinator InitSphere(Vector3 position, float radius) {
            init(boxType.BLAST, null);

            transform.position = position;
            transform.localScale = Vector3.one * radius * 2;

            blastboxShow(true, 5f);

            return this;
        }

        public Revealinator initHurtbox(Transform capsuleTransform, CapsuleCollider capsuleCollider, bool kino = false) {
            init(boxType.HURT, capsuleTransform);

            transform.localPosition = capsuleCollider.center;
            transform.localScale = new Vector3(capsuleCollider.radius * 2, capsuleCollider.height / 2, capsuleCollider.radius * 2);
            switch (capsuleCollider.direction) {
                case 0:
                    transform.localEulerAngles = new Vector3(0, 0, 90);
                    break;
                case 1:
                    //stay
                    break;
                case 2:
                    transform.localEulerAngles = new Vector3(90, 0, 0);
                    break;
            }

            hurtboxShow(true);
            return this;
        }

        public Revealinator initHurtbox(Transform sphereTransform, SphereCollider sphereCollider) {
            init(boxType.HURT, sphereTransform);
            transform.localPosition = sphereCollider.center;
            transform.localScale = Vector3.one * sphereCollider.radius * 2;

            hurtboxShow(true);
            return this;
        }

        public Revealinator initHurtbox(Transform boxTransform, BoxCollider boxCollider) {
            init(boxType.HURT, boxTransform);

            transform.localPosition = boxCollider.center;
            transform.localScale = boxCollider.size;

            hurtboxShow(true);
            return this;
        }

        private void hurtboxShow(bool v) {
            gameObject.SetActive(true);
        }

        public void blastboxShow(bool active, float showTime) {

            gameObject.SetActive(true);

            StartCoroutine(timedRemoveBlast(showTime));
        }

        private IEnumerator timedRemoveBlast(float killTime) {

            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            //_matColor *= 0.69f;
            //_matColor.a *= 1.449f;

            //_matProperties.SetColor("_Color", _matColor);
            //rend.SetPropertyBlock(_matProperties);

            yield return new WaitForSeconds(killTime);

            if (gameObject) {
                //show(false);
                //HitboxViewerMod.instance.returnPooledBlastRevealer(this);
                Destroy(gameObject);
            }
        }
    }

    [BepInPlugin("TheTimeSweeper.HitboxViewerMod2", "HitboxViewerMod2", "0.0.69")]
    public class HitboxViewer : BaseUnityPlugin {

        private static Revealinator spheere;
        public static Revealinator spheer {
            get {
                if (spheere == null)
                    spheere = createRevealinator(PrimitiveType.Sphere);
                return spheere;
            }
        }
        private static Revealinator booxe;
        public static Revealinator boox {
            get {
                if (booxe == null)
                    booxe = createRevealinator(PrimitiveType.Cube);
                return booxe;
            }
        }
        private static Revealinator capsuule;
        public static Revealinator capsuul {
            get {
                if (capsuule == null)
                    capsuule = createRevealinator(PrimitiveType.Capsule);
                return capsuule;
            }
        }

        void Awake() {
            
            Logger.LogWarning("nip");
            On.Shovel.HitShovel += Shovel_HitShovel;
            On.EnemyAI.Start += EnemyAI_Start;
            On.EnemyAICollisionDetect.IHittable_Hit += EnemyAICollisionDetect_IHittable_Hit;
        }

        private bool EnemyAICollisionDetect_IHittable_Hit(On.EnemyAICollisionDetect.orig_IHittable_Hit orig, EnemyAICollisionDetect self, int force, Vector3 hitDirection, GameNetcodeStuff.PlayerControllerB playerWhoHit, bool playHitSFX) {
            Logger.LogWarning("actually fuckin hit :3");
            return orig(self, force, hitDirection, playerWhoHit, playHitSFX);

        }

        private void EnemyAI_Start(On.EnemyAI.orig_Start orig, EnemyAI self) {
            orig(self);
            foreach(Collider col in self.GetComponentsInChildren<Collider>(true)) {

                if (col is CapsuleCollider) {
                    Instantiate(capsuul).initHurtbox(col.transform, col as CapsuleCollider);
                }
                if (col is SphereCollider) {
                    Instantiate(spheer).initHurtbox(col.transform, col as SphereCollider);
                }
                if (col is BoxCollider) {
                    Instantiate(boox).initHurtbox(col.transform, col as BoxCollider);
                }
            }
        }

        private static Revealinator createRevealinator(PrimitiveType primitivetype) {

            Revealinator prefab = GameObject.CreatePrimitive(primitivetype).AddComponent<Revealinator>();
            DestroyImmediate(prefab.GetComponent<Collider>());
            DontDestroyOnLoad(prefab);
            prefab.gameObject.SetActive(false);

            GameNetcodeStuff.PlayerControllerB playerControllerB = GameObject.FindObjectOfType<GameNetcodeStuff.PlayerControllerB>();  
            if(playerControllerB == null) {
                Debug.LogWarning("no player");
                return prefab;
            }
            Transform lod = playerControllerB.transform.Find("ScavengerModel/LOD1");
            if (lod == null) {
                Debug.LogWarning("no lod");
                return prefab;
            }
            Renderer renderer = lod.GetComponent<Renderer>();
            if (renderer == null) {
                Debug.LogWarning("no renderer");
                return prefab;
            }
            Material playerMat = renderer.sharedMaterial;
            if (playerMat == null) {
                Debug.LogWarning("no playerMat");
                return prefab;
            }
            prefab.GetComponent<Renderer>().sharedMaterial = playerMat;
            //spheere.gameObject.layer = lod.gameObject.layer;

            return prefab;
        }

        private void Shovel_HitShovel(On.Shovel.orig_HitShovel orig, Shovel self, bool cancel) {
            orig(self, cancel);
            
            if (!cancel) {
                Revealinator.InitSpheres(
                    self.previousPlayerHeldBy.gameplayCamera.transform.position + self.previousPlayerHeldBy.gameplayCamera.transform.right * -0.35f,
                    self.previousPlayerHeldBy.gameplayCamera.transform.forward,
                    1.5f,
                    0.8f);
            }
        }
    }
}
