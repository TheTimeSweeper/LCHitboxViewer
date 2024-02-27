using BepInEx.Configuration;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace HitboxViewer
{
    public class Configs
    {
        public static ConfigEntry<bool> showingAnyBoxes;
        public static ConfigEntry<bool> showingHitBoxes;
        public static ConfigEntry<bool> showingHurtBoxes;

        public static void Init(ConfigFile config)
        {
            string section = "Hello";

            showingAnyBoxes = config.Bind(
                section,
                "Show Boxes",
                true,
                "Show Any boxes. Overrides other configs");
            showingAnyBoxes.SettingChanged += ShowingHurtBoxes_SettingChanged;

            showingHitBoxes = config.Bind(
                section,
                "Show HitBoxes",
                true,
                "Show attacking hitboxes. Currently only shovel implemented");
            showingHurtBoxes = config.Bind(
                section,
                "Show Hurtboxes",
                true,
                "Show enemy Hurtboxes. Currently all colliders are showing");
            showingHurtBoxes.SettingChanged += ShowingHurtBoxes_SettingChanged;
        }

        private static void ShowingHurtBoxes_SettingChanged(object sender, System.EventArgs e)
        {
            HitboxViewerMod.BindShowAllHurtboxes();
        }
    }
}
