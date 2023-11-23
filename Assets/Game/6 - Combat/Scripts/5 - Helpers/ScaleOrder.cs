public class ScaleOrder {
    public int ShadowPoints = 0;
    public int LightPoints = 0;

    public ScaleOrder(int light, int dark) {
        ShadowPoints = dark;
        LightPoints = light;
    }
}
