using BepInEx;
using System;
using UnityEngine;

namespace HitboxViewer
{
    public class Prefabs
    {
        private static HitboxRevealer _hitboxSphere;
        public static HitboxRevealer hitboxSphere
        {
            get
            {
                if (_hitboxSphere == null)
                    _hitboxSphere = CreateHitboxSphere();
                return _hitboxSphere;
            }
        }

        private static HitboxRevealer CreateHitboxSphere()
        {
            return createRevealinator(PrimitiveType.Sphere);
        }

        private static HitboxRevealer _hitboxCube;
        public static HitboxRevealer hitboxCube
        {
            get
            {
                if (_hitboxCube == null)
                    _hitboxCube = CreateHitboxCube();
                return _hitboxCube;
            }
        }

        private static HitboxRevealer CreateHitboxCube()
        {
            return createRevealinator(PrimitiveType.Cube);
        }

        private static HitboxRevealer _hitboxCapsule;
        public static HitboxRevealer hitboxCapsule
        {
            get
            {
                if (_hitboxCapsule == null)
                    _hitboxCapsule = CreateHitboxCapsule();
                return _hitboxCapsule;
            }
        }

        private static HitboxRevealer CreateHitboxCapsule()
        {
            return createRevealinator(PrimitiveType.Capsule);
        }

        private static HitboxRevealer createRevealinator(PrimitiveType primitivetype)
        {

            HitboxRevealer prefab = GameObject.CreatePrimitive(primitivetype).AddComponent<HitboxRevealer>();
            prefab.gameObject.SetActive(false);
            UnityEngine.Object.DestroyImmediate(prefab.GetComponent<Collider>());
            UnityEngine.Object.DontDestroyOnLoad(prefab);

            Material hitboxMaterial = GetMaterial();
            prefab.GetComponent<Renderer>().sharedMaterial = hitboxMaterial;

            return prefab;
        }

        private static Material GetMaterial()
        {
            //please replace this with a better material. maybe object placing transparent material that one would be good, or just a custom one

            GameNetcodeStuff.PlayerControllerB playerControllerB = GameObject.FindObjectOfType<GameNetcodeStuff.PlayerControllerB>();
            if (playerControllerB == null)
            {
                Log.Warning("no player");
                return null;
            }
            Renderer renderer = playerControllerB.GetComponentInChildren<SkinnedMeshRenderer>();
            if (renderer == null)
            {
                Log.Warning("no renderer");
                return null;
            }
            Material playerMat = renderer.sharedMaterial;
            if (playerMat == null)
            {
                Log.Warning("no playerMat");
                return null;
            }

            return playerMat;
        }
    }
}