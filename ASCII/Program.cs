// See https://aka.ms/new-console-template for more information
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

char[] ascii_image_chars = { '@', '&', 'm', 'Q', 't', ']', '?', '~', '"', '^', '\'', '.' };

string camUrl = "http://192.168.0.19:8080";


using var httpClient = new HttpClient();

using var message = new HttpRequestMessage(HttpMethod.Get, camUrl);
//using var stream = await httpClient.GetStreamAsync(camUrl);
Console.WriteLine("TESt");


var tasks = new TaskCompletionSource();

var request = new HttpRequestMessage(HttpMethod.Get, camUrl);
var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
var stream = response.Content.ReadAsStream();
var contentType = response.Content.Headers.ContentType.Parameters.ElementAt(0).Value;
var boundary = HeaderUtilities.RemoveQuotes(contentType).Value;
int[] cColors = { 0x000000, 0x000080, 0x008000, 0x008080, 0x800000, 0x800080, 0x808000, 0xC0C0C0, 0x808080, 0x0000FF, 0x00FF00, 0x00FFFF, 0xFF0000, 0xFF00FF, 0xFFFF00, 0xFFFFFF };

for (var streamReader = new MultipartReader(boundary, stream); ;) {
    //Console.Clear();
    //await Task.Delay(100);

    MultipartSection nextFrame;
    Console.CursorLeft = 0;
    Console.CursorTop = 0;

    try {
        nextFrame = await streamReader.ReadNextSectionAsync();
    } catch (Exception) {
        continue;
    }

    if (nextFrame == null) break;

    using var sss = new MemoryStream();
    try {
        //Console.Clear();

        await nextFrame.Body.CopyToAsync(sss);
        using Bitmap image_bitmap = new(sss);
        ConsoleWriteImage(image_bitmap, cColors);
    } catch (Exception ex) {

    }
};

//Console.WriteLine("TESt");

await tasks.Task;

Bitmap ResizeImage(Bitmap image, int width, int height) {
    var destRect = new Rectangle(0, 0, width, height);
    var destImage = new Bitmap(width, height);

    destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

    using (var graphics = Graphics.FromImage(destImage)) {
        graphics.CompositingMode = CompositingMode.SourceCopy;
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

        using (var wrapMode = new ImageAttributes()) {
            wrapMode.SetWrapMode(WrapMode.TileFlipXY);
            graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
        }
    }

    return destImage;
}
static void ConsoleWritePixel(Color cValue, int[] cColors) {
    Color[] cTable = cColors.Select(x => Color.FromArgb(x)).ToArray();
    char[] rList = new char[] { (char)9617, (char)9618, (char)9619, (char)9608 }; // 1/4, 2/4, 3/4, 4/4
    int[] bestHit = new int[] { 0, 0, 4, int.MaxValue }; //ForeColor, BackColor, Symbol, Score

    for (int rChar = rList.Length; rChar > 0; rChar--) {
        for (int cFore = 0; cFore < cTable.Length; cFore++) {
            for (int cBack = 0; cBack < cTable.Length; cBack++) {
                int R = (cTable[cFore].R * rChar + cTable[cBack].R * (rList.Length - rChar)) / rList.Length;
                int G = (cTable[cFore].G * rChar + cTable[cBack].G * (rList.Length - rChar)) / rList.Length;
                int B = (cTable[cFore].B * rChar + cTable[cBack].B * (rList.Length - rChar)) / rList.Length;
                int iScore = (cValue.R - R) * (cValue.R - R) + (cValue.G - G) * (cValue.G - G) + (cValue.B - B) * (cValue.B - B);
                if (!(rChar > 1 && rChar < 4 && iScore > 50000)) // rule out too weird combinations
                {
                    if (iScore < bestHit[3]) {
                        bestHit[3] = iScore; //Score
                        bestHit[0] = cFore;  //ForeColor
                        bestHit[1] = cBack;  //BackColor
                        bestHit[2] = rChar;  //Symbol
                    }
                }
            }
        }
    }
    Console.ForegroundColor = (ConsoleColor)bestHit[0];
    Console.BackgroundColor = (ConsoleColor)bestHit[1];
    Console.Write(rList[bestHit[2] - 1]);
}

void ConsoleWriteImage(Bitmap source, int[] cColors) {
    int sMax = Console.WindowHeight - 2;
    decimal percent = Math.Min(decimal.Divide(sMax, source.Width), decimal.Divide(sMax, source.Height));
    Size dSize = new Size((int)(source.Width * percent), (int)(source.Height * percent));
    Bitmap bmpMax = new Bitmap(source, dSize.Width * 2, dSize.Height);
    for (int i = 0; i < dSize.Height; i++) {
        for (int j = 0; j < dSize.Width; j++) {
            ConsoleWritePixel(bmpMax.GetPixel(j * 2, i), cColors);
            ConsoleWritePixel(bmpMax.GetPixel(j * 2 + 1, i), cColors);
        }
        System.Console.WriteLine();
    }
    Console.ResetColor();
}

//

//continuasly stream from cam url 

//while (true) {
//    using (WebClient client = new WebClient()) {
//        string ascii_image = "";
//        string image = client.DownloadString(camUrl);
//        string[] image_rows = image.Split("\r");
//        foreach (string row in image_rows) {
//            foreach (char c in row) {
//                ascii_image += ascii_image_chars[(int)(c / 32)];
//            }
//            ascii_image += "\r";
//        }
//        Console.WriteLine(ascii_image);
//    }
//}