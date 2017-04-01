using NMagickWand;


namespace MawMvcApp.ViewModels.Tools
{
	public class BrowserHellModel
	{
		public string[,] GetColorArray(string imagePath)
		{
            using(var wand = new MagickWand(imagePath))
            using(var pit = new PixelIterator(wand))
            {
                string[,] imgColors = new string[wand.ImageWidth, wand.ImageHeight];

                for(int y = 0; y < wand.ImageHeight; y++)
		        {
                    var list = pit.GetNextIteratorRow();

                    for(int x = 0; x < wand.ImageWidth; x++)
                    {
                        imgColors[x,y] = list[x].HtmlColor;
                    }
                }

                return imgColors;
            }
		}
	}
}
