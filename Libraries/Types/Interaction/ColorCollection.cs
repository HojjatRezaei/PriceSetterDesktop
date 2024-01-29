namespace PriceSetterDesktop.Libraries.Types.Interaction
{
    using PriceSetterDesktop.Libraries.Types.Enum;
    using System;

    public class ColorCollection
    {
        private readonly string[] colorNameCollection =
            [
                "مشکی",
                "سیاه",
                "Black",
                "صورتی",
                "Pink",
                "ابی",
                "آبی",
                "Blue",
                "سبز",
                "Green",
                "سفید",
                "White",
            ];
        public ColorCollection(string articleName)
        {
            string searchResult = "";
            foreach (var item in colorNameCollection)
            {
                if (item != null && item.Contains(articleName))
                    searchResult = articleName;
            }
            ColorCollection? ins = default;
            switch (searchResult)
            {
                case "مشکی" or "سیاه" or "Black":
                    ins = new ColorCollection(ColorList.Black);
                    ColorNames = ins.ColorNames;
                    ColorRGBs = ins.ColorRGBs;
                    ColorHEXs = ins.ColorHEXs;
                    break;
                case "صورتی" or "Pink":
                    ins = new ColorCollection(ColorList.Pink);
                    ColorNames = ins.ColorNames;
                    ColorRGBs = ins.ColorRGBs;
                    ColorHEXs = ins.ColorHEXs;
                    break;
                case "ابی" or "آبی" or "Blue":
                    ins = new ColorCollection(ColorList.Blue);
                    ColorNames = ins.ColorNames;
                    ColorRGBs = ins.ColorRGBs;
                    ColorHEXs = ins.ColorHEXs;
                    break;
                case "سبز" or "Green":
                    ins = new ColorCollection(ColorList.Green);
                    ColorNames = ins.ColorNames;
                    ColorRGBs = ins.ColorRGBs;
                    ColorHEXs = ins.ColorHEXs;
                    break;
                case "زرد" or "طلایی" or "Yellow" or "Gold":
                    ins = new ColorCollection(ColorList.Yellow);
                    ColorNames = ins.ColorNames;
                    ColorRGBs = ins.ColorRGBs;
                    ColorHEXs = ins.ColorHEXs;
                    break;
                case "سفید" or "White":
                    ins = new ColorCollection(ColorList.White);
                    ColorNames = ins.ColorNames;
                    ColorRGBs = ins.ColorRGBs;
                    ColorHEXs = ins.ColorHEXs;
                    break;
                default:
                    ColorNames = [];
                    ColorRGBs = [];
                    ColorHEXs = [];
                    break;
            }
        }
        public ColorCollection(ColorList colorEnum)
        {
            switch (colorEnum)
            {
                case ColorList.Black:
                    ColorNames = ["مشکی", "سیاه", "Black"];
                    ColorRGBs = ["rgb(33, 33, 33)", "rgb(26, 26, 26)"];
                    ColorHEXs = [""];
                    break;
                case ColorList.Pink:
                    ColorNames = ["صورتی", "Pink"];
                    ColorRGBs = ["rgb(224, 92, 224)", "rgb(255, 128, 171)"];
                    ColorHEXs = [""];
                    break;
                case ColorList.Blue:
                    ColorNames = ["ابی", "آبی", "Blue"];
                    ColorRGBs = ["rgb(0, 108, 240)", "rgb(33, 150, 243)"];
                    ColorHEXs = [""];
                    break;
                case ColorList.Green:
                    ColorNames = ["سبز", "Green"];
                    ColorRGBs = ["rgb(34, 161, 72)", "rgb(0, 126, 51)"];
                    ColorHEXs = [""];
                    break;
                case ColorList.Yellow:
                    ColorNames = ["زرد", "طلایی", "Yellow", "Gold"];
                    ColorRGBs = [""];
                    ColorHEXs = [""];
                    break;
                case ColorList.White:
                    ColorNames = ["سفید", "White"];
                    ColorRGBs = ["rgb(255, 255, 255)"];
                    ColorHEXs = [""];
                    break;
                default:
                    ColorNames = [""];
                    ColorRGBs = [""];
                    ColorHEXs = [""];
                    break;
            }
        }
        public string[] ColorNames { get; set; }
        public string[] ColorRGBs { get; set; }
        public string[] ColorHEXs { get; set; }
    }

}
