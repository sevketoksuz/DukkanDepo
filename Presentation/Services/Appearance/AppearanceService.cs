namespace DukkanDepo.Presentation.Services.Appearance;

public static class AppearanceService
{
    public static AppearanceSettings Current { get; private set; } = new();

    public static void LoadAndApply()
    {
        Current = AppearanceStore.LoadOrDefault();
        Apply(Current);
    }

    public static void SaveAndApply(AppearanceSettings settings)
    {
        Current = settings ?? new AppearanceSettings();

        AppearanceStore.Save(Current);
        Apply(Current);
    }

    public static void Apply(AppearanceSettings? settings)
    {
        AppearanceApplier.Apply(settings ?? new AppearanceSettings());
    }
}