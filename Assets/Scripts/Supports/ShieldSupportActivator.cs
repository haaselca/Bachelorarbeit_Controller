public class ShieldSupportActivator : GenericSupportActivator
{
    public delegate void OnShieldToggle(bool isShieldActive);
    public static event OnShieldToggle onShieldToggle;

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void ActivateSupport(string support)
    {
        EnableInvincibility(support);
    }

    private static void EnableInvincibility(string support)
    {
        if (support == "ShieldSupport")
        {
            onShieldToggle(true);
        }
    }

    protected override void DeactivateSupport()
    {
        onShieldToggle(false);
    }
}
