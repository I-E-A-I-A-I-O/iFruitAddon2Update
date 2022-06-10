
namespace iFruitAddon2.Image_Types
{
    public sealed class Wallpaper : PhoneImage
    {
        /// <summary>
        /// Initialize the class.
        /// </summary>
        /// <param name="txd"></param>
        public Wallpaper(string txd) : base(txd)
        { }

        public static Wallpaper IFruitDefault => new("Phone_Wallpaper_ifruitdefault");
        public static Wallpaper BadgerDefault => new("Phone_Wallpaper_badgerdefault");
        public static Wallpaper Bittersweet => new("Phone_Wallpaper_bittersweet_b");
        public static Wallpaper PurpleGlow => new("Phone_Wallpaper_purpleglow");
        public static Wallpaper GreenSquares => new("Phone_Wallpaper_greensquares");
        public static Wallpaper OrangeHerringBone => new("Phone_Wallpaper_orangeherringbone");
        public static Wallpaper OrangeHalftone => new("Phone_Wallpaper_orangehalftone");
        public static Wallpaper GreenTriangles => new("Phone_Wallpaper_greentriangles");
        public static Wallpaper GreenShards => new("Phone_Wallpaper_greenshards");
        public static Wallpaper BlueAngles => new("Phone_Wallpaper_blueangles");
        public static Wallpaper BlueShards => new("Phone_Wallpaper_blueshards");
        public static Wallpaper BlueTriangles => new("Phone_Wallpaper_bluetriangles");
        public static Wallpaper BlueCircles => new("Phone_Wallpaper_bluecircles");
        public static Wallpaper Diamonds => new("Phone_Wallpaper_diamonds");
        public static Wallpaper GreenGlow => new("Phone_Wallpaper_greenglow");
        public static Wallpaper Orange8Bit => new("Phone_Wallpaper_orange8bit");
        public static Wallpaper OrangeTriangles => new("Phone_Wallpaper_orangetriangles");
        public static Wallpaper PurpleTartan => new("Phone_Wallpaper_purpletartan");
    }
}
