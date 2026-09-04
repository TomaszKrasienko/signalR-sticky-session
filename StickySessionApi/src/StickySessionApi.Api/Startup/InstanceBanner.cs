using Figgle.Fonts;
using StickySessionApi.Infrastructure.Messaging;

namespace StickySessionApi.Api.Startup;

public static class InstanceBanner
{
    public static void Print(InstanceId instanceId)
    {
        Console.WriteLine(FiggleFonts.Standard.Render("StickySessionApi"));
        Console.WriteLine($"Instance ID: {instanceId.Value}");
    }
}
