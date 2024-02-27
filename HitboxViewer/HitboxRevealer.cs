using System.Collections;
using System.Security;
using System.Security.Permissions;
using UnityEngine;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace HitboxViewer
{

    public class HitboxRevealer : MonoBehaviour
    {
        public static float cfg_BoxAlpha = 0.22f;
        public static float cfg_HurtAlpha = 0.22f;

        public static bool showingAnyBoxes => Configs.showingAnyBoxes.Value;
        public static bool showingHitBoxes = Configs.showingHitBoxes.Value;
        public static bool showingHurtBoxes = Configs.showingHurtBoxes.Value;

        private Renderer rend;

        private MaterialPropertyBlock _matProperties;
        private Color _matColor;

        void Awake()
        {
            _matProperties = new MaterialPropertyBlock();

            rend = GetComponent<Renderer>();

            rend.GetPropertyBlock(_matProperties);

            rend.enabled = false;
        }

        public static bool showing = true;

        public enum boxType
        {
            HIT,
            HURT
        }

        public HitboxRevealer Init(boxType box, Transform boxParentTransform, bool isMerc = false)
        {
            //_overlapAttackTransform = boxParentTransform;
            transform.parent = boxParentTransform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            gameObject.SetActive(true);

            //float setAlpha = getBoxAlpha(box);
            //float setLum = 0.6f;
            //float setHue = 0.00f; //random min
            //float setHueHue = 1.00f; //random max

            //_matColor = Random.ColorHSV(setHue, setHueHue, 0.5f, 0.5f, setLum, setLum);

            //_matColor.a = setAlpha;

            //_matProperties.SetColor("_Color", _matColor);

            //rend.SetPropertyBlock(_matProperties);

            return this;
        }

        private float getBoxAlpha(boxType box)
        {
            switch (box)
            {
                default:
                case boxType.HIT:
                    return cfg_BoxAlpha;
                case boxType.HURT:
                    return cfg_HurtAlpha;
            }
        }

        #region InitHurtboxes at positions and rotations

        public HitboxRevealer InitHitbox(Vector3 position, float radius)
        {
            Init(boxType.HIT, null);

            transform.position = position;
            transform.localScale = Vector3.one * radius * 2;

            ShowHitbox(true, 5f);

            return this;
        }

        public static void InitSpheresAlongSpherecast(Vector3 origin, Vector3 direction, float maxDistance, float radius)
        {
            int spheres = 3;
            for (int i = 0; i <= spheres; i++)
            {
                Vector3 spawnPosition = origin + direction * maxDistance / spheres * i;

                Instantiate(Prefabs.hitboxSphere).InitHitbox(spawnPosition, radius);
            }
        }

        public HitboxRevealer InitHurtbox(Transform capsuleTransform, CapsuleCollider capsuleCollider, bool kino = false)
        {
            Init(boxType.HURT, capsuleTransform);

            transform.localPosition = capsuleCollider.center;
            transform.localScale = new Vector3(capsuleCollider.radius * 2, capsuleCollider.height / 2, capsuleCollider.radius * 2);
            switch (capsuleCollider.direction)
            {
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

            ShowHurtbox(true);
            return this;
        }

        public HitboxRevealer InitHurtbox(Transform sphereTransform, SphereCollider sphereCollider)
        {
            Init(boxType.HURT, sphereTransform);
            transform.localPosition = sphereCollider.center;
            transform.localScale = Vector3.one * sphereCollider.radius * 2;

            ShowHurtbox(true);
            return this;
        }

        public HitboxRevealer InitHurtbox(Transform boxTransform, BoxCollider boxCollider)
        {
            Init(boxType.HURT, boxTransform);

            transform.localPosition = boxCollider.center;
            transform.localScale = boxCollider.size;

            ShowHurtbox(true);
            return this;
        }
        #endregion InitHurtboxes at positions and rotations

        public void ShowHurtbox(bool active)
        {
            active &= showingAnyBoxes && showingHurtBoxes;
            if (rend)
            {
                rend.enabled = active;
            }
        }

        public void ShowHitbox(bool active, float showTime)
        {
            active &= showingAnyBoxes && showingHitBoxes;
            if (rend)
            {
                rend.enabled = active;
            }

            StartCoroutine(TimedRemoveBlast(showTime));
        }


        private IEnumerator TimedRemoveBlast(float killTime)
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            //_matColor *= 0.69f;
            //_matColor.a *= 1.449f;

            //_matProperties.SetColor("_Color", _matColor);
            //rend.SetPropertyBlock(_matProperties);

            yield return new WaitForSeconds(killTime);

            if (gameObject)
            {
                Destroy(gameObject);
            }
        }
    }
}
