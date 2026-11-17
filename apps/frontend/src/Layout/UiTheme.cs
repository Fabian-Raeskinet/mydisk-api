using MudBlazor;

namespace MyDisks.Frontend.Layout;

/// <summary>Shared design tokens. Component behavior and responsive layout remain MudBlazor's.</summary>
public static class UiTheme
{
    public static MudTheme Default { get; } = new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#153F35",
            Secondary = "#B36C50",
            Warning = "#AD8940",
            AppbarBackground = "#FCFBF7",
            AppbarText = "#495D51",
            Background = "#F8F7F2",
            Surface = "#FFFEFA",
            TextPrimary = "#203B34",
            TextSecondary = "#727B70",
            DrawerBackground = "#153F35",
            DrawerText = "#C3D4C9",
            DrawerIcon = "#B8CCC1",
            Divider = "#E5E6DD",
            LinesDefault = "#E5E6DD",
            TableLines = "#E5E6DD"
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "10px",
            DrawerWidthLeft = "238px",
            AppbarHeight = "72px"
        },
        Typography = new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = ["-apple-system", "BlinkMacSystemFont", "Segoe UI", "sans-serif"]
            },
            H3 = new H3Typography { FontSize = "2.5rem", FontWeight = "700", LetterSpacing = "-0.04em" },
            H4 = new H4Typography { FontWeight = "700", LetterSpacing = "-0.04em" },
            H5 = new H5Typography { FontWeight = "650", LetterSpacing = "-0.025em" },
            H6 = new H6Typography { FontWeight = "600", LetterSpacing = "-0.02em" },
            Button = new ButtonTypography { TextTransform = "none", FontWeight = "600" }
        }
    };
}
